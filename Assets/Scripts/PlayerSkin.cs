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

   public void ShowSkinCatUI()
   {
       skinCatUI.SetActive(!skinCatUI.activeSelf);
       textPrice.text = "150";
       selectedSkin = "Cat";
       selectedPrice = 150;
   }

    public void ShowSkinWitchUI()
    {
         skinWitchUI.SetActive(!skinWitchUI.activeSelf);
         textPrice.text = "250";
         selectedSkin = "Witch";
         selectedPrice = 250;
    }

    public void ShowSkinCowgirlUI()
    {
        skinCowgirlUI.SetActive(!skinCowgirlUI.activeSelf);
        textPrice.text = "400";
        selectedSkin = "Cowgirl";
        selectedPrice = 400;
    }
    
    public void ShowSkinGhostUI()
    {
        skinGhostUI.SetActive(!skinGhostUI.activeSelf);
        textPrice.text = "900";
        distorsionUI.SetActive(!distorsionUI.activeSelf);
        selectedSkin = "Ghost";
        selectedPrice = 900;
    }

    public void ShowSkinSkeletonUI()
    {
        skinSkeletonUI.SetActive(!skinSkeletonUI.activeSelf);
        textPrice.text = "900";
        distorsionUI.SetActive(!distorsionUI.activeSelf);
        selectedSkin = "Skeleton";
        selectedPrice = 900;
    }

    public void ShowSkinRedHoodUI()
    {
        skinRedHoodUI.SetActive(!skinRedHoodUI.activeSelf);
        textPrice.text = "1000";
        distorsionUI.SetActive(!distorsionUI.activeSelf);
        selectedSkin = "RedHood";
        selectedPrice = 1000;
    }

    public void ShowSkinPrincessUI()
    {
        skinPrincessUI.SetActive(!skinPrincessUI.activeSelf);
        textPrice.text = "400";
        selectedSkin = "Princess";
        selectedPrice = 400;
    }

    public void ShowSkinFairyUI()
    {
        skinFairyUI.SetActive(!skinFairyUI.activeSelf);
        textPrice.text = "500";
        selectedSkin = "Fairy";
        selectedPrice = 500;
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
}


}