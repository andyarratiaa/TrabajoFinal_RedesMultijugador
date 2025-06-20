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

        Debug.Log("🚪 Cambio de escena para todos.");
        NetworkManager.Singleton.SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
    }
}









