using UnityEngine;
using System.Collections;

public class ClickObjectS : MonoBehaviour
{
    private SpriteRenderer rend;
    private Coroutine currentAnimation;

    public enum ObjectPlacementType
    {
        Wall,
        Ceiling,
        Floor
    }

    private enum ObjectState
    {
        Normal,
        Anomaly,
        Distraction,
        Mistake
    }

    private enum TransitionState
    {
        None,
        Transforming,
        Recovering
    }

    private ObjectState currentState = ObjectState.Normal;
    private TransitionState currentTransition = TransitionState.None;

    private Transform player;

    public ObjectPlacementType placementType;

    public float wallForwardRange = 20f;
    public float wallBackwardRange = 5f;

    public float floorForwardRange = 28f;
    public float floorZOffset = -7f;

    public float ceilingForwardRange = 25f;
    public float ceilingZOffset = -5f;

    public float anomalyProgress = 0f;
    public bool isStressActive = false;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        rend.color = Color.green;

        StartCoroutine(GenerateStateLoop());
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameEnded)
            return;

        if (!IsInsideActiveRange())
        {
            if (currentState != ObjectState.Normal &&
                currentTransition != TransitionState.Recovering)
            {
                RecoverToNormal();
            }
        }
    }

    // ---------------- RANGE CHECK ----------------
    bool IsInsideActiveRange()
    {
        float currentForwardRange = 0f;
        float currentBackwardRange = 0f;
        float zOffset = 0f;

        switch (placementType)
        {
            case ObjectPlacementType.Ceiling:
                currentForwardRange = ceilingForwardRange;
                zOffset = ceilingZOffset;
                break;

            case ObjectPlacementType.Floor:
                currentForwardRange = floorForwardRange;
                zOffset = floorZOffset;
                break;

            case ObjectPlacementType.Wall:
                currentForwardRange = wallForwardRange;
                currentBackwardRange = wallBackwardRange;
                break;
        }

        float relativeZ =
            transform.position.z
            - player.position.z
            + zOffset;

        if (relativeZ >= 0)
        {
            return relativeZ <= currentForwardRange;
        }
        else
        {
            return Mathf.Abs(relativeZ) <= currentBackwardRange;
        }
    }

    // ---------------- STATE LOOP ----------------
    IEnumerator GenerateStateLoop()
    {
        while (true)
        {
            if (GameManager.Instance != null && GameManager.Instance.gameEnded)
                yield break;

            yield return new WaitForSeconds(Random.Range(5f, 25f));

            if (GameManager.Instance != null && GameManager.Instance.gameEnded)
                yield break;

            if (!IsInsideActiveRange())
                continue;

            if (currentState == ObjectState.Normal &&
                currentTransition == TransitionState.None)
            {
                int randomChoice = Random.Range(0, 2);

                if (randomChoice == 0)
                    StartAnomaly();
                else
                    StartDistraction();
            }
        }
    }

    // ---------------- STATES ----------------
    void StartAnomaly()
    {
        currentState = ObjectState.Anomaly;

        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(ChangeColorSmoothly(Color.red, 2f));
    }

    void StartDistraction()
    {
        currentState = ObjectState.Distraction;

        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(ChangeColorSmoothly(Color.orange, 2f));

        StartCoroutine(DistractionLifetime());
    }

    IEnumerator DistractionLifetime()
    {
        yield return new WaitForSeconds(Random.Range(3f, 7f));

        if (currentState == ObjectState.Distraction)
            RecoverToNormal();
    }

    void RecoverToNormal()
    {
        isStressActive = false;
        currentState = ObjectState.Normal;

        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(ReturnToGreen(1f));
    }

    void TriggerMistake()
    {
        currentState = ObjectState.Mistake;

        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        rend.color = Color.blue;

        currentAnimation = StartCoroutine(ReturnToGreen(1f));
    }

    // ---------------- COROUTINES ----------------
    IEnumerator ChangeColorSmoothly(Color targetColor, float duration)
    {
        currentTransition = TransitionState.Transforming;

        float elapsed = 0f;
        Color startColor = rend.color;

        while (elapsed < duration)
        {
            if (GameManager.Instance != null && GameManager.Instance.gameEnded)
                yield break;

            elapsed += Time.deltaTime;

            anomalyProgress = elapsed / duration;

            rend.color = Color.Lerp(startColor, targetColor, anomalyProgress);
            
            if (anomalyProgress >= 0.5f && !isStressActive)
            {
                isStressActive = true;
            }

            yield return null;
        }

        rend.color = targetColor;
        currentTransition = TransitionState.None;
    }

    IEnumerator ReturnToGreen(float duration)
    {
        currentTransition = TransitionState.Recovering;

        float elapsed = 0f;
        Color startColor = rend.color;

        while (elapsed < duration)
        {
            if (GameManager.Instance != null && GameManager.Instance.gameEnded)
                yield break;

            elapsed += Time.deltaTime;

            rend.color = Color.Lerp(startColor, Color.green, elapsed / duration);
            yield return null;
        }

        rend.color = Color.green;
        currentTransition = TransitionState.None;
        currentState = ObjectState.Normal;
    }

    // ---------------- CLICK ----------------
    public void OnClicked()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameEnded)
            return;

        if (currentTransition == TransitionState.Recovering)
            return;

        if (currentState == ObjectState.Anomaly)
        {
            RecoverToNormal();
        }
        else
        {
            TriggerMistake();
        }
    }

    public bool IsInAnomalyState()
    {
        return currentState == ObjectState.Anomaly;
    }
}