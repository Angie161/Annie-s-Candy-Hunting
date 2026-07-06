using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Sustometer : MonoBehaviour
{
    [Header("UI")]
    public Slider stressBar;
    public Image fillImage;

    [Header("Border")]
    public Image borderImage;

    private float borderTargetAlpha = 0f;
    private float borderCurrentAlpha = 0f;
    private float mistakeFlashTimer = 0f;

    private Color lowStressColor;
    private Color mediumStressColor;
    private Color highStressColor;
    
    [Header("Stress Settings")]
    public float stress = 0f;
    public float maxStress = 100f;

    [Header("Breathing Audio")]
    public AudioSource breathingAudio;

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
        UpdateBorder();
        UpdateMistakeFlash();
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

    void UpdateBorder()
    {
        if (borderImage == null)
            return;
        
        if (mistakeFlashTimer > 0f)
        return;

        bool danger =
            System.Array.Exists(objects, obj =>
                obj != null && obj.IsInAnomalyState()
            );

        borderTargetAlpha = danger ? 0.6f : 0f;

        borderCurrentAlpha =
            Mathf.Lerp(
                borderCurrentAlpha,
                borderTargetAlpha,
                Time.deltaTime * 4f
            );

        // base color (normal state)
        ColorUtility.TryParseHtmlString("#780A0A", out Color baseColor);
        baseColor.a = borderCurrentAlpha;

        // 🔴 si hay flash de mistake, lo sobrescribimos SOLO aquí
        if (mistakeFlashTimer > 0f)
            return;

        borderImage.color = baseColor;
    }

    public void AddMistake()
    {
        stress += mistakeStress;
        mistakeFlashTimer = 0.2f;
    }

    void UpdateMistakeFlash()
    {
        if (mistakeFlashTimer > 0f)
        {
            mistakeFlashTimer -= Time.deltaTime;

            float t = mistakeFlashTimer / 0.2f;

            ColorUtility.TryParseHtmlString("#780A0A", out Color c);

            c.a = Mathf.Lerp(0.9f, 0f, 1f - t);

            borderImage.color = c;
        }
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
            }
        }
    }

    void UpdateUI()
    {
        if (stressBar != null)
        {
            stressBar.value = stress;
        }

        UpdateBreathing();

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

    void UpdateBreathing()
    {
        if (breathingAudio == null)
            return;

        float stressPercent = stress / maxStress;

        if (stressPercent < 0.75f)
        {
            breathingAudio.volume = 0f;

            if (breathingAudio.isPlaying)
                breathingAudio.Stop();

            return;
        }

        if (!breathingAudio.isPlaying)
        {
            breathingAudio.Play();
        }

        float volume =
            Mathf.InverseLerp(
                0.75f,
                1f,
                stressPercent
            );

        breathingAudio.volume =
            Mathf.Clamp01(volume);
    }
} 