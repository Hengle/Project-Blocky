using System;
using System.Collections;
using FeatherWorks;
using UnityEngine;

namespace ProjectBlocky.Actors {
	public interface IBlockTriggerEvent : IResettable {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
#if UNITY_EDITOR
		GameObject Parent { get; set; }
#endif

		/// <summary>
		/// Gets the tile object.
		/// </summary>
		GameObject TileObject { get; }

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Triggers this event.
		/// </summary>
		void Trigger();
		// ----------------------------------------------------------------------------------------------------

		#endregion
	}
}
