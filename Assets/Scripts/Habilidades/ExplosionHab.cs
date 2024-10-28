using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ExplosionHab : IHabilidad
{
    private bool recargada;
    private const int TIEMPO_RECARGA = 60;  //segundos para recargar
    private const int DURACION_PINTURA = 10;   //segundos que dura el humo
    private const int TIEMPO_DESDE_LANZAMIENTO = 3; //segundos desde que se lanza el bote de humo hasta que explota
    private const int FUERZA_LANZAMIENTO = 20;

    private PlayerController player;

    private GameObject granada;
    private GameObject pintura;

    public ExplosionHab()
    {
        recargada = true;
        player = null;
    }

    public void Use(PlayerController p)
    {
        player = p;
        if (recargada)
        {
            recargada = false;
            UseServerRpc();
        }
        else
        {
            Debug.Log("Habilidad no disponible");
        }
    }


    [ServerRpc]
    public void UseServerRpc()
    {
        /* Logica
             *      Aparece prefab de bote de humo
             *      Lanza prefab con una fuerza en una direccion
             *      Prefab espera con una corrutina
             *      Se cambia el prefab de bote de humo por la esfera de humo
             *      Prefab espera corrutina
             *      Se destruye el prefab
            */
        if (player.IsServer)
        {
            granada = GameObject.Instantiate(player.granadaPrefab, player.transform.position, Quaternion.identity);
            granada.GetComponent<NetworkObject>().Spawn();

            Rigidbody rb = granada.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(player.transform.forward * FUERZA_LANZAMIENTO, ForceMode.Impulse);
            }
            UseClientRpc();

            player.StartCoroutine(DuracionHabilidad());
            player.StartCoroutine(Recarga());

        }
    }

    [ClientRpc]
    public void UseClientRpc()
    {
        granada = GameObject.Instantiate(player.granadaPrefab, player.transform.position, Quaternion.identity);
        Rigidbody rb = granada.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(player.transform.forward * FUERZA_LANZAMIENTO, ForceMode.Impulse);
        }
    }


    IEnumerator DuracionHabilidad()
    {
        yield return new WaitForSeconds(TIEMPO_DESDE_LANZAMIENTO);

        Vector3 direccion = new Vector3(granada.transform.position.x, granada.transform.position.y + 1.5f, granada.transform.position.z);
        DuracionHabilidad1ServerRpc(direccion);

        player.StartCoroutine(DuracionHabilidad2());

    }

    IEnumerator DuracionHabilidad2()
    {
        yield return new WaitForSeconds(DURACION_PINTURA);
        DuracionHabilidad2ServerRpc();
    }

    [ServerRpc]

    public void DuracionHabilidad1ServerRpc(Vector3 direction)
    {
        if (player.IsServer)
        {
            pintura = GameObject.Instantiate(player.pinturaPrefab, direction, Quaternion.identity);
            pintura.GetComponent<NetworkObject>().Spawn();

            GameObject.Destroy(granada);
            granada.GetComponent<NetworkObject>().Despawn();
            DuracionHabilidad1ClientRpc(direction);
        }
    }


    [ClientRpc]
    public void DuracionHabilidad1ClientRpc(Vector3 direction)
    {
        pintura = GameObject.Instantiate(player.pinturaPrefab, direction, Quaternion.identity);
        GameObject.Destroy(granada);
    }

    [ServerRpc]
    public void DuracionHabilidad2ServerRpc()
    {
        if (player.IsServer)
        {
            GameObject.Destroy(pintura);
            pintura.GetComponent<NetworkObject>().Despawn();
            DuracionHabilidad2ClientRpc();
        }
    }


    [ClientRpc]
    public void DuracionHabilidad2ClientRpc()
    {
        GameObject.Destroy(pintura);
    }

    IEnumerator Recarga()
    {
        Debug.Log("Habilidad recargando");
        yield return new WaitForSeconds(TIEMPO_RECARGA);
        Debug.Log("Habilidad cargada");
        recargada = true;
    }
}
