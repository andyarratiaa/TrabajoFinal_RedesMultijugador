using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkPlayerInitializer : NetworkBehaviour
{
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
    }
}
