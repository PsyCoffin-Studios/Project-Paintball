using HelloWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIControllerInGame : MonoBehaviour
{

    [Header("\nVariables de Personaje \n")]

    private string arma;
    private string personaje;
    private string habilidad;

    [Header("\nVariables de UIInGame\n")]

    public GameObject canvasInGame;

    public Sprite[] listaDeSpritesArmas;
    public Sprite[] listaDeSpritesHabilidades;
    public string[] nombresDeArmas;
    public string[] nombresDeHabilidades;

    public Dictionary<string, Sprite> armas;
    public Dictionary<string, Sprite> habilidades;

    public GameObject armaImage;
    public GameObject habilidadImage;

    [Header("\nVariables de Opciones\n")]

    public GameObject canvasOpciones;
    public TextMeshProUGUI volumeDisplaySFX;
    public TextMeshProUGUI volumeDisplayMenu;
    public TextMeshProUGUI volumeDisplayAmbiente;
    public TextMeshProUGUI code;
    public TextMeshProUGUI playerID;
    public Toggle toggleSFX; 
    public Toggle toggleAmbiente; 
    public Toggle toggleMenu;

    [Header("\nVariables de Sonido\n")]

    public AudioMixer audioMixer;
    float currentSFXVolume;
    float currentAmbienteVolume;
    float currentMenuVolume;

    private const float MIN_VOLUME_DB = -80f;
    private const float MAX_VOLUME_DB = 0f;
    private const float volumenPaso = 1f;

    GameObject pausedPlayer;

    #region Logica de inicialización y actualización UIinGame y opciones
    void Start()
    {
        InitEstructures();
        LeerDatabase();
        ActualizarUI();
    }

    private void InitEstructures()
    {
        armas = new Dictionary<string, Sprite>();
        habilidades = new Dictionary<string, Sprite>();

        for (int i = 0; i < listaDeSpritesArmas.Length; i++)
        {
            armas.Add(nombresDeArmas[i], listaDeSpritesArmas[i]);
        }

        for (int i = 0; i < listaDeSpritesHabilidades.Length; i++)
        {
            habilidades.Add(nombresDeHabilidades[i], listaDeSpritesHabilidades[i]);
        }

        audioMixer.GetFloat("SFX", out currentSFXVolume);
        audioMixer.GetFloat("Ambiente", out currentAmbienteVolume);
        audioMixer.GetFloat("Menu", out currentMenuVolume);
    }

    private void ActualizarUI()
    {
        armaImage.GetComponent<Image>().sprite = ObtenerSpritePorNombre(arma);
        habilidadImage.GetComponent<Image>().sprite = ObtenerSpritePorNombre(habilidad);

        armaImage.SetActive(true);
        habilidadImage.SetActive(true);
    }

    public void ActualizarDatosNetcode()
    {
        code.text = GameObject.Find("@UIManager").GetComponent<UIManager>().GetCode();
        playerID.text = GameObject.Find("@UIManager").GetComponent<UIManager>().GetID();
    }
    private void LeerDatabase()
    {
        arma = DataBetweenScenes.instance.GetArma();
        personaje = DataBetweenScenes.instance.GetNombre();
        habilidad = DataBetweenScenes.instance.GetHabilidad();
    }

    public Sprite ObtenerSpritePorNombre(string nombre)
    {
        if (armas.ContainsKey(nombre))
        {
            Debug.LogWarning($"buscando el arma {arma}");
            return armas[nombre];  // Devuelve el sprite correspondiente al nombre del arma

        }
        else if (habilidades.ContainsKey(nombre))
        {
            return habilidades[nombre];
        }
        else
        {
            Debug.LogWarning("El nombre del arma o la habilidad no existe: " + nombre);
            return null;
        }
    }

    #endregion

    #region Logica de las opciones
    public void AbrirOpciones(GameObject player)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pausedPlayer = player;
        canvasInGame.SetActive(false);
        canvasOpciones.SetActive(true);
        pausedPlayer.GetComponent<InputController>().EnableUIControls();
    }

    public void CerrarOpciones()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        canvasOpciones.SetActive(false);
        canvasInGame.SetActive(true);
        pausedPlayer.GetComponent<InputController>().EnablePlayerControls();
        pausedPlayer=null;
    }

    public void SubirVolumen(string name)
    {
        switch (name)
        {
            case "SFX":
                if (toggleSFX.isOn)
                {
                    audioMixer.GetFloat(name, out currentSFXVolume);
                    // Calcular el nuevo volumen en el rango de 0 a 10
                    float nuevoSFXVolume = Mathf.Lerp(0f, 10f, Mathf.InverseLerp(MIN_VOLUME_DB, MAX_VOLUME_DB, currentSFXVolume));
                    nuevoSFXVolume = Mathf.Clamp(nuevoSFXVolume + volumenPaso, 0f, 10f);
                    // Convertir el nuevo volumen de 0 a 10 a decibelios
                    currentSFXVolume = Mathf.Lerp(MIN_VOLUME_DB, MAX_VOLUME_DB, nuevoSFXVolume / 10f);
                    audioMixer.SetFloat(name, currentSFXVolume);
                }
                break;

            case "Ambiente":
                if (toggleAmbiente.isOn)
                {
                    audioMixer.GetFloat(name, out currentAmbienteVolume);
                    float nuevoAmbienteVolume = Mathf.Lerp(0f, 10f, Mathf.InverseLerp(MIN_VOLUME_DB, MAX_VOLUME_DB, currentAmbienteVolume));
                    nuevoAmbienteVolume = Mathf.Clamp(nuevoAmbienteVolume + volumenPaso, 0f, 10f);
                    currentAmbienteVolume = Mathf.Lerp(MIN_VOLUME_DB, MAX_VOLUME_DB, nuevoAmbienteVolume / 10f);
                    audioMixer.SetFloat(name, currentAmbienteVolume);
                }
                break;

            case "Menu":
                if (toggleMenu.isOn)
                {
                    audioMixer.GetFloat(name, out currentMenuVolume);
                    float nuevoMenuVolume = Mathf.Lerp(0f, 10f, Mathf.InverseLerp(MIN_VOLUME_DB, MAX_VOLUME_DB, currentMenuVolume));
                    nuevoMenuVolume = Mathf.Clamp(nuevoMenuVolume + volumenPaso, 0f, 10f);
                    currentMenuVolume = Mathf.Lerp(MIN_VOLUME_DB, MAX_VOLUME_DB, nuevoMenuVolume / 10f);
                    audioMixer.SetFloat(name, currentMenuVolume);
                }
                break;
        }

        UpdateVolumeDisplay(name);
    }

    public void BajarVolumen(string name)
    {
        switch (name)
        {
            case "SFX":
                if (toggleSFX.isOn)
                {
                    audioMixer.GetFloat(name, out currentSFXVolume);
                    float nuevoSFXVolume = Mathf.Lerp(0f, 10f, Mathf.InverseLerp(MIN_VOLUME_DB, MAX_VOLUME_DB, currentSFXVolume));
                    nuevoSFXVolume = Mathf.Clamp(nuevoSFXVolume - volumenPaso, 0f, 10f);
                    currentSFXVolume = Mathf.Lerp(MIN_VOLUME_DB, MAX_VOLUME_DB, nuevoSFXVolume / 10f);
                    audioMixer.SetFloat(name, currentSFXVolume);
                }
                break;

            case "Ambiente":
                if (toggleAmbiente.isOn)
                {
                    audioMixer.GetFloat(name, out currentAmbienteVolume);
                    float nuevoAmbienteVolume = Mathf.Lerp(0f, 10f, Mathf.InverseLerp(MIN_VOLUME_DB, MAX_VOLUME_DB, currentAmbienteVolume));
                    nuevoAmbienteVolume = Mathf.Clamp(nuevoAmbienteVolume - volumenPaso, 0f, 10f);
                    currentAmbienteVolume = Mathf.Lerp(MIN_VOLUME_DB, MAX_VOLUME_DB, nuevoAmbienteVolume / 10f);
                    audioMixer.SetFloat(name, currentAmbienteVolume);
                }
                break;

            case "Menu":
                if (toggleMenu.isOn)
                {
                    audioMixer.GetFloat(name, out currentMenuVolume);
                    float nuevoMenuVolume = Mathf.Lerp(0f, 10f, Mathf.InverseLerp(MIN_VOLUME_DB, MAX_VOLUME_DB, currentMenuVolume));
                    nuevoMenuVolume = Mathf.Clamp(nuevoMenuVolume - volumenPaso, 0f, 10f);
                    currentMenuVolume = Mathf.Lerp(MIN_VOLUME_DB, MAX_VOLUME_DB, nuevoMenuVolume / 10f);
                    audioMixer.SetFloat(name, currentMenuVolume);
                }
                break;
        }

        UpdateVolumeDisplay(name);
    }

    private void UpdateVolumeDisplay(string name)
    {
        // Calcula el volumen actual en el rango de 0 a 10 usando Lerp
        float normalizedVolume;
        int volumeLevel;

        switch (name)
        {
            case "SFX":
                if (!toggleSFX.isOn)
                {
                    volumeDisplaySFX.text = "";
                }
                else
                {
                    normalizedVolume = Mathf.Lerp(0f, 10f, Mathf.InverseLerp(MIN_VOLUME_DB, MAX_VOLUME_DB, currentSFXVolume));
                    volumeLevel = Mathf.RoundToInt(normalizedVolume);
                    volumeDisplaySFX.text = "" + volumeLevel;
                }
                break;

            case "Ambiente":
                if (!toggleAmbiente.isOn)
                {
                    volumeDisplayAmbiente.text = "";
                }
                else
                {
                    normalizedVolume = Mathf.Lerp(0f, 10f, Mathf.InverseLerp(MIN_VOLUME_DB, MAX_VOLUME_DB, currentAmbienteVolume));
                    volumeLevel = Mathf.RoundToInt(normalizedVolume);
                    volumeDisplayAmbiente.text = "" + volumeLevel;
                }
                break;

            case "Menu":
                if (!toggleMenu.isOn)
                {
                    volumeDisplayMenu.text = "";
                }
                else
                {
                    normalizedVolume = Mathf.Lerp(0f, 10f, Mathf.InverseLerp(MIN_VOLUME_DB, MAX_VOLUME_DB, currentMenuVolume));
                    volumeLevel = Mathf.RoundToInt(normalizedVolume);
                    volumeDisplayMenu.text = "" + volumeLevel;
                }
                break;
        }
    }

    // Métodos para manejar los cambios de estado de los toggles
    public void SFXToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            audioMixer.SetFloat("SFX", 0f); // Activa el sonido si el toggle está activado
            audioMixer.GetFloat("SFX", out currentSFXVolume); // Actualiza currentSFXVolume
        }
        else
        {
            audioMixer.SetFloat("SFX", MIN_VOLUME_DB); // Silencia el sonido si el toggle está desactivado
        }
        UpdateVolumeDisplay("SFX");
    }

    public void AmbienteToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            audioMixer.SetFloat("Ambiente", 0f);
            audioMixer.GetFloat("Ambiente", out currentAmbienteVolume); // Actualiza currentAmbienteVolume
        }
        else
        {
            audioMixer.SetFloat("Ambiente", MIN_VOLUME_DB);
        }
        UpdateVolumeDisplay("Ambiente");
    }

    public void MenuToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            audioMixer.SetFloat("Menu", 0f);
            audioMixer.GetFloat("Menu", out currentMenuVolume); // Actualiza currentMenuVolume
        }
        else
        {
            audioMixer.SetFloat("Menu", MIN_VOLUME_DB);
        }
        UpdateVolumeDisplay("Menu");
    }

    #endregion
}
