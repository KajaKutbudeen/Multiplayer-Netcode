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

    private void HandleServerConnectedCallback(ulong clientId)
    {

    }
    private void HandleClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
            return; // Skip host
        Debug.Log("Client connected");
        GameObject clientObj = Instantiate(enemyprefab, p2.position, Quaternion.identity);
        clientObj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);


        //   SpawnPrefab(ClientPrefab);    
    }
}
