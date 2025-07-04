using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Collections;
using TMPro;

public class NetworkPlayerInitializer : NetworkBehaviour
{
    private NetworkVariable<FixedString64Bytes> playerName = new NetworkVariable<FixedString64Bytes>(
        writePerm: NetworkVariableWritePermission.Server);

    [SerializeField] private TMP_Text nameText;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (NetworkObject.IsOwner)
        {
            Debug.Log("Player object Spawned");
            FindObjectOfType<CameraController>().InitCameraSystem(this.transform);
        }
        else
        {
            this.transform.tag = "OnlineCharacter";
            if (this.TryGetComponent(out CharacterController characterControllerComponent))
            {
                CapsuleCollider capsule = this.AddComponent<CapsuleCollider>();
                capsule.height = characterControllerComponent.height;
                capsule.radius = characterControllerComponent.radius;
                capsule.center = characterControllerComponent.center;

                Destroy(characterControllerComponent);
            }

            if (this.TryGetComponent(out ThirdPersonController thirdPersonComponent))
            {
                Destroy(thirdPersonComponent);
            }
        }

        if (IsOwner && IsClient)
        {
            SendNameToServerRpc(PlayerDataHandler.Instance.PlayerName);
        }

        if (IsServer)
        {
            playerName.OnValueChanged += OnNameChanged;
        }

        Debug.Log($"Player spawned. Name: {playerName.Value}");
    }

    [ServerRpc]
    private void SendNameToServerRpc(string name)
    {
        playerName.Value = name;
        Debug.Log($"Server received player name: {name}");
    }

    private void OnNameChanged(FixedString64Bytes oldName, FixedString64Bytes newName)
    {
        Debug.Log($"Player name changed: {newName}");
        if (nameText != null)
        {
            nameText.text = newName.ToString();
        }
    }

    private void Update()
    {
        if (nameText != null)
        {
            nameText.text = playerName.Value.ToString();
        }
    }


    public string GetPlayerName()
    {
        return playerName.Value.ToString();
    }
}
