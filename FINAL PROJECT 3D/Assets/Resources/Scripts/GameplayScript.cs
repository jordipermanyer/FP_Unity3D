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
        public float spawnInterval = 2.5f; // Time between each spawn
        public static int currentRound = 0; // Current round number

        // Text
        public TMP_Text roundText;

        // Spawning
        private List<Transform> spawnPoints = new List<Transform>();

        private int enemiesToSpawn = 0;
        private int enemiesKilledRound = 0;

        private UIManagerScript gameUIManager;
        private PlayerSoundScript gamePlayerSound;


        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;


            if (PhotonNetwork.IsMasterClient)
            {
                resetScene();
            }

            gameUIManager = FindObjectOfType<UIManagerScript>();
            gamePlayerSound = FindObjectOfType<PlayerSoundScript>();
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
            currentRound++;
            enemiesToSpawn +=  3;
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

        //We spawn an enemy with a delay from the previous function
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

                if (enemiesKilledRound == enemiesToSpawn) //All enemies of the round dead, means new round
                {
                    gamePlayerSound.roundVictorySound();
                    StartNewRound();
                }
            }
        }

        //Sync all the data across users
        [PunRPC]
        void SyncGameInfoRPC(int round, int toSpawn, int killed)
        {
            currentRound = round;
            enemiesToSpawn = toSpawn;
            enemiesKilledRound = killed;

            UpdateRoundText();
        }

        //In cas of a later join in the game update the score ui immediatley
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
            enemiesToSpawn = 0;
            enemiesKilledRound = 0;
        }

    }
}
