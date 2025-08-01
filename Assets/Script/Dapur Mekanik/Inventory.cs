using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Inventory : MonoBehaviour
{
    private Stack<string> foodInventory = new Stack<string>();
    private Stack<Sprite> foodSprites = new Stack<Sprite>(); // Untuk UI inventory
    private Stack<GameObject> foodPrefabs = new Stack<GameObject>();
    private Stack<Vector3> foodScales = new Stack<Vector3>(); // Untuk skala makanan
    private int maxInventorySize = 5;
    public Image inventoryImage; // UI Image untuk menampilkan sprite makanan
    public TextMeshProUGUI inventoryText;

    public float scaleMultiplier = 20f; // Tambahkan di Inspector
    public Vector3 rotationOffset = new Vector3(-90f, 0f, 0f); // Rotasi manual
    public float spawnHeightOffset = 3f;
    public Image arrowImage; // UI Image untuk panah
    public float throwForce = 10f; // Kekuatan lontaran
    public int points = 0; // Poin pemain
    public TextMeshProUGUI pointsText; // UI Text untuk menampilkan poin

    void Start()
    {
        // Sembunyikan UI Image saat start
        if (inventoryImage != null)
        {
            inventoryImage.gameObject.SetActive(false);
        }
        if (inventoryText != null)
        {
            inventoryText.gameObject.SetActive(false);
        }
        if (arrowImage != null)
        {
            arrowImage.gameObject.SetActive(false);
        }
        UpdatePointsUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) // Tekan "I" untuk toggle UI (opsional)
        {
            UpdateInventoryUI();
        }

        // // Update rotasi panah agar sejajar dengan pemain
        // if (arrowImage != null && foodInventory.Count > 0)
        // {
        //     arrowImage.gameObject.SetActive(true);
        //     // Asumsikan pemain menghadap ke arah transform.forward
        //     Vector3 direction = transform.forward;
        //     Vector2 screenPoint = Camera.main.WorldToScreenPoint(transform.position + direction);
        //     arrowImage.rectTransform.position = screenPoint;
        //     float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg - 90f;
        //     arrowImage.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        // }
        // else if (arrowImage != null)
        // {
        //     arrowImage.gameObject.SetActive(false);
        // }

        // Lempar makanan dengan tombol "F"
        if (Input.GetKeyDown(KeyCode.F) && foodInventory.Count > 0)
        {
            ThrowFood();
        }
    }

    public void AddItem(string foodName, Sprite foodSprite, GameObject foodPrefab, Vector3 foodScale)
    {
        if (foodInventory.Count < maxInventorySize)
        {
            foodInventory.Push(foodName);
            foodSprites.Push(foodSprite);
            foodPrefabs.Push(foodPrefab);
            foodScales.Push(foodScale);
            Debug.Log($"Item {foodName} ditambahkan ke inventory.");
            UpdateInventoryUI();
        }
        else
        {
            Debug.Log("Inventory penuh!");
        }
    }

    private void ThrowFood()
    {
        if (foodInventory.Count > 0)
        {
            string food = foodInventory.Pop();
            GameObject prefab = foodPrefabs.Pop();
            Vector3 scale = foodScales.Pop();
            foodSprites.Pop();
            Debug.Log($"Melontarkan {food} dengan skala: {scale}, rotasi: {rotationOffset}");

            // Instantiate makanan di depan pemain
            Vector3 spawnPosition = transform.position + transform.forward * 1f + Vector3.up * spawnHeightOffset;
            GameObject thrownFood = Instantiate(prefab, spawnPosition, Quaternion.Euler(rotationOffset));
            thrownFood.transform.localScale = scale * scaleMultiplier; // Terapkan skala asli dengan pengali
            Rigidbody rb = thrownFood.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = thrownFood.AddComponent<Rigidbody>();
            }
            rb.useGravity = false; // Nonaktifkan gravitasi
            rb.linearDamping = 0f; // Ganti drag dengan linearDamping
            rb.angularDamping = 0f; // Ganti angularDrag dengan angularDamping
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY; // Kunci rotasi dan posisi Y
            rb.linearVelocity = transform.forward * throwForce; // Gunakan velocity untuk gerakan lurus

            // Tambahkan script untuk mendeteksi tabrakan
            ThrownFood thrownFoodScript = thrownFood.AddComponent<ThrownFood>();
            thrownFoodScript.inventory = this;

            UpdateInventoryUI();
        }
    }

    public void AddPoints(int amount)
    {
        points += amount;
        Debug.Log($"Poin bertambah: {amount}. Total poin: {points}");
        UpdatePointsUI();
    }

    private void UpdateInventoryUI()
    {
        if (inventoryImage != null)
        {
            if (foodSprites.Count > 0)
            {
                inventoryImage.sprite = foodSprites.Peek(); // Ambil sprite teratas
                inventoryImage.gameObject.SetActive(true);
            }
            else
            {
                inventoryImage.gameObject.SetActive(false); // Sembunyikan jika inventory kosong
            }
        }

        // Update UI Text untuk daftar makanan
        if (inventoryText != null)
        {
            if (foodInventory.Count > 0)
            {
                string inventoryList = "Inventory:\n";
                foreach (string food in foodInventory)
                {
                    inventoryList += food + "\n";
                }
                inventoryText.text = inventoryList;
                inventoryText.gameObject.SetActive(true);
            }
            else
            {
                inventoryText.gameObject.SetActive(false); // Sembunyikan jika inventory kosong
            }
        }
    }
    
    private void UpdatePointsUI()
    {
        if (pointsText != null)
        {
            pointsText.text = $"Poin: {points}";
        }
    }
}