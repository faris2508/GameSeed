using UnityEngine;

public class CustomerBehavior : MonoBehaviour
{
    LevelSO currentLevel;
    CustomerSO customerSO;
    MenuSO menu;
    string id;
    int number;
    GameObject Object;


    [Header("Customer Waiting Time")]
    float waitingTime;
    private bool isCustomerWaiting = false;
    private bool isServed = false; // apakah makanan sudah diberikan
    private int customerIndex = 0;

    public void GetCustomerData()
    {
        menu = customerSO.menu;
        id = customerSO.id;
        number = customerSO.number;
        Object = customerSO.Object;
        waitingTime = customerSO.waitingTime;
    }

    void Update()
    {
        if (isCustomerWaiting && isServed)
        {
            //float[] currentWaitingTime = new float[currentLevel.totalCustomers];

            
        }
    }

    void UpdateWaitingTime()
    {
        for (int i = 0; i < currentLevel.totalCustomers; i++)
            {

                currentLevel.customersData[i].waitingTime = waitingTime;
                if (currentLevel.customersData[i].waitingTime > 0)
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
    }
}
