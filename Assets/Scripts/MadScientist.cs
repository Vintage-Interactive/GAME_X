using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MadScientist : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isBlocked = false;
    private Vector2 lastPos;
    private Vector2 respPos = new Vector2(-100, -100);
    private NavMeshAgent agent;

    public LayerMask wallLayer;
    public LayerMask playerLayer;
    public bool angry = false;
    public float speed = 1.0f;
    public float chasingRadius = 6.0f;
    public float agroRadius = 3.0f; // has to be equal to the visible radius of the player, but no such a field in players
    public float stealthAgroRaius = 2.5f;
    public float damageRadius = 1f;
    public int hp = 2;
    public int damage = 1;
    public Transform door_target;
    public Player player_target;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speed;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        lastPos = transform.position;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isBlocked) {
            transform.position = lastPos;
            return;
        }
        Transform target = player_target.transform;
        float distance = Vector2.Distance(target.position, transform.position);
        float realRadius = agroRadius;
        if (player_target.inStealth) {
            realRadius = stealthAgroRaius;
        }
        if (!angry)
        {
            tryToGetAngry(distance, realRadius);
            return; // they have some delay, their reaction speed
        }
        Vector2 direction = (target.position - transform.position).normalized;
        spriteRenderer.flipX = (direction.x < 0);

        tryToDamage(distance);
        agent.SetDestination(door_target.position);
    }

    private void tryToGetAngry(float distance, float realRadius) {
        if (distance < realRadius)
        {
            angry = true;
        }
    }

    public void tryToDamage(float distance) {
        if (distance < damageRadius)
        {
            anim.SetTrigger("ToHit");
            Debug.Log("Damaging good guy!");
            player_target.HeroDamaged(damage);
            StartCoroutine(BlockMovementForDuration(2f)); 
            return;
            // some animation of zombie
        }
    }
    private void rotateTowardsPlayer() {
        Vector3 our_pos = player_target.transform.position - transform.position;
        our_pos.z = 0;
        transform.right = our_pos;
    }

    IEnumerator BlockMovementForDuration(float blockDuration)
    {
        isBlocked = true;

        rb.velocity = Vector2.zero; 
        rb.angularVelocity = 0;
        rb.isKinematic = true;
        lastPos = transform.position;

        Debug.Log("Movement Blocked! Static now");

        yield return new WaitForSeconds(blockDuration);

        rb.isKinematic = false;

        isBlocked = false;

        Debug.Log("Movement Unblocked!");
    }
}
