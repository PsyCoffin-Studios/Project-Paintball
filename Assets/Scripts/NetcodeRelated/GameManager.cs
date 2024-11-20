using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private NetworkManager _networkManager;
    [SerializeField] private GameObject[] _playerPrefab;
    //public GameObject[] players;
    int connectedPlayers = 0;
    List<ulong> connectedPlayersIDs = new List<ulong>();

    public GameObject canvasInGame;
    public TeamDeathmatchManager teamDeathmatchManager;

    // Start is called before the first frame update
    void Start()
    {
        teamDeathmatchManager = GameObject.Find("@TeamDeathmatchManager").GetComponent<TeamDeathmatchManager>();
        _playerPrefab = new GameObject[4];
        //players = new GameObject[6];

        _networkManager = NetworkManager.Singleton;
        for(int i=0;i< _playerPrefab.Length; i++)
        {
            _playerPrefab[i] = _networkManager.NetworkConfig.Prefabs.Prefabs[i].Prefab;
        }
        _networkManager.OnServerStarted += OnServerStarted;
        _networkManager.OnClientConnectedCallback += OnClientConnected;
    }
    private void Update()
    {
        if(connectedPlayers>0)
        { UpdateTextServerRpc(); }
    }

    [ServerRpc]
    public void UpdateTextServerRpc()
    {
        if (IsServer)
        {
            string textPlayers = "";

            for(int i = 0; i < connectedPlayersIDs.Count; i++)
            {
                GameObject obj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[connectedPlayersIDs[i]].gameObject;
                if (obj != null)
                {
                    textPlayers += obj.GetComponent<Player>().ToString()+"\n";
                }
            }

            canvasInGame.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = textPlayers;
            UpdateTextClientRpc(textPlayers);
        }
    }

    [ClientRpc]
    public void UpdateTextClientRpc(string a)
    {
        canvasInGame.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = a;
    }

    private void OnServerStarted()
    {
        print("Server ready");
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnCharacterServerRpc(ulong clientId, string chosenCharacter)
    {
        //if (connectedPlayers <= players.Length)
        //{
            GameObject player;
            switch (chosenCharacter)
            {
                case "Hex":
                    player = Instantiate(_playerPrefab[0]);
                    break;
                case "Joker":
                    player = Instantiate(_playerPrefab[1]);
                    break;
                case "Revenant":
                    player = Instantiate(_playerPrefab[2]);
                    break;
                case "Outcast":
                    player = Instantiate(_playerPrefab[3]);
                    break;
                default:
                    player = null;
                    break;
            }
            if (player != null)
            {
                player.transform.position = new Vector3(0, 2, 0);
                connectedPlayers++;
                player.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
                connectedPlayersIDs.Add(player.GetComponent<NetworkObject>().NetworkObjectId);
                player.GetComponent<Player>().ID.Value=Convert.ToInt32(connectedPlayersIDs[connectedPlayersIDs.Count - 1]);
                if (connectedPlayers % 2 != 0)
                {
                    player.GetComponent<Player>().Team.Value = 1;
                    teamDeathmatchManager.Equipo1.Add(player);
                }
                else {
                    player.GetComponent<Player>().Team.Value = 2;
                    teamDeathmatchManager.Equipo2.Add(player);
            }
        }
        //}
    }

    private void OnClientConnected(ulong obj)
    {
        if (IsClient && obj == NetworkManager.Singleton.LocalClientId)
        {
            var chosenCharacter = DataBetweenScenes.instance.GetNombre();
            RequestSpawnCharacterServerRpc(obj, chosenCharacter);
        }
    }

    public void OnDestroy()
    {
        _networkManager.OnServerStarted -= OnServerStarted;
        _networkManager.OnClientConnectedCallback -= OnClientConnected;
    }

}