using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MyControllerScript : MonoBehaviour {

    public Sprite[] collectionProgress;
    public Image collectionGUICounter;
    public Image collectionGUITotal;
    GameObject startButton;
    GameObject characterUI;
    GameObject answerText; //not needed
    GameObject selectedModel;
    List<Transform> targets;
    Dictionary<string, Character> targetCharMap;
    Dictionary<string, int>  targetCollectedMap;
    int collectedCharCounter;


    // Use this for initialization
    void Start () {
        
        startButton = GameObject.Find("Canvas").transform.FindChild("StartButton").gameObject;
        characterUI = GameObject.Find("Canvas").transform.FindChild("CharacterCollectionUI").gameObject;
        answerText = GameObject.Find("Canvas").transform.FindChild("AnswerText").gameObject;
        targets = new List<Transform>();
        targetCharMap = new Dictionary<string, Character>();
        targetCollectedMap = new Dictionary<string, int>();
        collectedCharCounter = 0;

        foreach (Transform t in GameObject.Find("Targets").transform)
        {
            targets.Add(t);
            targetCharMap[t.name] = new Character();
            targetCollectedMap[t.name] = 0;
        }

        endGame();


    }
	
	// Update is called once per frame
	void Update () {

        /* if (Input.GetMouseButton(0))
         {
             Vector3 mousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
             Vector3 mousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);

             Vector3 mousePosF = Camera.main.ScreenToWorldPoint(mousePosFar);
             Vector3 mousePosN = Camera.main.ScreenToWorldPoint(mousePosNear);
             RaycastHit hit;

             if (Physics.Raycast(mousePosN, mousePosF - mousePosN, out hit))
             {
                selectedModel = hit.transform.gameObject;
                GameObject parentCard = hit.transform.parent.gameObject;
                if (targetCollectedMap[parentCard.name] == 0)
                {
                    GameObject.Find("FirebaseHandler").GetComponent<FirebaseHandlerScript>().loadCharacter(targetCharMap[parentCard.name].id);
                    answerText.gameObject.SetActive(false);
                }
            }

        }*/

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject parentCard = hit.transform.parent.gameObject;
                if (parentCard.name == "cardJoker")
                {
                    //1.check if already collected
                    if (targetCollectedMap[parentCard.name] == 0)
                    {
                        GameObject.Find("FirebaseHandler").GetComponent<FirebaseHandlerScript>().loadCharacter(targetCharMap[parentCard.name].id);
                        answerText.gameObject.SetActive(false);
                    }
                    else
                    {
                        answerText.GetComponent<Text>().text = "You have already collected this character!";
                        answerText.gameObject.SetActive(true);
                    }
                }
                else if (parentCard.name == "card2")
                {
                    if (targetCollectedMap[parentCard.name] == 0)
                    {
                        GameObject.Find("FirebaseHandler").GetComponent<FirebaseHandlerScript>().loadCharacter(targetCharMap[parentCard.name].id);
                        answerText.gameObject.SetActive(false);
                    }
                    else
                    {
                        answerText.GetComponent<Text>().text = "You have already collected this character!";
                        answerText.gameObject.SetActive(true);
                    }
                }
                else if (parentCard.name == "cardQueen")
                {
                    if (targetCollectedMap[parentCard.name] == 0)
                    {
                        GameObject.Find("FirebaseHandler").GetComponent<FirebaseHandlerScript>().loadCharacter(targetCharMap[parentCard.name].id);
                        answerText.gameObject.SetActive(false);
                    }
                    else
                    {
                        answerText.GetComponent<Text>().text = "You have already collected this character!";
                        answerText.gameObject.SetActive(true);
                    }
                }
                else if (parentCard.name == "cardJ")
                {
                    if (targetCollectedMap[parentCard.name] == 0)
                    {
                        
                        GameObject.Find("FirebaseHandler").GetComponent<FirebaseHandlerScript>().loadCharacter(targetCharMap[parentCard.name].id);
                        answerText.gameObject.SetActive(false);
                    }
                    else
                    {
                        answerText.GetComponent<Text>().text = "You have already collected this character!";
                        answerText.gameObject.SetActive(true);
                    }

                }
                else if (parentCard.name == "cardKing")
                {
                    if (targetCollectedMap[parentCard.name] == 0)
                    {
                        GameObject.Find("FirebaseHandler").GetComponent<FirebaseHandlerScript>().loadCharacter(targetCharMap[parentCard.name].id);
                        answerText.gameObject.SetActive(false);
                    }
                    else
                    {
                        answerText.GetComponent<Text>().text = "You have already collected this character!";
                        answerText.gameObject.SetActive(true);
                    }
                }
               
            }
            else
            {
                answerText.gameObject.SetActive(false);
                //characterUI.gameObject.SetActive(false);
            }
        }


        if(collectedCharCounter == targetCollectedMap.Count)
        {
            answerText.GetComponent<Text>().text = "Congratulations!!You collected all the characters!!";
            answerText.gameObject.SetActive(true);
        }

    }




    public void setGame()
    {
        List<Character> temp = GameObject.Find("FirebaseHandler").GetComponent<FirebaseHandlerScript>().getCharacters();
        answerText.gameObject.SetActive(false);
        foreach (Transform obj in targets)
            {
                Character selected = temp[Random.Range(0, temp.Count)];
                temp.Remove(selected);
                targetCharMap[obj.name] = selected;
                targetCollectedMap[obj.name] = 0;
                foreach (Transform child in obj)
                {
                    child.gameObject.SetActive(false);
                }
                obj.transform.FindChild("type" + selected.type).gameObject.SetActive(true);
                obj.transform.FindChild("type" + selected.type).gameObject.GetComponent<Animator>().Play("Idle");
            Debug.Log(obj.name);

            }
        collectedCharCounter = 0;
        Sprite text = collectionProgress[targetCharMap.Count];
        collectionGUITotal.sprite = text;
        text = collectionProgress[collectedCharCounter];
        collectionGUICounter.sprite = text;
        GameObject.Find("Canvas").transform.FindChild("CollectionGUI").gameObject.SetActive(true);
        temp.Clear();
       
    }


    public void endGame()
    {        
        foreach (Transform obj in targets)
        {
            foreach (Transform child in obj)
            {
                child.gameObject.SetActive(false);
            }
        }
        collectedCharCounter = 0;
        characterUI.gameObject.SetActive(false);
        answerText.transform.GetComponent<Text>().text = "";
        answerText.transform.gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.FindChild("CollectionGUI").gameObject.SetActive(false);

    }


    public void updateCollectionMap(string charId)
    {
        foreach (KeyValuePair<string, Character> element in targetCharMap)
        {
            
            if ( element.Value.id == charId)
            {
                targetCollectedMap[element.Key] = 1;
            }
        }
        collectedCharCounter++;
        Sprite text = collectionProgress[collectedCharCounter];
        collectionGUICounter.sprite = text;
        //GameObject.Find("Canvas").transform.FindChild("Counter").GetComponent<Text>().text =collectedCharCounter.ToString();
       // GameObject.Find("Canvas").transform.FindChild("Counter").gameObject.SetActive(true);
    }


    
    public void printAssosiations()
    {
        answerText.GetComponent<Text>().text = "";
        foreach (KeyValuePair<string,Character> element in targetCharMap)
        {
            answerText.GetComponent<Text>().text += element.Key+" , "+element.Value.name+"\n" ;
        }
        
    }
    
    
    public void selectAnimation(int i)
    {
        if (i == 0)
          selectedModel.GetComponent<Animator>().Play("Idle");
        else if (i == 1)
          selectedModel.GetComponent<Animator>().Play("Death");
        else if (i == 2)
          selectedModel.GetComponent<Animator>().Play("Shout");
    }

    public void collectCharacter()
    {
        startButton.gameObject.SetActive(false);
        characterUI.gameObject.SetActive(true);
    }

    public void FinishInteraction()
    {
        //collectButton.gameObject.SetActive(true);
        characterUI.transform.FindChild("ControlButtonPanel").FindChild("FinishButton").gameObject.SetActive(false);
        characterUI.gameObject.SetActive(false);
        //answerText.gameObject.SetActive(false);
        
    }

}
