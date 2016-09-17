using UnityEngine;
using System.Collections;
using Zenject;

namespace Algorithms
{
	public class ColumnUnitHooks : MonoBehaviour {
	     
		public class Factory : GameObjectFactory<ColumnUnitHooks>
		{
		}
	}
}