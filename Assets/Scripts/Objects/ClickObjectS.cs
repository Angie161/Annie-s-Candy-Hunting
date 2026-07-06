using UnityEngine;
using System.Collections;

public class ClickObjectS : MonoBehaviour
{
    private SpriteAnimator spriteAnimator;
    private SpriteAnimationData currentAnimation;
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

    //------------------- AUDIO ----------------
    private AudioSource audioSource;

    // ---------------- RANGES ----------------
    private float wallForwardRange;
    private float wallBackwardRange;

    private float flatWallForwardRange;
    private float flatWallBackwardRange;

    private float floorForwardRange;
    private float floorZOffset;

    private float ceilingForwardRange;
    private float ceilingZOffset;

    void Awake(){
        wallForwardRange = 20f;
        wallBackwardRange = 5f;

        flatWallForwardRange = 15f;
        flatWallBackwardRange = 3.5f;

        floorForwardRange = 28f;
        floorZOffset = -10f;

        ceilingForwardRange = 25f;
        ceilingZOffset = -3f;
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        spriteAnimator = GetComponent<SpriteAnimator>();
        slot = GetComponent<ProceduralSlot>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

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
            {
                ProceduralSpriteData data = slot.currentData;

                if (data != null && data.visualType == VisualType.FlatWall)
                {
                    currentForwardRange = flatWallForwardRange;
                    currentBackwardRange = flatWallBackwardRange;
                }
                else
                {
                    currentForwardRange = wallForwardRange;
                    currentBackwardRange = wallBackwardRange;
                }

                break;
            }
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
                int randomChoice = Random.Range(0, 100);

                bool canSpawnAnomaly =
                CountActiveAnomalies() <
                GameManager.Instance.maxActiveAnomalies;

                if (randomChoice < 40 && canSpawnAnomaly)
                {
                    StartAnomaly();
                }
                else
                {
                    StartDistraction();
                }
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

        currentAnimation =
            slot.currentData.anomalies[
                Random.Range(0, slot.currentData.anomalies.Count)
            ];

        spriteAnimator.Play(
            currentAnimation.frames,
            currentAnimation.frameRate,
            currentAnimation.loop,
            currentAnimation.pingPong
        );

        if (currentAnimation.soundEffect != null)
        {
            audioSource.PlayOneShot(
                currentAnimation.soundEffect
            );
        }
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
            spriteAnimator.RecoverToFirstFrame(3f);

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
            spriteAnimator.RecoverToFirstFrame(3f);
        
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

    int CountActiveAnomalies()
    {
        ClickObjectS[] objects =
            FindObjectsByType<ClickObjectS>(
                FindObjectsSortMode.None
            );

        int count = 0;

        foreach (var obj in objects)
        {
            if (obj.currentState ==
                ObjectState.Anomaly)
            {
                count++;
            }
        }

        return count;
    }


void OnDrawGizmosSelected()
{
    if (placementType != ObjectPlacementType.Wall)
        return;

    GameObject p = GameObject.FindGameObjectWithTag("Player");
    if (p == null) return;

    Transform player = p.transform;

    float forward = wallForwardRange;
    float backward = wallBackwardRange;

    float zOffset = 0f; // los walls no usan offset en tu lógica actual

    Vector3 centerForward =
        player.position + new Vector3(0, 0, forward * 0.5f);

    Vector3 centerBackward =
        player.position - new Vector3(0, 0, backward * 0.5f);

    Gizmos.color = Color.cyan;

    // zona adelante
    Gizmos.DrawWireCube(centerForward, new Vector3(10f, 10f, forward));

    // zona atrás
    Gizmos.DrawWireCube(centerBackward, new Vector3(10f, 10f, backward));

    // player reference
    Gizmos.color = Color.red;
    Gizmos.DrawSphere(player.position, 0.15f);
}

}