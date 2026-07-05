using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Sustometer : MonoBehaviour
{
    [Header("UI")]
    public Slider stressBar;
    public Image fillImage;

    private Color lowStressColor;
    private Color mediumStressColor;
    private Color highStressColor;
    
    [Header("Stress Settings")]
    public float stress = 0f;
    public float maxStress = 100f;

    //[Header("UI")]
    //public TextMeshProUGUI stressText;

    [Header("Stress Gain")]
    public float anomalyStressPerSecond = 2f;
    public float mistakeStress = 1.0f;

    private ClickObjectS[] objects;

    void Start()
    {
        objects = FindObjectsByType<ClickObjectS>(FindObjectsSortMode.None);

        ColorUtility.TryParseHtmlString("#5E9C76", out lowStressColor);
        ColorUtility.TryParseHtmlString("#D8A84C", out mediumStressColor);
        ColorUtility.TryParseHtmlString("#A94442", out highStressColor);

        //Debug.Log("Objetos encontrados: " + objects.Length);

        /*if (stressText == null)
        {
            Debug.LogError("No asignaste Stress Text en el Inspector.");
        }*/

        UpdateUI();
    }

    void Update()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance es NULL");
            return;
        }

        if (GameManager.Instance.gameEnded)
            return;

        UpdateStressFromAnomalies();
        ClampAndCheck();
        UpdateUI();
    }

    void UpdateStressFromAnomalies()
    {
        foreach (var obj in objects)
        {
            if (obj == null)
                continue;

            if (obj.IsInAnomalyState())
            {
                stress += anomalyStressPerSecond * Time.deltaTime;
            }
        }
    }

    public void AddMistake()
    {
        stress += mistakeStress;

        //Debug.Log("Error cometido. Stress actual: " + stress);
    }

    void ClampAndCheck()
    {
        stress = Mathf.Clamp(stress, 0, maxStress);

        if (stress >= maxStress)
        {
            Debug.Log("GAME OVER ACTIVADO");

            if (GameManager.Instance != null)
            {
               GameManager.Instance.TriggerGameOver();
               SceneManager.LoadScene("GameOver");
            }
        }
    }

    void UpdateUI()
    {
        if (stressBar != null)
        {
            stressBar.value = stress;
        }

        if (fillImage != null)
        {
            float t = stress / maxStress;

            if (t < 0.5f)
            {
                fillImage.color =
                    Color.Lerp(
                        lowStressColor,
                        mediumStressColor,
                        t * 2f
                    );
            }
            else
            {
                fillImage.color =
                    Color.Lerp(
                        mediumStressColor,
                        highStressColor,
                        (t - 0.5f) * 2f
                    );
            }
        }
            /*if (stressText == null)
            return;

        float percent = (stress / maxStress) * 100f;

        stressText.text =
            $"Stress: {stress:F1}/{maxStress:F0}\n" +
            $"{percent:F1}%";*/

    }
} 