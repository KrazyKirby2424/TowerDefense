using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

//Notes
/* Additions:
 * Can add catapult - huge range and damage super slow fire rate
 * Add new particle systems - ice, fire to bullets
 * Add elemental damage: Ice freezes fire,
 * 
 */

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject towerPrefab;
    [SerializeField] GameObject barricadePrefab;
    [SerializeField] Light towerPlacement;
    [SerializeField] WaveSpawnPooling waves; //just WaveSpawn for not pooling dev scene
    [SerializeField] TMP_Text curWave;
    [SerializeField] TMP_Text gameText;
    [SerializeField] TMP_Text castleHealth;
    [SerializeField] TowerHP tower;
    [SerializeField] GameObject mainMenu;
   
    private bool gameState = true; //set to false when game ends
    private bool victory = false; //set if all waves have been defeated
    public int maxWaves = 10;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState) //playing 
        {
            castleHealth.text = "Castle Health: " + tower.CastleHp;
            if(waves.CheckWave() && waves.CurrentWave() < maxWaves)//if wave ended start new wave
            {
                waves.NewWave(waves.WaveSize * 1.1, Mathf.Max(.2f, waves.EnemyInterval / 1.1f));
                curWave.text = "Wave: " + waves.CurrentWave();
            }
            HandleTower();
            HandleBarricade();
            if (tower.CastleHp <= 0)
            {
                gameState = false;
                victory = false;
            }
            //Optimize by creating an enemy controller instance that keeps track alive enemys and decreases when dead - cond if enemies = 0 and no more waves
            if(FindObjectOfType<Enemy>() == null && waves.CurrentWave() >= maxWaves) //all enemies are dead and no waves left
            {
                gameState = false;
                victory = true;
            }
        }
        else //end game and set victory text
        {
            mainMenu.SetActive(true);
            if(victory)
            {
                gameText.text = "Victory!\nCongratulations you have successfully survived the onslaught!";
            }
            else
            {
                gameText.text = "Defeat!\nYou lost on wave " + waves.CurrentWave();
            }
        }
    }

    private void HandleTower()
    {
        if (buyTower)
        {
            towerPlacement.enabled = true;
            int layerMask = 1 << 6;
            RaycastHit hit;
            //Vector3 direction = Camera.current.ScreenToWorldPoint(Input.mousePosition);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if((1 << hit.collider.gameObject.layer & layerMask) != 0) //bit mask comparison - collider is on the placable tower layer
                {
                    towerPlacement.color = Color.green;
                    towerPlacement.gameObject.transform.position = hit.point + new Vector3(0f, 8f, 0f);
                    Debug.DrawRay(Camera.main.ScreenToWorldPoint(Input.mousePosition), ray.direction * hit.distance, Color.yellow);
                    //Debug.Log(hit.collider + " " + hit.point);
                    if (Input.GetMouseButtonDown(0)) //left mouse
                    {
                        PlaceTower(hit.point, cost);
                        towerPlacement.enabled = false;
                    }
                }
                else
                {
                    towerPlacement.color = Color.red;
                    towerPlacement.gameObject.transform.position = hit.point + new Vector3(0f, 8f, 0f);
                    Debug.DrawRay(Camera.main.ScreenToWorldPoint(Input.mousePosition), ray.direction * hit.distance, Color.white);
                    //Debug.Log(hit.collider + " " + hit.point);
                    //RaycastHit hitFail;
                    //if (Physics.Raycast(ray, out hitFail, Mathf.Infinity, ~layerMask)) //inverted mask
                    if (Input.GetMouseButtonDown(0)) //left mouse
                    {
                        buyTower = false;
                        towerPlacement.enabled = false; //invalid position stops tower placement
                    }
                }

                
            }
        }
    }

    private void HandleBarricade()
    {
        if (buyBarricade)
        {
            towerPlacement.enabled = true;
            int layerMask = 1 << 7;
            RaycastHit hit;
            //Vector3 direction = Camera.current.ScreenToWorldPoint(Input.mousePosition);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if ((1 << hit.collider.gameObject.layer & layerMask) != 0) //bit mask comparison - collider is on the placable tower layer
                {
                    towerPlacement.color = Color.green;
                    towerPlacement.gameObject.transform.position = hit.point + new Vector3(0f, 8f, 0f);
                    Debug.DrawRay(Camera.main.ScreenToWorldPoint(Input.mousePosition), ray.direction * hit.distance, Color.yellow);
                    //Debug.Log(hit.collider + " " + hit.point);
                    if (Input.GetMouseButtonDown(0)) //left mouse
                    {
                        PlaceBarricade(hit.point, barricadeCost, hit.collider.gameObject);
                        towerPlacement.enabled = false;
                    }
                }
                else
                {
                    towerPlacement.color = Color.red;
                    towerPlacement.gameObject.transform.position = hit.point + new Vector3(0f, 8f, 0f);
                    Debug.DrawRay(Camera.main.ScreenToWorldPoint(Input.mousePosition), ray.direction * hit.distance, Color.white);
                    //Debug.Log(hit.collider + " " + hit.point);
                    //RaycastHit hitFail;
                    //if (Physics.Raycast(ray, out hitFail, Mathf.Infinity, ~layerMask)) //inverted mask
                    if (Input.GetMouseButtonDown(0)) //left mouse
                    {
                        buyBarricade = false;
                        towerPlacement.enabled = false; //invalid position stops tower placement
                    }
                }


            }
        }
    }
    void FixedUpdate()
    {
        /*
        // Bit shift the index of the layer (6) to get a bit mask for TowerTerrain
        int layerMask = 1 << 6;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        //layerMask = ~layerMask;

        RaycastHit hit;
        //Vector3 direction = Camera.current.ScreenToWorldPoint(Input.mousePosition);
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            towerPlaceable = true;
            place = hit.point;
            Debug.DrawRay(Input.mousePosition, ray.direction * hit.distance, Color.yellow);
            //Debug.Log("Did Hit");
        }
        else
        {
            towerPlaceable = false;
            Debug.DrawRay(Input.mousePosition, ray.direction * 1000, Color.white);
            //Debug.Log("Did not Hit");
        }
        /*
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(Input.mousePosition, direction, out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(Input.mousePosition, direction * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(Input.mousePosition, direction * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
        */
        
    }

    private bool buyTower = false;
    private  int cost;
    public void BuyTower(int cost)
    {
        if(CoinSystem.instance.CheckCost(cost))
        {
            buyTower = true;
            this.cost = cost;
            //Debug.Log("Attempting to buy tower");
            //allow for tower to show
        }
        else
        {
            buyTower = false;
            //flash red
        }
    }

    private void PlaceTower(Vector3 place, int cost)
    {
        Instantiate(towerPrefab, place, Quaternion.identity);
        CoinSystem.instance.PurchaseCoins(cost);
        buyTower = false;
        //Debug.Log("Tower bought successfully");
    }

    //buy barricade
    private bool buyBarricade = false;
    private int barricadeCost;
    public void BuyBarricade(int cost)
    {
        if (CoinSystem.instance.CheckCost(cost))
        {
            buyBarricade = true;
            this.barricadeCost = cost;
            //Debug.Log("Attempting to buy tower");
            //allow for tower to show
        }
        else
        {
            buyBarricade = false;
            //flash red
        }
    }

    private void PlaceBarricade(Vector3 place, int cost, GameObject obj)
    {
        //rotated 90 on the y for vertical paths
        if (obj.CompareTag("vertical"))
        {
            Instantiate(barricadePrefab, place, Quaternion.identity * Quaternion.Euler(0f, 90f, 0f));
        }
        else if (obj.CompareTag("leftCorner"))
        {
            //Debug.Log("spawning left");
            Instantiate(barricadePrefab, place, Quaternion.identity * Quaternion.Euler(0f, 45f, 0f));
        }
        else if (obj.CompareTag("rightCorner"))
        {
            //Debug.Log("spawning right");
            Instantiate(barricadePrefab, place, Quaternion.identity * Quaternion.Euler(0f, -45f, 0f));
        }
        else
        {
            Instantiate(barricadePrefab, place, Quaternion.identity);
        }
        CoinSystem.instance.PurchaseCoins(cost);
        buyBarricade = false;
        //Debug.Log("Tower bought successfully");
    }

    //Could implement fences to stall goblins
    //Place them on a path layer and similar setup to purchasing towers
}
