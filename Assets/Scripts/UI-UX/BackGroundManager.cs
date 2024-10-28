using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundManager : MonoBehaviour { 
    
    public Sprite[] initialBackgrounds;
    public Sprite[] selectionBackgrounds;

    public Image panelBackground;

    void Start()
    {
        SetRandomBackground();
    }

    void SetRandomBackground()
    {
        if (initialBackgrounds.Length > 0)
        {
            int randomIndex = Random.Range(0, initialBackgrounds.Length);
            panelBackground.sprite = initialBackgrounds[randomIndex];
        }
        else
        {
            Debug.LogError("No hay imágenes de fondo asignadas en el array.");
        }
    }

    public void startPlayerSelection()
    {
        panelBackground.sprite = selectionBackgrounds[0];
    }

    public void updatePlayerSelection(int idx)
    {
        panelBackground.sprite = selectionBackgrounds[idx];
    }
}