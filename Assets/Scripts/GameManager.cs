using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;        //Allows us to use SceneManager
using UnityEngine.UI;

public enum RewardType {
    RewardNone,
    RewardPushCrates,
    RewardMoreTime,
    RewardCarryDoubleSouls,
    RewardEvenMoreTime,
    RewardDash, // cross gaps
    RewardDestroyDemon,
    RewardMax
}

public struct RewardLevel
{
    public int soulsRequired;
    public RewardType reward;

    public RewardLevel(int soulsRequired, RewardType reward)
    {
        this.soulsRequired = soulsRequired;
        this.reward = reward;
    }
}

public class GameManager : CheckPointObject
{  
    public bool playersTurn = true;

    public Grid grid;

    public Vector3 gridSize { get; private set; }

    public bool paused { get; private set; } = false;

    private bool showingDialog = false;

    public static GameManager instance;

    private HashSet<Enemy> enemies = new HashSet<Enemy>();

    private bool waitingToRestart = false;

    int nextRewardLevel = 1;

    private DialogContainer dialogs = new DialogContainer();
    public DialogKey plotPoint = DialogKey.WhereAmI;

    [SerializeField]
    private Player player;

    public RewardLevel currentRewardLevel { get; private set; }

    private RewardLevel[] rewardLevels = new RewardLevel[]
    {
        new RewardLevel(0, RewardType.RewardNone),
        new RewardLevel(4, RewardType.RewardPushCrates),
        new RewardLevel(9, RewardType.RewardMoreTime),
        new RewardLevel(12, RewardType.RewardCarryDoubleSouls),
        new RewardLevel(16, RewardType.RewardEvenMoreTime),
        new RewardLevel(18, RewardType.RewardDash),
        new RewardLevel(20, RewardType.RewardDestroyDemon),
        new RewardLevel(21, RewardType.RewardMax),
    };
   
    [SerializeField]
    private SoulBar soulsOffered;

    [SerializeField]
    private SoulBar soulsCollected;

    [SerializeField]
    private Text timeText;

    [SerializeField]
    private Text gameOverText;

    [SerializeField]
    private Text dialogText;

    [SerializeField]
    private GameObject dialog;

    private void Awake()
    {         
        if (!GameManager.instance)
        {
            instance = this;
        }
        
        gridSize = grid.cellSize;        
    }

    public void Start()
    {
        InitializeSoulSliders();        
    }

    public override void SaveState()
    {
        SaveInt("plotPoint", (int)plotPoint);
        SaveInt("nextRewardLevel", nextRewardLevel);
    }

    public override void LoadState()
    {
        paused = false;
        waitingToRestart = false;
        gameOverText.gameObject.SetActive(false);
        plotPoint = (DialogKey)RestoreInt("plotPoint");
        nextRewardLevel = RestoreInt("nextRewardLevel");
    }

    public void InitializeSoulSliders()
    {
        soulsOffered.UpdateMaximumPossible(rewardLevels[rewardLevels.Length - 1].soulsRequired);
        soulsOffered.UpdateCurrentMaximum(rewardLevels[1].soulsRequired, false);
        soulsOffered.UpdateCurrent(0);

        soulsCollected.UpdateMaximumPossible(player.maxSoulCapacity);
        soulsCollected.UpdateCurrentMaximum(player.soulCapacity, false);
        soulsCollected.UpdateCurrent(player.souls);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (waitingToRestart)
            {
                Restart();
            } else if (showingDialog)
            {
                HideDialog();
            }            
        }
    }

    private void HideDialog()
    {
        dialogText.text = "";
        showingDialog = false;
        paused = false;
        dialog.SetActive(false);
    }

    public void ShowDialog(DialogKey key)
    {
        dialogText.text = dialogs.TextForKey(key);
        dialog.SetActive(true);
        showingDialog = true;
        paused = true;
        plotPoint = key + 1;
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
            if (enemy.gameObject.active)
            {
                enemy.StartAI();
            }            
        }        
    }

    public void EnemyFinishAction()
    {
        foreach (Enemy enemy in enemies)
        {
            if (!enemy.AIComplete())
            {
                // print($"{enemy} not done moving");
                return;
            }
        }
        playersTurn = true;
    }
    

    public void GameOver(string reason)
    {
        paused = true;
        gameOverText.gameObject.SetActive(true);        
        print("Game over " + reason);
        gameOverText.text = "You Failed To Stay Alive\n\n" + reason + "\n\nHit Space To Try Again";

        waitingToRestart = true;
    }

    //Restart reloads the scene when called.
    private void Restart()
    {
        if (CheckPointManager.instance.hasCheckpoint)
        {
            print("Checkpoint found. Reloading checkpoint.");
            CheckPointManager.instance.LoadState();            
        }
        else
        {
            print("No checkpoint found. Reloading scene.");
            //Load the last scene loaded, in this case Main, the only scene in the game.
            SceneManager.LoadScene(0);
        }               
    }

    public void UpdateSoulCapacity()
    {
        soulsCollected.UpdateMaximumPossible(player.maxSoulCapacity);
        soulsCollected.UpdateCurrentMaximum(player.soulCapacity);
        soulsCollected.UpdateCurrent(player.souls);
    }

    public void UpdateSouls(int souls)
    {
        soulsCollected.UpdateCurrent(souls);
    }

    public void UpdateTime(int time)
    {
        timeText.text = "" + time;
    }

    public void UpdateSoulsOffered(int souls)
    {
        print("Update souls offered to " + souls);
        RewardLevel rewardLevel = rewardLevels[0];
        bool levelUp = false;        
        for (int idx = 0; idx < rewardLevels.Length; idx ++)
        {
            rewardLevel = rewardLevels[idx];
            if (souls < rewardLevel.soulsRequired)
            {
                if (nextRewardLevel < idx)
                {
                    nextRewardLevel++;                    
                    levelUp = true;
                }
                break;
            } else
            {
                switch(rewardLevel.reward)
                {
                    case RewardType.RewardCarryDoubleSouls: player.EnableDoubleSouls(); break;
                    case RewardType.RewardMoreTime: player.SetTimeMultiplier(2); break;
                    case RewardType.RewardDash: player.EnableDash(); break;
                    case RewardType.RewardEvenMoreTime: player.SetTimeMultiplier(3); break;
                    case RewardType.RewardDestroyDemon: player.EnableKillDemon(); break;
                    case RewardType.RewardPushCrates: player.EnablePushCrates(); break;
                }
            }
        }
        soulsOffered.UpdateCurrent(souls);
        soulsOffered.UpdateCurrentMaximum(rewardLevel.soulsRequired);

        if (levelUp)
        {
            LevelUp(rewardLevels[nextRewardLevel - 1]);                                   
        }
    }

    private void LevelUp(RewardLevel rewardLevel)
    {
        // TODO: animation to show level up along with what power was received.
        print("Level up. " + rewardLevel.reward);        

        DialogKey dialogKey = DialogKey.None;
        switch (rewardLevel.reward)
        {
            case RewardType.RewardPushCrates: dialogKey = DialogKey.MoveCrates; break;
            case RewardType.RewardCarryDoubleSouls: dialogKey = DialogKey.MoreSoulCapacity; break;
            case RewardType.RewardMoreTime: dialogKey = DialogKey.MoreTime; break;
            case RewardType.RewardDash: dialogKey = DialogKey.NowCanDash; break;
            case RewardType.RewardEvenMoreTime: dialogKey = DialogKey.EvenMoreTime; break;            
        }

        if (dialogKey != DialogKey.None)
        {
            ShowDialog(dialogKey);
        }        
    }

    public void IndicateFullSouls()
    {
        // TODO: Some sort of flash?
    }
}
