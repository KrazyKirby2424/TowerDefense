﻿using UnityEngine;
using System.Collections;

public class TowerTriggerPooling : MonoBehaviour {

	public TowerPooling twr;    
    public bool lockE;
	public GameObject curTarget;



    //added code to facilitate reaggro of units already inside range
    //since Stay comes first units steady at tower will be prioritized instead of oncoming
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("enemyBug") && !lockE)
        {
            twr.target = other.gameObject.transform;
            curTarget = other.gameObject;
            lockE = true;
        }
    }
    void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("enemyBug") && !lockE)
		{   
			twr.target = other.gameObject.transform;            
            curTarget = other.gameObject;
			lockE = true;
		}
       
    }
    void Update()
	{
        if (curTarget)
        {
            if (curTarget.CompareTag("Dead")) // get it from EnemyHealth
            {
                lockE = false;
                twr.target = null;               
            }
        }




        if (!curTarget) 
		{
			lockE = false;            
        }
	}
	void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("enemyBug") && other.gameObject == curTarget)
		{
			lockE = false;
            twr.target = null;            
        }
	}
	
}
