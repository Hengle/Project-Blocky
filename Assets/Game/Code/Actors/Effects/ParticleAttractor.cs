using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	public class ParticleAttractor : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the target transform.
		/// </summary>
		public Transform TargetTransform {
			get { return this.targetTransform; }
			set { this.targetTransform = value; }
		}
		[BoxGroup("Debug")]
		[SerializeField, ReadOnly]
		private Transform targetTransform;

		[BoxGroup("Settings")]
		[MinValue(0)]
		[SerializeField]
		private float forceDelay = 0.5f;

		private float currentForceDelay = 0;

		new private ParticleSystem particleSystem;

		private readonly ParticleSystem.Particle[] particles = new ParticleSystem.Particle[50];
		private int particleCount = 0;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			this.particleSystem = this.GetComponent<ParticleSystem>();
		}

		private void OnEnable() {
			this.currentForceDelay = this.forceDelay;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Unity Update
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Updates this instance.
		/// </summary>
		private void LateUpdate() {
			this.particleCount = this.particleSystem.GetParticles(this.particles);

			for (int i = 0; i < this.particleCount; i++) {
				var particle = this.particles[i];
				var lifetime = particle.startLifetime - particle.remainingLifetime;
				if (lifetime > forceDelay) {
					var lifetimeMultiplier = math.clamp(lifetime - forceDelay, 0, 1.5f) / 1.5f;
					var remainingDistance = (Vector2)particle.position - (Vector2)this.targetTransform.position;
					particle.velocity = (particle.velocity - new Vector3(remainingDistance.x * lifetimeMultiplier, remainingDistance.y * lifetimeMultiplier, 0)) * math.min(remainingDistance.magnitude * 1.75f, 1);

					this.particles[i] = particle;
				}
			}

			this.particleSystem.SetParticles(this.particles, this.particleCount);
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
