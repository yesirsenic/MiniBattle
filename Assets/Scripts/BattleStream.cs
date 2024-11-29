using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.MiniBattle.Game
{


    public class BattleStream : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        public bool is_Ready = false;

        #endregion

        #region private Fields


        #endregion 

        #region MonoBehaviourPunCallbacks Methods

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(is_Ready);
            }

            else
            {
                this.is_Ready = (bool)stream.ReceiveNext();
            }
        }

        #endregion

        #region Monobehaviour Methods

        private void Awake()
        {
            



        }

        #endregion
    }
}
