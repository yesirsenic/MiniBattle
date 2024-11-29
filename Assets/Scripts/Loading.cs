using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;


namespace Com.MiniBattle.Game
{
    public class Loading : MonoBehaviour
    {
        #region Private Fields

        private Text loading_Text;
        private Scrollbar gage_Bar;

        private float gageIncreaseStartDelay;
        private float gageIncreaseRepeatDelay;
        private float gageMaxSize;
        private float gageControllMaxSize;
        private float gageIncreaseValue;
        private float text_Repeat_Delay;
        private float loading_Close_Delay;

        #endregion

        #region Monobehaviour Methods

        private void Start()
        {
            gageIncreaseStartDelay = 0.1f;
            gageIncreaseRepeatDelay = 0.2f;
            gageMaxSize = 1f;
            gageControllMaxSize = 0.75f;
            gageIncreaseValue = 0.1f;
            text_Repeat_Delay = 0.5f;
            loading_Close_Delay = 1f;
            loading_Text = gameObject.transform.Find("Loading_Text").GetComponent<Text>();
            gage_Bar = gameObject.transform.Find("Loading_Gage").GetComponent<Scrollbar>();

            //if(SceneManager.GetActiveScene().name == "CharacterSelect" || SceneManager.GetActiveScene().name == "Battle")
            //{
            //    GameManager.Instance.SetPlayerScene(PhotonNetwork.CurrentRoom.Name);
            //}

            InvokeRepeating("GageIncrease", gageIncreaseStartDelay, gageIncreaseRepeatDelay);
            StartCoroutine(Loading_Text_Play());
        }

        #endregion

        #region Private Methods

        private void GageIncrease()
        {
            if(gage_Bar.size <=gageControllMaxSize)
                gage_Bar.size += gageIncreaseValue;

        }

        #endregion

        #region Public Methods

        public void GageControll()
        {
            CancelInvoke("GageIncrease");
            gage_Bar.size = gageMaxSize;
            StartCoroutine(Loading_Close());
        }


        #endregion

        #region Corutine Methods

        IEnumerator Loading_Text_Play()
        {
            int count = 0;

            while(true)
            {
                if(count == 3)
                {
                    loading_Text.text = "Loading";

                    count = 0;
                }

                else
                {
                    loading_Text.text = loading_Text.text + '.';

                    count += 1;
                }

                

                yield return new WaitForSeconds(text_Repeat_Delay);
            }
        }

        IEnumerator Loading_Close()
        {
            yield return new WaitForSeconds(loading_Close_Delay);

            StopCoroutine(Loading_Text_Play());

            if (SceneManager.GetActiveScene().name == "Mainmenu")
            {
                GameObject backGround = GameObject.Find("BackGround");
                backGround.GetComponent<Animator>().SetBool("Start", true);
            }

            gameObject.SetActive(false);
        }

        #endregion




    }
}
