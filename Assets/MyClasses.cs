using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MyClasses : MonoBehaviour {
}

public class Game
{
    public string id;
    public string playerId;
    public List<string> collectedCharacters;
    public string startDate;
    //public string endDate;

    public Game() { }

    public Game(string id, string pId, List<string> collectedChar,string sDate)
    {
        this.id = id;
        this.playerId = pId;
        this.collectedCharacters = collectedChar;
        this.startDate= sDate;
    }

    public string toJson()
    {
        return JsonUtility.ToJson(this);
    }
}



public class Character
{
    public string id;
    public string name;
    public List<string> situations;
    public float minScoreToCollect;
    public double type;

    public Character()
    {

    }

    public Character(string id, string name, List<string> situations, float minScoreToCollect, int type)
    {
        this.id = id;
        this.name = name;
        this.situations = situations;
        this.minScoreToCollect = minScoreToCollect;
        this.type = type;
    }

    public string toJson()
    {
        return JsonUtility.ToJson(this);
    }
}
public class Situation
{
    public string id;
    public string title;
    public List<string> choices;

    public Situation() { }

    public Situation(string id, string title, List<string> choices)
    {
        this.id = id;
        this.title = title;
        this.choices = choices;
    }

    public string toJson()
    {
        return JsonUtility.ToJson(this);
    }
}

public class Choice
{
    public string id;
    public string choiceText;
    public float score;
    public string justification;

    public Choice() { }

    public Choice(string id, string choiceText, float score, string justification)
    {
        this.id = id;
        this.choiceText = choiceText;
        this.score = score;
        this.justification = justification;
    }

    public string toJson()
    {
        return JsonUtility.ToJson(this);
    }
}


public class Response
{
    public string id;
    public string choiceId;
    public string situationId;
    public float score;
    public string dateAdded;

    public Response() { }

    public Response(string id, string choiceId, string situationId, float score, string dateAdded)
    {
        this.id = id;
        this.choiceId = choiceId;
        this.situationId = situationId;
        this.score = score;
        this.dateAdded = dateAdded;

    }

    public string toJson()
    {
        return JsonUtility.ToJson(this);
    }
}


public class CollectedCharacter
{
    public string id;
    public string playerId;
    public string characterId;
    public string gameId;
    public float totalScore;
    public List<string> responses;
    public string dateCollected;


    public CollectedCharacter() { }

    public CollectedCharacter(string id, string playerId, string characterId, string gameId, 
                              float totalScore, List<string> responses, string dateCollected)
    {
        this.id = id;
        this.playerId = playerId;
        this.characterId = characterId;
        this.gameId = gameId;
        this.totalScore = totalScore;
        this.responses = responses;
        this.dateCollected = dateCollected;
    }

    public string toJson()
    {
        return JsonUtility.ToJson(this);
    }
}