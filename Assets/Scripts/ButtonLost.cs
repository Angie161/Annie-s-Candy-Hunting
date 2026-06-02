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
}