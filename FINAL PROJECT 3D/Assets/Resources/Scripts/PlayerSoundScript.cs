using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class PlayerSoundScript : MonoBehaviour
    {
        public AudioClip[] audiosStep;
        public AudioClip[] audiosShot;
        public AudioClip[] audiosImpact;
        public AudioClip pickObject;
        public AudioClip noAmmo;
        public AudioClip death;
        public AudioClip jump;
        public AudioClip roundVictory;

        public AudioSource Walkingsource;
        public AudioSource PlayerSourceSound;

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
                Walkingsource.volume = 0.5f;
                Walkingsource.Play();
            }
            else if (!isWalking)
            {
                //Sometimes walking sounds got bugged, we use a variable to make sure the player has stopped (weird blendtree things)
                Walkingsource.Stop();
            }
        }

        //Sounds
        public void PlayerShotSound()
        {
            AudioClip clip = audiosShot[(int)Random.Range(0, audiosShot.Length)];
            PlayerSourceSound.clip = clip;
            PlayerSourceSound.volume = 1.2f;
            PlayerSourceSound.Play();
        }

        public void pickObj()
        {
            AudioClip clip = pickObject;
            PlayerSourceSound.PlayOneShot(clip, 1.5f);
        }
        
        public void PlayerNoShotSound()
        {
            AudioClip clip = noAmmo;
            PlayerSourceSound.PlayOneShot(clip, 1.5f);
        }

        public void deathSound()
        {
            AudioClip clip = death;
            PlayerSourceSound.PlayOneShot(clip, 1f);
        }

        public void impactSound()
        {
            AudioClip clip = audiosImpact[(int)Random.Range(0, audiosImpact.Length)];
            PlayerSourceSound.clip = clip;
            PlayerSourceSound.volume = 1f;
            PlayerSourceSound.Play();
        }

        public void jumpSound()
        {
            AudioClip clip = jump;
            PlayerSourceSound.PlayOneShot(clip, 1f);
        }

        public void roundVictorySound()
        {
            AudioClip clip = roundVictory;
            PlayerSourceSound.PlayOneShot(clip, 1f);
        }
    }
}
