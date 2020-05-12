using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO: Where should the check live to see if we actually want to checkpoint?
        // This would be:
        //   enough steps to make it to the demon
        //   enough souls
        // Seems each checkpoint would define it but we can have reasonable defaults        

        if (collision.gameObject.CompareTag("Player"))
        {
            print("Checkpoint entered");
            
            CheckPointManager.instance.SaveState(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            print("Checkpoint exited");
        }
    }
}
