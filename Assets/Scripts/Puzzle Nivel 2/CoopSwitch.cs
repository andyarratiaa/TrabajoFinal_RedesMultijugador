using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject), typeof(Collider))]
public class CoopSwitch : NetworkBehaviour
{
    [SerializeField] private Material offMat, onMat;
    private MeshRenderer rend;

    private int pressCount = 0;

    public NetworkVariable<bool> IsPressed = new(
        false, NetworkVariableReadPermission.Everyone,
               NetworkVariableWritePermission.Server);

    private void Awake() => rend = GetComponent<MeshRenderer>();

    private void OnEnable()
    {
        IsPressed.OnValueChanged += (_, v) =>
            rend.material = v ? onMat : offMat;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (IsServer)
            RegisterPressServer(true);
        else
            RegisterPressServerRpc(true); 
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (IsServer)
            RegisterPressServer(false);
        else
            RegisterPressServerRpc(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RegisterPressServerRpc(bool down)
    {
        RegisterPressServer(down);    
    }

    private void RegisterPressServer(bool down)
    {
        pressCount = Mathf.Max(pressCount + (down ? 1 : -1), 0);

        bool prev = IsPressed.Value;
        IsPressed.Value = pressCount > 0;  

        if (prev != IsPressed.Value)
            CoopSwitchManager.Instance?.NotifySwitchChanged();
    }
}


