using UnityEngine;

public class Llave : MonoBehaviour
{
    //public ControlPuerta puerta1; // Primera puerta que abre esta llave
    //public ControlPuerta puerta2; // Segunda puerta que abre esta llave
    //public AudioClip sonidoRecoger; // Sonido de recoger llave

    //private AudioSource audioSource;
    //private bool recogida = false; // Para evitar que se active varias veces

    //void Start()
    //{
    //    // Si no hay un AudioSource en el objeto, lo añadimos
    //    audioSource = GetComponent<AudioSource>();
    //    if (audioSource == null)
    //    {
    //        audioSource = gameObject.AddComponent<AudioSource>();
    //    }
    //}

    //void OnTriggerEnter(Collider col)
    //{
    //    if (col.CompareTag("Player") && !recogida)
    //    {
    //        recogida = true; // Evita que se active múltiples veces

    //        if (puerta1 != null && puerta2 != null)
    //        {
    //            puerta1.ObtenerLlave(); // Da acceso a la primera puerta
    //            puerta2.ObtenerLlave(); // Da acceso a la segunda puerta
    //        }

    //        Debug.Log("Has recogido la llave para estas dos puertas.");

    //        // Reproducir sonido y desactivar visualmente el objeto antes de destruirlo
    //        if (sonidoRecoger != null && audioSource != null)
    //        {
    //            audioSource.PlayOneShot(sonidoRecoger);
    //            GetComponent<MeshRenderer>().enabled = false; // Oculta la llave
    //            GetComponent<Collider>().enabled = false; // Desactiva la colisión
    //            Destroy(gameObject, sonidoRecoger.length); // Espera a que termine el sonido antes de destruir
    //        }
    //        else
    //        {
    //            Destroy(gameObject); // Si no hay sonido, destruye inmediatamente
    //        }
    //    }
    //}
}





