using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;


namespace Com.MiniBattle.Game
{
    public class CharacterSelectManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region SerializeFields
        [Tooltip("Ä³¸¯ÅÍ ¼±ÅÃÃ¢ ³» Ä­ ½½·Ôµé")]
        [SerializeField]
        private GameObject[] character_Slot;
        [SerializeField]
        private int selectNumber_1p;
        [SerializeField]
        private int selectNumber_2p;

        #endregion

        #region Private Fields

        PhotonView script_PV;
        private bool is_SelectNumber = false;
        private Color red;
        private Color blue;
        private Color gray;
        private Vector3 color_Red = new Vector3(1, 0, 0);
        private Vector3 color_Gray = new Vector3(0.556f, 0.556f, 0.556f);
        private Vector3 color_Blue = new Vector3(0, 0, 1);
        private GameObject loading;
        private Character character;
        CharacterSelectManager other_CharacterSelectManager;
        
        #endregion

        #region Public Fields

        public bool is_Select;
        public bool is_Moved;
        public bool is_1p;
        public bool is_2p;
        public CharacterSelectAnim characterAnim_1p;
        public CharacterSelectAnim characterAnim_2p;

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(selectNumber_1p);
                stream.SendNext(selectNumber_2p);
                stream.SendNext(is_Select);
                stream.SendNext(is_Moved);
            }

            else
            {
                // Network player, receive data
                this.selectNumber_1p = (int)stream.ReceiveNext();
                this.selectNumber_2p = (int)stream.ReceiveNext();
                this.is_Select = (bool)stream.ReceiveNext();
                this.is_Moved = (bool)stream.ReceiveNext();
            }
        }

        #endregion

        #region MonoBehavoir Callbacks

        private void Start()
        {
            GameManager.is_Loading = true;
            character_Slot = GameObject.FindGameObjectsWithTag("CharacterSlot").OrderBy(obj => obj.transform.GetSiblingIndex()).ToArray();
            loading = GameObject.Find("Loading");
            characterAnim_1p = GameObject.Find("CharacterSelectAnim_1p").GetComponent<CharacterSelectAnim>();
            characterAnim_2p = GameObject.Find("CharacterSelectAnim_2p").GetComponent<CharacterSelectAnim>();
            selectNumber_1p = 0;
            selectNumber_2p = 0;
            is_Select = false;
            is_Moved = false;
            script_PV = photonView;
            if (script_PV) script_PV.ObservedComponents.Add(this); // µð¹ö±ë
            red = new Color(color_Red.x, color_Red.y, color_Red.z);
            blue = new Color(color_Blue.x, color_Blue.y, color_Blue.z);
            gray = new Color(color_Gray.x, color_Gray.y, color_Gray.z);
            if (GameManager.is_1p)
            {
                SetColor(selectNumber_1p, color_Red);

                if(script_PV.IsMine)
                {
                    is_1p = true;
                }

                else
                {
                    is_2p = true;
                }
            }

            if(GameManager.is_2p)
            {
                SetColor(selectNumber_2p, color_Blue);

                if (script_PV.IsMine)
                {
                    is_2p = true;
                }

                else
                {
                    is_1p = true;
                }
            }


        }

        private void Update()
        {
            if(other_CharacterSelectManager == null)
            {
                FindOtherManger();   
            }

            if (script_PV.IsMine)
            {
                if (other_CharacterSelectManager != null)
                {
                    selectNumberSync();
                }
            }


            if (is_SelectNumber == true && loading != null && loading.activeSelf == true && GameManager.is_Loading == true && GameManager.is_SameScene == true)
            {
                GameManager.Instance.Loading_Close();
                if(SoundManager.Instance.gameObject.GetComponent<AudioSource>().clip == null)
                {
                    SoundManager.Instance.BGMInsert("Character_Select");
                    SoundManager.Instance.gameObject.GetComponent<AudioSource>().volume = 1;
                }
            }


            if (GameManager.is_Loading == true)
                return;

            if(script_PV.IsMine)
            {
                if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
                    script_PV.RPC("Character_Slot_Move", RpcTarget.AllBuffered);

                if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))
                {
                    script_PV.RPC("Character_Select", RpcTarget.AllBuffered);
                }
            }

            if(GameManager.is_CharacterSelectComplete == false)
            {
                if(is_Select == true && other_CharacterSelectManager.is_Select == true && !is_Moved && !other_CharacterSelectManager.is_Moved)
                {
                    GameManager.character = character;
                    GameManager.is_CharacterSelectComplete = true;
                }
            }

            SetColorText();



        }

        #endregion

        #region Private Methods

        void FindOtherManger()
        {
            GameObject[] managers;
            GameObject other_Manger = null;

            managers = GameObject.FindGameObjectsWithTag("CharacterSelectManager");

            for(int i=0; i<managers.Length;i++)
            {
                if(managers[i] != this.gameObject)
                {
                    Debug.Log("Find other Manager");
                    other_Manger = managers[i];
                }
            }


            if(other_Manger != null)
                other_CharacterSelectManager = other_Manger.GetComponent<CharacterSelectManager>();
        }

        void selectNumberSync()
        {
            if(GameManager.is_1p)
            {
                if(selectNumber_2p != other_CharacterSelectManager.selectNumber_2p)
                {
                    selectNumber_2p = other_CharacterSelectManager.selectNumber_2p;
                }
            }

            if(GameManager.is_2p)
            {
                if(selectNumber_1p != other_CharacterSelectManager.selectNumber_1p)
                {
                    selectNumber_1p = other_CharacterSelectManager.selectNumber_1p;
                }
            }

            is_SelectNumber = true;
        }

        void SetColorText()
        {
            for(int i=0; i<character_Slot.Length;i++)
            {
                if(character_Slot[i].GetComponent<Image>().color == red)
                {
                    character_Slot[i].transform.Find("1p_Character_Select_Text").gameObject.SetActive(true);
                    character_Slot[i].transform.Find("2p_Character_Select_Text").gameObject.SetActive(false);
                }

                if(character_Slot[i].GetComponent<Image>().color == blue)
                {
                    character_Slot[i].transform.Find("2p_Character_Select_Text").gameObject.SetActive(true);
                    character_Slot[i].transform.Find("1p_Character_Select_Text").gameObject.SetActive(false);
                }

                if(character_Slot[i].GetComponent<Image>().color == gray)
                {
                    character_Slot[i].transform.Find("1p_Character_Select_Text").gameObject.SetActive(false);
                    character_Slot[i].transform.Find("2p_Character_Select_Text").gameObject.SetActive(false);
                }
            }
        }

        #endregion

        #region Private Methods(PunRPC)

        [PunRPC]
        private void Character_Slot_Move()
        {
            if (is_Select == true)
                return;

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if(GameManager.is_1p)
                {
                    if(selectNumber_2p == selectNumber_1p)
                    {
                        script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_1p, color_Blue);
                        
                    }

                    else
                    {
                        script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_1p, color_Gray);
                    }

                    

                    if (selectNumber_1p ==3 || selectNumber_1p == 7)
                    {
                        selectNumber_1p -= 3;
                    }

                    else
                    {
                        selectNumber_1p += 1;
                    }

                    SoundManager.Instance.SFXInsert("Character Block Move");
                    script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_1p, color_Red);
                }

                if(GameManager.is_2p)
                {
                    if (selectNumber_1p == selectNumber_2p)
                    {
                        script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_2p, color_Red);
                    }

                    else
                    {
                        script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_2p, color_Gray);
                    }

                    if (selectNumber_2p == 3 || selectNumber_2p == 7)
                    {
                        selectNumber_2p -= 3;
                    }

                    else
                    {
                        selectNumber_2p += 1;
                    }

                    SoundManager.Instance.SFXInsert("Character Block Move");
                    script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_2p, color_Blue);
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (GameManager.is_1p)
                {

                    if (selectNumber_2p == selectNumber_1p)
                    {
                        script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_1p, color_Blue);
                    }

                    else
                    {
                        script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_1p, color_Gray);
                    }

                    if (selectNumber_1p == 0 || selectNumber_1p == 4)
                    {
                        selectNumber_1p += 3;
                    }

                    else
                    {
                        selectNumber_1p -= 1;
                    }

                    SoundManager.Instance.SFXInsert("Character Block Move");
                    script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_1p, color_Red);

                }

                if (GameManager.is_2p)
                {

                    if (selectNumber_1p == selectNumber_2p)
                    {
                        script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_2p, color_Red);
                    }

                    else
                    {
                        script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_2p, color_Gray);
                    }

                    if (selectNumber_2p == 0 || selectNumber_2p == 4)
                    {
                        selectNumber_2p += 3;
                    }

                    else
                    {
                        selectNumber_2p -= 1;
                    }

                    SoundManager.Instance.SFXInsert("Character Block Move");
                    script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_2p, color_Blue);
                }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (GameManager.is_1p)
                {

                    if (selectNumber_2p == selectNumber_1p)
                    {
                        script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_1p, color_Blue);
                    }

                    else
                    {
                        script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_1p, color_Gray);
                    }

                    if (selectNumber_1p >= 0 && selectNumber_1p<=3)
                    {
                        selectNumber_1p += 4;
                    }

                    else
                    {
                        selectNumber_1p -= 4;
                    }

                    SoundManager.Instance.SFXInsert("Character Block Move");
                    script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_1p, color_Red);

                }

                if (GameManager.is_2p)
                {

                    if (selectNumber_1p == selectNumber_2p)
                    {
                        script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_2p, color_Red);
                    }

                    else
                    {
                        script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_2p, color_Gray);
                    }

                    if (selectNumber_2p >= 0 && selectNumber_2p <=3)
                    {
                        selectNumber_2p += 4;
                    }

                    else
                    {
                        selectNumber_2p -= 4;
                    }

                    SoundManager.Instance.SFXInsert("Character Block Move");
                    script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_2p, color_Blue);
                }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (GameManager.is_1p)
                {

                    if (selectNumber_2p == selectNumber_1p)
                    {
                        script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_1p, color_Blue);
                    }

                    else
                    {
                        script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_1p, color_Gray);
                    }

                    if (selectNumber_1p >= 0 && selectNumber_1p <= 3)
                    {
                        selectNumber_1p += 4;
                    }

                    else
                    {
                        selectNumber_1p -= 4;
                    }

                    SoundManager.Instance.SFXInsert("Character Block Move");
                    script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_1p, color_Red);

                }

                if (GameManager.is_2p)
                {

                    if (selectNumber_1p == selectNumber_2p)
                    {
                        script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_2p, color_Red);
                    }

                    else
                    {
                        script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_2p, color_Gray);
                    }

                    if (selectNumber_2p >= 0 && selectNumber_2p <= 3)
                    {
                        selectNumber_2p += 4;
                    }

                    else
                    {
                        selectNumber_2p -= 4;
                    }

                    SoundManager.Instance.SFXInsert("Character Block Move");
                    script_PV.RPC("SetColor", RpcTarget.AllBuffered, selectNumber_2p, color_Blue);
                }
            }
        }

        [PunRPC]
        private void Character_Select()
        {
            if(Input.GetKeyDown(KeyCode.Z) && is_Select == false && !is_Moved)
            {
                if(GameManager.is_1p)
                {
                    script_PV.RPC("CharacetrImageChange_1p", RpcTarget.AllBuffered, selectNumber_1p,true);
                    is_Moved = true;
                    is_Select = true;
                }

                if(GameManager.is_2p)
                {
                    script_PV.RPC("CharacetrImageChange_2p", RpcTarget.AllBuffered, selectNumber_2p,true);
                    is_Moved = true;
                    is_Select = true;
                }
            }

            if(Input.GetKeyDown(KeyCode.X) && is_Select == true && !is_Moved && !GameManager.is_CharacterSelectComplete)
            {
                if (GameManager.is_1p)
                {
                    script_PV.RPC("CharacetrImageChange_1p", RpcTarget.AllBuffered, selectNumber_1p,false);
                    is_Moved = true;
                    is_Select = false;
                }

                if (GameManager.is_2p)
                {
                    script_PV.RPC("CharacetrImageChange_2p", RpcTarget.AllBuffered, selectNumber_2p,false);
                    is_Moved = true;
                    is_Select = false;
                }
            }
        }

        [PunRPC]
        void CharacetrImageChange_1p(int selectNumber, bool insertimage)
        {
            if(insertimage == false)
            {
                if (GameManager.is_1p)
                {
                    characterAnim_1p.GetComponent<Animator>().SetBool("Leave", true);
                    character = null;
                }
                    


                if (GameManager.is_2p)
                    characterAnim_1p.GetComponent<Animator>().SetBool("Leave", true);


                return;
            }


            if(GameManager.is_1p)
            {
                characterAnim_1p.GetComponent<Animator>().SetBool(character_Slot[selectNumber_1p].GetComponent<CharacterSlot>().character.character_Name, true);
                character = character_Slot[selectNumber_1p].GetComponent<CharacterSlot>().character;
            }

            if(GameManager.is_2p)
            {
                characterAnim_1p.GetComponent<Animator>().SetBool(character_Slot[selectNumber_1p].GetComponent<CharacterSlot>().character.character_Name, true);
            }
            
        }

        [PunRPC]
        void CharacetrImageChange_2p(int selectNumber, bool insertimage)
        {
            if (insertimage == false)
            {
                if (GameManager.is_1p)
                    characterAnim_2p.GetComponent<Animator>().SetBool("Leave", true);

                if (GameManager.is_2p)
                {
                    characterAnim_2p.GetComponent<Animator>().SetBool("Leave", true);
                    character = null;
                }
                    


                return;
            }


            if (GameManager.is_1p)
            {
                characterAnim_2p.GetComponent<Animator>().SetBool(character_Slot[selectNumber_2p].GetComponent<CharacterSlot>().character.character_Name, true);
            }

            if (GameManager.is_2p)
            {
                characterAnim_2p.GetComponent<Animator>().SetBool(character_Slot[selectNumber_2p].GetComponent<CharacterSlot>().character.character_Name, true);
                character = character_Slot[selectNumber_2p].GetComponent<CharacterSlot>().character;
            }

        }

        [PunRPC]
        private void SetColor(int selectcount , Vector3 color)
        {
            Color c = new Color(color.x, color.y, color.z);



            if(GameManager.is_1p)
            {
                character_Slot[selectcount].GetComponent<Image>().color = c;
            }

            if(GameManager.is_2p)
            {
                character_Slot[selectcount].GetComponent<Image>().color = c;
            }
        }


        



        #endregion
    }
}
