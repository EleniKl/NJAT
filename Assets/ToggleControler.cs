using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class ToggleControler : MonoBehaviour {

    public ToggleGroup choices;
    public Text answerText;

	// Use this for initialization
	void Start () {
	
	}


    public int GetActiveToggle()
    {
        int result = 0;
        // get first active toggle (and actually there should be only one in a group)
        foreach (var item in choices.ActiveToggles())
        {
            //Debug.Log("Your answer was : "+ );
            if (item.name == "Toggle1")
            {
                //answerText.text = "You selected the 1st answer." ;
                result= 0;
            }
            else if (item.name == "Toggle2")
            {
                //answerText.text = "You selected the 2nd answer.";
               result = 1;
            }
            else if (item.name == "Toggle3")
            {
                //answerText.text = "You selected the 3rd answer.";
                result = 2;
            }
            //answerText.gameObject.SetActive(true);
            break;
        }
        return result;
    }
}
