using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour //TODO: do not spawn near zombies for now
{
    private Rigidbody2D rigidbody_;
    [SerializeField] private float initialSpeed = 1.0f; // Movement speed
    [SerializeField] private float stelsSpeed = 0.5f;
    [SerializeField] private float runSpeed = 2.0f;
    [SerializeField] private float angularSpeed = 100.0f; // The player's rotation speed when pressing buttons.
    [SerializeField] private float maxStamina = 10.0f;
    [SerializeField] private float staminaRecoverySpeed = 0.01f;
    [SerializeField] private float staminaWasteSpeed = 0.1f;
    [SerializeField] private LayerMask trapLayer = 1 << 10;

    public bool isItStels = false;              // Maybe it will be useful for mobs

    private Vector3 respawnPosition; // Future respawn point

    private int healthPoints = 5;
    private float speedScaler = 1.0f;
    private float speed;
    private bool enabledRunning = true;
    private bool isBlocked = false;
    private Vector2 blockedPos;
    [SerializeField] private float stamina = 0; // Displayed in the inspector at the moment only for debugging
                                                // (so you can understand what level your stamina is now)

    // for debugging
    SpriteRenderer spriteRenderer_;

    public List<Vector3> trace = new List<Vector3>(); // for mob to avoid obstacles

    void Start()
    {
        trace.Add(transform.position);
        rigidbody_ = GetComponentInParent<Rigidbody2D>();
        respawnPosition = rigidbody_.position;
        speed = initialSpeed;
        enabledRunning = true;
        stamina = maxStamina;

        // For debugging
        spriteRenderer_ = GetComponentInParent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (isBlocked) {
            transform.position = blockedPos;
            rigidbody_.velocity = Vector2.zero;
            Debug.Log("blocked POs is " + blockedPos[0] + " " + blockedPos[1]);
            return;
        }
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
        
        // ��� ������������
        if (trace[trace.Count - 1] != transform.position)
        {
            trace.Add(transform.position);
        }
        Vector2 pressing = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        pressing.Normalize(); // � �� �� ��������� ����� �������� ���������(
        passTheTrap(getCurrentTrap());
        rigidbody_.velocity = pressing * speed * speedScaler;

        // ��� �������� �� q/e
        rigidbody_.angularVelocity = Input.GetAxis("Rotate") * angularSpeed;
    }

    public void Restart() // and reset
    {
        Debug.Log("Resetting player settings to default");
        // rigidbody_.position = startPosition;
        speedScaler = 1.0f; // If necessary for optimization (because changing the multiplier occurs clearly less often than changing the mode),
                            // then I can just make a set of speeds and change it
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
            // TODO: delete the following line
            // Destroy(gameObject);
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

    public GameObject getCurrentTrap() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.01f, trapLayer);
        // Debug.Log("Length " + colliders.Length);
        if (colliders.Length > 0) {
            return colliders[0].gameObject;
        }
        return null;
    }

    IEnumerator BlockMovementForDuration(float blockDuration)
    {
        isBlocked = true;
        blockedPos = transform.position;

        // rigidbody_.velocity = Vector2.zero; 
        // rigidbody_.angularVelocity = 0;
        // rigidbody_.isKinematic = true;

        Debug.Log("Movement Blocked! BStatic now");

        yield return new WaitForSeconds(blockDuration);

        // rigidbody_.isKinematic = false;

        isBlocked = false;

        Debug.Log("Movement Unblocked!");
    }
    public void passTheTrap(GameObject trap) {
        Vector2 respPos = new Vector2(-100, -100);
        if (trap is null) {
            return;
        }
        String trap_tag = trap.tag;
        if (string.Equals(trap_tag, "Sand")) {
            Debug.Log("Sand");
            return;
        }
        if (string.Equals(trap_tag, "Barbwire")) {
            Debug.Log("Barbwire");
            HeroDamaged(1);
            trap.transform.position = respPos;
            return;
        }
        if (string.Equals(trap_tag, "Swamp")) {
            speed *= 0.3f;
            Debug.Log("Swamp");
        }
        if (string.Equals(trap_tag, "CreakyFloor")) {
            isItStels = false;
            Debug.Log("CreakyFloor");
        }
        if (string.Equals(trap_tag, "ManTrap")) {
            StartCoroutine(BlockMovementForDuration(3f));
            trap.transform.position = respPos;
        }
    }
}