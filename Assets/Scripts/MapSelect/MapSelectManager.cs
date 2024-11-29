using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;

namespace Com.MiniBattle.Game
{


    public class MapSelectManager : MonoBehaviour
    {
        #region SerialzeField Fields
        [Tooltip("랜덤으로 하여 정해진 맵의 숫자")]
        [SerializeField]
        private int Map_Number;

        [Tooltip("맵 블록들")]
        [SerializeField]
        private GameObject[] Map_Blocks;

        #endregion

        #region Priavte Fields

        private Vector3 color_Red = new Vector3(1, 0, 0);
        private Vector3 color_Green = new Vector3(0, 1, 0);

        private int max_Map_Number = 5;
        private int max_FastTime = 4;
        private int total_MapCount = 4;
        private float fast_Map_Pass_Time;
        private float fast_Map_Pass_Delay = 0.2f;
        private float map_Select_Delay = 0.3f;
        private float map_Select_Delay_Plus = 0.1f;
        private bool is_Lastmove;

        private PhotonView PV;

        #endregion

        #region MonoBehaviour Methods

        private void Start()
        {
            is_Lastmove = false;
            Map_Number = Random.Range(1, max_Map_Number);
            fast_Map_Pass_Time = Random.Range(3, max_FastTime);
            PV = this.gameObject.GetPhotonView();
            Map_Blocks = GameObject.FindGameObjectsWithTag("MapBlock").OrderBy(obj => obj.transform.GetSiblingIndex()).ToArray();

            Debug.Log("맵 넘버:" + Map_Number);

            PV.RPC("SetColor", RpcTarget.AllBuffered, 0, color_Red);

            if(PV.IsMine)
                PV.RPC("Map_Select_Start", RpcTarget.MasterClient);


        }

        #endregion

        #region Photon Methods

        #endregion

        #region Public Fields

        #endregion

        #region Priavte Methods(PunRPC)

        [PunRPC]
        private void SetColor(int count, Vector3 color)
        {
            Color c = new Color(color.x, color.y, color.z);

            Map_Blocks[count].GetComponent<Image>().color = c;
            Map_Blocks[count].GetComponent<MapSlot>().ChangeBackGround();

        }

        [PunRPC]
        private void Map_Select_Start()
        {
            StartCoroutine(Map_Select());
        }

        [PunRPC]
        private void MapMoveSound()
        {
            SoundManager.Instance.SFXInsert("Map Block Move");
        }

        [PunRPC]
        private void MapSelectSound()
        {
            SoundManager.Instance.SFXInsert("Map_Select");
        }

        #endregion

        #region Private Corutine

        IEnumerator Map_Select()
        {
            int count = 0;
            float time = 0;



            while (time <= fast_Map_Pass_Time)
            {
                yield return new WaitForSeconds(fast_Map_Pass_Delay);

                PV.RPC("SetColor", RpcTarget.AllBuffered, count, color_Green);

                if (count < 4)
                    count += 1;

                if (count >= 4)
                    count = 0;

                PV.RPC("SetColor", RpcTarget.AllBuffered, count, color_Red);
                PV.RPC("MapMoveSound", RpcTarget.All);

                time += fast_Map_Pass_Delay;
            }

            int pass_BlockCount = 0;
            int now_BlockCount = 0;
            int max_move_Count = 0;


            pass_BlockCount = (int)(fast_Map_Pass_Time / fast_Map_Pass_Delay);

            if (fast_Map_Pass_Time % fast_Map_Pass_Delay > 0)
            {
                pass_BlockCount += 1;
            }

            now_BlockCount = pass_BlockCount / total_MapCount;

            Debug.Log("현재 블록 카운트: " + now_BlockCount);

            switch (now_BlockCount) // 0일때 첫번째 맵, 3일때는 4번째 맵.+ 숫자는 임시 디버깅후 어색하면 변경 예정
            {
                case 0:
                    max_move_Count = Map_Number + 6;
                    break;

                case 1:
                    max_move_Count = Map_Number + 5;
                    break;

                case 2:
                    max_move_Count = Map_Number + 4;
                    break;

                case 3:
                    max_move_Count = Map_Number + 3;
                    break;
            }

            int move_Count = 0;

            while (move_Count <= max_move_Count)
            {
                yield return new WaitForSeconds(map_Select_Delay);

                if (move_Count == max_move_Count)
                {
                    PV.RPC("MapSelectSound", RpcTarget.All);
                }

                else
                {
                    PV.RPC("MapMoveSound", RpcTarget.All);
                }
                    


                

                PV.RPC("SetColor", RpcTarget.AllBuffered, count, color_Green);

                if (count < 4)
                    count += 1;

                if (count >= 4)
                    count = 0;

                PV.RPC("SetColor", RpcTarget.AllBuffered, count, color_Red);
                

                move_Count++;
                map_Select_Delay += map_Select_Delay_Plus;
            }



            GameManager.map = Map_Blocks[Map_Number-1].GetComponent<MapSlot>().map;
            PhotonNetwork.Instantiate("BattleSceneMove", new Vector3(0, 0, 0), Quaternion.identity);





        }

        #endregion
    }
}
