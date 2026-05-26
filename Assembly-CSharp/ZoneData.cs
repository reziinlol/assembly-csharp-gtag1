using System;
using UnityEngine;

// Token: 0x020003B8 RID: 952
[Serializable]
public class ZoneData
{
	// Token: 0x0400221E RID: 8734
	public GTZone zone;

	// Token: 0x0400221F RID: 8735
	public string sceneName;

	// Token: 0x04002220 RID: 8736
	public float CameraFarClipPlane = 500f;

	// Token: 0x04002221 RID: 8737
	public GameObject[] rootGameObjects;

	// Token: 0x04002222 RID: 8738
	[NonSerialized]
	public bool active;
}
