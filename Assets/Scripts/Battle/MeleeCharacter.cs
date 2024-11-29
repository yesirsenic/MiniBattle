using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.MiniBattle.Game
{

    public class MeleeCharacter : CharacterInput , IPunObservable
    {
        #region Private Fields

        private Vector3 black;
        private Vector3 usual;
        private Vector2 networkPosition;
        private Rigidbody2D player_Rb;
        private SpriteRenderer player_SpriteRender;
        private Animator player_Anim;
        private GameObject attack_Object;
        private Vector2 presentVelocity;
        private Vector2 trigger_Recognize_Vector;
        private AudioSource audio;
        private PhotonView PV;

        #endregion

        #region Public Fields


        public bool is_Control;
        public bool is_Right;
        public bool is_Jump;
        public bool is_LongJump;
        public bool is_Attack;
        public bool is_Damaged;
        public bool is_Death;

        #endregion

        #region Photon Methods

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(player_Rb.position);
                stream.SendNext(is_Right);
                stream.SendNext(player_Rb.velocity);
                stream.SendNext(Hp);
                stream.SendNext(is_Death);
            }
            else
            {
                networkPosition = (Vector2)stream.ReceiveNext();
                is_Right = (bool)stream.ReceiveNext();
                player_Rb.velocity = (Vector2)stream.ReceiveNext();
                Hp = (float)stream.ReceiveNext();
                is_Death = (bool)stream.ReceiveNext();

                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
                networkPosition += (this.player_Rb.velocity * lag);
            }
        }


        #endregion


        #region Monobehaviour Methods

        private void Start()
        {
            is_Control = false;
            is_Jump = false;
            is_LongJump = false;
            is_Attack = false;
            is_Damaged = false;
            is_Death = false;
            black = new Vector3(Color.gray.r, Color.gray.g, Color.gray.b);
            usual = new Vector3(255, 255, 255);
            trigger_Recognize_Vector = new Vector2(0.0001f, 0);
            player_Rb = GetComponent<Rigidbody2D>();
            player_SpriteRender = GetComponent<SpriteRenderer>();
            player_Anim = GetComponent<Animator>();
            attack_Object = gameObject.transform.Find("AttackObject").gameObject;
            attack_Object.SetActive(false);
            audio = gameObject.GetComponent<AudioSource>();
            PV = gameObject.GetPhotonView();
        }

        private void FixedUpdate()
        {
            if (!PV.IsMine)
            {
                if ((player_Rb.position - networkPosition).sqrMagnitude >= 10f * 10.0f)
                {
                    player_Rb.position = networkPosition;
                }

                else
                {
                    player_Rb.position = Vector2.Lerp(player_Rb.position, networkPosition, Time.deltaTime * 10.0f);

                }




                }

                if (is_Right)
                gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
            else
                gameObject.transform.eulerAngles = new Vector3(0, -180, 0);

            if (GameManager.is_Game_Finish)
                return;

            if (!is_Control)
                return;

            if (is_Death)
                return;

            if (is_Attack == true)
                return;

            if (is_Damaged == true)
                return;



            if(PV.IsMine)
            {
                Character_Move(ref is_Right, player_Rb, player_SpriteRender, player_Anim);
            }
            

        }

        private void Update()
        {
            DeathSFX(audio, player_Anim);

            if (GameManager.is_Game_Finish)
                return;

            Game_Finish();

            CharacterSFX(audio, player_Anim);

            if (!is_Control)
                return;

            if (is_Death)
                return;


            if (PV.IsMine)
            {

                Character_Dead(ref is_Death, player_Anim);
                Character_Damaged(ref is_Damaged, player_Anim);

                CharacterChangeColor();

                if (is_Damaged == true)
                    return;

                Character_LongJump(player_Rb, ref is_LongJump);
                Character_Jump(player_Rb, ref is_Jump, ref is_LongJump, player_Anim);


                if (is_Attack == true)
                    return;

                Character_Attack();
            }

            
        }

        #endregion

        #region CharacterInput Methods

        protected override void Character_Move(ref bool is_Right,Rigidbody2D rb, SpriteRenderer spriteRenderer, Animator anim)
        {
            base.Character_Move(ref is_Right,rb, spriteRenderer , anim);
        }

        protected override void Character_Jump(Rigidbody2D rb, ref bool jumping, ref bool longJump, Animator anim)
        {

            base.Character_Jump(rb, ref jumping , ref longJump, anim);
        }

        protected override void Character_LongJump(Rigidbody2D rb, ref bool is_LongJump)
        {
            base.Character_LongJump(rb, ref is_LongJump);
        }

        protected override void Character_Damaged(ref bool is_Damaged, Animator anim)
        {
            base.Character_Damaged(ref is_Damaged, anim);
        }

        protected override void Character_Dead(ref bool is_Death, Animator anim)
        {
            base.Character_Dead(ref is_Death, anim);
        }

        protected override void CharacterSFX(AudioSource audio, Animator anim)
        {
            base.CharacterSFX(audio, anim);
        }

        protected override void AttackSFX(string name, AudioSource characteraudio)
        {
            base.AttackSFX(name, characteraudio);
        }

        protected override void DeathSFX(AudioSource audio, Animator anim)
        {
            base.DeathSFX(audio, anim);
        }

        #endregion

        #region Private Methods

        private void Character_Attack()
        {
            if (Input.GetKeyDown(KeyCode.A) && is_Attack == false)
            {
                player_Anim.SetBool("Attack", true);
            }

            if(player_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                if(player_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >=1f)
                {
                    player_Anim.SetBool("Attack", false);
                }
            }

        }

        private void CharacterChangeColor()
        {
            if(player_Anim.GetBool("Damaged") && gameObject.GetComponent<SpriteRenderer>().color != new Color(black.x,black.y,black.z))
            {
                PV.RPC("DamageSetColor", RpcTarget.All, black);
            }

            if(!player_Anim.GetBool("Damaged") && gameObject.GetComponent<SpriteRenderer>().color != new Color(usual.x, usual.y, usual.z))
            {
                PV.RPC("DamageSetColor", RpcTarget.All, usual);
            }
        }

        private void Game_Finish()
        {
            if(is_Death)
            {
                if(PV.IsMine)
                {
                    audio.Stop();
                    GameManager.Instance.BattleFinish(false);
                }

                else
                {
                    audio.Stop();
                    GameManager.Instance.BattleFinish(true);
                }
            }
        }

        #endregion

        #region Public Methods

        public void AttackSetBool(int set)
        {

            if(set == 1)
            {
                is_Attack = true;
            }

            else
            {
                is_Attack = false;
                player_Anim.SetBool("Attack", false);
            }
            
        }

        public void AttackOn()
        {
            attack_Object.SetActive(true);
            attack_Object.GetComponent<PolygonCollider2D>().offset = trigger_Recognize_Vector;
        }

        public void AttackOff()
        {
            attack_Object.SetActive(false);
            attack_Object.GetComponent<PolygonCollider2D>().offset = Vector2.zero;
        }

        public void DamagedOn()
        {
            presentVelocity = player_Rb.velocity;
            player_Rb.constraints = RigidbodyConstraints2D.FreezePositionX;
            player_Rb.constraints = RigidbodyConstraints2D.FreezePositionY;
            
        }

        public void DamagedOff()
        {
            is_Damaged = false;
            player_Anim.SetBool("Damaged", false);
            player_Rb.velocity = presentVelocity;
            player_Rb.constraints = RigidbodyConstraints2D.None;
            player_Rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        public void AttackSFXStart(string name)
        {
            AttackSFX(name, audio);
        }

        [PunRPC]
        public void DamageSetColor(Vector3 color)
        {
            Color c = new Color(color.x, color.y, color.z);

            gameObject.GetComponent<SpriteRenderer>().color = c;
            Debug.Log(gameObject.GetComponent<SpriteRenderer>().color);
        }
        #endregion

    }
}
