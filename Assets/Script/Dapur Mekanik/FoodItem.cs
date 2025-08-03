using UnityEngine;
using TMPro;

public class FoodItem : MonoBehaviour
{
    public string foodName;
    public Sprite foodSprite;
    public GameObject foodPrefab;
    public Vector3 foodScale;
    public TextMeshProUGUI promptText;

    private bool isPlayerInRange = false;
    private Inventory playerInventory;

    private void Start()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);

        foodScale = transform.lossyScale;
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && playerInventory != null)
        {
            playerInventory.AddItem(foodName, foodSprite, foodPrefab, foodScale);
            // prompt tetap muncul jika kamu tidak ingin sembunyikan
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerInventory = other.GetComponent<Inventory>();

            if (promptText != null)
            {
                promptText.text = $"Tekan E untuk mengambil {foodName}";
                promptText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerInventory = null;

            if (promptText != null)
                promptText.gameObject.SetActive(false);
        }
    }
}