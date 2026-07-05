using UnityEngine;
using TMPro;

public class WalletUI : MonoBehaviour
{
    public TextMeshProUGUI walletText;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        walletText.text = SaveData.Data.totalCandies.ToString();
    }
}