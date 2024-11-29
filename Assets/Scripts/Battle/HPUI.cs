using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.MiniBattle.Game
{

    public class HPUI : MonoBehaviour
    {
        #region Public Fields

        public bool is_1p;

        #endregion

        #region Private Fields

        private GameObject character_1p;
        private GameObject character_2p;
        private string name_1p;
        private string name_2p;
        private float hp_Delay = 0.5f;
        private float presentHP;
        private bool is_HP_Change;

        private Slider bar;
        private GameObject backGround;
        private GameObject fillArea;
        private Image icon;

        #endregion

        #region Monobehaviour Methods

        private void Start()
        {
            if(is_1p)
            {
                bar = gameObject.transform.Find("1p_HPBar").GetComponent<Slider>();
                backGround = bar.gameObject.transform.Find("Background").gameObject;
                fillArea = bar.gameObject.transform.Find("Fill Area").gameObject;
                icon = gameObject.transform.Find("1p_CharacterIconBackGround").Find("1p_CharacterIcon").GetComponent<Image>();

            }

            else
            {
                bar = gameObject.transform.Find("2p_HPBar").GetComponent<Slider>();
                backGround = bar.gameObject.transform.Find("Background").gameObject;
                fillArea = bar.gameObject.transform.Find("Fill Area").gameObject;
                icon = gameObject.transform.Find("2p_CharacterIconBackGround").Find("2p_CharacterIcon").GetComponent<Image>();
            }

            presentHP = 100f;
            is_HP_Change = true;
        }

        private void Update()
        {
            if (character_1p == null && character_2p == null)
                return;

            SetIcon();


            if (!is_HP_Change)
                return;

            SetHpSync();

            
        }

        #endregion

        #region Private Methods

        private void SetHpSync()
        {
            if(is_1p)
            {
                if(character_1p.GetComponent<MeleeCharacter>() && presentHP != character_1p.GetComponent<MeleeCharacter>().Hp)
                {
                    bar.value = character_1p.GetComponent<MeleeCharacter>().Hp;
                    presentHP = character_1p.GetComponent<MeleeCharacter>().Hp;
                    is_HP_Change = false;
                    StartCoroutine(Hp_ChangeDelay());
                }

                if (character_1p.GetComponent<RangedCharacter>() && presentHP != character_1p.GetComponent<RangedCharacter>().Hp)
                {
                    bar.value = character_1p.GetComponent<RangedCharacter>().Hp;
                    presentHP = character_1p.GetComponent<RangedCharacter>().Hp;
                    is_HP_Change = false;
                    StartCoroutine(Hp_ChangeDelay());
                }
            }

            else
            {
                if (character_2p.GetComponent<MeleeCharacter>() && presentHP != character_2p.GetComponent<MeleeCharacter>().Hp)
                {
                    bar.value = character_2p.GetComponent<MeleeCharacter>().Hp;
                    presentHP = character_2p.GetComponent<MeleeCharacter>().Hp;
                    is_HP_Change = false;
                    StartCoroutine(Hp_ChangeDelay());
                }

                if (character_2p.GetComponent<RangedCharacter>() && presentHP != character_2p.GetComponent<RangedCharacter>().Hp)
                {
                    bar.value = character_2p.GetComponent<RangedCharacter>().Hp;
                    presentHP = character_2p.GetComponent<RangedCharacter>().Hp;
                    is_HP_Change = false;
                    StartCoroutine(Hp_ChangeDelay());
                }
            }
        }

        private void SetIcon()
        {
            if(is_1p)
            {
                if(icon.sprite == null)
                {
                    int index = character_1p.name.IndexOf("(Clone)");
                    if (index > 0)
                        name_1p = character_1p.name.Substring(0, index);

                    Sprite sprite = Resources.Load<Sprite>("CharacterSelect/Block_Image/" + name_1p + "_Block");
                    icon.sprite = sprite; 

                }
            }

            else
            {
                if (icon.sprite == null)
                {
                    int index = character_2p.name.IndexOf("(Clone)");
                    if (index > 0)
                        name_2p = character_2p.name.Substring(0, index);

                    Sprite sprite = Resources.Load<Sprite>("CharacterSelect/Block_Image/" + name_2p + "_Block");
                    icon.sprite = sprite;

                }
            }
        }

        

        #endregion


        #region Public Methods

        public void SetCharacter1P(GameObject character)
        {
            character_1p = character;
        }

        public void SetCharacter2P(GameObject character)
        {
            character_2p = character;
        }

        public void Hp_Zero_disappear()
        {
            if (bar.value == 0)
            {
                if (fillArea == null)
                    return;

                if (fillArea.activeSelf == true)
                    fillArea.SetActive(false);
            }
        }

        #endregion

        #region Corutine Methods

        IEnumerator Hp_ChangeDelay()
        {
            yield return new WaitForSeconds(hp_Delay);

            is_HP_Change = true;
        }

        #endregion
    }
}
