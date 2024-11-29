using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MiniBattle.Game
{


    public class CharacterInput : MonoBehaviour
    {
        #region Serialize Fields
        [Tooltip("캐릭터 스피드")]
        [SerializeField]
        private float speed = 10f;

        [Tooltip("캐릭터 점프 파워")]
        [SerializeField]
        private float jump_Power = 175f;

        [Tooltip("점프 할때 중력 값")]
        [SerializeField]
        private float jump_UpGravity = 1.5f;

        [Tooltip("점프후 내려올 때 중력 값")]
        [SerializeField]
        private float jump_DownGravity = 8f;

        #endregion

        #region Private Fields

        private int moveHorizontal =0;
        private Vector2 inputHorizontalVelocity = Vector2.zero;

        #endregion

        #region Pulbic Fields

        public float Hp = 100f;
        public float present_Hp = 100f;
        public float damage = 5f;

        #endregion

        #region Private Methods

        private void PlayClip(AudioSource audio, string clipName, bool loop)
        {
            if (audio.clip != null && audio.clip.name == clipName)
                return;

            audio.clip = (AudioClip)Resources.Load("Audio/SFX/" + clipName);
            audio.loop = loop;
            audio.Play();
        }

        #endregion

        #region Protected Virtual Methods

        protected virtual void Character_Move(ref bool is_Right,Rigidbody2D rb, SpriteRenderer spriteRenderer , Animator anim)
        {
            moveHorizontal = 0;
            

            if (Input.GetKey(KeyCode.RightArrow))
            {
                anim.SetBool("Move", true);
                gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
                moveHorizontal = 1;
                is_Right = true;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                anim.SetBool("Move", true);
                gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
                moveHorizontal = -1;
                is_Right = false;
            }

            if(!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            {
                anim.SetBool("Move", false);
            }
                
            inputHorizontalVelocity = new Vector2(moveHorizontal * speed, rb.velocity.y);

            rb.velocity = inputHorizontalVelocity;

        }

        protected virtual void Character_Jump(Rigidbody2D rb, ref bool jumping, ref bool is_LongJump, Animator anim)
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {

                if(!jumping)
                {
                    rb.AddForce(Vector3.up * jump_Power, ForceMode2D.Impulse);
                    jumping = true;
                    anim.SetBool("Jump", true);
                    Debug.Log("Jump");
                }
            }

            if(Input.GetKey(KeyCode.Space))
            {
                is_LongJump = true;
            }

            else if(Input.GetKeyUp(KeyCode.Space))
            {
                is_LongJump = false;
            }
        }

        protected virtual void Character_LongJump(Rigidbody2D rb, ref bool is_LongJump)
        {
            if(is_LongJump == true && rb.velocity.y>0)
            {
                rb.gravityScale = jump_UpGravity;
            }

            else
            {
                rb.gravityScale = jump_DownGravity;
            }
        }

        protected virtual void Character_Damaged(ref bool is_Damaged ,Animator anim)
        {
            if(present_Hp != Hp && is_Damaged == false && !anim.GetCurrentAnimatorStateInfo(0).IsName("Damaged"))
            {
                is_Damaged = true;
                anim.SetBool("Damaged", true);
                present_Hp = Hp;
                Debug.Log("Damaged");
            }
        }

        protected virtual void Character_Dead(ref bool is_Death, Animator anim)
        {
            if(Hp ==0 && is_Death == false)
            {
                is_Death = true;
                present_Hp = Hp;
                Debug.Log("Death");
            }
        }

        protected virtual void CharacterSFX(AudioSource audio, Animator anim)
        {

            if (anim.GetBool("Damaged"))
            {
                PlayClip(audio, "Character Damaged", false);
                return;
            }

            if (!anim.GetBool("Damaged") && audio.clip != null && audio.clip.name == "Character Damaged")
            {
                audio.Stop();
                audio.clip = null;
            }

            if (anim.GetBool("Attack"))
            {
                return;
            }

            if (anim.GetBool("Jump"))
            {
                PlayClip(audio, "Character Jump", false);
                return;
            }

            if (!anim.GetBool("Jump") && audio.clip != null && audio.clip.name == "Character Jump")
            {
                audio.Stop();
                audio.clip = null;
            }

            if (anim.GetBool("Move"))
            {
                PlayClip(audio, "Character Move", true);
            }
            else
            {
                if (audio.clip != null && audio.clip.name == "Character Move")
                {
                    audio.Stop();
                    audio.clip = null;
                }
            }
        }
        
        protected virtual void DeathSFX(AudioSource audio, Animator anim)
        {
            if (anim.GetBool("Death"))
            {
                PlayClip(audio, "Character Death", false);
                return;
            }

            if (!anim.GetBool("Death") && audio.clip != null && audio.clip.name == "Character Death")
            {
                audio.Stop();
                audio.clip = null;
            }
        }

        protected virtual void AttackSFX(string name, AudioSource characteraudio)
        {
            AudioClip clip = (AudioClip)Resources.Load("Audio/SFX/" + name);
            characteraudio.clip = clip;
            characteraudio.loop = false;
            characteraudio.Play();
        }

        



        #endregion


    }
}
