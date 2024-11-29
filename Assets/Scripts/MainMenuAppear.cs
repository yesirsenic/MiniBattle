using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Com.MiniBattle.Game
{


    public class MainMenuAppear : MonoBehaviour
    {

        #region Serialize Fields

        [SerializeField]
        private GameObject buttons;

        #endregion

        #region Public Fields

        public Animator anim;

        #endregion

        #region Public Methods

        public void AppearEnd()
        {
            buttons.SetActive(true);
            SoundManager.Instance.BGMInsert("MainmenuBGM");
            GameManager.Instance.__Init__();
        }

        #endregion

    }
}
