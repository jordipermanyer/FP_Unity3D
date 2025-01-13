using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class CameraCollisionScript : MonoBehaviour
    {
        public Transform player; // Reference to the player transform
        public LayerMask collisionLayers; // Layers the camera can collide with

        public float maxCameraDistance = 2f; // Max distance from the player
        public float collisionOffset = 0.2f; // Offset from the collision point

        private Vector3 cameraInitialPosition; // Store the initial position of the camera relative to the player

        void Start()
        {
            if (player != null)
            {
                // Store the camera's initial position relative to the player
                cameraInitialPosition = transform.position - player.position;
            }
        }

        void LateUpdate()
        {
            HandleCameraCollision();
        }

        private void HandleCameraCollision()
        {
            if (player == null) return;

            // Calculate the desired position based on the initial offset
            Vector3 playerPosition = player.position;
            Vector3 initialCameraPosition = playerPosition + cameraInitialPosition;

            // Calculate direction from the camera to the player
            Vector3 cameraDirection = (initialCameraPosition - playerPosition).normalized;
            float targetDistance = maxCameraDistance;

            // Check for collisions using a raycast
            if (Physics.Raycast(playerPosition, cameraDirection, out RaycastHit hit, maxCameraDistance, collisionLayers))
            {
                // Adjust distance based on the collision
                targetDistance = hit.distance - collisionOffset;
                targetDistance = Mathf.Max(0.5f, targetDistance); // Prevent the camera from getting too close

                // Calculate the new desired camera position after collision handling
                Vector3 desiredCameraPosition = playerPosition + cameraDirection * targetDistance;

                // Smoothly move the camera to the new position
                transform.position = Vector3.Lerp(transform.position, desiredCameraPosition, Time.deltaTime * 10f);
            }
            else
            {
                // If no collision, maintain the initial camera position relative to the player
                transform.position = Vector3.Lerp(transform.position, initialCameraPosition, Time.deltaTime * 10f);
            }
        }
    }

}
