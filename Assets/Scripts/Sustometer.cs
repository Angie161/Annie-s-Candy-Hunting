using UnityEngine;
using TMPro;

public class Sustometer : MonoBehaviour
{
    public float stress = 0f;
    public float maxStress = 100f;

    public TextMeshProUGUI stressText;

    public float anomalyStressPerSecond = 0.02f;
    public float mistakeStress = 1f;

    private ClickObjectS[] objects;

    void Start()
    {
        objects = FindObjectsOfType<ClickObjectS>();
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.gameEnded) return;

        UpdateStressFromAnomalies();
        ClampAndCheck();
        UpdateUI();
    }

    void UpdateStressFromAnomalies()
    {
        foreach (var obj in objects)
        {
            if (obj == null) continue;

            if (obj.IsInAnomalyState() && obj.anomalyProgress >= 0.5f)
            {
                stress += anomalyStressPerSecond * Time.deltaTime;
            }
        }
    }

    public void AddMistake()
    {
        stress += 0.5f;
    }

    void ClampAndCheck()
    {
        stress = Mathf.Clamp(stress, 0, maxStress);

        if (stress >= maxStress)
        {
            GameManager.Instance.TriggerGameOver();
        }
    }

    void UpdateUI()
    {
        if (stressText == null) return;

        float percent = (stress / maxStress) * 100f;
        stressText.text = $"Stress: {percent:0}%";
    }
}