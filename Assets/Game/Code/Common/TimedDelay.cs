using Sirenix.OdinInspector;
using System;
using Unity.Mathematics;
using UnityEngine;

namespace ProjectBlocky {
	[Serializable, HideMonoScript]
	public class TimedDelay {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the delay time (in seconds).
		/// </summary>
		public float DelayTime {
			get { return this.delayTime; }
			set { this.delayTime = value; }
		}
		[SerializeField]
		private float delayTime;
		private float currentDelay = 0;

		public bool HasReachedZero => this.hasReachedZero;
		private bool hasReachedZero = false;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="DelayTime"/> class.
		/// </summary>
		public TimedDelay(float delayTime) {
			this.delayTime = delayTime;
		}

		/// <summary>
		/// Resets this delay.
		/// </summary>
		public void Reset() {
			this.currentDelay = this.delayTime;
			this.hasReachedZero = this.currentDelay >= 0;
		}

		/// <summary>
		/// Sets the delay.
		/// </summary>
		/// <param name="delaytime">The delaytime (in seconds).</param>
		/// <param name="doReset">if set to <c>true</c>, do a reset.</param>
		public void SetDelay(float delaytime, bool doReset = false) {
			this.delayTime = delaytime;
			if (doReset) {
				this.Reset();
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Update Methods
		// ----------------------------------------------------------------------------------------------------
		public void Update() {
			if (this.currentDelay >= 0) {
				this.currentDelay = math.max(this.currentDelay - Time.deltaTime, 0);
			}
			this.hasReachedZero = this.currentDelay >= 0;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
