using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour //TODO: do not spawn near zombies for now
{
    private Rigidbody2D rigidbody_;
    [SerializeField] private float initialSpeed = 1.0f; // Скорость передвижения
    [SerializeField] private float stelsSpeed = 0.5f;
    [SerializeField] private float runSpeed = 2.0f;
    [SerializeField] private float angularSpeed = 100.0f; // Скорость вращения игрока при нажатии кнопок.
    [SerializeField] private float maxStamina = 10.0f;
    [SerializeField] private float staminaRecoverySpeed = 0.01f;
    [SerializeField] private float staminaWasteSpeed = 0.1f;

    public bool isItStels = false;              // Кажется, что это будет нужно мобам

    private Vector3 startPosition; // Это точка воскрешения. В данный момент это стартовая позиция игрока

    private int healthPoints = 5;
    private float speedScaler = 1.0f;
    private float speed;
    private bool enabledRunning = true;
    [SerializeField] private float stamina = 0; // Выведен в инспектор на данный момент исключительно для отладки
                                                // (чтобы можно было понять на каком уровне выносливость сейчас)

    // for debugging
    SpriteRenderer spriteRenderer_;

    public List<Vector3> trace = new List<Vector3>(); // for mob to avoid obstacles

    void Start()
    {
        trace.Add(transform.position);
        rigidbody_ = GetComponentInParent<Rigidbody2D>();
        startPosition = rigidbody_.position;
        speed = initialSpeed;
        enabledRunning = true;
        stamina = maxStamina;

        // For debugging
        spriteRenderer_ = GetComponentInParent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (Input.GetButton("Stels"))
        {
            // Debug.Log("I'm in stels!");
            isItStels = true;
            speed = stelsSpeed;

            stamina += staminaRecoverySpeed;
            if (stamina > maxStamina)
            {
                stamina = maxStamina;
            }
            spriteRenderer_.color = Color.yellow;
        } else if (Input.GetButton("Run") && enabledRunning && (stamina > staminaWasteSpeed))
        {
            // Debug.Log("Run!");
            speed = runSpeed;
            isItStels = false;
            stamina -= staminaWasteSpeed;
            spriteRenderer_.color = Color.green;
        } else
        {
            // Debug.Log("Walk");
            speed = initialSpeed;
            isItStels = false;

            stamina += staminaRecoverySpeed;
            if (stamina > maxStamina)
            {
                stamina = maxStamina;
            }
            spriteRenderer_.color = Color.white;
        }
        
        // Это передвижение
        trace.Add(transform.position);
        Vector2 pressing = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        pressing.Normalize(); // А то по диагонали будет выгоднее двигаться(
        rigidbody_.velocity = pressing * speed * speedScaler;

        // Это вращение по q/e
        rigidbody_.angularVelocity = Input.GetAxis("Rotate") * angularSpeed;
    }

    public void Restart() // and reset
    {
        Debug.Log("Resetting player settings to default");
        rigidbody_.position = startPosition;
        speedScaler = 1.0f; // Если нужно для оптимизации (потому что изменение множителя происходит явно реже, чем смена режима),
                            // то могу просто сделать набор скоростей и менять его
        enabledRunning = true;
        stamina = maxStamina;
        // !! NEED TO RETURN LIGHT RADIUS TO DEFAULT
    }

    public void HeroDamaged(int damageCount)
    {
        healthPoints -= damageCount;
        if (healthPoints == 2)
        {
            speedScaler = 2.0f / 3.0f;
        } else if (healthPoints == 1)
        {
            // !! NEED TO CHANGE LIGHT RADIUS HERE!
            enabledRunning = false;
            speedScaler = 0.5f;
        } else if (healthPoints <= 0)
        {
            Debug.Log("I died!");
            Restart();
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