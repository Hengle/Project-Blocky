using Sirenix.OdinInspector;
using System;
using ProjectBlocky.Actors;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectBlocky.UI {
	[Serializable, HideMonoScript]
	public class HealthHUDController : PlayerHUDComponent {
        #region Fields & Properties
        // ----------------------------------------------------------------------------------------------------
        [BoxGroup("Components"), Required]
        [SerializeField]
        private Image healthImage = null;

        [BoxGroup("Components"), Required]
        [SerializeField]
        private Image redHealthImage = null;

		[BoxGroup("Red Health Settings")]
		[LabelText("Hold Time")]
		[SerializeField]
		private float redHealthHoldTime = 1f;
		private float redHealthHoldDelay = 0;

		[BoxGroup("Red Health Settings")]
		[LabelText("Fade Time")]
		[SerializeField]
		private float redHealthFadeTime = 1f;
		private float redHealthFadeDelay = 0;

		private bool updateHealth = false;
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
				player.Health.OnHealthChanged -= UpdateHealth;
				player.Health.OnHealthChanged += UpdateHealth;

				ResetHealth(player.Health);
			};

			this.playerHUDController.OnPlayerUnregistered += (player) => {
				player.Health.OnHealthChanged -= UpdateHealth;
			};

			var playerController = this.playerHUDController.PlayerController;
			if (playerController != null) {
				playerController.Health.OnHealthChanged -= UpdateHealth;
				playerController.Health.OnHealthChanged += UpdateHealth;

				ResetHealth(playerController.Health);
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Update HUD
		// ----------------------------------------------------------------------------------------------------
		private void FixedUpdate() {
			if (updateHealth) {
				if (this.redHealthHoldDelay > 0) {
					this.redHealthHoldDelay = Mathf.Max(this.redHealthHoldDelay - Time.fixedDeltaTime, 0);
				}
				else {
					if (this.redHealthFadeDelay < this.redHealthFadeTime) {
						this.redHealthFadeDelay = Mathf.Clamp(this.redHealthFadeDelay + Time.fixedDeltaTime, 0, this.redHealthFadeTime);
						this.redHealthImage.fillAmount = Mathf.Lerp(this.redHealthImage.fillAmount, this.healthImage.fillAmount, this.redHealthFadeDelay);
					}
					else {
						updateHealth = false;
					}
				}
			}
		}

		/// <summary>
		/// Resets the health.
		/// </summary>
		/// <param name="healthComponent">The health component.</param>
		private void ResetHealth(ActorHealth healthComponent) {
			this.healthImage.fillAmount = healthComponent.Percentage;
			this.redHealthImage.fillAmount = this.healthImage.fillAmount;
			UpdateHealth(healthComponent);
		}

		/// <summary>
		/// Updates the health.
		/// </summary>
		/// <param name="healthComponent">The health component.</param>
		private void UpdateHealth(ActorHealth healthComponent) {
			var percentage = healthComponent.Percentage;

			if (percentage > this.redHealthImage.fillAmount) {
				this.healthImage.fillAmount = percentage;
				this.redHealthImage.fillAmount = percentage;
				updateHealth = false;
			}
			else {
				this.healthImage.fillAmount = percentage;
				this.redHealthHoldDelay = this.redHealthHoldTime;
				this.redHealthFadeDelay = 0;
				updateHealth = true;
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
