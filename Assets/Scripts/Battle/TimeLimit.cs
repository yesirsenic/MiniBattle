using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;

namespace Com.MiniBattle.Game
{
    public class TimeLimit : MonoBehaviour , IPunObservable
    {

        #region Private Fields

        private float time;
        private float delay;
        private float startDelay;
        private float present_Time;
        private bool is_draw;
        private PhotonView PV;

        #endregion

        #region Photon Methods

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(time);
            }

            else
            {
                this.time = (float)stream.ReceiveNext();
            }
        }

        #endregion

        #region Monobehaviour Methods

        private void Start()
        {
            delay = 1f;
            startDelay = 1f;
            time = 60f;
            present_Time = 60f;
            PV = gameObject.GetPhotonView();

            if(PV.IsMine)
            {
                PV.RPC("Map_BGM", RpcTarget.All, GameManager.map.name);
                InvokeRepeating("TimeDecrease", startDelay, delay);
            }

          

        }

        private void Update()
        {
            if (GameManager.is_Game_Finish)
                return;
            
                

            TimeUpdate();

            TimeZero();
        }

        #endregion

        #region Private Methods

        private void TimeUpdate()
        {
            if(present_Time != time)
            {
                this.gameObject.GetComponent<Text>().text = time.ToString();
                present_Time = time;
                if(time <10 && time>0)
                {
                    SoundManager.Instance.SFXInsert("Countdown Warning");
                }

            }
            
        }

        private void TimeDecrease()
        {
            time -= delay;
        }

        private void TimeZero()
        {
            if(time<=0)
            {
                if(PV.IsMine)
                {
                    CancelInvoke("TimeDecrease");
                }

                GameObject[] characters = GameObject.FindGameObjectsWithTag("Character").OrderBy(obj => obj.transform.GetSiblingIndex()).ToArray();
                GameObject my_Character = null;
                GameObject other_Character = null;
                bool result = false;


                for (int i = 0; i<characters.Length;i++)
                {
                    if(characters[i].GetPhotonView().IsMine)
                    {
                        my_Character = characters[i];
                    }

                    else if(!characters[i].GetPhotonView().IsMine)
                    {
                        other_Character = characters[i];
                    }

                }

                my_Character.GetComponent<AudioSource>().Stop();
                other_Character.GetComponent<AudioSource>().Stop();

                if (my_Character.GetComponent<MeleeCharacter>())
                {
                    if(other_Character.GetComponent<MeleeCharacter>())
                    {
                        result =  HP_Comparison(my_Character.GetComponent<MeleeCharacter>().Hp, other_Character.GetComponent<MeleeCharacter>().Hp);
                        
                    }

                    if (other_Character.GetComponent<RangedCharacter>())
                    {
                        result = HP_Comparison(my_Character.GetComponent<MeleeCharacter>().Hp, other_Character.GetComponent<RangedCharacter>().Hp);
                        
                    }
                }

                else if(my_Character.GetComponent<RangedCharacter>())
                {
                    if (other_Character.GetComponent<MeleeCharacter>())
                    {
                        result = HP_Comparison(my_Character.GetComponent<RangedCharacter>().Hp, other_Character.GetComponent<MeleeCharacter>().Hp);
                       
                    }

                    if (other_Character.GetComponent<RangedCharacter>())
                    {
                        result = HP_Comparison(my_Character.GetComponent<RangedCharacter>().Hp, other_Character.GetComponent<RangedCharacter>().Hp);
                        
                    }
                }

                

                if(is_draw)
                {
                    GameManager.Instance.BattleFinishDraw();
                    return;
                }

                GameManager.Instance.BattleFinish(result);


            }
        }

        private bool HP_Comparison(float myHP, float otherHP)
        {
            if(myHP > otherHP)
            {
                return true;
            }

            else if(myHP < otherHP)
            {
                return false;
            }

            else
            {
                is_draw = true;
                return false;
            }
        }

        [PunRPC]
        private void Map_BGM(string map_Name)
        {
            switch(map_Name)
            {
                case "Forest":
                    SoundManager.Instance.BGMInsert("Forest");
                    break;

                case "Town":
                    SoundManager.Instance.BGMInsert("Town");
                    break;

                case "Land":
                    SoundManager.Instance.BGMInsert("Land");
                    break;

                case "Cave":
                    SoundManager.Instance.BGMInsert("Cave");
                    break;
            }
        }

        

        #endregion




    }
}
