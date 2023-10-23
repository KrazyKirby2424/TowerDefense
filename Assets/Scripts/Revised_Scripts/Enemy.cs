using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{

    public enum EnemyType
    {
        Normal,
        Sprinter,
        Boss,
    };
    [SerializeField]
    private EnemyType enemyType;
    public Transform shootElement;
    public GameObject bullet;
    public GameObject Enemybug;
    public int Creature_Damage = 10;
    public float curSpeed;
    public Transform[] waypoints;
    int curWaypointIndex = 0;
    public float setSpeed;
    private Animator anim;
    public EnemyHp Enemy_Hp;
    public Transform target;
    public GameObject EnemyTarget;

    public int value = 10;
    public bool coinsAdded = false;

    private void Awake()
    {
        Enemy_Hp = Enemybug.GetComponent<EnemyHp>(); //needs to be in awake since reset is being called from pool
        anim = GetComponent<Animator>();
    }

    // Attack

    void OnTriggerEnter(Collider other)

    {
        if (other.tag == "Castle")
        {
            curSpeed = 0;
            EnemyTarget = other.gameObject;
            target = other.gameObject.transform;
            Vector3 targetPosition = new Vector3(EnemyTarget.transform.position.x, transform.position.y, EnemyTarget.transform.position.z);            
            transform.LookAt(targetPosition);
            anim.SetBool("RUN", false);
            anim.SetBool("Attack", true);
            //Debug.Log(this.gameObject + " aggros " + EnemyTarget);
        }

    }

    // Attack
    void Shooting ()
    {
        //if (EnemyTarget)
       // {           
            GameObject с = GameObject.Instantiate(bullet, shootElement.position, Quaternion.identity) as GameObject;
            с.GetComponent<EnemyBullet>().target = target;
            с.GetComponent<EnemyBullet>().twr = this;
       // }  

    }

    
    void GetDamage () //is triggered by animation event
    {
        if(EnemyTarget) //safety check to make sure not null
        {
            EnemyTarget.GetComponent<TowerHP>().Dmg_2(Creature_Damage);
        }
    }       
    


    void Update () 
	{

        
        //Debug.Log("Animator  " + anim);


        // MOVING

        if (curWaypointIndex < waypoints.Length)
        {
	        transform.position = Vector3.MoveTowards(transform.position,waypoints[curWaypointIndex].position,Time.deltaTime*curSpeed);
            
            if (!EnemyTarget)
            {
                transform.LookAt(waypoints[curWaypointIndex].position);
            }
	
	        if(Vector3.Distance(transform.position,waypoints[curWaypointIndex].position) < 0.5f)
	        {
		        curWaypointIndex++;
	        }    
	    }          

        else
        {
            anim.SetBool("Victory", true);  // Victory
        }

        // Attack to Run

        //Order is important since else causes enemy speed to be set after death
        if (EnemyTarget)
        {
            //Debug.Log(this.gameObject + " attemting to reset " + EnemyTarget);
            if (EnemyTarget.CompareTag("Castle_Destroyed")) // get it from BuildingHp
            {
                //Debug.Log(this.gameObject + " resetting castle from " + EnemyTarget);
                anim.SetBool("Attack", false);
                anim.SetBool("RUN", true);
                curSpeed = setSpeed;
                EnemyTarget = null;
            }
        }
        else //safety check when null in case it doesn't deaggro 
        {
            anim.SetBool("Attack", false);
            anim.SetBool("RUN", true);
            curSpeed = setSpeed;
            EnemyTarget = null;
        }

        // DEATH

        if (Enemy_Hp.EnemyHP <= 0)
        {
            if(!coinsAdded)
            {
                CoinSystem.instance.DeathCoins(value);
                coinsAdded = true;
            }
            curSpeed = 0;
            anim.SetBool("Death", true);
            StartCoroutine(DeathTimer()); //pooling
            
            //Destroy(gameObject, 5f);//not pooling
        }
    }
    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(5);
        WaveSpawnPooling.instance.EnemyRelease(this, enemyType);//pooling
    }
    public void Reset()
    {
        //hp needs to be reset as well
        Enemy_Hp.ResetHP();
        coinsAdded = false;
        curSpeed = setSpeed;
        curWaypointIndex = 0;
        EnemyTarget = null;
        target = null;
    }
}

