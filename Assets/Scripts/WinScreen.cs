using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    private bool isCanvasShown = false;

    // Update is called once per frame
    void Update()
    {
        if (isCanvasShown && Input.GetButton("Submit"))
        {
            canvas.enabled = false;
            isCanvasShown = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canvas.enabled = true;
            isCanvasShown = true;
        }
    }
}
