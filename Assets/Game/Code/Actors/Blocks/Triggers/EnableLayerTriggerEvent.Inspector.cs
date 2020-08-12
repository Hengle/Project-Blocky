#if UNITY_EDITOR
using CreativeSpore.SuperTilemapEditor;
using ProjectBlocky.GameFlow;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectBlocky.Actors {
	public partial class EnableLayerTriggerEvent {
		#region Static Fields
		// ----------------------------------------------------------------------------------------------------
		private static readonly string[] IgnoreLayers = { "Background", "Objects" };
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Layer Selection
		// ----------------------------------------------------------------------------------------------------
		private static List<ValueDropdownItem<GameObject>> layerList;
		private IList<ValueDropdownItem<GameObject>> GetLayers() {
			if (layerList == null) {
				layerList = new List<ValueDropdownItem<GameObject>>(10);
			}

			if (this.Parent == null) {
				return layerList;
			}

			var tileMapGroup = this.Parent.GetComponentInParent<TilemapGroup>() ?? this.Parent.GetComponentInParent<LevelSection>().GetComponentInChildren<TilemapGroup>();
			var tileMaps = tileMapGroup.Tilemaps;

			layerList.Clear();
			foreach (var currentTileMap in tileMaps) {
				if (!IgnoreLayers.Contains(currentTileMap.name)) {
					layerList.Add(new ValueDropdownItem<GameObject>(currentTileMap.name, currentTileMap.gameObject));
				}
			}
			return layerList;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
#endif