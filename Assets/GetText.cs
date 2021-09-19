using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class GetText : MonoBehaviour
{
    public bool isTalking;

    //public Transform contentWindow;

    public GameObject recallTextObject;

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name == "NPC")
        {
            isTalking = true;
            Talk();
        }

    }

    public void Talk()

    {
        string readFromFilePath = Application.streamingAssetsPath + "/Recall_Chat/" + "Greeting" + ".txt";
        // Finding path
        List<string> fileLines = File.ReadAllLines(readFromFilePath).ToList(); //Reads from the whole text & puts it
                                                                               // in a list

        foreach (string line in fileLines) // Loop through the list
        {

            if (isTalking)
            {
                GameObject a = Instantiate(recallTextObject/*, contentWindow*/); // Puts the text in the window
                recallTextObject.GetComponent<Text>().text = line; // Getting every single line from the component
                Destroy(a);
                isTalking = false;
                
            }



        }


    }

}
