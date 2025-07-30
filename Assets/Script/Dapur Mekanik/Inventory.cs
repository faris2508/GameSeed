using UnityEngine;
using TMPro; // Jika menggunakan TextMeshPro
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    private Stack<string> foodInventory = new Stack<string>();
    private int maxInventorySize = 5;
    public TextMeshProUGUI inventoryText; // Referensi ke UI Text di Inspector

    void Start()
    {
        UpdateInventoryUI(); // Perbarui UI saat game dimulai
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) // Tekan "I" untuk toggle UI (opsional)
        {
            UpdateInventoryUI();
        }
    }

    public void AddItem(string foodName)
    {
        if (foodInventory.Count < maxInventorySize)
        {
            foodInventory.Push(foodName);
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
        if (inventoryText != null)
        {
            string inventoryList = "Inventory:\n";
            foreach (string food in foodInventory)
            {
                inventoryList += food + "\n";
            }
            inventoryText.text = inventoryList;
        }
    }
}