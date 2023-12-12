using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using TMPro;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.Assertions;

public class ClassicZombie : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private int current_pos_in_trace = -1;
    
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

    void FixedUpdate() // TODO: to cast a ray to a person, if sees, to go straight to him.
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

        Vector3 targetPosition = player_target.trace[current_pos_in_trace];
        MoveToPosition(targetPosition);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        // or, if cannot see the character, follow his smell, his steps - LAST IDEA, SEEMS THE BEST(were a lot of difficult
        // ideas to implement, this is a masterpiece, trust me:) )

    }
    public void MoveToPosition(Vector3 targetPos)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.fixedDeltaTime);
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            Debug.Log("Achieved position " + current_pos_in_trace);
            current_pos_in_trace++;
        }
    }
}
