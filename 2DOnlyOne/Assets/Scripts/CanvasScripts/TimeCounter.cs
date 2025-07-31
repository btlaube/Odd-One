using UnityEngine;
using TMPro;
using System.Collections;

public class TimeCounter : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text timeDisplay;
    public TMP_Text scoreDisplay;

    [Header("Timer Settings")]
    public int countUpSeconds = 720; // total real seconds to run the timer (12 hours of game time)
    public int startHour = 0; // 12:00 AM
    public int startMinute = 0;

    private int currentMinutes;
    private Coroutine timeCoroutine;

    void Start()
    {
        currentMinutes = startHour * 60 + startMinute;
        UpdateDisplay();
        timeCoroutine = StartCoroutine(UpdateTime());
        scoreDisplay.text = $"Night {GameStateManager.Instance.gameState.level}";
    }

    IEnumerator UpdateTime()
    {
        for (int i = 0; i < countUpSeconds; i++)
        {
            currentMinutes++;

            if (currentMinutes % 5 == 0)
            {
                UpdateDisplay();
            }

            yield return new WaitForSeconds(1f);
        }
        TimerEnd();
    }

    private void TimerEnd()
    {
        GameStateManager.Instance.FailLevel();
    }

    private void UpdateDisplay()
    {
        int hours = (currentMinutes / 60) % 24;
        int minutes = currentMinutes % 60;
        string ampm = hours >= 12 ? "PM" : "AM";
        int displayHour = hours % 12;
        if (displayHour == 0) displayHour = 12;

        timeDisplay.text = $"{displayHour:D2}:{minutes:D2} {ampm}";
    }
}
