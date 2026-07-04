using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    // ---------------- GLOBAL ACCESS ----------------
    public static GameManager Instance;

    // ---------------- FADE ----------------
    [Header("Fade")]
    public Image blackScreen;

    // ---------------- CAMERA ----------------
    [Header("Camera")]
    public Transform cameraPivot;
    public Vector3 startPosition;
    public CameraMovement cameraMovement;

    // ---------------- LOOP ----------------
    private bool isResetting = false;

    // ---------------- TIMER ----------------
    /*[Header("Timer")]
    public float timeRemaining = 120f;
    public TextMeshProUGUI timerText;*/

    // ---------------- DIFFICULTY ----------------
    [Header("Difficulty")]
    public int maxActiveAnomalies = 3;
    public float difficultyIncreaseInterval = 30f;

    private float difficultyTimer;

    // ---------------- CANDY SYSTEM ----------------
    [Header("Candy System")]
    public int runCandies = 0;
    public int totalCandies = 0;
    public TextMeshProUGUI candyText;

    // ---------------- END GAME ----------------
    [Header("End Game")]
    public GameObject winScreen;
    public bool gameEnded = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gameEnded = false;
        isResetting = false;
    }

    void Update()
    {
        if (gameEnded) return;

        //HandleTimer();
        HandleLoopReset();
        HandleDifficulty();
    }

    // ---------------- DIFFICULTY ----------------
    void HandleDifficulty()
    {
        difficultyTimer += Time.deltaTime;

        if (difficultyTimer >= difficultyIncreaseInterval)
        {
            difficultyTimer = 0f;
            maxActiveAnomalies++;

            Debug.Log(
                "Máximo de anomalías: " +
                maxActiveAnomalies
            );
        }
    }

    // ---------------- CANDY SYSTEM ----------------
    public void AddCandies(int amount)
    {
        runCandies += amount;
    }

    // ---------------- TIMER ----------------
    /*void HandleTimer()
    {
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            StartCoroutine(EndGameSequence());
            return;
        }

        UpdateTimerUI();
    }*/

    /*void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }*/

    // ---------------- LOOP ----------------
    void HandleLoopReset()
    {
        if (isResetting) return;

        if (cameraPivot.position.z >= cameraMovement.endPoint)
        {
            StartCoroutine(ResetLoop());
        }
    }

    IEnumerator ResetLoop()
    {
        isResetting = true;

        cameraMovement.canMove = false;

        yield return StartCoroutine(FadeBlack(1));

        ProceduralSlot[] slots = FindObjectsOfType<ProceduralSlot>();

        foreach (ProceduralSlot slot in slots)
        {
            slot.GenerateSprite();
        }

        cameraPivot.position = startPosition;

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(FadeBlack(0));

        cameraMovement.canMove = true;

        isResetting = false;
    }

    // ---------------- FADE ----------------
    IEnumerator FadeBlack(float targetAlpha)
    {
        float duration = 0.5f;
        float elapsed = 0f;

        Color startColor = blackScreen.color;
        Color targetColor = blackScreen.color;

        targetColor.a = targetAlpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            blackScreen.color = Color.Lerp(
                startColor,
                targetColor,
                elapsed / duration
            );

            yield return null;
        }

        blackScreen.color = targetColor;
    }

    // ---------------- END GAME ----------------
    IEnumerator EndGameSequence()
    {
        if (gameEnded) yield break;

        gameEnded = true;

        cameraMovement.canMove = false;

        Debug.Log("🔥 END GAME STARTED");

        yield return StartCoroutine(FadeBlack(1));

        if (winScreen != null)
            winScreen.SetActive(true);

        Debug.Log("🎉 GAME COMPLETED");
    }

    public void TriggerGameOver()
    {
        if (gameEnded) return;
        StartCoroutine(EndGameSequence());
    }
}