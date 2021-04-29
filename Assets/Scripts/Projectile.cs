using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField, Range(0.1f, 500f)]
    float speed = 50f;
    [SerializeField, Range(0.1f, 15f)]
    float keepAlive = 5f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, keepAlive);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime);
    }
}
