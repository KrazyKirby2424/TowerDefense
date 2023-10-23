using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHp : MonoBehaviour {

    public int EnemyHP = 30;
    private int resetHP; //created instead of totally revamping script for now
    private void Awake()//sets the reset
    {
        resetHP = EnemyHP;
    }
    public void Dmg(int DMGcount)
    {
        EnemyHP -= DMGcount;
    }
    public void ResetHP()//used by enemy to reset for pooling 
    {
        EnemyHP = resetHP;
        gameObject.tag = "enemyBug";
    }

    private void Update()
    {        

        if (EnemyHP <= 0)
        {
            gameObject.tag = "Dead"; // send it to TowerTrigger to stop the shooting
           
        }
    }
    
}
