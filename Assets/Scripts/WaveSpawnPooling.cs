using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.EventSystems.EventTrigger;

public class WaveSpawnPooling : MonoBehaviour
{

    ObjectPool<Enemy> pool;
    ObjectPool<Enemy> poolSprinter;
    ObjectPool<Enemy> poolBoss;
    public static WaveSpawnPooling instance;

    public int WaveSize;
    public Enemy EnemyPrefab;
    public Enemy[] enemies;
    public float EnemyInterval;
    public Transform spawnPoint;
    public float startTime;
    public Transform[] WayPoints;
    private int enemyCount = 0;
    private bool waveEnd = false;
    private int currentWave = 0;


    void PoolHelper(Enemy enemy)
    {
        enemy.Reset();//resets default settings of enemy
        enemy.transform.position = spawnPoint.position; //reset position to spawner
        enemy.waypoints = WayPoints;
        enemy.gameObject.SetActive(true); //get
    }
    //Instantiate is 
    private void Awake()
    {
        if (enemies.Count() < 3) //failsafe check in case editor settings have been tampered, count should always be equal to the number of pools 
            return;
        if (instance == null)
        {
            instance = this;
        }
        pool = new ObjectPool<Enemy>(() =>
        {
            return Instantiate(enemies[0], spawnPoint.position, Quaternion.identity); //create
        },
        enemy =>
        {
            PoolHelper(enemy);
        },
        enemy =>
        {
            enemy.gameObject.SetActive(false); //release
        },
        enemy =>
        {
            Destroy(enemy.gameObject);
        }, false, 1000, 10000); //check to false to save cpu, default capacity, max size
        poolSprinter = new ObjectPool<Enemy>(() =>
        {
            return Instantiate(enemies[1], spawnPoint.position, Quaternion.identity); //create
        },
        enemy =>
        {
            PoolHelper(enemy);
        },
        enemy =>
        {
            enemy.gameObject.SetActive(false); //release
        },
        enemy =>
        {
            Destroy(enemy.gameObject);
        }, false, 1000, 10000); //check to false to save cpu, default capacity, max size
        poolBoss = new ObjectPool<Enemy>(() =>
        {
            return Instantiate(enemies[2], spawnPoint.position, Quaternion.identity); //create
        },
        enemy =>
        {
            PoolHelper(enemy);
        },
        enemy =>
        {
            enemy.gameObject.SetActive(false); //release
        },
        enemy =>
        {
            Destroy(enemy.gameObject);
        }, false, 1000, 10000); //check to false to save cpu, default capacity, max size

        //Create Large Pool at start to draw from
        List<Enemy> list = new();
        Init(list, pool, 250);
        Init(list, poolSprinter, 250);
        Init(list, poolBoss, 250);
    }
    private void Init(List<Enemy> list, ObjectPool<Enemy> p, int num)
    {
        for (int i = 0; i < num; i++)
        {
            Enemy e = p.Get();
            list.Add(e);
        }
        for (int i = 0; i < num; i++)
        {
            p.Release(list[i]);
        }
        list.Clear();
    }

    void Start()
    {
        NewWave(WaveSize, EnemyInterval);
    }

    void Update()
    {
        if (enemyCount == WaveSize)
        {
            CancelInvoke(nameof(SpawnEnemy));
            waveEnd = true;
        }
    }

    int spawnRef = 0;
    void SpawnEnemy()
    {
        enemyCount++;
        if(currentWave % 10 == 0) //Spawn Boss
        {
            if(enemyCount < WaveSize-1) //last spawn boss
            {
                if (spawnRef < 2)
                {
                    poolSprinter.Get();
                    spawnRef++;
                }
                else
                {
                    pool.Get();
                    spawnRef = 0;
                }
            }
            else
            {
                poolBoss.Get();
                spawnRef = 0;
            }
        }
        else if(currentWave % 5 == 0) //Every 5 spawn both
        {
            if(spawnRef < 2)
            {
                poolSprinter.Get();
                spawnRef++;
            }
            else
            {
                pool.Get();
                spawnRef = 0;
            }
        }
        else if (currentWave % 3 == 0) //Every Three Waves Spawn Sprinters
        {
            poolSprinter.Get();
        }
        else
        {
            pool.Get();
        }
    }
    //New Wave Setup
    public void NewWave(double waveSize, float interval)
    {
        currentWave++;
        waveEnd = false;
        WaveSize = (int)waveSize;
        EnemyInterval = interval;
        enemyCount = 0;
        InvokeRepeating(nameof(SpawnEnemy), startTime, EnemyInterval);
    }
    public bool CheckWave()
    {
        return waveEnd;
    }
    public int CurrentWave()
    {
        return currentWave;
    }
    public void EnemyRelease(Enemy enemy, Enemy.EnemyType type)
    {
        switch (type)
        {
            case Enemy.EnemyType.Normal:
                pool.Release(enemy);
                break;
            case Enemy.EnemyType.Sprinter:
                poolSprinter.Release(enemy);
                break;
            case Enemy.EnemyType.Boss:
                poolBoss.Release(enemy);
                break;
            default:
                throw new Exception("Invalid type: " + nameof(type.ToString));
        }

        
    }
}
