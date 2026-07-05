using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ---------------- GLOBAL ACCESS ----------------
    public static GameManager Instance;

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

    // ---------------- FADE ----------------
    private FadeController fadeController;

    // ---------------- CANDY SYSTEM ----------------
    [Header("Candy System")]
    public int runCandies = 0;

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

        ResetRun();
    }

    void Update()
    {
        if (gameEnded)
            return;

        HandleCandyGeneration();
        HandleDifficulty();

        if (cameraPivot == null)
            return;

        HandleLoopReset();
    }

    void HandleCandyGeneration()
    {
        if (gameEnded) return;

        candyTimer += Time.deltaTime;

        if (candyTimer >= candyInterval)
        {
            candyTimer = 0f;
            runCandies += candiesPerTick;
            //Debug.Log("Candies: " + runCandies);
        }
    }
    // ---------------- FADE ----------------
    void FindFadeController()
    {
        if (fadeController == null)
        {
            fadeController = FindFirstObjectByType<FadeController>();
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

            //Debug.Log(
            //    "Máximo de anomalías: " +
            //    maxActiveAnomalies
            //);
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
        if (gameEnded) return;
        if (isResetting) return;
        if (cameraPivot == null || cameraMovement == null) return;

        if (cameraPivot.position.z >= cameraMovement.endPoint)
        {
            StartCoroutine(ResetLoop());
        }
    }

    IEnumerator ResetLoop()
    {
        isResetting = true;

        cameraMovement.canMove = false;

        FindFadeController();

        if (fadeController != null)
            yield return StartCoroutine(fadeController.Fade(1));

        ProceduralSlot[] slots = FindObjectsOfType<ProceduralSlot>();

        foreach (ProceduralSlot slot in slots)
        {
            slot.GenerateSprite();
        }

        cameraPivot.position = startPosition;

        yield return new WaitForSeconds(0.5f);

        FindFadeController();

        if (fadeController != null)
            yield return StartCoroutine(fadeController.Fade(0));
        cameraMovement.canMove = true;

        isResetting = false;
    }

    // ---------------- END GAME ----------------
    IEnumerator EndGameSequence()
    {
        Debug.Log("ENDGAME COROUTINE EMPEZO");
        if (gameEnded) yield break;

        gameEnded = true;

        cameraMovement.canMove = false;

        Debug.Log("🔥 END GAME STARTED");

        FindFadeController();

        if (fadeController != null)
            yield return StartCoroutine(fadeController.Fade(1));

        bool isNewRecord =
            runCandies > SaveData.Data.highScore;

        SaveData.LastRunCandies = runCandies;
        SaveData.LastRunWasNewRecord = isNewRecord;

        if (isNewRecord)
        {
            SaveData.Data.highScore = runCandies;
        }

        SaveData.Data.totalCandies += runCandies;
        SaveSystem.Save(SaveData.Data);
        SceneManager.LoadScene("GameOver");

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