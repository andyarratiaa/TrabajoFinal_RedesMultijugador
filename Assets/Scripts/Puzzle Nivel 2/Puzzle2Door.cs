using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class Puzzle2Door : NetworkBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip openClip;           
    [SerializeField] private bool playAtDoorPosition = true; 


    private static readonly List<Puzzle2Door> allDoors = new();

    private Animator[] animators;
    private AudioSource audioSource;
    private bool soundPlayed = false;

    /* Estado compartido: ¿puerta abierta? */
    private readonly NetworkVariable<bool> puertaAbierta = new(
        false, NetworkVariableReadPermission.Everyone,
               NetworkVariableWritePermission.Server);


    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>(true);


        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (playAtDoorPosition)
        {
            audioSource.spatialBlend = 1f;                
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        }

        allDoors.Add(this);
    }

    public override void OnNetworkSpawn()
    {
        puertaAbierta.OnValueChanged += (_, opened) =>
        {
            if (opened) PlayOpen();
        };

        if (puertaAbierta.Value)
            PlayOpen();
    }

    public override void OnNetworkDespawn() => allDoors.Remove(this);

    private void PlayOpen()
    {
        foreach (var anim in animators)
            if (anim != null) anim.SetBool("Abierto", true);

        if (soundPlayed || openClip == null) return;
        soundPlayed = true;
        audioSource.clip = openClip;
        audioSource.Play();
    }

    public static void SetAllDoorsOpen()
    {
        foreach (var door in allDoors)
            if (door != null && door.IsServer)
                door.puertaAbierta.Value = true;   
    }

    public static bool AreDoorsOpen()
    {
        foreach (var door in allDoors)
            if (!door.puertaAbierta.Value) return false;
        return true;
    }
}


