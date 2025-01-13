using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class BulletControllerScript : MonoBehaviour
    {
        [Header("Configuraci�n de bala")]
        public float lifetime = 10f;              // Tiempo de vida m�ximo en segundos
        public GameObject explosionVfx;          // Prefab de la explosi�n

        private float timer = 0f;                // Acumula el tiempo transcurrido

        void Update()
        {
            // Controla el tiempo de vida de la bala
            timer += Time.deltaTime;
            if (timer >= lifetime)
            {
                DestroyBullet(); // Se destruye al superar el tiempo, con explosi�n
            }
        }


        void OnTriggerEnter(Collider other)
        {
            
            DestroyBullet();
            
        }

        /// <summary>
        /// Destruye la bala e instancia la explosi�n (si existe el prefab).
        /// </summary>
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
