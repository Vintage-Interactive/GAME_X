using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    public float stoppingForce = 10f;
    //private void OnTriggerEnter2D(Collider2D other)
    //{
        //if (other.CompareTag("Hero"))
        //{
        //    Debug.Log("Oops");
        //   var obj = other.GetComponent<MainCharacter>();
            // other.GetComponent<Rigidbody2D>().position = other.GetComponent<MainCharacter>().previousPosition;
        //    Vector2 cur_vel = obj.rb.velocity;
        //    obj.rb.AddForce(-cur_vel.normalized * stoppingForce, ForceMode2D.Impulse);
        //}
    //}

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
