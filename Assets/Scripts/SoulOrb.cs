using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulOrb : CheckPointObject
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
    private bool isBeingOffered = false;
    private bool wasCollected = false;

    private GameManager gameManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();        
    }

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    public override void SaveState()
    {
        SavePosition(transform.position);
        SaveBool("wasCollected", wasCollected);
    }

    public override void LoadState()
    {
        transform.position = RestorePosition();
        wasCollected = RestoreBool("wasCollected");
        // TODO: Generic way to do this for collectibles
        if (wasCollected)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameManager.paused)
        {
            return;
        }

        if (isBeingOffered)
        {
            if (collision.CompareTag("Demon"))
            {
                // print("Collided with " + collision.transform);
                // print("Offered");
                demon.SoulOffered();
                Destroy(this.gameObject);
            }            
        } else if (collision.CompareTag("Player"))
        {
            player = collision.gameObject.GetComponent<Player>();
            if (!isBeingCollected)
            {
                if (player.CouldAddSoul())
                {
                    // print("Start collecting");
                    isBeingCollected = true;
                    animator.SetTrigger("Pulse");                    
                    proximityCollider.enabled = false;
                    moveTarget = player.gameObject;
                } else {
                    player.CantAddSouls();
                }
            }
            else
            {
                // print("Collected");
                player.AddSoul();
                // Destroy(this.gameObject);
                wasCollected = true;
                gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (gameManager.paused)
        {
            return;
        }

        if (isBeingCollected || isBeingOffered)
        {
            float distance = Vector3.Distance(transform.position, moveTarget.transform.position);            
            transform.position = Vector3.MoveTowards(transform.position, moveTarget.transform.position,  (1 / distance) * (1 / moveSpeed) * Time.deltaTime);
        }
    }

    public void StartOffering(Demon demon)
    {        
        moveTarget = demon.gameObject;
        this.demon = demon;
        animator.SetTrigger("Pulse");
        proximityCollider.enabled = false;
        isBeingOffered = true;
    }
}
