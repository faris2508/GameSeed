using UnityEngine;
using TMPro;

public class FoodItem : MonoBehaviour
{
    public string foodName; // Nama makanan, misalnya "Rendang", "Sate", dll.
    public Sprite foodSprite; // Sprite untuk UI inventory
    public GameObject foodPrefab;
    public Vector3 foodScale; // Skala asli makanan di meja
    public TextMeshProUGUI promptText; // Referensi ke UI Text untuk prompt

    private bool isPlayerInRange = false;

    private void Start()
    {
        // Pastikan promptText disembunyikan saat start
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
        foodScale = transform.lossyScale;
        Debug.Log($"FoodItem {foodName} scale: {foodScale}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (promptText != null)
            {
                promptText.text = $"Tekan E untuk mengambil {foodName}";
                promptText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E) && isPlayerInRange)
        {
            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null)
            {
                inventory.AddItem(foodName, foodSprite, foodPrefab, foodScale);
                // Tidak menyembunyikan item atau prompt, biarkan tetap aktif
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (promptText != null)
            {
                promptText.gameObject.SetActive(false); // Sembunyikan prompt saat keluar
            }
        }
    }
}