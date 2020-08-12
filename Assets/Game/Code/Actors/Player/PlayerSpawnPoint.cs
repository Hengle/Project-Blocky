using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	public class PlayerSpawnPoint : MonoBehaviour {
		#region Enumerations
		// ----------------------------------------------------------------------------------------------------
		public enum PlayerSpawnType {
			Singleplayer,
			PlayerOne,
			PlayerTwo
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the type of the spawn.
		/// </summary>
		public PlayerSpawnType SpawnType => this.spawnType;

		[BoxGroup("Settings")]
		[SerializeField]
		private PlayerSpawnType spawnType = PlayerSpawnType.Singleplayer;

		private SpriteRenderer spriteRenderer;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			this.spriteRenderer = this.GetComponent<SpriteRenderer>();

			PlayerManager.Instance.OnPlayerChanged += (pc) => {
				UpdateSprite();
			};

			UpdateSprite();
		}

		private void UpdateSprite() {
			this.spriteRenderer.enabled = false;

			var manager = PlayerManager.Instance;
			switch (this.spawnType) {
				case PlayerSpawnType.PlayerOne:
				case PlayerSpawnType.PlayerTwo:
					this.spriteRenderer.enabled = manager.IsMultiplayer;
					break;
				default:
					this.spriteRenderer.enabled = manager.IsSingleplayer;
					break;
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Gizmo Methods
		// ----------------------------------------------------------------------------------------------------
		//private void OnDrawGizmos() {
		//	switch (spawnType) {
		//		case PlayerSpawnType.PlayerOne:
		//			Gizmos.DrawIcon(transform.position, "ProjectBlocky/PlayerSpawnP1.png", true);
		//			return;
		//		case PlayerSpawnType.PlayerTwo:
		//			Gizmos.DrawIcon(transform.position, "ProjectBlocky/PlayerSpawnP2.png", true);
		//			return;
		//		default:
		//			Gizmos.DrawIcon(transform.position, "ProjectBlocky/PlayerSpawnSP.png", true);
		//			return;
		//	}
		//}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
