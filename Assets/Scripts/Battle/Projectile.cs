using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.MiniBattle.Game
{


    public class Projectile : MonoBehaviour 
    {
        #region Public Fields

        public GameObject myPlayer;
        public float damage = 10f;
        public Vector2 velocity;

        #endregion

        #region Private Fields

        private Rigidbody2D rb;
        private PhotonView PV;


        #endregion



        #region MonoBehaviour Methods

        private void Start()
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
            PV = gameObject.GetPhotonView();

            if (myPlayer.transform.rotation.y ==0)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }

            else 
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }


        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag != "Map")
            {


                if (collision.tag == "Character" && collision.gameObject != myPlayer)
                {
                    if (collision.GetComponent<MeleeCharacter>() && collision.GetComponent<MeleeCharacter>().is_Damaged == false)
                    {
                        collision.GetComponent<MeleeCharacter>().Hp -= damage;
                    }


                    if (collision.GetComponent<RangedCharacter>() && collision.GetComponent<RangedCharacter>().is_Damaged == false)
                    {
                        collision.GetComponent<RangedCharacter>().Hp -= damage;
                    }


                }

                if (collision.gameObject != myPlayer)
                {

                    Destroy(gameObject);
                }
            }

           
        }

        #endregion



    }
}
