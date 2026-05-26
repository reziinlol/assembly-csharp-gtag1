using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200116C RID: 4460
	[CreateAssetMenu(fileName = "GorillaButtonColorSettings", menuName = "ScriptableObjects/GorillaButtonColorSettings", order = 0)]
	public class ButtonColorSettings : ScriptableObject
	{
		// Token: 0x0400811C RID: 33052
		public Color UnpressedColor;

		// Token: 0x0400811D RID: 33053
		public Color PressedColor;

		// Token: 0x0400811E RID: 33054
		[Tooltip("Optional\nThe time the change will be in effect")]
		public float PressedTime;
	}
}
