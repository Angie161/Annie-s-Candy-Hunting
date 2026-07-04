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

    private float candyTimer = 0f;
    public float candyInterval = 2f;
    public int candiesPerTick = 1;

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
        if (!gameEnded)
        {
            HandleCandyGeneration();
        }

        UpdateCandyUI();

        HandleLoopReset();
        HandleDifficulty();
    }

    void UpdateCandyUI()
    {
        if (candyText == null) return;

        candyText.text = $"{runCandies}";
    }

    void HandleCandyGeneration()
    {
        if (gameEnded) return;

        candyTimer += Time.deltaTime;

        if (candyTimer >= candyInterval)
        {
            candyTimer = 0f;
            runCandies += candiesPerTick;
        }
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
        {
            totalCandies += runCandies;
            winScreen.SetActive(true);
        }

        Debug.Log("🎉 GAME COMPLETED");
    }

    public void TriggerGameOver()
    {
        if (gameEnded) return;
        StartCoroutine(EndGameSequence());
    }

    public void ResetRun()
    {
        runCandies = 0;
        candyTimer = 0f;
    }
}