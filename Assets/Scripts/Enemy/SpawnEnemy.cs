using UnityEngine;
using Unity.Netcode;
public class SpawnEnemy : NetworkBehaviour
{
    public GameObject enemyprefab;
    public Transform p2;
    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            
           GameObject obj = Instantiate(enemyprefab , p2.transform.position,Quaternion.identity);
            obj.GetComponent<NetworkObject>().Spawn();

        }
        if (IsClient)
        {
         //   NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
       //     NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisConnected;
        }
    }

}
