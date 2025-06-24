using Unity.Netcode;
using UnityEngine;

namespace HelloWorld {
    public class NetworkCharacterSwitch : NetworkBehaviour
    {       

        public GameObject hostPrefab;
        public GameObject ClientPrefab;

        public Transform P1;
        public Transform P2;

        public NetworkVariable<bool> Grounded = new NetworkVariable<bool>(default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner

            );

        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

        public override void OnNetworkSpawn()
        {
            Grounded.Value = false;

            if (IsServer)
            {               
                if(IsHost)
                {
                    GameObject hostObj = Instantiate(hostPrefab, P1.transform.position , Quaternion.identity);
                    hostObj.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);
                }
                NetworkManager.Singleton.OnServerStarted += HandleServerConneceted;
                NetworkManager.Singleton.OnClientStopped += HandleServerDisConnected;

             
            }

            if (IsClient)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisConnected;              
            }
        }

      
        public override void OnNetworkDespawn()
        {
            if(IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisConnected;
                NetworkManager.Singleton.OnServerStarted -= HandleServerConneceted;
                NetworkManager.Singleton.OnServerStopped -= HandleServerDisConnected;
            }
        }

        private void HandleServerConneceted()
        {
            Debug.Log("Server Connected");
           
        }
       
        
        private void HandleServerDisConnected(bool dis)
        {
            Debug.Log("Server DisConnected");
        }
        private void HandleClientConnected(ulong clientId) 
        {
            if (clientId == NetworkManager.Singleton.LocalClientId)
                return; // Skip host
            Debug.Log("Client connected");
            GameObject clientObj = Instantiate(ClientPrefab, P2.position, Quaternion.identity);
            clientObj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

            
            //   SpawnPrefab(ClientPrefab);    
        }

        private void HandleClientDisConnected(ulong clientId) 
        {
            Debug.Log("client DisConnected");
        }
    }
}
