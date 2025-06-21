using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorScene2Loader : NetworkBehaviour
{
    [SerializeField] private string nextSceneName = "Level3";

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (!other.CompareTag("Player")) return;

        if (Puzzle2Door.AreDoorsOpen())
        {
            Debug.Log("🚪 Puertas del nivel 2 abiertas. Cargando siguiente escena...");
            NetworkManager.Singleton.SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("❌ Las puertas del nivel 2 siguen cerradas.");
        }
    }
}
