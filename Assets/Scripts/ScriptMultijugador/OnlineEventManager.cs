using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineEventManager : MonoBehaviour
{
    private static OnlineEventManager singleton;
    public static OnlineEventManager Singleton => singleton;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    [SerializeField] private string MainMenu = "MENÚ INICIO";
    private void Start()
    {
        DontDestroyOnLoad(this);
        NetworkManager.Singleton.OnClientStopped += OnDisconnect;

    }

    private void OnDisconnect(bool isHost)
    {
        SceneManager.LoadScene(MainMenu, LoadSceneMode.Single);
    }
}
