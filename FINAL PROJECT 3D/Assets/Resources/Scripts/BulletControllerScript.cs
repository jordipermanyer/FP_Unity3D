using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class BulletControllerScript : MonoBehaviour
    {
        public float lifetime = 5f; // Bullet lifetime in seconds
        public GameObject explosionVfx; // Explosion VFX prefab

        private float timer;

        private void Start()
        {
            timer = 0f;
        }
        private void Update()
        {
            timer += Time.deltaTime;

            if (timer >= lifetime)
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Enemy"))
            {
                if (explosionVfx != null)
                {
                    Instantiate(explosionVfx, transform.position, transform.rotation);
                }
                Destroy(gameObject);
            }
        }
    }
}