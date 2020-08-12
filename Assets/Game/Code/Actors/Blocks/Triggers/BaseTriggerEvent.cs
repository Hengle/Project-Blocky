using System;
using System.Collections;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable]
	public abstract class BaseTriggerEvent : IBlockTriggerEvent {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
#if UNITY_EDITOR
		public GameObject Parent { get; set; }
#endif

		public abstract GameObject TileObject { get; }
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		public virtual void Initialize() { }
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Trigger Methods
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Triggers this event.
		/// </summary>
		public abstract void Trigger();

		/// <summary>
		/// Resets the state.
		/// </summary>
		public abstract void ResetState();
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
