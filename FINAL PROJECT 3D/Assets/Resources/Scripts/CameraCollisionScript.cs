using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class CameraCollisionScript : MonoBehaviour
    {
        public Transform cameraTransform; // Reference to the camera's transform
        public Transform playerTransform; // Reference to the player's transform
        public LayerMask wallLayer; // Layer for walls
        public float collisionRadius = 0.5f; // Radius for detecting obstacles
        public float smoothingSpeed = 10f; // Speed of the camera adjustment

        private Vector3 originalOffset; // The initial offset of the camera relative to the player
        private Vector3 targetPosition; // Target position for the camera

        void Start()
        {
            if (cameraTransform == null || playerTransform == null)
            {
                Debug.LogError("CameraCollisionHandler: Missing references to the camera or player!");
                enabled = false;
                return;
            }

            // Calculate the initial offset between the player and the camera
            originalOffset = cameraTransform.position - playerTransform.position;
            targetPosition = cameraTransform.position;
        }

        void LateUpdate()
        {
            HandleCameraCollision();
        }

        private void HandleCameraCollision()
        {
            // Calculate the desired camera position based on the player's position and the original offset
            Vector3 desiredPosition = playerTransform.position + originalOffset;

            // Check for obstacles between the player and the desired camera position
            if (Physics.SphereCast(playerTransform.position, collisionRadius, (desiredPosition - playerTransform.position).normalized, out RaycastHit hit, originalOffset.magnitude, wallLayer))
            {
                // If an obstacle is detected, adjust the target position to the hit point
                targetPosition = hit.point - (desiredPosition - playerTransform.position).normalized * collisionRadius;
            }
            else
            {
                // If no obstacles, reset the target position to the desired position
                targetPosition = desiredPosition;
            }

            // Smoothly move the camera to the target position
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, Time.deltaTime * smoothingSpeed);

            // Ensure the camera always looks at the player
            cameraTransform.LookAt(playerTransform.position + Vector3.up * 1.5f); // Adjust vertical offset if needed
        }

        void OnDrawGizmos()
        {
            // Visualize the collision radius and ray in the scene view
            if (playerTransform != null && cameraTransform != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(cameraTransform.position, collisionRadius);
                Gizmos.DrawLine(playerTransform.position, cameraTransform.position);
            }
        }
    }

}
