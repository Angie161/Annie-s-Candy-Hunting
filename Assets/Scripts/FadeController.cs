using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
    [SerializeField] private Image blackScreen;

    public IEnumerator Fade(float targetAlpha, float duration = 0.5f)
    {
        if (blackScreen == null)
            yield break;

        float elapsed = 0f;

        Color start = blackScreen.color;
        Color target = blackScreen.color;
        target.a = targetAlpha;

        while (elapsed < duration)
        {
            if (blackScreen == null)
                yield break;

            elapsed += Time.deltaTime;

            blackScreen.color = Color.Lerp(
                start,
                target,
                elapsed / duration
            );

            yield return null;
        }

        if (blackScreen != null)
            blackScreen.color = target;
    }
}