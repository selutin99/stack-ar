using UnityEngine;
using UnityEngine.Events;
using PnCCasualGameKit;

/// <summary>
/// Consits of Game state events and other General Game specific functionalities. 
/// </summary>
public class GameManager : LazySingleton<GameManager>
{
    [HideInInspector]
    public GameMode gameMode;

    //Avoiding unncessary platform sepecific warnings
    #pragma warning disable
    [SerializeField]
    private string playStoreURL = default;

    [SerializeField]
    private string appStoreURL = default;
    #pragma warning restore

    /// <summary>
    ///  Game state events
    /// </summary>
    public System.Action GameInitialized, GameStarted, Scored, GameContinued, GameOver;

    /// <summary>
    /// Unity events for the above states
    /// </summary>
    public UnityEvent OnGameIntialized, OnGameStarted, OnGameOver;

    private void Awake()
    {
        // Make the game run as fast as possible. Avoids setting to default frame rate on device.
        Application.targetFrameRate = 300;
        gameMode = GetComponent<GameMode>();
        PlayerData.Create();
        GameInitialized += delegate { OnGameIntialized.Invoke(); };
        GameStarted += delegate { OnGameStarted.Invoke(); };
        GameOver += delegate { OnGameOver.Invoke(); };
    }

    private void OnDestroy()
    {
        GameInitialized -= delegate { OnGameIntialized.Invoke(); };
        GameStarted -= delegate { OnGameStarted.Invoke(); };
        GameOver -= delegate { OnGameOver.Invoke(); };
    }

    /// <summary>
    /// Init Game in Start because other classes register for events in Awake
    /// </summary>
    void Start()
    {
        InitGame();
        if (PlayerData.Instance.adsRemoved)
        {
            UIManager.Instance.removeAdBtn.SetActive(false);
        }
    }

    /// <summary>
    /// Initialise game
    /// </summary>
    public void InitGame()
    {
        //In editor, the game doesn't pause when ad is shown. Workaround : set timescale 0 when showing ad.
#if UNITY_EDITOR
        if (Time.timeScale == 0)
            return;
#endif

        if (GameInitialized != null)
            GameInitialized();

        gameMode.enabled = true;
    }

    /// <summary>
    /// Start Game
    /// </summary>
    public void StartGame()
    {
        //In editor, the game doesn't pause when ad is shown. Workaround : set timescale 0 when showing ad.
#if UNITY_EDITOR
        if (Time.timeScale == 0)
            return;
#endif
        if (GameStarted != null)
            GameStarted();
    }

    /// <summary>
    /// Game over
    /// </summary>
    public void EndGame()
    {
#if UNITY_EDITOR
        if (Time.timeScale == 0)
            return;
#endif

        if (GameOver != null)
            GameOver();
    }

    /// <summary>
    /// Opens the store pages
    /// </summary>
    public void RateGame()
    {
#if UNITY_ANDROID
        Application.OpenURL(playStoreURL);
#elif UNITY_IOS
        Application.OpenURL(appStoreURL);
#endif
    }

    /// <summary>
    /// Removes ad permanantly from the game.
    /// </summary>
    public void RemoveAds()
    {
        PlayerData.Instance.adsRemoved = true;
        PlayerData.Instance.SaveData();
        AdsManager.Instance.isAdRemoved = true;
        UIManager.Instance.removeAdBtn.SetActive(false);
    }


    /// <summary>
    ///For testing.  Editor Buttons available in the inspector.
    /// </summary>
#region TESTING

    /// <summary>
    /// Increases player cash
    /// </summary>
    public void IncreasePlayerCash()
    {
        float increaseBy = 100;
        PlayerData.Instance.cash += increaseBy;
        PlayerData.Instance.SaveData();
        Debug.Log("player cash increased by " + increaseBy);
    }

   /// <summary>
   /// Decreases the player cash.
   /// </summary>
    public void DecreasePlayerCash()
    {
        float decreaseBy = 100;
        PlayerData.Instance.cash -= decreaseBy;
        PlayerData.Instance.SaveData();
        Debug.Log("player cash decreased by " + decreaseBy);
    }

    /// <summary>
    /// Deletes and resets player data
    /// </summary>
    public void ClearPlayerData()
    {
       // UnityEditor.EditorPrefs.DeleteAll();
        PlayerData.Clear();
        Debug.Log("player data cleared");
    }
#endregion

}
