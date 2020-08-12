using ProjectBlocky.GameFlow;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	[RequireComponent(typeof(InteractionTarget))]
	public class ExitBlock : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the target section.
		/// </summary>
		public LevelSection TargetSection => this.targetSection;
		/// <summary>
		/// Gets the target section identifier.
		/// </summary>
		public int TargetSectionID => this.targetSection.SectionID;

        [SerializeField, Required]
        private LevelSection targetSection = null;

		private InteractionTarget interactionTarget;

		private PlayerController playerController;

		private bool isActive = true;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Events
		// ----------------------------------------------------------------------------------------------------
		public event Action<ExitBlock, PlayerStateController> OnExitReached;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			this.interactionTarget = this.GetComponent<InteractionTarget>();
			this.interactionTarget.OnInteraction += (pc) => {
				this.UseExit(pc.StateController);
			};
		}

		private void OnEnable() {
			this.isActive = true;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Exit Methods
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Uses the exit.
		/// </summary>
		public void UseExit(PlayerStateController player) {
			if (this.isActive) {
				this.OnExitReached?.Invoke(this, player);
				this.isActive = false;
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
