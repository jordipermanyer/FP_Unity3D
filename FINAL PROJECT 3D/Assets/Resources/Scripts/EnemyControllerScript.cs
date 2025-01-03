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
        private float wanderInterval = 3f; // Time interval between setting new wander destinations
        private float wanderTimer;

        private float health;

        void Start()
        {
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            wanderTimer = wanderInterval;

            if (PhotonNetwork.IsConnected)
            {
                //player = GetClosestPlayer(); //Per main escena
                player = GameObject.FindWithTag("Player").transform; //Per escene test
            }
            player = GameObject.FindWithTag("Player").transform;
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

            return closestPlayer;
        }




        void Update()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= shootingRadius)
            {
                // Stop moving and start shooting
                agent.isStopped = true;
                animator.SetFloat("state", 2);
                Debug.Log("Shooting");

                if (!isShooting)
                {
                    StartCoroutine(ShootPlayer());
                }
            }
            else if (distanceToPlayer <= detectionRadius)
            {
                // Chase the player
                agent.isStopped = false;
                agent.SetDestination(player.position);
                animator.SetFloat("state", 1);
            }
            else
            {
                // Random wandering when player is not nearby
                wanderTimer -= Time.deltaTime;

                if (wanderTimer <= 0f)
                {
                    WanderRandomly();
                    wanderTimer = wanderInterval; // Reset the timer
                }

                animator.SetFloat("state", 0);
            }
        }

        void WanderRandomly()
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(navHit.position);
            }
        }

        IEnumerator ShootPlayer()
        {
            isShooting = true;

            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // Add randomness to the shooting direction
            float spreadAngle = 5f;
            directionToPlayer = ApplyRandomSpread(directionToPlayer, spreadAngle);

            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, shootingRadius))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.Log("Player hit by raycast!");
                    // Apply damage logic here
                }
                else
                {
                    Debug.Log("Missed the shot!");
                }
            }

            // Wait for 3 seconds before allowing another shot
            yield return new WaitForSeconds(3f);

            isShooting = false;
        }

        //Unprecise shot
        Vector3 ApplyRandomSpread(Vector3 direction, float spreadAngle)
        {
            Quaternion spreadRotation = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0
            );

            return spreadRotation * direction;
        }




        public void TakeDamage(int damage)
        {

            health -= damage;
            if (health <= 0)
            {
                Die();
            }
        }

        // Handle player death
        private void Die()
        {
            GameplayScript gameplayScript = FindObjectOfType<GameplayScript>();
            if (gameplayScript != null)
            {
                gameplayScript.enemyDeath();
            }
            //Destroy();
        }
    }
}
