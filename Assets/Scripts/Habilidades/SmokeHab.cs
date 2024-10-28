using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SmokeHab : IHabilidad
{
    private bool recargada;
    private const int TIEMPO_RECARGA = 45;  //segundos para recargar
    private const int DURACION_HUMO = 10;   //segundos que dura el humo
    private const int TIEMPO_DESDE_LANZAMIENTO = 2; //segundos desde que se lanza el bote de humo hasta que explota
    private const int FUERZA_LANZAMIENTO = 20;

    private PlayerController player;

    private GameObject boteHumo;
    private GameObject humo;

    public SmokeHab()
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
            boteHumo = GameObject.Instantiate(player.boteHumoPrefab, player.transform.position, Quaternion.identity);
            boteHumo.GetComponent<NetworkObject>().Spawn();
            Rigidbody rb = boteHumo.GetComponent<Rigidbody>();
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
        boteHumo = GameObject.Instantiate(player.boteHumoPrefab, player.transform.position, Quaternion.identity);
        Rigidbody rb = boteHumo.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(player.transform.forward * FUERZA_LANZAMIENTO, ForceMode.Impulse);
        }
    }

    IEnumerator DuracionHabilidad()
    {
        yield return new WaitForSeconds(TIEMPO_DESDE_LANZAMIENTO);

        Vector3 direccion = new Vector3(boteHumo.transform.position.x, boteHumo.transform.position.y + 1.5f, boteHumo.transform.position.z);
        DuracionHabilidad1ServerRpc(direccion);

        
        player.StartCoroutine(DuracionHabilidad2());

    }

    IEnumerator DuracionHabilidad2()
    {
        yield return new WaitForSeconds(DURACION_HUMO);
        DuracionHabilidad2ServerRpc();
    }


    [ServerRpc]
    public void DuracionHabilidad1ServerRpc(Vector3 direction)
    {
        if (player.IsServer)
        {
            humo = GameObject.Instantiate(player.esferaHumoPrefab, boteHumo.transform.position, Quaternion.identity);
            humo.GetComponent<NetworkObject>().Spawn();

            GameObject.Destroy(boteHumo);
            boteHumo.GetComponent<NetworkObject>().Despawn();
            DuracionHabilidad1ClientRpc(direction);
        }
    }


    [ClientRpc]
    public void DuracionHabilidad1ClientRpc(Vector3 direction)
    {
        humo = GameObject.Instantiate(player.esferaHumoPrefab, boteHumo.transform.position, Quaternion.identity);

        GameObject.Destroy(boteHumo);
    }

    [ServerRpc]
    public void DuracionHabilidad2ServerRpc()
    {
        if (player.IsServer)
        {
            GameObject.Destroy(humo);
            humo.GetComponent<NetworkObject>().Despawn();
            DuracionHabilidad2ClientRpc();
        }
    }

    [ClientRpc]
    public void DuracionHabilidad2ClientRpc()
    {
        GameObject.Destroy(humo);
    }

    IEnumerator Recarga()
    {
        Debug.Log("Habilidad recargando");
        yield return new WaitForSeconds(TIEMPO_RECARGA);
        Debug.Log("Habilidad cargada");
        recargada = true;
    }
}
