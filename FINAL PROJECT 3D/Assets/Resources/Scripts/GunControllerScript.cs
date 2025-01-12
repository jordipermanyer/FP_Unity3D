using System.Collections;
using UnityEngine;

namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class Gun : MonoBehaviour
    {

        public GameObject explosionVfx;
        public GameObject bulletPrefab;  
        

        public void Shoot(Vector3 direction, float distanceToTarget)
        {
            // Create the ray from the center of the screen
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            // Instantiate the bullet at the camera's position
            GameObject bullet = Instantiate(bulletPrefab, Camera.main.transform.position, Quaternion.identity);

            bullet.transform.rotation = Quaternion.LookRotation(ray.direction);
            bullet.transform.Rotate(new Vector3(0, 90, 90));

            // Get the Rigidbody of the bullet
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Dynamically set the bullet's speed based on the target distance
                float adjustedSpeed = distanceToTarget / 0.1f; // 0.1 seconds to visually match
                rb.velocity = ray.direction * adjustedSpeed;
            }
        }

    }
}
