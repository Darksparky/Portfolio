using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The following is a script I created to manage interactions with interactable game objects/characters
public class InteractionManager : MonoBehaviour
{
    public GameObject primaryContextualFocus;

    public GameObject secondaryContextualFocus;
    public GameObject contextualActionUIObject;
    public GameObject contextualActionUIText;

    public GameObject contextualActionUIObject2;
    public GameObject contextualActionUIText2;
    public GameObject contextualActionUIWarning;
    public Animator contextualActionUIWarningAnimation;

    public PositionData positionData;

    string mostRecentAnimation = "FailedInteract1";

    public GameObject mainGameUI;

    public bool canFocus = true;

    enum TypeOfFocus
    {
        Nothing,
        Dialog,
        Item,
        HintObject,
        Map,
        Interactable
    }
    TypeOfFocus typeOfFocus;

    // Start is called before the first frame update
    void Start()
    {
        typeOfFocus = TypeOfFocus.Nothing; 
    }

    public void ResetInteracts(){
        primaryContextualFocus = null;
        secondaryContextualFocus = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (canFocus)
        {
            if (primaryContextualFocus == null)
            {
                if(secondaryContextualFocus == null){
                    contextualActionUIObject.SetActive(false);
                    contextualActionUIWarning.SetActive(false);
                    //Change the focus enum
                    typeOfFocus = TypeOfFocus.Nothing;
                    SetUIText(contextualActionUIText,primaryContextualFocus);
                }
                else{
                    primaryContextualFocus = secondaryContextualFocus;
                    secondaryContextualFocus = null;
                }
                contextualActionUIObject2.SetActive(false);

            }
            if(secondaryContextualFocus == null){
                contextualActionUIObject2.SetActive(false);
            }
            if(primaryContextualFocus != null)
            {
                //Check for the different components that we want to interact with
                if (primaryContextualFocus.GetComponent<YarnInteractable>())
                {
                    if (primaryContextualFocus.GetComponent<YarnInteractable>().isInteractable)
                    {
                        typeOfFocus = TypeOfFocus.Dialog;
                        contextualActionUIWarning.SetActive(true);
                        contextualActionUIObject.SetActive(true);
                        SetUIText(contextualActionUIText,primaryContextualFocus);

                    }
                    else
                    {
                        contextualActionUIObject.SetActive(false);
                        contextualActionUIWarning.SetActive(false);
                        typeOfFocus = TypeOfFocus.Nothing;
                        SetUIText(contextualActionUIText,primaryContextualFocus);
                    }
                }
                else if (primaryContextualFocus.GetComponent<ItemInteract>())
                {
                    if (primaryContextualFocus.GetComponent<ItemInteract>().isInteractable)
                    {
                        contextualActionUIWarning.SetActive(true);
                        contextualActionUIObject.SetActive(true);
                        typeOfFocus = TypeOfFocus.Item;
                        SetUIText(contextualActionUIText,primaryContextualFocus);
                    }
                    else
                    {
                        contextualActionUIObject.SetActive(false);
                        contextualActionUIWarning.SetActive(false);
                        typeOfFocus = TypeOfFocus.Nothing;
                        SetUIText(contextualActionUIText,primaryContextualFocus);
                    }
                }
                else if (primaryContextualFocus.GetComponent<HintInteract>())
                {
                    if (primaryContextualFocus.GetComponent<HintInteract>().isInteractable)
                    {
                        contextualActionUIWarning.SetActive(true);
                        contextualActionUIObject.SetActive(true);
                        typeOfFocus = TypeOfFocus.HintObject;
                        SetUIText(contextualActionUIText,primaryContextualFocus);
                    }
                    else
                    {
                        contextualActionUIObject.SetActive(false);
                        contextualActionUIWarning.SetActive(false);
                        typeOfFocus = TypeOfFocus.Nothing;
                        SetUIText(contextualActionUIText,primaryContextualFocus);
                    }
                }
                else if (primaryContextualFocus.GetComponent<LandmarkTrigger>())
                {
                    if (primaryContextualFocus.GetComponent<LandmarkTrigger>().isInteractable)
                    {
                        contextualActionUIWarning.SetActive(true);
                        contextualActionUIObject.SetActive(true);
                        typeOfFocus = TypeOfFocus.Map;
                        SetUIText(contextualActionUIText,primaryContextualFocus);
                    }
                    else
                    {
                        contextualActionUIObject.SetActive(false);
                        contextualActionUIWarning.SetActive(false);
                        typeOfFocus = TypeOfFocus.Nothing;
                        SetUIText(contextualActionUIText,primaryContextualFocus);
                    }
                }


                else if (primaryContextualFocus.GetComponent<Interactable>())
                {
                    if (primaryContextualFocus.GetComponent<Interactable>().isInteractable)
                    {
                        contextualActionUIWarning.SetActive(true);
                        contextualActionUIObject.SetActive(true);
                        typeOfFocus = TypeOfFocus.Interactable;
                        SetUIText(contextualActionUIText,primaryContextualFocus);
                    }
                    else
                    {
                        contextualActionUIObject.SetActive(false);
                        contextualActionUIWarning.SetActive(false);
                        typeOfFocus = TypeOfFocus.Nothing;
                        SetUIText(contextualActionUIText,primaryContextualFocus);
                    }
                }
            }
            if(secondaryContextualFocus != null)
            {
                //Check for the different components that we want to interact with
                if (secondaryContextualFocus.GetComponent<YarnInteractable>())
                {
                    if (secondaryContextualFocus.GetComponent<YarnInteractable>().isInteractable)
                    {
                        typeOfFocus = TypeOfFocus.Dialog;
                        contextualActionUIObject2.SetActive(true);
                        SetUIText(contextualActionUIText2,secondaryContextualFocus);

                    }
                    else
                    {
                        contextualActionUIObject2.SetActive(false);
                        typeOfFocus = TypeOfFocus.Nothing;
                        SetUIText(contextualActionUIText2,secondaryContextualFocus);
                    }
                }
                else if (secondaryContextualFocus.GetComponent<ItemInteract>())
                {
                    if (secondaryContextualFocus.GetComponent<ItemInteract>().isInteractable)
                    {
                        contextualActionUIObject2.SetActive(true);
                        typeOfFocus = TypeOfFocus.Item;
                        SetUIText(contextualActionUIText2,secondaryContextualFocus);
                    }
                    else
                    {
                        contextualActionUIObject2.SetActive(false);
                        typeOfFocus = TypeOfFocus.Nothing;
                        SetUIText(contextualActionUIText2,secondaryContextualFocus);
                    }
                }
                else if (secondaryContextualFocus.GetComponent<HintInteract>())
                {
                    if (secondaryContextualFocus.GetComponent<HintInteract>().isInteractable)
                    {
                        contextualActionUIObject2.SetActive(true);
                        typeOfFocus = TypeOfFocus.HintObject;
                        SetUIText(contextualActionUIText2,secondaryContextualFocus);
                    }
                    else
                    {
                        contextualActionUIObject2.SetActive(false);
                        typeOfFocus = TypeOfFocus.Nothing;
                        SetUIText(contextualActionUIText2,secondaryContextualFocus);
                    }
                }
                else if (secondaryContextualFocus.GetComponent<LandmarkTrigger>())
                {
                    if (secondaryContextualFocus.GetComponent<LandmarkTrigger>().isInteractable)
                    {
                        contextualActionUIObject2.SetActive(true);
                        typeOfFocus = TypeOfFocus.Map;
                        SetUIText(contextualActionUIText2,secondaryContextualFocus);
                    }
                    else
                    {
                        contextualActionUIObject2.SetActive(false);
                        typeOfFocus = TypeOfFocus.Nothing;
                        SetUIText(contextualActionUIText2,secondaryContextualFocus);
                    }
                }


                else if (secondaryContextualFocus.GetComponent<Interactable>())
                {
                    if (secondaryContextualFocus.GetComponent<Interactable>().isInteractable)
                    {
                        contextualActionUIObject2.SetActive(true);
                        typeOfFocus = TypeOfFocus.Interactable;
                        SetUIText(contextualActionUIText2,secondaryContextualFocus);
                    }
                    else
                    {
                        contextualActionUIObject2.SetActive(false);
                        typeOfFocus = TypeOfFocus.Nothing;
                        SetUIText(contextualActionUIText2,secondaryContextualFocus);
                    }
                }
            }
        }
        else
        {
            contextualActionUIObject.SetActive(false);
            contextualActionUIWarning.SetActive(false);
            contextualActionUIObject2.SetActive(false);
            
        }
        if(primaryContextualFocus){
            if(Vector3.Distance(positionData.player.transform.position,primaryContextualFocus.transform.position) > 19f){
                primaryContextualFocus = null;
            }
        }
        if(secondaryContextualFocus){ 
            if(Vector3.Distance(positionData.player.transform.position,secondaryContextualFocus.transform.position) > 19f){
                secondaryContextualFocus = null;
            } 
        }
    }

    public void InteractFailed(string reason){
        contextualActionUIWarningAnimation.SetTrigger(mostRecentAnimation);
        contextualActionUIWarning.GetComponent<TMPro.TextMeshProUGUI>().text = reason;
    }


    void SetUIText(GameObject text, GameObject obj)
    {
        if(typeOfFocus!= TypeOfFocus.Nothing)
        {
            text.GetComponent<TMPro.TextMeshProUGUI>().text = obj.GetComponent<Interactable>().text;
        }
        else
        {
            text.GetComponent<TMPro.TextMeshProUGUI>().text = "";
        }
        
    }

    public void RemoveFocus(GameObject obj)
    {
        if (primaryContextualFocus == obj)
        {
            primaryContextualFocus = null;
            typeOfFocus = TypeOfFocus.Nothing;
        }
        else if(secondaryContextualFocus == obj){
            secondaryContextualFocus = null;
        }

    }


    public void BecomeFocus(GameObject newFocus)
    {
        
        if(primaryContextualFocus == null){
            primaryContextualFocus = newFocus;
            Debug.Log("Become Focus - New primary focus: " + newFocus.name);
        }
        else{
            if(primaryContextualFocus.GetComponent<Interactable>().priority < newFocus.GetComponent<Interactable>().priority){
                secondaryContextualFocus = primaryContextualFocus;
                primaryContextualFocus = newFocus;
                Debug.Log("Become Focus - New primary focus: " + newFocus.name);
            }
            else{
                if(secondaryContextualFocus == null){
                    secondaryContextualFocus = newFocus;
                    Debug.Log("Become Focus - New secondary focus: " + newFocus.name);
                }
                else if(secondaryContextualFocus.GetComponent<Interactable>().priority < newFocus.GetComponent<Interactable>().priority){
                    secondaryContextualFocus = newFocus;
                    Debug.Log("Become Focus - New secondary focus: " + newFocus.name);
                }
            }
        }
        
    }

    public void Interact()
    {
        if (canFocus)
        {
            if (primaryContextualFocus != null)
            {
                if(primaryContextualFocus.GetComponent<Interactable>().isInteractable){
                    mostRecentAnimation = "FailedInteract1";
                    primaryContextualFocus.GetComponent<Interactable>().interact();
                }
            }
        }

    }

    public void InteractSecondary()
    {
        if (canFocus)
        {
            if (secondaryContextualFocus != null)
            {
                if(secondaryContextualFocus.GetComponent<Interactable>().isInteractable){
                    mostRecentAnimation = "FailedInteract2";
                    secondaryContextualFocus.GetComponent<Interactable>().interact();
                }
            }
        }
    }

}
