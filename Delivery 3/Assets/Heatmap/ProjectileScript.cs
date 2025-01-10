using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    // Time duration before destroying the projectile
    [SerializeField] private float lifeTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        // Destroy the projectile after the specified lifetime
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        // Move the projectile downward
        transform.position = transform.position + new Vector3(0f, -1f, 0f) * Time.deltaTime;
    }
}
