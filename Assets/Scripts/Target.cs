using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField, Range(1, 10)]
    int? health = 3;

    public void GetDamage(int damage)
    {
        health = health - damage >= 0 ? health - damage : health;
        if(health <= 0) Destroy(gameObject);
    }
}
