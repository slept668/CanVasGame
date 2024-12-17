using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator anim;
    public int health;
    public bool canTakeDamage;


    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        canTakeDamage = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            StartCoroutine(DeathTimer());
        }
    }

    public void TakeDamage(int damage)
    {if (canTakeDamage)
        {
            health -= damage;
            anim.SetTrigger("gotHit");
        }
    }

    public void TakeDamageTrue()
    {
        canTakeDamage = true;
    }

    public void TakeDamageFalse()
    {
        canTakeDamage = false;
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(0.3f);
        anim.SetBool("isAlive", false);
        yield return new WaitForSeconds(0.3f);
        gameObject.SetActive(false);

    }
}
