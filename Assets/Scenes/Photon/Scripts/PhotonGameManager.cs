using Photon.Pun;
using UnityEngine;


namespace Photon
{
    public class PhotonGameManager : MonoBehaviourPunCallbacks
    {
        public Transform[] dest;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            

            Debug.Log("Player count: " +PhotonNetwork.CurrentRoom.PlayerCount);
            int id = PhotonNetwork.LocalPlayer.ActorNumber;
            Debug.Log("Id No: "+id);
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate("Riode", dest[0].position, Quaternion.identity);
            }
            else
            {
                PhotonNetwork.Instantiate("Stark", dest[id -1].position, Quaternion.identity);
            }
        }


    }
}
