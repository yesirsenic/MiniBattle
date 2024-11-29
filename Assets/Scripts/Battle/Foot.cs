using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Com.MiniBattle.Game
{
    public class Foot : MonoBehaviour
    {
        #region Private Fields

        private GameObject character;
        private Rigidbody2D player_Rb;
        private Animator player_Anim;

        #endregion

        #region Public Fields

        public bool is_Ranged;

        #endregion 

        #region Monobehaviour Methods

        private void Start()
        {
            character = gameObject.transform.parent.gameObject;
            player_Rb = character.GetComponent<Rigidbody2D>();
            player_Anim = character.GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            
        }

        private void Update()
        {
            RayDetect();
        }

        #endregion

        #region Private Methods

        void RayDetect()
        {
            if (player_Rb.velocity.y <= 0)
            {
                Debug.DrawRay(gameObject.transform.position, Vector3.down, new Color(0, 1, 0));
                // �Ʒ��� Ray ���,
                RaycastHit2D rayHit = Physics2D.Raycast(gameObject.transform.position, Vector3.down, 1,LayerMask.GetMask("Ground"));

                if (character.GetComponent<MeleeCharacter>())
                {
                    if (rayHit.collider != null && character.GetComponent<MeleeCharacter>().is_Jump == true)
                    {
                        // ���ߵ� ��ü�� ���̱����� 0.5���� ���� ��� 
                        if (rayHit.distance < 0.5f)
                        {
                            character.GetComponent<MeleeCharacter>().is_Jump = false;
                            player_Anim.SetBool("Jump", false);
                        }
                    }
                }

                if(character.GetComponent<RangedCharacter>())
                {
                    if (rayHit.collider != null && character.GetComponent<RangedCharacter>().is_Jump == true)
                    {
                        if (rayHit.distance < 0.5f)
                        {
                            character.GetComponent<RangedCharacter>().is_Jump = false;
                            player_Anim.SetBool("Jump", false);
                        }
                    }
                }

                
                
            }

            
        }

        #endregion


    }
}
