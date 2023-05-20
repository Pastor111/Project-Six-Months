using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string Character;
    [TextArea(3, 10)]
    public string Text;
}

public class DialogueManager : MonoBehaviour
{
    public Dialogue[] dialogues;
    public float TextSpeed;

    public int current;
    bool isTalking;
    //bool accelerate;

    Dialogue current_D;

    // Start is called before the first frame update
    void Start()
    {
        current_D = new Dialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if(isTalking && Input.GetKeyDown(KeyCode.I))
        {
            NextDialogue();
        }
    }

    public void StartDialogue()
    {
        StartDialogue(0);
    }

    public void StartDialogue(int i)
    {
        if (isTalking)
            return;

        isTalking = true;
        Player.instance.SetDialogueInput(this);
        current = i;

        NextDialogue();
    }

    public Dialogue GetCurrentDialogue()
    {
        return current_D;
    }

    public void NextDialogue()
    {

        if (current >= dialogues.Length)
            EndDialogue();


        current++;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(dialogues[current - 1]));
    }

    public void EndDialogue()
    {
        StartCoroutine(END());
    }

    IEnumerator TypeSentence(Dialogue d)
    {
        string t = d.Text;

        current_D.Character = d.Character;


        int p = 0;

        while(p < t.Length)
        {
            p = (int)Mathf.MoveTowards(p, t.Length, TextSpeed * Time.deltaTime);

            current_D.Text = d.Text.Substring(0, p);

            yield return null;
        }



    }

    IEnumerator END()
    {
        yield return new WaitForEndOfFrame();
        Player.instance.SetDialogueInput(null);
        isTalking = false;
    }
}
