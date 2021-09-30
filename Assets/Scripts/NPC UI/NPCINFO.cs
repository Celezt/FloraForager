using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCINFO : MonoBehaviour
{
    public DialogueTrigger trigger;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") == true)
        {
            trigger.StartDialogue();

        }
    }
}
