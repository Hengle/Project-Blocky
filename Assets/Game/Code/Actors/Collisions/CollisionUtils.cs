using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace ProjectBlocky {
	[Serializable]
	public static class CollisionUtils {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		private static readonly Vector2 HalfVector = new Vector2(-0.5f, 0.5f);
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Tile Methods
		// ----------------------------------------------------------------------------------------------------
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float DistanceToTile(Vector2 objectPosition, Vector2 tilePosition) {
			return math.abs((objectPosition - tilePosition).sqrMagnitude);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsObjectInsideTile(Vector2 objectPosition, Vector2 tilePosition) {
			return (SnapToTile(objectPosition) - SnapToTile(tilePosition)).sqrMagnitude <= 0.01f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 SnapToTile(Vector2 tilePosition)
        {
            return new Vector2(math.round(tilePosition.x - 0.5f) + 0.5f, math.round(tilePosition.y - 0.5f) + 0.5f); //For centering in tile
            //return new Vector2(math.round(tilePosition.x), math.round(tilePosition.y));
        }
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
