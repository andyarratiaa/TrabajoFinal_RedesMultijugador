using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class DoorSceneLoader : NetworkBehaviour
{
    [SerializeField] private string nextSceneName = "Level2";

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (!other.CompareTag("Player")) return;


        if (PuzzleDoor.AreDoorsOpen())
        {
            Debug.Log("Todas las puertas abiertas. Cambiando de escena...");
            NetworkManager.Singleton.SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("Las puertas aún no están abiertas.");
        }
    }
}











