using Photon.Pun;
using TMPro;
using UnityEngine;


namespace Photon {
    public class ConnectionPanel : MonoBehaviour
    {

        public TextMeshProUGUI ConnectionStatusText;

        private void Update()
        {
            ConnectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;
        }
    }
}