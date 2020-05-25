using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;

public class Candle : MonoBehaviour
{
    private Light2D light2D;
    private CircleCollider2D circleCollider;
    private Animator animator;

    private void Start()
    {        
        light2D = GetComponent<Light2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
    }

    public void Light(bool lit)
    {
        light2D.enabled = lit;
        circleCollider.enabled = lit;

        if (!lit)
        {
            animator.SetTrigger("Extinguish");
        } else
        {
            animator.SetTrigger("Light");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy obj = collision.gameObject.GetComponent<Enemy>();

        // print("Candle collided with " + collision + " obj:" + obj);
        if (obj && obj.VulnerableToLight())
        {            
            animator.SetTrigger("Flash");
        }
    }
}
