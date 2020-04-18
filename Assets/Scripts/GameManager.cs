using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{  
    public bool playersTurn = true;

    public Grid grid;

    public Vector3 gridSize { get; private set; }

    public static GameManager instance;

    private HashSet<Enemy> enemies = new HashSet<Enemy>();        

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
}
