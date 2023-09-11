using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask targetLayer;
    public int damage;
    public float moveSpeed = 1f;
    public GameObject hitEffectPrefab;
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {        
        // 3초 후 자동 삭제 
        StartCoroutine(DelayDestory(3f));
    }

    // Update is called once per frame
    void Update()
    {
        rb.position += transform.forward * moveSpeed * Time.deltaTime;
        //transform.Translate(0, 0, moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter : " + other.name);

        if ((1 << other.gameObject.layer & targetLayer) == 0) return;

        IDamagable damagable = other.gameObject.GetComponent<IDamagable>();
        if (damagable != null)
        {
            damagable.TakeDamage(damage, null);
        }

        //StartCoroutine(DestroyParticle(0.0f));

        AfterHit();
    }

    IEnumerator DelayDestory(float time)
    {
        yield return new WaitForSeconds(time);

        AfterHit();
    }

    void AfterHit()
    {
        Instantiate(hitEffectPrefab, transform.position, hitEffectPrefab.transform.rotation);
        Destroy(gameObject);
    }
}
