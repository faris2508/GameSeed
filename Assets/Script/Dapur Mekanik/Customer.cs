using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Customer : MonoBehaviour
{
    public string requiredFoodName;
    public Image orderUI;
    public Sprite[] allFoodSprites;
    public string[] allFoodNames;
    public Inventory playerInventory;
    public CustomerManager spawner;

    public TextMeshProUGUI timerText; // 🔥 Tambahkan variabel untuk teks timer
    
    private float orderTimer = 90f;
    public int spawnPointIndex;

    void Start()
    {
        // Cari objek pemain secara otomatis
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerInventory = playerObject.GetComponent<Inventory>();
        }

        // Cari spawner di scene
        spawner = FindFirstObjectByType<CustomerManager>();

        // Secara acak memilih pesanan
        if (allFoodNames.Length > 0)
        {
            int randomIndex = Random.Range(0, allFoodNames.Length);
            requiredFoodName = allFoodNames[randomIndex];
            
            if (orderUI != null && allFoodSprites.Length > randomIndex)
            {
                orderUI.sprite = allFoodSprites[randomIndex];
                orderUI.gameObject.SetActive(true);
            }
        }
    }

    void Update()
    {
        // Kurangi timer setiap frame
        orderTimer -= Time.deltaTime;

        // 🔥 Perbarui tampilan teks timer
        if (timerText != null)
        {
            // Ubah detik menjadi format menit:detik
            int minutes = Mathf.FloorToInt(orderTimer / 60);
            int seconds = Mathf.FloorToInt(orderTimer % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // Jika timer hampir habis, mungkin ubah warnanya
            if (orderTimer <= 10f)
            {
                timerText.color = Color.red;
            }
            else
            {
                timerText.color = Color.white;
            }
        }

        // Jika waktu habis
        if (orderTimer <= 0f)
        {
            OrderFailed();
        }
    }

    private void OnDestroy()
    {
        // 🔥 Saat pelanggan pergi, kembalikan indeks titik spawn
        if (spawner != null)
        {
            spawner.ReturnSpawnPoint(spawnPointIndex);
        }
    }

    public bool CheckOrder(string foodThrownName)
    {
        if (foodThrownName == requiredFoodName)
        {
            playerInventory.AddPoints(10);
            playerInventory.CustomerServed();
            
            if (spawner != null) spawner.SpawnCustomer();
            Destroy(gameObject);
            return true;
        }
        else
        {
            OrderFailed();
            return false;
        }
    }

    public void OrderFailed()
    {
        if (playerInventory != null)
        {
            playerInventory.SubtractPoints(5);
        }
        if (spawner != null) spawner.SpawnCustomer();
        Destroy(gameObject);
    }
}