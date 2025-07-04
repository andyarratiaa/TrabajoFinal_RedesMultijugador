using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class PuzzleDoor : NetworkBehaviour
{
    /* ---------- configuración ---------- */
    [Header("Audio")]
    [SerializeField] private AudioClip openClip;      // Efecto de abrir puerta
    [SerializeField] private bool playAtDoorPosition = true; // si quieres 3D-audio

    /* ---------- internals ---------- */
    private static readonly List<PuzzleDoor> allDoors = new();
    private Animator animator;
    private AudioSource audioSource;                  // fuente para el clip
    private bool soundPlayed = false;                 // evita doble reproducción

    private readonly NetworkVariable<bool> puertaAbierta = new(
        false, NetworkVariableReadPermission.Everyone,
               NetworkVariableWritePermission.Server);

    /* ---------- ciclo de vida ---------- */
    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        // Configura el AudioSource para 3D si se desea
        if (playAtDoorPosition)
        {
            audioSource.spatialBlend = 1f;  // 3D
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        }
        audioSource.playOnAwake = false;

        allDoors.Add(this);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        allDoors.Remove(this);
    }

    /* ---------- sincronización ---------- */
    public override void OnNetworkSpawn()
    {
        puertaAbierta.OnValueChanged += (_, newValue) =>
        {
            if (newValue) PlayOpen();
        };

        // Si ya estaba abierta al llegar a la escena
        if (puertaAbierta.Value) PlayOpen();
    }

    /* ---------- API estática ---------- */
    // Llamado por el PuzzleManager cuando corresponda
    public static void SetAllDoorsOpen()
    {
        foreach (var door in allDoors)
            if (door.IsServer)
                door.puertaAbierta.Value = true;   // sincroniza a todos
    }

    public static bool AreDoorsOpen()
    {
        foreach (var door in allDoors)
            if (!door.puertaAbierta.Value) return false;
        return true;
    }

    /* ---------- helpers ---------- */
    private void PlayOpen()
    {
        animator.SetBool("Abierto", true);

        if (soundPlayed || openClip == null) return; // evita doble sonido
        soundPlayed = true;
        audioSource.clip = openClip;
        audioSource.Play();
    }
}








