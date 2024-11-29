using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;

namespace Com.MiniBattle.Game
{


    public class AppearedManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region SerialzeField Fields

        #endregion

        #region Private Fields


        private float zoomInCameraSize = 8f;
        private float zoomOutCamearaSize = 14f;
        private float appearaninTime = 5f;
        private string other_CharacterName;
        private bool is_SettingStart = false;
        private bool is_Battle_Start = false;
        private bool is_Countdown_Set = false;
        private bool is_countdownAllSet = false;
        private GameObject map;
        private GameObject countdown;
        private GameObject characterSpawn;
        private GameObject character_1p;
        private GameObject character_2p;
        private GameObject[] battleStream;
        private GameObject loading;
        private Camera camera;

        private AppeardState state;

        private Vector3 startPos_1p;
        private Vector3 startPos_2p;

        private PhotonView PV;
        #endregion

        #region Public Fields
        public enum AppeardState
        {
            none, ready, spawn, appeared, countDown, start, battle
        }
        public CheckRoomScene checkRoomscene;
        public Text TimeLimit_Text;
        public GameObject hpUI_1p;
        public GameObject hpUI_2p;
        public GameObject appear_Anim;
        public GameObject explain;
        public bool is_character_spawn_ready;
        public bool is_countdown_ready;



        #endregion

        #region   MonoBehaviourPun Callbacks

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(startPos_1p);
                stream.SendNext(startPos_2p);
                stream.SendNext(is_character_spawn_ready);
                stream.SendNext(is_countdown_ready);

            }

            else
            {
                this.startPos_1p = (Vector3)stream.ReceiveNext();
                this.startPos_2p = (Vector3)stream.ReceiveNext();
                this.is_character_spawn_ready = (bool)stream.ReceiveNext();
                this.is_countdown_ready = (bool)stream.ReceiveNext();


            }
        }

        #endregion

        #region MonoBehaviour Methods

        private void Start()
        {
            GameManager.is_Loading = true;
            PV = gameObject.GetPhotonView();
            loading = GameObject.Find("Loading");

            is_character_spawn_ready = false;
            is_countdown_ready = false;
            camera = GameObject.Find("Main Camera").GetComponent<Camera>();

            hpUI_1p.SetActive(false);
            hpUI_2p.SetActive(false);
            TimeLimit_Text.gameObject.SetActive(false);
            explain.SetActive(false);

            

        }

        private void Update()
        {

            if(loading != null && loading.activeSelf == true && GameManager.is_SameScene)
            {
                GameManager.Instance.Loading_Close();
            }

            if (GameManager.is_Loading == true)
                return;

            if(!is_SettingStart)
            {
                AppearSetting();
                is_SettingStart = true;
            }


            if (PV.IsMine)
            {
                if (state == AppeardState.none && other_CharacterName != null && other_CharacterName != "")
                {
                    is_character_spawn_ready = true;
                    state = AppeardState.ready;
                }

                if (state == AppeardState.ready && checkRoomscene.AreAllPlayersInSameScene())
                {
                    PV.RPC("CharacterInstantiate", RpcTarget.MasterClient);
                    state = AppeardState.spawn;
                }

                if (state == AppeardState.countDown)
                {
                    PV.RPC("CountdownStart", RpcTarget.MasterClient);

                }


            }

            if (other_CharacterName == null || other_CharacterName == "")
            {
                PV.RPC("OtherCharacterFind", RpcTarget.All);
            }

            if (!PV.IsMine)
            {
                Set_2pComponent();
                CountdownSet2P();
            }

            if (state == AppeardState.spawn)
            {
                PV.RPC("CharacterAppearedStart", RpcTarget.All);
                state = AppeardState.appeared;
            }

            if (state == AppeardState.appeared || is_countdown_ready == true)
            {
                FindBattleStream();

                Debug.Log(is_countdownAllSet);
                if (is_countdownAllSet)
                {
                    is_countdown_ready = false;
                    state = AppeardState.countDown;
                }

            }

            if (is_Battle_Start)
            {
                Set_OtherCharacter();
                SetCameraSettingToBattle();
                if (GameManager.is_Game_Finish)
                    GameManager.is_Game_Finish = false;
                state = AppeardState.battle;
            }

        }

        #endregion

        #region Private Methods(PunRPC)

        [PunRPC]
        void MapSet()
        {
            map = PhotonNetwork.Instantiate(GameManager.map.map_Name, new Vector3(0, 0, 0), Quaternion.identity);
        }

        [PunRPC]
        void CharacterInstantiate()
        {

            if (GameManager.is_1p)
            {

                string gameCharacter1p_Name = GameManager.character.character_Name;
                string gameCharacter2p_Name = other_CharacterName;


                startPos_1p = map.transform.Find("1p_StartPos").gameObject.GetComponent<SpawnPositions>().spawnPosCharacter(gameCharacter1p_Name);
                startPos_2p = map.transform.Find("2p_StartPos").gameObject.GetComponent<SpawnPositions>().spawnPosCharacter(gameCharacter2p_Name);


                character_1p = PhotonNetwork.Instantiate(gameCharacter1p_Name, startPos_1p, Quaternion.identity);


                if (character_1p.GetComponent<MeleeCharacter>())
                {
                    character_1p.GetComponent<MeleeCharacter>().is_Right = true;
                }

                if (character_1p.GetComponent<RangedCharacter>())
                {
                    character_1p.GetComponent<RangedCharacter>().is_Right = true;
                }

                state = AppeardState.spawn;
            }

        }



        [PunRPC]
        void OtherCharacterFind()
        {

            GameObject[] streams = GameObject.FindGameObjectsWithTag("CharacterStream");

            for (int i = 0; i < streams.Length; i++)
            {
                if (streams[i].GetComponent<PhotonView>().IsMine == false)
                {
                    other_CharacterName = streams[i].GetComponent<CharacterStream>().other_Character_Name;
                    Debug.Log(other_CharacterName);
                }
            }
        }

        [PunRPC]
        void CharacterAppearedStart()
        {
            StartCoroutine(Character_Appeared());
        }

        [PunRPC]
        void CameraMove(Vector3 pos)
        {
            camera.transform.position = pos;
            camera.transform.Translate(0, 0, -10);
        }

        [PunRPC]
        void FindBattleStream()
        {
            if (battleStream == null || battleStream.Length < 2)
            {
                battleStream = GameObject.FindGameObjectsWithTag("BattleStream");
            }

            Debug.Log(countdown);

            if (battleStream.Length < 2)
                return;

            for (int i = 0; i < battleStream.Length; i++)
            {
                if (battleStream[i].GetComponent<PhotonView>().IsMine)
                {
                    if (PV.IsMine)
                    {
                        if (battleStream[i].GetComponent<BattleStream>().is_Ready)
                            break;

                        battleStream[i].GetComponent<BattleStream>().is_Ready = true;
                        Debug.Log(state);
                    }

                    if (!PV.IsMine)
                    {
                        if (countdown != null)
                        {
                            if (battleStream[i].GetComponent<BattleStream>().is_Ready)
                                break;

                            battleStream[i].GetComponent<BattleStream>().is_Ready = true;
                            Debug.Log(state);
                        }
                    }



                }
            }


            Debug.Log("1: " + battleStream[0].GetComponent<BattleStream>().is_Ready);
            Debug.Log("2: " + battleStream[1].GetComponent<BattleStream>().is_Ready);

            if (battleStream[0].GetComponent<BattleStream>().is_Ready  && battleStream[1].GetComponent<BattleStream>().is_Ready)
            {
                Debug.Log("!!!!!!!");
                is_countdownAllSet = true;
            }
        }

        [PunRPC]
        void CountdownStart()
        {
            if (battleStream.Length < 2)
            {
                FindBattleStream();
                return;
            }

            Debug.Log("CountDown");

            if (battleStream[0].GetComponent<BattleStream>().is_Ready == true && battleStream[1].GetComponent<BattleStream>().is_Ready == true)
            {
                PV.RPC("CountDownCorutineStart", RpcTarget.All);
                state = AppeardState.start;
            }
        }

        [PunRPC]
        void CountDownCorutineStart()
        {
            StartCoroutine(CountDown());
        }

        [PunRPC]
        void SetString(string str) // 소리가 2개 들릴 경우에는 이전 mapselect에서 했던 방식 생각해보기.
        {
            if(PV.IsMine)
            {
                PV.RPC("SetSFX", RpcTarget.All, str);
            }
            
            GameObject text = GameObject.FindGameObjectWithTag("CountDown");
            text.GetComponent<Text>().text = str;

        }

        [PunRPC]
        void SetSFX(string str)
        {
            if (str != "Start")
            {

                switch (str)
                {
                    case "3":
                        SoundManager.Instance.SFXInsert("Three");
                        break;
                    case "2":
                        SoundManager.Instance.SFXInsert("Two");
                        break;
                    case "1":
                        SoundManager.Instance.SFXInsert("One");
                        break;
                }

            }

            else
            {
                SoundManager.Instance.SFXInsert("GameStart");
            }
        }

        #endregion

        #region Private Methods

        void SetCameraSettingToBattle()
        {
            if (GameManager.is_1p)
            {
                camera.orthographicSize = zoomInCameraSize;
                camera.GetComponent<CameraSetting>().map = map;
                camera.GetComponent<CameraSetting>().character = character_1p;

                if (character_1p.GetComponent<MeleeCharacter>())
                {
                    character_1p.GetComponent<MeleeCharacter>().is_Control = true;
                }

                if (character_1p.GetComponent<RangedCharacter>())
                {
                    character_1p.GetComponent<RangedCharacter>().is_Control = true;
                }

                camera.GetComponent<CameraSetting>().MapSetBound();
                is_Battle_Start = false;

            }

            if (GameManager.is_2p)
            {
                camera.orthographicSize = zoomInCameraSize;
                camera.GetComponent<CameraSetting>().map = map;
                camera.GetComponent<CameraSetting>().character = character_2p;

                if (character_2p.GetComponent<MeleeCharacter>())
                {
                    character_2p.GetComponent<MeleeCharacter>().is_Control = true;
                }

                if (character_2p.GetComponent<RangedCharacter>())
                {
                    character_2p.GetComponent<RangedCharacter>().is_Control = true;
                }

                camera.GetComponent<CameraSetting>().MapSetBound();
                CameraMove(startPos_2p);
                is_Battle_Start = false;
            }
        }

        private void Set_2pComponent()
        {
            if (map == null)
            {
                map = GameObject.FindGameObjectWithTag("Map");
                characterSpawn.GetComponent<CharacterSpawnStream>().map = map;
            }
        }

        private void Set_OtherCharacter()
        {
            if (GameManager.is_1p && character_2p == null)
            {
                GameObject[] characters = GameObject.FindGameObjectsWithTag("Character").OrderBy(obj => obj.transform.GetSiblingIndex()).ToArray();

                for (int i = 0; i < characters.Length; i++)
                {
                    if (characters[i] != character_1p)
                    {
                        character_2p = characters[i];

                        hpUI_1p.SetActive(true);
                        hpUI_2p.SetActive(true);
                        TimeLimit_Text.gameObject.SetActive(true);
                        explain.SetActive(true);
                        hpUI_1p.GetComponent<HPUI>().SetCharacter1P(character_1p);
                        hpUI_2p.GetComponent<HPUI>().SetCharacter2P(character_2p);

                        return;

                    }
                }
            }

            if (GameManager.is_2p && character_1p == null)
            {
                GameObject[] characters = GameObject.FindGameObjectsWithTag("Character").OrderBy(obj => obj.transform.GetSiblingIndex()).ToArray();

                for (int i = 0; i < characters.Length; i++)
                {
                    if (characters[i] != character_2p)
                    {
                        character_1p = characters[i];

                        hpUI_1p.SetActive(true);
                        hpUI_2p.SetActive(true);
                        TimeLimit_Text.gameObject.SetActive(true);
                        explain.SetActive(true);
                        hpUI_1p.GetComponent<HPUI>().SetCharacter1P(character_1p);
                        hpUI_2p.GetComponent<HPUI>().SetCharacter2P(character_2p);

                        return;
                    }
                }
            }
        }

        private void CountdownSet2P()
        {
            if (!is_Countdown_Set)
                return;

            if (countdown == null)
            {
                countdown = GameObject.FindGameObjectWithTag("CountDown");
            }

            if (countdown == null)
                return;
                

            int index = map.name.IndexOf("(Clone)");
            if (index > 0)
                map.name = map.name.Substring(0, index);

            if (map.name == "Forest" || map.name == "Land")
            {
                countdown.GetComponent<Text>().color = Color.black;
            }

            is_Countdown_Set = false;
            GameManager.Instance.BattleCountDownStart(countdown);
        }

        private void AppearSetting()
        {
            if (PV.IsMine)
            {
                PV.RPC("MapSet", RpcTarget.MasterClient);
            }

            PhotonNetwork.Instantiate("CharacterStream", new Vector3(0, 0, 0), Quaternion.identity);
            PhotonNetwork.Instantiate("BattleStream", new Vector3(0, 0, 0), Quaternion.identity);
            characterSpawn = PhotonNetwork.Instantiate("CharacterSpawn", new Vector3(0, 0, 0), Quaternion.identity);
            characterSpawn.GetComponent<CharacterSpawnStream>().appearedManager = gameObject;
            battleStream = null;
        }

        #endregion

        #region Public Methods

        public void SetCharacter2p(GameObject character)
        {
            character_2p = character;
        }

        #endregion



        #region Corutine Methods

        IEnumerator Character_Appeared()
        {
            if (PV.IsMine)
                PV.RPC("CameraMove", RpcTarget.All, startPos_1p);


            GameObject appeared = Instantiate(appear_Anim, new Vector3(0, 0, 0), Quaternion.identity);

            GameManager.Instance.CharacterAppearAnimStart(appeared, 1, other_CharacterName);

            yield return new WaitForSeconds(appearaninTime);

            Destroy(appeared);

            if (PV.IsMine)
                PV.RPC("CameraMove", RpcTarget.All, startPos_2p);

            GameObject appeared_2 = Instantiate(appear_Anim, new Vector3(0, 0, 0), Quaternion.identity);

            GameManager.Instance.CharacterAppearAnimStart(appeared_2, 2, other_CharacterName);

            yield return new WaitForSeconds(appearaninTime);

            Destroy(appeared_2);

            if (PV.IsMine)
                PV.RPC("CameraMove", RpcTarget.All, Vector3.zero);

            camera.orthographicSize = zoomOutCamearaSize;

            if (PV.IsMine)
            {
                countdown = PhotonNetwork.Instantiate("CountDownText", Vector3.zero, Quaternion.identity);
            }


            if (PV.IsMine)
            {
                int index = map.name.IndexOf("(Clone)");
                if (index > 0)
                    map.name = map.name.Substring(0, index);

                if (map.name == "Forest" || map.name == "Land")
                {
                    countdown.GetComponent<Text>().color = Color.black;
                }

                is_countdown_ready = true;
                GameManager.Instance.BattleCountDownStart(countdown);
            }

            is_Countdown_Set = true;
            




        }

        IEnumerator CountDown()
        {
            if (countdown == null)
            {
                countdown = GameObject.FindGameObjectWithTag("CountDown");
            }

            Animator count_Anim = countdown.GetComponent<Animator>();
            count_Anim.SetBool("Count", true);

            if(PV.IsMine)
                PV.RPC("SetString", RpcTarget.All, "3");

            yield return new WaitForSeconds(1f);

            if (PV.IsMine)
                PV.RPC("SetString", RpcTarget.All, "2");

            yield return new WaitForSeconds(1f);

            if (PV.IsMine)
                PV.RPC("SetString", RpcTarget.All, "1");

            yield return new WaitForSeconds(1f);

            if (PV.IsMine)
                PV.RPC("SetString", RpcTarget.All, "Start");

            count_Anim.SetBool("Count", false);

            Debug.Log("Battle Start");

            Destroy(countdown);

            is_Battle_Start = true;

            
        }

        #endregion


    }
}
