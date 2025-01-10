using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class FinalScreenScript : MonoBehaviour
    {

        public Button reJoin;
        public Button quitButton;
        public TMP_Text killsText;

        void Start()
        {
            reJoin.onClick.AddListener(ReJoinRoom);
            quitButton.onClick.AddListener(QuitGame);

            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("killCount"))
            {
                int killCount = (int)PhotonNetwork.LocalPlayer.CustomProperties["killCount"];
                killsText.text = "Score: " + killCount;
            }
        }



        private void ReJoinRoom()
        {
            SceneManager.LoadScene("Launcher");
        }



        private void QuitGame()
        {
            UnityEngine.Application.Quit();
        }

    }
}
