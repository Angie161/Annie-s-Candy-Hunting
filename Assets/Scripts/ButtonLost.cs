using UnityEngine;

public class GameOver : MonoBehaviour
{
    public void StartGame()
    {
        // Cargar la escena del juego
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
    public void GoToMenu()
    {
        // Ir al menú principal
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    public void GoToCredits()
    {
        // Ir a la pantalla de créditos
        UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
    }

    public void GoToShop()
    {
        // Ir a la tienda
        UnityEngine.SceneManagement.SceneManager.LoadScene("Shop");
    }

    public void HowToPlay()
    {
        // Ir a la pantalla de cómo jugar
        UnityEngine.SceneManagement.SceneManager.LoadScene("HowToPlay");
    }

    public void ExitGame()
    {
        // Salir del juego
        Application.Quit();
    }
}