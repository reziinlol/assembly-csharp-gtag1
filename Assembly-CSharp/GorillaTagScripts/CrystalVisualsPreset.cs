using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000EF9 RID: 3833
	[CreateAssetMenu(fileName = "CrystalVisualsPreset", menuName = "ScriptableObjects/CrystalVisualsPreset", order = 0)]
	public class CrystalVisualsPreset : ScriptableObject
	{
		// Token: 0x06005F45 RID: 24389 RVA: 0x001EAB34 File Offset: 0x001E8D34
		public override int GetHashCode()
		{
			return new ValueTuple<CrystalVisualsPreset.VisualState, CrystalVisualsPreset.VisualState>(this.stateA, this.stateB).GetHashCode();
		}

		// Token: 0x06005F46 RID: 24390 RVA: 0x000028C5 File Offset: 0x00000AC5
		[Conditional("UNITY_EDITOR")]
		private void Save()
		{
		}

		// Token: 0x04006E04 RID: 28164
		public CrystalVisualsPreset.VisualState stateA;

		// Token: 0x04006E05 RID: 28165
		public CrystalVisualsPreset.VisualState stateB;

		// Token: 0x02000EFA RID: 3834
		[Serializable]
		public struct VisualState
		{
			// Token: 0x06005F48 RID: 24392 RVA: 0x001EAB60 File Offset: 0x001E8D60
			public override int GetHashCode()
			{
				int item = CrystalVisualsPreset.VisualState.<GetHashCode>g__GetColorHash|2_0(this.albedo);
				int item2 = CrystalVisualsPreset.VisualState.<GetHashCode>g__GetColorHash|2_0(this.emission);
				return new ValueTuple<int, int>(item, item2).GetHashCode();
			}

			// Token: 0x06005F49 RID: 24393 RVA: 0x001EAB98 File Offset: 0x001E8D98
			[CompilerGenerated]
			internal static int <GetHashCode>g__GetColorHash|2_0(Color c)
			{
				return new ValueTuple<float, float, float>(c.r, c.g, c.b).GetHashCode();
			}

			// Token: 0x04006E06 RID: 28166
			[ColorUsage(false, false)]
			public Color albedo;

			// Token: 0x04006E07 RID: 28167
			[ColorUsage(false, false)]
			public Color emission;
		}
	}
}
