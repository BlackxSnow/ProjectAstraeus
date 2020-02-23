using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MedicalUIIcon : MonoBehaviour
    {
        protected bool Initialised = false;

        public Image Background;
        public Image Foreground;
        public Image DurationBar;

        protected virtual void UpdateUI()
        {

        }
    } 
}
