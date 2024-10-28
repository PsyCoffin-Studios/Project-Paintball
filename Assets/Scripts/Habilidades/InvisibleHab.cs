using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleHab : IHabilidad
{
    private bool activa;
    private bool recargada;
    private const int TIEMPO_RECARGA = 30; //segundos
    private const int TIEMPO_DE_USO = 5; //segundos

    private PlayerController player;
    private Renderer[] renderers;

    public InvisibleHab()
    {
        activa = false;
        recargada = true;
        player = null;
        renderers = null;
    }
    public void Use(PlayerController p)
    {
        player = p;
        renderers = player.GetComponentsInChildren<Renderer>();

        if (activa)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true;
            }
                player.StartCoroutine(Recarga());
        }
        else if (!activa && recargada)
        {
            Debug.Log("Habilidad usando");
            player.StartCoroutine(TiempoDeUso());
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }
        }
        else if (!activa && !recargada)
        {
            // no pasa nada, o avisar en la interfaz de que no esta lista
            Debug.Log("Habilidad no esta preparada");
        }
    }
    IEnumerator Recarga()
    {
        activa = false;
        Debug.Log("Habilidad recargando");
        yield return new WaitForSeconds(TIEMPO_RECARGA);
        Debug.Log("Habilidad cargada");
        recargada = true;
    }
    IEnumerator TiempoDeUso()
    {
        recargada = false;
        yield return new WaitForSeconds(TIEMPO_DE_USO);
        Debug.Log("Habilidad terminada");
        activa = false;
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
        }
        player.StartCoroutine(Recarga());
    }
}