using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class DashHab : NetworkBehaviour, IHabilidad
{
    private bool recargadaDash = true;
    private bool activaDash = false;
    private const int FUERZA_DASH = 20000; // velocidad * potenciador al usar el dash
    private const int TIEMPO_RECARGA = 15; //segundos

    public PlayerController player;


    [SerializeField] TextMeshProUGUI textoDebug;
    private void Start()
    { }

    public DashHab()
    {
        //recargadaDash = true;
        //activaDash = false;
    }
    public void Use()
    {
        UseDashServerRpc();
    }

    [ServerRpc(RequireOwnership = true)]
    public void UseDashServerRpc()
    {
        if (IsServer)
        {
            if (recargadaDash && !activaDash)
            {
                activaDash = true;
                Debug.Log("Dash");
                Vector3 direccionDash = player.transform.forward;
                player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y+0.25f, player.transform.position.z);
                player.GetComponent<Rigidbody>().AddForce(direccionDash.normalized * FUERZA_DASH, ForceMode.Impulse);
                Debug.Log("Dash terminado");
                activaDash = false;
                player.StartCoroutine(RecargaDash());
            }
            else if (recargadaDash && activaDash)
            {

            }
            else
            {
                // no esta recargadaDash
            }

            UseDashClientRpc();
        }
        
    }

    IEnumerator RecargaDash()
    {
        recargadaDash = false;
        textoDebug.GetComponent<TextMeshProUGUI>().text = "recargando habilidad...";
        UseDashClientRpc();
        yield return new WaitForSeconds(TIEMPO_RECARGA);
        Debug.Log("Habilidad cargada");
        recargadaDash = true;
        textoDebug.GetComponent<TextMeshProUGUI>().text = "habilidad lista";
        UseDashClientRpc();
    }

    [ClientRpc]
    public void UseDashClientRpc()
    {
    }
}
