using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Reference to the pause menu UI GameObject
    [SerializeField] private GameObject pauseMenuUI;
    
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true); // Show the pause menu UI
        Time.timeScale = 0f; // Pausar el juego estableciendo la escala de tiempo en 0
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false); // Hide the pause menu UI
        Time.timeScale = 1f; // Resume the game by setting time scale back to 1
        // Additional logic to hide pause menu UI can be added here
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Resume the game before loading the main menu
        SceneManager.LoadScene("MainMenu"); // Load the main menu scene
    }

    public void Shop()
    {
        Time.timeScale = 1f; // Resume the game before loading the shop
        SceneManager.LoadScene("Shop"); // Load the shop scene
    }

}
