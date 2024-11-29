using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


namespace Com.MiniBattle.Game
{


    public class SoundManager : MonoBehaviour
    {
        #region Private Fields

        private static SoundManager instance = null;
        private string sceneName;
        private GameObject SFX_Comp;
        private float SFX_ObjectNum;

        #endregion

        #region Public Fields

        public AudioMixer mixer;
        public AudioMixerGroup SFX_Mixer;

        #endregion


        #region Public Fields

        public static SoundManager Instance
        {
            get
            {
                if (null == instance)
                {
                    return null;
                }
                return instance;
            }
        }

        #endregion

        #region MonoBehaviour Methods

        private void Awake()
        {
            if (null == instance)
            {

                instance = this;

                DontDestroyOnLoad(this.gameObject);
            }

            else
            {
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            SFX_ObjectNum = 10;
            mixer.SetFloat("BGM", PlayerPrefs.GetFloat("BGM"));
            mixer.SetFloat("SFX", PlayerPrefs.GetFloat("SFX"));
        }

        private void Update()
        {
            SoundSceneMove();
        }

        #endregion

        #region Private Fields

        private void SoundSceneMove()
        {
            if(sceneName == null || sceneName != SceneManager.GetActiveScene().name)
            {
                gameObject.GetComponent<AudioSource>().clip = null;
                SFXObjectsInstantitate();
                sceneName = SceneManager.GetActiveScene().name;
            }
        }

        private void SFXObjectsInstantitate()
        {
            SFX_Comp = new GameObject("SFX Comp");
            SFX_Comp.AddComponent<SFX_List>();

            for(int i = 0; i<SFX_ObjectNum; i++)
            {
                GameObject Object = new GameObject("SFX_Object" + (i+1));
                Object.AddComponent<AudioSource>();
                Object.AddComponent<SFX_Object>();
                Object.GetComponent<AudioSource>().playOnAwake = false;
                Object.GetComponent<AudioSource>().outputAudioMixerGroup = SFX_Mixer;
                SFX_Comp.GetComponent<SFX_List>().SFX_list.Add(Object.GetComponent<AudioSource>());
                Object.transform.parent = SFX_Comp.transform;
            }
        }

        #endregion

        #region Public Fields

        public void BGMInsert(string name)
        {
            AudioClip clip = (AudioClip)Resources.Load("Audio/BGM/" + name);
            gameObject.GetComponent<AudioSource>().clip = clip;
            gameObject.GetComponent<AudioSource>().Play();
        }

        public void SFXInsert(string name)
        {
            if (SFX_Comp == null)
                return;
            
            for(int i = 0; i<SFX_Comp.GetComponent<SFX_List>().SFX_list.Count;i++)
            {
                if(SFX_Comp.GetComponent<SFX_List>().SFX_list[i].GetComponent<AudioSource>().clip == null)
                {
                    AudioClip clip = (AudioClip)Resources.Load("Audio/SFX/" + name);
                    SFX_Comp.GetComponent<SFX_List>().SFX_list[i].GetComponent<AudioSource>().clip = clip;
                    SFX_Comp.GetComponent<SFX_List>().SFX_list[i].GetComponent<AudioSource>().Play();
                    return;
                }
            }
        }

        public AudioSource TextSFX()
        {
            GameObject Object = new GameObject("TextObject");
            Object.AddComponent<AudioSource>();
            Object.GetComponent<AudioSource>().playOnAwake = false;
            Object.GetComponent<AudioSource>().outputAudioMixerGroup = SFX_Mixer;

            return Object.GetComponent<AudioSource>();
        }

        public void AudioVolumeInit()
        {
            gameObject.GetComponent<AudioSource>().volume = 1;
        }

        #endregion
    }
}
