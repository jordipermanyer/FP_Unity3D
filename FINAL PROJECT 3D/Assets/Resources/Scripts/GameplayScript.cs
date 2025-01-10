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

        // Update the round display on the UI
        void UpdateRoundText()
        {
           
            roundText.text = "Round: " + currentRound + "\n";

            if (roundText != null)
            {
                // Iterate through all players to get their kill counts
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    if (player.IsLocal)
                    {
                        PhotonView photonView = player.TagObject as PhotonView;
                        if (photonView != null)
                        {
                            GameObject playerObject = photonView.gameObject;  // Get the player GameObject
                            if (playerObject != null)
                            {
                                PlayerControllerScript playerController = playerObject.GetComponent<PlayerControllerScript>();
                                roundText.text += "Kills" + playerController.GetKills();
                            }
                        }
                        else
                        {
                            Debug.LogWarningFormat("Aqui1 - PhotonView not found for player: " + player.NickName);
                        }
                    }

                }

                
            }
        }

    }
}
