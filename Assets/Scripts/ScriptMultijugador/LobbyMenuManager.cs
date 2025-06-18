using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyMenuManager : MonoBehaviour
{
    [SerializeField] private string firstGameScene = "Level1";

    private void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }

    public void StartGame()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }
        NetworkManager.Singleton.SceneManager.LoadScene(firstGameScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
