using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ProjectBlocky.GameFlow {
	[Serializable, HideMonoScript]
	[TypeInfoBox("This component automatically disables a section after initialization.")]
	public class LevelSectionInitializer : MonoBehaviour {
		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			this.gameObject.SetActive(false);
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
