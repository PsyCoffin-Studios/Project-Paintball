using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ExplosionHab : NetworkBehaviour, IHabilidad
{
    private bool recargadaExplosion = true;

    private const int TIEMPO_RECARGA = 3;  //segundos para recargar
    private const int DURACION_PINTURA = 10;   //segundos que dura el humo
    private const int TIEMPO_DESDE_LANZAMIENTO = 3; //segundos desde que se lanza el bote de humo hasta que explota
    private const int FUERZA_LANZAMIENTO = 20;

    public PlayerController player;

    private GameObject granada;
    private GameObject pintura;

    [SerializeField] TextMeshProUGUI textoDebug;
    [SerializeField] Transform origenLanzamiento; 

    public ExplosionHab()
    {
        //recargadaExplosion = true;
        //player = null;
    }

    public void Use()
    {
        if (recargadaExplosion)
        {
            if (IsOwner)
            { recargadaExplosion = false; }
            UseExplosionServerRpc();


            player.StartCoroutine(RecargaExplosion());
        }
        else
        {
            Debug.Log("Habilidad no disponible");
        }
    }


    [ServerRpc]
    public void UseExplosionServerRpc()
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
            granada = GameObject.Instantiate(
                player.granadaPrefab, origenLanzamiento.position, Quaternion.identity);

            granada.GetComponent<NetworkObject>().Spawn(true);
        }
        Rigidbody rb = granada.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce((player.cinemachineCamera.transform.forward + player.transform.up * 0.25f).normalized * FUERZA_LANZAMIENTO, ForceMode.Impulse);
        }
        player.StartCoroutine(DuracionHabilidad());

    }


    IEnumerator DuracionHabilidad()
    {
        yield return new WaitForSeconds(TIEMPO_DESDE_LANZAMIENTO);

        Vector3 direccion = new Vector3(granada.transform.position.x, granada.transform.position.y + 1.5f, granada.transform.position.z);
        pintura = GameObject.Instantiate(player.pinturaPrefab, direccion, Quaternion.identity);
        pintura.GetComponent<NetworkObject>().Spawn(true);
        granada.GetComponent<NetworkObject>().Despawn(true);

        yield return new WaitForSeconds(DURACION_PINTURA);
        pintura.GetComponent<NetworkObject>().Despawn(true);

    }

    IEnumerator RecargaExplosion()
    {
        Debug.Log("Habilidad recargando");
        textoDebug.GetComponent<TextMeshProUGUI>().text = "recargando habilidad...";
        yield return new WaitForSeconds(TIEMPO_RECARGA);
        Debug.Log("Habilidad cargada");
        recargadaExplosion = true;
        textoDebug.GetComponent<TextMeshProUGUI>().text = "habilidad lista";
    }


}
