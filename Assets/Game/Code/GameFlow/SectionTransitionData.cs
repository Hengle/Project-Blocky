namespace ProjectBlocky.GameFlow {
	public struct SectionTransitionData {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		public int TargetSectionID;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="SectionTransitionData"/> class.
		/// </summary>
		public SectionTransitionData(int targetSectionID) {
			this.TargetSectionID = targetSectionID;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
