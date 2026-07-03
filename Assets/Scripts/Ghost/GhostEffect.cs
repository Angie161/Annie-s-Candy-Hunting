using UnityEngine;

public class GhostEffect : MonoBehaviour
{
    private SpriteAnimator animator;

    public SpriteAnimationData ghostAnimation;

    void Awake()
    {
        animator = GetComponent<SpriteAnimator>();

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.enabled = false;
    }

    public void Play()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.enabled = true;

        if (animator == null || ghostAnimation == null)
            return;

        animator.Play(
            ghostAnimation.frames,
            ghostAnimation.frameRate,
            ghostAnimation.loop,
            ghostAnimation.pingPong
        );

        StartCoroutine(DestroyAfter());
    }

    System.Collections.IEnumerator DestroyAfter()
    {
        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
    }
}