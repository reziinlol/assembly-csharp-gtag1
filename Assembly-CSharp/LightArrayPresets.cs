using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000413 RID: 1043
[CreateAssetMenu(fileName = "LightArrayPresets", menuName = "Scriptable Objects/LightArrayPresets")]
public class LightArrayPresets : ScriptableObject
{
	// Token: 0x060018D2 RID: 6354 RVA: 0x0008CD20 File Offset: 0x0008AF20
	private void initLookup()
	{
		this.lookup = new Dictionary<string, LightArrayPresets.LightArrayPreset>();
		for (int i = 0; i < this.presets.Length; i++)
		{
			this.lookup.Add(this.presets[i].name, this.presets[i]);
		}
	}

	// Token: 0x060018D3 RID: 6355 RVA: 0x0008CD6B File Offset: 0x0008AF6B
	public LightArrayPresets.LightArrayPreset GetPreset(int i)
	{
		return this.presets[i];
	}

	// Token: 0x060018D4 RID: 6356 RVA: 0x0008CD75 File Offset: 0x0008AF75
	public LightArrayPresets.LightArrayPreset GetPreset(string n)
	{
		if (this.lookup == null)
		{
			this.initLookup();
		}
		return this.lookup[n];
	}

	// Token: 0x040023FB RID: 9211
	private Dictionary<string, LightArrayPresets.LightArrayPreset> lookup;

	// Token: 0x040023FC RID: 9212
	[SerializeField]
	private LightArrayPresets.LightArrayPreset[] presets;

	// Token: 0x02000414 RID: 1044
	[Serializable]
	public class LightArrayPreset
	{
		// Token: 0x040023FD RID: 9213
		public string name = "Color";

		// Token: 0x040023FE RID: 9214
		public Color color = Color.white;

		// Token: 0x040023FF RID: 9215
		public float intensity = 1f;
	}
}
