using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class TestLauncher : MonoBehaviourPunCallbacks
    {
        // UI elements
        public GameObject menuPanel;        // Main menu panel
        public Button joinRoomButton;      // Join Room button
        public Button quitButton;          // Quit button
        public Text statusText;            // Status text display

        private bool connecting = false;   // Connection state

        void Start()
        {
            // Set up button listeners
            joinRoomButton.onClick.AddListener(JoinRoom);
            quitButton.onClick.AddListener(QuitGame);

            menuPanel.SetActive(true);     // Show the menu initially
            statusText.text = "Welcome! Click 'Join' to enter the game.";
        }

        // Called when the Join Room button is clicked
        private void JoinRoom()
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                if (PhotonNetwork.InLobby)
                {
                    statusText.text = "Joining room...";
                    PhotonNetwork.JoinRoom("Lobby"); // Attempt to join the predefined room
                }
                else
                {
                    statusText.text = "Not in a lobby yet. Trying to join lobby...";
                    PhotonNetwork.JoinLobby(); // Ensure we're in the lobby
                }
            }
            else
            {
                statusText.text = "Not connected. Connecting...";
                Connect(); // Call the Connect function if not already connected
            }
        }

        // Connects to the Photon server
        private void Connect()
        {
            if (!PhotonNetwork.IsConnected)
            {
                connecting = true;
                statusText.text = "Connecting to server...";
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = "1";
            }
        }

        // Called when successfully connected to Photon master server
        public override void OnConnectedToMaster()
        {
            if (connecting)
            {
                connecting = false;
                statusText.text = "Connected! Joining lobby...";
                PhotonNetwork.JoinLobby(); // Move to the lobby first
            }
        }

        // Called when successfully joined a lobby
        public override void OnJoinedLobby()
        {
            statusText.text = "Joined lobby. Now joining the room...";
            PhotonNetwork.JoinRoom("Lobby"); // Join the room once the lobby is joined
        }

        // Called when the room join attempt fails
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            statusText.text = "Room not found. Creating a new room...";
            PhotonNetwork.CreateRoom("Lobby"); // Create the room if it doesn't exist
        }

        // Called when successfully joined a room
        public override void OnJoinedRoom()
        {
            menuPanel.SetActive(false); // Hide the menu panel
            PhotonNetwork.LoadLevel("ArenaScene");
        }

        // Quit the game when Quit button is clicked
        private void QuitGame()
        {
            statusText.text = "Exiting the game...";
            Application.Quit();
        }
    }
}
