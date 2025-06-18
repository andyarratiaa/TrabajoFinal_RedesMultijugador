using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;

public class LobbyMenuLoader : MonoBehaviour
{
    [SerializeField] private string lobbyMenuScene = "LOBBY";

    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnClientConnectLoadLobby;
    }

    private void OnClientConnectLoadLobby()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(lobbyMenuScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= OnClientConnectLoadLobby;
        }
    }
}
