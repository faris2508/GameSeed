using UnityEngine;
using UnityEngine.Playables; // Untuk mengontrol Timeline
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    [Header("References")]
    public PlayableDirector timeline; // Timeline intro
    public VideoPlayer videoPlayer;   // Video Player di RawImage
    public GameObject skipSprite;     // UI ikon skip

    [Header("Settings")]
    public string nextSceneName = "Level1";
    public float skipDelay = 3f; // Berapa detik sebelum tombol skip aktif

    private bool canSkip = false;
    private bool introFinished = false;

    void Start()
    {
        // Pastikan skip sprite awalnya tidak muncul
        skipSprite.SetActive(false);

        // Play video dan timeline otomatis
        if (videoPlayer != null)
            videoPlayer.Play();

        if (timeline != null)
            timeline.Play();

        // Mulai delay untuk bisa skip
        StartCoroutine(EnableSkipAfterDelay(skipDelay));

        // Listener saat timeline selesai
        if (timeline != null)
            timeline.stopped += OnTimelineFinished;
    }

    IEnumerator EnableSkipAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        canSkip = true;
        skipSprite.SetActive(true);
    }

    void Update()
    {
        if (canSkip && Input.GetKeyDown(KeyCode.Space) && !introFinished)
        {
            SkipIntro();
        }
    }

    void SkipIntro()
    {
        introFinished = true;

        // Stop timeline dan video
        if (timeline != null)
            timeline.time = timeline.duration; // Lompat ke akhir

        if (videoPlayer != null)
            videoPlayer.Stop();

        // Pindah scene langsung
        SceneManager.LoadScene(nextSceneName);
    }

    void OnTimelineFinished(PlayableDirector pd)
    {
        if (!introFinished) // Biar tidak double load
        {
            introFinished = true;
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
