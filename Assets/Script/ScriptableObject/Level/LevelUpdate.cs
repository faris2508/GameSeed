using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using NUnit.Framework.Internal;

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

    //level section
    /*public void getLevelData(LevelSO currentLevel)
    {
        
        
    }*/
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

        if (isOrderTaken)
        {
            orderPrefab.SetActive(true);
        }
        
        /*if (isCustomerWaiting && isServed)
        {
            //float[] currentWaitingTime = new float[currentLevel.totalCustomers];

            for (int i = 0; i < currentLevel.totalCustomers; i++)
            {
                
                var customer = spawnedCustomers[i];
                //customer.waitingTime = waitingTime;
                if (spawnedCustomers[i].waitingTime > 0)
                {
                    spawnedCustomers[i].waitingTime -= Time.deltaTime;
                    if (spawnedCustomers[i].waitingTime < 0) spawnedCustomers[i].waitingTime = 0;
                    Debug.Log(spawnedCustomers[i].waitingTime);
                }
                if (spawnedCustomers[i].waitingTime == 0)
                {
                    Debug.Log("Customer " + i + " sudah pergi!");
                    // TODO: Destroy GameObject-nya atau sembunyikan
                }
                //currentLevel.customersData[i].waitingTime = waitingTime;
                    /*if (currentLevel.customersData[i].waitingTime > 0)
                    {
                        currentLevel.customersData[i].waitingTime -= Time.deltaTime;
                        if (currentLevel.customersData[i].waitingTime < 0)
                        currentLevel.customersData[i].waitingTime = 0;

                    }
                    if (currentLevel.customersData[i].waitingTime == 0)
                    {
                        Debug.Log("Customer " + i + " sudah pergi!");

                        // TODO: Destroy GameObject-nya atau sembunyikan
                    }
            }
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

        public SpawnedCustomer(CustomerSO customerData, float time, MenuSO menu)
        {
            data = customerData;
            waitingTime = time;
            menuCustomer = menu;
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
    yield return new WaitForSecondsRealtime(3f);
    int spawnCount = 0;

    while (isTimerRunning && spawnCount < maxSpawn)
    {
        for (int i = 0; i < customersPerWave && spawnCount < maxSpawn; i++)
        {
            if (levelData.customersData.Length > 0)
            {
                int randomIndex = Random.Range(0, levelData.customersData.Length);
                CustomerSO selected = levelData.customersData[randomIndex];

                SpawnedCustomer sc = new SpawnedCustomer(selected, selected.waitingTime, selected.menu);
                spawnedCustomers.Add(sc);
                spawnedCustomers[customerIndex].waitingTime = waitingTime;

                // Ambil spawn point secara acak atau sesuai urutan
                Transform spawnPoint = spawnPoints[spawnCount % spawnPoints.Count];
                GameObject prefab = selected.Object;

                GameObject customerGO = Instantiate(prefab, spawnPoint.position, Quaternion.Euler(spawnRotation));
                //sc.instance = customerGO;

                customerIndex++;
                spawnCount++;
                StartCoroutine(CustomerWaitingTimer(sc, customerGO));
            }
        }

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
}
