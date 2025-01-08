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
        private int currentRound = 0;

        // Text
        public TMP_Text roundText;

        // Spawning
        private List<Transform> spawnPoints = new List<Transform>();
        private bool roundInProgress = true;

        private int enemiesToSpawn;
        private int enemiesKilledRound = 0;

        void Start()
        {
            GameObject spawnPointsParent = GameObject.FindGameObjectWithTag("Spawnpoints");

            foreach (Transform child in spawnPointsParent.transform)
            {
                spawnPoints.Add(child);
            }

            StartNewRound();
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
            UpdateRoundText();

            // Start spawning enemies with a delay
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
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
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


        // Update the round display on the UI
        void UpdateRoundText()
        {
            if (roundText != null)
            {
                roundText.text = "Round: " + currentRound;
            }
        }
    }
}
