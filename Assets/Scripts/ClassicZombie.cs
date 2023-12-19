using System;
using System.Collections;
using System.Collections.Generic;
// using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

// TODO: different mobs, stealth
public class ClassicZombie : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private int current_pos_in_trace = -1; // has to be visible position!
    private bool isBlocked = false;
    private Vector2 lastPos;
    private Vector2 respPos = new Vector2(-100, -100);

    public LayerMask wallLayer;
    public LayerMask playerLayer;
    public float impulseMagnitude = 10f;
    public float wallHeroCoeff = 0.5f;
    public bool angry = false;
    public float speed = 1.0f;
    public float chasingRadius = 6.0f;
    public float agroRadius = 3.0f; // has to be equal to the visible radius of the player, but no such a field in players
    public float stealthAgroRaius = 2.5f;
    public float damageRadius = 0.9f;
    public int hp = 2;
    public int damage = 1;
    public Player player_target;

    void Start()
    {
        lastPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() 
    {
        if (isBlocked) {
            return;
        }
        Assert.AreEqual(rb.isKinematic, false);
        Transform target = player_target.transform;
        Assert.IsNotNull(target);
        float distance = Vector2.Distance(target.position, transform.position);
        float realRadius = agroRadius;
        if (player_target.isItStels) {
            realRadius = stealthAgroRaius;
        }
        if (!angry)
        {
            if (distance < realRadius)
            {
                angry = true;
                if (player_target.trace.Count < 1)
                {
                    Debug.Log("Bad trace " + player_target.trace.Count);
                }
                current_pos_in_trace = player_target.trace.Count - 1;
            }
            return; // they have some delay, their reaction speed
        }
        Vector2 direction = (target.position - transform.position).normalized;
        spriteRenderer.flipX = (direction.x < 0);

        if (distance < damageRadius)
        {
            Debug.Log("Damaging good guy!");
            player_target.HeroDamaged(damage);
            StartCoroutine(BlockMovementForDuration(2f)); 
            return;
            // some animation of zombie
        }

        if (CanSee(target))
        {
            lastPos = transform.position;
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.fixedDeltaTime);
            return;
        }
        Vector2 targetPosition = player_target.trace[current_pos_in_trace];
        if (chasingRadius >= Vector2.Distance(targetPosition, transform.position)) {
            MoveToPositionInTrace(targetPosition);
        }
    }

    public bool CanSee(Transform target)
    {
        float maxDistance = Math.Min(Vector2.Distance(transform.position, target.position), chasingRadius);
        var hit = Physics2D.Raycast(transform.position, target.position - transform.position, maxDistance, wallLayer | playerLayer);
        if (hit.collider == null) {
            Debug.Log("Do not see");
            return false;
        }
        if (1 << hit.collider.gameObject.layer != playerLayer)
        {
            return false;
        }
        // Assert.AreEqual(hit.collider.transform, target); // does not pass when i hit player: one collider turns, other does not?
        Collider2D playerCollider = player_target.GetComponent<Collider2D>();
        Vector2[] playerCorners = GetColliderEdges(playerCollider, 1.415f);
        Vector2 zombie_pos = transform.position;
        for (int i = 0; i < playerCorners.Length; i++) 
        {
            var hit_corner = Physics2D.Raycast(zombie_pos, playerCorners[i] - zombie_pos,
                maxDistance, wallLayer);
            Debug.DrawRay(zombie_pos, playerCorners[i] - zombie_pos);
            if (hit_corner.collider != null) {
                return false;
            }
        }
        current_pos_in_trace = player_target.trace.Count - 1;
        return true;
    }
    public void MoveToPositionInTrace(Vector2 targetPos)
    {
        lastPos = transform.position;
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.fixedDeltaTime);
        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            current_pos_in_trace++;
        }
    }
    Vector2[] GetColliderEdges(Collider2D collider, float coef)
    {
        Vector2[] edges = new Vector2[4];
        Vector2 center = collider.bounds.center;
        Vector2 extents = collider.bounds.extents;
        edges[0] = center + coef * new Vector2(extents.x, 0);
        edges[1] = center + coef * new Vector2(0, -extents.y);
        edges[2] = center + coef * new Vector2(-extents.x, 0);
        edges[3] = center + coef * new Vector2(0, extents.y);
        return edges;
    }

    IEnumerator BlockMovementForDuration(float blockDuration)
    {
        isBlocked = true;

        Debug.Log("Movement Blocked!");

        rb.isKinematic = true; // Set Rigidbody to static

        yield return new WaitForSeconds(blockDuration);

        rb.isKinematic = false;

        isBlocked = false;

        Debug.Log("Movement Unblocked!");
    }
    public void damageMob(int damage) {
        hp -= damage;
        StartCoroutine(BlockMovementForDuration(1f));
        if (hp <= 0) {
            Die();
        }
    }

    private void Die() {
        transform.position = respPos;
    }
}
