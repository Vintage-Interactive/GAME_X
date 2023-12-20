using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockroachEgg : MonoBehaviour
{
    // Start is called before the first frame update
    public Cockroach cockroach;
    public Player player_target;
    public float agroRadius = 1f;
    private Vector2 respPos = new Vector2(-100, -100);
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var target = player_target.transform;
        float distance = Vector2.Distance(target.position, transform.position);
        if (distance < agroRadius)
        {
            cockroach.transform.position = transform.position;
            transform.position = respPos;
        }
    }
}
