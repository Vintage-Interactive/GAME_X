using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour //TODO: do not spawn near zombies for now
{
    Transform cameraTrans_;
    public bool IsWalking;
    
    public Animator animations;
    private Rigidbody2D rigidbody_;
    [SerializeField] private float initialSpeed = 1.0f; // Movement speed
    [SerializeField] private float stelsSpeed = 0.5f;
    [SerializeField] private float runSpeed = 2.0f;
    [SerializeField] private float angularSpeed = 100.0f; // The player's rotation speed when pressing buttons.
    [SerializeField] private float maxStamina = 10.0f;
    [SerializeField] private float staminaRecoverySpeed = 0.01f;
    [SerializeField] private float staminaWasteSpeed = 0.1f;
    [SerializeField] private LayerMask trapLayer = 1 << 10;
    [SerializeField] private LayerMask mobLayer = 1 << 9;

    public bool inStealth = false;              // Maybe it will be useful for mobs

    private Vector3 respawnPosition; // Future respawn point

    private int healthPoints = 5;
    private float speedScaler = 1.0f;
    private float speed;
    private bool enabledRunning = true;
    private bool isBlocked = false;
    private Vector2 blockedPos;

    [SerializeField] private Light lightForPlayer;
    [SerializeField] private float lightDecreasingAtOneHeart = 0.5f;
    private float defaultLightRadius;

    [SerializeField] private float stamina = 0; // Displayed in the inspector at the moment only for debugging
                                                // (so you can understand what level your stamina is now)

    enum WeaponState { Dagger = 0, Shotgun = 1 };
    [SerializeField] private WeaponState currentWeapon = WeaponState.Dagger;
    float coolDownBetweenChanging = 0.25f;
    float lastTimeToWaitForChanging = 0.0f;

    float coolDownBetweenAttack = 0.25f;
    float lastTimeToWaitForAttack = 0.0f;

    float daggerAttackRadius = 1.0f;
    int attackDamage = 1;

    public int patronsCount = 10;  // Count of possible shots without special items

    
    

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
        
        cameraTrans_ = Camera.main.transform;
        defaultLightRadius = lightForPlayer.intensity;
    }

    // TODO: need to move from FixedUpdate to Update rotation and changing weapon
    // remove float time in fixed update. Use 'int' instead.
    // or just move it to update and use GetButtonDown instead of GetButton - time will be useless

    void FixedUpdate()
    {
        if (lastTimeToWaitForChanging <= 0.0f && Input.GetButton("Change_weapon"))
        {
            ChangeWeapon();
            lastTimeToWaitForChanging = coolDownBetweenChanging;
        } else if (lastTimeToWaitForChanging > 0.0f)
        {
            lastTimeToWaitForChanging -= Time.deltaTime;
        }

        if (lastTimeToWaitForAttack <= 0.0f && Input.GetButton("Attack"))
        {
            if (currentWeapon == WeaponState.Dagger)
            {
                animations.Play("Attack");
                Attack(FindNearestMonster(daggerAttackRadius), attackDamage);
                lastTimeToWaitForAttack = coolDownBetweenAttack;
                
            } else if (currentWeapon == WeaponState.Shotgun)
            {
                Transform enemy = FindNearestMonster(defaultLightRadius);
                if (enemy != null)
                {
                    patronsCount -= 1;
                    animations.Play("Shoot");
                    
                    Attack(enemy, attackDamage);
                    lastTimeToWaitForAttack = coolDownBetweenAttack;

                    if (patronsCount == 0)
                    {
                        ChangeWeapon();
                        lastTimeToWaitForChanging = coolDownBetweenChanging;
                    }
                }
            }
        } else if (lastTimeToWaitForAttack > 0.0f)
        {
            lastTimeToWaitForAttack -= Time.deltaTime;
        }

        if (isBlocked) {
            transform.position = blockedPos;
            rigidbody_.velocity = Vector2.zero;
            Debug.Log("blocked POs is " + blockedPos[0] + " " + blockedPos[1]);
            return;
        }
        if (Input.GetButton("Stels"))
        {
            // Debug.Log("I'm in stels!");
            inStealth = true;
            animations.SetBool("isSneaking", inStealth);
            animations.SetBool("IsWalking", IsWalking);
            animations.SetBool("isRunning", false);
            speed = stelsSpeed;

            stamina += staminaRecoverySpeed;
            if (stamina > maxStamina)
            {
                stamina = maxStamina;
            }
        } else if (Input.GetButton("Run") && enabledRunning && (stamina > staminaWasteSpeed))
        {
            // Debug.Log("Run!");
            inStealth = false;
            animations.SetBool("isSneaking", inStealth);
            animations.SetBool("IsWalking", IsWalking);
            animations.SetBool("isRunning", true);
            speed = runSpeed;
            stamina -= staminaWasteSpeed;
        } else
        {
            // Debug.Log("Walk");
            animations.SetBool("isSneaking", false);
            animations.SetBool("IsWalking", IsWalking);
            animations.SetBool("isRunning", false);
            speed = initialSpeed;
            inStealth = false;

            stamina += staminaRecoverySpeed;
            if (stamina > maxStamina)
            {
                stamina = maxStamina;
            }
            
        }
        
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            if (!IsWalking)
            {
                IsWalking = true;
                animations.SetBool("IsWalking", IsWalking);
            }
        }
        else
        {
            if (IsWalking)
            {
                IsWalking = false;
                animations.SetBool("IsWalking", IsWalking);
            }
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

        cameraTrans_.position = new Vector3(rigidbody_.position.x, rigidbody_.position.y, cameraTrans_.position.z);
    }

    public void Restart() // and reset
    {
        Debug.Log("Resetting player settings to default");
        rigidbody_.position = respawnPosition;
        speedScaler = 1.0f; // If necessary for optimization (because changing the multiplier occurs clearly less often than changing the mode),
                            // then I can just make a set of speeds and change it
        enabledRunning = true;
        stamina = maxStamina;

        lightForPlayer.intensity = defaultLightRadius;
    }

    public void HeroDamaged(int damageCount)
    {
        healthPoints -= damageCount;
        if (healthPoints == 2)
        {
            speedScaler = 2.0f / 3.0f;
        } else if (healthPoints == 1)
        {
            lightForPlayer.intensity *= lightDecreasingAtOneHeart;
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

    public GameObject getCurrentTrap() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.01f, trapLayer);
        if (colliders.Length > 0) {
            return colliders[0].gameObject;
        }
        return null;
    }

    IEnumerator BlockMovementForDuration(float blockDuration)
    {
        isBlocked = true;
        blockedPos = transform.position;

        Debug.Log("Movement Blocked! BStatic now");

        yield return new WaitForSeconds(blockDuration);

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
            Debug.Log("Barbwire, ouch!");
            HeroDamaged(1);
            trap.transform.position = respPos;
            return;
        }
        if (string.Equals(trap_tag, "Swamp")) {
            speed *= 0.3f;
            Debug.Log("Swamp");
        }
        if (string.Equals(trap_tag, "CreakyFloor")) {
            inStealth = false;
            Debug.Log("CreakyFloor");
        }
        if (string.Equals(trap_tag, "ManTrap")) {
            StartCoroutine(BlockMovementForDuration(3f));
            trap.transform.position = respPos;
        }
    }

    private void ChangeWeapon()
    {
        //Debug.Log("Change");
        if (currentWeapon == WeaponState.Dagger && patronsCount > 0)
        {
            currentWeapon = WeaponState.Shotgun;
        } else if (currentWeapon == WeaponState.Shotgun)
        {
            currentWeapon = WeaponState.Dagger;
        }
    }

    Transform FindNearestMonster(float radiusOfSearch)
    {
        Collider2D[] monsters = Physics2D.OverlapCircleAll(rigidbody_.position, radiusOfSearch, mobLayer);

        Transform closestMonster = null;
        float closestDistanceSqr = Mathf.Infinity; // Используем квадрат расстояния

        foreach (Collider2D monster in monsters)
        {
            Transform curMonster = monster.transform;
            Vector2 pos = curMonster.position;
            float distanceToMonsterSqr = (rigidbody_.position - pos).sqrMagnitude;

            if (distanceToMonsterSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceToMonsterSqr;
                closestMonster = curMonster;
            }
        }

        return closestMonster;
    }

    void Attack(Transform enemy, int damageCount)
    {
        if (enemy is null)
        {
            return;
        }

        ClassicZombie simpleEnemy = enemy.GetComponent<ClassicZombie>();
        if (simpleEnemy != null)
        {
            simpleEnemy.damageMob(damageCount);
            Debug.Log("Left: (hp)");
            Debug.Log(simpleEnemy.hp);
        }

        Cockroach cockroachEnemy = enemy.GetComponent<Cockroach>();
        if (cockroachEnemy != null)
        {
            cockroachEnemy.damageMob(damageCount);
            Debug.Log("Left: (hp)");
            Debug.Log(cockroachEnemy.hp);
        }

        MadScientist scientEnemy = enemy.GetComponent<MadScientist>();
        if (scientEnemy != null)
        {
            Debug.Log("Impossible to hurt mad scientist!");
        }

        Hunter hunterEnemy = enemy.GetComponent<Hunter>();
        if (hunterEnemy != null)
        {
            hunterEnemy.damageMob(damageCount);
            Debug.Log("Left: (hp)");
            Debug.Log(hunterEnemy.hp);
        }
    }
}