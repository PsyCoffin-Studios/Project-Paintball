using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBetweenScenes : MonoBehaviour
{

    //SINGLETON PARA PASAR DATOS ENTRE ESCENAS

    private string nombrePersonaje;
    private string arma;
    private string habilidad;

    public static DataBetweenScenes instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if(instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    public string GetNombre()
    {
        return nombrePersonaje;
    }

    public string GetArma()
    {
        return arma;
    }

    public void SetNombre(string name)
    {
       nombrePersonaje = name;
    }

    public void SetArma(string a)
    {
        arma = a;
    }

    internal string GetHabilidad()
    {
        return habilidad;
    }
    public void SetHabilidad(string a)
    {
        habilidad = a;
    }
}
