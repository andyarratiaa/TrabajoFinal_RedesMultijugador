using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlPuerta : MonoBehaviour
{
    //Animator anim;
    //public bool Dentro = false;
    //bool puerta = false;
    //public bool tieneLlave = false; //La puerta solo se abre si tiene la llave
    //public string nombreEscenaDestino; //Nombre de la escena a la que se cambiará
    //public Transform posicionLlegada; //Posición donde aparecerá el jugador en la siguiente escena

    //private AudioSource audioSource; //AudioSource para sonidos de la puerta
    //public AudioClip openDoor;

    //public GameObject objetoActivar; //El GameObject que se activará al obtener la llave

    //private void Awake()
    //{
    //    audioSource = GetComponent<AudioSource>();
    //    if (audioSource == null)
    //    {
    //        audioSource = gameObject.AddComponent<AudioSource>();
    //    }
    //}

    //void Start()
    //{
    //    anim = GetComponent<Animator>();

    //    //Asegurarse de que el objeto está desactivado al inicio
    //    if (objetoActivar != null)
    //    {
    //        objetoActivar.SetActive(false);
    //    }
    //}

    //void OnTriggerEnter(Collider col)
    //{
    //    if (col.CompareTag("Player"))
    //    {
    //        Dentro = true;
    //    }
    //}

    //void OnTriggerExit(Collider col)
    //{
    //    if (col.CompareTag("Player"))
    //    {
    //        Dentro = false;
    //    }
    //}

    //void Update()
    //{
    //    if (Dentro && Input.GetKeyDown(KeyCode.E))
    //    {
    //        if (tieneLlave) //Si el jugador tiene la llave correcta
    //        {
    //            puerta = !puerta;
    //            anim.SetBool("Abierto", puerta);

    //            if (puerta) //Si la puerta se abre, cambiar de escena
    //            {
    //                PlayDoorSound();
    //                StartCoroutine(CambiarEscena());
    //            }
    //        }
    //        else
    //        {
    //            Debug.Log("Necesitas la llave para abrir esta puerta.");
    //        }
    //    }
    //}

    ////Método para asignar la llave a esta puerta (llamado desde el script de la llave)
    //public void ObtenerLlave()
    //{
    //    tieneLlave = true;
    //    Debug.Log("Has obtenido la llave para estas puertas.");

    //    // 🔥 Activar el objeto cuando se obtiene la llave
    //    if (objetoActivar != null)
    //    {
    //        objetoActivar.SetActive(true);
    //    }
    //}

    ////Método para cambiar de escena con una pequeña espera
    //IEnumerator CambiarEscena()
    //{
    //    //Guardar la salud del jugador antes de cambiar de escena
    //    if (FindObjectOfType<PlayerHealthManager>() != null)
    //    {
    //        PlayerPrefs.SetFloat("PlayerHealth", FindObjectOfType<PlayerHealthManager>().currentHealth);
    //        PlayerPrefs.Save();
    //    }

    //    yield return new WaitForSeconds(1.25f); //Pequeña pausa antes de cambiar de escena

    //    //Guardar la posición de llegada en PlayerPrefs
    //    if (posicionLlegada != null)
    //    {
    //        PlayerPrefs.SetFloat("PosX", posicionLlegada.position.x);
    //        PlayerPrefs.SetFloat("PosY", posicionLlegada.position.y);
    //        PlayerPrefs.SetFloat("PosZ", posicionLlegada.position.z);
    //    }

    //    SceneManager.LoadScene(nombreEscenaDestino);
    //}

    //void PlayDoorSound()
    //{
    //    if (openDoor != null && audioSource != null)
    //    {
    //        audioSource.PlayOneShot(openDoor);
    //    }
    //}
}







