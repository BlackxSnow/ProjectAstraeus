using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IOwnable
{
    public int FactionID { get; set; }

    public string Name;

    public Animator animator;
    protected Renderer rendererComponent;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        Name = name;
        FactionID = 0;//Mathf.RoundToInt(Random.value * (FactionManager.Factions.Count - 1));
        rendererComponent = gameObject.GetComponentInChildren<Renderer>();
        if (rendererComponent)
            rendererComponent.material.color = FactionManager.Factions[FactionID].FactionColour; //Debug; Visually shows faction colour
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void Awake()
    {

    }
}
