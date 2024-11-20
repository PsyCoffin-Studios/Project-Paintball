using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeamDeathmatchManager : MonoBehaviour
{
    //datos de muertes
    private int muertesEquipo1 = 0;
    private int muertesEquipo2 = 0;
    private int muertesParaGanar = 50;

    [SerializeField] GameObject spawnEquipo1;
    [SerializeField] GameObject spawnEquipo2;
    [SerializeField] TextMeshProUGUI textMeshProUGUI;

    public List<GameObject> Equipo1;
    public List<GameObject> Equipo2;

    void Start()
    {
        textMeshProUGUI.text = "EQUIPO 1: " + muertesEquipo1 + "/EQUIPO 2: " + muertesEquipo2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //necesita un mutex esta funcion para la suma de datos?
    public void SumarMuertes(int equipo)
    {
        if (equipo == 1)
        {
            muertesEquipo1++;
            if (muertesEquipo1 >= muertesParaGanar)
            {
                //gana el equipo 1
                EndMatch(1);
            }
        }
        else
        {
            muertesEquipo2++;
            if (muertesEquipo2 >= muertesParaGanar)
            {
                //gana el equipo 2
                EndMatch(2);
            }
        }
        textMeshProUGUI.text = "EQUIPO 1: "+muertesEquipo1+ "/EQUIPO 2: " + muertesEquipo2;
    }

    public void Respawn(GameObject player)
    {
        if (player.GetComponent<Player>().Team.Value == 1)
        {
            int index = Equipo1.IndexOf(player);
            player.transform.position = spawnEquipo1.transform.GetChild(index).position;
        }
        else
        {
            int index = Equipo2.IndexOf(player);
            player.transform.position = spawnEquipo2.transform.GetChild(index).position;
        }
        player.GetComponent<Player>().Health.Value = 100;
    }

    public void EndMatch(int i)
    {
        //holaaaaaa
    }
}
