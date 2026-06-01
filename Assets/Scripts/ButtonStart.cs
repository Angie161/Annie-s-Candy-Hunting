using UnityEngine;

public class MainMenu : MonoBehaviour
{
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