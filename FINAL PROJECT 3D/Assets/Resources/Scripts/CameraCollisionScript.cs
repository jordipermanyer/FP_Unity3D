using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class CameraCollisionScript : MonoBehaviour
    {
        private Transform cameraTransform;
        private Transform playerTransform;

        [SerializeField] private string wallTag = "Wall"; // Tag to identify walls
        [SerializeField] private float maxDistance = 5f; // Maximum allowed distance from the player
        [SerializeField] private float minDistance = 1f; // Minimum allowed distance from the player
        private float currentDistance;

        void Start()
        {
            // Find the player dynamically based on the tag
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("No object with the 'Player' tag found in the scene!");
                return;
            }

            playerTransform = player.transform;
            cameraTransform = player.transform.Find("Camera");

            if (cameraTransform == null)
            {
                Debug.LogError("Player does not have a child object named 'Camera'!");
                return;
            }

            // Set initial camera distance
            currentDistance = maxDistance;
        }

        void LateUpdate()
        {
            if (playerTransform == null || cameraTransform == null)
                return;

            // Calculate direction from player to camera
            Vector3 cameraDirection = (cameraTransform.position - playerTransform.position).normalized;

            // Raycast to detect walls between the player and the camera
            if (Physics.Raycast(playerTransform.position, cameraDirection, out RaycastHit hit, maxDistance))
            {
                if (hit.collider.CompareTag(wallTag))
                {
                    // Adjust camera distance based on wall position
                    currentDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
                }
            }
            else
            {
                // Reset to max distance if no wall is detected
                currentDistance = maxDistance;
            }

            // Update camera position
            cameraTransform.position = playerTransform.position + cameraDirection * currentDistance;
        }
    }

}
