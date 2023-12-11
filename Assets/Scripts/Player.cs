using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rigidbody_;
    public float speed = 1.0f; // Скорость передвижения
    [SerializeField] private float angularSpeed = 100.0f; // Скорость вращения игрока при нажатии кнопок.

    private int healthPoints = 3;

    void Start()
    {
        rigidbody_ = GetComponentInParent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Это передвижение
        Vector2 pressing = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        pressing.Normalize(); // А то по диагонали будет выгоднее двигаться(
        rigidbody_.velocity = pressing * speed;

        // Это вращение по q/e
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