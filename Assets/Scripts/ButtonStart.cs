using UnityEngine;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMenuMusic();
        }
    }
    public void StartGame()
    {
        // Cargar la escena del juego
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
    public void ExitGame()
    {
        // Salir del juego
        Application.Quit();
    }
}