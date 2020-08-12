using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
namespace ProjectBlocky.GameFlow {
	public partial class LevelManager {
		#region Preview Methods
		// ----------------------------------------------------------------------------------------------------
		[BoxGroup("Preview"), LabelText("Selected Section")]
		[ValueDropdown("GetLevelSections", FlattenTreeView = false)]
		[ShowInInspector, PropertyOrder(-1), ShowIf("HasLevelSections"), DisableInPlayMode]
		private LevelSection SelectedLevelSection {
			get {
				if (selectedLevelSection == null && this.levelSections.Count > 0) {
					this.selectedLevelSection = this.levelSections[0];
				}
				return this.selectedLevelSection;
			}
			set {
				this.selectedLevelSection = value; 

				this.DisableAllSections();
				this.selectedLevelSection.gameObject.SetActive(true);
			}
		}
		[SerializeField, HideInInspector]
		private LevelSection selectedLevelSection;

		private IList<LevelSection> GetLevelSections() {
			this.GetComponentsInChildren<LevelSection>(true, this.levelSections);
			return this.levelSections;
		}

		private bool HasLevelSections {
			get { return this.levelSections.Count > 0; }
		}
		// ----------------------------------------------------------------------------------------------------

		#endregion
	}
}
#endif