#if UNITY_EDITOR
using System.Collections.Generic;
using ProjectBlocky.Actors;

namespace ProjectBlocky.GameFlow {
	public partial class LevelSection {
		#region Spawn Points
		// ----------------------------------------------------------------------------------------------------
		private readonly List<PlayerSpawnPoint> spawnPointList = new List<PlayerSpawnPoint>(10);

		/// <summary>
		/// Fetches all spawn points.
		/// </summary>
		private void FetchSpawnPoints() {
			this.GetComponentsInChildren<PlayerSpawnPoint>(true, spawnPointList);

			foreach (var spawnPoint in spawnPointList) {
				switch (spawnPoint.SpawnType) {
					case PlayerSpawnPoint.PlayerSpawnType.PlayerOne:
						this.spawnPointP1 = spawnPoint;
						break;
					case PlayerSpawnPoint.PlayerSpawnType.PlayerTwo:
						this.spawnPointP2 = spawnPoint;
						break;
					default:
						this.spawnPointSP = spawnPoint;
						break;
				}
			}
		}

		/// <summary>
		/// Validates the SinglePlayer spawn point.
		/// </summary>
		private bool ValidateSpawnPointSP(PlayerSpawnPoint spawnPoint) {
			if (this.spawnPointSP == null) {
				FetchSpawnPoints();
			}
			return this.spawnPointSP != null;
		}

		/// <summary>
		/// Validates the Player One spawn point.
		/// </summary>
		private bool ValidateSpawnPointP1(PlayerSpawnPoint spawnPoint) {
			if (this.spawnPointP1 == null) {
				FetchSpawnPoints();
			}
			return this.spawnPointP1 != null;
		}

		/// <summary>
		/// Validates the Player Two spawn point.
		/// </summary>
		private bool ValidateSpawnPointP2(PlayerSpawnPoint spawnPoint) {
			if (this.spawnPointP2 == null) {
				FetchSpawnPoints();
			}
			return this.spawnPointP2 != null;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Exit Blocks
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Validates the exit blocks.
		/// </summary>
		private bool ValidateExitBlocks(List<ExitBlock> exitBlockList) {
			this.GetComponentsInChildren<ExitBlock>(true, exitBlockList);

			return this.exitBlocks.Count > 0;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
#endif