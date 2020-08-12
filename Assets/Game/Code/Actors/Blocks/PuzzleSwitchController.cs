using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	[RequireComponent(typeof(ActorCollisionController))]
	[TypeInfoBox("This component acts as a target for puzzle blocks.")]
	public class PuzzleSwitchController : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether this switch is activated.
		/// </summary>
		public bool IsActivated {
			get { return this.isActivated; }
			set {
				var prevActive = this.isActivated;

				this.isActivated = value;

				if (this.isActivated != prevActive) {
					OnActivationChange?.Invoke(this);
				}
			}
		}
		private bool isActivated = false;

		private ActorCollisionController collisionController;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Events
		// ----------------------------------------------------------------------------------------------------
		public event Action<PuzzleSwitchController> OnActivationChange;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			this.collisionController = this.GetComponent<ActorCollisionController>();

			this.collisionController.OnTriggerCollision += (collisionTarget) => {
				var puzzleBlock = collisionTarget.GetComponent<PuzzleBlockController>();
				if (puzzleBlock != null) {
					if (CollisionUtils.IsObjectInsideTile(transform.position, puzzleBlock.transform.position)) {
						this.IsActivated = true;
						puzzleBlock.SetActivation(true);
					}
					else {
						this.IsActivated = false;
						puzzleBlock.SetActivation(false);
					}
				}
			};
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
