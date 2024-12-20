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
        //variables in game manager
        public GameObject playerPrefab;
        string text = "";
        bool connecting = false;
        // Start is called before the first frame update
        void Start()
        {
            Vector3 pos = new Vector3 (UnityEngine.Random.Range(-5,5), 0.5f, UnityEngine.Random.Range(-5, 5));
            PhotonNetwork.Instantiate(playerPrefab.name,pos,Quaternion.identity);
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
        }
        public override void OnPlayerLeftRoom(Player newPlayer)
        {
            text += "Player" + newPlayer.ActorNumber + " has left the room.\n";
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