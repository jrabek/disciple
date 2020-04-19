using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : MonoBehaviour
{
    [SerializeField]
    private Player player;

    [SerializeField]
    private SoulOrb orbPrefab;

    [SerializeField]
    float baseTimeBetweenExtraction = 0.2f;

    [SerializeField]
    CircleCollider2D proximityCollider;

    public int souls { get; private set; } = 0;

    private float lastExtractionTime = 0.0f;

    private Animator animator;

    private GameManager gameManager;

    private int outstandingSouls = 0;

    bool isOffering = false;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        gameManager = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFacingDirection();
        CheckSoulOffering();
    }

    private void UpdateFacingDirection()
    {
        float horizontal = transform.position.x - player.transform.position.x > 0 ? -1 : 1;
        transform.localScale = new Vector3(horizontal, transform.localScale.y, 1);
    }

    private void CheckSoulOffering()
    {
        if (isOffering)
        {
            float timeBetweenExtraction = baseTimeBetweenExtraction * 10 / player.souls;
            timeBetweenExtraction = timeBetweenExtraction > 0.5f ? 0.5f : timeBetweenExtraction;
            if (Time.time - lastExtractionTime > timeBetweenExtraction)
            {
                lastExtractionTime = Time.time;
                if (player.souls > 0)
                {
                    // print("Souls left to offer " + player.souls);
                    SoulOrb newOrb = Instantiate(orbPrefab, player.transform.position, Quaternion.identity, transform);
                    newOrb.StartOffering(this);                    
                    player.RemoveSoul();
                    outstandingSouls++;
                }               
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {       
        if (!isOffering && collision.CompareTag("Player") && player.souls > 0)
        {
            proximityCollider.enabled = false;
            // print("Start collecting");
            isOffering = true;
            animator.SetTrigger("Munch");            
        }       
    }

    public void SoulOffered(int souls = 1)
    {
        this.souls += souls;
        gameManager.UpdateSoulsOffered(this.souls);

        outstandingSouls--;
        if (outstandingSouls == 0)
        {
            // print("Done offering");
            isOffering = false;
            proximityCollider.enabled = true;
            animator.SetTrigger("Idle");
            player.RestoreTime();
        }
    }
}
