using UnityEngine;

public enum AnimationType
{
    Hold,       // Intro y se queda en el último frame
    IntroLoop,  // Intro y luego loop del final
    FullLoop    // Loop completo (como las distracciones)
}

[System.Serializable]
public class SpriteAnimationData
{
    public string animationName;

    public Sprite[] frames;

    public float frameRate = 0.1f;
    public float loopFrameRate = 0.15f;

    [Header("Animation")]
    public AnimationType animationType = AnimationType.Hold;

    [Min(1)]
    public int introFrames = 1;

    [Header("Audio")]
    public AudioClip soundEffect;
}