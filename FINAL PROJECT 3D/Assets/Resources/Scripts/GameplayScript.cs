using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class GameplayScript : MonoBehaviourPunCallbacks
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

        private int enemiesToSpawn;
        private int enemiesKilledRound = 0;

        private UIManagerScript gameUIManager;


        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;


            if (PhotonNetwork.IsMasterClient)
            {
                resetScene();
            }

                gameUIManager = FindObjectOfType<UIManagerScript>();
            gameUIManager.UpdateEnemies(enemiesToSpawn - enemiesKilledRound);
            gameUIManager.UpdateRound(currentRound);

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

            currentRound++;
            enemiesToSpawn = enemiesPerRound[currentRound - 1];
            enemiesKilledRound = 0;

            photonView.RPC("SyncGameInfoRPC", RpcTarget.All, currentRound, enemiesToSpawn, enemiesKilledRound); //Sync all players with usefull info

            StartCoroutine(SpawnEnemiesWithDelay());
        }

        IEnumerator SpawnEnemiesWithDelay()
        {
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                SpawnEnemy();
                photonView.RPC("SyncGameInfoRPC", RpcTarget.All, currentRound, enemiesToSpawn, enemiesKilledRound);
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
            if (PhotonNetwork.IsMasterClient)
            {
                enemiesKilledRound++;
                photonView.RPC("SyncGameInfoRPC", RpcTarget.All, currentRound, enemiesToSpawn, enemiesKilledRound); //Sync all players with usefull info

                if (AllEnemiesDead())
                {
                    StartNewRound();
                }
            }
        }


        [PunRPC]
        void SyncGameInfoRPC(int round, int toSpawn, int killed)
        {
            currentRound = round;
            enemiesToSpawn = toSpawn;
            enemiesKilledRound = killed;

            UpdateRoundText();
        }

        bool AllEnemiesDead()
        {
            return enemiesKilledRound == enemiesToSpawn;
        }


        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // Synchronize game state with the new player
                photonView.RPC("SyncGameInfoRPC", newPlayer, currentRound, enemiesToSpawn, enemiesKilledRound);
            }
        }



        // Update the round display on the UI
        void UpdateRoundText()
        {
            gameUIManager.UpdateEnemies(enemiesToSpawn - enemiesKilledRound);
            gameUIManager.UpdateRound(currentRound);
        }

        public void resetScene()
        {
            currentRound = 0;
            enemiesKilledRound = 0;
        }

    }
}
