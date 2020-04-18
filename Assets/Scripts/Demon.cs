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

    bool isAbsorbing = false;
    
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
        CheckSoulAbsorbtion();
    }

    private void UpdateFacingDirection()
    {
        float horizontal = transform.position.x - player.transform.position.x > 0 ? -1 : 1;
        transform.localScale = new Vector3(horizontal, transform.localScale.y, 1);
    }

    private void CheckSoulAbsorbtion()
    {
        if (isAbsorbing)
        {
            float timeBetweenExtraction = baseTimeBetweenExtraction * 10 / player.souls;
            timeBetweenExtraction = timeBetweenExtraction > 0.5f ? 0.5f : timeBetweenExtraction;
            if (Time.time - lastExtractionTime > timeBetweenExtraction)
            {
                lastExtractionTime = Time.time;
                if (player.souls > 0)
                {
                    // print("Souls left to absorb " + player.souls);
                    SoulOrb newOrb = Instantiate(orbPrefab, player.transform.position, Quaternion.identity, transform);
                    newOrb.StartAbsortion(this);                    
                    player.RemoveSoul();
                    outstandingSouls++;
                }               
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {       
        if (!isAbsorbing && collision.CompareTag("Player") && player.souls > 0)
        {
            proximityCollider.enabled = false;
            // print("Start collecting");
            isAbsorbing = true;
            animator.SetTrigger("Munch");            
        }       
    }

    public void SoulAbsorbed(int souls = 1)
    {
        this.souls += souls;
        gameManager.UpdateSoulsAbsorbed(this.souls);

        outstandingSouls--;
        if (outstandingSouls == 0)
        {
            // print("Done absorbing");
            isAbsorbing = false;
            proximityCollider.enabled = true;
            animator.SetTrigger("Idle");
        }
    }
}
