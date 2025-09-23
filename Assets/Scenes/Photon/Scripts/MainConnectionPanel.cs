using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Photon.Pun.Demo.Asteroids;
using System.Collections;
namespace Photon
{
    public class MainConnectionPanel : MonoBehaviourPunCallbacks
    {
        public GameObject LoginPanel;

        public  TMP_InputField PlayerNameInput;

        [Header("Selection Panel")]
        public GameObject SelectionPanel;

        [Header("Create Room Panel")]
        public GameObject CreateRoomPanel;

        public TMP_InputField RoomNameInputField;
        public TMP_InputField MaxPlayersInputField;

        [Header("Join Random Room Panel")]
        public GameObject JoinRandomRoomPanel;

        [Header("Room List Panel")]
        public GameObject RoomListPanel;

        public GameObject RoomListContent;
        public GameObject RoomListEntryPrefab;

        [Header("Inside Room Panel")]
        public GameObject InsideRoomPanel;

        public Button StartGameButton;
        public GameObject PlayerListEntryPrefab;

        public Dictionary<string, RoomInfo> cachednamelist;
        public Dictionary<string, RoomInfo> roomListEntries;
        public Dictionary<int, GameObject> playerListEntries;

        #region Unity
        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            cachednamelist = new Dictionary<string, RoomInfo>();
            roomListEntries = new Dictionary<string, RoomInfo>();

            PlayerNameInput.text = "Player" + Random.Range(0, 100);
        }
        #endregion

        #region InitialStage
        public void OnLoginButtonClicked()
        {
            string playername = PlayerNameInput.text;

            if(!playername.Equals(""))
            {
                PhotonNetwork.LocalPlayer.NickName = playername;
                PhotonNetwork.ConnectUsingSettings();
            }

            else
            {
                Debug.Log("Player name invalid");
            }
        }


        public override void OnConnectedToMaster()
        {
            this.SetActivePanel(SelectionPanel.name);
        }

      public void OnCreateRoomButtonClicked()
        {
            string roomname = RoomNameInputField.text;
            roomname = (roomname.Equals(string.Empty)) ? "Room" + Random.Range(0, 100) : roomname;
            byte maxplayers;
            byte.TryParse(MaxPlayersInputField.text, out maxplayers);
            maxplayers = (byte) Mathf.Clamp(maxplayers, 2, 8);

            RoomOptions options = new RoomOptions { MaxPlayers = maxplayers, PlayerTtl = 10000 };
            PhotonNetwork.CreateRoom(roomname, options);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            this.SetActivePanel(SelectionPanel.name);
        }

        public void OnJoinRandomButtonClicked()
        {
            this.SetActivePanel(JoinRandomRoomPanel.name);
            PhotonNetwork.JoinRandomRoom();
        }

        public void OnRoomListButton()
        {
            if(!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
            this.SetActivePanel(RoomListPanel.name);
        }

        #endregion



        public override void OnJoinedRoom()
        {
            cachednamelist.Clear();

            SetActivePanel(InsideRoomPanel.name);

            if(playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                //This is Single player respawn, chnage it to two;
                GameObject entry = Instantiate(PlayerListEntryPrefab);
                entry.transform.SetParent(InsideRoomPanel.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
                }

                playerListEntries.Add(p.ActorNumber, entry);
            }
       }

        public void OnLeaveGameButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            this.SetActivePanel(SelectionPanel.name);
        }

        public override void OnPlayerEnteredRoom(Player newplayer)
        {
            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.SetParent(InsideRoomPanel.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<PlayerListEntry>().Initialize(newplayer.ActorNumber, newplayer.NickName);

            playerListEntries.Add(newplayer.ActorNumber, entry);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnPlayerLeftRoom(Player otherplayer)
        {
            Destroy(playerListEntries[otherplayer.ActorNumber].gameObject);
            playerListEntries.Remove(otherplayer.ActorNumber);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnMasterClientSwitched(Player Masterclient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == Masterclient.ActorNumber)
            {
                StartGameButton.gameObject.SetActive(CheckPlayersReady());
            }
        }

        public void OnBackButtonClicked()
        {
            if(PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }
            SetActivePanel(SelectionPanel.name);
        }
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            GameObject entry;
            if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
            {
                object isPlayerReady;
                if (changedProps.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
                }
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }
        public void LocalPlayerPropertiesUpdated()
        {
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }
        private bool CheckPlayersReady()
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return false;
            }
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object isplayerReady;
                if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_READY, out isplayerReady))
                {
                    if(! (bool) isplayerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
           
        }


        public void SetActivePanel(string activePanel)
        {
            LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
            SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
            CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
            JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
            RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));    
            InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
        }

    }
}
