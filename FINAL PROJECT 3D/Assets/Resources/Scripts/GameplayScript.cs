using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class GameplayScript : MonoBehaviour
    {
        // Enemy logic
        public GameObject enemyPrefab;
        private int[] enemiesPerRound = { 3, 3, 3, 3 }; // Number of enemies per round
        public float spawnInterval = 2.5f; // Time between each spawn
        public static int currentRound = 0; // Current round number

        // Text
        public TMP_Text roundText;

        // Spawning
        private List<Transform> spawnPoints = new List<Transform>();
        private bool roundInProgress = true;

        private int enemiesToSpawn;
        private int enemiesKilledRound = 0;

        private PhotonView photonView;

        void Awake()
        {
            photonView = GetComponent<PhotonView>();
        }

        void Start()
        {
            UpdateRoundText();
            if (PhotonNetwork.IsMasterClient)
            {
                // Master client is responsible for spawning enemies
                GameObject spawnPointsParent = GameObject.FindGameObjectWithTag("Spawnpoints");
                foreach (Transform child in spawnPointsParent.transform)
                {
                    spawnPoints.Add(child);
                }
                StartNewRound();
            }
        }

        void StartNewRound()
        {
            if (currentRound >= enemiesPerRound.Length)
            {
                Debug.Log("All rounds completed!");
                return;
            }

            roundInProgress = true;

            currentRound++;
            enemiesToSpawn = enemiesPerRound[currentRound - 1];
            enemiesKilledRound = 0;

            // Update the round text and notify all clients (but only master client spawns)
            photonView.RPC("UpdateRoundTextRPC", RpcTarget.All, currentRound);

            Debug.LogWarning($"Starting Round {currentRound}, Enemies to spawn: {enemiesToSpawn}");

            // Master client handles enemy spawning
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(SpawnEnemiesWithDelay());
            }
        }

        IEnumerator SpawnEnemiesWithDelay()
        {
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnInterval);  // Delay between each spawn
            }
        }

        void SpawnEnemy()
        {
            // Randomly select a spawn point
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            Vector3 spawnPosition = randomSpawnPoint.position;

            // Instantiate the enemy at the selected spawn point
            PhotonNetwork.Instantiate(enemyPrefab.name, spawnPosition, Quaternion.identity);
        }

        public void enemyDeath()
        {
            photonView.RPC("EnemyKilledRPC", RpcTarget.All);
        }

        bool AllEnemiesDead()
        {
            return enemiesKilledRound == enemiesToSpawn;
        }

        [PunRPC]
        void UpdateRoundTextRPC(int round)
        {
            currentRound = round;
            UpdateRoundText();
        }

        [PunRPC]
        void EnemyKilledRPC()
        {
            enemiesKilledRound++;
            UpdateRoundText(); // Update the UI to show the number of enemies killed

            // Check if all enemies in the round are dead and notify the master client to handle round progression
            if (AllEnemiesDead() && PhotonNetwork.IsMasterClient)
            {
                Debug.LogWarning("All enemies are dead, starting new round...");
                photonView.RPC("StartNewRoundRPC", RpcTarget.All); // Notify all players to start the next round
            }
        }

        [PunRPC]
        void StartNewRoundRPC()
        {
            Debug.LogWarning("RPC: Starting new round on all clients.");
            // Call the round start logic on all clients (but only the master client should do the spawning)
            StartNewRound();
        }

        // Update the round display on the UI
        void UpdateRoundText()
        {
            if (roundText != null)
            {
                roundText.text = "Round: " + currentRound + "\n";
                roundText.text += "Killed:  " + enemiesKilledRound;
            }
        }
    }
}
