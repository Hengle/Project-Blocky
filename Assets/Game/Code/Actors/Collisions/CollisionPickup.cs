using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	public class CollisionPickup : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the damage.
		/// </summary>
		public int Score {
			get { return this.score; }
		}

		[BoxGroup("Settings")]
		[MinValue(0)]
		[SerializeField]
		private int score = 0;
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
