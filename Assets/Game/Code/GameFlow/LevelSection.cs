using CreativeSpore.SuperTilemapEditor;
using ProjectBlocky.Actors;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBlocky.GameFlow {
	[Serializable, HideMonoScript]
	public partial class LevelSection : MonoBehaviour {
		#region Static Fields
		// ----------------------------------------------------------------------------------------------------
		private static readonly WaitForSeconds TeleportExitCoroutine = new WaitForSeconds(2.0f);
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the section identifier.
		/// </summary>
		public int SectionID => sectionId;
		private int sectionId = 0;

		/// <summary>
		/// Gets a value indicating whether this instance is active.
		/// </summary>
		public bool IsActive => this.isActive;
		private bool isActive = false;

		/// <summary>
		/// Gets the tilemap gorup
		/// </summary>
		[ShowInInspector, Required]
		[BoxGroup("Section Settings")]
		public TilemapGroup TilemapGroup {
			get {
#if UNITY_EDITOR
				if (this.tilemapGroup == null) {
					this.tilemapGroup = this.GetComponentInChildren<TilemapGroup>();
				}
#endif
				return this.tilemapGroup;
			}
		}
		[SerializeField, HideInInspector]
		private TilemapGroup tilemapGroup;

		[BoxGroup("Spawn Points"), Required, ShowInInspector]
		[LabelText("Single Player")]
		[ValidateInput("ValidateSpawnPointSP")]
		public PlayerSpawnPoint SpawnPointSinglePlayer => this.spawnPointSP;
		[SerializeField, HideInInspector]
		private PlayerSpawnPoint spawnPointSP;

		[BoxGroup("Spawn Points"), Required, ShowInInspector]
		[LabelText("Player One")]
		[ValidateInput("ValidateSpawnPointP1")]
		public PlayerSpawnPoint SpawnPointPlayerOne => this.spawnPointP1;
		[SerializeField, HideInInspector]
		private PlayerSpawnPoint spawnPointP1;

		[BoxGroup("Spawn Points"), Required, ShowInInspector]
		[LabelText("Player Two")]
		[ValidateInput("ValidateSpawnPointP2")]
		public PlayerSpawnPoint SpawnPointPlayerTwo => this.spawnPointP2;
		[SerializeField, HideInInspector]
		private PlayerSpawnPoint spawnPointP2;


		/// <summary>
		/// A list of exit blocks.
		/// </summary>
		[ListDrawerSettings(Expanded = true, IsReadOnly = true, ShowIndexLabels = false)]
		[ValidateInput("ValidateExitBlocks", "There must be at least one ExitBlock!", InfoMessageType.Warning)]
		[SerializeField, ReadOnly, PropertyOrder(100)]
		private List<ExitBlock> exitBlocks = new List<ExitBlock>();
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Events
		// ----------------------------------------------------------------------------------------------------
		public event Action OnSectionActivated;
		public event Action<SectionTransitionData> OnSectionFinished;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes this section.
		/// </summary>
		public void Initialize(int id) {
			this.sectionId = id;

			foreach (var exitBlock in this.exitBlocks) {
				exitBlock.OnExitReached += InvokeSectionFinished;
			}
		}

		private void InvokeSectionFinished(ExitBlock reachedExit, PlayerStateController player) {
			StartCoroutine(TeleportToExit(reachedExit, player));
		}

		private IEnumerator TeleportToExit(ExitBlock reachedExit, PlayerStateController player) {
			PlayerManager.Instance.TeleportPlayer(player, reachedExit.transform, false);

			yield return TeleportExitCoroutine;

			this.OnSectionFinished?.Invoke(new SectionTransitionData(reachedExit.TargetSectionID));
			this.DisableSection();
		}

		/// <summary>
		/// Activates the section.
		/// </summary>
		public void ActivateSection() {
			this.isActive = true;
			this.gameObject.SetActive(true);

			PlayerManager.Instance.SpawnPlayers();

			this.OnSectionActivated?.Invoke();
		}

		/// <summary>
		/// Disables the section.
		/// </summary>
		public void DisableSection() {
			this.gameObject.SetActive(false);
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Spawn Points
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the spawn position for a specified player type.
		/// </summary>
		public Vector2 GetSpawnPosition(PlayerType playerType) {
			switch (playerType) {
				case PlayerType.PlayerTwo:
					return this.spawnPointP2.transform.position;
				default:
					if (PlayerManager.Instance.IsSingleplayer) {
						return this.spawnPointSP.transform.position;
					}
					else {
						return this.spawnPointP1.transform.position;
					}
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region ToString Implementation
		// ----------------------------------------------------------------------------------------------------
		public override string ToString() {
			return this.name;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
