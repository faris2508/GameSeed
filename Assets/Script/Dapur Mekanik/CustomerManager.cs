using UnityEngine;
using System.Collections.Generic;

public class CustomerManager : MonoBehaviour
{
    public GameObject customerPrefab;
    public Transform[] spawnPoints;
    public int initialCustomers = 3;
    public int maxCustomers = 5;

    // 🔥 Gunakan List untuk melacak indeks yang tersedia
    private List<int> availableSpawnIndexes = new List<int>();

    void Start()
    {
        // 🔥 Inisialisasi daftar dengan semua indeks titik spawn
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            availableSpawnIndexes.Add(i);
        }

        // Munculkan pelanggan awal
        for (int i = 0; i < initialCustomers; i++)
        {
            SpawnCustomer();
        }
    }

    public void SpawnCustomer()
    {
        GameObject[] currentCustomers = GameObject.FindGameObjectsWithTag("Customer");
        if (currentCustomers.Length < maxCustomers && availableSpawnIndexes.Count > 0)
        {
            // 🔥 Pilih indeks acak dari daftar yang tersedia
            int listIndex = Random.Range(0, availableSpawnIndexes.Count);
            int spawnPointIndex = availableSpawnIndexes[listIndex];
            
            Transform randomSpawnPoint = spawnPoints[spawnPointIndex];
            GameObject newCustomer = Instantiate(customerPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);

            // 🔥 Atur indeks yang digunakan di skrip pelanggan dan hapus dari daftar
            Customer customerScript = newCustomer.GetComponent<Customer>();
            if(customerScript != null)
            {
                customerScript.spawnPointIndex = spawnPointIndex;
            }
            availableSpawnIndexes.RemoveAt(listIndex);
        }
    }

    // 🔥 Metode baru untuk mengembalikan indeks titik spawn
    public void ReturnSpawnPoint(int index)
    {
        availableSpawnIndexes.Add(index);
    }
}
