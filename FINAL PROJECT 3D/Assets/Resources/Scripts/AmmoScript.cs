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
                    PhotonView view = PhotonView.Get(this);
                    view.RPC("RequestDestroy", RpcTarget.MasterClient, view.ViewID);
                }
            }
        }

        [PunRPC]
        private void RequestDestroy(int viewID)
        {
            // MasterClient destroys the object
            PhotonView targetView = PhotonView.Find(viewID);
            if (targetView != null && PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(targetView.gameObject);
            }
        }
    }
}
