using UnityEngine;

public class AmbienceManager : MonoBehaviour
{
    public AudioSource audioSource; // AudioSource para reproducir el sonido
    public AudioClip ambienceClip; // Clip de sonido ambiente

    void Start()
    {
        // Si no hay AudioSource, lo agregamos dinámicamente
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configuración del AudioSource
        audioSource.clip = ambienceClip;
        audioSource.loop = true; // Que se repita en bucle
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D Sound
        audioSource.volume = 0.5f; // Ajusta el volumen

        PlayAmbience();
    }

    public void PlayAmbience()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopAmbience()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void AdjustVolume(float newVolume)
    {
        audioSource.volume = Mathf.Clamp(newVolume, 0f, 1f);
    }
}

