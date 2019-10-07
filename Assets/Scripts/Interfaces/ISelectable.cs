using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{
    GameObject ISelectablegameObject //The base GameObject
    {
        get;
        set;
    }
    bool Selected //Is it selected
    {
        get;
        set;
    }
    bool FinalisedSelection //Is the selection finalised - Used for shift selection
    {
        get;
        set;
    }
    bool Circled //Is it circled
    {
        get;
        set;
    }
    bool ViewableOnly //Is the object viewable only
    {
        get;
    }

    void SelectControl(); //Method to check and apply circled/selected
}
