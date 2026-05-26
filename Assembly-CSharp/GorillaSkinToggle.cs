using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020002D4 RID: 724
public class GorillaSkinToggle : MonoBehaviour, ISpawnable
{
	// Token: 0x170001D2 RID: 466
	// (get) Token: 0x06001285 RID: 4741 RVA: 0x00062B51 File Offset: 0x00060D51
	public bool applied
	{
		get
		{
			return this._applied;
		}
	}

	// Token: 0x170001D3 RID: 467
	// (get) Token: 0x06001286 RID: 4742 RVA: 0x00062B59 File Offset: 0x00060D59
	// (set) Token: 0x06001287 RID: 4743 RVA: 0x00062B61 File Offset: 0x00060D61
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170001D4 RID: 468
	// (get) Token: 0x06001288 RID: 4744 RVA: 0x00062B6A File Offset: 0x00060D6A
	// (set) Token: 0x06001289 RID: 4745 RVA: 0x00062B72 File Offset: 0x00060D72
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x0600128A RID: 4746 RVA: 0x00062B7C File Offset: 0x00060D7C
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this._rig = base.GetComponentInParent<VRRig>(true);
		if (this.coloringRules.Length != 0)
		{
			this._activeSkin = GorillaSkin.CopyWithInstancedMaterials(this._skin);
			for (int i = 0; i < this.coloringRules.Length; i++)
			{
				this.coloringRules[i].Init();
			}
			return;
		}
		this._activeSkin = this._skin;
	}

	// Token: 0x0600128B RID: 4747 RVA: 0x000028C5 File Offset: 0x00000AC5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x0600128C RID: 4748 RVA: 0x00062BE4 File Offset: 0x00060DE4
	private void OnPlayerColorChanged(Color playerColor)
	{
		foreach (GorillaSkinToggle.ColoringRule coloringRule in this.coloringRules)
		{
			coloringRule.Apply(this._activeSkin, playerColor);
		}
	}

	// Token: 0x0600128D RID: 4749 RVA: 0x00062C1C File Offset: 0x00060E1C
	private void OnEnable()
	{
		if (this.coloringRules.Length != 0)
		{
			this._rig.OnColorChanged += this.OnPlayerColorChanged;
			this.OnPlayerColorChanged(this._rig.playerColor);
		}
		this.Apply();
	}

	// Token: 0x0600128E RID: 4750 RVA: 0x00062C55 File Offset: 0x00060E55
	private void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.Remove();
		if (this.coloringRules.Length != 0)
		{
			this._rig.OnColorChanged -= this.OnPlayerColorChanged;
		}
	}

	// Token: 0x0600128F RID: 4751 RVA: 0x00062C85 File Offset: 0x00060E85
	public void Apply()
	{
		GorillaSkin.ApplyToRig(this._rig, this._activeSkin, GorillaSkin.SkinType.cosmetic);
		this._applied = true;
	}

	// Token: 0x06001290 RID: 4752 RVA: 0x00062CA0 File Offset: 0x00060EA0
	public void ApplyToMannequin(GameObject mannequin, bool swapMesh = false)
	{
		if (this._skin.IsNull())
		{
			Debug.LogError("No skin set on GorillaSkinToggle");
			return;
		}
		if (mannequin.IsNull())
		{
			Debug.LogError("No mannequin set on GorillaSkinToggle");
			return;
		}
		this._skin.ApplySkinToMannequin(mannequin, swapMesh);
	}

	// Token: 0x06001291 RID: 4753 RVA: 0x00062CDC File Offset: 0x00060EDC
	public void Remove()
	{
		GorillaSkin.ApplyToRig(this._rig, null, GorillaSkin.SkinType.cosmetic);
		float @float = PlayerPrefs.GetFloat("redValue", 0f);
		float float2 = PlayerPrefs.GetFloat("greenValue", 0f);
		float float3 = PlayerPrefs.GetFloat("blueValue", 0f);
		GorillaTagger.Instance.UpdateColor(@float, float2, float3);
		this._applied = false;
	}

	// Token: 0x04001691 RID: 5777
	private VRRig _rig;

	// Token: 0x04001692 RID: 5778
	[SerializeField]
	private GorillaSkin _skin;

	// Token: 0x04001693 RID: 5779
	private GorillaSkin _activeSkin;

	// Token: 0x04001694 RID: 5780
	[SerializeField]
	private GorillaSkinToggle.ColoringRule[] coloringRules;

	// Token: 0x04001695 RID: 5781
	[Space]
	[SerializeField]
	private bool _applied;

	// Token: 0x020002D5 RID: 725
	[Serializable]
	private struct ColoringRule
	{
		// Token: 0x06001293 RID: 4755 RVA: 0x00062D3A File Offset: 0x00060F3A
		public void Init()
		{
			if (string.IsNullOrEmpty(this.shaderColorProperty))
			{
				this.shaderColorProperty = "_BaseColor";
			}
			this.shaderHashId = new ShaderHashId(this.shaderColorProperty);
		}

		// Token: 0x06001294 RID: 4756 RVA: 0x00062D68 File Offset: 0x00060F68
		public void Apply(GorillaSkin skin, Color color)
		{
			if (this.colorMaterials.HasFlag(GorillaSkinMaterials.Body))
			{
				skin.bodyMaterial.SetColor(this.shaderHashId, color);
			}
			if (this.colorMaterials.HasFlag(GorillaSkinMaterials.Chest))
			{
				skin.chestMaterial.SetColor(this.shaderHashId, color);
			}
			if (this.colorMaterials.HasFlag(GorillaSkinMaterials.Scoreboard))
			{
				skin.scoreboardMaterial.SetColor(this.shaderHashId, color);
			}
		}

		// Token: 0x04001698 RID: 5784
		public GorillaSkinMaterials colorMaterials;

		// Token: 0x04001699 RID: 5785
		public string shaderColorProperty;

		// Token: 0x0400169A RID: 5786
		private ShaderHashId shaderHashId;
	}
}
