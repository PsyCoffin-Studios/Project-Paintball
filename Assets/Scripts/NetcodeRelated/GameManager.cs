using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private NetworkManager _networkManager;
    private GameObject _playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        _networkManager = NetworkManager.Singleton;
        _playerPrefab = _networkManager.NetworkConfig.Prefabs.Prefabs[0].Prefab;
        _networkManager.OnServerStarted += OnServerStarted;
        _networkManager.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnServerStarted()
    {
        print("Server ready");
    }

    private void OnClientConnected(ulong obj)
    {
        if (IsServer)
        {
            var player = Instantiate(_playerPrefab);
            player.GetComponent<NetworkObject>().SpawnWithOwnership(obj);
        }
    }

    public void OnDestroy()
    {
        _networkManager.OnServerStarted -= OnServerStarted;
        _networkManager.OnClientConnectedCallback -= OnClientConnected;
    }
}