using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{  
    public bool playersTurn = true;

    public Grid grid;

    public Vector3 gridSize { get; private set; }

    public static GameManager instance;

    private HashSet<Enemy> enemies = new HashSet<Enemy>();

    [SerializeField]
    private Text soulText;

    [SerializeField]
    private Text spiritText;

    private void Awake()
    {         
        if (!GameManager.instance)
        {
            instance = this;
        }

        gridSize = grid.cellSize;
    }

    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);        
    }

    public void PlayerStartAction()
    {
        playersTurn = false;
    }

    public void PlayerFinishAction()
    {        
        foreach (Enemy enemy in enemies)
        {
            enemy.RunAI();
        }

        playersTurn = true;
    }

    public void GameOver()
    {

    }

    public void UpdateSouls(int souls)
    {
        soulText.text = "Souls: " + souls;
    }

    public void UpdateSpirit(int spirit)
    {
        spiritText.text = "Spirit: " + spirit;
    }
}
