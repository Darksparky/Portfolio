using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public StorageData storageData = new StorageData();
    private IDataService DataService = new JsonDataService();
    private bool isEncrypted = false; 
    public GameObject player;
    private long SaveTime;
    private long LoadTime;
    public bool Continuing = false;
    public bool Tutorial = false;
    public bool ReturningToMain = false;
    public bool TestUnfrozen = false;
    public QuestManager questManager;
    public DayNightCycle dayNightCycle;
    public DialogueManager dialogueManager;
    public MapGenerator mapGenerator;
    public InventoryController inventoryController;
    public Landmark[] mainLandmarks;

    public bool UseDyslexicFont = false;

    //Awake is called before start, as soon as the game object is loaded
    private void Awake()
    {
        

        //Create a singleton instance of the game manager
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
        }

    }

    public IEnumerator LoadAsync(int sceneIndex){
        LoadingBar loadingBar = DontDestroy.Instance.loadingBar.GetComponent<LoadingBar>();
        loadingBar.gameObject.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;
        while(operation.progress < 0.9f){
            loadingBar.goalWidth = Mathf.Clamp01(operation.progress / 0.9f) * 1050f;
            loadingBar.width = Mathf.Lerp(loadingBar.width,loadingBar.goalWidth,loadingBar.speed);
            loadingBar.rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, loadingBar.width);
            Debug.Log(Mathf.Clamp01(operation.progress / 0.9f) * 1050f);
            yield return null;
        }
        loadingBar.goalWidth = 1050f;
        operation.allowSceneActivation = true;
        yield return null;
    }

    public void ToggleEncryption(bool value)
    {
        this.isEncrypted = value;
    }


    public void OnSave()
    {
        bool success = false;
        Debug.Assert(questManager && dayNightCycle && dialogueManager && mapGenerator && inventoryController);
        mapGenerator.OnSave();
        storageData.playerPosition = new SerializableVector3(player.transform.position);
        storageData.playerRotation = new SerializableVector3(player.transform.eulerAngles);
        storageData.cameraRotation = new SerializableVector3(player.GetComponent<CameraController>().cameraTarget.transform.eulerAngles);
        storageData.itemObjs = ItemManager.findAllItemsType();
        questManager.OnSave();
        dialogueManager.OnSave();
        inventoryController.OnSave();
        dayNightCycle.OnSave();
        storageData.mainLandmarks = mainLandmarks;
        try
        {
            success = DataService.SaveData("/StorageData.json", storageData, isEncrypted);
        }
        catch(Exception e)
        {
            Debug.LogError("Unable to save data due to: " + e.Message + " " + e.StackTrace);
        }

        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        if(pauseMenu!= null)
        {
            StartCoroutine(pauseMenu.DisplaySaveFeedback(success));
        }
    }


    public void OnLoad()
    {
        try
        {
            storageData = DataService.LoadData<StorageData>("/StorageData.json", isEncrypted);
        }catch(Exception e)
        {
            Debug.LogError(("Failed to load data - " + e.Message + " " + e.StackTrace));
        }
        foreach(Landmark land in mainLandmarks)
        {
            foreach(Landmark sLand in storageData.mainLandmarks)
            {
                if(sLand.name == land.name)
                {
                    land.startingSpot = sLand.startingSpot;
                    land.freeze = sLand.freeze;
                    land.found = sLand.found;
                    land.mapped = sLand.mapped;
                    land.index = sLand.index;
                    land.position = sLand.position;
                    land.humidity = sLand.humidity;
                    land.temperature = sLand.temperature;
                    land.height = sLand.height;
                }
            }
        }
    }

    public void SerializeJson()
    {
        long startTime = DateTime.Now.Ticks;
        if (DataService.SaveData("/StorageData.json", storageData, isEncrypted)){
            SaveTime = DateTime.Now.Ticks - startTime;
            Debug.Log($"Save Time: {(SaveTime/ TimeSpan.TicksPerMillisecond): N4}ms");
            
        }else
        {

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
/*            public QuestManager questManager;
    public DayNightCycle dayNightCycle;
    public DialogueManager dialogueManager;
    public MapGenerator mapGenerator;
    public InventoryController inventoryController;*/
        if (player == null)
        {
            if (GameObject.FindGameObjectWithTag("Player")!= null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
        }
        if (questManager == null)
        {
            questManager = QuestManager.Instance;
        }
        if(dayNightCycle == null)
        {
            dayNightCycle = DayNightCycle.Instance;
        }
        if(dialogueManager == null)
        {
            dialogueManager = DialogueManager.Instance;
        }
        if(mapGenerator == null)
        {
            mapGenerator = MapGenerator.Instance;
        }
        if (inventoryController == null)
        {
            inventoryController = InventoryController.Instance;
        }
    }


    public void Quit()
    {
        //Do the saving stuff then 
        Application.Quit();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += LoadedScene;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= LoadedScene;
    }

    void LoadedScene(Scene scene,LoadSceneMode mode)
    {

        if (scene.buildIndex==1 && !Continuing && !TestUnfrozen)
        {
            Debug.Log("FROZEN RESET");
            foreach(Landmark land in mainLandmarks)
            {
                land.freeze = false;
                land.found = false;
                land.mapped = false;
            }
        }
        else{
            Debug.Log("FROZEN NOT RESET");
        }

    }



}
