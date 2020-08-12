using System;
using System.Collections.Generic;
using ProjectBlocky.GameFlow;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	public class PlayerManager : MonoBehaviour {
		#region Singleton Instance
		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		public static PlayerManager Instance {
			get {
				if (!hasInstance) {
					instance = (PlayerManager)FindObjectOfType(typeof(PlayerManager));
					if (instance == null) {
						Debug.LogError("[PlayerManager] Cannot find an instance of PlayerManager!");
						return null;
					}
					hasInstance = true;
				}
				return instance;
			}
		}
		private static PlayerManager instance;
		private static bool hasInstance = false;
        #endregion

        #region Fields & Properties
        // ----------------------------------------------------------------------------------------------------
        [BoxGroup("Players")]
        [LabelText("Player One")]
        [SerializeField, Required]
        private GameObject playerOnePrefab = null;

        [BoxGroup("Players")]
        [LabelText("Player Two")]
        [SerializeField, Required]
        private GameObject playerTwoPrefab = null;

		public PlayerController PlayerOne => this.playerOne;
		private PlayerController playerOne;

		public PlayerController PlayerTwo => this.playerTwo;
		private PlayerController playerTwo;

		private PlayerStateController playerOneState;
		private PlayerStateController playerTwoState;

		public bool IsIsPlayerTwoActive => this.isPlayerTwoActive;
		private bool isPlayerTwoActive = false;

		public bool IsIsPlayerOneActive => this.isPlayerOneActive;
		private bool isPlayerOneActive = false;

		public List<PlayerController> PlayerList => this.playerList;
		private readonly List<PlayerController> playerList = new List<PlayerController>(2);

		public List<PlayerController> ActivePlayers => this.activePlayers;
		private readonly List<PlayerController> activePlayers = new List<PlayerController>(2);

		public List<Transform> ActivePlayerTransforms => this.activePlayerTransforms;
		private readonly List<Transform> activePlayerTransforms = new List<Transform>(2);

		public bool IsSingleplayer => (this.isPlayerOneActive && !this.isPlayerTwoActive) || (this.isPlayerTwoActive && !this.isPlayerOneActive);

		public bool IsMultiplayer => this.isPlayerOneActive && this.isPlayerTwoActive;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Events
		// ----------------------------------------------------------------------------------------------------
		public event Action<PlayerController> OnPlayerRegistered;
		public event Action<PlayerController> OnPlayerUnregistered;
		public event Action<PlayerController> OnPlayerChanged;

		public event Action<PlayerController, InteractionTarget> OnPlayerInteract;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			var playerContainer = new GameObject("Players");
			playerContainer.transform.position = this.transform.position;
			playerContainer.transform.rotation = Quaternion.identity;
			playerContainer.transform.localScale = Vector3.one;
			//playerContainer.transform.SetParent(this.transform);

			this.playerOne = GameObject.Instantiate(this.playerOnePrefab, playerContainer.transform, false).GetComponent<PlayerController>();
			this.playerOne.name = "Player One";
			this.playerOneState = this.playerOne.GetComponent<PlayerStateController>();
			this.playerOneState.Disable();

			this.playerTwo = GameObject.Instantiate(this.playerTwoPrefab, playerContainer.transform, false).GetComponent<PlayerController>();
			this.playerTwo.name = "Player Two";
			this.playerTwoState = this.playerTwo.GetComponent<PlayerStateController>();
			this.playerTwoState.Disable();

			this.playerList.Add(this.playerOne);
			this.playerList.Add(this.playerTwo);


			this.OnPlayerRegistered += (player) => { this.OnPlayerChanged?.Invoke(player); };
			this.OnPlayerUnregistered += (player) => { this.OnPlayerChanged?.Invoke(player); };

			this.playerOne.gameObject.SetActive(false);
			this.playerTwo.gameObject.SetActive(false);

			this.playerOne.GetComponent<InteractionController>().OnInteract += (pc, it) => {
				this.OnPlayerInteract?.Invoke(pc, it);
			};

			this.playerTwo.GetComponent<InteractionController>().OnInteract += (pc, it) => {
				this.OnPlayerInteract?.Invoke(pc, it);
			};
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Player Registration
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Registers a player.
		/// </summary>
		/// <param name="playerType">The player type.</param>
		public void RegisterPlayer(PlayerType playerType) {
			switch (playerType) {
				case PlayerType.PlayerTwo:
					AddPlayerTwo(this.playerTwo);
					break;
				default:
					AddPlayerOne(this.playerOne);
					break;
			}
		}

		public void UnregisterPlayer(PlayerType playerType) {
			switch (playerType) {
				case PlayerType.PlayerTwo:
					RemovePlayerTwo();
					break;
				default:
					RemovePlayerOne();
					break;
			}
		}

		public void UnregisterPlayer(PlayerController player) {
			this.UnregisterPlayer(player.PlayerType);
		}


		private void AddPlayerOne(PlayerController player) {
			this.playerOne = player;
			this.playerOne.gameObject.SetActive(true);
			this.isPlayerOneActive = true;
			if (!this.activePlayers.Contains(this.playerOne)) {
				this.activePlayers.Add(this.playerOne);
				this.activePlayerTransforms.Add(this.playerOne.transform);
				this.OnPlayerRegistered?.Invoke(player);
			}
		}

		private void AddPlayerTwo(PlayerController player) {
			this.playerTwo = player;
			this.playerTwo.gameObject.SetActive(true);
			this.isPlayerTwoActive = true;
			if (!this.activePlayers.Contains(this.playerTwo)) {
				this.activePlayers.Add(this.playerTwo);
				this.activePlayerTransforms.Add(this.playerTwo.transform);
				this.OnPlayerRegistered?.Invoke(player);
			}
		}

		private void RemovePlayerOne() {
			this.isPlayerOneActive = false;
			this.playerOne.gameObject.SetActive(false);
			this.activePlayers.Remove(this.playerOne);
			this.activePlayerTransforms.Remove(this.playerOne.transform);
			this.OnPlayerUnregistered?.Invoke(this.playerOne);
		}

		private void RemovePlayerTwo() {
			this.isPlayerTwoActive = false;
			this.playerTwo.gameObject.SetActive(false);
			this.activePlayers.Remove(this.playerTwo);
			this.activePlayerTransforms.Remove(this.playerTwo.transform);
			this.OnPlayerUnregistered?.Invoke(this.playerTwo);
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Player Spawning
		// ----------------------------------------------------------------------------------------------------
		public void SpawnPlayers() {
			if (this.isPlayerOneActive) {
				this.SpawnPlayer(this.playerOneState);
			}

			if (this.isPlayerTwoActive) {
				this.SpawnPlayer(this.playerTwoState);
			}
		}

		private void SpawnPlayer(PlayerStateController player) {
			player.Respawn(0.5f);
		}

		public void TeleportPlayer(PlayerStateController player, Transform targetTransform, bool isWithinSection = true) {
			if (isWithinSection) {
				StartCoroutine(player.TeleportTo(targetTransform));
			}
			else {
				this.playerOneState.TeleportAway(targetTransform);
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
