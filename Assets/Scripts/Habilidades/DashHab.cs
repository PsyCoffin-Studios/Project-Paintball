using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashHab : IHabilidad
{
    private bool recargada;
    private bool activa;
    private const int FUERZA_DASH = 20000; // velocidad * potenciador al usar el dash
    private const int TIEMPO_RECARGA = 15; //segundos

    private PlayerController player;

    public DashHab()
    {
        recargada = true;
        activa = false;
        player = null;
    }
    public void Use(PlayerController p)
    {
        player = p;
        if (recargada && !activa)
        {
            activa = true;
            Debug.Log("Dash");
            Vector3 direccionDash = player.transform.forward;
            player.GetComponent<Rigidbody>().AddForce(direccionDash.normalized * FUERZA_DASH, ForceMode.Impulse);
            Debug.Log("Dash terminado");
            activa = false;
            player.StartCoroutine(Recarga());
        }
        else if (recargada && activa)
        {

        }
        else
        {
            // no esta recargada
        }
    }
    IEnumerator Recarga()
    {
        recargada = false;
        yield return new WaitForSeconds(TIEMPO_RECARGA);
        Debug.Log("Habilidad cargada");
        recargada = true;
    }
}
