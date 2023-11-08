using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager Instance;
    public Scene StartScreen;
    public Scene GameScene;
    public Scene OverScene;

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
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartScreen = SceneManager.GetSceneByName("StartMenu");
        GameScene = SceneManager.GetSceneByName("GameScene");
        OverScene = SceneManager.GetSceneByName("GameOver");
        Application.targetFrameRate = -1;
        QualitySettings.vSyncCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Check if the scene is the menu
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
        {
            //check for player input
            if (Input.GetKeyDown(KeyCode.X))
            {
                GoToGame();
            }
/*            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {

                }
            }*/
        }
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(2))
        {
            Debug.Log("In OverScene");
            //check for player input
            if (Input.GetKeyDown(KeyCode.X))
            {
                GoToMenu();
            }
/*            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {

                }
            }*/
        }
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(3))
        {
            Debug.Log("In OverScene");
            //check for player input
            if (Input.GetKeyDown(KeyCode.X))
            {
                GoToMenu();
            }
            /*            if (Input.touchCount > 0)
                        {
                            if (Input.GetTouch(0).phase == TouchPhase.Began)
                            {

                            }
                        }*/
        }
    }

    public void GoToMenu()
    {
        Debug.Log("Called GoToMenu");
        SceneManager.LoadScene(0);

    }

    public void GoToGame()
    {
        SceneManager.LoadScene(4);
    }

    public void GoToOver()
    {
        SceneManager.LoadScene(2);
    }

    public void GoToWin()
    {
        SceneManager.LoadScene(3);
    }
}
