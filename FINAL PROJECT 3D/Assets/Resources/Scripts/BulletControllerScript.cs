using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class BulletControllerScript : MonoBehaviour
    {
        public float lifetime = 10f;
        public GameObject explosionVfx;
        public AudioClip[] impactSounds;

        private float timer = 0f;

        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= lifetime)
            {
                DestroyBullet(); 
            }
        }


        void OnTriggerEnter(Collider other)
        {
            HandleImpact();
            DestroyBullet();
        }

        private void HandleImpact()
        {
            
            AudioClip randomSound = impactSounds[Random.Range(0, impactSounds.Length)];
            AudioSource.PlayClipAtPoint(randomSound, transform.position);
            
        }

        private void DestroyBullet()
        {
            if (explosionVfx != null)
            {
                GameObject explosion = Instantiate(explosionVfx, transform.position, transform.rotation);
                Destroy(explosion, 1f);
            }

            Destroy(gameObject);
        }
    }
}
