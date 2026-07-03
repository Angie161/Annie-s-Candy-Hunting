using UnityEngine;
using System.Collections;

public class ClickObjectS : MonoBehaviour
{
    private SpriteAnimator spriteAnimator;
    private ProceduralSlot slot;

    public GameObject ghostPrefab;
    [Header("Ghost")]
    public Vector3 ghostOffset = new Vector3(0f, 0.2f, -0.1f);

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

    private float wallForwardRange;
    private float wallBackwardRange;

    private float floorForwardRange;
    private float floorZOffset;

    private float ceilingForwardRange;
    private float ceilingZOffset;

    void Awake(){
        wallForwardRange = 20f;
        wallBackwardRange = 5f;

        floorForwardRange = 28f;
        floorZOffset = -7f;

        ceilingForwardRange = 25f;
        ceilingZOffset = -3f;
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        spriteAnimator = GetComponent<SpriteAnimator>();
        slot = GetComponent<ProceduralSlot>();

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

        if (slot == null || slot.currentData == null)
            return;

        if (slot.currentData.anomalies.Count == 0)
            return;
        
        currentState = ObjectState.Anomaly;

        SpriteAnimationData anim =
            slot.currentData.anomalies[
                Random.Range(0, slot.currentData.anomalies.Count)
            ];

        spriteAnimator.Play(
            anim.frames,
            anim.frameRate,
            anim.loop,
            anim.pingPong
        );
    }

    void StartDistraction()
    {
        if (slot == null || slot.currentData == null)
            return;

        if (slot.currentData.distractions.Count == 0)
            return;

        currentState = ObjectState.Distraction;

        SpriteAnimationData anim =
            slot.currentData.distractions[
                Random.Range(0, slot.currentData.distractions.Count)
            ];

        spriteAnimator.Play(
            anim.frames,
            anim.frameRate,
            anim.loop,
            anim.pingPong
        );

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
        if (currentTransition == TransitionState.Recovering)
            return;

        currentTransition = TransitionState.Recovering;

        currentState = ObjectState.Normal;

        if (spriteAnimator != null)
            spriteAnimator.RecoverToFirstFrame();

        StartCoroutine(ResetTransition());
    }

    IEnumerator ResetTransition()
    {
        // espera a que termine la animación de recovery
        yield return new WaitForSeconds(0.7f);

        currentTransition = TransitionState.None;
    }

    void TriggerMistake()
    {
        currentState = ObjectState.Mistake;
        currentTransition = TransitionState.Recovering;

        Sustometer sustometer = FindFirstObjectByType<Sustometer>();

        if (sustometer != null)
        {
            sustometer.AddMistake();
        }

        SpawnGhost();

        if (spriteAnimator != null)
            spriteAnimator.RecoverToFirstFrame();
        
        StartCoroutine(ResetTransition());
    }

  void SpawnGhost()
    {
        if (ghostPrefab == null)
            return;

        GameObject ghost = Instantiate(
            ghostPrefab,
            transform.position + ghostOffset,
            Quaternion.identity
        );

        SpriteRenderer sr = ghost.GetComponent<SpriteRenderer>();

        if (sr != null)
            sr.sortingOrder = 999; // siempre delante

        GhostEffect effect = ghost.GetComponent<GhostEffect>();

        if (effect != null)
            effect.Play();
    }

    // ---------------- CLICK ----------------
    public void OnClicked()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameEnded)
            return;

        // 🚨 BLOQUEO TOTAL DURANTE TRANSICIÓN
        if (currentTransition != TransitionState.None)
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