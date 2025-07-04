using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class PuzzleDoor : NetworkBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip openClip;   
    [SerializeField] private bool playAtDoorPosition = true; 

    private static readonly List<PuzzleDoor> allDoors = new();
    private Animator animator;
    private AudioSource audioSource;                 
    private bool soundPlayed = false;                

    private readonly NetworkVariable<bool> puertaAbierta = new(
        false, NetworkVariableReadPermission.Everyone,
               NetworkVariableWritePermission.Server);


    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();


        if (playAtDoorPosition)
        {
            audioSource.spatialBlend = 1f;  
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

    public override void OnNetworkSpawn()
    {
        puertaAbierta.OnValueChanged += (_, newValue) =>
        {
            if (newValue) PlayOpen();
        };

        if (puertaAbierta.Value) PlayOpen();
    }


    public static void SetAllDoorsOpen()
    {
        foreach (var door in allDoors)
            if (door.IsServer)
                door.puertaAbierta.Value = true; 
    }

    public static bool AreDoorsOpen()
    {
        foreach (var door in allDoors)
            if (!door.puertaAbierta.Value) return false;
        return true;
    }


    private void PlayOpen()
    {
        animator.SetBool("Abierto", true);

        if (soundPlayed || openClip == null) return; 
        soundPlayed = true;
        audioSource.clip = openClip;
        audioSource.Play();
    }
}








