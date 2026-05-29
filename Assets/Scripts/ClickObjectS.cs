using UnityEngine;
using System.Collections;

public class ClickObjectS : MonoBehaviour
{
    private SpriteRenderer rend;
    private Coroutine currentAnimation;

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

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();

        rend.color = Color.green;

        StartCoroutine(GenerateStateLoop());
    }

    IEnumerator GenerateStateLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 25f));

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
}