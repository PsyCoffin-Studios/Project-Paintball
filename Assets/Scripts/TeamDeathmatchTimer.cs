using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeamDeathmatchTimer : MonoBehaviour
{
    public float timerDuration = 600f; // Duración del timer en segundos.
    private float timeRemaining;
    private bool timerRunning = false;

    TextMeshProUGUI textMeshProUGUI;
    public TeamDeathmatchManager teamDeathmatchManager;

    private void Start()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        teamDeathmatchManager = GameObject.Find("@TeamDeathmatchManager").GetComponent<TeamDeathmatchManager>();
    }

    void InitTimer()
    {
        timeRemaining = timerDuration;
        timerRunning = true;
    }

    void Update()
    {
        if (timerRunning)
        {
            // Descontar tiempo usando Time.deltaTime.
            timeRemaining -= Time.deltaTime;

            // Calcular minutos y segundos.
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);

            // Mostrar el tiempo restante en el formato MM:SS.
            textMeshProUGUI.text=string.Format("{0:00}:{1:00}", minutes, seconds);

            if (timeRemaining <= 0f)
            {
                timerRunning = false;
                TimerFinished();
            }
        }
    }

    void TimerFinished()
    {
        Debug.Log("El tiempo ha terminado!");
        teamDeathmatchManager.EndMatch(0);
    }
}
