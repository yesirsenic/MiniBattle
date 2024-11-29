using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace Com.MiniBattle.Game
{


    public class FPSController : MonoBehaviour
    {
        #region public Fields

        public Button[] fps_Buttons;

        #endregion

        #region Private Fields

        private Button press_Button;

        #endregion

        #region Prviate Methods



        #endregion

        #region Public Methods
        public void ButtonOnOff()
        {
            for (int i = 0; i < fps_Buttons.Length; i++)
            {
                if (fps_Buttons[i] != press_Button)
                {
                    fps_Buttons[i].gameObject.GetComponent<Image>().color = Color.white;
                }
            }
        }
        public void setFps(int fps)
        {
            PhotonNetwork.Disconnect();
            PhotonNetwork.SendRate = fps;
            PhotonNetwork.SerializationRate = fps;
            PhotonNetwork.ConnectUsingSettings();

        }

        public void ColorChange(GameObject button)
        {
            press_Button = button.GetComponent<Button>();

            press_Button.gameObject.GetComponent<Image>().color = Color.red;

        }

        #endregion
    }
}
