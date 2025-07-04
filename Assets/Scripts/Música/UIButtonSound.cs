using UnityEngine;

public class UIButtonSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;

    private bool allowPlay = false;

    void Start()
    {
        // Esperar un pequeño tiempo antes de permitir sonido
        Invoke(nameof(EnableSound), 0.2f);
    }

    void EnableSound()
    {
        allowPlay = true;
    }

    public void PlayClickSound()
    {
        if (!allowPlay) return;

        Debug.Log("PlayClickSound llamado");
        audioSource.PlayOneShot(clickSound);
    }

}


