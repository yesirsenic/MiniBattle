using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;
using ExitGames.Client.Photon;


namespace Com.MiniBattle.Game
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Photon Callbacks

        #endregion

        #region Private Fields

        private static GameManager instance = null;
        private float delayTime_SceneMove = 4f;
        private float delayTime_MatchingMove = 5f;
        private float loading_Time = 3f;
        private float leaveRoomTime = 2f;
        private float gameSetDelay = 1.5f;
        private float deathDelay = 1.5f;
        private bool is_PausePopupOn;
        private string sceneName;
        private Coroutine character_SelectMove;
        private Coroutine map_SelectMove;
        private Coroutine battle_SelectMove;

        private GameObject pausePopup;
        private GameObject inGamePausePopup;

        private const string PlayerSceneKey = "PlayerScene";


        #endregion

        #region Public Fields

        public static bool is_SameScene = false;
        public static bool is_Loading = false;
        public static bool is_Matching = false; // 매칭되었는지를 확인하는 bool값
        public static bool is_MatchingReady = false;
        public static bool is_CharacterSelect = false;
        public static bool is_CharacterSelectComplete = false;
        public static bool is_MapSelect = false; // 맵 선택 씬 이동 되었는지 확인하는 bool 값.
        public static bool is_MapSelectComplete = false;
        public static bool is_Battle = false; // 배틀 씬 이동 되었는지 확인하는 bool 값.
        public static bool is_1p = false;
        public static bool is_2p = false;
        public static bool is_Win = true; //그 배틀에서 이겼는가를 판가름하는 bool 값.
        public static bool is_Game_Finish = false; //본 배틀이 끝났기에 캐릭터 조작이 안된다는 것을 말해주는 bool 값.
        public static bool is_Draw = false;
        public static bool is_PausePopupExit = false;   
        public static Character character;
        public static Map map;
        public static string finish_Text;

        public static GameManager Instance
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

        #region Private Methods

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
            is_PausePopupOn = false;
            pausePopup = Resources.Load<GameObject>("PausePopup");

        }

        private void Update()
        {
            if (is_Matching == false && is_MatchingReady)
            {
                Matching();
            }

            if (is_CharacterSelect == false)
                CharacterSelect();

            if (is_CharacterSelectComplete == true && is_MapSelect == false)
            {
                CharacterSelectComplete();
            }

            if (is_MapSelectComplete == true && is_Battle == false)
            {
                MapSelectComplete();
            }

            if(SceneManager.GetActiveScene().name != "Mainmenu")
            {
                PausePopupOnOff();
            }

            Scene_Change();

            DebugBool();

        }

        private void Matching()
        {
            if (SceneManager.GetActiveScene().name == "Matching")
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    Debug.Log("매칭 완료");
                    Text matching_Text = GameObject.Find("MatchText").GetComponent<Text>();
                    GameObject mainmenu_Button = GameObject.Find("MainmenuButton");
                    Matching_Animation anim_BackGround = GameObject.Find("BackGround").GetComponent<Matching_Animation>();
                    matching_Text.text = "Found\nOther\nPlayer";
                    mainmenu_Button.SetActive(false);
                    is_Matching = true;
                    is_MatchingReady = false;
                    anim_BackGround.PanelSetStart();
                    character_SelectMove = StartCoroutine(Character_SelectMove());

                }
            }
        }

        private void CharacterSelect()
        {
            if (SceneManager.GetActiveScene().name == "CharacterSelect" && is_SameScene == true)
            {
                Debug.Log("캐릭터 선택 시작");
                PhotonNetwork.Instantiate("CharacterSelectManager", new Vector3(0, 0, 0), Quaternion.identity);
                is_CharacterSelect = true;
            }
        }

        private void CharacterSelectComplete()
        {
            if (SceneManager.GetActiveScene().name == "CharacterSelect")
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    if (is_CharacterSelectComplete == true && is_MapSelect == false)
                    {
                        Debug.Log("선택한 캐릭터:" + character.character_Name);
                        GameObject text_Object = GameObject.Find("Text_Object");
                        text_Object.transform.GetChild(0).gameObject.SetActive(true);
                        SoundManager.Instance.SFXInsert("Ready To Start");

                        is_MapSelect = true;
                        map_SelectMove = StartCoroutine(Map_SelectMove());

                    }
                }


            }
        }

        private void MapSelectComplete()
        {
            if (SceneManager.GetActiveScene().name == "MapSelect")
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    if (is_MapSelectComplete == true && is_Battle == false)
                    {
                        if (map != null)
                            Debug.Log("선택한 맵: " + map.map_Name);
                        is_Battle = true;
                        battle_SelectMove = StartCoroutine(Battle_SelectMove());

                    }
                }
            }
        }

        private void PausePopupOnOff()
        {
            if (is_PausePopupExit)
            {
                PlayerPrefs.SetFloat("BGM", inGamePausePopup.GetComponent<AudioVolume>().BGMSlider.value);
                PlayerPrefs.SetFloat("SFX", inGamePausePopup.GetComponent<AudioVolume>().SFXSlider.value);
                Destroy(inGamePausePopup);
                is_PausePopupOn = false;
                is_PausePopupExit = false;
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                

                if (!is_PausePopupOn)
                {
                    GameObject can = GameObject.Find("Canvas");
                    inGamePausePopup = Instantiate(pausePopup, Vector3.zero, Quaternion.identity);
                    inGamePausePopup.transform.SetParent(can.transform);
                    inGamePausePopup.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                    inGamePausePopup.GetComponent<AudioVolume>().BGMSlider.value = PlayerPrefs.GetFloat("BGM");
                    inGamePausePopup.GetComponent<AudioVolume>().SFXSlider.value = PlayerPrefs.GetFloat("SFX");
                    SoundManager.Instance.SFXInsert("Button Click");
                    is_PausePopupOn = true;
                }

                else
                {
                    PlayerPrefs.SetFloat("BGM", inGamePausePopup.GetComponent<AudioVolume>().BGMSlider.value);
                    PlayerPrefs.SetFloat("SFX", inGamePausePopup.GetComponent<AudioVolume>().SFXSlider.value);
                    Destroy(inGamePausePopup);
                    SoundManager.Instance.SFXInsert("Button Click");
                    is_PausePopupOn = false;
                }
            }
        }

        private void Scene_Change()
        {
            if(sceneName == null || sceneName != SceneManager.GetActiveScene().name)
            {
                is_PausePopupOn = false;
                is_SameScene = false;
                sceneName = SceneManager.GetActiveScene().name;
                SetPlayerScene(sceneName);
            }
        }

        private Texture2D CaptureScreen()
        {
            RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
            Camera.main.targetTexture = renderTexture;
            Camera.main.Render();
            RenderTexture.active = renderTexture;

            Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
            screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenImage.Apply();

            Camera.main.targetTexture = null;
            RenderTexture.active = null;
            Destroy(renderTexture);

            return screenImage;
        }

        

        private void CheckAllPlayersInSameScene()
        {
            string expectedScene = SceneManager.GetActiveScene().name; // 방 이름을 기준으로 씬 이름 확인
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.TryGetValue(PlayerSceneKey, out object sceneName))
                {
                    if ((string)sceneName != expectedScene)
                    {
                        Debug.Log("모든 플레이어가 아직 같은 씬에 있지 않습니다.");
                        is_SameScene = false;
                        return;
                    }
                }
                else
                {
                    Debug.Log("플레이어 중 일부가 아직 씬 정보를 설정하지 않았습니다.");
                    is_SameScene = false;
                    return;
                }
            }

            Debug.Log("모든 플레이어가 같은 씬에 있습니다!");
            is_SameScene = true;
        }

        private void AnimInit(Animator anim)
        {
            anim.SetBool("Move", false);
            anim.SetBool("Jump", false);
            anim.SetBool("Attack", false);
            anim.SetBool("Damaged", false);
        }

        private void DebugBool()
        {
            if(Input.GetKeyDown(KeyCode.F4))
            {
                Debug.Log("is_SameScene:" + is_SameScene);
                Debug.Log("is_Loading:" + is_Loading);
                Debug.Log("is_Matching:" + is_Matching);
                Debug.Log("is_MatchingReady:" + is_MatchingReady);
                Debug.Log("is_CharacterSelect:" + is_CharacterSelect);
                Debug.Log("is_CharacterSelectComplete:" + is_CharacterSelectComplete);
                Debug.Log("is_MapSelect:" + is_MapSelect);
                Debug.Log("is_MapSelectComplete:" + is_MapSelectComplete);
                Debug.Log("is_Battle:" + is_Battle);
                Debug.Log("is_1p:" + is_1p);
                Debug.Log("is_2p:" + is_2p);
                Debug.Log("is_Win:" + is_Win);
                Debug.Log("is_Game_Finish:" + is_Game_Finish);
            }
        }



        #endregion

        #region PunCallback Methods

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if(!is_Game_Finish)
            {
                StartCoroutine(ExitOtherPlayer());
            }
            
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if (changedProps.ContainsKey(PlayerSceneKey))
            {
                CheckAllPlayersInSameScene();
            }
        }


        #endregion

        #region Public Methods

        public void Loading_Close()
        {
            GameManager.is_Loading = false;
            GameObject loading = GameObject.Find("Loading");
            if (loading != null)
                loading.GetComponent<Loading>().GageControll();

        }

        public void CharacterAppearAnimStart(GameObject appeared , int order , string othercharacterName)
        {
            if (SceneManager.GetActiveScene().name == "Battle")
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    if (is_Battle == true && is_SameScene == true)
                    {
                        GameObject can = GameObject.Find("Canvas");
                        appeared.transform.SetParent(can.transform);
                        appeared.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
                        Character_AppearedrAnim char_Text = appeared.GetComponent<Character_AppearedrAnim>();

                        if(is_1p)
                        {
                            if (order == 1)
                                char_Text.targetText.text = GameManager.character.character_Name;
                            else if (order == 2)
                                char_Text.targetText.text = othercharacterName;
                        }

                        if(is_2p)
                        {
                            if (order == 1)
                                char_Text.targetText.text = othercharacterName;

                            else if(order==2)
                                char_Text.targetText.text = GameManager.character.character_Name;
                        }


                        char_Text.text = char_Text.targetText.text.ToString();
                        char_Text.targetText.text = " ";
                        Debug.Log("Text start");
                        appeared.GetComponent<Animator>().SetTrigger("start");
                    }
                }
            }
        }

        public void BattleCountDownStart(GameObject text)
        {
            if (SceneManager.GetActiveScene().name == "Battle")
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    if (is_Battle == true && text != null)
                    {
                        GameObject can = GameObject.Find("Canvas");
                        text.transform.SetParent(can.transform);
                        text.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                        text.GetComponent<Text>().text = "Ready?";
                    }
                }
            }
        }

        public void BattleFinish(bool is_Win)
        {
            if (SceneManager.GetActiveScene().name == "Battle")
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    if (is_Game_Finish)
                        return;


                    is_Game_Finish = true;
                    StartCoroutine(GameSetFinish(is_Win));
                }
            }
        }

        public void BattleFinishDraw()
        {
            if (SceneManager.GetActiveScene().name == "Battle")
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    if (is_Game_Finish)
                        return;


                    is_Game_Finish = true;
                    is_Draw = true;
                    StartCoroutine(GameSetFinish(true));



                }
            }
        }

        public void SetPlayerScene(string sceneName)
        {
            // ExitGames.Client.Photon.Hashtable 사용
            ExitGames.Client.Photon.Hashtable playerSceneProp = new ExitGames.Client.Photon.Hashtable
            {
                { PlayerSceneKey, sceneName }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerSceneProp);
        }

        public void __Init__()
        {
            is_SameScene = false;
            is_Loading = false;
            is_Matching = false;
            is_MatchingReady = false;
            is_CharacterSelect = false;
            is_CharacterSelectComplete = false;
            is_MapSelect = false;
            is_MapSelectComplete = false;
            is_Battle = false;
            is_1p = false;
            is_2p = false;
            is_Win = true;
            is_Game_Finish = false;
            is_Draw = false;
            character = null;
            map = null;
            SoundManager.Instance.AudioVolumeInit();
        }

        #endregion

        #region Coroutine Methods

        IEnumerator Character_SelectMove()
        {
            yield return new WaitForSeconds(delayTime_MatchingMove);

            SceneManager.LoadScene("CharacterSelect");
        }

        IEnumerator Map_SelectMove()
        {
            yield return new WaitForSeconds(delayTime_SceneMove);

            SceneManager.LoadScene("MapSelect");
        }

        IEnumerator Battle_SelectMove()
        {
            yield return new WaitForSeconds(delayTime_SceneMove);

            SceneManager.LoadScene("Battle");
        }

        IEnumerator ExitOtherPlayer()
        {
            Time.timeScale = 0;
            __Init__();
            if(character_SelectMove != null)
                StopCoroutine(character_SelectMove);
            if(map_SelectMove != null)
                StopCoroutine(map_SelectMove);
            if (battle_SelectMove != null)
                StopCoroutine(battle_SelectMove);

            GameObject exitPopup = Resources.Load("ExitPopup") as GameObject;

            GameObject can = GameObject.Find("Canvas");
            GameObject popUp = Instantiate(exitPopup, Vector3.zero, Quaternion.identity);
            popUp.transform.SetParent(can.transform);
            popUp.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

            Canvas.ForceUpdateCanvases();

            yield return new WaitForSecondsRealtime(leaveRoomTime);


            Time.timeScale = 1;
            PhotonNetwork.LeaveRoom();

            yield return new WaitForSeconds(0.1f);

            SceneManager.LoadScene("Mainmenu");


        }

        IEnumerator GameSetFinish(bool result)
        {
            GameObject target = null;
            GameObject othertarget = null;

            

            GameObject[] targets = GameObject.FindGameObjectsWithTag("Character").OrderBy(obj => obj.transform.GetSiblingIndex()).ToArray();

            for(int i = 0; i< targets.Length;i++)
            {
                if(targets[i].GetPhotonView().IsMine)
                {
                    target = targets[i];
                }

                else
                {
                    othertarget = targets[i];
                }
            }

            if(target == null || othertarget == null)
            {
                Debug.LogError("타겟을 찾지 못함.");
            }

            AnimInit(target.GetComponent<Animator>());
            SoundManager.Instance.gameObject.GetComponent<AudioSource>().Stop();
            SoundManager.Instance.SFXInsert("Game Set");

            yield return new WaitForSeconds(gameSetDelay);

            if (!is_Draw)
            {
               
                if (target.GetComponent<MeleeCharacter>())
                {
                    if (!result)
                        target.GetComponent<MeleeCharacter>().Hp = 0;

                    if (target.GetComponent<MeleeCharacter>().Hp == 0)
                    {
                        target.GetComponent<Animator>().SetBool("Death", true);
                    }
                }

                if (target.GetComponent<RangedCharacter>())
                {
                    if (!result)
                        target.GetComponent<RangedCharacter>().Hp = 0;

                    if (target.GetComponent<RangedCharacter>().Hp == 0)
                    {
                        target.GetComponent<Animator>().SetBool("Death", true);
                    }
                }

                yield return new WaitForSeconds(deathDelay);
            }

            



            GameObject captureImage = GameObject.Find("CaptureImage");
            captureImage.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            captureImage.GetComponent<RawImage>().texture = CaptureScreen();
            GameObject _instance = Resources.Load("GameFinishPopup") as GameObject;
            GameObject can = GameObject.Find("Canvas");
            GameObject Popup = Instantiate(_instance, Vector3.zero, Quaternion.identity);
            Popup.transform.SetParent(can.transform);
            Popup.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

            if(!is_Draw)
            {
                if (result)
                {
                    finish_Text = "WIN";
                    Popup.GetComponent<Animator>().SetBool("Win", true);
                    yield break;
                }

                else
                {
                    finish_Text = "LOSE";
                    Popup.GetComponent<Animator>().SetBool("Lose", true);
                    yield break;
                }
            }

            else
            {
                finish_Text = "DRAW";
                Popup.GetComponent<Animator>().SetBool("Draw", true);
                yield break;
            }
            
        }

        #endregion

    }
}
