using System.Collections;
using UnityEngine;

namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class Gun : MonoBehaviour
    {
        public Transform bulletSpawnPoint;
        public GameObject bulletPrefab;
        public float bulletSpeed = 10f;
        public float fireRate = 0.2f; // Frecuencia de disparo en segundos

        private float nextFireTime = 0f;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && Time.time > nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }

        void Shoot()
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                bullet.transform.Rotate(new Vector3(90f, 0f, 0f));
                rb.velocity = bulletSpawnPoint.forward * bulletSpeed;
            }
        }
    }
}