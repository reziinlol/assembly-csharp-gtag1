using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Pool;

// Token: 0x0200058C RID: 1420
public class PlayerColoredCosmetic : MonoBehaviour
{
	// Token: 0x06002403 RID: 9219 RVA: 0x000C16D8 File Offset: 0x000BF8D8
	public void Awake()
	{
		for (int i = 0; i < this.coloringRules.Length; i++)
		{
			this.coloringRules[i].Init();
		}
	}

	// Token: 0x06002404 RID: 9220 RVA: 0x000C170C File Offset: 0x000BF90C
	private void InitIfNeeded()
	{
		if (!this.didInit)
		{
			this.didInit = true;
			this.rig = base.GetComponentInParent<VRRig>();
			if (this.rig == null && GorillaTagger.Instance != null)
			{
				this.rig = GorillaTagger.Instance.offlineVRRig;
			}
			this.particleMains = new ParticleSystem.MainModule[this.particleSystems.Length];
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				this.particleMains[i] = this.particleSystems[i].main;
			}
		}
	}

	// Token: 0x06002405 RID: 9221 RVA: 0x000C179E File Offset: 0x000BF99E
	private void OnEnable()
	{
		this.InitIfNeeded();
		if (this.rig != null)
		{
			this.rig.OnColorChanged += this.UpdateColor;
			this.UpdateColor(this.rig.playerColor);
		}
	}

	// Token: 0x06002406 RID: 9222 RVA: 0x000C17DC File Offset: 0x000BF9DC
	private void OnDisable()
	{
		if (this.rig != null)
		{
			this.rig.OnColorChanged -= this.UpdateColor;
		}
	}

	// Token: 0x06002407 RID: 9223 RVA: 0x000C1804 File Offset: 0x000BFA04
	public void UpdateColor(Color color)
	{
		this.InitIfNeeded();
		Color color2 = Color.Lerp(color, this.lerpToColor, this.lerpStrength);
		for (int i = 0; i < this.coloringRules.Length; i++)
		{
			this.coloringRules[i].Apply(color2);
		}
		for (int j = 0; j < this.particleSystems.Length; j++)
		{
			this.particleMains[j].startColor = color2;
		}
	}

	// Token: 0x04002F3A RID: 12090
	private const string preLog = "[GT/PlayerColoredCosmetic]  ";

	// Token: 0x04002F3B RID: 12091
	private const string preErr = "ERROR!!!  ";

	// Token: 0x04002F3C RID: 12092
	private bool didInit;

	// Token: 0x04002F3D RID: 12093
	private VRRig rig;

	// Token: 0x04002F3E RID: 12094
	[SerializeField]
	private Color lerpToColor = Color.white;

	// Token: 0x04002F3F RID: 12095
	[SerializeField]
	[Range(0f, 1f)]
	private float lerpStrength;

	// Token: 0x04002F40 RID: 12096
	[SerializeField]
	private PlayerColoredCosmetic.ColoringRule[] coloringRules;

	// Token: 0x04002F41 RID: 12097
	[SerializeField]
	private ParticleSystem[] particleSystems;

	// Token: 0x04002F42 RID: 12098
	private ParticleSystem.MainModule[] particleMains;

	// Token: 0x0200058D RID: 1421
	[Serializable]
	private struct ColoringRule
	{
		// Token: 0x06002409 RID: 9225 RVA: 0x000C188C File Offset: 0x000BFA8C
		public void Init()
		{
			this.hashId = Shader.PropertyToID(this.shaderColorProperty);
			if (this.meshRenderer == null)
			{
				Debug.LogError("ERROR!!!  ColoringRule.Init: Default meshRenderer cannot be null! Path=" + this.meshRenderer.transform.GetPathQ());
			}
			List<Material> list;
			using (CollectionPool<List<Material>, Material>.Get(out list))
			{
				this.meshRenderer.GetSharedMaterials(list);
				if (this.materialIndex < 0 || this.materialIndex >= list.Count)
				{
					Debug.LogError("ERROR!!!  " + string.Format("ColoringRule.Init: Material index {0} is out of range! Path=", this.materialIndex) + this.meshRenderer.transform.GetPathQ(), this.meshRenderer);
				}
				this.defaultMaterial = list[this.materialIndex];
				if (this.defaultMaterial == null)
				{
					Debug.LogError("ERROR!!!  ColoringRule.Init: Default material cannot be null! Path=" + this.meshRenderer.transform.GetPathQ(), this.meshRenderer);
				}
				this.instancedMaterial = new Material(list[this.materialIndex]);
				list[this.materialIndex] = this.instancedMaterial;
				this.meshRenderer.SetSharedMaterials(list);
			}
		}

		// Token: 0x0600240A RID: 9226 RVA: 0x000C19D8 File Offset: 0x000BFBD8
		public void Apply(Color color)
		{
			this.instancedMaterial.SetColor(this.hashId, color);
		}

		// Token: 0x04002F43 RID: 12099
		[SerializeField]
		private string shaderColorProperty;

		// Token: 0x04002F44 RID: 12100
		private int hashId;

		// Token: 0x04002F45 RID: 12101
		[SerializeField]
		private Renderer meshRenderer;

		// Token: 0x04002F46 RID: 12102
		[SerializeField]
		private int materialIndex;

		// Token: 0x04002F47 RID: 12103
		private Material instancedMaterial;

		// Token: 0x04002F48 RID: 12104
		private Material defaultMaterial;
	}
}
