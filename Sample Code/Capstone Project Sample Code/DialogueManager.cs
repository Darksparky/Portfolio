using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

//The following is the DialogeManage I made for my Capstone Project "Minutes to Map"
//I have left this file as is, I did not make any extra comments for the sake of my portfolio. 

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    [SerializeField] List<string> visitedNodes = new List<string>();
    public DialogueRunner dialogueRunner;
    public InputManager inputManager;
    public HandinGrid itemHandinGrid;
    InventoryController inventoryController;
    //Needs to be filled in in the inspector
    [SerializeField] List<GameObject> Characters = new List<GameObject>();
    public CameraController cameraController;
    public InterfaceManager interfaceManager;
    public QuestManager questManager;
    public GameObject mainVirtualCamera;
    public bool handin = false;

    public GameObject credits;

    public enum Character
    {
        Leader,
        Farmer,
        Archeologist,
        Immortal
    }

    public GameObject ContextButtonUI;
    public InteractionManager interactionManager;
    public GameObject player;

    static string currentYarnNode = "null";

    public CustomVariableStorage storage;
    //this is a test variable 
    static bool foundWantedLandmark = false;

    public GameObject fetchCharacter(string characterName){
        foreach(GameObject character in Characters){
            if(character.GetComponent<NPC>().name == characterName){
                return character;
            }
        }
        return null;
    }

    private void Awake()
    {
        inputManager = interfaceManager.inputManager;
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    public void OnSave()
    {
        GameManager gameManager = GameManager.Instance;
        Debug.Assert(gameManager, "Character Data couldn't save becaus gameManager was null");
        StorageData storageData = gameManager.storageData;
        (storageData.yarnFloats, storageData.yarnStrings, storageData.yarnBools) = storage.GetAllVariables();
        foreach(GameObject item in Characters)
        {
            item.GetComponent<NPC>().OnSave();
            
        }
        storageData.visitedNodes = visitedNodes;
    }

    public void OnLoad()
    {
        GameManager gameManager = GameManager.Instance;
        Debug.Assert(gameManager, "Character Data couldn't load becaus gameManager was null");
        StorageData sData = gameManager.storageData;
        storage.SetAllVariables(sData.yarnFloats, sData.yarnStrings, sData.yarnBools);
        visitedNodes = sData.visitedNodes;
    }

    // Start is called before the first frame update

    public List<string> GetVisitedNodes()
    {
        return visitedNodes;
    }
    void Start()
    {   
        storage = dialogueRunner.GetComponent<CustomVariableStorage>();
        inventoryController = FindObjectOfType<InventoryController>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (GameManager.Instance.Continuing)
        {
            OnLoad();
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentYarnNode = dialogueRunner.CurrentNodeName;

    }



    #region Dialogue Functions
    [YarnFunction("find_icon")]
    public static string FindIconIndex(string control)
    {
        
        switch(control){
            case "Interact":
                return "<sprite="+InputManager.instance.currentImgs.InteractIndex+">";
            case "InteractSecondary":
                return "<sprite="+InputManager.instance.currentImgs.InteractSecondaryIndex+">";
            case "Map":
                return "<sprite="+InputManager.instance.currentImgs.MapIndex+">";
            case "MapClose":
                return "<sprite="+InputManager.instance.currentImgs.MapCloseIndex+">";
            case "Inventory":
                return "<sprite="+InputManager.instance.currentImgs.InventoryIndex+">";
            case "InventoryClose":
                return "<sprite="+InputManager.instance.currentImgs.InventoryCloseIndex+">";
            case "PauseMenu":
                return "<sprite="+InputManager.instance.currentImgs.PauseMenuIndex+">";
            case "Click":
                return "<sprite="+InputManager.instance.currentImgs.ClickIndex+">";
            case "MoveCursor":
                return "<sprite="+InputManager.instance.currentImgs.MoveCursorIndex+">";
            default:
                return "<sprite=0>";
        }
    }

    [YarnCommand("TurnToPlayer")]
    public void TurnToPlayer(string character, bool value)
    {
        GameObject npcCharacter;
        npcCharacter = GameObject_CheckForCharacter(character);
        Debug.Assert(npcCharacter != null, "TurnToPlayer method failed to find an NPC character gameObject"); //This needs to always work, if it ever fails its a massive problem so I opted to assert instead of an if statement
        NPC npc = npcCharacter.GetComponent<NPC>();
        Debug.Assert(npc != null, "TurnToPlayer method failed to get the NPC script component off of the provided character's gameobject");
        if (value)
        {
            npc.SlerpTowards(player.transform.position, 2f, 0.3f);
        }
        else
        {
            npc.EndSlerpTowards();
        }
    }

    [YarnCommand("PlayCredits")]
    public void PlayCredits()
    {
        Debug.Log("Attempted!");
        interfaceManager.PauseAction();
        Instantiate(credits);
    }


    [YarnCommand("TalkAnimation")]
    public void TalkAnimation(string character, bool value)
    {
        Debug.Log(character + " called TalkAnimation with value " + value);
        GameObject npcCharacter;
        npcCharacter = GameObject_CheckForCharacter(character);
        Debug.Assert(npcCharacter!=null, "TalkAnimation method failed to find an NPC character gameObject");
        NPC npc = npcCharacter.GetComponent<NPC>();
        Debug.Assert(npc != null, "TalkAnimation method failed to get the NPC script component off of the provided character's gameobject");
        npc.animator.SetBool("Talk", value);
    }

    [YarnCommand("LoveAnimation")]
    public void LoveAnimation(string character)
    {
        Debug.Log(character + " called LoveAnimation");
        GameObject npcCharacter;
        npcCharacter = GameObject_CheckForCharacter(character);
        Debug.Assert(npcCharacter!=null, "LoveAnimation method failed to find an NPC character gameObject");
        NPC npc = npcCharacter.GetComponent<NPC>();
        Debug.Assert(npc != null, "LoveAnimation method failed to get the NPC script component off of the provided character's gameobject");
        npc.animator.SetTrigger("Love");
    }

    [YarnCommand("Ignore")]
    public void Ignore(string character)
    {
        Debug.Log(character + " called Ignore");
        GameObject npcCharacter;
        npcCharacter = GameObject_CheckForCharacter(character);
        Debug.Assert(npcCharacter!=null, "Ignore method failed to find an NPC character gameObject");
        NPC npc = npcCharacter.GetComponent<NPC>();
        Debug.Assert(npc != null, "Ignore method failed to get the NPC script component off of the provided character's gameobject");
        npc.ignore = true;
    }

    #endregion

    #region Node Tracker

    public void NodeComplete()
    {
        string nodeName = dialogueRunner.CurrentNodeName;
        visitedNodes.Add(nodeName);

        //We need to check if this node was in any of the character's queues
        //if it was then we can remove it from the queue. 
        foreach (GameObject c in Characters)
        {

            if (c.GetComponent<NPC>().characterData.dialogueNodeQueue.Count > 0)
            {
                if (c.GetComponent<NPC>().characterData.dialogueNodeQueue[0] == nodeName)
                {
                    Debug.Log("Trying to remove the first index");
                    c.GetComponent<NPC>().characterData.dialogueNodeQueue.RemoveAt(0);
                }
            }


        }

    }

    public bool WasVisited(string nodeName)
    {
        return visitedNodes.Contains(nodeName);
    }
    #endregion Node Tracker


    #region Check Functions

    private bool Bool_CheckForCharacter(string characterName)
    {
        bool reValue = false;
        foreach (GameObject c in Characters) {
            if (c.GetComponent<NPC>().characterData.characterName == characterName)
            {
                reValue = true;
            }
        }
        return reValue;
    }

    private GameObject GameObject_CheckForCharacter(string characterName)
    {
        GameObject reValue = null;
        foreach (GameObject c in Characters)
        {
            if (c.GetComponent<NPC>().characterData.characterName == characterName)
            {
                reValue = c;
            }
        }
        return reValue;
    }

    [YarnCommand("openHandin")]
    public  void open_item_handin(string character)
    {
        foreach (GameObject c in Characters)
        {
            if (c.GetComponent<NPC>().characterData.characterName == character)
            {
                itemHandinGrid.SetHandinOwner(c);
                //its important that the trade comes after the inventory
                interfaceManager.OnTrade(character);
            }
        }
    }
    
     public void AddDialogueNodeToQueue(NPC character, string dialogueNode)
    {
        foreach (GameObject c in Characters)
        {
            if (c.GetComponent<NPC>().characterData.characterName == character.characterData.characterName)
            {
                character.characterData.addToNodeQueue(dialogueNode);
            }
        }
    }

    public void AddDialogueNodeToQueue(QuestData qD)
    {
        foreach(GameObject c in Characters)
        {
            if(c.GetComponent<NPC>().characterData.characterName == qD.questGiver.characterName)
            {
                c.GetComponent<NPC>().characterData.addToNodeQueue(qD.DialogueNodeForQuestComplete);
            }
        }
    }

    /// <summary>
    /// Add a dialogue node to the queue;
    /// </summary>
    /// <param name="character">This is the character data for the character that you want to add the node to</param>
    /// <param name="dialogueNode">This needs to be written as the name of the node WITHOUT the character name</param>
    public void AddDialogueNodeToQueue(CharacterData character, string dialogueNode)
    {
        foreach (GameObject c in Characters)
        {
            if (c.GetComponent<NPC>().characterData.characterName == character.characterName)
            {
                c.GetComponent<NPC>().characterData.addToNodeQueue(character.characterName+dialogueNode);
            }
        }
    }



    public void OnItemHandinConfirmed(CharacterData characterData)
    {
        switch (characterData.characterName)
        {
            case "Leader":
                //StartDialogue("LeaderCheckForNodeItem");
                AddDialogueNodeToQueue(characterData, "CheckForNodeItem");
                break;
            case "Farmer":
                //StartDialogue("FarmerCheckForNodeItem");
                AddDialogueNodeToQueue(characterData, "CheckForNodeItem");
                break;
            case "Archeologist":
                //StartDialogue("ArcheologistCheckForNodeItem");
                AddDialogueNodeToQueue(characterData, "CheckForNodeItem");
                break;
            case "Immortal":
                //StartDialogue("ImmortalCheckForNodeItem");
                AddDialogueNodeToQueue(characterData, "CheckForNodeItem");
                break;
        }
    }

    [YarnCommand("determineFirstLine")]
    public void determineFirstLine(string character, string variable)
    {

        string firstLine = character + "Introduction";

        storage.SetValue(variable, firstLine);
       //Needs to fuck with a variable in yarn
    }

    [YarnCommand("determineTalkNode")]
    public string determineTalkNode(string character)
    {
        Debug.Log("Determining Talk Node");
        string talkNode = character + "Introduction";
        if(character == "Leader")
        {
            talkNode = "LeaderTalkedHusband";
        }
        CharacterData cData = null;
        foreach (GameObject c in Characters)
        {
            if(c.GetComponent<NPC>().characterData.characterName == character)
            {
                cData = c.GetComponent<NPC>().characterData;
                Debug.Log("cData = " + cData.characterName);
            }
        }
        if (cData!=null)
        {
            Debug.Log("cData!=null");
            List<Quest> questsByCharacter = questManager.QuestsByGiver(cData);
            List<string> nodes = new List<string>();
            if (questsByCharacter!=null)
            {
                Debug.Log("questsByCharacter!=null");
                foreach (Quest q in questsByCharacter)
                {
                    if (q.questData.goalOrder != null && q.questData.goalOrder.Count>1)
                    {
                        bool found = false;
                        foreach (string s in q.questData.goalOrder)
                        {
                            foreach (Goal g in q.questData.GetCombinedGoalList())
                            {
                                if (s == g.goalName && g.completed == false)
                                {
                                    if (q.questData.worldWasReset)
                                    {
                                        if(g.DialogueNodeForWorldReset!= null && g.DialogueNodeForWorldReset !="")
                                        {
                                            nodes.Add(g.DialogueNodeForWorldReset);
                                            found = true;
                                            break;
                                        }
                                        else
                                        {
                                        nodes.Add(AddInProgressNode(g));
                                        found = true;
                                        break;
                                        }

                                    }
                                    else
                                    {
                                        nodes.Add(AddInProgressNode(g));
                                        found = true;
                                        break;
                                    }
                                }
                            }
                            if (found)
                            {
                                break;
                            }
                        }

                    }
                    else
                    {
                        foreach (Goal g in q.questData.GetCombinedGoalList())
                        {
                            if (g.completed == false)
                            {
                                nodes.Add(AddInProgressNode(g));
                            }
                        }
                    }
                }
                if (nodes.Count >= 1)
                {
                    int choice = Mathf.FloorToInt(Random.Range(0, nodes.Count));
                    talkNode = nodes[choice];
                }
            }
            else
            {

                Debug.Log("Trying to get the next quest");
                QuestData nextQuest = questManager.DetermineNextQuest(cData);
                if(nextQuest!= null)
                {
                    if(nextQuest.DialogueNodeForQuestStart != null && nextQuest.DialogueNodeForQuestStart != "")
                    {
                        talkNode = nextQuest.DialogueNodeForQuestStart;   
                    }
                    else
                    {
                        Debug.LogError("DialogueNodeForQuestStart is null or blank on the Next quest in DialogueManager");
                    }
                    
                }
                else
                {
                    Debug.LogWarning("NextQuest is null in DialogueManager");
                }
            }  
        }
        else
        {
            Debug.LogError("determin talk node couldn't find character data");
        }
        

       // Debug.Log("character = " + character + ", variable = " + variable);
       
        if(talkNode!=null&& talkNode != "")
        {
            //storage.SetValue(variable, talkNode);
            return talkNode;
        }
        else
        {
            Debug.LogError("Couldn't find a talk node");
            //storage.SetValue(variable, character + "Introduction");
            return character+"Introduction";
        }
        
    }





    [YarnCommand("CheckForQuest")]
    public void YarnCheckForNextQuest(string characterName, string yarnvar)
    {
        Debug.Log("Yarn is checking for next quest");
        string talkNode = "null";
        CharacterData cData = null;
        foreach (GameObject c in Characters)
        {
            if (c.GetComponent<NPC>().characterData.characterName == characterName)
            {
                cData = c.GetComponent<NPC>().characterData;
                Debug.Log("cData = " + cData.characterName);
            }
        }
        if (cData != null)
        {
            Debug.Log("cData!=null");
            
            if (cData.dialogueNodeQueue.Count>1)
            {
                if (cData.dialogueNodeQueue[1] != dialogueRunner.CurrentNodeName && cData.dialogueNodeQueue[1] != null && cData.dialogueNodeQueue[1] !="")
                {
                    Debug.Log("Found dialogue node in queue [1] that isn't the currrent node of " + dialogueRunner.CurrentNodeName);
                    storage.SetValue(yarnvar, cData.dialogueNodeQueue[1]);
                    return;
                }
                else
                {
                    Debug.LogWarning("Dialogue node queue is broken yo.");
                }
            }
            else
            {
                List<Quest> questsByCharacter = questManager.QuestsByGiver(cData);
                List<string> nodes = new List<string>();
                QuestData nextQuest = questManager.DetermineNextQuest(cData);

                if (nextQuest != null)
                {
                    if (nextQuest.DialogueNodeForQuestStart != null && nextQuest.DialogueNodeForQuestStart != "")
                    {
                        talkNode = nextQuest.DialogueNodeForQuestStart;
                    }
                    else
                    {
                        Debug.LogError("DialogueNodeForQuestStart is null or blank on the Next quest in DialogueManager");
                    }

                }
                else
                {
                    Debug.LogWarning("NextQuest is null in DialogueManager");
                }
            }
        }

        storage.SetValue(yarnvar, talkNode);
        return;
    }
    [YarnCommand("CheckForContinuedDialogue")]
    public void YarnCheckContinuedDialogue(string characterName, string yarnvar)
    {
        Debug.Log("Yarn is checking for more nodes!" );
        string talkNode = "null";
        CharacterData cData = null;
        foreach (GameObject c in Characters)
        {
            if (c.GetComponent<NPC>().characterData.characterName == characterName)
            {
                cData = c.GetComponent<NPC>().characterData;
                Debug.Log("cData = " + cData.characterName);
            }
        }
        if (cData != null)
        {
            if (cData.dialogueNodeQueue.Count >= 1)
            {
                foreach(string n in cData.dialogueNodeQueue)
                {
                    if(n != dialogueRunner.CurrentNodeName && n!= null && n != "")
                    {
                        talkNode = n;
                        break;
                    }
                }
            }
        }
        storage.SetValue(yarnvar, talkNode);
        return;
    }






    [YarnCommand("giveQuest")]
    public void giveQuest (string questName)
    {
        var yesno = questManager.AddQuest(questName);
        if(yesno == false)
        {
            Debug.Log("Did not add quest");
        }
        
    }


    [YarnCommand("getQuestItemCount")]
    public void getQuestItemCount(string questName, string itemName, string variableName)
    {
        QuestData questData = questManager.GetQuestDataByString(questName);
        int needed = questManager.checkForItemCountAsQuestGoal(questData, itemName);
        if (needed > 0)
        {
            storage.SetValue(variableName, (float)needed);
        }

    }

    private string AddInProgressNode(Goal goal){
        if(!goal.goalProgressLine){
            goal.goalProgressLine = true;
            return goal.DialogueNodesForGoalInProgress[0];
        }
        else{
            return goal.DialogueNodesForGoalInProgress[Random.Range(0,goal.DialogueNodesForGoalInProgress.Length)];
        }
    }

    public void startConversation(DialogueManager.Character character)
    {
        
        switch (character)
        {
            //Leader Dialogue Logic
            case DialogueManager.Character.Leader:
                if (visitedNodes.Contains("FarmerIntroduction")==false)
                {
                    StartDialogue("LeaderIntroduction");
                    
                }
                else
                {
                    //this is for testing and wont remain the same
                    if (visitedNodes.Contains("FarmerIntroduction"))
                    {
                        string nextNode = getNode("Leader", "$nextNode");
                        if (nextNode == "null")
                        {
                            nextNode = determineTalkNode("Leader");
                            if(nextNode == "LeaderTalkedHusband")
                            {
                                if (visitedNodes.Contains("LeaderImpactSiteComplete"))
                                {
                                    nextNode = "LeaderImpactSiteComplete";
                                }
                                else if (visitedNodes.Contains("LeaderTreasureQuestComplete"))
                                {
                                    nextNode = "LeaderHelping";
                                }
                                else if (visitedNodes.Contains("LeaderFoodQuestComplete"))
                                {
                                    nextNode = "LeaderSunGod";
                                }
                            }

                            StartDialogue(nextNode);
                        }
                        else
                        {
                            StartDialogue(nextNode);
                        }
                    }
                    
                }
                break;

            // Farmer Dialogue Logic
            case DialogueManager.Character.Farmer:
                if (visitedNodes.Contains("FarmerIntroduction")==false)
                {
                    StartDialogue("FarmerIntroduction");
                }
                else
                {
                    //this is for testing and wont remain the same
                    //StartDialogue("FarmerCheckForNode");
                    string nextNode = getNode("Farmer", "$nextNode");
                    if (nextNode == "null")
                    {
                        nextNode = determineTalkNode("Farmer");

                        if (nextNode == "FarmerIntroduction")
                        {
                            if (visitedNodes.Contains("FarmerLoveTreeComplete"))
                            {
                                nextNode = "FarmerLoveTreeComplete";
                            }
                            else if (visitedNodes.Contains("FarmerWellWaterComplete"))
                            {
                                nextNode = "FarmerFlowersGrowSlow";
                            }
                            else if (visitedNodes.Contains("FarmerPlaceToPicnicComplete"))
                            {
                                nextNode = "FarmerOverworked";
                            }
                            else if (visitedNodes.Contains("FarmerApplePieComplete"))
                            {
                                nextNode = "FarmerBaking";
                            }
                        }

                        StartDialogue(nextNode);
                    }
                    else
                    {
                        StartDialogue(nextNode);
                    }
                }
                break;

            // Archeologist Dialogue Logic
            case DialogueManager.Character.Archeologist:
                if (visitedNodes.Contains("FarmerIntroduction")==false)
                {
                    StartDialogue("ArcheologistIntroduction");

                }
                else
                {
                    //StartDialogue("ArcheologistCheckForNode");
                    string nextNode = getNode("Archeologist", "$nextNode");
                    if (nextNode == "null")
                    {
                        nextNode = determineTalkNode("Archeologist");
                        if (nextNode == "ArcheologistIntroduction")
                        {
                            if (visitedNodes.Contains("ArcheologistStonehengeComplete"))
                            {
                                nextNode = "ArcheologistStonehengeComplete";
                            }
                            else if (visitedNodes.Contains("ArcheologistMagicComplete"))
                            {
                                nextNode = "ArcheologistBusyWork";
                            }
                            else if (visitedNodes.Contains("ArcheologistLanguageComplete"))
                            {
                                nextNode = "ArcheologistTranslations";
                            }
                        }

                        StartDialogue(nextNode);
                    }
                    else
                    {
                        StartDialogue(nextNode);
                    }
                }
                break;

            //Immortal Dialogue Logic
            case DialogueManager.Character.Immortal:
                if (visitedNodes.Contains("FarmerIntroduction") == false)
                {
                    StartDialogue("ImmortalIntroduction");
                }
                else
                {
                    //StartDialogue("ImmortalCheckForNode");
                    string nextNode = getNode("Immortal", "$nextNode");
                    if (nextNode == "null")
                    {
                        nextNode = determineTalkNode("Immortal");
                        if (nextNode == "ImmortalIntroduction")
                        {
                            if (visitedNodes.Contains("ImmortalMagicTowerComplete"))
                            {
                                nextNode = "ImmortalMagicTowerComplete";
                            }
                            else if (visitedNodes.Contains("ImmortalOldCultComplete"))
                            {
                                nextNode = "ImmortalMoon";
                            }
                            else if (visitedNodes.Contains("ImmortalWrittenInStoneComplete"))
                            {
                                nextNode = "ImmortalGodsIntent";
                            }
                        }


                        StartDialogue(nextNode);
                    }
                    else
                    {
                        StartDialogue(nextNode);
                    }
                }
                break;
        }
    }

    public void StartDialogue(string node)
    {        
        dialogueRunner.StartDialogue(node);
        interfaceManager.OnDialogueStart();
        InputManager.instance.OnDialogueStart();
        //Time.timeScale = 0; its breaking the way dialogue works
    }
    
    //[YarnCommand("set_foundWantedLandmark")]
    public static void set_foundWantedLandmark(bool answer)
    {
        foundWantedLandmark = answer;
    }

    //[YarnCommand("get_foundWantedLandmark")]
    public static bool get_foundWantedLandmark()
    {
        return foundWantedLandmark;
    }
    #endregion

    [YarnCommand("getQuestResponse")]
    public void getQuestResponse()
    {

    }

    public void dialogueStop()
    {
        dialogueRunner.Stop();
    }

    /// <summary>
    /// This changes a variable in the yarn mem so it can jump to the correct node. 
    /// </summary>
    [YarnCommand("getNode")]
    public string getNode(string character, string variable)
    {
        GameObject characterInDialogue;
        string nodeToGoTo = "null";
        bool foundCharacter = false;
        //first thing we need to do is check who the character is
        foreach(GameObject c in Characters)
        {
            if(c.GetComponent<NPC>().characterData.characterName == character)
            {
                characterInDialogue = c;
                foundCharacter = true;
                //Then we need to check the the queue stack 
                int queueSize = characterInDialogue.GetComponent<NPC>().characterData.dialogueNodeQueue.Count;
                //If there are things in the queue list then we pick the first index
                if (queueSize > 0)
                {
                    nodeToGoTo = characterInDialogue.GetComponent<NPC>().characterData.dialogueNodeQueue[0];
                }
            }
        }

        if (foundCharacter==false)
        {    
            Debug.LogError("There was an issue finding the character passed into the getNode method in the DialogueManager. ");

        }


        if (nodeToGoTo == "null")
        {
            if (handin)
            {
                nodeToGoTo = character + "ItemHandin";
                handin = false;
            }
        }
        //Now we have to change the yarn variable so we can effectively return a value. 
        //storage.SetValue(variable, nodeToGoTo);
        return nodeToGoTo;
    }

    [YarnCommand("setHandin")]
    public void setHandin(bool value)
    {
        handin = value;
    }
}
