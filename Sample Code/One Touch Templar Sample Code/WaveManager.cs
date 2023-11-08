using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;
    GameManager gameManager;
    public GameObject ReferenceEnemy;
    public int waveCounter = 1;
    public int[] WaveSpawnNumbers;
    public TMPro.TextMeshProUGUI WaveCountUI;
    public List<GameObject> EnemyList = new List<GameObject>();
    public List<Spawner> spawners = new List<Spawner>();
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        WaveCountUI.text = waveCounter.ToString();
        
        /*if(Input.GetKeyDown(KeyCode.P)){
            changeWave();
        }*/

        if(EnemyList.Count == 0)
        {
            changeWave(); 
        }
    }
    int checkHowManyToSpawn(int WaveIndex)
    {
        int numberToSpawn = 0;
        numberToSpawn = WaveSpawnNumbers[WaveIndex];
        return numberToSpawn;
    }
    public void changeWave()
    {

        if (waveCounter == 10)
        {
            gameManager.GoToWin();
        }
        else
        {
            List<Spawner> usableSpawns = new List<Spawner>();
            waveCounter += 1;
            foreach(Spawner s in spawners)
            {
                if (s.isPlayerInside == false)
                {
                    if (!usableSpawns.Contains(s))
                    {
                        usableSpawns.Add(s);
                    }
                }
            }
            int n = checkHowManyToSpawn(waveCounter);
            for(int i = n; i > 0; i--)
            {
                if (usableSpawns.Count == 0)
                {
                    Debug.Log("No spawns to spawn on");
                }
                else
                {
                    int maxRange = usableSpawns.Count - 1;
                    int choice = Mathf.RoundToInt(Random.value * maxRange);
                    Spawner chosenSpawn = usableSpawns[choice];
                    chosenSpawn.Spawn(ReferenceEnemy);
                    usableSpawns.Remove(chosenSpawn);
                }

            }
        }
       
    }
}
