using UnityEngine;
using TMPro;

public class PlayerSkin : MonoBehaviour
{
   public void ReturnToLastScene()
   {
       // Cargar la escena anterior
       UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1);
   }

   public void GoToMainMenu()
   {
       // Ir al menú principal
       UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
   }

   [SerializeField] private GameObject skinCatUI;
   [SerializeField] private GameObject skinWitchUI;
   [SerializeField] private GameObject skinCowgirlUI;
   [SerializeField] private GameObject skinGhostUI;
   [SerializeField] private GameObject skinSkeletonUI;
   [SerializeField] private GameObject skinRedHoodUI;
   [SerializeField] private GameObject skinPrincessUI;
   [SerializeField] private GameObject skinFairyUI;

   [SerializeField] private GameObject skinDefaultUI;
   [SerializeField] private GameObject distorsionUI;

   private string selectedSkin = "Default";
   private int selectedPrice = 0;

   [SerializeField] private TMP_Text textPrice;

   private void HideAllSkins()
{
    skinDefaultUI.SetActive(false);
    skinCatUI.SetActive(false);
    skinWitchUI.SetActive(false);
    skinCowgirlUI.SetActive(false);
    skinGhostUI.SetActive(false);
    skinSkeletonUI.SetActive(false);
    skinRedHoodUI.SetActive(false);
    skinPrincessUI.SetActive(false);
    skinFairyUI.SetActive(false);
}

   public void ShowSkinDefaultUI()
   {
       HideAllSkins();
       textPrice.text = "";
       selectedSkin = "Default";
       selectedPrice = 0;
   }

   public void ShowSkinCatUI()
    {
        HideAllSkins();
        skinCatUI.SetActive(!skinCatUI.activeSelf);
        distorsionUI.SetActive(true);

        SetSkinPrice("Cat", 150);
    }
    public void ShowSkinWitchUI()
    {
         HideAllSkins();
         skinWitchUI.SetActive(!skinWitchUI.activeSelf);
         distorsionUI.SetActive(true);
         SetSkinPrice("Witch", 250);
    }

    public void ShowSkinCowgirlUI()
    {
        HideAllSkins();
        skinCowgirlUI.SetActive(!skinCowgirlUI.activeSelf);
        distorsionUI.SetActive(true);
        SetSkinPrice("Cowgirl", 400);
    }
    
    public void ShowSkinGhostUI()
    {
        HideAllSkins();
        skinGhostUI.SetActive(!skinGhostUI.activeSelf);
        distorsionUI.SetActive(false);
        SetSkinPrice("Ghost", 900);
    }

    public void ShowSkinSkeletonUI()
    {
        HideAllSkins();
        skinSkeletonUI.SetActive(!skinSkeletonUI.activeSelf);
        distorsionUI.SetActive(false);
        SetSkinPrice("Skeleton", 900);
    }

    public void ShowSkinRedHoodUI()
    {
        HideAllSkins();
        skinRedHoodUI.SetActive(!skinRedHoodUI.activeSelf);
        distorsionUI.SetActive(false);
        SetSkinPrice("RedHood", 1000);
    }

    public void ShowSkinPrincessUI()
    {
        HideAllSkins();
        skinPrincessUI.SetActive(!skinPrincessUI.activeSelf);
        distorsionUI.SetActive(true);
        SetSkinPrice("Princess", 400);
    }

    public void ShowSkinFairyUI()
    {
        HideAllSkins();
        skinFairyUI.SetActive(!skinFairyUI.activeSelf);
        distorsionUI.SetActive(true);
        SetSkinPrice("Fairy", 500);
    }

    public void BuySkin()
{
    // Ya la compró
    if (SaveData.Data.unlockedSkins.Contains(selectedSkin))
    {
        Debug.Log("Ya tienes esta skin.");
        return;
    }

    // No alcanza el dinero
    if (SaveData.Data.totalCandies < selectedPrice)
    {
        Debug.Log("No tienes suficientes caramelos.");
        return;
    }

    // Descontar caramelos
    SaveData.Data.totalCandies -= selectedPrice;

    // Desbloquear skin
    SaveData.Data.unlockedSkins.Add(selectedSkin);

    // Guardar
    SaveSystem.Save(SaveData.Data);

    // Actualizar la UI del Wallet
    WalletUI wallet = FindFirstObjectByType<WalletUI>();
    if (wallet != null)
        wallet.Refresh();

    Debug.Log("Compraste " + selectedSkin);
    UpdatePriceText();
}

public void WearSkin()
{
    if (!SaveData.Data.unlockedSkins.Contains(selectedSkin))
    {
        Debug.Log("Primero debes comprar esta skin.");
        return;
    }

    SaveData.Data.equippedSkin = selectedSkin;
    SaveSystem.Save(SaveData.Data);

    Debug.Log("Skin equipada: " + selectedSkin);
    UpdatePriceText();
}

private void SetSkinPrice(string skinName, int price)
{
    selectedSkin = skinName;
    selectedPrice = price;

    UpdatePriceText();
}

private void UpdatePriceText()
{
    if (SaveData.Data.equippedSkin == selectedSkin)
    {
        textPrice.text = "Wearing";
    }
    else if (SaveData.Data.unlockedSkins.Contains(selectedSkin))
    {
        textPrice.text = "Owned";
    }
    else
    {
        textPrice.text = selectedPrice.ToString();
    }
}


}