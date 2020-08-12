using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectBlocky.Materials {
	[Serializable, HideMonoScript]
	[TypeInfoBox("This component controls the Highlight Material shader.")]
	public class HighlightMaterialController : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		private static readonly int HighlightAmountProperty = Shader.PropertyToID("_HighlightAmount");
		private const float HighlightSpeedMultiplier = 1.5f;

		private Material material;

		private static float highlightAmount = 0;
		private static bool countForward = true;
		private static bool hasBeenUpdated = false;

		private bool isOverridden = false;
		private float overwriteValue = 0;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			this.material = this.GetComponent<SpriteRenderer>().material;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Override Methods
		// ----------------------------------------------------------------------------------------------------
		public void SetOverrideValue(float value) {
			this.isOverridden = true;
			this.overwriteValue = value;
		}

		public void ResetOverride() {
			this.isOverridden = false;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Unity Update
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Updates this instance.
		/// </summary>
		private void FixedUpdate() {
			if (!hasBeenUpdated) {
				var highlightDelta = (Time.deltaTime * HighlightSpeedMultiplier);
				if (countForward) {
					highlightAmount = Mathf.Min(highlightAmount + highlightDelta, 1);
					if (highlightAmount >= 1) {
						countForward = false;
					}
				}
				else {
					highlightAmount = Mathf.Max(highlightAmount - highlightDelta, 0);
					if (highlightAmount <= 0) {
						countForward = true;
					}
				}
				hasBeenUpdated = true;
			}

			this.material.SetFloat(HighlightAmountProperty, this.isOverridden ? this.overwriteValue : highlightAmount);
		}

		private void LateUpdate() {
			hasBeenUpdated = false;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
