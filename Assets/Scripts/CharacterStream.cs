using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.MiniBattle.Game
{


    public class CharacterStream : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        public string other_Character_Name;

        #endregion

        #region private Fields

        private Character other_Character;

        #endregion 

        #region MonoBehaviourPunCallbacks Methods

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(other_Character_Name);
            }

            else
            {
                this.other_Character_Name = (string)stream.ReceiveNext();
            }
        }

        #endregion

        #region Monobehaviour Methods

        private void Awake()
        {
            if(photonView.IsMine)
            {
                other_Character = GameManager.character;
                other_Character_Name = other_Character.character_Name;
            }
             
            
             
        }

        #endregion
    }
}
