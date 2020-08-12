using System.Collections;
using System.Collections.Generic;
using ProjectBlocky.Actors;
using UnityEngine;
using Rewired;
using Sirenix.OdinInspector;
using PlayerController = ProjectBlocky.Actors.PlayerController;
using System;
using Unity.Mathematics;

namespace ProjectBlocky.Actors
{
    [HideMonoScript]
    [RequireComponentWarning(typeof(PlayerController))]
    [RequireComponentWarning(typeof(MovementController))]
	[TypeInfoBox("This component controls the player's animations.")]
	public class InputController : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		private MovementController movementController;
		private PlayerController playerController;
		private PlayerStateController playerStateController;
		private Player player;

		public int AxisHorizontal => axisHorizontal;
		private int axisHorizontal;
		public int AxisVertical => axisVertical;
		private int axisVertical;
		public int ActionInteract => actionInteract;
		private int actionInteract;

		public bool IsInteracting => this.isInteracting;
		private bool isInteracting = false;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Events
		// ----------------------------------------------------------------------------------------------------
		public event Action<PlayerController> OnInteractButtonDown;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		private void Awake() {
			this.axisHorizontal = ReInput.mapping.GetActionId("Horizontal");
			this.axisVertical = ReInput.mapping.GetActionId("Vertical");
			this.actionInteract = ReInput.mapping.GetActionId("Interact");

			this.playerController = this.GetComponent<PlayerController>();
			this.playerStateController = this.GetComponent<PlayerStateController>();
			this.movementController = this.GetComponent<MovementController>();
			this.player = ReInput.players.Players[0];
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Unity Update
		// ----------------------------------------------------------------------------------------------------
		// Update is called once per frame
		private void FixedUpdate() {
			const float deadzone = 0.25f;
			const float exponent = 2f;

			Vector2 direction = Vector2.zero;
			this.isInteracting = false;

			if (playerStateController.IsAlive) {
				var horizontal = player.GetAxis(axisHorizontal);
				var horizontalSign = math.sign(horizontal);
				horizontal = math.abs(horizontal) >= deadzone ? math.abs(horizontal) : 0;

				var vertical = player.GetAxis(axisVertical);
				var verticalSign = math.sign(vertical);
				vertical = math.abs(vertical) >= deadzone ? math.abs(vertical) : 0;

				direction = new Vector2(math.pow(horizontal, exponent) * horizontalSign, math.pow(vertical, exponent) * verticalSign);

				if (player.GetButtonDown(actionInteract)) {
					this.isInteracting = true;
					this.OnInteractButtonDown?.Invoke(playerController);
				}
			}

			movementController.AddForceWithSpeedMultipler(direction, direction.magnitude);
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}