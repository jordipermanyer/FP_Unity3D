using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class PlayerControllerScript : MonoBehaviourPun,IPunObservable
    {
        //Photon Team
        public string team = "NoTeam"; // Team
        public string text = ""; // Text for the player info

        //Movment
        public float speed = 7.0f;
        public float sprintSpeed = 13.0f;
        
        //Jump
        private bool isGrounded;
        public float jumpForce = 1f;

        //Camera
        public float rotationSpeed = 30f;
        private Transform cameraTransform;
        private Rigidbody rb;
        private Transform player;

        //Animations
        private Animator animator;
        private float velX = 0.0f;
        private float velZ = 0.0f;
        public float acc = 2.0f;
        public float decel = 2.0f;

        //Player
        public float health = 100.0f;


        void Start()
        {
            player = transform;
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
                // We add the camera with an offset
                cameraTransform = playerCamera.transform;
                Vector3 newPosition = cameraTransform.localPosition;
                newPosition.x += 1f;
                cameraTransform.localPosition = newPosition;

                Vector3 directionToCamera = cameraTransform.position - player.transform.position;
                directionToCamera.y = 0; // Ensure we don't tilt the player up or down, only rotate horizontally

                Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * 5f);

                Debug.Log("Camera successfully assigned from the player prefab.");
            }
            else
            {
                Debug.LogError("No camera found as a child of the player prefab!");
            }

            //Evitar bugs camera rotation
            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.freezeRotation = true; // Lock all rotations
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (photonView.IsMine) //only moves if the player is yourself
            {
                movment();
                shoot();
                
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
            UpdateAnimationState();
        }

        private void GetInput()
        {
            if (cameraTransform == null) return; // Prevent movement if cameraTransform is null

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            float currentSpeed = isSprinting ? sprintSpeed : speed;

            Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

            if (inputDirection.magnitude >= 0.1f)
            {
                // Calculate the movement direction relative to the camera
                Vector3 moveDirection = cameraTransform.forward * inputDirection.z + cameraTransform.right * inputDirection.x;
                moveDirection.y = 0f; // Ensure the character doesn't move vertically

                transform.Translate(moveDirection * currentSpeed * Time.deltaTime, Space.World);

                velX = Mathf.Lerp(velX, inputDirection.x * (isSprinting ? 2.0f : 1.0f), Time.deltaTime * acc);
                velZ = Mathf.Lerp(velZ, inputDirection.z * (isSprinting ? 2.0f : 1.0f), Time.deltaTime * acc);
            }
            else
            {
                // Decelerate parameters smoothly
                velX = Mathf.Lerp(velX, 0f, Time.deltaTime * decel);
                velZ = Mathf.Lerp(velZ, 0f, Time.deltaTime * decel);
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
            //Debug.Log("Is Grounded: " + isGrounded);
            if (isGrounded && Input.GetKeyDown(KeyCode.Space)) // Only jump if grounded
            {
                Debug.Log("Jump");
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply a jump force upwards
                
            }
        }

        private void UpdateAnimationState()
        {
            if (isGrounded)
            {
                animator.SetBool("isJumping", false);
            }
            else
            {
                animator.SetBool("isJumping", true);
            }
            animator.SetFloat("VelocityX", velX);
            animator.SetFloat("VelocityY", velZ);
        }

        private void shoot()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {

                    Debug.Log("Raycast hit: " + hit.collider.name);

                }
            }
        }



        void OnTriggerEnter(Collider other)
        {
            // Check if the player is colliding with the ground
            if (!other.CompareTag("Player"))
            {
                isGrounded = true;
                Debug.Log("Player is grounded");
            }
        }

        void OnTriggerExit(Collider other)
        {
            // Check if the player has left the ground
            if (!other.CompareTag("Player"))
            {
                isGrounded = false;
                Debug.Log("Player is not grounded");
            }
        }


    }


    
}

