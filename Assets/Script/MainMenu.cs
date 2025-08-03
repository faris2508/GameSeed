using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panel Credit")]
    public GameObject creditPanel; // drag panel Credit di inspector

    // Dipanggil saat tombol Start ditekan
    public void StartGame()
    {
        SceneManager.LoadScene("Level1"); 
    }

    // Dipanggil saat tombol Credit ditekan
    public void ShowCredit()
    {
        creditPanel.SetActive(true);
    }

    // Dipanggil saat tombol Back di panel Credit ditekan
    public void CloseCredit()
    {
        creditPanel.SetActive(false);
    }

    // Dipanggil saat tombol Exit ditekan
    public void ExitGame()
    {
        Debug.Log("Keluar Game"); // Supaya kelihatan di editor
        Application.Quit();
    }
}
