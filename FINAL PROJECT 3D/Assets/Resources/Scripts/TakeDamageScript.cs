using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class TakeDamageScript : MonoBehaviour
    {
        private Vector3 originalCameraPosition;
        private Camera playerCamera;
        public float shakeMagnitude = 0.01f;
        public float shakeDuration = 0.5f;

        void Start()
        {
            playerCamera = GetComponentInChildren<Camera>();
            originalCameraPosition = playerCamera.transform.localPosition;
        }

        // Function to change the color of the player's components
        public void damageEffect(Transform playerTransform, float duration)
        {
            StartCoroutine(ChangeColorCoroutine(playerTransform, duration));
            StartCoroutine(CameraShake());
        }

        private IEnumerator ChangeColorCoroutine(Transform playerTransform, float duration)
        {
            // Find the specific components you want to change color on
            Renderer armsRenderer = playerTransform.Find("Arm1").GetComponent<Renderer>();
            Renderer backRenderer = playerTransform.Find("Backpack1").GetComponent<Renderer>();
            Renderer bodyRenderer = playerTransform.Find("Body1").GetComponent<Renderer>();
            Renderer legRenderer = playerTransform.Find("Leg1").GetComponent<Renderer>();
            Renderer headRenderer = playerTransform.Find("head1").GetComponent<Renderer>();

            // Store the original colors to revert later
            Color originalArmsColor = armsRenderer.material.color;
            Color originalBackColor = backRenderer.material.color;
            Color originalBodyColor = bodyRenderer.material.color;
            Color originalLegColor = legRenderer.material.color;
            Color originalHeadsColor = headRenderer.material.color;

            // Temporarily change their colors to red
            armsRenderer.material.color = Color.red; 
            backRenderer.material.color = Color.red;
            bodyRenderer.material.color = Color.red;
            legRenderer.material.color = Color.red;
            headRenderer.material.color = Color.red;

            // Wait for the specified duration
            yield return new WaitForSeconds(duration);

            // Reset the colors back to the original
            armsRenderer.material.color = originalArmsColor;
            backRenderer.material.color = originalBackColor;
            bodyRenderer.material.color = originalBodyColor;
            legRenderer.material.color = originalLegColor;
            headRenderer.material.color = originalHeadsColor;
        }

        private IEnumerator CameraShake()
        {
            float elapsedTime = 0.0f;

            // Create an offset by getting the current camera position in the beginning
            Vector3 shakeOffset = playerCamera.transform.localPosition - originalCameraPosition;

            while (elapsedTime < shakeDuration)
            {
                float x = Random.Range(-shakeMagnitude, shakeMagnitude);
                float y = Random.Range(-shakeMagnitude, shakeMagnitude);

                // Apply shake using the offset
                playerCamera.transform.localPosition = originalCameraPosition + shakeOffset + new Vector3(x, y, 0f);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Reset the camera's position back to the original, including the offset
            playerCamera.transform.localPosition = originalCameraPosition + shakeOffset;
        }
    }
}
