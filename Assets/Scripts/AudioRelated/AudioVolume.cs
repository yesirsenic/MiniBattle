using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Com.MiniBattle.Game
{



    public class AudioVolume : MonoBehaviour
    {

        #region Public Fields

        public AudioMixer masterMixer;
        public Slider BGMSlider;
        public Slider SFXSlider;

        #endregion


        #region MonoBehaviour CallBack
        public void AudioControl()
        {
            float bgm_Sound = BGMSlider.value;
            float sfx_Sound = SFXSlider.value;

            if (bgm_Sound == -40f) masterMixer.SetFloat("BGM", -80);
            else masterMixer.SetFloat("BGM", bgm_Sound);

            if (sfx_Sound == -40f) masterMixer.SetFloat("SFX", -80);
            else masterMixer.SetFloat("SFX", sfx_Sound);
        }

        private void Update()
        {
            AudioControl();
        }

        #endregion
    }
}
