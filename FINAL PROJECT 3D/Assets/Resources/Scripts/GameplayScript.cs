using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class GameplayScript : MonoBehaviour
    {
        // Enemy logic
        public GameObject enemyPrefab;
        public int[] enemiesPerRound = { 3, 15, 20, 40 };
        public float spawnInterval = 2.5f; // Time between each spawn
        public static int currentRound = 0;

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
            if (PhotonNetwork.IsMasterClient)
            {
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

            currentRound++;
            enemiesToSpawn = enemiesPerRound[currentRound - 1];
            enemiesKilledRound = 0;

            photonView.RPC("UpdateRoundTextRPC", RpcTarget.All, currentRound);

            // Start spawning enemies with a delay
            //StartCoroutine(SpawnEnemiesWithDelay());
            photonView.RPC("StartSpawningEnemies", RpcTarget.All);
        }

        [PunRPC]
        void StartSpawningEnemies()
        {
            StartCoroutine(SpawnEnemiesWithDelay());
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


        void Update()
        {
            
        }

        // Check if all enemies in the round are dead
        

        public void enemyDeath()
        {
            enemiesKilledRound++;

            if (AllEnemiesDead() && roundInProgress)
            {
                roundInProgress = false;
                Invoke("StartNewRound", 5f); // Delay the start of the next round
            }
        }
        bool AllEnemiesDead()
        {
            if (enemiesKilledRound == enemiesToSpawn)
            {
                return true;
            }
            return false;
        }

        [PunRPC]
        void UpdateRoundTextRPC(int round)
        {
            currentRound = round;
            UpdateRoundText();
        }


        // Update the round display on the UI
        void UpdateRoundText()
        {
            if (roundText != null)
            {
                //roundText.text = "Round: " + currentRound;
                roundText.text = "Round: " + enemiesKilledRound;
            }
        }
    }
}
