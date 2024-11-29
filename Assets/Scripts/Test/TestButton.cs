using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


namespace Com.MiniBattle.Game
{
    public class TestButton : MonoBehaviour
    {

        #region Private Fields

        PhotonView PV;
        GameObject map;
        GameObject player1p;
        GameObject player2p;

        #endregion

        #region Monobehaviour Methods

        private void Start()
        {
            PV = gameObject.GetPhotonView();
        }

        private void Update()
        {
            FindMap();   
        }

        #endregion

        #region Private Methods

        private void FindMap()
        {
            if (map != null)
                return;

            map = GameObject.FindGameObjectWithTag("Map");
        }

        #endregion

        #region Public Methods

        public void MapButton(string name)
        {
            if (!PV.IsMine)
                return;


            if (map != null)
            {
                PhotonNetwork.Destroy(map);
                map = null;

                if(player1p !=null)
                {
                    PhotonNetwork.Destroy(player1p);
                    player1p = null;
                }

                if(player2p != null)
                {
                    PhotonNetwork.Destroy(player2p);
                    player2p = null;
                }
            }
                

            map = PhotonNetwork.Instantiate(name, Vector3.zero, Quaternion.identity);
        }

        public void PersonPointButton(int point)
        {
            if (point == 1)
            {
                GameManager.is_1p = true;
                Debug.Log("1p");
            }
                
            if (point == 2)
            {
                GameManager.is_2p = true;
                Debug.Log("2p");
            }
                
        }

        public void CharacterButton(string name)
        {
            if(GameManager.is_1p)
            {
                if (player1p != null)
                {
                    PhotonNetwork.Destroy(player1p);
                    player1p = null;
                }


                Vector2 startPos_1p = map.transform.Find("1p_StartPos").gameObject.GetComponent<SpawnPositions>().spawnPosCharacter(name);
                player1p = PhotonNetwork.Instantiate(name, startPos_1p, Quaternion.identity);
            }

            if(GameManager.is_2p)
            {
                if (player2p != null)
                {
                    PhotonNetwork.Destroy(player2p); 
                    player2p = null;
                }
                    

                Vector2 startPos_2p = map.transform.Find("2p_StartPos").gameObject.GetComponent<SpawnPositions>().spawnPosCharacter(name);
                player2p = PhotonNetwork.Instantiate(name, startPos_2p, Quaternion.identity);
                player2p.transform.eulerAngles = new Vector3(0, -180, 0);
            }
        }

        #endregion

    }
}
