using UnityEngine;
using UnityEngine.UI;
using PnCCasualGameKit;

/// <summary>
/// Mangages Game's UI. 
/// </summary>
public class UIManager : PnCUIManger
{

    [Header("Home screen")]
    public Image soundImage;
    public Sprite soundOnSprite, soundOffSprite;
    public GameObject removeAdBtn;
    public Button ARBtn;
    public Sprite AROnSprite, AROffSprite;

    [Header("HUD")]
    public Text scoretext_HUD;
    public Text cashtext_HUD;

    [Header("Shop")]
    public Text cashText;
    public Text buyPopUpText;

    [Header("Game over")]
    public Text scoreText_Gameover;
    public Text highScoreText_GameOver, cashText_GameOver;

    //Singleton instance
    public static UIManager Instance;

    protected override void AwakeInit()
    {
        Instance = this;
        GameManager.Instance.GameInitialized += OpenHomeScreen;
        GameManager.Instance.GameStarted += OpenHUDScreen;
        GameManager.Instance.GameContinued += OpenGameContinueScreen;
        GameManager.Instance.GameOver += OpenGameOverScreen;
    }

    private void OnDestroy()
    {
        GameManager.Instance.GameInitialized -= OpenHomeScreen;
        GameManager.Instance.GameStarted -= OpenHUDScreen;
        GameManager.Instance.GameContinued -= OpenGameContinueScreen;
        GameManager.Instance.GameOver -= OpenGameOverScreen;
    }

    /// <summary> Opens the home screen. </summary>
    void OpenHomeScreen()
    {
        OpenScreen(UIScreensList.HomeScreen);
    }

    /// <summary> Opens the home screen. </summary>
    void OpenHUDScreen()
    {
        SoundManager.Instance.playSound(AudioClips.UI);
        OpenScreen(UIScreensList.HUD);
    }

    /// <summary> Opens the ContinueGame screen. </summary>
    private void OpenGameContinueScreen()
    {
        UIManager.Instance.OpenScreen(UIScreensList.ContinueGame);
    }

    /// <summary> Opens the Game over screen. </summary>
    void OpenGameOverScreen()
    {
        OpenScreen(UIScreensList.GameOver);
    }

    /// <summary> Opens the HUD screen. </summary>
    public void UpdateHudData(int score, float cash)
    {
        scoretext_HUD.text = score.ToString();
        cashtext_HUD.text = cash.ToString();
    }

    /// <summary> Update UI in gameover screen. </summary>
    public void UpdateGameOverData(int score, int highScore, float cash)
    {
        scoreText_Gameover.text = score.ToString();
        highScoreText_GameOver.text = highScore.ToString();
        cashText_GameOver.text = cash.ToString();
    }

    /// <summary> toggle cost pop up screen in the shop </summary>
    public void ToggleOpenCostPopUp(bool status, string text = "buy this skin?")
    {
        if (status)
        {
            buyPopUpText.text = text;
            //OpenModal(UIScreensList.BuyPopUp, true);
            OpenModal(UIScreensList.BuyPopUp);
        }
        else
        {
            CloseModal();
           // CloseModal(UIScreensList.BuyPopUp);
        }
    }

    /// <summary> Toggle sound setting sprite. </summary>
    public void ToggleSoundSprite(bool isSoundOn)
    {
        soundImage.sprite = isSoundOn? soundOnSprite : soundOffSprite;
    }

    /// <summary> Toggle AR mode sprite. </summary>
    public void ToggleARModeSprite(bool isARModeOn)
    {
        ARBtn.image.sprite = isARModeOn ? AROnSprite : AROffSprite;
    }

}




