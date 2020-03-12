using System.Collections.Generic;
using PnCCasualGameKit;

/// <summary>
/// Data holding player's game progress.
/// </summary>
[System.Serializable]
public class PlayerData : PlayerDataHandler<PlayerData> {

    public int score;
    public int highScore;
    public float cash;
    public List<int> unlockedSkinItems;
    public int selectedSkinID;
    public bool soundSetting;
    public bool adsRemoved;

    public PlayerData()
    {
        score = 0;
        cash = 100;
        unlockedSkinItems = new List<int>();
        selectedSkinID = 0;
        soundSetting = true;
        adsRemoved = false;
    }   

}