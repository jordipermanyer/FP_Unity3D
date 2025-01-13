using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class EnemySoundScript : MonoBehaviour
    {
        public AudioClip[] audiosStep;
        public AudioClip[] audiosShot;

        public AudioSource Walkingsource;
        public AudioSource ShootingSource;

        public void EnemyFootstepSound()
        {
            AudioClip clip = audiosStep[(int)Random.Range(0, audiosStep.Length)];
            Walkingsource.clip = clip;
            Walkingsource.volume = 0.8f;
            Walkingsource.Play();
        }


        public void EnemyShotSound()
        {
            AudioClip clip = audiosShot[(int)Random.Range(0, audiosShot.Length)];
            ShootingSource.clip = clip;
            ShootingSource.volume = 1.2f;
            ShootingSource.Play();
        }


    }
}
