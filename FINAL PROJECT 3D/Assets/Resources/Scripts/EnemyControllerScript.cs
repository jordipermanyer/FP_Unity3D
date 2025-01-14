using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class EnemyControllerScript : MonoBehaviour
    {
        public NavMeshAgent agent;
        private Transform player;
        public float detectionRadius = 10f;
        public float shootingRadius = 5f;
        private Animator animator;

        private bool isShooting = false;
        private float wanderRadius = 15f; // Radius for random wandering
        private float wanderInterval = 2f; // Time interval between setting new wander destinations
        private float wanderTimer;

        private float health = 30;
        private PhotonView photonViewEnemy;

        private float updateInterval = 1f;
        private float timeSinceLastUpdate = 0f;

        private bool isDying = false;

        public GameObject ammoBoxPrefab;
        public float ammoBoxHeight = 1f;

        public AudioClip[] missSound;
        private EnemySoundScript enemySoundScript;

        void Awake()
        {
            photonViewEnemy = GetComponent<PhotonView>();
        }

        void Start()
        {
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            wanderTimer = wanderInterval;
            enemySoundScript = GetComponent<EnemySoundScript>();

            if (PhotonNetwork.IsConnected)
            {
                player = GetClosestPlayer();
                timeSinceLastUpdate = 0f;
            }
        }

        Transform GetClosestPlayer()
        {
            float closestDistance = Mathf.Infinity;
            Transform closestPlayer = null;

            // Loop through all Photon players in the game
            foreach (Player photonPlayer in PhotonNetwork.PlayerList)
            {
                    // Get the PhotonView attached to the player
                    PhotonView photonView = photonPlayer.TagObject as PhotonView;
                    if (photonView != null)
                    {
                        GameObject playerObj = photonView.gameObject;  // Get the player GameObject

                        // Ensure the player object exists and isn't this enemy
                        if (playerObj != null && playerObj != gameObject)
                        {
                            PlayerControllerScript playerScript = playerObj.GetComponent<PlayerControllerScript>();
                            if (playerScript != null && !playerScript.isD())
                            {
                                float distance = Vector3.Distance(transform.position, playerObj.transform.position);

                                // Track the closest player
                                if (distance < closestDistance)
                                {
                                    closestDistance = distance;
                                    closestPlayer = playerObj.transform;
                                }
                            }
                        }
                }
            }
            return closestPlayer;
        }

        void Update()
        {
            timeSinceLastUpdate += Time.deltaTime;

            // Update closest player at a regular interval
            if (timeSinceLastUpdate >= updateInterval)
            {
                player = GetClosestPlayer();
                timeSinceLastUpdate = 0f;
            }
            if (player == null) return;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            PlayerControllerScript playerScript = player.GetComponent<PlayerControllerScript>();

            //Shooting
            if (distanceToPlayer <= shootingRadius && !playerScript.isD())
            {
                if (!isShooting)
                {
                    if (isDying) return;

                    Vector3 directionToPlayerFlat = new Vector3(player.position.x - transform.position.x, 0, player.position.z - transform.position.z);
                    if (directionToPlayerFlat != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayerFlat);
                        if (Quaternion.Angle(transform.rotation, targetRotation) > 5f)
                        {
                            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                            return; // Wait until rotation aligns before shooting
                        }
                    }

                    agent.isStopped = true;
                    animator.SetFloat("state", 2);
                    StartCoroutine(ShootPlayer());
                }
            }
            //Following
            else if (distanceToPlayer <= detectionRadius && !playerScript.isD())
            {
                // Chase the player
                agent.isStopped = false;
                agent.SetDestination(player.position);
                animator.SetFloat("state", 1);
            }
            //Random wander
            else
            {
                wanderTimer -= Time.deltaTime;

                if (wanderTimer <= 0f)
                {
                    WanderRandomly();
                    wanderTimer = wanderInterval; // Reset the timer
                }

                animator.SetFloat("state", 0);
            }
            if (photonViewEnemy.IsMine) // Only themaster will send the position update
            {
                photonViewEnemy.RPC("SyncPosition", RpcTarget.Others, transform.position);
            }
        }

        void WanderRandomly()
        {
            // Generate a random destination only for the owner of the enemy
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(navHit.position);
                photonViewEnemy.RPC("SyncWanderDestination", RpcTarget.Others, navHit.position); // Sync the wander destination across all clients
            }
        }

        // This is the RPC to sync the wander destination across all clients
        [PunRPC]
        void SyncWanderDestination(Vector3 destination)
        {
            if (!photonViewEnemy.IsMine)
            {
                agent.SetDestination(destination);
            }
        }

        [PunRPC]
        public void SyncPosition(Vector3 position)
        {
            // Sync the position on other clients
            transform.position = position;
        }


        IEnumerator ShootPlayer()
        {
            
            isShooting = true;

            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // Add randomness to the shooting direction
            float spreadAngle = 2f;
            directionToPlayer = ApplyRandomSpread(directionToPlayer, spreadAngle);

            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, shootingRadius))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    PlayerControllerScript playerScript = hit.collider.GetComponent<PlayerControllerScript>();
                    playerScript.TakeDamage((hit.collider.transform.position - transform.position).normalized);

                }
                else
                {
                    PlayMissSound();
                }
            }

            // Wait before allowing another shot
            yield return new WaitForSeconds(1f);

            isShooting = false;
        }

        //Function to add random spread
        Vector3 ApplyRandomSpread(Vector3 direction, float spreadAngle)
        {
            Quaternion spreadRotation = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0
            );

            return spreadRotation * direction;
        }

        //If enemy misses we play sound of bullet hiss
        void PlayMissSound()
        {
            AudioClip clip = missSound[(int)Random.Range(0, missSound.Length)];
            Vector3 randomPosition = player.position + (Random.onUnitSphere * 5f);
            AudioSource.PlayClipAtPoint(clip, randomPosition);
        }



        [PunRPC]
        public void TakeDamage(int damage, string playerId)
        {
            if (isDying) return;
            if (PhotonNetwork.IsMasterClient)
            {
                enemySoundScript.impactSound();
                health -= damage;

                if (health <= 0)
                {
                    photonViewEnemy.RPC("Die", RpcTarget.All);
                }
            }
        }

        [PunRPC]
        // Handle enemy death
        private void Die()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Vector3 spawnPosition = transform.position + Vector3.up * ammoBoxHeight;
                PhotonNetwork.Instantiate("Prefabs/Ammo", spawnPosition, Quaternion.identity, 0, null);

                GameplayScript gameplayScript = FindObjectOfType<GameplayScript>();
                if (gameplayScript != null)
                {
                    gameplayScript.enemyDeath();
                }
            }
            isDying = true;
            StartCoroutine(DeathAnimation());
            
        }

        private IEnumerator DeathAnimation()
        {
            enemySoundScript.EnemyDeath();
            animator.SetBool("dying", true);
            float deathAnimationLength = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            yield return new WaitForSeconds(deathAnimationLength);

            Destroy(gameObject);
        }
    }
}
