using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using System.Collections.Generic;

public class FirebaseHandlerScript : MonoBehaviour
{

    DatabaseReference dbRef;
    DataSnapshot loadedData;
    protected List<Character> characters;

    private string currentGame;
    private string playerId;
    private Character currentLoadedCharacter;
    //private CollectedCharacter newCollectedCharacter;
    private int charSituationCounter;
    private Situation currentSituation;
    private List<Choice> currentChoices;
    private float currentCharacterScore;
    private List<string> currentResponses;

    public Text answerText;
    public Transform questionDialog;


    // Use this for initialization
    void Start()
    {

        string url = "https://my-firebase-test-88855.firebaseio.com/";
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(url);
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;

        currentGame = "";
        playerId = "";
        characters = new List<Character>();
        currentLoadedCharacter = new Character();
        //newCollectedCharacter = new CollectedCharacter();
        currentChoices = new List<Choice>();
        currentResponses = new List<string>();
        currentSituation = new Situation();
        charSituationCounter = 0;
        currentCharacterScore = 0.0f;

        getCharacters();


    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<Character> getCharacters()
    {

        //Get characters data from database.
        dbRef.Child("characters").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (var obj in snapshot.Children)
                {
                    characters.Add(JsonUtility.FromJson<Character>(obj.GetRawJsonValue()));
                }

            }
        });

        return characters;
    }

    protected void initialiseVariables()
    {
        //currentLoadedCharacter = new Character();
        //newCollectedCharacter = new CollectedCharacter();
        charSituationCounter = 0;
        //currentSituation = new Situation();
        currentChoices.Clear();
        currentResponses.Clear();
        currentCharacterScore = 0.0f;
    }
    


    public void loadCharacter(string characterId)
    {
        initialiseVariables();
        questionDialog.gameObject.SetActive(false);

        //Get character's data from database.
        dbRef.Child("characters").Child(characterId).GetValueAsync().ContinueWith(task =>
         {
             if (task.IsCompleted)
             {

                 DataSnapshot snapshot = task.Result;
                 //Get json string for specific character to be loaded and create Character object.
                 currentLoadedCharacter = JsonUtility.FromJson<Character>(snapshot.GetRawJsonValue());

                 //Set first situation question in UI-Dialog.
                 //charSituationCounter = 0;
                 //currentCharacterScore = 0.0f;
                 setSituationUI(currentLoadedCharacter.situations[charSituationCounter]);
                 Debug.Log("Score now is: " + currentCharacterScore);
             }
             else
             {
                 Debug.Log("Task was not successful.");
             }
         });


    }


    private void setSituationUI(string situation)
    {
        //Get situation data from database.
        dbRef.Child("situations").Child(situation).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                //Get json string for situation to be loaded and create Situation object.
                Situation sitData = JsonUtility.FromJson<Situation>(snapshot.GetRawJsonValue());
                currentSituation = sitData;
                //Set question title in UI-Dialog.
                questionDialog.FindChild("Question").FindChild("Text").GetComponent<Text>().text = sitData.title;

                //Get the data for choices. (now only 3 choices..might be dynamic)
                
                dbRef.Child("choices").Child(sitData.choices[0]).GetValueAsync().ContinueWith(task1 =>
                {
                    
                    if (task1.IsCompleted)
                    {
                        DataSnapshot snapshot1 = task1.Result;
                        Choice choData = JsonUtility.FromJson<Choice>(snapshot1.GetRawJsonValue());
                        currentChoices.Add(choData);

                        questionDialog.FindChild("Choices").FindChild("Choice1").FindChild("Toggle1").FindChild("Label").GetComponent<Text>().text = choData.choiceText;
                    }
                });

                dbRef.Child("choices").Child(sitData.choices[1]).GetValueAsync().ContinueWith(task1 =>
                {
                    
                    if (task1.IsCompleted)
                    {
                        DataSnapshot snapshot1 = task1.Result;
                        Choice choData = JsonUtility.FromJson<Choice>(snapshot1.GetRawJsonValue());
                        currentChoices.Add(choData);
                        questionDialog.FindChild("Choices").FindChild("Choice2").FindChild("Toggle2").FindChild("Label").GetComponent<Text>().text = choData.choiceText;
                    }
                });

                dbRef.Child("choices").Child(sitData.choices[2]).GetValueAsync().ContinueWith(task1 =>
                {
                    if (task1.IsCompleted)
                    {
                        DataSnapshot snapshot1 = task1.Result;
                        Choice choData = JsonUtility.FromJson<Choice>(snapshot1.GetRawJsonValue());
                        currentChoices.Add(choData);
                        questionDialog.FindChild("Choices").FindChild("Choice3").FindChild("Toggle3").FindChild("Label").GetComponent<Text>().text = choData.choiceText;
                    }
                });



            }
        });

        //Set next button and UI-Dialog active.
        questionDialog.FindChild("ControlButtonPanel").FindChild("NextButton").gameObject.SetActive(true);
        questionDialog.gameObject.SetActive(true);


    }



    public void getNextSituation()
    {
        saveResponse();

        //Increase situation counter to go to next situation.
        charSituationCounter++;

        //Get next situation and set the UI-Dialog.
        if (charSituationCounter < currentLoadedCharacter.situations.Count - 1)
        {
            setSituationUI(currentLoadedCharacter.situations[charSituationCounter]);
            //Debug.Log(currentLoadedCharacter.situations[charSitCounter]);
        }
        //If this is the last situation...
        else if (charSituationCounter == currentLoadedCharacter.situations.Count - 1)
        {
            //..get set UI-Dialog..
            setSituationUI(currentLoadedCharacter.situations[charSituationCounter]);
            //.. and activate Finish button/deactivate Next button.
            questionDialog.FindChild("ControlButtonPanel").FindChild("NextButton").gameObject.SetActive(false);
            questionDialog.FindChild("ControlButtonPanel").FindChild("FinishButton").gameObject.SetActive(true);
        }
        else
        {
            //completed questions
            Debug.Log("All questions are completed.");

        }



    }


    public void saveResponse()
    {
        string resId = dbRef.Child("responses").Push().Key;
        int toggleAnswer = questionDialog.FindChild("Choices").GetComponent<ToggleControler>().GetActiveToggle();
        Debug.Log(currentChoices[0].id+" , "+ currentChoices[1].id + " , "+currentChoices[2].id);
        Choice ch = currentChoices[toggleAnswer];
        currentCharacterScore += ch.score;
        Debug.Log("Score now is: " + currentCharacterScore);
        Response res = new Response(resId, ch.id, currentSituation.id, ch.score, System.DateTime.Now.ToString());
        currentResponses.Add(res.id);
        dbRef.Child("responses").Child(resId).SetRawJsonValueAsync(res.toJson());
        currentChoices.Clear();

    }


    public void finishCharacterCollection()
    {
        saveResponse();
        if (currentCharacterScore >= currentLoadedCharacter.minScoreToCollect)
        {
            answerText.text ="Congratulations, you won the character!!!!You scored " + currentCharacterScore + "/" + currentLoadedCharacter.minScoreToCollect + ".";
            answerText.gameObject.SetActive(true);
            string newCharId = dbRef.Child("collectedCharacters").Push().Key;
            CollectedCharacter newChar = new CollectedCharacter(newCharId, playerId, currentLoadedCharacter.id, currentGame, currentCharacterScore , currentResponses, System.DateTime.Now.ToString());
            dbRef.Child("collectedCharacters").Child(newCharId).SetRawJsonValueAsync(newChar.toJson());
            dbRef.Child("games").Child(currentGame).Child("collectedCharacters").Child(newCharId).SetRawJsonValueAsync(newChar.toJson());

            GameObject.Find("MyController").GetComponent<MyControllerScript>().updateCollectionMap(currentLoadedCharacter.id);
            //show animation for collection
        }
        else
        {
            
            answerText.text = "You scored " + currentCharacterScore + "/" + currentLoadedCharacter.minScoreToCollect + ". Sorry, you didn't get the character.";
            answerText.gameObject.SetActive(true);
            //Update database by removing added responses.
            foreach (string response in currentResponses)
            {
                dbRef.Child("responses").Child(response).RemoveValueAsync();
            }

            //show animation for no success.
        }
    }


    public void createNewGame()
    {
        currentGame = dbRef.Child("games").Push().Key;
        List<string> temp = new List<string>();
        Game newGame = new Game(currentGame, playerId,temp,System.DateTime.Now.ToString());
        dbRef.Child("games").Child(currentGame).SetRawJsonValueAsync(newGame.toJson());
    }


    /*
    public void addCollectedCharacter(string cId)
    {
        dbRef.Child("games").Child("-KoZdC8kT6l-tA59ejUh").Child("collectedCharacters").Push().SetValueAsync(cId);
    }
    */

    //Initialise database with some data.
    public void addData()
    {        
        List<string> temp = new List<string>();
        /*
        temp.Add("s1");
        temp.Add("s2");
        temp.Add("s3");
        Character elephant = new Character("char1", "elephant", temp, 7, 0);
        dbRef.Child("characters").Child(elephant.id).SetRawJsonValueAsync(elephant.toJson());
        temp.Clear();
        temp.Add("s4");
        temp.Add("s5");
        temp.Add("s6");
        Character rabbit = new Character("char2", "bunny", temp, 5, 1);
        dbRef.Child("characters").Child(rabbit.id).SetRawJsonValueAsync(rabbit.toJson());
        temp.Clear();
        temp.Add("s7");
        temp.Add("s8");
        temp.Add("s9");
        Character chan = new Character("char3", "chan", temp, 6.5f, 2);
        dbRef.Child("characters").Child(chan.id).SetRawJsonValueAsync(chan.toJson());
        temp.Clear();
        temp.Add("s7");
        temp.Add("s8");
        temp.Add("s9");
        Character mino = new Character("char4", "minotaur", temp, 6.5f, 3);
        dbRef.Child("characters").Child(mino.id).SetRawJsonValueAsync(mino.toJson());
        temp.Clear();
        temp.Add("s7");
        temp.Add("s8");
        temp.Add("s9");
        Character rhino = new Character("char5", "rhino", temp, 6.5f, 4);
        dbRef.Child("characters").Child(rhino.id).SetRawJsonValueAsync(rhino.toJson());
        
        temp.Clear();
        temp.Add("choice1");
        temp.Add("choice2");
        temp.Add("choice3");
        Situation sit1 = new Situation("s1", "The little elephant seems a bit sad...What would you do? ", temp);
        dbRef.Child("situations").Child(sit1.id).SetRawJsonValueAsync(sit1.toJson());
        temp.Clear();
        temp.Add("choice2");
        temp.Add("choice4");
        temp.Add("choice5");
        Situation sit2 = new Situation("s2", "Should we say something nice to the little elephant ? Like how pretty it looks today, for example ? ", temp);
        dbRef.Child("situations").Child(sit2.id).SetRawJsonValueAsync(sit2.toJson());
        temp.Clear();
        temp.Add("choice6");
        temp.Add("choice7");
        temp.Add("choice8");
        Situation sit3 = new Situation("s3","How is your day going?" , temp);
        dbRef.Child("situations").Child(sit3.id).SetRawJsonValueAsync(sit3.toJson());
        temp.Clear();
        temp.Add("choice1");
        temp.Add("choice3");
        temp.Add("choice4");
        Situation sit4 = new Situation("s4", "Look how happy the bunny is!Do you feel like doing something for that? ", temp);
        dbRef.Child("situations").Child(sit4.id).SetRawJsonValueAsync(sit4.toJson());
        temp.Clear();
        temp.Add("choice1");
        temp.Add("choice2");
        temp.Add("choice5");
        Situation sit5 = new Situation("s5", "Do you want to join the bunny and play with it?", temp);
        dbRef.Child("situations").Child(sit5.id).SetRawJsonValueAsync(sit5.toJson());
        temp.Clear();
        temp.Add("choice1");
        temp.Add("choice2");
        temp.Add("choice5");
        Situation sit6 = new Situation("s6","What do you feel like doing now?", temp);
        dbRef.Child("situations").Child(sit6.id).SetRawJsonValueAsync(sit6.toJson());
        temp.Clear();
        temp.Add("choice1");
        temp.Add("choice2");
        temp.Add("choice5");
        Situation sit7 = new Situation("s7", "Chan just said hello to you!!", temp);
        dbRef.Child("situations").Child(sit7.id).SetRawJsonValueAsync(sit7.toJson());
        temp.Clear();
        temp.Add("choice1");
        temp.Add("choice2");
        temp.Add("choice5");
        Situation sit8 = new Situation("s8", "Do you like talking with people?", temp);
        dbRef.Child("situations").Child(sit8.id).SetRawJsonValueAsync(sit8.toJson());
        temp.Clear();
        temp.Add("choice1");
        temp.Add("choice2");
        temp.Add("choice5");
        Situation sit9 = new Situation("s9", "Do you think you need to change that?", temp);
        dbRef.Child("situations").Child(sit9.id).SetRawJsonValueAsync(sit9.toJson());
        temp.Clear();
        temp.Add("choice11");
        temp.Add("choice7");
        temp.Add("choice16");
        Situation sit10 = new Situation("s10", "Will you be doing anything special today?", temp);
        dbRef.Child("situations").Child(sit10.id).SetRawJsonValueAsync(sit10.toJson());
        temp.Clear();
        temp.Add("choice10");
        temp.Add("choice12");
        temp.Add("choice17");
        Situation sit11 = new Situation("s11", "Oh no!The minotaur seems angry. What will you do?", temp);
        dbRef.Child("situations").Child(sit11.id).SetRawJsonValueAsync(sit11.toJson());
        temp.Clear();
        temp.Add("choice13");
        temp.Add("choice14");
        temp.Add("choice15");
        Situation sit12 = new Situation("s12", "Look at this big rhino!! Do you think he is a wild animal?", temp);
        dbRef.Child("situations").Child(sit12.id).SetRawJsonValueAsync(sit12.toJson());
        temp.Clear();
        temp.Add("choice9");
        temp.Add("choice13");
        temp.Add("choice15");
        Situation sit13 = new Situation("s13", "Do you want to be like that rhino? It looks like a tough guy!", temp);
        dbRef.Child("situations").Child(sit13.id).SetRawJsonValueAsync(sit13.toJson());
        */

        Choice cho1 = new Choice("choice1", "Tell his parents...", 3, "Feeling betrayed!");
        dbRef.Child("choices").Child(cho1.id).SetRawJsonValueAsync(cho1.toJson());
        Choice cho2 = new Choice("choice2", "Ignore and leave..", 4, "Abandoned and alone.");
        dbRef.Child("choices").Child(cho2.id).SetRawJsonValueAsync(cho2.toJson());
        Choice cho3 = new Choice("choice3", "Ask if everything is ok..", 6, "Someone cares for me!");
        dbRef.Child("choices").Child(cho3.id).SetRawJsonValueAsync(cho3.toJson());
        Choice cho4 = new Choice("choice4", "Shout at it because it's annoying!", 3, "It will make it feel even more alone!");
        dbRef.Child("choices").Child(cho4.id).SetRawJsonValueAsync(cho4.toJson());
        Choice cho5 = new Choice("choice5", "Yes, I should definitely do that!", 6, "It will make it happy!!");
        dbRef.Child("choices").Child(cho5.id).SetRawJsonValueAsync(cho5.toJson());
        Choice cho6 = new Choice("choice6", "I am really happy and full of energy.", 7, "---blouh blouh---");
        dbRef.Child("choices").Child(cho6.id).SetRawJsonValueAsync(cho6.toJson());
        Choice cho7 = new Choice("choice7", "Not that much..ordinary day.", 5, "---blouh blouh---");
        dbRef.Child("choices").Child(cho7.id).SetRawJsonValueAsync(cho7.toJson());
        Choice cho8 = new Choice("choice8", "I don't feel really well today.", 3, "---blouh blouh---");
        dbRef.Child("choices").Child(cho8.id).SetRawJsonValueAsync(cho8.toJson());
        Choice cho9 = new Choice("choice9", "No, I don't want to.", 3, "---blouh blouh---");
        dbRef.Child("choices").Child(cho9.id).SetRawJsonValueAsync(cho9.toJson());
        Choice cho10 = new Choice("choice10", "I want to go back home.", 5, "---blouh blouh---");
        dbRef.Child("choices").Child(cho10.id).SetRawJsonValueAsync(cho10.toJson());
        Choice cho11 = new Choice("choice11", "I want to go and play with my best friend.", 4, "---blouh blouh---");
        dbRef.Child("choices").Child(cho11.id).SetRawJsonValueAsync(cho11.toJson());
        Choice cho12 = new Choice("choice12", "I don't know..I don't want to do anything.", 3, "---blouh blouh---");
        dbRef.Child("choices").Child(cho12.id).SetRawJsonValueAsync(cho12.toJson());
        Choice cho13 = new Choice("choice13", "Yes, I do.", 7, "---blouh blouh---");
        dbRef.Child("choices").Child(cho13.id).SetRawJsonValueAsync(cho13.toJson());
        Choice cho14 = new Choice("choice14", "No, I don't.", 4, "---blouh blouh---");
        dbRef.Child("choices").Child(cho14.id).SetRawJsonValueAsync(cho14.toJson());
        Choice cho15 = new Choice("choice15", "It depends. Sometimes yes and sometimes no.", 5, "---blouh blouh---");
        dbRef.Child("choices").Child(cho15.id).SetRawJsonValueAsync(cho15.toJson());
        Choice cho16 = new Choice("choice16", "Maybe..I don't want to talk about it.", 3, "---blouh blouh---");
        dbRef.Child("choices").Child(cho16.id).SetRawJsonValueAsync(cho16.toJson());
        Choice cho17 = new Choice("choice17", "I should run and hide quickly", 2, "---blouh blouh---");
        dbRef.Child("choices").Child(cho17.id).SetRawJsonValueAsync(cho17.toJson());
    }

}

