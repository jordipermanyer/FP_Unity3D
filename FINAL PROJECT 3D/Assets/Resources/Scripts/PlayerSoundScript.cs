using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class PlayerSoundScript : MonoBehaviour
    {
        public AudioClip[] audiosStep;
        public AudioClip[] audiosShot;

        public AudioSource Walkingsource;
        public AudioSource ShootingSource;

        private bool isWalking = true;

        public void StartWalking()
        {
            isWalking = true;
        }

        public void StopWalking()
        {
            isWalking = false;
        }

        public void PlayFootstepSound()
        {
            if (isWalking)
            {
                AudioClip clip = audiosStep[(int)Random.Range(0, audiosStep.Length)];
                Walkingsource.clip = clip;
                Walkingsource.volume = 0.8f;
                Walkingsource.Play();
            }
            else if (!isWalking)
            {
                Walkingsource.Stop();
            }
            
        }

        public void PlayerShotSound()
        {
            AudioClip clip = audiosShot[(int)Random.Range(0, audiosShot.Length)];
            ShootingSource.clip = clip;
            ShootingSource.volume = 1.2f;
            ShootingSource.Play();
        }

    }
}
