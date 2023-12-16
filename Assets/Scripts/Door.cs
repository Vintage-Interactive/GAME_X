using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    enum StateOfDoor { Wait = 0, Openning = 1, Closing = 2 };

    private StateOfDoor currentState = StateOfDoor.Wait;
    private float progress = 0.0f; // ��� ������� ���������� �����, �� ����

    private Vector3 startPosition; // �������, ����� ����� �������
    private Vector3 endPosition;   // �������, ����� ����� �������
    Transform transform_;          // ������ transform ����� (������ ��� �����, �� ������� ������ �����)

    // ��������� ����, ���� �� � ��� ����.
    // � ������� ������ �� ����� true (�������� ����� � ����������)
    // � ������ � ������ ���������� ����� false (�.�. �������� � ���������� ����� �� �������)
    [SerializeField] private bool isKeyCollected = false;

    // ����������: ����� �� ��� ����������� �� ������� � ���������� ��� ����� ���� ������� ������?
    bool isHerePlayer = false;

    // �������������� ����������� ��� �����:
    // � ��� ����� �����, �� ������� ����� ������ ����� ��� 'Door'. ��� ����� ���� ����� �� ����� �����������.
    // ���� ����� ��������� �� layout 'Door'
    // ���������� ����� ���������� ������������� ����� ��������� ���� �������������� ������ � ������

    // �������������� ����� �������� ������ � ��������� ���� 'Player'.

    // ���� ������� ����� ���, �� ����� ����� ������� �������������� ���: ����� ����� �� ����������� �� ������,
    // � ������� ��� ����������������� �� ����� (� Update ��������� ������ ��� ������� ������)
    // � �������� onTriggerExit2D � onTriggerEnter2D

    public void KeyCollected()
    {
        Debug.Log("My key collected!");
        isKeyCollected = true;
    }

    private void Start()
    {
        transform_ = gameObject.transform.GetChild(0);
        startPosition = transform_.position;
        endPosition = transform_.position + transform_.up;
    }

    private void Open()
    {
        // Debug.Log("Open");
        if (isKeyCollected)
        {
            currentState = StateOfDoor.Openning;
        }
    }

    private void Close()
    {
        // Debug.Log("Close");
        currentState = StateOfDoor.Closing;
    }

    private void Update()
    {
        if (isHerePlayer && Input.GetButtonDown("Open_door"))
        {
            Open();
        }
    }

    void FixedUpdate()
    {
        // ��������� ��������� �����
        if (currentState == StateOfDoor.Openning)
        {
            // � "�������������" ��������� ����� ��������� ������� ��������� (������� ���������� �����) � �������� �����
            // ���� ������ �������� �� �����, �� ��� � ��������� "�����'
            progress += Time.deltaTime;
            if (progress >= 1.0f) {
                transform_.position = endPosition;
                currentState = StateOfDoor.Wait;
            } else
            {
                transform_.position = Vector3.Lerp(startPosition, endPosition, progress);
            }
        } else if (currentState == StateOfDoor.Closing)
        {
            // � "�������������" ��������� ����� ��������� ������� ��������� (������� ���������� �����) � �������� �����
            // ���� ������ �������� �� �����, �� ��� � ��������� "�����'
            progress -= Time.deltaTime;
            if (progress <= 0.0f)
            {
                transform_.position = startPosition;
                currentState = StateOfDoor.Wait;
            }
            else
            {
                transform_.position = Vector3.Lerp(startPosition, endPosition, progress);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ����������������� � ������� ����� ������ ������, ������� ����� �� ������ �������� ����/����
        if (collision.CompareTag("Player"))
        {
            isHerePlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // ����� ���� ������ ����, ������� �� ��������� �� ������� ������� ������

        // ����� - ������ ��� ������������ ������ �������� � ������ "������� ����� - ���� �������������� ������"
        if (collision.CompareTag("Player"))
        {
            isHerePlayer = false;
            Close();
        }
    }
}
