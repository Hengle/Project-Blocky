using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	public class ActorHealth {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the health.
		/// </summary>
		[MinValue(0)]
		[HorizontalGroup, HideLabel]
		[SuffixLabel("Health", true)]
		[ShowInInspector]
		public float Health {
			get { return this.health; }
			set {
				this.health = Mathf.Clamp(value, 0, this.maxHealth);
				this.OnHealthChanged?.Invoke(this);
			}
		}

		[HideInInspector]
		[SerializeField]
		private float health = 100;

		/// <summary>
		/// Gets or sets the maximum health.
		/// </summary>
		[MinValue(100)]
		[HorizontalGroup, LabelText("/")]
		[SuffixLabel("Max Health", true)]
		[ShowInInspector]
		public float MaxHealth {
			get { return this.maxHealth; }
			set {
				this.maxHealth = Mathf.Max(value, 100);
				this.OnMaxHealthChanged?.Invoke(this);

				if (this.health > this.maxHealth) {
					this.Health = this.maxHealth;
				}
			}
		}
		[HideInInspector]
		[SerializeField]
		private float maxHealth = 100;

		/// <summary>
		/// Gets the percentage of health.
		/// </summary>
		public float Percentage {
			get { return this.health / this.maxHealth; }
		}

#if UNITY_EDITOR
		[OnInspectorGUI]
		private void DrawHealthBar() {
			Rect rect = GUILayoutUtility.GetRect(18, 18);
			rect.x += 5;
			rect.width -= 5;

			var percentage = this.Percentage;
			UnityEditor.EditorGUI.ProgressBar(rect, percentage, String.Format("{0:N2}%", percentage * 100));
		}
#endif
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Events
		// ----------------------------------------------------------------------------------------------------
		public event Action<ActorHealth> OnHealthChanged;
		public event Action<ActorHealth> OnMaxHealthChanged;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="ActorHealth"/> class.
		/// </summary>
		public ActorHealth() {
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
