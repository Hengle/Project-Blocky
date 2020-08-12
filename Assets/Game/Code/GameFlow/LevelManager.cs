using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using ProjectBlocky.Actors;
using UnityEngine;

namespace ProjectBlocky.GameFlow {
	[Serializable, HideMonoScript]
	public partial class LevelManager : MonoBehaviour {
		#region Singleton Instance
		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		public static LevelManager Instance {
			get {
				if (!hasInstance) {
					instance = (LevelManager)FindObjectOfType(typeof(LevelManager));
					if (instance == null) {
						Debug.LogError("[LevelManager] Cannot find an instance of LevelManager!");
						return null;
					}
					hasInstance = true;
				}
				return instance;
			}
		}
		private static LevelManager instance;
		private static bool hasInstance = false;
		#endregion

		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the active section.
		/// </summary>
		public LevelSection ActiveSection => activeSection;
		private LevelSection activeSection;
		private int activeSectionId = 0;

		[ListDrawerSettings(Expanded = true, IsReadOnly = true)]
		[SerializeField, ReadOnly]
		private List<LevelSection> levelSections = new List<LevelSection>();
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			var sectionAmount = this.levelSections.Count;
			for (int i = 0; i < sectionAmount; i++) {
				var levelSection = this.levelSections[i];
				levelSection.gameObject.SetActive(true);
				levelSection.Initialize(i);

				levelSection.OnSectionFinished += (sectionData) => {
					SetActiveSection(sectionData.TargetSectionID);
				};
			}
		}

		/// <summary>
		/// Called upon start this behavior.
		/// </summary>
		private void Start() {
			PlayerManager.Instance.RegisterPlayer(PlayerType.PlayerOne);
			this.SetActiveSection(0);
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Section Methods
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the active section.
		/// </summary>
		public void SetActiveSection(LevelSection section) {
			this.SetActiveSection(section.SectionID);
		}

		/// <summary>
		/// Sets the active section.
		/// </summary>
		public void SetActiveSection(int sectionId) {
			this.DisableAllSections();

			this.activeSection = this.levelSections[sectionId];
			this.activeSectionId = sectionId;

			this.activeSection.ActivateSection();
		}

		/// <summary>
		/// Disables the inactive sections.
		/// </summary>
		private void DisableInactiveSections() {
			var sectionAmount = this.levelSections.Count;
			for (int i = 0; i < sectionAmount; i++) {
				if (this.activeSectionId != i) {
					var levelSection = this.levelSections[i];
					levelSection.DisableSection();
				}
			}
		}

		/// <summary>
		/// Disables all sections.
		/// </summary>
		private void DisableAllSections() {
			var sectionAmount = this.levelSections.Count;
			for (int i = 0; i < sectionAmount; i++) {
				var levelSection = this.levelSections[i];
				levelSection.DisableSection();
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
