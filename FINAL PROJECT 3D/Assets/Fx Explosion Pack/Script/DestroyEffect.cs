using UnityEngine;
using System.Collections;

public class DestroyEffect : MonoBehaviour 
{
    void Start()
    {
        // Se destruirá automáticamente a los 2 segundos
        Destroy(gameObject, 2f);
    }
}
