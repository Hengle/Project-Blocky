using UnityEngine;
using System.Collections.Generic;

namespace FeatherWorks.Pooling {
	public class FeatherPoolInstance {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the pool item.
		/// </summary>
		public GameObject PrefabInstance { get; private set; }

		// Cached Interfaces
		private List<ISpawnable> cachedSpawnables = new List<ISpawnable>(10);
		private List<IDespawnable> cachedDespawnables = new List<IDespawnable>(10);
		private List<IResettable> cachedResettables = new List<IResettable>(10);
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="FeatherPoolInstance"/> class.
		/// </summary>
		public FeatherPoolInstance(GameObject prefabInstance) {
			PrefabInstance = prefabInstance;

			RecacheComponentList();
		}

		/// <summary>
		/// Recaches the component list.
		/// </summary>
		private void RecacheComponentList() {
			PrefabInstance.GetComponentsInChildren<IResettable>(true, cachedResettables);

			PrefabInstance.GetComponentsInChildren<ISpawnable>(true, cachedSpawnables);

			PrefabInstance.GetComponentsInChildren<IDespawnable>(true, cachedDespawnables);
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Invoke Cached Interfaces
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Invokes OnReset
		/// </summary>
		public void InvokeResettable() {
			//foreach (IResettable resettable in cachedResettables.Iterator<IResettable>()) {
			//	resettable.OnReset();
			//}

			float count = cachedResettables.Count;
			for (int i = 0; i < count; i++) {
				cachedResettables[i].ResetState();
			}
		}

		/// <summary>
		/// Invokes OnSpawning
		/// </summary>
		public void InvokeOnSpawning() {
			//foreach (ISpawnable poolable in cachedSpawnables.Iterator<ISpawnable>()) {
			//	poolable.OnSpawning();
			//}

			float count = cachedSpawnables.Count;
			for (int i = 0; i < count; i++) {
				cachedSpawnables[i].OnSpawning();
			}
		}

		/// <summary>
		/// Invokes OnSpawned
		/// </summary>
		public void InvokeOnSpawned() {
			//foreach (ISpawnable poolable in cachedSpawnables.Iterator<ISpawnable>()) {
			//	poolable.OnSpawned();
			//}

			float count = cachedSpawnables.Count;
			for (int i = 0; i < count; i++) {
				cachedSpawnables[i].OnSpawned();
			}
		}

		/// <summary>
		/// Invokes InvokeOnDespawning
		/// </summary>
		public void InvokeOnDespawning() {
			//foreach (IDespawnable poolable in cachedDespawnables.Iterator<IDespawnable>()) {
			//	poolable.OnDespawning();
			//}

			float count = cachedDespawnables.Count;
			for (int i = 0; i < count; i++) {
				cachedDespawnables[i].OnDespawning();
			}
		}

		/// <summary>
		/// Invokes OnDespawned
		/// </summary>
		public void InvokeOnDespawned() {
			//foreach (IDespawnable poolable in cachedDespawnables.Iterator<IDespawnable>()) {
			//	poolable.OnDespawned();
			//}

			float count = cachedDespawnables.Count;
			for (int i = 0; i < count; i++) {
				cachedDespawnables[i].OnDespawned();
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Public Methods
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the active.
		/// </summary>
		/// <param name="isActive">if set to <c>true</c> [is active].</param>
		public void SetActive(bool isActive = true) {
			this.PrefabInstance.SetActive(isActive);
		}

		/// <summary>
		/// Sets the transform.
		/// </summary>
		/// <param name="transform">The transform.</param>
		public void SetTransform(Transform transform, bool applyScale = true) {
			this.PrefabInstance.transform.position = transform.position;
			this.PrefabInstance.transform.rotation = transform.rotation;
			if (applyScale) {
				this.PrefabInstance.transform.localScale = transform.localScale;
			}
		}

		/// <summary>
		/// Sets the position.
		/// </summary>
		/// <param name="position">The position.</param>
		public void SetPosition(Vector3 position) {
			this.PrefabInstance.transform.position = position;
		}

		/// <summary>
		/// Sets the rotation.
		/// </summary>
		/// <param name="rotation">The rotation.</param>
		public void SetRotation(Quaternion rotation) {
			this.PrefabInstance.transform.rotation = rotation;
		}

		/// <summary>
		/// Sets the scale.
		/// </summary>
		/// <param name="scale">The scale.</param>
		public void SetScale(Vector3 scale) {
			this.PrefabInstance.transform.localScale = scale;
		}

		/// <summary>
		/// Resets the transform.
		/// </summary>
		public void ResetTransform() {
			Transform transform = this.PrefabInstance.transform;

			transform.localRotation = Quaternion.identity;
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}