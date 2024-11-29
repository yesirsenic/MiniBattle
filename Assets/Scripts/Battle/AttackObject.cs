using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Com.MiniBattle.Game
{

    public class AttackObject : MonoBehaviour
    {
        #region Private Methods

        float damage;

        #endregion


        #region MonoBehaviour Methods

        private void Start()
        {
            if (gameObject.transform.parent.GetComponent<MeleeCharacter>())
                damage = gameObject.transform.parent.GetComponent<MeleeCharacter>().damage;

            if (gameObject.transform.parent.GetComponent<RangedCharacter>())
                damage = gameObject.transform.parent.GetComponent<RangedCharacter>().damage;


        }




        #endregion

        #region Private Methods

        #endregion


        private void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.tag == "Character")
            {
                if (collision.GetComponent<MeleeCharacter>() && collision.GetComponent<MeleeCharacter>().is_Damaged == false)
                    collision.GetComponent<MeleeCharacter>().Hp -= damage;

                if (collision.GetComponent<RangedCharacter>() && collision.GetComponent<RangedCharacter>().is_Damaged == false)
                    collision.GetComponent<RangedCharacter>().Hp -= damage;

            }
        }

       

    }
}
