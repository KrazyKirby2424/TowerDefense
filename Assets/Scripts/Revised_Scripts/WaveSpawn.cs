using UnityEngine;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine.Pool;

public class WaveSpawn : MonoBehaviour {

	public int WaveSize;
	public GameObject EnemyPrefab;
	public float EnemyInterval;
	public Transform spawnPoint;
	public float startTime;
	public Transform[] WayPoints;
	private int enemyCount=0;
	private bool waveEnd = false;
	private int currentWave = 0;

    void Start ()
    {
		NewWave(WaveSize, EnemyInterval);
	}

	void Update()
	{
		if(enemyCount == WaveSize)
		{
			CancelInvoke(nameof(SpawnEnemy));
			waveEnd= true;
		}
	}

	void SpawnEnemy()
	{
		enemyCount++;
		GameObject enemy = GameObject.Instantiate(EnemyPrefab,spawnPoint.position,Quaternion.identity) as GameObject;
		enemy.GetComponent<Enemy>().waypoints = WayPoints;
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
}
