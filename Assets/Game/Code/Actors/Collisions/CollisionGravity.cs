using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	[RequireComponent(typeof(CollisionTarget))]
	public class CollisionGravity : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether this instance is active.
		/// </summary>
		public bool IsActive {
			get { return this.isActive; }
			set { this.isActive = value; }
		}

		[BoxGroup("Settings")]
		[SerializeField]
		private bool isActive = true;

		/// <summary>
		/// Gets the gravity.
		/// </summary>
		public float Gravity => this.gravity;

		[BoxGroup("Settings")]
		[MinValue(0)]
		[SerializeField]
		private float gravity = 0;
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
