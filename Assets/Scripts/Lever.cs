using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lever : MonoBehaviour
{
    // ��� ������ �� �����, ������� ��������� ������ ����
    // �������� � ���������� (� ������� ��� �����)
    [SerializeField] private GameObject Door;
    public Sprite inactiveImage;

    // �������������� ������������:
    // ���� �� ���� 'Collectable'
    // ���������� �������������
    // ����� ����� ��������� Lever �������������� (�� ���� �������!)

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
        if (other.CompareTag("Player"))
        {
            Door.GetComponent<ExitDoor>().KeyCollected();
            //this.gameObject.SetActive(false);
            this.GetComponentInChildren<SpriteRenderer>().sprite = inactiveImage;
            this.enabled = false;
        }
    }
}
