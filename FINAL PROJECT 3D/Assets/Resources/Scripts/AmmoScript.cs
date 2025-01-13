using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class AmmoScript : MonoBehaviour
    {
        public int bulletsToGive = 5;

        private void OnTriggerEnter(Collider other)
        {
            // Check if the player is the one that collided with the ammo box
            if (other.CompareTag("Player"))
            {
                PlayerControllerScript playerScript = other.GetComponent<PlayerControllerScript>();

                if (playerScript != null)
                {
                    // Give the player bullets
                    playerScript.AddBullets(bulletsToGive);

                    // Destroy the ammo box after the player picks it up
                    PhotonNetwork.Destroy(gameObject); // Destroy the ammo box across all clients
                }
                else
                {
                    Debug.LogWarningFormat("No script found, WTF?");
                }
            }
        }
    }
}
