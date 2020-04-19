using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField]
    DialogKey dialogType = DialogKey.None;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player"))
        {
            return;
        }

        print("DialogTrigger " + dialogType + "collided with player.");

        DialogKey plotPoint = GameManager.instance.plotPoint;

        if (plotPoint == dialogType)
        {
            print("Show the dialog");
            GameManager.instance.ShowDialog(dialogType);
            Destroy(this.gameObject);
        } else
        {
            print("Not time yet to show " + dialogType + ". Plot point is " + plotPoint);
        }
    }
}
