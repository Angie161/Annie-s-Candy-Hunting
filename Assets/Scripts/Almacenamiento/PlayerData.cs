using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public int totalCandies;
    public int highScore;

    public string equippedSkin = "Default";

    public List<string> unlockedSkins = new List<string>()
    {
        "Default"
    };
}