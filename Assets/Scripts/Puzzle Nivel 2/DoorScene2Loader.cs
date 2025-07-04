using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine;



public class DoorScene2Loader : NetworkBehaviour
{
    [SerializeField] private string nextSceneName = "Level3";

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (!other.CompareTag("Player")) return;

        if (Puzzle2Door.AreDoorsOpen())
        {
            Debug.Log("Puertas del nivel 2 abiertas. Cargando siguiente escena...");

            NetworkManager.Singleton.SceneManager.LoadScene(nextSceneName, LoadSceneMode.Additive);

            Invoke(nameof(UnloadCurrentScene), 3f);
        }
        else
        {
            Debug.Log("❌ Las puertas del nivel 2 siguen cerradas.");
        }
    }

    private void UnloadCurrentScene()
    {
        var activeScene = SceneManager.GetActiveScene();
        if (activeScene.isLoaded && activeScene.name != nextSceneName)
        {
            SceneManager.UnloadSceneAsync(activeScene);
            Debug.Log("Escena anterior descargada.");
        }
    }
}
