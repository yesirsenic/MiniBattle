using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MiniBattle.Game
{


    public class SFX_Object : MonoBehaviour
    {

        #region MonoBehaviour Methods

        private void Update()
        {
            SoundDelete();
        }

        #endregion

        #region Private Methods

        private void SoundDelete()
        {
            if (!gameObject.GetComponent<AudioSource>())
                return;
            if (gameObject.GetComponent<AudioSource>().clip == null)
                return;

            if(gameObject.GetComponent<AudioSource>().isPlaying == false)
            {
                gameObject.GetComponent<AudioSource>().clip = null;
            }


        }

        #endregion
    }
}
