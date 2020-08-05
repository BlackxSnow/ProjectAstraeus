using Items.Parts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interfaces
{
	public interface ICraftable
	{
		string PartGroup { get; }
		ItemPart CorePart { get; set; }
		void CalculateStats();
	} 
}
