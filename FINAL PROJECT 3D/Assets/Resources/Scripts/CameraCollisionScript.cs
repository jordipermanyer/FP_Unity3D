using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class CameraCollisionScript : MonoBehaviour
    {
        private Camera playerCamera;
        private Transform playerTransform;

        public float cameraDistance = 2f; // Camera distance from the player
        public float smoothing = 10f; // Smoothing of the camera movement

        // Called when the object is instantiated
        void Start()
        {
            // Ensure that the camera is attached to the player prefab
            playerCamera = GetComponentInChildren<Camera>();
            playerTransform = transform; // The player transform is the parent of the camera
        }

        void Update()
        {
            if (playerCamera != null)
            {
                HandleCameraCollision();
            }
        }

        // Adjusts the camera position if it collides with something
        private void HandleCameraCollision()
        {
            RaycastHit hit;
            Vector3 targetPosition = playerTransform.position - playerTransform.forward * cameraDistance;

            if (Physics.Raycast(playerTransform.position, -playerTransform.forward, out hit, cameraDistance))
            {
                targetPosition = hit.point; // Set the camera position to the collision point
            }

            // Smooth the camera movement
            playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, targetPosition, Time.deltaTime * smoothing);
        }
    }

}
