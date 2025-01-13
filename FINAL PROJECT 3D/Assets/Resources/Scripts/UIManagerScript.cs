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
    }
}
