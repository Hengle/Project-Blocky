using System.Collections.Generic;
using UnityEngine;

namespace ProjectBlocky.Actors {
	public class TriggerEventGroup {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		private readonly List<IBlockTriggerEvent> triggerEvents;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="TriggerEventGroup"/> class.
		/// </summary>
		public TriggerEventGroup(List<IBlockTriggerEvent> triggerEvents) {
			this.triggerEvents = triggerEvents;

			this.Initialize();
		}

		public void Initialize() {
			foreach (var triggerEvent in this.triggerEvents) {
				triggerEvent.Initialize();
			}
		}

		public void Reset() {
			foreach (var triggerEvent in this.triggerEvents) {
				triggerEvent.ResetState();
			}
		}

		public void Invoke() {
			foreach (var triggerEvent in this.triggerEvents) {
				triggerEvent.Trigger();
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
