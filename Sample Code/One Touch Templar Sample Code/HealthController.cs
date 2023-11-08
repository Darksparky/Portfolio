using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public GameObject Healthbar;
    public GameObject Fill;
    public GameObject blood;
    private float bloodEffectTimer = 0;
    GameManager GM;
    [SerializeField]
    Sprite highFill;
    [SerializeField]
    Sprite midFill;
    [SerializeField]
    Sprite lowFill;
    public int health = 100;
    // Start is called before the first frame update
    void Start()
    {
       // highFill = Resources.Load<Sprite>("UI_Bar_Fill_Green");
        //midFill = Resources.Load<Sprite>("UI_Bar_Fill_Orange");
       // lowFill = Resources.Load<Sprite>("UI_Bar_Fill_Red");
    }

    // Update is called once per frame
    void Update()
    {
        if (GM == null)
        {
            GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        }
        if (health > 0)
        {
            // if the healthbar UI element exists scale it horizontally based on the players health. 
            if (Healthbar != null && Fill!=null)
            {
                Healthbar.GetComponent<Slider>().value = health;
            }

           

        }
        if(lowFill!=null && midFill!=null && highFill != null && Fill !=null)
        {
            if (health >= 66)
            {
                Fill.gameObject.GetComponent<Image>().sprite = highFill;
            }
            else if (health < 66 && health > 33)
            {
                Fill.gameObject.GetComponent<Image>().sprite = midFill;
            }
            else
            {
                Fill.gameObject.GetComponent<Image>().sprite = lowFill;
            }
        }

        if (bloodEffectTimer > 0f)
        {   
            if (blood.activeInHierarchy == false)
            {
                blood.SetActive(true);
            }
            bloodEffectTimer -= Time.deltaTime;
            
            if (bloodEffectTimer <= 0f)
            {
                if (blood.activeInHierarchy)
                {
                    blood.SetActive(false);
                }
            }
        }



        /*
        if (Input.GetKeyDown(KeyCode.Q))
        {
            health = 100;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            health = 50;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            health = 25;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReduceHealth();
        } 
        */
    }

    public void ReduceHealth()
    {
        health = health - 10;
        bloodEffectTimer = 0.25f;
        if (health <= 0)
        {
            health = 0;
            GM.GoToOver();
        }
        Debug.Log("health reduced to " + health);

    }
}
