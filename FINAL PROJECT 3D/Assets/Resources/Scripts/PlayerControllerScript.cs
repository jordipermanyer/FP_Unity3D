using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor.PackageManager;


namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class PlayerControllerScript : MonoBehaviourPun,IPunObservable
    {
        //Photon Team
        public string team = "NoTeam"; // Team

        //Movment
        public float speed = 8.0f;
        public float sprintSpeed = 11.5f;
        
        //Jump
        private bool isGrounded;
        public float jumpForce = 18f;

        //Camera
        public float rotationSpeed = 30f;
        private Transform cameraTransform;
        private Rigidbody rb;
        private Transform player;
        private float verticalRotation = 0.0f;

        //Animations
        private Animator animator;
        private float velX = 0.0f;
        private float velZ = 0.0f;
        public float acc = 2.0f;
        public float decel = 2.0f;

        private TakeDamageScript takeDamage;

        //Player
        public float health = 100.0f;
        private float score = 0f;
        private bool isDead = false;
        public int killCount = 0;
        //Shooting
        private int bulletCount = 50;
        private bool canShoot = true;
        public float shootCooldown = 0.45f;


        void Start()
        {
            player = transform;
            animator = GetComponent<Animator>();
            takeDamage = GetComponent<TakeDamageScript>();

            if (!photonView.IsMine)
            {
                transform.GetChild(0).gameObject.SetActive(false); //disable camera
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



            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { "killCount", killCount }
            });

            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { "IsAlive", true }
            });
        }

        // Update is called once per frame
        void Update()
        {
            if (photonView.IsMine) //only moves if the player is yourself
            {
                if (isDead) return;
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

                rb.velocity = new Vector3(moveDirection.x * currentSpeed, rb.velocity.y, moveDirection.z * currentSpeed);

                velX = Mathf.Lerp(velX, inputDirection.x * (isSprinting ? 2.0f : 1.0f), Time.deltaTime * acc);
                velZ = Mathf.Lerp(velZ, inputDirection.z * (isSprinting ? 2.0f : 1.0f), Time.deltaTime * acc);
            }
            else
            {
                rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
                // Decelerate parameters smoothly
                velX = Mathf.Lerp(velX, 0f, Time.deltaTime * decel);
                velZ = Mathf.Lerp(velZ, 0f, Time.deltaTime * decel);
            }

            // Rotate the player based on mouse movement
            RotatePlayerWithMouse();
        }

        void RotatePlayerWithMouse()
        {
            // Get the mouse movement on the X-axis and rotate
            float mouseX = Input.GetAxis("Mouse X");
            float rotationAmount = mouseX * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up * rotationAmount);


            // Get the mouse movement on the Y-axis
            float mouseY = Input.GetAxis("Mouse Y");
            verticalRotation -= mouseY * rotationSpeed * Time.deltaTime * 0.8f;

            // Prevent excessive movement
            verticalRotation = Mathf.Clamp(verticalRotation, -20f, 20f);

            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }


        private void Jump()
        {
            //Debug.Log("Is Grounded: " + isGrounded);
            if (isGrounded && Input.GetKeyDown(KeyCode.Space)) // Only jump if grounded
            {
                Debug.Log("Jump");
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply a jump force upwards
                isGrounded = false;
            }
        }

        private void UpdateAnimationState()
        {
            animator.SetBool("isJumping", !isGrounded);
            animator.SetFloat("VelocityX", velX);
            animator.SetFloat("VelocityY", velZ);
        }

        private void shoot()
        {
            if (Input.GetMouseButtonDown(0) && canShoot && bulletCount > 0)
            {
                bulletCount--;
                canShoot = false;

                animator.SetBool("isShooting", true);
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100.0f))
                {
                    Debug.Log("Raycast hit: " + hit.collider.name);
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        EnemyControllerScript enemyScript = hit.collider.GetComponent<EnemyControllerScript>();
                        PhotonView enemyPhotonView = enemyScript.GetComponent<PhotonView>();
                        enemyPhotonView.RPC("TakeDamage", RpcTarget.All, 10, PhotonNetwork.LocalPlayer.UserId);

                    }
                }
                StartCoroutine(StopShootingAnimation());
            }
        }

        private IEnumerator StopShootingAnimation()
        {
            // Assuming your shooting animation length is 0.5 seconds (adjust as needed)
            yield return new WaitForSeconds(shootCooldown);
            animator.SetBool("isShooting", false);
            canShoot = true;
        }




        /* COLISIONS I ATAC */



        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Floor"))
            {
                isGrounded = true;
            }
            else if (other.gameObject.CompareTag("Limit"))
            {
                Die();
            }
        }


        public void TakeDamage(Vector3 hitDirection)
        {
            if (isDead) return; // If the player is already dead, do nothing
            
            health -= 150;

            if (health <= 0)
            {
                Die();
            }
            else
            {
                rb.AddForce(hitDirection * 10f, ForceMode.Impulse);
                takeDamage.damageEffect(transform, 0.2f);
            }
        }

        
        private void Die()
        {
            isDead = true;

            animator.SetBool("isDying", true);

            StartCoroutine(DeathAnimation());

        }

        private IEnumerator DeathAnimation()
        {
            animator.SetBool("isDying", true);
            float deathAnimationLength = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            yield return new WaitForSeconds(deathAnimationLength);

            //Destroy(gameObject);

            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { "IsAlive", false }
            });
        }



        public void AddKill()
        {
            killCount++;
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { "killCount", killCount }
            });
            bulletCount += 5;
        }

        public int GetKills()
        {
            return killCount;
        }



    }


    
}

