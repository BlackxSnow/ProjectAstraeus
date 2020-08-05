using Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using UI.Control;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Crafting
{
	public class ItemSelection : MonoBehaviour
	{
#pragma warning disable 0649
		[SerializeField]
		private GameObject UI_ListContent;
		[SerializeField]
		private ToggleGroup ListToggleGroup;
		[SerializeField]
		private CraftingUI CraftingWindow;
#pragma warning restore 0649
		private async void Start()
		{
			await UIController.DataLoadedEvent.WaitAsync();
			CreateToggles();
			CheckReferences();
		}

		private void CheckReferences()
		{
			List<bool> checks = new List<bool>
			{
				UI_ListContent != null,
				ListToggleGroup != null,
				CraftingWindow != null
			};

			foreach(bool check in checks)
			{
				if(!check)
				{
					Debug.LogWarning("One or more serialized fields is null");
					return;
				}
			}
		}

		List<GameObject> ItemInstances = new List<GameObject>();
		private void CreateToggles()
		{
			for(int i = 0; i < UI_ListContent.transform.childCount; i++)
			{
				Destroy(UI_ListContent.transform.GetChild(i));
			}
			foreach(GameObject instance in ItemInstances)
			{
				Destroy(instance);
			}
			ItemInstances.Clear();

			List<ICraftable> craftables = FindCraftables();
			foreach(ICraftable craftable in craftables)
			{
				GameObject obj = Instantiate(UIController.ObjectPrefabs[UIController.ObjectPrefabsEnum.TogglePrefab], UI_ListContent.transform);
				UnityEngine.UI.Toggle toggleComponent = obj.GetComponent<UnityEngine.UI.Toggle>();
				Control.Toggle toggleScript = obj.GetComponent<Control.Toggle>();

				toggleComponent.group = ListToggleGroup;
				toggleScript.UI_Label.text = craftable.GetType().Name;
				toggleScript.ActivateAction = delegate { CraftingWindow.StartDesign(craftable); };
			}
		}

		private List<ICraftable> FindCraftables()
		{
			List<Type> types = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => s.GetTypes())
				.Where(p => typeof(ICraftable).IsAssignableFrom(p) && !p.IsInterface).ToList();

			List<ICraftable> craftables = new List<ICraftable>();
			foreach(Type type in types)
			{
				GameObject instance = new GameObject($"Temp_Crafting_Item_{type.Name}");
				instance.SetActive(false);
				ICraftable result = (ICraftable)instance.AddComponent(type);
				craftables.Add(result);
				ItemInstances.Add(instance);
			}

			return craftables;
		}
	} 
}
