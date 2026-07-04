using UnityEngine;
using System.Collections;

public class SpriteAnimator : MonoBehaviour
{
    private SpriteRenderer rend;
    private Coroutine currentAnimation;
    private Sprite originalSprite;

    private Sprite[] currentFrames;

    private int currentFrame = 0;

    private bool playingForward = true;
    private float currentFrameRate = 0.1f;

    private PolygonCollider2D polygonCollider;
    private BoxCollider boxCollider;

    void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    public void SetOriginalSprite(Sprite sprite)
    {
        originalSprite = sprite;
    }

    public void Play(Sprite[] frames, float frameRate, bool loop, bool pingPong)
    {
        if (frames == null || frames.Length == 0)
            return;

        if (frames.Length == 1)
        {
            rend.sprite = frames[0];
            UpdateCollider();
            return;
        }

        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentFrames = frames;
        currentFrameRate = frameRate;

        currentAnimation = StartCoroutine(
            PlayRoutine(loop, pingPong)
        );
    }

    IEnumerator PlayRoutine(bool loop, bool pingPong)
    {
        currentFrame = 0;
        playingForward = true;

        while (true)
        {
            rend.sprite = currentFrames[currentFrame];
            UpdateCollider();

            yield return new WaitForSeconds(currentFrameRate);

            if (pingPong)
            {
                if (playingForward)
                {
                    currentFrame++;

                    if (currentFrame >= currentFrames.Length)
                    {
                        currentFrame = currentFrames.Length - 2;
                        playingForward = false;
                    }
                }
                else
                {
                    currentFrame--;

                    if (currentFrame < 0)
                    {
                        if (!loop)
                        {
                            currentFrame = 0;
                            yield break;
                        }

                        currentFrame = 1;
                        playingForward = true;
                    }
                }
            }
            else
            {
                currentFrame++;

                if (currentFrame >= currentFrames.Length)
                {
                    if (!loop)
                    {
                        currentFrame = currentFrames.Length - 1;
                        yield break;
                    }

                    currentFrame = 0;
                }
            }
        }
    }

    public void RecoverToFirstFrame(float speedMultiplier = 1f)
    {
        if (currentFrames == null)
            return;

        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation =
            StartCoroutine(
                RecoverRoutine(speedMultiplier)
            );
    }

    IEnumerator RecoverRoutine(float speedMultiplier)
    {
        while (currentFrame > 0)
        {
            currentFrame--;
            rend.sprite = currentFrames[currentFrame];
            UpdateCollider();

            yield return new WaitForSeconds(currentFrameRate/speedMultiplier);
        }
        if (originalSprite != null)
        {
            rend.sprite = originalSprite;
            UpdateCollider();
        }

        currentAnimation = null;
        playingForward = true;
    }

    void UpdateCollider()
    {
        polygonCollider = GetComponent<PolygonCollider2D>();
        boxCollider = GetComponent<BoxCollider>();

        if (polygonCollider != null)
        {
            polygonCollider.pathCount = 0;

            Sprite sprite = rend.sprite;

            if (sprite == null)
                return;

            int shapeCount = sprite.GetPhysicsShapeCount();

            polygonCollider.pathCount = shapeCount;

            for (int i = 0; i < shapeCount; i++)
            {
                var points = new System.Collections.Generic.List<Vector2>();

                sprite.GetPhysicsShape(i, points);

                polygonCollider.SetPath(i, points);
            }
        }

        if (boxCollider != null)
        {
            boxCollider.center = Vector3.zero;
            boxCollider.size = rend.sprite.bounds.size;
        }
    }

    /*

    public void TestPlay(Sprite[] frames)
    {
        Play(frames, 0.1f, true, true);
    }*/ 
}