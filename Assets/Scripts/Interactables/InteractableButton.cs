using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableButton : MonoBehaviour
{

    public bool Active;


    public Color DeactiveColor;
    public Color ActiveColor;
    public Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Active)
            renderer.material.color = ActiveColor;
        else
            renderer.material.color = DeactiveColor;

    }

    public void Interact()
    {
        Active = !Active;
    }
}
