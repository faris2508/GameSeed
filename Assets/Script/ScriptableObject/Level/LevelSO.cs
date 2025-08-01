using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level", fileName = "New Level")]
public class LevelSO : ScriptableObject
{
    public int levelNumber;
    public string levelID;
    public CustomerSO[] customersData;

    public int currentCustomer;
    public int goalCustomers;
    public int totalCustomers;

    public float currentScore;
    public float totalScore;
    public float highScore;

    public float startTimeInSeconds; // Durasi Level (misalnya 60 detik)
    private float currentTime;
    public bool isTimerRunning;
    public bool isLevelFinished;

    public int scoreFor1Star;
    public int scoreFor2Stars;
    public int scoreFor3Stars;
}
