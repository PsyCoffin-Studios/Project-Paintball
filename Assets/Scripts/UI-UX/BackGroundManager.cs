using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackGroundManager : MonoBehaviour { 
    
    public Sprite[] initialBackgrounds;
    public Sprite[] creditosBackgrounds;
    public Sprite[] selectionBackgrounds;

    public Image panelBackground;


    private string arma;
    private Dictionary<string, Sprite> armas;
    public string[] nombresDeArmas;
    public Sprite[] listaDeSpritesArmas;
    public GameObject armaImage;

    private int currentArmaIndex = 0; // Índice actual del arma seleccionada

    public void Start()
    {
        InitEstructures();
        SetRandomBackground();

    }

    private void InitEstructures()
    {

        armas = new Dictionary<string, Sprite>();

        for (int i = 0; i < listaDeSpritesArmas.Length; i++)
        {
            armas.Add(nombresDeArmas[i], listaDeSpritesArmas[i]);
        }

        // Configurar arma inicial
        if (nombresDeArmas.Length > 0)
        {
            arma = nombresDeArmas[currentArmaIndex];
            ActualizarUI();
        }
    }

    private void ActualizarUI()
    {
        armaImage.GetComponent<Image>().sprite = ObtenerSpritePorNombre(arma);
        armaImage.SetActive(true);
    }

    public Sprite ObtenerSpritePorNombre(string nombre)
    {
        if (armas.ContainsKey(nombre))
        {
            return armas[nombre];  // Devuelve el sprite correspondiente al nombre del arma

        }
        else
        {
            Debug.LogWarning("El nombre del arma no existe: " + nombre);
            return null;
        }
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

    public void cambiarArmaHaciaDerecha()
    {
        if (nombresDeArmas.Length == 0)
        {
            Debug.LogWarning("No hay armas configuradas.");
            return;
        }

        // Avanzar al siguiente arma (circular)
        currentArmaIndex = (currentArmaIndex + 1) % nombresDeArmas.Length;
        arma = nombresDeArmas[currentArmaIndex];


        ActualizarUI();
    }
    public void cambiarArmaHaciaIzquierda()
    {
        if (nombresDeArmas.Length == 0)
        {
            Debug.LogWarning("No hay armas configuradas.");
            return;
        }

        // Retroceder al arma anterior (circular)
        currentArmaIndex = (currentArmaIndex - 1 + nombresDeArmas.Length) % nombresDeArmas.Length;
        arma = nombresDeArmas[currentArmaIndex];

        ActualizarUI();
    }

    public string ObtenerArmaSeleccionada()
    {
        return arma;
    }

    internal void startCreditos()
    {
        panelBackground.sprite = creditosBackgrounds[0];
    }
    public void cambiarCreditos()
    {
        if (creditosBackgrounds.Length > 0)
        {
            panelBackground.sprite = creditosBackgrounds[1];
        }
    }


}