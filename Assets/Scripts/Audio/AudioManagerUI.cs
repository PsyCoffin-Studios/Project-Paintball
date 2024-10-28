
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;

public class AudioManagerUI : MonoBehaviour
{
    public AudioMixer audioMixer;
    public GameObject canvasOpciones;

    // Región para los sonidos de música
    #region Música

    [Header("Música")]
    public AudioSource audioSourceMusica; // Componente AudioSource para la música
    public TextMeshProUGUI cancionText; // Referencia al objeto Text de la UI para mostrar el nombre de la canción

    public AudioClip[] musicaRandom;
    public AudioClip[] chooseDialogs;
    public AudioClip musicaInicio;

    private AudioClip ultimaCancion; // Para almacenar la última canción reproducida


    public void PlayMusic(AudioClip musicClip)
    {
        if (audioSourceMusica != null && musicClip != null)
        {
            audioSourceMusica.clip = musicClip;
            audioSourceMusica.Play();

            cancionText.text = "Reproduciendo: " + musicClip.name;
            ultimaCancion = musicClip;
        }
    }

    public void StopMusic()
    {
        if (audioSourceMusica != null)
        {
            audioSourceMusica.Stop();
        }
    }

    #endregion

    // Región para los sonidos SFX (efectos de sonido)
    #region SFX

    [Header("Sonidos SFX")]
    public AudioClip hoverSound; // Sonido para cuando el ratón pasa por encima del botón
    public AudioClip clickSound; // Sonido para cuando se hace clic en el botón

    private AudioSource audioSourceBotones; // Componente para reproducir los sonidos de los botones
    private AudioSource audioSourceDialogos;

    // Métodos para reproducir sonidos SFX
    public void EmitirSonidoHover()
    {
        if (hoverSound != null)
        {
            audioSourceBotones.PlayOneShot(hoverSound);
        }
    }

    public void EmitirSonidoClick()
    {
        if (clickSound != null)
        {
            audioSourceBotones.PlayOneShot(clickSound);
        }

        StartCoroutine(CambiarEscenaConRetraso(2.0f));
    }

    internal void playChooseDialog(int seleccionPersonaje)// Descomentar cuando se añadan los 'DIÁLOGOS.
    {
        //audioSourceDialogos.PlayOneShot(chooseDialogs[seleccionPersonaje]);
    }


    #endregion

    private void Start()
    {
        if (!SceneManager.GetActiveScene().name.Equals("SampleScene")) { 
            audioSourceMusica = GameObject.Find("MusicaAudioSource").GetComponent<AudioSource>();
        }

        audioSourceBotones = GameObject.Find("BotonesAudioSource").GetComponent<AudioSource>();
        audioSourceDialogos = GameObject.Find("DialogosAudioSource").GetComponent<AudioSource>();

        if (audioSourceBotones == null)
        {
            Debug.LogError("No se ha podido asignar correctamente un AudioSource para los botones");
        }

        if (audioSourceMusica == null)
        {
            Debug.LogError("No se ha podido asignar correctamente un AudioSource para la musica");
        }

        if (audioSourceDialogos == null)
        {
            Debug.LogError("No se ha podido asignar correctamente un AudioSource para los dialogos");
        }
    }

    private void Update()
    {
        if (!SceneManager.GetActiveScene().name.Equals("SampleScene") || (canvasOpciones.active && canvasOpciones!=null))
        {
            if (!audioSourceMusica.isPlaying)
            {
                int randomIndex;
                AudioClip cancionRandom;

                do
                {
                    randomIndex = Random.Range(0, musicaRandom.Length);
                    cancionRandom = musicaRandom[randomIndex];

                } while (cancionRandom == ultimaCancion);

                PlayMusic(cancionRandom);
            }
        } else 
        {
            StopMusic();
        }
    }


    // Método para cambiar el volumen global
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20); // Convierte el valor lineal a dB
    }

    // Corutina para esperar antes de cambiar la escena
    private IEnumerator CambiarEscenaConRetraso(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Cambiando de escena...");
    }


}
