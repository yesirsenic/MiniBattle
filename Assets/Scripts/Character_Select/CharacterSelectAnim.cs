using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Com.MiniBattle.Game
{
    public class CharacterSelectAnim : MonoBehaviour
    {
        #region Private Fields

        bool is_Move;
        bool is_Appear;
        float speed;
        float arrivePoint;
        float leavePoint;
        Rigidbody2D rb;
        Animator anim;
        CharacterSelectManager manager;
        AudioSource audio;

        #endregion

        #region Public Fields

        public bool is_1p;
        public bool is_2p;

        #endregion

        #region Monobehaviour Methods

        private void Start()
        {
            is_Move = false;
            is_Appear = false;
            speed = 700f;
            arrivePoint = 450f;
            leavePoint = 1210f;
            rb = gameObject.GetComponent<Rigidbody2D>();
            anim = gameObject.GetComponent<Animator>();
            audio = gameObject.GetComponent<AudioSource>();
        }

        private void Update()
        {
            SetManager();
            CharacterSelectMoveStop();
            CharacterLeaveMoveStop();
            
        }

        #endregion

        #region Private Methods

        private void CharacterSelectMoveStop()
        {
            if(is_Move && is_1p && is_Appear && gameObject.GetComponent<RectTransform>().anchoredPosition.x >= - arrivePoint)
            {
                rb.velocity = Vector2.zero;
                gameObject.GetComponent<RectTransform>().anchoredPosition =new Vector2(-arrivePoint, gameObject.GetComponent<RectTransform>().anchoredPosition.y);
                is_Move = false;
                manager.is_Moved = false;
                FootStepsAudioStop();
                anim.SetBool("Idle", true);
            }

            if (is_Move && is_2p && is_Appear && gameObject.GetComponent<RectTransform>().anchoredPosition.x <= arrivePoint)
            {
                rb.velocity = Vector2.zero;
                gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(arrivePoint, gameObject.GetComponent<RectTransform>().anchoredPosition.y);
                is_Move = false;
                manager.is_Moved = false;
                FootStepsAudioStop();
                anim.SetBool("Idle", true);
            }
        }

        private void CharacterLeaveMoveStop()
        {
            if (is_Move && is_1p && !is_Appear && gameObject.GetComponent<RectTransform>().anchoredPosition.x <= -leavePoint)
            {
                rb.velocity = Vector2.zero;
                gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-leavePoint, gameObject.GetComponent<RectTransform>().anchoredPosition.y);
                is_Move = false;
                manager.is_Moved = false;
                FootStepsAudioStop();
                anim.SetBool("MasterIdle", true);
                __Init__();
            }

            if (is_Move && is_2p && !is_Appear && gameObject.GetComponent<RectTransform>().anchoredPosition.x >= leavePoint)
            {
                rb.velocity = Vector2.zero;
                gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(leavePoint, gameObject.GetComponent<RectTransform>().anchoredPosition.y);
                is_Move = false;
                manager.is_Moved = false;
                FootStepsAudioStop();
                anim.SetBool("MasterIdle", true);
                __Init__();
            }
        }

        private void SetDirection()
        {

            if(is_1p && !anim.GetBool("Leave"))
            {
                gameObject.transform.eulerAngles = Vector3.zero;
            }

            else if (is_1p && anim.GetBool("Leave"))
            {
                gameObject.transform.eulerAngles = new Vector3(0,180,0);
            }

            if (is_2p && !anim.GetBool("Leave"))
            {
                gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
            }

            else if (is_2p && anim.GetBool("Leave"))
            {
                gameObject.transform.eulerAngles = Vector3.zero;
            }

        }

        private void SetManager()
        {
            if (manager != null)
                return;

            GameObject[] managers = GameObject.FindGameObjectsWithTag("CharacterSelectManager");

            for(int i = 0; i< managers.Length; i++)
            {
                if(is_1p)
                {
                    if(managers[i].GetComponent<CharacterSelectManager>().is_1p)
                    {
                        manager = managers[i].GetComponent<CharacterSelectManager>();
                    }
                }

                if (is_2p)
                {
                    if (managers[i].GetComponent<CharacterSelectManager>().is_2p)
                    {
                        manager = managers[i].GetComponent<CharacterSelectManager>();
                    }
                }
            }
        }

        private void FootStepsAudioStart()
        {
            AudioClip clip = (AudioClip)Resources.Load("Audio/SFX/FootSteps");
            audio.clip = clip;
            audio.Play();
        }

        private void FootStepsAudioStop()
        {
            audio.Stop();
        }

        #endregion



        #region Public Methods

        public void CharacterSelectMove()
        {
            if(!is_Move && is_1p)
            {
                rb.velocity = new Vector2(speed, 0);
                is_Move = true;
                is_Appear = true;
                anim.SetBool("MasterIdle", false);
                FootStepsAudioStart();
                SetDirection();
            }

            if (!is_Move && is_2p)
            {
                rb.velocity = new Vector2(-speed, 0);
                is_Move = true;
                is_Appear = true;
                anim.SetBool("MasterIdle", false);
                FootStepsAudioStart();
                SetDirection();
            }

        }

        public void CharacterLeaveMove()
        {
            if (!is_Move && is_1p)
            {
                rb.velocity = new Vector2(-speed, 0);
                is_Move = true;
                is_Appear = false;
                FootStepsAudioStart();
                SetDirection();
            }

            if (!is_Move && is_2p)
            {
                rb.velocity = new Vector2(speed, 0);
                is_Move = true;
                is_Appear = false;
                FootStepsAudioStart();
                SetDirection();
            }
        }

        public void __Init__()
        {
            anim.SetBool("Idle", false);
            anim.SetBool("Leave", false);
            anim.SetBool("Evil_Wizard", false);
            anim.SetBool("Hero_Knight", false);
            anim.SetBool("King", false);
            anim.SetBool("Martial", false);
            anim.SetBool("Solider", false);
            anim.SetBool("TownsMan", false);
            anim.SetBool("Warrior", false);
            anim.SetBool("Witch", false);
        }

        #endregion
    }
}
