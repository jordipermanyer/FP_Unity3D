using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class TakeDamageScript : MonoBehaviour
    {
 

        // Function to change the color of the player's components
        public void damageEffect(Transform playerTransform, float duration)
        {
            StartCoroutine(ChangeColorCoroutine(playerTransform, duration));
        }

        private IEnumerator ChangeColorCoroutine(Transform playerTransform, float duration)
        {
            // Specific components to change color on
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
    }
}
