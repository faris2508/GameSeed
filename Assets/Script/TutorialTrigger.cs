using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialTrigger : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject tutorialPanel;   // Panel utama tutorial
    public TextMeshProUGUI dialogText; // Teks dialog tutorial

    [Header("Dialog Settings")]
    [TextArea] public string[] dialogLines; // Isi dialog tutorial
    public float typingSpeed = 0.03f;

    private int currentLineIndex;
    private bool isTyping;
    private bool tutorialActive;
    private Coroutine typingCoroutine;

    private void Start()
    {
        tutorialPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !tutorialActive)
        {
            StartTutorial();
        }
    }

    public void StartTutorialExternally()
    {
        if (!tutorialActive)
        {
            StartTutorial();
        }
    }

    void StartTutorial()
    {
        tutorialActive = true;
        PauseManager.TutorialActive = true; // Kasih tahu PauseManager kalau tutorial aktif
        currentLineIndex = 0;
        tutorialPanel.SetActive(true);

        // Pause game untuk fokus tutorial
        Time.timeScale = 0f;

        ShowLine();
    }

    void Update()
    {
        if (!tutorialActive) return;

        // Kalau sedang pause menu → jangan respon tombol space tutorial
        if (PauseManager.IsPaused) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // Skip animasi ketik
                StopCoroutine(typingCoroutine);
                dialogText.text = dialogLines[currentLineIndex];
                isTyping = false;
            }
            else
            {
                // Lanjut ke dialog berikut
                currentLineIndex++;
                if (currentLineIndex < dialogLines.Length)
                {
                    ShowLine();
                }
                else
                {
                    EndTutorial();
                }
            }
        }
    }

    void ShowLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(dialogLines[currentLineIndex]));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed); // Tetap jalan meskipun timescale = 0
        }

        isTyping = false;
    }

    void EndTutorial()
    {
        tutorialActive = false;
        PauseManager.TutorialActive = false; // Kasih tahu PauseManager kalau tutorial sudah selesai

        // Kalau tidak sedang pause menu, baru jalankan game
        if (!PauseManager.IsPaused)
            Time.timeScale = 1f;

        tutorialPanel.SetActive(false);
        Destroy(gameObject);
    }
}
