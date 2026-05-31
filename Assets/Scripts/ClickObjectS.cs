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

    bool IsInsideActiveRange()
    {
        float currentForwardRange = 0f;
        float currentBackwardRange = 0f;

        float zOffset = 0f;

        switch (placementType)
        {
            case ObjectPlacementType.Ceiling:
                currentForwardRange = ceilingForwardRange;
                currentBackwardRange = 0f;
                zOffset = ceilingZOffset;
                break;

            case ObjectPlacementType.Floor:
                currentForwardRange = floorForwardRange;
                currentBackwardRange = 0f;
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

        // DELANTE
        if (relativeZ >= 0)
        {
            return relativeZ <= currentForwardRange;
        }

        // DETRÁS
        else
        {
            return Mathf.Abs(relativeZ)
                <= currentBackwardRange;
        }
    }

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();

        player = GameObject
            .FindGameObjectWithTag("Player")
            .transform;

        rend.color = Color.green;

        StartCoroutine(GenerateStateLoop());
    }

    void Update()
    {
        if (!IsInsideActiveRange())
        {
            if (
                currentState != ObjectState.Normal
                && currentTransition != TransitionState.Recovering
            )
            {
                RecoverToNormal();
            }
        }
    }

    IEnumerator GenerateStateLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 25f));
            if (!IsInsideActiveRange())
            {
                continue;
            }
            if (currentState == ObjectState.Normal
                && currentTransition == TransitionState.None)
            {
                int randomChoice = Random.Range(0, 2);

                if (randomChoice == 0)
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

    void StartAnomaly()
    {
        currentState = ObjectState.Anomaly;

        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        currentAnimation = StartCoroutine(
            ChangeColorSmoothly(Color.red, 2f)
        );
    }

    void StartDistraction()
    {
        currentState = ObjectState.Distraction;

        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        currentAnimation = StartCoroutine(
            ChangeColorSmoothly(Color.orange, 2f)
        );

        StartCoroutine(DistractionLifetime());
    }

    IEnumerator DistractionLifetime()
    {
        yield return new WaitForSeconds(Random.Range(3f, 7f));

        if (currentState == ObjectState.Distraction)
        {
            RecoverToNormal();
        }
    }

    void RecoverToNormal()
    {
        currentState = ObjectState.Normal;

        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        currentAnimation = StartCoroutine(
            ReturnToGreen(1f)
        );
    }

    void TriggerMistake()
    {
        currentState = ObjectState.Mistake;

        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        rend.color = Color.blue;

        currentAnimation = StartCoroutine(
            ReturnToGreen(1f)
        );
    }

    IEnumerator ChangeColorSmoothly(Color targetColor, float duration)
    {
        currentTransition = TransitionState.Transforming;

        float elapsed = 0f;

        Color startColor = rend.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            rend.color = Color.Lerp(
                startColor,
                targetColor,
                elapsed / duration
            );

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
            elapsed += Time.deltaTime;

            rend.color = Color.Lerp(
                startColor,
                Color.green,
                elapsed / duration
            );

            yield return null;
        }

        rend.color = Color.green;

        currentTransition = TransitionState.None;
        currentState = ObjectState.Normal;
    }

    void OnMouseDown()
    {
        if (currentTransition == TransitionState.Recovering)
            return;

        // ANOMALÍA CORRECTA
        if (currentState == ObjectState.Anomaly)
        {
            RecoverToNormal();
        }

        // DISTRACCIÓN O NORMAL
        else if (
            currentState == ObjectState.Normal
            || currentState == ObjectState.Distraction
        )
        {
            TriggerMistake();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                return;
            }
        }

        float currentForwardRange = 0f;
        float currentBackwardRange = 0f;

        float zOffset = 0f;

        switch (placementType)
        {
            case ObjectPlacementType.Ceiling:
                currentForwardRange = ceilingForwardRange;
                currentBackwardRange = 0f;
                zOffset = ceilingZOffset;
                break;

            case ObjectPlacementType.Floor:
                currentForwardRange = floorForwardRange;
                currentBackwardRange = 0f;
                zOffset = floorZOffset;
                break;

            case ObjectPlacementType.Wall:
                currentForwardRange = wallForwardRange;
                currentBackwardRange = wallBackwardRange;
                break;
        }

        Gizmos.color = Color.cyan;

        // DELANTE
        Vector3 forwardCenter =
            player.position
            + Vector3.forward *
            ((currentForwardRange / 2f) - zOffset);

        Gizmos.DrawWireCube(
            forwardCenter,
            new Vector3(3f, 3f, currentForwardRange)
        );

        // DETRÁS
        if (currentBackwardRange > 0f)
        {
            Vector3 backwardCenter =
                player.position
                - Vector3.forward *
                (currentBackwardRange / 2f);

            Gizmos.DrawWireCube(
                backwardCenter,
                new Vector3(3f, 3f, currentBackwardRange)
            );
        }
    }
}