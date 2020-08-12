using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	public class CollisionDamage : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the damage.
		/// </summary>
		public float Damage {
			get { return this.damage; }
		}

		[BoxGroup("Settings")]
		[MinValue(1)]
		[SerializeField]
		private float damage = 10;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
