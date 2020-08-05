using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.Control
{
    public class Toggle : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI ui_Label;
        public TextMeshProUGUI UI_Label { get => ui_Label; }

        [SerializeField]
        private UnityEngine.UI.Toggle toggleComponent;
        public UnityEngine.UI.Toggle ToggleComponent { get => toggleComponent; }
#pragma warning restore 0649

        public Action ActivateAction { get; set; }

        public void Activate()
        {
            ActivateAction();
        }
    } 
}
