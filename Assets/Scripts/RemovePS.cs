using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovePS : MonoBehaviour
{
    ParticleSystem ps;
    public float liveTime = 2f;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        StartCoroutine(StopPs());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StopPs()
    {
        yield return new WaitForSeconds(liveTime);

        ps.Stop();

        yield return new WaitForSeconds(5f);

        Destroy(gameObject);
    }
}
