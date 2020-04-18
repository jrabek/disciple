using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


enum RewardType {
    RewardPushCrates,
    RewardCarryDoubleSouls,
    RewardDestroyDemon
}

struct RewardLevel
{
    public int soulsRequired;
    public RewardType reward;

    public RewardLevel(int soulsRequired, RewardType reward)
    {
        this.soulsRequired = soulsRequired;
        this.reward = reward;
    }
}

public class GameManager : MonoBehaviour
{  
    public bool playersTurn = true;

    public Grid grid;

    public Vector3 gridSize { get; private set; }

    public static GameManager instance;

    private HashSet<Enemy> enemies = new HashSet<Enemy>();

    [SerializeField]
    RewardLevel currentRewardLevel;

    private RewardLevel[] rewardLevels = new RewardLevel[]
    {
        new RewardLevel(5, RewardType.RewardPushCrates),
        new RewardLevel(10, RewardType.RewardCarryDoubleSouls),
        new RewardLevel(15, RewardType.RewardDestroyDemon)
    };
   
    [SerializeField]
    private Text soulText;

    [SerializeField]
    private Text spiritText;

    [SerializeField]
    private Text soulAbsorbedText;

    private void Awake()
    {         
        if (!GameManager.instance)
        {
            instance = this;
        }

        gridSize = grid.cellSize;
        UpdateSoulsAbsorbed(0);
        UpdateSouls(0);
        UpdateSpirit(0);
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

    public void UpdateSoulsAbsorbed(int souls)
    {
        RewardLevel rewardLevel = rewardLevels[0];
        for (int idx = 0; idx < rewardLevels.Length; idx ++)
        {
            rewardLevel = rewardLevels[idx];
            if (souls < rewardLevel.soulsRequired)
            {
                break;
            }            
        }
        soulAbsorbedText.text = "Delivered Souls: " + souls + "\nNext reward at " + rewardLevel.soulsRequired + " souls";
    }
}
