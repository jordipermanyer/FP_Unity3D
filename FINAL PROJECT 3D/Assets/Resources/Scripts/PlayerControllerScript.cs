using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class PlayerControllerScript : MonoBehaviourPun,IPunObservable
    {
        public float speed = 5.0f; // Speed of the player movement
        public string team = "NoTeam"; // Team
        public string text = ""; // Text for the player info

        private Rigidbody rb;
        private Animator animator;
        private Vector3 moveDir;
        private bool isGrounded;
        public float jumpForce = 25f;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();

            if (!photonView.IsMine)
            {
                transform.GetChild(0).gameObject.SetActive(false); //disable camera
            }
            else
            {
                //defining the team of the local player
                if (Random.Range(0f, 1f) < 0.5f) // 50% chance for each team
                {
                    team += "Team Alpha";
                    text += "I'm on team Alpha";
                }
                else
                {
                    team += "Team Beta";
                    text += "I'm on team Beta";
                }

            }
        }
        // Update is called once per frame
        void Update()
        {
            if (photonView.IsMine) //only moves if the player is yourself
            {
                movment();
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(team);
            }
            else
            {
                team = (string)stream.ReceiveNext();
            }
        }
        private void OnGUI()
        {
            GUI.TextArea(new Rect(20, 120, 300, 100), text);
        }


        /* LOGICA MOVIMENT */

        public void movment()
        {
            /*
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(horizontal, 0, vertical);
            transform.Translate(movement * speed * Time.deltaTime, Space.World);

            // Rotación con el mouse (eje X)
            float mouseX = Input.GetAxis("Mouse X");
            if (Mathf.Abs(mouseX) > 0.01f)
            {
                transform.Rotate(Vector3.up, mouseX * speed * Time.deltaTime);
            }
            */
            GetInput();
            Jump();
            UpdateAnimationState();
        }

        private void GetInput()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 targetVelocity = moveDir * speed;
            if (!isGrounded && rb.velocity.y > 0)
            {
                targetVelocity *= 0.9f; // Reduce speed a bit when moving upwards
            }

            rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
        }

        private void Jump()
        {
            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset vertical velocity before jumping
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply jump force
            }
        }

        private void UpdateAnimationState()
        {
            if (moveDir == Vector3.zero && isGrounded) // Idle state
            {
                animator.SetInteger("state", 0);
            }
            else if (isGrounded) // Walking state
            {
                animator.SetInteger("state", 1);
            }
            else if (!isGrounded) // Jumping state
            {
                animator.SetInteger("state", 3);
            }
        }







    }


    
}

