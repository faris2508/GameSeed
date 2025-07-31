using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Inventory : MonoBehaviour
{
    private Stack<string> foodInventory = new Stack<string>();
    private Stack<Sprite> foodSprites = new Stack<Sprite>(); // Untuk UI inventory
    private int maxInventorySize = 5;
    public Image inventoryImage; // UI Image untuk menampilkan sprite makanan
    public TextMeshProUGUI inventoryText;

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) // Tekan "I" untuk toggle UI (opsional)
        {
            UpdateInventoryUI();
        }
    }

    public void AddItem(string foodName, Sprite foodSprite)
    {
        if (foodInventory.Count < maxInventorySize)
        {
            foodInventory.Push(foodName);
            foodSprites.Push(foodSprite);
            Debug.Log($"Item {foodName} ditambahkan ke inventory.");
            UpdateInventoryUI();
        }
        else
        {
            Debug.Log("Inventory penuh!");
        }
    }

    public string GiveFood()
    {
        if (foodInventory.Count > 0)
        {
            string food = foodInventory.Pop();
            foodSprites.Pop();
            Debug.Log($"Memberikan {food} ke customer.");
            UpdateInventoryUI();
            return food;
        }
        else
        {
            Debug.Log("Inventory kosong!");
            return null;
        }
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
}