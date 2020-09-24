﻿using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityStandardAssets.Effects;

[RequireComponent(typeof(Damage))]
public class Explosive : MonoBehaviour {
    public float _damage = 10;
    public Vector3 direction;
    public float speed;
    public float timeToExpire = 5f;
    public float radius = 7f;
    public float power = 10f;
    public float fuseTime = 0f;
    
    [SerializeField]
    private bool _explode;

    public void Launch(Vector3 direction, float speed)
    {
        this.direction = direction;
        this.speed = speed;
        gameObject.GetComponent<Rigidbody>().velocity = direction * speed;
        StartCoroutine(Explode(timeToExpire));
    }
    
    public void OnTriggerEnter(Collider other)
    {
        // if (other.gameObject.tag.Equals("Player")  || other.gameObject.tag.Equals("Projectile"))
        // {
        //     return;
        // }
        
        Detonate(fuseTime);
    }

    public void Detonate(float time = 0)
    {
        if (_explode) return;
        StopAllCoroutines();
        _explode = true;
        StartCoroutine(Explode(time));
    }

    IEnumerator Explode(float time)
    {
        yield return new WaitForSeconds(time);
        Vector3 explosionPos = transform.position;
        var damageScript = GetComponent<Damage>();
        damageScript.SetDamage(_damage);   
        
        var colliders = Physics.OverlapSphere(explosionPos, radius);

        foreach (Collider hit in colliders)
        {
            damageScript.DealDamage(hit);
        }
        
        AudioManager.PlaySoundAtPosition("explosion", transform.position);
        ScreenShakeManager.Instance.ScreenShake(0.5f, 0.8f);
        //ONly works for one prefab
        var o = gameObject.transform.GetChild(1).gameObject;
        var explosionPhysicsForce = o.GetComponent<ExplosionPhysicsForce>();
        explosionPhysicsForce.explosionForce = power;
        explosionPhysicsForce.explosionRadius = radius;
        o.transform.localScale = Vector3.one * radius / 7f;
        o.SetActive(true);
        o.transform.SetParent(null);
        Destroy(gameObject);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, radius);
    }
}