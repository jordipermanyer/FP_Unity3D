using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class CameraCollisionScript : MonoBehaviour
    {
        [SerializeField] private LayerMask collisionMask; // Layers the camera can collide with
        [SerializeField] private float collisionRadius = 0.5f; // Radius for sphere cast
        [SerializeField] private float smoothingSpeed = 10f; // Speed of position smoothing
        [SerializeField] private float verticalOffset = 0.5f; // Keeps the camera above the floor
        [SerializeField] private float minVerticalPosition = 1f; // Minimum height for the camera
        [SerializeField] private float bufferDistance = 0.1f; // Prevents minor clipping

        private Vector3 originalLocalPosition; // Camera's original local position relative to the player
        private Transform playerTransform; // Reference to the player's transform

        void Start()
        {
            // Cache the original local position of the camera
            originalLocalPosition = transform.localPosition;

            // Cache the parent transform (assumes the camera is a child of the player)
            playerTransform = transform.parent;
        }

        void LateUpdate()
        {
            HandleCameraCollision();
        }

        private void HandleCameraCollision()
        {
            // Determine the desired world position of the camera
            Vector3 desiredWorldPosition = playerTransform.TransformPoint(originalLocalPosition);

            // Add the vertical offset to keep the camera above the ground
            desiredWorldPosition.y = Mathf.Max(desiredWorldPosition.y + verticalOffset, minVerticalPosition);

            // Perform a sphere cast from the player to the desired camera position
            if (Physics.SphereCast(
                    playerTransform.position + Vector3.up * verticalOffset, // Start point above the ground
                    collisionRadius,
                    (desiredWorldPosition - playerTransform.position).normalized,
                    out RaycastHit hit,
                    Vector3.Distance(playerTransform.position, desiredWorldPosition),
                    collisionMask))
            {
                // If a collision is detected, adjust the camera's position
                Vector3 hitPointLocal = playerTransform.InverseTransformPoint(hit.point);

                // Pull the camera slightly forward (bufferDistance) to avoid clipping
                Vector3 directionToPlayer = (playerTransform.position - hit.point).normalized;
                hitPointLocal += directionToPlayer * bufferDistance;

                // Ensure the vertical position doesn't drop below the minimum
                hitPointLocal.y = Mathf.Max(hitPointLocal.y, minVerticalPosition);

                // Smoothly move the camera to the adjusted position
                transform.localPosition = Vector3.Lerp(transform.localPosition, hitPointLocal, smoothingSpeed * Time.deltaTime);
            }
            else
            {
                // No collision, smoothly return to the original local position
                transform.localPosition = Vector3.Lerp(transform.localPosition, originalLocalPosition, smoothingSpeed * Time.deltaTime);
            }
        }
    }
}
