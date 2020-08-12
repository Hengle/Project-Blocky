using Sirenix.OdinInspector;
using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace ProjectBlocky.UI {
	[Serializable, HideMonoScript]
	public class ScoreHUDController : PlayerHUDComponent {
		#region Static Fields
		// ----------------------------------------------------------------------------------------------------
		private static readonly int[] ScoreScales = {
			10000000,
			1000000,
			100000,
			10000,
			1000,
			100,
			10,
			1
		};

		private static readonly int ScoreGrowthFrames = 20;
		private static readonly int ScoreGrowthMinimum = 3;
        // ----------------------------------------------------------------------------------------------------
        #endregion

        #region Fields & Properties
        // ----------------------------------------------------------------------------------------------------
        [BoxGroup("Components"), Required]
        [SerializeField]
        private TextMeshProUGUI scoreLabel = null;

		private float currentScore = 0;
		private float displayScore = 0;
		private bool updateScore = false;

		private readonly StringBuilder scoreString = new StringBuilder(8);
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
				player.OnScoreChanged -= UpdateScore;
				player.OnScoreChanged += UpdateScore;

				ResetScore(player.Score);
			};

			this.playerHUDController.OnPlayerUnregistered += (player) => {
				player.OnScoreChanged -= UpdateScore;
			};

			var playerController = this.playerHUDController.PlayerController;
			if (playerController != null) {
				playerController.OnScoreChanged -= UpdateScore;
				playerController.OnScoreChanged += UpdateScore;

				ResetScore(playerController.Score);
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Update HUD
		// ----------------------------------------------------------------------------------------------------
		public void FixedUpdate() {
			if (updateScore) {
				if (this.displayScore > this.currentScore) {
					var remainder = this.displayScore - this.currentScore;
					var growthStep = GetGrowthSteps(remainder);
					if (remainder <= growthStep) {
						this.displayScore = this.currentScore;
						this.updateScore = false;
					}
					else {
						displayScore -= growthStep;
					}
				}
				else {
					var remainder = this.currentScore - this.displayScore;
					var growthStep = GetGrowthSteps(remainder);
					if (remainder <= growthStep) {
						this.displayScore = this.currentScore;
						this.updateScore = false;
					}
					else {
						displayScore += growthStep;
					}
				}

				UpdateScore();
			}
		}

		/// <summary>
		/// Gets the growth steps.
		/// </summary>
		/// <param name="scoreRemainder">The score remainder.</param>
		private float GetGrowthSteps(float scoreRemainder) {
			return Mathf.Max(scoreRemainder / (float)ScoreGrowthFrames, ScoreGrowthMinimum);
		}

		/// <summary>
		/// Resets the score.
		/// </summary>
		/// <param name="playerScore">The player score.</param>
		private void ResetScore(int playerScore) {
			this.currentScore = playerScore;
			this.displayScore = playerScore;
			UpdateScore();
		}

		/// <summary>
		/// Updates the score.
		/// </summary>
		public void UpdateScore() {
			UpdateScoreString();
			this.scoreLabel.text = scoreString.ToString();
		}

		/// <summary>
		/// Updates the score.
		/// </summary>
		/// <param name="newScore">The new score.</param>
		private void UpdateScore(int newScore) {
			this.currentScore = newScore;
			if (Math.Abs(this.displayScore - this.currentScore) > 1) {
				this.updateScore = true;
			}

			UpdateScore();
		}

		/// <summary>
		/// Updates the score string.
		/// </summary>
		private void UpdateScoreString() {
			scoreString.Clear();

			for (int i = 0; i < 8; i++) {
				var currentScale = ScoreScales[i];

				if (displayScore >= currentScale) {
					scoreString.AppendInvariant((uint)displayScore);
					return;
				}
				else {
					scoreString.Append("0");
				}
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
