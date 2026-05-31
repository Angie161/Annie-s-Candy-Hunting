using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Image blackScreen;
    public Transform cameraPivot;

    public Vector3 startPosition;

    public CameraMovement cameraMovement;
    private bool isResetting = false;

    void Update()
    {
        if (
            !isResetting &&
            cameraPivot.position.z >= cameraMovement.endPoint
        )
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
}