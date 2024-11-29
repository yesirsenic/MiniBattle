using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


namespace Com.MiniBattle.Game
{


    public class Matching_Animation : MonoBehaviourPunCallbacks
    {
        #region Serialize Fields

        [SerializeField]
        private float delay;

        [SerializeField]
        private Text targetText;

        #endregion

        #region Private Fields
        string text;
        float textPlayDelay;
        float panelDelay;
        Animator anim;


        #endregion

        #region PunMonobehaviour Methods

        public override void OnCreatedRoom()
        {
            Debug.Log("�� ����� �Ϸ�");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogWarningFormat("��� �������� ���� �������� ���� {0}", message);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("�� ���� �Ϸ�");

            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                GameManager.is_1p = true;
                Debug.Log("1p");
            }

            else
            {
                GameManager.is_2p = true;
                Debug.Log("2p");
            }

        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogWarningFormat("��� �������� �뿡 ���ӵ��� ���� {0}", message);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogWarningFormat("��� �������� �� ���� ���� ���� {0}", message);

            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
        }

        #endregion

        #region Monobehaviour Methods

        private void Start()
        {
            delay = 0.15f;
            textPlayDelay = 0.5f;
            panelDelay = 2f;
            text = "Search\nOther\nPlayer";
            anim = gameObject.GetComponent<Animator>();
        }

        #endregion

        #region Public Methods

        public void TextWriteStart()
        {
            StartCoroutine(textPrint(delay));
        }

        public void PanelSetStart()
        {
            StartCoroutine(PanelStart());
        }

        public void SFXPlay(string name)
        {
            SoundManager.Instance.SFXInsert(name);
        }

        #endregion

        #region Private Methods

        private void Connect() //�� ����
        {

            if (PhotonNetwork.IsConnected)
            {

                Debug.Log("��Ī �뿡 ����");
                PhotonNetwork.JoinRandomRoom();



            }

            else
            {
                Debug.Log("�뿡 ���� �����Ͽ� �ٽ� �õ� ��");
                PhotonNetwork.ConnectUsingSettings();

            }
        }

        #endregion


        #region Coroutines Methods

        IEnumerator textPrint(float d)
        {
            Debug.Log(text.ToString());
            int count = 0;

            AudioSource source = SoundManager.Instance.TextSFX();
            AudioClip AudioClip = (AudioClip)Resources.Load("Audio/SFX/PaperPen");

            while (count != text.Length)
            {
                if (count < text.Length)
                {
                    targetText.text += text[count].ToString();
                    source.clip = AudioClip;
                    if (!source.isPlaying)
                        source.Play();
                    count++;
                }

                yield return new WaitForSeconds(d);
            }

            GameManager.is_MatchingReady = true;

            yield return new WaitForSeconds(textPlayDelay);

            StartCoroutine(TextPlay());

            Connect();
        }

        IEnumerator TextPlay()
        {
            int count = 0;
            SoundManager.Instance.BGMInsert("Matching_Crowd");

            while(GameManager.is_MatchingReady)
            {
                if(count ==3)
                {
                    targetText.text = "Search\nOther\nPlayer";
                    count = 0;
                }

                else
                {              
                    targetText.text += ".";
                    count += 1;
                }

                yield return new WaitForSeconds(textPlayDelay);
            }
        }

        IEnumerator PanelStart()
        {
            yield return new WaitForSeconds(panelDelay);

            anim.SetBool("Match", true);

            StartCoroutine(FadeOutCoroutine());


        }

        private IEnumerator FadeOutCoroutine()
        {
            float currentTime = 0;
            AudioSource bgmAudioSource = SoundManager.Instance.gameObject.GetComponent<AudioSource>();
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            float initialVolume = bgmAudioSource.volume;
            float fadeDuration = stateInfo.length;

            while (currentTime < fadeDuration)
            {
                currentTime += Time.deltaTime;
                bgmAudioSource.volume = Mathf.Lerp(initialVolume, 0, currentTime / fadeDuration);
                yield return null;
            }

            // ������ ������ 0���� �����ϰ� ������� ����
            bgmAudioSource.volume = 0;
        }

        #endregion
    }
}
