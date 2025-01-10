using System.Collections;
using UnityEngine;

namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class Gun : MonoBehaviour
    {
        [Header("Puntos de referencia")]
        public Transform bulletSpawnPoint; // Transform donde se genera la bala
        
        [Header("Prefabs")]
        public GameObject bulletPrefab;    // Prefab de la bala
        
        [Header("Parámetros de disparo")]
        [Tooltip("Velocidad de la bala")]
        public float bulletSpeed = 50f;
        [Tooltip("Frecuencia de disparo (segundos entre cada disparo)")]
        public float fireRate = 0.2f;

        private float nextFireTime = 0f;

        void Update()
        {
                // Dispara con la tecla Espacio y respeta la cadencia (fireRate)
                if (Input.GetMouseButtonDown(0) && Time.time > nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }

        void Shoot()
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

            // Obtener el Rigidbody de la bala
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                
                // Asignamos la velocidad directamente en la dirección 'forward' de bulletSpawnPoint
                rb.velocity = bulletSpawnPoint.forward * bulletSpeed;
            }
        }
    }
}
