// Puzzle2Door.cs
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class Puzzle2Door : NetworkBehaviour
{
    /* ────────────────────────────────────────────────
       ‖ Puertas registradas en la escena           ‖
       ─────────────────────────────────────────────── */
    private static readonly List<Puzzle2Door> allDoors = new();

    /* Todos los Animator (raíz + niños) de esta puerta */
    private Animator[] animators;

    /* Estado compartido: ¿la puerta ya está abierta? */
    [SerializeField]
    private NetworkVariable<bool> puertaAbierta =
        new NetworkVariable<bool>(false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

    /* ────────────────────────────────────────────────
       ‖ Ciclo de vida                               ‖
       ─────────────────────────────────────────────── */
    private void Awake()
    {
        /* Cachear TODOS los Animator bajo la puerta */
        animators = GetComponentsInChildren<Animator>(true);
        allDoors.Add(this);
    }

    public override void OnNetworkSpawn()
    {
        /* Sincronizar animación cuando cambie el valor */
        puertaAbierta.OnValueChanged += (_, opened) =>
        {
            if (opened) SetBoolOnAll();
        };

        /* Si ya estaba abierta al aparecer el objeto */
        if (puertaAbierta.Value)
            SetBoolOnAll();
    }

    public override void OnNetworkDespawn()
    {
        allDoors.Remove(this);
    }

    /* ────────────────────────────────────────────────
       ‖ Animación local                             ‖
       ─────────────────────────────────────────────── */
    private void SetBoolOnAll()
    {
        foreach (var anim in animators)
            if (anim != null)
                anim.SetBool("Abierto", true);   // 🡒 nombre exacto en el Animator
    }

    /* ────────────────────────────────────────────────
       ‖ Llamadas estáticas desde Puzzle2Manager      ‖
       ─────────────────────────────────────────────── */
    public static void SetAllDoorsOpen()
    {
        foreach (var door in allDoors)
            if (door != null && door.IsServer)
                door.puertaAbierta.Value = true;   // replica a todos los clientes
    }

    public static bool AreDoorsOpen()
    {
        foreach (var door in allDoors)
            if (!door.puertaAbierta.Value)
                return false;
        return true;
    }
}

