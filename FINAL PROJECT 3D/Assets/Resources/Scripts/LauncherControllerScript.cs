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
    public class LauncherControllerScript : MonoBehaviourPunCallbacks {
        private string text = "";
        bool connecting = false;
        void Start()
        {
            text += "Press Enter to connect!\n";
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Connect();
            }
        }
        private void Connect()
        {
            

            if (!PhotonNetwork.IsConnected)
            {
                connecting = true;
                text += "Connected to server\n";
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = "1";
            }
            else
            {
                text += "Reconnecting...\n";
                PhotonNetwork.JoinRandomRoom();
            }

            
        }
        public override void OnConnectedToMaster()
        {
            //Debug.Log("Connected to master. Joining a room...");
            if (connecting)
            {
                connecting = false;
                text += "Connected to master. Joining a room...\n";
                PhotonNetwork.JoinRandomRoom();
            }
            
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            //Debug.Log("No room joined! " + message);
            text += "No room joined! " + message +"\n";
            PhotonNetwork.CreateRoom("lobby");
            //Debug.Log("Welcome to Lobby");
            text += "Welcome to Lobby\n";
        }
        public override void OnJoinedRoom()
        {
            text += "Joined Room\n";
            PhotonNetwork.LoadLevel("ArenaScene");
        }
        private void OnGUI()
        {
            GUI.TextArea(new Rect(20, 20, 300, 100), text);
        }
    }
}