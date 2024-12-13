using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class PlayerControllerScript : MonoBehaviourPun,IPunObservable
    {
        public float speed = 5.0f; // Speed of the player movement
        public string team = "NoTeam"; // Team
        public string text = ""; // Text for the player info

        void Start()
        {
            if (!photonView.IsMine)
            {
                transform.GetChild(0).gameObject.SetActive(false); //disable camera
            }
            else
            {
                //defining the team of the local player
                if (Random.Range(0f, 1f) < 0.5f) // 50% chance for each team
                {
                    team += "Team Alpha";
                    text += "I'm on team Alpha";
                }
                else
                {
                    team += "Team Beta";
                    text += "I'm on team Beta";
                }

            }
        }
        // Update is called once per frame
        void Update()
        {
            if (photonView.IsMine) //only moves if the player is yourself
            {
                float horizontal = Input.GetAxis("Horizontal");
                float vertical = Input.GetAxis("Vertical");

                Vector3 movement = new Vector3(horizontal, 0, vertical);
                transform.Translate(movement * speed * Time.deltaTime, Space.World);

                // Rotación con el mouse (eje X)
                float mouseX = Input.GetAxis("Mouse X");
                if (Mathf.Abs(mouseX) > 0.01f)
                {
                    transform.Rotate(Vector3.up, mouseX * speed * Time.deltaTime);
                }
            }
        }
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(team);
            }
            else
            {
                team = (string)stream.ReceiveNext();
            }
        }
        private void OnGUI()
        {
            GUI.TextArea(new Rect(20, 120, 300, 100), text);
        }
    }
}

