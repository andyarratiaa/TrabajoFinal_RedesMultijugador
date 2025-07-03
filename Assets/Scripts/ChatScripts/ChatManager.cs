using Unity.Netcode;
using UnityEngine;
using TMPro;

public class ChatManager : NetworkBehaviour
{
    public static bool IsChatOpen { get; private set; } = false;

    [Header("UI References")]
    [SerializeField] private GameObject chatMessagePrefab;   // Prefab del mensaje de chat (debe tener TMP_Text)
    [SerializeField] private Transform chatContent;          // Content del ScrollView
    [SerializeField] private TMP_InputField chatInput;       // InputField donde se escribe

    private bool chatOpen = false;

    private void Update()
    {
        if (Level1IntroUIManager.IsIntroOpen) return;   // ← NUEVO

        if (Level2IntroUIManager.IsIntroOpen) return;

        /* 1. NO permitir abrir chat si el Lose Panel está activo */
        if (Level2UIManager.IsLosePanelOpen) return;   // ← NUEVO

        // Abrir chat con T
        if (!chatOpen && Input.GetKeyDown(KeyCode.T))
            ToggleChat(true);

        // Si chat está abierto y se presiona Enter
        if (chatOpen && Input.GetKeyDown(KeyCode.Return))
        {
            if (!string.IsNullOrWhiteSpace(chatInput.text))
                SendMessageServerRpc(chatInput.text.Trim());

            chatInput.text = string.Empty;
            ToggleChat(false);
        }
    }

    private void ToggleChat(bool open)
    {
        chatOpen = open;
        IsChatOpen = open;

        chatInput.interactable = open;
        chatInput.text = string.Empty;

        if (open)
        {
            chatInput.ActivateInputField();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            chatInput.DeactivateInputField();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendMessageServerRpc(string message, ServerRpcParams rpcParams = default)
    {
        ulong senderId = rpcParams.Receive.SenderClientId;

        // Buscar el nombre real del jugador
        string playerName = $"Player {senderId}";

        foreach (var player in FindObjectsOfType<NetworkPlayerInitializer>())
        {
            if (player.OwnerClientId == senderId)
            {
                playerName = player.GetPlayerName();
                break;
            }
        }

        string displayMessage = $"{playerName}: {message}";
        BroadcastMessageClientRpc(displayMessage);
    }

    [ClientRpc]
    private void BroadcastMessageClientRpc(string message)
    {
        Debug.Log($"📨 Mensaje recibido: {message}");

        // Instanciar el mensaje en el ScrollView
        GameObject instance = Instantiate(chatMessagePrefab, chatContent);

        TMP_Text textComponent = instance.GetComponentInChildren<TMP_Text>();
        if (textComponent != null)
        {
            textComponent.text = message;
        }
        else
        {
            Debug.LogWarning("❌ No se encontró TMP_Text en el prefab del mensaje.");
        }
    }
}

