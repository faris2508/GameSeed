using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused;         // Status pause menu
    public static bool TutorialActive;   // Status tutorial aktif

    public GameObject pausePanel;

    void Update()
    {
        // Tekan ESC untuk toggle pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        IsPaused = false;

        // Kalau tutorial masih aktif → tetap 0, biar tutorial dulu yang jalan
        if (!TutorialActive)
        {
            Time.timeScale = 1f;
        }
    }
}
