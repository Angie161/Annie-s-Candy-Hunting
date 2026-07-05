using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI runCandiesText;
    public TextMeshProUGUI walletText;
    public GameObject newRecordImage;
    public TextMeshProUGUI highScoreText;

    void Start()
    {

        Debug.Log("GAMEOVER UI START");

        Debug.Log("RUN = " + SaveData.LastRunCandies);
        Debug.Log("HIGH = " + SaveData.Data.highScore);
        Debug.Log("TOTAL = " + SaveData.Data.totalCandies);

        runCandiesText.text =
            SaveData.LastRunCandies.ToString();
        
        highScoreText.text =
            SaveData.Data.highScore.ToString();

        walletText.text =
            SaveData.Data.totalCandies.ToString();

        if (SaveData.LastRunWasNewRecord)
        {
            ShowNewRecord();
        }
    }

    public void ShowNewRecord()
    {
        if (newRecordImage != null)
            newRecordImage.SetActive(true);
    }
}