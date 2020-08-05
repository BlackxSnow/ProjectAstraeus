using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Control
{
    public class DropdownHandler : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI UI_Title;
        [SerializeField]
        private TMP_Dropdown UI_Dropdown;
#pragma warning restore 0649

        public struct OptionData
        {
            public string name;
            public object value;
            public OptionData(string n, object v)
            {
                name = n;
                value = v;
            }
        }

        private List<OptionData> Options = new List<OptionData>();

        public delegate object GetDataDelegate();
        public delegate void SetDataDelegate(object data);

        public GetDataDelegate GetData { get; set; }
        public SetDataDelegate SetData { get; set; }

        private bool IsInitialised = false;
        public void Initialise(string title, GetDataDelegate getData, SetDataDelegate setData)
        {
            if (!IsInitialised)
            {
                UI_Title.text = title;
                GetData = getData;
                SetData = setData;
                IsInitialised = true;
            }
        }

        private void Start()
        {
            UI_Dropdown.onValueChanged.AddListener(OnValueChanged);
            CheckReferences();
        }

        private void CheckReferences()
        {
            List<bool> checks = new List<bool>
            {
                UI_Title != null,
                UI_Dropdown != null
            };

            foreach (bool check in checks)
            {
                if (!check)
                {
                    Debug.LogWarning("One or more serialized fields is null");
                    return;
                }
            }
        }

        private void OnValueChanged(int index)
        {
            if(SetData == null)
            {
                Debug.LogError($"Dropdown tried to call SetData, but no method was assigned.");
                return;
            }

            SetData(Options[index].value);
        }

        public void UpdateValues()
        {
            object val = GetData();
            UI_Dropdown.value = Options.IndexOf(Options.First(o => o.value == val));
        }

        public void SetOptions(params OptionData[] data)
        {
            Options.Clear();
            UI_Dropdown.ClearOptions();

            AddOptions(data);
        }

        public void AddOptions(params OptionData[] data)
        {
            List<TMP_Dropdown.OptionData> tmpOptions = new List<TMP_Dropdown.OptionData>();

            foreach(OptionData option in data)
            {
                Options.Add(option);
                tmpOptions.Add(new TMP_Dropdown.OptionData(option.name));
            }
            UI_Dropdown.AddOptions(tmpOptions);
        }
    } 
}
