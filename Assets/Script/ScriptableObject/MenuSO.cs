using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Makanan", fileName = "New Makanan")]
public class MenuSO : ScriptableObject
{
    public string nama;
    public string id;
    public GameObject Object;
}
