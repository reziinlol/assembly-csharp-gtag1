using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x020008C7 RID: 2247
public class GorillaTurning : GorillaTriggerBox
{
	// Token: 0x06003AD2 RID: 15058 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Awake()
	{
	}

	// Token: 0x04004B3D RID: 19261
	public Material redMaterial;

	// Token: 0x04004B3E RID: 19262
	public Material blueMaterial;

	// Token: 0x04004B3F RID: 19263
	public Material greenMaterial;

	// Token: 0x04004B40 RID: 19264
	public Material transparentBlueMaterial;

	// Token: 0x04004B41 RID: 19265
	public Material transparentRedMaterial;

	// Token: 0x04004B42 RID: 19266
	public Material transparentGreenMaterial;

	// Token: 0x04004B43 RID: 19267
	public MeshRenderer smoothTurnBox;

	// Token: 0x04004B44 RID: 19268
	public MeshRenderer snapTurnBox;

	// Token: 0x04004B45 RID: 19269
	public MeshRenderer noTurnBox;

	// Token: 0x04004B46 RID: 19270
	public GorillaSnapTurn snapTurn;

	// Token: 0x04004B47 RID: 19271
	public string currentChoice;

	// Token: 0x04004B48 RID: 19272
	public float currentSpeed;
}
