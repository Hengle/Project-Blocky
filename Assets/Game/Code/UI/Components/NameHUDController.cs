using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;

namespace ProjectBlocky.UI {
	[Serializable, HideMonoScript]
	public class NameHUDController : PlayerHUDComponent {
        #region Fields & Properties
        // ----------------------------------------------------------------------------------------------------
        [BoxGroup("Components"), Required]
        [SerializeField]
        private TextMeshProUGUI nameLabel = null;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			this.Initialize();

			this.playerHUDController.OnPlayerRegistered += (player) => {
				this.UpdateName(player.Name);
			};

			this.playerHUDController.OnPlayerUnregistered += (player) => {
				this.UpdateName("-");
			};

			var playerController = this.playerHUDController.PlayerController;
			if (playerController != null) {
				this.UpdateName(playerController.Name);
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Update HUD
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Updates the name.
		/// </summary>
		/// <param name="playerName">Name of the player.</param>
		private void UpdateName(string playerName) {
			this.nameLabel.text = playerName;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
