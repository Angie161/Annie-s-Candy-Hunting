using UnityEngine;


public class PlayerSkinLoader : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        string skin = PlayerPrefs.GetString("CurrentSkin", "Default");

        switch (skin)
        {
            case "Cat":
                animator.Play("CatWalking");
                break;

            case "Witch":
                animator.Play("WitchWalking");
                break;

            case "Cowgirl":
                animator.Play("CowgirlWalking");
                break;

            case "Ghost":
                animator.Play("GhostWalking");
                break;

            case "Skeleton":
                animator.Play("SkeletonWalking");
                break;

            case "RedHood":
                animator.Play("RedHoodWalking");
                break;

            case "Princess":
                animator.Play("PrincessWalking");
                break;

            case "Fairy":
                animator.Play("FairyWalking");
                break;

            default:
                animator.Play("Walking");
                break;
        }
    }
}
