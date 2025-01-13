using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor.PackageManager;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun.Demo.Asteroids;


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
        public int health = 100;
        private bool isDead = false;

        //Shooting
        private int bulletCount = 50;
        private bool canShoot = true;
        public float shootCooldown = 0.45f;
        public GameObject explosionVfx;
        public Transform explotionSpawnPoint;

        private UIManagerScript gameUIManager;
        private Gun gunScript;
        private PlayerSoundScript playerSoundScript;

        public PhotonView view;

        private void Awake()
        {
            if (!view.IsMine)
            {
                this.gameObject.SetActive(false);
            }
        }

        void Start()
        {
            player = transform;
            animator = GetComponent<Animator>();
            takeDamage = GetComponent<TakeDamageScript>();
            gameUIManager = FindObjectOfType<UIManagerScript>();
            gameUIManager.UpdateHealth(health);
            gameUIManager.UpdateBullets(bulletCount);
            gunScript = GetComponent<Gun>();
            playerSoundScript = GetComponent<PlayerSoundScript>();

            if (!photonView.IsMine)
            {
                transform.GetChild(0).gameObject.SetActive(false); //disable camera
            }

            Camera playerCamera = GetComponentInChildren<Camera>();

            

            SetCameraPosition(transform, playerCamera);

            //Evitar bugs camera rotation
            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.freezeRotation = true; // Lock all rotations
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { "IsAlive", true }
            });
        }

        public void SetCameraPosition(Transform playerTransform, Camera playerCamera)
        {
            if (playerCamera != null)
            {
                cameraTransform = playerCamera.transform;

                // Set the local position offset
                Vector3 newPosition = cameraTransform.localPosition;
                newPosition.x += 1f;
                cameraTransform.localPosition = newPosition;

                // Adjust the rotation to face the player
                Vector3 directionToCamera = cameraTransform.position - playerTransform.position;
                directionToCamera.y = 0; // Ignore vertical axis for rotation
                Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

                playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, Time.deltaTime * 5f);
            }
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

                playerSoundScript.StartWalking();
            }
            else
            {
                rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
                // Decelerate parameters smoothly
                velX = Mathf.Lerp(velX, 0f, Time.deltaTime * decel);
                velZ = Mathf.Lerp(velZ, 0f, Time.deltaTime * decel);

                playerSoundScript.StopWalking();
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
            if (isGrounded && Input.GetKeyDown(KeyCode.Space)) // Only jump if grounded
            {
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
                gameUIManager.UpdateBullets(bulletCount);
                canShoot = false;


                Instantiate(explosionVfx, explotionSpawnPoint.position, Quaternion.identity);

                animator.SetBool("isShooting", true);
                RaycastHit hit;

                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    Debug.Log("Raycast hit: " + hit.collider.name);
                    float distanceToTarget = hit.distance; // Distance from camera to target

                    if (hit.collider.CompareTag("Enemy"))
                    {
                        EnemyControllerScript enemyScript = hit.collider.GetComponent<EnemyControllerScript>();
                        PhotonView enemyPhotonView = enemyScript.GetComponent<PhotonView>();
                        enemyPhotonView.RPC("TakeDamage", RpcTarget.All, 10, PhotonNetwork.LocalPlayer.UserId);
                    }

                    // Shoot the bullet and adjust speed to match the distance
                    gunScript.Shoot(ray.direction, distanceToTarget);
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




        /* COLISIONS , DAMAGE I MORT*/



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
            
            health -= 20;
            gameUIManager.UpdateHealth(health);

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
            yield return new WaitForSeconds(deathAnimationLength + 1.0f * Time.deltaTime);

            //Destroy(gameObject);

            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { "IsAlive", false }
            });
        }

        public void AddBullets(int amount)
        {
            if (photonView.IsMine)
            {
                Debug.LogWarning("Adding bullets");
                bulletCount += amount;
                if(bulletCount > 50)
                {
                    bulletCount = 50;   
                }
                gameUIManager.ShowPopup();
                gameUIManager.UpdateBullets(bulletCount);
            }
        }



        public bool isD()
        {
            return isDead;
        }



    }


    
}

