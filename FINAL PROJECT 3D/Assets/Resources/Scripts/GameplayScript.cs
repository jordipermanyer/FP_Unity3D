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
        public int[] enemiesPerRound = { 10, 15, 20, 40 };
        public float spawnInterval = 2.5f; // Time between each spawn
        private int currentRound = 0;

        // Text
        public TMP_Text roundText;

        // Spawning
        private List<Transform> spawnPoints = new List<Transform>();
        private List<GameObject> spawnedEnemies = new List<GameObject>();

        private int enemiesToSpawn;
        private Terrain terrain;

        void Start()
        {
            terrain = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Terrain>();
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
            spawnedEnemies.Clear();  // Clear the list of spawned enemies for the new round
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
            bool spawnInsideBuilding = Random.value > 0.5f; // 50% chance to spawn inside the building
            Vector3 spawnPosition;

            if (spawnInsideBuilding)
            {
                spawnPosition = GetRandomPositionInsideBuilding();
            }
            else
            {
                spawnPosition = GetRandomPositionOnTerrain();
            }

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            spawnedEnemies.Add(enemy);  // Add the enemy to the list of spawned enemies
        }

        Vector3 GetRandomPositionOnTerrain()
        {
            // Generate a random position on the terrain
            float xPos = Random.Range(0, terrain.terrainData.size.x);
            float zPos = Random.Range(0, terrain.terrainData.size.z);
            float yPos = terrain.SampleHeight(new Vector3(xPos, 0, zPos));
            return new Vector3(xPos, yPos, zPos);
        }

        Vector3 GetRandomPositionInsideBuilding()
        {
            // Find all objects with the "Floor" tag inside the building
            GameObject[] floorElements = GameObject.FindGameObjectsWithTag("Floor");

            // Randomly select a floor element
            GameObject randomFloor = floorElements[Random.Range(0, floorElements.Length)];
            Collider floorCollider = randomFloor.GetComponent<Collider>();

            if (floorCollider == null)
            {
                Debug.LogWarning("Selected floor element does not have a collider.");
                return Vector3.zero;
            }

            Vector3 randomPosition = new Vector3(
                Random.Range(floorCollider.bounds.min.x, floorCollider.bounds.max.x),
                floorCollider.bounds.center.y, // Y is the center height of the floor element
                Random.Range(floorCollider.bounds.min.z, floorCollider.bounds.max.z)
            );

            return randomPosition;
        }

        void Update()
        {
            // Check if all enemies are dead
            if (AllEnemiesDead())
            {
                // Wait a few seconds before starting the next round
                if (spawnedEnemies.Count == enemiesToSpawn)
                {
                    Invoke("StartNewRound", 2f); // Delay the start of the next round
                }
            }
        }

        // Check if all enemies in the round are dead
        bool AllEnemiesDead()
        {
            // Iterate through the list of spawned enemies and check if they're alive
            foreach (GameObject enemy in spawnedEnemies)
            {
                if (enemy != null)  // If the enemy is still alive
                {
                    return false;  // Not all enemies are dead yet
                }
            }
            return true;  // All enemies are dead
        }

        public void enemyDeath()
        {
            
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
