using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Com.MiniBattle.Game
{


    public class MapSlot : MonoBehaviour
    {
        #region Public Fields

        public Map map;
        public Sprite map_BackGround;
        public GameObject backGround;
        public Text map_Text;

        #endregion

        #region Public Methods

        public void ChangeBackGround()
        {
            backGround.GetComponent<Image>().sprite = map_BackGround;

            map_Text.text = map.map_Name;

            if(map.map_Name == "Cave" || map.map_Name == "Town")
            {
                map_Text.color = Color.white;
            }

            else
            {
                map_Text.color = Color.black;
            }
        }

        #endregion

    }
}
