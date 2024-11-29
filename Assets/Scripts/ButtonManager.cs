using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;


namespace Com.MiniBattle.Game
{


    public class ButtonManager : MonoBehaviour
    {
        #region MonoBehaviou CallBacks

        public void StartButton()
        {
            SceneManager.LoadScene("Matching");
        }

        public void OptionPopupOn(GameObject optionPopup)
        {
            optionPopup.GetComponent<AudioVolume>().BGMSlider.value = PlayerPrefs.GetFloat("BGM");
            optionPopup.GetComponent<AudioVolume>().SFXSlider.value = PlayerPrefs.GetFloat("SFX");
        }

        public void OptionPopupOff(GameObject optionPopup)
        {
            PlayerPrefs.SetFloat("BGM", optionPopup.GetComponent<AudioVolume>().BGMSlider.value);
            PlayerPrefs.SetFloat("SFX", optionPopup.GetComponent<AudioVolume>().SFXSlider.value);
        }

        public void ExitButton()
        {
            Application.Quit();
        }

        public void MainmenuButton()
        {
            SceneManager.LoadScene("Mainmenu");
        }

        public void MatchingMainmenuButton()
        {
            GameManager.Instance.__Init__();
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Mainmenu");
        }

        public void GameFinishMainmenuButton()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Mainmenu");
        }

        public void PausePopupOff()
        {
            GameManager.is_PausePopupExit = true;           
        }

        public void ButtonSFX()
        {
            SoundManager.Instance.SFXInsert("Button Click");
        }

        #endregion
    }
}
