using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;


namespace Com.MiniBattle.Game
{
    public class MatchingManager : MonoBehaviourPunCallbacks
    {

        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            if (!PhotonNetwork.InLobby)
                PhotonNetwork.JoinLobby();

            if(GameObject.Find("Loading"))
            {
                GameObject loading = GameObject.Find("Loading");
                loading.GetComponent<Loading>().GageControll();
            }
            
            Debug.Log("마스터 서버 접속 완료");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("어떠한 원인으로 연결이 끊김 {0}", cause);

            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnJoinedLobby()
        {
            
            Debug.Log("로비 접속 완료");
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("룸 만들기 완료");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogWarningFormat("어떠한 원인으로 룸이 생성되지 못함 {0}", message);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("룸 접속 완료");

            if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                GameManager.is_1p = true;
                Debug.Log("1p");
            }

            else
            {
                GameManager.is_2p = true;
                Debug.Log("2p");
            }


            if (is_Test)
                PhotonNetwork.LoadLevel("CharacterTest");

            else
                PhotonNetwork.LoadLevel("Matching");

        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogWarningFormat("어떠한 원인으로 룸에 접속되지 않음 {0}", message);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogWarningFormat("어떠한 원인으로 룸 랜덤 접속 실패 {0}", message);

            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
        }

        #endregion


        #region Private Serializable Fields

        #endregion

        #region Private Fields

        string gameVersion = "1";

        #endregion

        #region Public Fields

        public bool is_Test; // test 공간으로 이동하기 위한 불값

        #endregion

        #region MonoBehaviour CallBacks



        private void Start()
        {
            PhotonNetwork.GameVersion = this.gameVersion;
            PhotonNetwork.SendRate = 90;
            PhotonNetwork.SerializationRate = 90;

            PhotonNetwork.ConnectUsingSettings();

        }

        #endregion

        #region Private Methods

        [ContextMenu("정보")]
        void Info()
        {
            if (PhotonNetwork.InRoom)
            {
                print("현재 방 이름 : " + PhotonNetwork.CurrentRoom.Name);
                print("현재 방 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
                print("현재 방 최대인원수 : " + PhotonNetwork.CurrentRoom.MaxPlayers);

                string playerStr = "방에 있는 플레이어 목록 : ";
                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
                print(playerStr);
            }
            else
            {
                print("접속한 인원 수 : " + PhotonNetwork.CountOfPlayers);
                print("방 개수 : " + PhotonNetwork.CountOfRooms);
                print("모든 방에 있는 인원 수 : " + PhotonNetwork.CountOfPlayersInRooms);
                print("로비에 있는지? : " + PhotonNetwork.InLobby);
                print("연결됐는지? : " + PhotonNetwork.IsConnected);
            }
        }

        #endregion

        #region Public Methods

        public void Connect() //룸 연결
        {

            if(PhotonNetwork.IsConnected)
            {

                Debug.Log("매칭 룸에 접속");
                PhotonNetwork.JoinRandomRoom();



            }

            else
            {
                Debug.Log("룸에 접속 실패하여 다시 시도 중");
                PhotonNetwork.ConnectUsingSettings();

            }
        }

        #endregion
    }
}
