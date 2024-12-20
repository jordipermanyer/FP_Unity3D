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

        private Animator animator;
        private Vector3 moveDir;
        private bool isGrounded;
        public float jumpForce = 25f;

        private CharacterController controller;
        private Vector3 movementDirection;

        public float rotationSpeed = 220f; // Degrees per second
        private Transform cameraTransform;

        void Start()
        {
            controller = GetComponent<CharacterController>();
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

            Camera playerCamera = GetComponentInChildren<Camera>();

            if (playerCamera != null)
            {
                cameraTransform = playerCamera.transform;
                Debug.Log("Camera successfully assigned from the player prefab.");
            }
            else
            {
                Debug.LogError("No camera found as a child of the player prefab!");
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
            GetInput();
            Jump();
            //UpdateAnimationState();
        }

        private void GetInput()
        {
            if (cameraTransform == null) return; // Prevent movement if cameraTransform is null

            // Get input
            float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrows
            float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down Arrows

            // Combine input into a direction relative to the camera
            Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

            if (inputDirection.magnitude >= 0.1f)
            {
                // Calculate the movement direction relative to the camera
                Vector3 moveDirection = cameraTransform.forward * inputDirection.z + cameraTransform.right * inputDirection.x;
                moveDirection.y = 0f; // Ensure the character doesn't move vertically

                // Move the player
                transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
            }

            // Rotate the player based on mouse movement
            RotatePlayerWithMouse();
        }

        void RotatePlayerWithMouse()
        {
            // Get the mouse movement on the X-axis
            float mouseX = Input.GetAxis("Mouse X");

            float rotationAmount = mouseX * rotationSpeed * Time.deltaTime;

            // Apply the rotation around the Y-axis (horizontal)
            transform.Rotate(Vector3.up * rotationAmount);
        }


        private void Jump()
        {
            
        }

        private void UpdateAnimationState()
        {
            /*
             ANIMTAIONS variable = state (Ignorar encara treballant)
            Idle
                1  normal
                2  shooting
                3  crouch
            Walk
                4 recte
                5 esquerra
                6 dreta
                7 backwards
            Run
                8 Normal
                9 Aiming (! disparar)
            Shoot
                10
                11
                   

            Extra logic
                Bool isJumping
                Bool isDying

             */


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

