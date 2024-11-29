using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Com.MiniBattle.Game
{


    public class GameFinishAnim : MonoBehaviour
    {

        #region Serialize Field
        [SerializeField]
        Text text;

        #endregion

        #region Public Methods

        public void TextChange()
        {
            text.text = GameManager.finish_Text;
        }

        public void PopupSFX(string name)
        {
            SoundManager.Instance.SFXInsert(name);
        }

        #endregion
    }
}
