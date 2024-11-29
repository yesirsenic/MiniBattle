using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

namespace Com.MiniBattle.Game
{


    public class CheckRoomScene : MonoBehaviour
    {

        #region Monobehaviour Methods        
        void Start()
        {
            // ���� �� �̸��� �����ͼ� ����
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "SceneName", currentScene }
             };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        #endregion

        #region Public Methods
        public bool AreAllPlayersInSameScene()
        {
            string localScene = (string)PhotonNetwork.LocalPlayer.CustomProperties["SceneName"];

            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.TryGetValue("SceneName", out object sceneName))
                {
                    if ((string)sceneName != localScene)
                    {
                        return false; // �ٸ� ���� �ִ� �÷��̾ ����
                    }
                }
                else
                {
                    return false; // �� ������ ���� �÷��̾ ����
                }
            }
            return true; // ��� �÷��̾ ���� ���� ����
        }

        #endregion
    }
}
