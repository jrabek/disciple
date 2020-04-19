using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;        //Allows us to use SceneManager

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

public class GameManager : MonoBehaviour
{  
    public bool playersTurn = true;

    public Grid grid;

    public Vector3 gridSize { get; private set; }

    public bool paused { get; private set; } = false;

    private bool showingDialog = false;

    public static GameManager instance;

    private int highestReward;    
    private float soulBarMaxWidth;

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
        new RewardLevel(10, RewardType.RewardMoreTime),
        new RewardLevel(6, RewardType.RewardCarryDoubleSouls),
        new RewardLevel(10, RewardType.RewardEvenMoreTime),
        new RewardLevel(8, RewardType.RewardDash),
        new RewardLevel(10, RewardType.RewardDestroyDemon),
        new RewardLevel(12, RewardType.RewardMax),
    };
   
    [SerializeField]
    private Slider soulsOfferedSlider;

    [SerializeField]
    private Text timeText;

    [SerializeField]
    private Text gameOverText;

    [SerializeField]
    private Text dialogText;

    [SerializeField]
    private GameObject dialog;

    [SerializeField]
    private Slider soulsCollectedSlider;

    private void Awake()
    {         
        if (!GameManager.instance)
        {
            instance = this;
        }

        highestReward = rewardLevels[rewardLevels.Length - 1].soulsRequired;
        gridSize = grid.cellSize;        
    }

    public void Start()
    {

        //soulBarMaxWidth = soulsOfferedSlider.GetComponent<RectTransform>().rect.width;

        //print("Max soulbar width " + soulBarMaxWidth);

        soulsOfferedSlider.maxValue = highestReward;        

        UpdateSoulsOffered(0);
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
            enemy.RunAI();
        }

        playersTurn = true;
    }

    public void GameOver(string reason)
    {
        gameOverText.enabled = true;
        print("Game over " + reason);
        gameOverText.text = "You Failed To Stay Alive\n\n" + reason + "\n\nHit Space To Tray Again";

        waitingToRestart = true;
    }

    //Restart reloads the scene when called.
    private void Restart()
    {
        //Load the last scene loaded, in this case Main, the only scene in the game.
        SceneManager.LoadScene(0);
    }

    public void UpdateSoulCapacity(int soulCapacity)
    {                
        UpdateSliderMaxValue(soulsCollectedSlider, soulCapacity, Player.maxSoulCapacity);
    }

    public void UpdateSouls(int souls)
    {
        soulsCollectedSlider.value = souls;
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
        // UpdateSliderMaxValue(soulsOfferedSlider, rewardLevel.soulsRequired, highestReward);
        UpdateSliderMaxValue(soulsOfferedSlider, highestReward, highestReward);
        soulsOfferedSlider.value = souls;

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
            case RewardType.RewardCarryDoubleSouls: dialogKey = DialogKey.MoveCrates; break;
            case RewardType.RewardMoreTime: dialogKey = DialogKey.MoveCrates; break;
            case RewardType.RewardDash: dialogKey = DialogKey.MoveCrates; break;
            case RewardType.RewardEvenMoreTime: dialogKey = DialogKey.MoveCrates; break;            
        }

        if (dialogKey != DialogKey.None)
        {
            ShowDialog(dialogKey);
        }        
    }

    private void UpdateSliderMaxValue(Slider slider, int maxValue, int maxBarValue)
    {
        // TODO: How can we make it look like the bar extends when maxValue is updated?
     //   RectTransform rectTransform = slider.GetComponent<RectTransform>();
     //   float newWidth = rectTransform.sizeDelta.x + ((float)maxValue / (float)maxBarValue) * soulBarMaxWidth;
     //   rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.sizeDelta.y);        
        slider.maxValue = maxValue;
    }

    public void IndicateFullSouls()
    {
        // TODO: Some sort of flash?
    }
}
