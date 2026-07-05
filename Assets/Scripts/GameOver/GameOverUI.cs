using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI runCandiesText;
    public TextMeshProUGUI walletText;

    void Start()
    {
        runCandiesText.text =
            SaveData.LastRunCandies.ToString();

        walletText.text =
            SaveData.TotalCandies.ToString();
    }

    
}