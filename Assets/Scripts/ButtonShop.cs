using UnityEngine;

public class ButtonShop : MonoBehaviour
{
    [SerializeField] private Animator previewAnimator;
    [SerializeField] private RuntimeAnimatorController[] skinControllers;

    [SerializeField] private ButtonSkin[] ButtonSkins;
    
    private ButtonSkin currentButton;

    private int selectedSkin = 0;

    public void GoToLastScene()
    {
        // Cargar la escena anterior
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1);
    }

    private void Start()
{
    selectedSkin = PlayerPrefs.GetInt("SelectedSkin", 0);
    previewAnimator.runtimeAnimatorController = skinControllers[selectedSkin];

    currentButton = ButtonSkins[selectedSkin];
    currentButton.SetSelected(true);
}

    public void PreviewSkin(int skinIndex)
{
    if (currentButton != null)
        currentButton.SetSelected(false);

    currentButton = ButtonSkins[skinIndex];
    currentButton.SetSelected(true);

    selectedSkin = skinIndex;
    previewAnimator.runtimeAnimatorController = skinControllers[skinIndex];
}

    // Se llama al presionar el botón Wear
    public void WearSkin()
    {
        PlayerPrefs.SetInt("SelectedSkin", selectedSkin);
        PlayerPrefs.Save();

        Debug.Log("Skin equipada: " + selectedSkin);
    }
}