using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


//The following was a script made to create interactable objects in my capstone project
public class Interactable : MonoBehaviour
{ 
    //Variables and declarations
    public bool isInteractable = true;
    public string text = "";
    public InteractionManager interactionManager;
    public InterfaceManager interfaceManager;

    public UnityEvent InteractEvent;

    public UnityEvent EnterTriggerEvent;

    public UnityEvent LeaveTriggerEvent;
    //The larger the priority the higher chance it will have of showing up as an interact.
    public int priority = 0;
    

    // Start is called before the first frame update
    void Awake()
    {
        interactionManager = FindObjectOfType<InteractionManager>();
        interfaceManager = GameObject.Find("Interface Manager").GetComponent<InterfaceManager>();
        
    }

    // Update is called once per frame
    protected void Update()
    {
        if (interactionManager == null)
        {
            if (FindObjectOfType<InteractionManager>() != null)
            {
                interactionManager = FindObjectOfType<InteractionManager>();
            }
        }
        if(interfaceManager == null)
        {
            interfaceManager = GameObject.Find("Interface Manager").GetComponent<InterfaceManager>();
        }
    }

    public void SetIsInteractable(bool value)
    {
        isInteractable = value;
    }

    public void EnterInteractionTrigger()
    {
        if (EnterTriggerEvent != null)
        {
            EnterTriggerEvent.Invoke();
        }
            Debug.Log("Entering Interaction trigger for " + gameObject.name);
        if (interactionManager == null)
        {
            interactionManager = FindObjectOfType<InteractionManager>();
        }
        
        Debug.Assert(interactionManager);
        interactionManager.BecomeFocus(this.gameObject);
    }

    public void ExitInteractionTrigger()
    {
        if (LeaveTriggerEvent != null)
        {
            LeaveTriggerEvent.Invoke();
        }

        Debug.Log("Exiting Interaction trigger for " + gameObject.name);
        if (interactionManager == null)
        {
            interactionManager = FindObjectOfType<InteractionManager>();
        }
        Debug.Assert(interactionManager);
        interactionManager.RemoveFocus(this.gameObject);
    }
    public string get_text()
    {
        return text;
    }

    public void interact()
    {
        if(isInteractable) InteractEvent.Invoke();
    }


}
