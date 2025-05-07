using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;


namespace HelloWorld
{
    public class CinemachineCameraController : NetworkBehaviour
    {
        public CinemachineCamera vcam;

        public NetworkVariable<ushort> VcamPriority = new(0,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if(IsOwner)
            {
                vcam = GetComponent<CinemachineCamera>();
                AssignVCam();
            }
        }

        public void AssignVCam()
        {
            if(vcam == null)
            {
                Debug.Log("Vcam not found");
                return;
            }
           
            if (vcam.Priority == VcamPriority.Value)
            {
                vcam.Priority++;
                VcamPriority.Value++;
            }

            else if (vcam.Priority < VcamPriority.Value)
            {
                vcam.Priority += VcamPriority.Value;
                VcamPriority.Value++;
            }



        }

    }
}
