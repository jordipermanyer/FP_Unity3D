using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;

//namespace es superior a classe
namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    //s'ha modificat el monobehaviour per treballar amb photon
    public class GameManagerControllerScript : MonoBehaviourPunCallbacks
    {
        
        public GameObject playerPrefab;
        public GameObject spawnpoint;

        void Start()
        {
            Vector3 pos = spawnpoint.transform.position;
            pos += new Vector3(
                UnityEngine.Random.Range(-1.0f, 1.0f),
                0f,                                    
                UnityEngine.Random.Range(-1.0f, 1.0f)  
            );
            pos.y = 2f;
            GameObject playerObject = PhotonNetwork.Instantiate(playerPrefab.name, pos, Quaternion.identity);

            // Get the PhotonView attached to the instantiated player
            PhotonView photonView = playerObject.GetComponent<PhotonView>();
            // Assign the PhotonView to the player's TagObject
            if (photonView != null)
            {
                PhotonNetwork.LocalPlayer.TagObject = photonView; // Store the PhotonView in TagObject
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {

                PhotonNetwork.LeaveRoom();
                Application.Quit();
            }
            CheckIfAllPlayersDead();
        }


        public override void OnLeftRoom()
        {
            PhotonNetwork.LoadLevel("Launcher");
        }



        /* MORT */
        public void CheckIfAllPlayersDead()
        {

            bool allDead = PhotonNetwork.PlayerList
            .All(player => player.CustomProperties.ContainsKey("IsAlive") &&
                           !(bool)player.CustomProperties["IsAlive"]);

            if (allDead)
            {
                GameOver();
            }

        }

        private void GameOver()
        {
            PhotonNetwork.Disconnect();

            PhotonNetwork.LoadLevel("FinalScreen");

        }



    }
}