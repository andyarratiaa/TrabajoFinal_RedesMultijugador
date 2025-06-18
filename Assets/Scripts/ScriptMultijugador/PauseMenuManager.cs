using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    private static PauseMenuManager staticReference;
    private bool isPaused = false;

    [SerializeField] private GameObject pauseMenuHolder;


    public static bool IsPaused
    {
        get
        {
            return staticReference != null && staticReference.isPaused;
        }
    }

    void Awake()
    {
        if (staticReference != null)
        {
            Destroy(staticReference.gameObject);
        }

        staticReference = this;
        pauseMenuHolder.SetActive(isPaused);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            UnityEngine.Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            UnityEngine.Cursor.visible = isPaused ? true : false;
            pauseMenuHolder.SetActive(isPaused);
        }
    }

    public void DisconnectFromServer()
    {
        NetworkManager.Singleton.Shutdown();
    }

    public void Exit()
    {
        Application.Quit();
    }


}



