using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class UIManagerScript : MonoBehaviour
    {

        public TMP_Text healthText;
        public TMP_Text bulletsText;
        public TMP_Text enemiesText;
        public TMP_Text roundText;

        private int currentHealth;
        private int currentBullets;
        private int enemiesAlive;
        private int currentRound;

        public TMP_Text ammoPopupText;
        public float displayDuration = 1f;
        public float fadeDuration = 0.5f;

        /*UPDATING UI*/
        public void UpdateHealth(int health)
        {
            currentHealth = health;
            healthText.text = currentHealth + "/100";
        }

        // Call this method to update bullets
        public void UpdateBullets(int bullets)
        {
            currentBullets = bullets;
            bulletsText.text = currentBullets + "/50";
        }

        // Call this method to update enemies alive
        public void UpdateEnemies(int enemies)
        {
            enemiesAlive = enemies;
            enemiesText.text = "Enemies Alive: " + enemiesAlive;
        }

        // Call this method to update the round
        public void UpdateRound(int round)
        {
            currentRound = round;
            roundText.text = "Round: " + currentRound;
        }



        /*BULLET POP UP*/

        public void ShowPopup()
        {
            // Set the popup text to display the ammo amount
            ammoPopupText.text = "+5";

            // Start the popup animation
            StartCoroutine(PopupAnimation());
        }

        private IEnumerator PopupAnimation()
        {
            // The text is invisible at the start
            Color textColor = ammoPopupText.color;
            textColor.a = 0;
            ammoPopupText.color = textColor;

            // Show the text (fade in)
            ammoPopupText.gameObject.SetActive(true);
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                textColor.a = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
                ammoPopupText.color = textColor;
                yield return null;
            }

            // Wait for the popup to stay visible for the desired time
            yield return new WaitForSeconds(displayDuration);

            // Fade out the text
            elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                textColor.a = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
                ammoPopupText.color = textColor;
                yield return null;
            }

            // Hide the text once the fade-out is complete
            ammoPopupText.gameObject.SetActive(false);
        }
    }
}
