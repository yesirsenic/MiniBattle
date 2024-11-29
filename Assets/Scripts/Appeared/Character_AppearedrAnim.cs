using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.MiniBattle.Game
{


    public class Character_AppearedrAnim : MonoBehaviour
    {

        #region Private Fields

        private int fuc_Count=0;
        private float delay = 0.125f;

        #endregion

        #region Public Fields

        public string text;
        public Text targetText;

        #endregion

        #region MonoBehaviour Callbacks

        #endregion

        #region Public Methods
        public void Check()
        {
            Debug.Log(text.ToString());
            Debug.Log(targetText.ToString());
            fuc_Count++;
            Debug.Log(fuc_Count);

        }

        public void PrintText()
        {
            StartCoroutine(textPrint(delay));
        }

        public void BlackPanelSFX()
        {
            SoundManager.Instance.SFXInsert("Appear_Black_Panel");
        }
        #endregion

        #region Coroutines Methods

        IEnumerator textPrint(float d)
        {
            Debug.Log(text.ToString());
            int count = 0;

            AudioSource audio = SoundManager.Instance.TextSFX();
            AudioClip clip = (AudioClip)Resources.Load("Audio/SFX/Character_Appeared_Text"); //임시 효과음 마음에 안들으면 다른거로 변경
            audio.clip = clip;

            while (count != text.Length)
            {
                if (count < text.Length)
                {
                    targetText.text += text[count].ToString();
                    audio.Play();
                    count++;
                }

                yield return new WaitForSeconds(d);
            }

            audio.Stop();
        }

        #endregion





    }
}
