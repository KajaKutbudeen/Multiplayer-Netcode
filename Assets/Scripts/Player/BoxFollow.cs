using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace HelloWorld
{
    public class BoxFollow: NetworkBehaviour
    {
        

        public NetworkVariable<ushort> speed = new(1, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }
    }
}
