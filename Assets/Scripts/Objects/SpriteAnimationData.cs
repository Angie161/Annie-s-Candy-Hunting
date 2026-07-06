using UnityEngine;

[System.Serializable]
public class SpriteAnimationData
{
    public string animationName;

    public Sprite[] frames;

    public bool loop = true;

    public bool pingPong = true;

    public float frameRate = 0.1f;

    [Header("Audio")]
    public AudioClip soundEffect;
}