using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Customer", fileName = "New Customer")]
public class CustomerSO : ScriptableObject
{
    public MenuSO menu;
    public string id;
    public int number;
    public GameObject Object;
    public GameObject Order;
    public float waitingTime;
}
