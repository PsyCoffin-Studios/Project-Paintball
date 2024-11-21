using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class UIUXController : MonoBehaviour
{
    public BackGroundManager backGroundManager;
    public AudioManagerUI audioManagerUI;


    public GameObject startButton; // Botón de "Iniciar Partida"
    public GameObject creditsButton; // Botón de "Créditos"

    public GameObject nextButton;
    public GameObject reiButton;

    public GameObject botonesPersonajes;
    public GameObject botonesCreditos;
    public GameObject botonInicio;
    public GameObject botonesPistola;

    public void Start()
    {
        backGroundManager = GameObject.Find("@BackgroundManager").GetComponent<BackGroundManager>();
        audioManagerUI = GameObject.Find("@AudioManager").GetComponent<AudioManagerUI>();

    }

    public void seleccionPersonaje()
    {

        // Ocultar los botones de Iniciar Partida y Créditos
        startButton.SetActive(false);
        creditsButton.SetActive(false);

        // Mostrar los botones de selección de personaje
        backGroundManager.startPlayerSelection();
        botonesPersonajes.SetActive(true);

        //Parar la música de inicio
        audioManagerUI.StopMusic();
    }

    public void pantallaCreditos()
    {
        // Ocultar los botones de Iniciar Partida y Créditos
        startButton.SetActive(false);
        creditsButton.SetActive(false);

        // Mostrar el primer boton de los creditos y su canvas
        backGroundManager.startCreditos();
        botonesCreditos.SetActive(true);

        //Parar la música de inicio
        audioManagerUI.StopMusic();
    }

    public void cambiarCreditos()
    {
        nextButton.SetActive(false);
        reiButton.SetActive(true);
        
        backGroundManager.cambiarCreditos();
    }

    public void iniciarPartida()
    {
        // Configurar el arma seleccionada
        string armaSeleccionada = backGroundManager.ObtenerArmaSeleccionada();
        DataBetweenScenes.instance.SetArma(armaSeleccionada);


        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void clickEnPersonaje(GameObject button)
    {
        string nombreBoton = button.name; // Obtener el nombre del botón clicado
        int seleccionPersonaje = 0; // Valor por defecto
        botonesPistola.SetActive(true);

        // Asignar un valor basado en el nombre del botón
        switch (nombreBoton)
        {
            case "BotonJoker":
                seleccionPersonaje = 1;

                DataBetweenScenes.instance.SetNombre("Joker");
                DataBetweenScenes.instance.SetHabilidad("Explosiones");

                break;
            case "BotonOutcast":
                seleccionPersonaje = 2;

                DataBetweenScenes.instance.SetNombre("Outcast");
                DataBetweenScenes.instance.SetHabilidad("BombasDeHumo");

                break;
            case "BotonRevenant":
                seleccionPersonaje = 3;

                DataBetweenScenes.instance.SetNombre("Revenant");
                DataBetweenScenes.instance.SetHabilidad("Invisibilidad");

                break;
            case "BotonHex":
                seleccionPersonaje = 4;

                DataBetweenScenes.instance.SetNombre("Hex");
                DataBetweenScenes.instance.SetHabilidad("Dash");

                break;
            default:
                Debug.LogError("Botón no reconocido");
                break;
        }

        backGroundManager.updatePlayerSelection(seleccionPersonaje);
        backGroundManager.ChangeActivePlayer(seleccionPersonaje - 1);
        audioManagerUI.playChooseDialog(seleccionPersonaje);

        if (!botonInicio.active) { botonInicio.SetActive(true); }
    }


    public void volverInicio()
    {
        // Recarga la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
 
}
