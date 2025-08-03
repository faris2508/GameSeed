using UnityEngine;
using System.Collections;

public class UIWarningController : MonoBehaviour
{
    [Header("UI Tanda Seru")]
    public GameObject leftWarning;
    public GameObject centerWarning;
    public GameObject rightWarning;

    [Header("Settings")]
    public float warningDuration = 2f;
    public float blinkInterval = 0.2f; // kecepatan kedip

    [Header("SFX Settings")]
    public AudioClip warningSFX; // Drag file audio di sini
    public AudioSource audioSource; // Drag AudioSource di sini

    private Coroutine blinkCoroutine;

    public void ShowWarning(int lane)
    {
        HideAllWarnings();

        GameObject warningToShow = null;

        if (lane == 0) warningToShow = leftWarning;
        else if (lane == 1) warningToShow = centerWarning;
        else if (lane == 2) warningToShow = rightWarning;

        if (warningToShow != null)
        {
            // 🔊 Putar SFX (loop)
            if (audioSource != null && warningSFX != null)
            {
                audioSource.clip = warningSFX;
                audioSource.loop = true;
                audioSource.Play();
            }

            // Jalankan blink
            blinkCoroutine = StartCoroutine(BlinkWarning(warningToShow));

            // Auto hilang setelah durasi
            StartCoroutine(HideWarningAfterDelay(warningToShow));
        }
    }

    private IEnumerator BlinkWarning(GameObject warning)
    {
        float elapsed = 0f;
        while (elapsed < warningDuration)
        {
            warning.SetActive(!warning.activeSelf);
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }
        warning.SetActive(true); // pastikan nyala terakhir
    }

    private IEnumerator HideWarningAfterDelay(GameObject warning)
    {
        yield return new WaitForSeconds(warningDuration);

        // Stop blink
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        // Pastikan tanda seru mati
        warning.SetActive(false);

        // 🔇 Stop SFX
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    public void HideAllWarnings()
    {
        if (leftWarning != null) leftWarning.SetActive(false);
        if (centerWarning != null) centerWarning.SetActive(false);
        if (rightWarning != null) rightWarning.SetActive(false);

        // 🔇 Pastikan suara berhenti
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        // Hentikan blink jika ada
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);
    }
}
