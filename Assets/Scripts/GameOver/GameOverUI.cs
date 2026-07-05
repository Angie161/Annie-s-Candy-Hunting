using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI runCandiesText;
    public TextMeshProUGUI walletText;

    void Start()
    {
        GameManager gm = GameManager.Instance;

        runCandiesText.text =
            gm.runCandies.ToString();

        walletText.text =
            SaveData.Data.totalCandies.ToString();
    }
}