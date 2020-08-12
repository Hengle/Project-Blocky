using Sirenix.OdinInspector;
using System;
using ProjectBlocky.Actors;
using UnityEngine;

namespace ProjectBlocky.UI {
	[Serializable, HideMonoScript]
	public class PlayerHUDController : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the player data controller.
		/// </summary>
		public PlayerController PlayerController {
			get { return this.playerController; }
		}
		[BoxGroup("Player"), ReadOnly, HideInEditorMode]
		[LabelText("Player Controller")]
		[SerializeField]
		private PlayerController playerController;


		/// <summary>
		/// Gets the name controller.
		/// </summary>
		public NameHUDController NameController {
			get {
				return this.nameController;
			}
		}
		[BoxGroup("Components"), Required]
		[InlineButton("FetchControllers", "Fetch")]
		[SerializeField]
		private NameHUDController nameController;

		/// <summary>
		/// Gets the health controller.
		/// </summary>
		public HealthHUDController HealthController {
			get { return this.healthController; }
		}
		[BoxGroup("Components"), Required]
		[InlineButton("FetchControllers", "Fetch")]
		[SerializeField]
		private HealthHUDController healthController;

		/// <summary>
		/// Gets the score controller.
		/// </summary>
		public ScoreHUDController ScoreController {
			get { return this.scoreController; }
		}
		[BoxGroup("Components"), Required]
		[InlineButton("FetchControllers", "Fetch")]
		[SerializeField]
		private ScoreHUDController scoreController;


        [BoxGroup("Panels"), Required]
        [LabelText("Player Panel")]
        [SerializeField]
        private Transform playerPanelTransform = null;

        [BoxGroup("Panels"), Required]
        [LabelText("Search Panel")]
        [SerializeField]
        private Transform searchPanelTransform = null;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			UpdatePanels();
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Events
		// ----------------------------------------------------------------------------------------------------
		public event Action<PlayerController> OnPlayerRegistered;
		public event Action<PlayerController> OnPlayerUnregistered;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Player Registration
		// ----------------------------------------------------------------------------------------------------
		public void RegisterPlayer(PlayerController player) {
			this.playerController = player;
			UpdatePanels();

			OnPlayerRegistered?.Invoke(player);
		}

		public void UnregisterPlayer() {
			var previousPlayerController = this.playerController;
			this.playerController = null;
			UpdatePanels();

			OnPlayerUnregistered?.Invoke(previousPlayerController);
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Update Methods
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Updates the panels in accordance of player presence.
		/// </summary>
		private void UpdatePanels() {
#if UNITY_EDITOR
			if (isQuitting) {
				return;
			}
#endif
			if (this.playerController != null) {
				this.playerPanelTransform.gameObject.SetActive(true);
				this.searchPanelTransform.gameObject.SetActive(false);
			}
			else {
				this.playerPanelTransform.gameObject.SetActive(false);
				this.searchPanelTransform.gameObject.SetActive(true);
			}
		}

#if UNITY_EDITOR
		private bool isQuitting = false;
		private void OnApplicationQuit() {
			isQuitting = true;
		}
#endif
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Fetching Methods
		// ----------------------------------------------------------------------------------------------------
#if UNITY_EDITOR
		private void FetchControllers() {
			if (this.nameController == null) {
				this.nameController = this.GetComponentInChildren<NameHUDController>();
			}
			if (this.healthController == null) {
				this.healthController = this.GetComponentInChildren<HealthHUDController>();
			}
			if (this.scoreController == null) {
				this.scoreController = this.GetComponentInChildren<ScoreHUDController>();
			}
		}
#endif
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
