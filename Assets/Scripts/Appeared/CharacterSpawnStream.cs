using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.MiniBattle.Game
{
    public class CharacterSpawnStream : MonoBehaviour
    {
        #region Public Fields

        public GameObject appearedManager;
        public GameObject map;
        public GameObject character_2p;
        public Vector3 startPos_2p;

        #endregion

        #region Private Fields

        private PhotonView PV;

        #endregion

        #region MonoBehaviour Methods

        private void Start()
        {
            PV = gameObject.GetPhotonView();
            startPos_2p = new Vector3(0, 0, 0);
        }

        private void Update()
        {
            if (appearedManager == null)
                return;

            if(map != null && character_2p ==null && GameManager.is_2p)
            {
                characterSpawn2P();
            }
        }

        #endregion

        #region Private Methods

        
        private void characterSpawn2P()
        {
            if(appearedManager.GetComponent<AppearedManager>().is_character_spawn_ready == true)
            {
                if (GameManager.is_2p)
                {
                    string gameCharacter2p_Name = GameManager.character.character_Name;

                    startPos_2p = map.transform.Find("2p_StartPos").gameObject.GetComponent<SpawnPositions>().spawnPosCharacter(gameCharacter2p_Name);
                    character_2p = PhotonNetwork.Instantiate(gameCharacter2p_Name, startPos_2p, Quaternion.identity);

                    if (character_2p.GetComponent<MeleeCharacter>())
                    {
                        character_2p.GetComponent<MeleeCharacter>().is_Right = false;
                    }

                    if (character_2p.GetComponent<RangedCharacter>())
                    {
                        character_2p.GetComponent<RangedCharacter>().is_Right = false;
                    }

                    appearedManager.GetComponent<AppearedManager>().SetCharacter2p(character_2p);

                }
            }
        }

        #endregion
    }
}
