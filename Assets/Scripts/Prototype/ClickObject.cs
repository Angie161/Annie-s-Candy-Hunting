using UnityEngine;
using System.Collections;

public class ClickObject : MonoBehaviour
{
    private Renderer rend;

    private Coroutine currentAnimation;

    private enum State
    {
        Normal,
        TurningRed,
        Recovering
    }

    private State currentState = State.Normal;

    void Start()
    {
        rend = GetComponent<Renderer>();

        rend.material.color = Color.green;

        StartCoroutine(GenerateAnomalyLoop());
    }

    IEnumerator GenerateAnomalyLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 6f));

            if (currentState == State.Normal)
            {
                if (currentAnimation != null)
                {
                    StopCoroutine(currentAnimation);
                }

                currentAnimation = StartCoroutine(TurnRedSlowly());
            }
        }
    }

    IEnumerator TurnRedSlowly()
    {
        currentState = State.TurningRed;

        float duration = 2f;
        float elapsed = 0f;

        Color startColor = rend.material.color;
        Color targetColor = Color.red;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            rend.material.color = Color.Lerp(
                startColor,
                targetColor,
                elapsed / duration
            );

            yield return null;
        }

        rend.material.color = Color.red;
    }

    IEnumerator ReturnToGreen(float duration)
    {
        currentState = State.Recovering;

        float elapsed = 0f;

        Color startColor = rend.material.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            rend.material.color = Color.Lerp(
                startColor,
                Color.green,
                elapsed / duration
            );

            yield return null;
        }

        rend.material.color = Color.green;

        currentState = State.Normal;
    }

    void OnMouseDown()
{
    if (currentState == State.Recovering)
        return;

    if (currentAnimation != null)
    {
        StopCoroutine(currentAnimation);
    }

    Color currentColor = rend.material.color;

    bool anomalyVisible =
        currentColor.r > 0;

    if (anomalyVisible)
    {
        currentAnimation = StartCoroutine(ReturnToGreen(0.9f));
    }
    else
    {
        rend.material.color = Color.blue;

        currentAnimation = StartCoroutine(ReturnToGreen(0.8f));
    }
}
}