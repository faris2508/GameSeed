using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [Header("Video Settings")]
    public VideoPlayer videoPlayer;
    public string nextSceneName = "level1";

    [Header("Skip Settings")]
    public float skipDelay = 5f; // Waktu tunggu sebelum bisa skip
    public GameObject skipUI; // UI tanda skip

    private bool canSkip = false;

    void Start()
    {
        // Pastikan skip UI awalnya mati
        if (skipUI != null)
            skipUI.SetActive(false);

        // Event ketika video selesai
        videoPlayer.loopPointReached += OnVideoFinished;

        // Aktifkan skip setelah skipDelay detik
        Invoke(nameof(EnableSkip), skipDelay);
    }

    void Update()
    {
        // Jika boleh skip dan Space ditekan
        if (canSkip && Input.GetKeyDown(KeyCode.Space))
        {
            LoadNextScene();
        }
    }

    void EnableSkip()
    {
        canSkip = true;

        // Tampilkan UI tanda skip
        if (skipUI != null)
            skipUI.SetActive(true);

        Debug.Log("Skip sudah aktif. Tekan Space untuk skip cutscene.");
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        LoadNextScene();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
