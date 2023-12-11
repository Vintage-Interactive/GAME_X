using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Key : MonoBehaviour
{
    // ��� ������ �� �����, ������� ��������� ������ ����
    // �������� � ���������� (� ������� ��� �����)
    [SerializeField] private GameObject Door;

    // �������������� ������������:
    // ���� �� ���� 'Collectable'
    // ���������� �������������
    // ����� ����� �������������� (�������� �� ����������)

    // ���� ����� ������� �� ������� ������, �� ��� �� ��������:
    // ����� ������� ������ � ������� �������: ���������� IsPlayerHere � Update ��� ����� �������

    void Start()
    {
        if (Door == null)
        {
            Debug.Log("Key: I don't have door!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Door.GetComponent<Door>().KeyCollected();
        this.gameObject.SetActive(false);
    }
}
