using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    [SerializeField] TMPro.TMP_InputField joinCodeInputField;


    private async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async Task <string> StartHostWithRelay(int maxConnections = 4)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            return NetworkManager.Singleton.StartHost() ? joinCode : null;
        }
        catch
        {
            Debug.LogError("Allocation failed");
            throw;
        }
    }

    public async void StartRelay()
    {
        string joinCode = await StartHostWithRelay();
        JoinCodeUI.Singleton.JoinCodeText.text = "(Host) Invite Code: " + joinCode;
    }

    public async void JoinRelay()
    {
        bool joinResult = await StartClientWithRelay(joinCodeInputField.text);
        if(joinResult)
        {
            JoinCodeUI.Singleton.JoinCodeText.text = "(Client) Invite Code: " + joinCodeInputField.text;

        }
    }

    private async Task<bool> StartClientWithRelay(string joinCode)
    {
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }
}
