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
    private int current_pos_in_trace = -1; // has to be visible position!

    public LayerMask wallLayer;
    public LayerMask playerLayer;
    public float wallHeroCoeff = 0.5f;
    public bool angry = false;
    public float speed = 1.0f;
    public float agroRadius = 10.0f;
    public float damageRadius = 1.0f;
    public int hp = 2;
    public int damage = 1;
    public Player player_target;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate() 
    {
        Transform target = player_target.transform;
        Assert.IsNotNull(target);
        float distance = Vector3.Distance(target.position, transform.position);
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
        Vector3 direction = (target.position - transform.position).normalized;
        spriteRenderer.flipX = (direction.x < 0);

        if (distance < damageRadius)
        {
            Debug.Log("Damaging good guy!");
            player_target.HeroDamaged(damage);
            return;
            // some animation of pushing zombie back
            // TODO: give an impulse for mob after biting
        }

        if (CanSee(target))
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.fixedDeltaTime);
            return;
        }
        Vector3 targetPosition = player_target.trace[current_pos_in_trace];
        MoveToPositionInTrace(targetPosition);

        // or, if cannot see the character, follow his smell, his steps - LAST IDEA, SEEMS THE BEST(were a lot of difficult
        // ideas to implement, this is a masterpiece, trust me:) )
    }
    public bool CanSee(Transform target)
    {
        float maxDistance = Vector3.Distance(transform.position, target.position);
        var hit = Physics2D.Raycast(transform.position, target.position - transform.position, maxDistance, wallLayer | playerLayer);
        if (1 << hit.collider.gameObject.layer != playerLayer)
        {
            return false;
        }
        Debug.Log("Target " + target); // target is logical, hit.collider is player
        Assert.AreEqual(hit.collider.transform, target);
        // Collider2D zombieCollider = GetComponent<Collider2D>();
        Collider2D playerCollider = player_target.GetComponent<Collider2D>();
        // Vector2[] zombieCorners = GetColliderEdges(zombieCollider);
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
        // go with rays that are outside! for example, from the center of the zombie to raycast rays to four directions in
        // double distance from the center of the hero. if no walls, then we see.
        // for (int i = 0; i < zombieCorners.Length; i++)
        // {
        //     var hit_corner = Physics2D.Raycast(zombieCorners[i], playerCorners[i] - zombieCorners[i],
        //         Mathf.Infinity, wallLayer | playerLayer);
        //     Debug.Log("Zombie corners " + zombieCorners[0] + " " + zombieCorners[1] + " " + zombieCorners[2] + " " + zombieCorners[3]);
        //     Debug.DrawRay(zombieCorners[i], playerCorners[i] - zombieCorners[i]);
        //     if (1 << hit_corner.collider.gameObject.layer != playerLayer)
        //     {
        //         return false; 
        //     }
        // }
        current_pos_in_trace = player_target.trace.Count - 1; // if found //TODO: this
        // here to do bin search to find the closest position from last before the current_pos_in_trace?
        //current_pos_in_trace = FindClosestPos(current_pos_in_trace);
        //Debug.Log("Closest is " + player_target.trace[current_pos_in_trace]);
        return true;
    }

    public int FindClosestPos(int prev_pos)
    {
        int max_pos = player_target.trace.Count - 1;
        float min_dist = Mathf.Infinity;
        int min_idx = prev_pos;
        for (int i = 0; i < max_pos - prev_pos; i += 10)
        {
            Vector3 cur_pos = player_target.trace[prev_pos + i];
            float distance = Vector3.Distance(cur_pos, transform.position);
            if (distance < min_dist)
            {
                min_dist = distance;
                min_idx = prev_pos + i;
            }
        }
        Assert.AreNotEqual(min_idx, -1);
        return min_idx;
    }
    public void MoveToPositionInTrace(Vector3 targetPos)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.fixedDeltaTime);
        //Debug.Log("Actually Moving to " + targetPos);
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            //Debug.Log("Achieved position " + current_pos_in_trace);
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

        //Vector3[] localVertices = new Vector3[]
        //{
        //    new Vector3(extents.x, extents.y, 0),
        //    new Vector3(-extents.x, extents.y, 0),
        //    new Vector3(-extents.x, -extents.y, 0),
        //    new Vector3(extents.x, -extents.y, 0),
        //};

        //collider.transform.TransformPoints(localVertices);
        ////// Transform local coordinates to world coordinates, taking rotation into account
        //Vector2[] worldVertices = new Vector2[localVertices.Length];
        //for (int i = 0; i < localVertices.Length; i++)
        //{
        //    worldVertices[i] = (Vector2)localVertices[i];
        //}

        //return worldVertices;
        return edges;
    }
}
