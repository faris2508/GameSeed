using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using NUnit.Framework.Internal;
using System.Linq;

public class LevelUpdate : MonoBehaviour
{
    [Header("Level Config")]
    [SerializeField] List<LevelSO> levels;
    LevelSO currentLevel;
    private int currentLevelIndex = 0;


    [Header("Timer")]
    public float startTimeInSeconds = 60f; // Durasi challenge (misalnya 60 detik)
    private float currentTime;
    public TextMeshProUGUI timerText; // UI Text untuk menampilkan waktu
    public bool isTimerRunning = false; // Kontrol timer dari luar

    [Header("Customer Spawn")]
    public GameObject orderPrefab;
    public List<Transform> spawnPoints;
    public Vector3 spawnRotation;
    [SerializeField] private int customersPerWave = 3;
    public float spawnInterval = 60f; // Jarak antar spawn
    private Coroutine spawnCoroutine;
    public bool isOrderTaken = false;

    [Header("Customer Waiting Time")]
    public float waitingTime;// Waktu menunggu sebelum customer pergi

    private bool isCustomerWaiting = false;
    private bool isServed = false; // apakah makanan sudah diberikan
    private int customerIndex = 0;
    private int maxSpawn;

    [Header("Player & Order Interaction")]
    public Transform player; // drag player object ke sini
    public float interactRange = 2f;
    private GameObject currentCustomerGO = null;
    private CustomerSO currentCustomerData = null;
    private CustomerIdentity customerIdentity;
    public TextMeshProUGUI promptText;
    private bool isNearCustomer = false;

    //level section
    /*public void getLevelData(LevelSO currentLevel)
    {
        
        
    }*/

    public static LevelUpdate Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    void Start()
    {
        currentLevel = levels[currentLevelIndex];
        maxSpawn = currentLevel.totalCustomers;

        orderPrefab.SetActive(false);
        StartLevel(levels[currentLevelIndex].levelNumber);
        currentTime = startTimeInSeconds;
        UpdateTimerDisplay();
    }

    void Update()
    {

        if (isTimerRunning)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                if (currentTime < 0) currentTime = 0;
                UpdateTimerDisplay();

            }
            else
            {
                isTimerRunning = false;
                TimerEnded();
            }
        }

        if (isNearCustomer && Input.GetKeyDown(KeyCode.E))
        {
            if (currentCustomerData != null)
            {
                ShowOrderUI(currentCustomerData);
                isOrderTaken = true;
            }
        }

        /*if (isOrderTaken)
        {
            orderPrefab.SetActive(true);
        }*/
    }






    //timer section
    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    void TimerEnded()
    {
        Debug.Log("Level Selesai!");
        // Tambahkan logika kalau timer habis, misal:
        // GameOver(), ShowLosePanel(), dll
    }

    public void StartLevel(int levelIndex)
    {
        currentTime = startTimeInSeconds;
        isTimerRunning = true;

        StartSpawningRandomCustomers(levelIndex);
    }
    public void StopLevel()
    {
        isTimerRunning = false;
    }


    //spawn cust

    public class SpawnedCustomer
    {
        public CustomerSO data;
        public float waitingTime;
        public MenuSO menuCustomer;
        public Sprite orderUI;

        public SpawnedCustomer(CustomerSO customerData, float time, MenuSO menu, Sprite order)
        {
            data = customerData;
            waitingTime = time;
            menuCustomer = menu;
            orderUI = order;
        }
    }
    public List<SpawnedCustomer> spawnedCustomers = new List<SpawnedCustomer>();

    public void StartSpawningRandomCustomers(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Count)
        {
            Debug.LogWarning("Level index tidak valid.");
            return;
        }

        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnCustomersCoroutine(levels[levelIndex]));
    }

    IEnumerator SpawnCustomersCoroutine(LevelSO levelData)
    {
        yield return new WaitForSecondsRealtime(3f); // Delay awal

        int spawnCount = 0;

        while (isTimerRunning && spawnCount < maxSpawn)
        {
            if (levelData.customersData.Length > 0)
            {
                int randomIndex = Random.Range(0, levelData.customersData.Length);
                CustomerSO selected = levelData.customersData[randomIndex];

                // Gunakan waiting time dari data customer langsung
                SpawnedCustomer sc = new SpawnedCustomer(selected, selected.waitingTime, selected.menu, selected.Order);
                spawnedCustomers.Add(sc);

                Transform spawnPoint = spawnPoints[spawnCount % spawnPoints.Count];
                GameObject prefab = selected.Object;

                GameObject customerGO = Instantiate(prefab, spawnPoint.position, Quaternion.Euler(spawnRotation));
                CustomerIdentity identity = customerGO.AddComponent<CustomerIdentity>();
                if (identity != null)
                {
                    identity.customerData = selected;

                    // Cari OrderSpawnPoint dari child prefab customer
                    Transform spawnPointUI = customerGO.transform.Find("OrderSpawnPoint");
                    if (spawnPointUI != null)
                    {
                        identity.orderSpawnPoint = spawnPointUI;
                    }
                    else
                    {
                        Debug.LogWarning("OrderSpawnPoint not found in customer prefab: " + customerGO.name);
                    }
                }
                else
                {
                    Debug.LogWarning("CustomerTrigger not found on customer prefab: " + customerGO.name);
                }


                //Transform spawnPointUI = customerGO.GetComponentsInChildren<Transform>(true)
                //                   .FirstOrDefault(t => t.name == "OrderSpawnPoint");


                customerGO.tag = "Customer";

                StartCoroutine(CustomerWaitingTimer(sc, customerGO));

                spawnCount++;
            }

            // Tunggu sebelum spawn customer berikutnya
            yield return new WaitForSecondsRealtime(spawnInterval);
        }
    }

    IEnumerator CustomerWaitingTimer(SpawnedCustomer customer, GameObject customerGO)
    {
        float time = customer.waitingTime;

        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        Debug.Log("Customer sudah pergi (waktu habis): " + customer.data.id);

        if (customerGO != null)
        {
            Destroy(customerGO);
        }
    }

    public void SpawnOrder()
    {
        if (!isOrderTaken && (Input.GetKeyDown(KeyCode.E)))
        {
            isOrderTaken = true;
            orderPrefab.SetActive(true);
            //Debug.Log("Order taken");
        }


    }

    public void OnCustomerTriggerEnter(CustomerIdentity customer)
    {
        customerIdentity = customer;
        currentCustomerData = customer.customerData;
        isNearCustomer = true;

        if (promptText != null)
        {
            promptText.text = "Tekan E untuk mengambil pesanan";
            promptText.gameObject.SetActive(true);
        }
    }

    public void OnCustomerTriggerExit(CustomerIdentity customer)
    {
        if (customerIdentity == customer)
        {
            customerIdentity = null;
            currentCustomerData = null;
            isNearCustomer = false;

            if (promptText != null)
                promptText.gameObject.SetActive(false);
        }
    }



    void ShowOrderUI(CustomerSO customer)
    {
        Debug.Log("Spawn UI");
        if (orderPrefab == null || customerIdentity == null) return;

        if (customerIdentity.currentOrderUI != null) return;

        GameObject orderUI = Instantiate(orderPrefab, customerIdentity.orderSpawnPoint.position, Quaternion.identity);
        orderUI.SetActive(true);
        orderUI.transform.SetParent(customerIdentity.orderSpawnPoint);
        orderUI.transform.localPosition = Vector3.zero;

        customerIdentity.currentOrderUI = orderUI;

        FollowUI followScript = orderUI.GetComponent<FollowUI>();
        if (followScript != null)
        {
            followScript.SetTarget(customerIdentity.orderSpawnPoint);
        }

        // Ambil Sprite dari customer.order.orderSprite
        Sprite orderSprite = customer.Order;
        if (orderSprite != null)
        {
            UnityEngine.UI.Image image = orderUI.GetComponentInChildren<UnityEngine.UI.Image>();
            if (image != null)
                image.sprite = orderSprite;
                image.preserveAspect = true;
        }
        else
        {
            Debug.LogWarning("Order sprite not found in CustomerSO: " + customer.name);
        }
    
    }




}
