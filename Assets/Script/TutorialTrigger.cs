using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialTrigger : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject tutorialPanel;   // Panel utama (berisi icon + dialog box + text)
    public TextMeshProUGUI dialogText; // Text dialog

    [Header("Dialog Settings")]
    [TextArea] public string[] dialogLines; // Isi dialog tutorial
    public float typingSpeed = 0.03f;

    private int currentLineIndex;
    private bool isTyping;
    private bool tutorialActive;
    private Coroutine typingCoroutine;

    private void Start()
    {
        tutorialPanel.SetActive(false); // Pastikan panel off di awal
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !tutorialActive)
        {
            StartTutorial();
        }
    }

    void StartTutorial()
    {
        tutorialActive = true;
        currentLineIndex = 0;
        tutorialPanel.SetActive(true);

        // Pause game
        Time.timeScale = 0f;

        ShowLine();
    }

    public void StartTutorialExternally()
    {
        if (!tutorialActive)
        {
            StartTutorial();
        }
    }

    void Update()
    {
        if (!tutorialActive) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // Skip typing langsung tampil full text
                StopCoroutine(typingCoroutine);
                dialogText.text = dialogLines[currentLineIndex];
                isTyping = false;
            }
            else
            {
                // Lanjut ke dialog berikutnya
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

        // Supaya typing tetap jalan walau Time.timeScale = 0 → pakai realtimeWait
        foreach (char letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
    }

    void EndTutorial()
    {
        // Resume game
        Time.timeScale = 1f;

        tutorialPanel.SetActive(false);
        Destroy(gameObject); // Hapus trigger agar tidak muncul lagi
    }
}
