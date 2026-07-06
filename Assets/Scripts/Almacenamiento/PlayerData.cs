<<<<<<< Updated upstream
=======
using System.Collections.Generic;

[System.Serializable]
>>>>>>> Stashed changes
public class PlayerData
{
    public int totalCandies;
    public int highScore;

<<<<<<< Updated upstream
    // Aquí puedes guardar las adquisiciones Natiiii
    //public List<string> skins; (No sé si sería así, son ideas jsdjddf)
    //public string equippedSkin;
=======
    public string equippedSkin = "Default";

    public List<string> unlockedSkins = new List<string>()
    {
        "Default"
    };
>>>>>>> Stashed changes
}