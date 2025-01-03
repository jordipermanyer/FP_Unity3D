using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

//namespace es superior a classe
namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    //s'ha modificat el monobehaviour per treballar amb photon
    public class GameManagerControllerScript : MonoBehaviourPunCallbacks
    {
        
        public GameObject playerPrefab;
        public GameObject spawnpoint;
        string text = "";
        bool connecting = false;
        public int alive = 0;

        void Start()
        {
            Vector3 pos = spawnpoint.transform.position;
            pos += new Vector3(
                UnityEngine.Random.Range(-1.0f, 1.0f),
                0f,                                    
                UnityEngine.Random.Range(-1.0f, 1.0f)  
            );
            pos.y = 0.5f;
            PhotonNetwork.Instantiate(playerPrefab.name, pos, Quaternion.identity);

            alive = PhotonNetwork.PlayerList.Length;
            text += "You have joined the Arena\n";
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {

                PhotonNetwork.LeaveRoom();
                Application.Quit();
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            text += "Player" + newPlayer.ActorNumber + " has joined the room.\n";
            alive++;
        }
        public override void OnPlayerLeftRoom(Player newPlayer)
        {
            text += "Player" + newPlayer.ActorNumber + " has left the room.\n";
            alive--;
            CheckGameOver();
        }

        public void OnPlayerDeath()
        {
            alive--;
            CheckGameOver();
        }

        private void CheckGameOver()
        {
            Debug.Log("UN MENYS");
            if (alive <= 0)
            {
                // Anar a escne game over
                //PhotonNetwork.LoadLevel("GameOver");
                Debug.Log("TOOOOOTS MORTS");
            }
        }
        public override void OnLeftRoom()
        {
            text += "Leaving the Arena...\n";
            PhotonNetwork.LoadLevel("Launcher");
        }

        private void OnGUI()
        {
            GUI.TextArea(new Rect(20, 20, 300, 100), text);
        }
    }
}