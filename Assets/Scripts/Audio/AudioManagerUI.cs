
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

    // Regi�n para los sonidos de m�sica
    #region M�sica

    [Header("M�sica")]
    public AudioSource audioSourceMusica; // Componente AudioSource para la m�sica
    public TextMeshProUGUI cancionText; // Referencia al objeto Text de la UI para mostrar el nombre de la canci�n

    public AudioClip[] musicaRandom;
    public AudioClip[] chooseDialogs;
    public AudioClip musicaInicio;

    private AudioClip ultimaCancion; // Para almacenar la �ltima canci�n reproducida


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

    // Regi�n para los sonidos SFX (efectos de sonido)
    #region SFX

    [Header("Sonidos SFX")]
    public AudioClip hoverSound; // Sonido para cuando el rat�n pasa por encima del bot�n
    public AudioClip clickSound; // Sonido para cuando se hace clic en el bot�n

    private AudioSource audioSourceBotones; // Componente para reproducir los sonidos de los botones
    private AudioSource audioSourceDialogos;

    // M�todos para reproducir sonidos SFX
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

    internal void playChooseDialog(int seleccionPersonaje)// Descomentar cuando se a�adan los 'DI�LOGOS.
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


    // M�todo para cambiar el volumen global
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
