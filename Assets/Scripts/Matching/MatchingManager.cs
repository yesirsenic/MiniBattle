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
            
            Debug.Log("������ ���� ���� �Ϸ�");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("��� �������� ������ ���� {0}", cause);

            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnJoinedLobby()
        {
            
            Debug.Log("�κ� ���� �Ϸ�");
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("�� ����� �Ϸ�");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogWarningFormat("��� �������� ���� �������� ���� {0}", message);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("�� ���� �Ϸ�");

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
            Debug.LogWarningFormat("��� �������� �뿡 ���ӵ��� ���� {0}", message);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogWarningFormat("��� �������� �� ���� ���� ���� {0}", message);

            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
        }

        #endregion


        #region Private Serializable Fields

        #endregion

        #region Private Fields

        string gameVersion = "1";

        #endregion

        #region Public Fields

        public bool is_Test; // test �������� �̵��ϱ� ���� �Ұ�

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

        [ContextMenu("����")]
        void Info()
        {
            if (PhotonNetwork.InRoom)
            {
                print("���� �� �̸� : " + PhotonNetwork.CurrentRoom.Name);
                print("���� �� �ο��� : " + PhotonNetwork.CurrentRoom.PlayerCount);
                print("���� �� �ִ��ο��� : " + PhotonNetwork.CurrentRoom.MaxPlayers);

                string playerStr = "�濡 �ִ� �÷��̾� ��� : ";
                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
                print(playerStr);
            }
            else
            {
                print("������ �ο� �� : " + PhotonNetwork.CountOfPlayers);
                print("�� ���� : " + PhotonNetwork.CountOfRooms);
                print("��� �濡 �ִ� �ο� �� : " + PhotonNetwork.CountOfPlayersInRooms);
                print("�κ� �ִ���? : " + PhotonNetwork.InLobby);
                print("����ƴ���? : " + PhotonNetwork.IsConnected);
            }
        }

        #endregion

        #region Public Methods

        public void Connect() //�� ����
        {

            if(PhotonNetwork.IsConnected)
            {

                Debug.Log("��Ī �뿡 ����");
                PhotonNetwork.JoinRandomRoom();



            }

            else
            {
                Debug.Log("�뿡 ���� �����Ͽ� �ٽ� �õ� ��");
                PhotonNetwork.ConnectUsingSettings();

            }
        }

        #endregion
    }
}
