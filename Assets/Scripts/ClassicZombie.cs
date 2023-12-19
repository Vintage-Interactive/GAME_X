using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class ClassicZombie : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private int current_pos_in_trace = -1; // has to be visible position!

    public LayerMask wallLayer;
    public LayerMask playerLayer;
    public float impulseMagnitude = 10f;
    public float wallHeroCoeff = 0.5f;
    public bool angry = false;
    public float speed = 1.0f;
    public float agroRadius = 3.0f; // has to be equal to the visible radius of the player, but no such a field in players
    public float damageRadius = 0.9f;
    public int hp = 2;
    public int damage = 1;
    public Player player_target;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() 
    {
        Transform target = player_target.transform;
        Assert.IsNotNull(target);
        float distance = Vector2.Distance(target.position, transform.position);
        if (!angry)
        {
            if (distance < agroRadius)
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
            Vector2 currentVelocity = rb.velocity;

            // Calculate the impulse in the opposite direction
            Vector2 impulse = -currentVelocity.normalized * impulseMagnitude;

            // Apply the impulse to the rigid body, or teleport and stun?
            rb.AddForce(impulse, ForceMode2D.Impulse);
            return;
            // some animation of pushing zombie back
            // TODO: give an impulse for mob after biting
        }

        if (CanSee(target))
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.fixedDeltaTime);
            return;
        }
        Vector2 targetPosition = player_target.trace[current_pos_in_trace];
        MoveToPositionInTrace(targetPosition);
    }
    public bool CanSee(Transform target)
    {
        float maxDistance = Vector2.Distance(transform.position, target.position);
        var hit = Physics2D.Raycast(transform.position, target.position - transform.position, maxDistance, wallLayer | playerLayer);
        if (1 << hit.collider.gameObject.layer != playerLayer)
        {
            return false;
        }
        // Assert.AreEqual(hit.collider.transform, target); // does not pass when i hit player: one collider moves, other does not?
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

    public int FindClosestPos(int prev_pos)
    {
        int max_pos = player_target.trace.Count - 1;
        float min_dist = Mathf.Infinity;
        int min_idx = prev_pos;
        for (int i = 0; i < max_pos - prev_pos; i += 10)
        {
            Vector2 cur_pos = player_target.trace[prev_pos + i];
            float distance = Vector2.Distance(cur_pos, transform.position);
            if (distance < min_dist)
            {
                min_dist = distance;
                min_idx = prev_pos + i;
            }
        }
        Assert.AreNotEqual(min_idx, -1);
        return min_idx;
    }
    public void MoveToPositionInTrace(Vector2 targetPos)
    {
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
}
