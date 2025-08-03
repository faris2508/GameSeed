using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Inventory : MonoBehaviour
{
    private Stack<string> foodInventory = new Stack<string>();
    private Stack<Sprite> foodSprites = new Stack<Sprite>();
    private Stack<GameObject> foodPrefabs = new Stack<GameObject>();
    private Stack<Vector3> foodScales = new Stack<Vector3>();
    private int maxInventorySize = 5;
    public Image inventoryImage;
    public TextMeshProUGUI inventoryText;

    public AudioClip smashSound;

    public float throwScaleMultiplier = 20f;
    public float stackScaleMultiplier = 1f;

    public Vector3 rotationOffset = new Vector3(-90f, 0f, 0f);
    public float spawnHeightOffset = 3f;
    public Image arrowImage;
    public float throwForce = 10f;
    public int points = 0;
    public TextMeshProUGUI pointsText;

    public Transform foodStackParent;
    private List<GameObject> foodStackObjects = new List<GameObject>();
    public float stackHeightOffset = 0.05f;
    public Vector3 baseStackPosition = new Vector3(0.5f, 0, 0.5f);
    public Vector3 baseStackRotation = Vector3.zero;
    public float horizontalStackOffset = 0.3f;
    private int maxPlatesPerStack = 2;
    private Quaternion plateRotation = Quaternion.Euler(-90f, 0, 0);

    void Start()
    {
        // ... (kode yang sudah ada)
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

        // Terapkan rotasi dasar tumpukan ke objek parent
        if (foodStackParent != null)
        {
            foodStackParent.localRotation = Quaternion.Euler(baseStackRotation);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            UpdateInventoryUI();
        }

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

            SpawnFoodPlate(foodPrefab, foodScale);

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

            Vector3 spawnPosition = transform.position + transform.forward * 1f + Vector3.up * spawnHeightOffset;
            GameObject thrownFood = Instantiate(prefab, spawnPosition, Quaternion.Euler(rotationOffset));
            thrownFood.transform.localScale = scale * throwScaleMultiplier;
            Rigidbody rb = thrownFood.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = thrownFood.AddComponent<Rigidbody>();
            }
            rb.useGravity = false;
            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            rb.linearVelocity = transform.forward * throwForce;

            ThrownFood thrownFoodScript = thrownFood.AddComponent<ThrownFood>();
            thrownFoodScript.inventory = this;
            thrownFoodScript.SetSmashSound(smashSound);

            if (foodStackObjects.Count > 0)
            {
                GameObject topPlate = foodStackObjects[foodStackObjects.Count - 1];
                Destroy(topPlate);
                foodStackObjects.RemoveAt(foodStackObjects.Count - 1);
            }

            PlayerMovement playerMovement = GetComponent<PlayerMovement>();
            if (playerMovement != null && playerMovement.animator != null)
            {
                // 🔥 Picu animasi lempar!
                playerMovement.animator.SetTrigger("ThrowItem");
                
                // Jika ini adalah item terakhir
                if (foodInventory.Count == 0)
                {
                    playerMovement.animator.SetBool("isCarryingIdle", false);
                    playerMovement.animator.SetBool("isCarryingWalk", false);
                    playerMovement.animator.SetBool("isCarryingIdleWalk", false);
                    playerMovement.animator.SetBool("isIdle", true);
                }
            }


            UpdateInventoryUI();
        }
    }

    private void SpawnFoodPlate(GameObject foodPrefab, Vector3 foodScale)
    {
        int currentCount = foodStackObjects.Count;
        int stackIndex = currentCount / maxPlatesPerStack;
        int plateIndexInStack = currentCount % maxPlatesPerStack;

        Vector3 stackPosition = baseStackPosition;
        if (stackIndex == 1) {
            stackPosition += new Vector3(horizontalStackOffset, 0, 0);
        } else if (stackIndex == 2) {
            stackPosition += new Vector3(horizontalStackOffset, stackHeightOffset * 2, 0);
        }

        Vector3 platePosition = new Vector3(0, plateIndexInStack * stackHeightOffset, 0);
        if (stackIndex == 2) {
            platePosition = Vector3.zero;
        }

        // Hitung posisi akhir relatif terhadap parent, tanpa rotasi yang rumit
        Vector3 spawnPosition = stackPosition + platePosition;

        // Instantiate piring sebagai anak dari FoodStackParent
        GameObject newPlate = Instantiate(foodPrefab, foodStackParent);
        newPlate.transform.localPosition = spawnPosition;
        newPlate.transform.localRotation = plateRotation;
        newPlate.transform.localScale = foodScale * stackScaleMultiplier;
        
        Rigidbody rb = newPlate.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Destroy(rb);
        }
        
        Collider col = newPlate.GetComponent<Collider>();
        if (col != null)
        {
            Destroy(col);
        }

        foodStackObjects.Add(newPlate);
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
                inventoryImage.sprite = foodSprites.Peek();
                inventoryImage.gameObject.SetActive(true);
            }
            else
            {
                inventoryImage.gameObject.SetActive(false);
            }
        }

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
                inventoryText.gameObject.SetActive(false);
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

    public bool isCarryingItem
    {
        get { return foodInventory.Count > 0; }
    }

    public void ClearInventory()
    {
        foodInventory.Clear();
        foodSprites.Clear();
        foodPrefabs.Clear();
        foodScales.Clear();

        foreach (GameObject plate in foodStackObjects)
        {
            if (plate != null)
                Destroy(plate);
        }
        foodStackObjects.Clear();

        if (inventoryImage != null) inventoryImage.gameObject.SetActive(false);
        if (inventoryText != null) inventoryText.gameObject.SetActive(false);
        if (arrowImage != null) arrowImage.gameObject.SetActive(false);
    }
}