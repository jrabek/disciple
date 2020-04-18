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
    float moveSpeed = 1.5;

    Player player;

    bool isBeingCollected = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Collided with " + collision.transform);

        if (!collision.CompareTag("Player"))
        {
            return;
        }

        if (!isBeingCollected)
        {
            print("Start collecting");
            isBeingCollected = true;
            animator.SetTrigger("Pulse");
            player = collision.gameObject.GetComponent<Player>();
            proximityCollider.enabled = false;
        } else
        {
            print("Collected");
            player.AddSoul();
            Destroy(this.gameObject);
        }                
    }

    private void Update()
    {
        if (isBeingCollected)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            print("distance " + distance);
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position,  (1 / distance) * (1 / moveSpeed) * Time.deltaTime);
        }
    }
}
