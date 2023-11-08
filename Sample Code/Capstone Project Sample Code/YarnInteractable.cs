using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;


public class YarnInteractable : Interactable
{

    public bool isCurrentConversation;
    public DialogueRunner dialogueRunner;
    public DialogueManager dialogueManager; 
    public string conversationStartNode;
    [SerializeField] GameObject convoCamera;
    public GameObject player;

    public Animator npcAnimator;
    
    Quaternion defaultRot;

    public NPC npc;

    public ItemInteractable itemInteractable;

    //Set in the Editor
    public DialogueManager.Character character;

    private void Start()
    {
        Debug.Assert(player.GetComponent<PlayerController>());
        Debug.Assert(npc);
        Debug.Assert(dialogueRunner);
        dialogueRunner.onDialogueComplete.AddListener(EndConversation);
        Debug.Assert(convoCamera != null, "Yarn interactable does not have an associated virtual camera assigned to convoCamera");
        defaultRot = transform.rotation;
    }
    public new void Update()
    {
        base.Update();
/*        if (npc.characterData.NumberOfWantedItems < 1)
        {
            //make the give interaction disabled
            itemInteractable.isInteractable = false;
        }
        else
        {
            //make the give interaction enabled. 
            itemInteractable.isInteractable = true;
        }*/
    }



    public DialogueManager.Character GetCharacter()
    {
        return character;
    }

    public void StartConversation()
    {
        StartCoroutine(convoStart());
    }

    IEnumerator convoStart(){


        dialogueManager.startConversation(character);
        isCurrentConversation = true;
        Debug.Log(gameObject.name + " is the starting conversation.");
        StartCoroutine(player.GetComponent<PlayerController>().SlerpTowards(gameObject.transform.position, 2f));

        if (!npc.ignore)
        {
            if (npcAnimator != null)
            {
                npcAnimator.SetBool("Talk", true);
            }
            yield return StartCoroutine(npc.SlerpTowards(player.transform.position, .4f,15f,0));
            convoCamera.SetActive(true);
            dialogueManager.mainVirtualCamera.SetActive(false);
            isInteractable = false;
            itemInteractable.isInteractable = false;
        }

        yield return null;
    }

    


    /// <summary>
    /// In a typical game, several changes would be triggered when beginning or ending dialogue, 
    /// such as changing UI mode and starting and stopping a speaking animation on the relevant character or similar.
    /// So it's sensible to have bookend functions that hold all this code,
    /// even if we won't be doing anything useful with EndConversation()until later.
    /// </summary>
    public void EndConversation()
    {
        if(!interfaceManager.isTrading && isCurrentConversation){
            StartCoroutine(convoEnd());
        }
        else if(isCurrentConversation){
            interfaceManager.yarnInteract = this;
        }
    }

    IEnumerator convoEnd(){
        Debug.Log(gameObject.name + " is the ending conversation.");
        isCurrentConversation = false;
        player.GetComponent<PlayerController>().EndSlerpTowards();
        if(!npc.ignore){
            convoCamera.SetActive(false);
            dialogueManager.mainVirtualCamera.SetActive(true);
            yield return new WaitForSeconds(.65f);
            yield return StartCoroutine(npc.SlerpTowards(defaultRot, .4f,15f,0));
            if(npcAnimator != null){
                npcAnimator.SetBool("Talk",false);
            }
            yield return new WaitForSeconds(.3f);
        }
        else{
            dialogueManager.mainVirtualCamera.SetActive(true);
            npc.ignore = false;
        }
        isInteractable = true;
        //itemInteractable.isInteractable = true;
        npc.OnWantedItemNumberChanged();
        yield return null;

    }







}
