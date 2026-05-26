using System;
using UnityEngine;

// Token: 0x02000397 RID: 919
[ExecuteInEditMode]
public class SkyboxSettings : MonoBehaviour
{
	// Token: 0x06001651 RID: 5713 RVA: 0x000817CA File Offset: 0x0007F9CA
	private void OnEnable()
	{
		if (this._skyMaterial)
		{
			RenderSettings.skybox = this._skyMaterial;
		}
	}

	// Token: 0x0400206B RID: 8299
	[SerializeField]
	private Material _skyMaterial;
}
