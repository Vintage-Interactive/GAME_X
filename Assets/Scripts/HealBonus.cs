using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealBonus : MonoBehaviour
{
    // ��� ������ �� �����, ������� ��������� ������ ����
    // �������� � ���������� (� ������� ��� �����)
    [SerializeField] private GameObject mainCharacter_;
    [SerializeField] private int value = 1;

    // �������������� ������������:
    // ���� �� ���� 'Collectable'
    // ���������� �������������
    // ����� ����� �������������� (�������� �� ����������)

    // ���� ����� ������� �� ������� ������, �� ��� �� ��������:
    // ����� ������� ������ � ������� �������: ���������� IsPlayerHere � Update ��� ����� �������

    void Start()
    {
        if (mainCharacter_ == null)
        {
            Debug.Log("Ammo bonus: I don't have hero!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            mainCharacter_.GetComponent<Player>().HealHero(value);
            this.gameObject.SetActive(false);
        }
    }
}
