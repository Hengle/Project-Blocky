using Sirenix.OdinInspector;
using System;
using ProjectBlocky.Actors;
using UnityEngine;

namespace ProjectBlocky.UI {
	[Serializable, HideMonoScript]
	public class HUDManager : MonoBehaviour {
		#region Singleton Instance
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the HUDManager singleton instance.
		/// </summary>
		public static HUDManager Instance {
			get {
				if (!hasInstance) {
					instance = (HUDManager)FindObjectOfType(typeof(HUDManager));
					if (instance == null) {
						Debug.LogError("[HUDManager] Cannot find an instance of HUDManager!");
						return null;
					}
					hasInstance = true;
				}
				return instance;
			}
		}
		private static HUDManager instance;
		private static bool hasInstance = false;
        // ----------------------------------------------------------------------------------------------------
        #endregion

        #region Fields & Properties
        // ----------------------------------------------------------------------------------------------------
        [BoxGroup("Prefab Settings"), Required]
        [SerializeField]
        private GameObject playerPanelPrefab = null;

		/// <summary>
		/// Gets the HUD controller for Player One.
		/// </summary>
		public Transform PlayerOnePanel => this.playerOnePanel;
        [BoxGroup("Panel Parents"), Required]
        [SerializeField]
        private Transform playerOnePanel = null;

		/// <summary>
		/// Gets the HUD controller for Player Two.
		/// </summary>
		public Transform PlayerTwoPanel => this.playerTwoPanel;
        [BoxGroup("Panel Parents"), Required]
        [SerializeField]
        private Transform playerTwoPanel = null;

		private PlayerHUDController playerOneController;
		private PlayerHUDController playerTwoController;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			if (instance != null && instance != this) {
				Destroy(this.gameObject);
				Debug.LogWarning("[HUDManager] More than one instance of HUDManager found!");
				return;
			}
			instance = this;
			hasInstance = true;


			// Create Panels
			var playerPanel = GameObject.Instantiate(this.playerPanelPrefab, this.playerOnePanel, false);
			playerPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			this.playerOneController = playerPanel.GetComponent<PlayerHUDController>();

			playerPanel = GameObject.Instantiate(this.playerPanelPrefab, this.playerTwoPanel, false);
			playerPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			this.playerTwoController = playerPanel.GetComponent<PlayerHUDController>();

			// Handle Player registration
			PlayerManager.Instance.OnPlayerRegistered += this.RegisterPlayer;
			PlayerManager.Instance.OnPlayerUnregistered += UnregisterPlayer;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Player Registration
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Registers a player.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="playerType">The player type.</param>
		private void RegisterPlayer(PlayerController player) {
			switch (player.PlayerType) {
				case PlayerType.PlayerTwo:
					this.playerTwoController.RegisterPlayer(player);
					break;
				default:
					this.playerOneController.RegisterPlayer(player);
					break;
			}
		}

		private void UnregisterPlayer(PlayerController playerType) {
			switch (playerType.PlayerType) {
				case PlayerType.PlayerTwo:
					this.playerTwoController.UnregisterPlayer();
					break;
				default:
					this.playerOneController.UnregisterPlayer();
					break;
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
