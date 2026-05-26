using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001F2 RID: 498
public class VoxelMaterialManager : MonoBehaviour
{
	// Token: 0x06000D1C RID: 3356 RVA: 0x00047D0B File Offset: 0x00045F0B
	private void OnEnable()
	{
		this.SetLightingProfile(this.startingIndex);
	}

	// Token: 0x06000D1D RID: 3357 RVA: 0x00047D19 File Offset: 0x00045F19
	private void Update()
	{
		if (this._timeOfDayIndex != BetterDayNightManager.instance.currentTimeIndex)
		{
			this.UpdateMaterial();
		}
	}

	// Token: 0x06000D1E RID: 3358 RVA: 0x00047D38 File Offset: 0x00045F38
	private void UpdateMaterial()
	{
		string currentTimeOfDay = BetterDayNightManager.instance.currentTimeOfDay;
		if (string.IsNullOrEmpty(currentTimeOfDay))
		{
			return;
		}
		int num = this.lightmapNames.IndexOf(currentTimeOfDay);
		if (num < 0 || num >= this.lightingProfiles.Count)
		{
			return;
		}
		this.SetLightingProfile(num);
		this._timeOfDayIndex = BetterDayNightManager.instance.currentTimeIndex;
	}

	// Token: 0x06000D1F RID: 3359 RVA: 0x00047D94 File Offset: 0x00045F94
	private void SetLightingProfile(int index)
	{
		index = Mathf.Clamp(index, 0, this.lightingProfiles.Count - 1);
		VoxelMaterialManager.LightingProfile lightingProfile = this.lightingProfiles[index];
		Shader.SetGlobalVector("_Light_Direction", lightingProfile.direction);
		Shader.SetGlobalColor("_Light_Color", lightingProfile.color);
		Shader.SetGlobalColor("_Shadow_Color", lightingProfile.color * this.shadowBrightness);
		Shader.SetGlobalColor("_Backlight_Color", lightingProfile.color * this.backlightBrightness);
		this._timeOfDayIndex = index;
	}

	// Token: 0x04000FAF RID: 4015
	public Material[] voxelMats;

	// Token: 0x04000FB0 RID: 4016
	public List<string> lightmapNames;

	// Token: 0x04000FB1 RID: 4017
	public List<VoxelMaterialManager.LightingProfile> lightingProfiles;

	// Token: 0x04000FB2 RID: 4018
	[Range(0f, 1f)]
	[SerializeField]
	private float shadowBrightness = 0.3f;

	// Token: 0x04000FB3 RID: 4019
	[Range(0f, 1f)]
	[SerializeField]
	private float backlightBrightness = 0.2f;

	// Token: 0x04000FB4 RID: 4020
	[SerializeField]
	private int startingIndex = 2;

	// Token: 0x04000FB5 RID: 4021
	private int _timeOfDayIndex = -1;

	// Token: 0x020001F3 RID: 499
	[Serializable]
	public struct LightingProfile
	{
		// Token: 0x04000FB6 RID: 4022
		public Color color;

		// Token: 0x04000FB7 RID: 4023
		public Vector3 direction;
	}
}
