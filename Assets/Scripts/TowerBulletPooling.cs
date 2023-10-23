using UnityEngine;
using System.Collections;
using static UnityEngine.ParticleSystem;
using Unity.VisualScripting;

public class TowerBulletPooling : MonoBehaviour {

    public float Speed;
    public Transform target;
    
    public Vector3 impactNormal; 
    Vector3 lastBulletPosition; 
    public TowerPooling twr;    
    float i = 0.05f; // delay time of bullet destruction

    
    void Update() {

        // Bullet move

        if (target) 
        {        
            
            transform.LookAt(target);
            transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * Speed); 
            lastBulletPosition = target.transform.position; 

        }

        // Move bullet ( enemy was disapeared )

        else          {
                        
                transform.position = Vector3.MoveTowards(transform.position, lastBulletPosition, Time.deltaTime * Speed); 

                if (transform.position == lastBulletPosition) 
                {
                //Destroy(gameObject,i);
                StartCoroutine(ReleaseBulletTime()); //pooling

                // Bullet hit ( enemy was disapeared )

                GameObject impactParticle = BulletSpawnPooling.instance.particlePool.Get();
                impactParticle.SetActive(true);
                BulletSpawnPooling.instance.ParticleRelease(impactParticle);
                return;

            }           
        }     
    }

    // Bullet hit

    void OnTriggerEnter (Collider other) // tower`s hit if bullet reached the enemy
    {
        if(other.gameObject.transform == target)
        {
            target.GetComponent<EnemyHp>().Dmg(twr.dmg);
            //Destroy(gameObject, i); // destroy bullet
            StartCoroutine(ReleaseBulletTime()); //pooling
            //impactParticle = Instantiate(impactParticle, target.transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal)) as GameObject;
            //impactParticle.transform.parent = target.transform;
            //Destroy(impactParticle, 3);
            GameObject impactParticle = BulletSpawnPooling.instance.particlePool.Get();
            impactParticle.SetActive(true);
            impactParticle.transform.position = target.transform.position;
            /*
            Debug.Log("Hopefully releases particle");
            BulletSpawnPooling.instance.ParticleRelease(impactParticle);
            */
            BulletSpawnPooling.instance.ParticleRelease(impactParticle); //calls method to release from BulletSpawnPooling since a coroutine wont work - this script will be inactive by the time it finishes
            return;  
        }
    }

    IEnumerator ReleaseBulletTime()
    {
        yield return new WaitForSeconds(i);
        BulletSpawnPooling.instance.BulletRelease(this);
    }
}



