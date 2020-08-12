using ProjectBlocky.UI;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	[RequireComponent(typeof(PlayerStateController))]
	public class PlayerController : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the type of the player.
		/// </summary>
		public PlayerType PlayerType => this.playerType;
		[BoxGroup("Player")]
		[SerializeField]
		private PlayerType playerType = PlayerType.PlayerOne;

		/// <summary>
		/// Gets the state controller.
		/// </summary>
		public PlayerStateController StateController => this.stateController;
		private PlayerStateController stateController;

		/// <summary>
		/// Gets or sets the player's name.
		/// </summary>
		public string Name {
			get { return this.playerName; }
			set { this.playerName = value; }
		}
		[BoxGroup("Player")]
		[InlineProperty(LabelWidth = 15)]
		[LabelText("Name")]
		[SerializeField]
		private string playerName = "Player";

		/// <summary>
		/// Gets the health.
		/// </summary>
		public ActorHealth Health {
			get { return this.health; }
		}
		[BoxGroup("Stats")]
		[InlineProperty(LabelWidth = 15)]
		[SerializeField]
		private readonly ActorHealth health = new ActorHealth();

		/// <summary>
		/// Gets or sets the score.
		/// </summary>
		[BoxGroup("Stats")]
		[MinValue(0)]
		[MaxValue(99999999)]
		[ShowInInspector]
		public int Score {
			get { return this.score; }
			set {
				this.score = Mathf.Clamp(value, 0, 99999999);
				this.OnScoreChanged?.Invoke(this.score);
			}
		}
		[HideInInspector]
		[SerializeField]
		private int score = 0;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Events
		// ----------------------------------------------------------------------------------------------------
		public event Action<PlayerController> OnPlayerDamaged;
		public event Action<int> OnScoreChanged;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		private void Awake() {
			this.stateController = this.GetComponent<PlayerStateController>();
		}

		/// <summary>
		/// Called upon enabling this behavior.
		/// </summary>
		private void OnEnable() {
			//PlayerManager.Instance.RegisterPlayer(this, this.playerType);
		}

		/// <summary>
		/// Called upon disabling this behavior.
		/// </summary>
		private void OnDisable() {
			//PlayerManager.Instance.UnregisterPlayer(this.playerType);
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Player Methods
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Damages the specified damage.
		/// </summary>
		public void Damage(float damage) {
			this.health.Health -= damage;
			this.OnPlayerDamaged?.Invoke(this);
		}

		/// <summary>
		/// Adds the score.
		/// </summary>
		/// <param name="addedScore">The added score.</param>
		public void AddScore(int addedScore) {
			if (addedScore == 0) {
				return;
			}
			this.Score = this.score + addedScore;
		}

		public void ResetHealth() {
			this.health.Health = this.health.MaxHealth;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Unity Update
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Fixeds the update.
		/// </summary>
		private void FixedUpdate() {
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
