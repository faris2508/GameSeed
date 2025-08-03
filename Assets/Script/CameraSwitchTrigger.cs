using UnityEngine;

public class CameraTriggerSwitch : MonoBehaviour
{
    [Header("Camera Settings")]
    public GameObject mainCamera; // Kamera utama
    public GameObject fixCamera;  // Kamera fix (posisi tertentu)

    private void OnTriggerEnter(Collider other)
    {
        // Pastikan yang masuk adalah Player
        if (other.CompareTag("Player"))
        {
            if (mainCamera != null && fixCamera != null)
            {
                mainCamera.SetActive(false);
                fixCamera.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Saat Player keluar trigger
        if (other.CompareTag("Player"))
        {
            if (mainCamera != null && fixCamera != null)
            {
                mainCamera.SetActive(true);
                fixCamera.SetActive(false);
            }
        }
    }
}
