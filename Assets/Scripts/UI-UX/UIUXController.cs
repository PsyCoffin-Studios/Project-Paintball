using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class UIUXController : MonoBehaviour
{
    public BackGroundManager backGroundManager;
    public AudioManagerUI audioManagerUI;


    public GameObject startButton; // Bot�n de "Iniciar Partida"
    public GameObject creditsButton; // Bot�n de "Cr�ditos"

    public GameObject botonesPersonajes;
    public GameObject botonInicio;

    public void Start()
    {
        backGroundManager = GameObject.Find("@BackgroundManager").GetComponent<BackGroundManager>();
        audioManagerUI = GameObject.Find("@AudioManager").GetComponent<AudioManagerUI>();

    }

    public void SeleccionPersonaje()
    {

        // Ocultar los botones de Iniciar Partida y Cr�ditos
        startButton.SetActive(false);
        creditsButton.SetActive(false);

        // Mostrar los botones de selecci�n de personaje
        backGroundManager.startPlayerSelection();
        botonesPersonajes.SetActive(true);

        //Parar la m�sica de inicio
        audioManagerUI.StopMusic();
    }

    public void PantallaCreditos()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + _______);

        throw new NotImplementedException();
    }

    public void IniciarPartida()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void clickEnPersonaje(GameObject button)
    {
        string nombreBoton = button.name; // Obtener el nombre del bot�n clicado
        int seleccionPersonaje = 0; // Valor por defecto

        // Asignar un valor basado en el nombre del bot�n
        switch (nombreBoton)
        {
            case "BotonJoker":
                seleccionPersonaje = 1;

                DataBetweenScenes.instance.SetNombre("Joker");
                DataBetweenScenes.instance.SetArma("Rifle");
                DataBetweenScenes.instance.SetHabilidad("Explosiones");

                break;
            case "BotonOutcast":
                seleccionPersonaje = 2;

                DataBetweenScenes.instance.SetNombre("Outcast");
                DataBetweenScenes.instance.SetArma("Francotirador");
                DataBetweenScenes.instance.SetHabilidad("BombasDeHumo");

                break;
            case "BotonRevenant":
                seleccionPersonaje = 3;

                DataBetweenScenes.instance.SetNombre("Revenant");
                DataBetweenScenes.instance.SetArma("Escopeta");
                DataBetweenScenes.instance.SetHabilidad("Invisibilidad");

                break;
            case "BotonHex":
                seleccionPersonaje = 4;

                DataBetweenScenes.instance.SetNombre("Hex");
                DataBetweenScenes.instance.SetArma("Pistola");
                DataBetweenScenes.instance.SetHabilidad("Dash");

                break;
            default:
                Debug.LogError("Bot�n no reconocido");
                break;
        }


        backGroundManager.updatePlayerSelection(seleccionPersonaje);
        audioManagerUI.playChooseDialog(seleccionPersonaje);

        if (!botonInicio.active) { botonInicio.SetActive(true); }
    }
}
