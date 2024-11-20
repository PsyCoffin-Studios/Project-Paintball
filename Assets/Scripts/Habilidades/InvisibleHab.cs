using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class InvisibleHab : NetworkBehaviour, IHabilidad
{
    private bool activaInvisible = false;
    private bool recargadaInvisible = true;
    

    private const int TIEMPO_RECARGA = 30; //segundos
    private const int TIEMPO_DE_USO = 5; //segundos

    public PlayerController player;
    [SerializeField] private Renderer[] renderers;


    [SerializeField] TextMeshProUGUI textoDebug;

    private void Start()
    {
        
    }


    public InvisibleHab()
    {
        //activaInvisible = false;
        //recargadaInvisible = true;
    }

    public void Use()
    {
            //renderers = player.GetComponentsInChildren<Renderer>();
            if (activaInvisible)
            {
                RendererServerRpc(true);
                player.StartCoroutine(RecargaInvisible());
            }
            else if (!activaInvisible && recargadaInvisible)
            {
                Debug.Log("Habilidad usando");
                player.StartCoroutine(TiempoDeUso());
                RendererServerRpc(false);
            }
            else if (!activaInvisible && !recargadaInvisible)
            {
                // no pasa nada, o avisar en la interfaz de que no esta lista
                Debug.Log("Habilidad no esta preparada");
            }
    }

    IEnumerator RecargaInvisible()
    {
        activaInvisible = false;
        textoDebug.GetComponent<TextMeshProUGUI>().text = "recargando habilidad...";
        Debug.Log("Habilidad recargando");
        yield return new WaitForSeconds(TIEMPO_RECARGA);
        Debug.Log("Habilidad cargada");
        recargadaInvisible = true;
        textoDebug.GetComponent<TextMeshProUGUI>().text = "habilidad lista";
    }
    IEnumerator TiempoDeUso()
    {
        recargadaInvisible = false;
        yield return new WaitForSeconds(TIEMPO_DE_USO);
        Debug.Log("Habilidad terminada");
        RendererServerRpc(true);
        player.StartCoroutine(RecargaInvisible());
    }

    [ServerRpc]
    public void RendererServerRpc(bool render)
    {
        if (IsServer)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = render;
            }
        }
        RendererClientRpc(render);
    }

    [ClientRpc(RequireOwnership = false)]
    public void RendererClientRpc(bool render)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = render;
        }
    }
}