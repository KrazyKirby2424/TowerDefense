using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.GraphicsBuffer;

public class BulletSpawnPooling : MonoBehaviour
{
    public ObjectPool<TowerBulletPooling> pool;
    public TowerBulletPooling bullet;
    public static BulletSpawnPooling instance;

    public ObjectPool<GameObject> particlePool;
    public GameObject impactParticle; // bullet impact

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        

        //Bullets
        pool = new ObjectPool<TowerBulletPooling>(() =>
        {
            return Instantiate(bullet, transform.position, Quaternion.identity);
        },
        bullet => //get
        {
        },
        bullet => //release
        {
            bullet.gameObject.SetActive(false);
        },
        bullet => //destroy
        {

        }, false, 1000, 10000); //check to false to save cpu, default capacity, max size

        //Particles
        particlePool = new ObjectPool<GameObject>(() =>
        {
            return Instantiate(impactParticle, transform.position, Quaternion.FromToRotation(Vector3.up, new Vector3(0f,0f,0f)));
        },
        particle => //get
        {
        },
        particle => //release
        {
            particle.SetActive(false);
        },
        particle => //destroy
        {

        }, false, 1000, 10000); //check to false to save cpu, default capacity, max size

        //Create Large Pool at start to draw from
        //Create Large Pool at start to draw from
        List<TowerBulletPooling> listB = new List<TowerBulletPooling>();
        List<GameObject> listP = new List<GameObject>();
        for (int i = 0; i < 500; i++)
        {
            TowerBulletPooling b = pool.Get();
            GameObject p = particlePool.Get();
            listB.Add(b);
            listP.Add(p);
        }
        for (int i = 0; i < 500; i++)
        {
            pool.Release(listB[i]);
            particlePool.Release(listP[i]);
        }
        listB.Clear();
        listP.Clear();
    }

    public void BulletRelease(TowerBulletPooling bullet)
    {
        pool.Release(bullet);
    }

    public void ParticleRelease(GameObject particle)
    {
        StartCoroutine(ReleaseParticleTime(particle));
    }
    IEnumerator ReleaseParticleTime(GameObject particle)
    {
        yield return new WaitForSeconds(3);
        /*
        foreach (ParticleSystem sys in particle.GetComponentsInChildren<ParticleSystem>())
        {
            sys.Stop();
        }
        */
        particlePool.Release(particle);
    }
}
