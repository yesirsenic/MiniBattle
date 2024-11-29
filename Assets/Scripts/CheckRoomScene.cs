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
            // 현재 씬 이름을 가져와서 저장
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
                        return false; // 다른 씬에 있는 플레이어가 있음
                    }
                }
                else
                {
                    return false; // 씬 정보가 없는 플레이어가 있음
                }
            }
            return true; // 모든 플레이어가 같은 씬에 있음
        }

        #endregion
    }
}
