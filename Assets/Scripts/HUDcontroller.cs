using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public TextMeshProUGUI candyText;

    void Update()
{
    if (GameManager.Instance == null)
    {
        candyText.text = "0";
        return;
    }

    candyText.text =
        GameManager.Instance.runCandies.ToString();
}
}