using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class SmokeHab : NetworkBehaviour, IHabilidad
{
    private bool recargadaSmoke = true;

    private const int TIEMPO_RECARGA = 4;//45;  //segundos para recargar
    private const int DURACION_HUMO = 10;   //segundos que dura el humo
    private const int TIEMPO_DESDE_LANZAMIENTO = 2; //segundos desde que se lanza el bote de humo hasta que explota
    private const int FUERZA_LANZAMIENTO = 20;

    public PlayerController player;

    private GameObject boteHumo;
    private GameObject humo;


    [SerializeField] TextMeshProUGUI textoDebug;

    [SerializeField] Transform origenLanzamiento;

    private void Start()
    { }



    public SmokeHab()
    {
        //recargadaSmoke = true;
    }

    public void Use()
    {
        if (recargadaSmoke)
        {
            recargadaSmoke = false;
            UseSmokeServerRpc();
            player.StartCoroutine(RecargaSmoke());
        }
        else
        {
            Debug.Log("Habilidad no disponible");
        }
    }

    [ServerRpc]
    public void UseSmokeServerRpc()
    {
        /* Logica
             *      Aparece prefab de bote de humo
             *      Lanza prefab con una fuerza en una direccion
             *      Prefab espera con una corrutina
             *      Se cambia el prefab de bote de humo por la esfera de humo
             *      Prefab espera corrutina
             *      Se destruye el prefab
            */
        if (IsServer)
        {
            boteHumo = GameObject.Instantiate(
                player.boteHumoPrefab,
                origenLanzamiento.position,
                Quaternion.identity);
            boteHumo.GetComponent<NetworkObject>().Spawn(true);
        }
        Rigidbody rb = boteHumo.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce((player.cinemachineCamera.transform.forward + player.transform.up * 0.25f).normalized * FUERZA_LANZAMIENTO, ForceMode.Impulse);
        }
        player.StartCoroutine(DuracionHabilidad());
        
        
    }


    IEnumerator DuracionHabilidad()
    {
        yield return new WaitForSeconds(TIEMPO_DESDE_LANZAMIENTO);

        Vector3 direccion = new Vector3(boteHumo.transform.position.x, boteHumo.transform.position.y + 1.5f, boteHumo.transform.position.z);
        humo = GameObject.Instantiate(player.esferaHumoPrefab, boteHumo.transform.position, Quaternion.identity);
        humo.GetComponent<NetworkObject>().Spawn(true);

        boteHumo.GetComponent<NetworkObject>().Despawn(true);
        yield return new WaitForSeconds(DURACION_HUMO);

        humo.GetComponent<NetworkObject>().Despawn(true);
    }


    IEnumerator RecargaSmoke()
    {
        Debug.Log("Habilidad recargando");
        textoDebug.GetComponent<TextMeshProUGUI>().text = "recargando habilidad...";
        yield return new WaitForSeconds(TIEMPO_RECARGA);
        Debug.Log("Habilidad cargada");
        recargadaSmoke = true;
        textoDebug.GetComponent<TextMeshProUGUI>().text = "habilidad lista";
    }



}
