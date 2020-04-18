using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulOrb : MonoBehaviour
{
    private Animator animator;

    [SerializeField]
    CircleCollider2D proximityCollider;

    [SerializeField]
    CircleCollider2D collectCollider;

    [SerializeField]
    float moveSpeed = 1.5f;

    private Player player;

    private Demon demon;

    private GameObject moveTarget;

    private bool isBeingCollected = false;
    private bool isBeingAbsorbed = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {       
        if (isBeingAbsorbed)
        {
            if (collision.CompareTag("Demon"))
            {
                // print("Collided with " + collision.transform);
                // print("Absorbed");
                demon.SoulAbsorbed();
                Destroy(this.gameObject);
            }            
        } else if (collision.CompareTag("Player"))
        {
            if (!isBeingCollected)
            {
                // print("Start collecting");
                isBeingCollected = true;
                animator.SetTrigger("Pulse");
                player = collision.gameObject.GetComponent<Player>();
                proximityCollider.enabled = false;
                moveTarget = player.gameObject;
            }
            else
            {
                // print("Collected");
                player.AddSoul();
                Destroy(this.gameObject);
            }
        }
    }

    private void Update()
    {
        if (isBeingCollected || isBeingAbsorbed)
        {
            float distance = Vector3.Distance(transform.position, moveTarget.transform.position);            
            transform.position = Vector3.MoveTowards(transform.position, moveTarget.transform.position,  (1 / distance) * (1 / moveSpeed) * Time.deltaTime);
        }
    }

    public void StartAbsortion(Demon demon)
    {        
        moveTarget = demon.gameObject;
        this.demon = demon;
        animator.SetTrigger("Pulse");
        proximityCollider.enabled = false;
        isBeingAbsorbed = true;
    }
}
