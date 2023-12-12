using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour //TODO: do not spawn near zombies for now
{
    private Rigidbody2D rigidbody_;
    public float speed = 1.0f; // �������� ������������
    [SerializeField] private float angularSpeed = 100.0f; // �������� �������� ������ ��� ������� ������.

    private int healthPoints = 3;
    public List<Vector3> trace = new List<Vector3>(); // for mob to avoid obstacles


    void Start()
    {
        trace.Add(transform.position);
        rigidbody_ = GetComponentInParent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // ��� ������������
        trace.Add(transform.position);
        Vector2 pressing = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        pressing.Normalize(); // � �� �� ��������� ����� �������� ���������(
        rigidbody_.velocity = pressing * speed;

        // ��� �������� �� q/e
        rigidbody_.angularVelocity = Input.GetAxis("Rotate") * angularSpeed;
    }

    public void Die() // and respawn maybe...
    {
        healthPoints = 0;
    }

    public void HeroDamaged(int damageCount)
    {
        healthPoints -= damageCount;
        if (healthPoints < 0)
        {
            Debug.Log("I died!");
            Die();
        }
    }

    public void HealHero(int healCount)
    {
        healthPoints += healCount;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}