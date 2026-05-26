using System;
using UnityEngine;

// Token: 0x02000536 RID: 1334
public class PlantableObject : TransferrableObject
{
	// Token: 0x060021AE RID: 8622 RVA: 0x000B3C2B File Offset: 0x000B1E2B
	protected override void Awake()
	{
		base.Awake();
		this.materialPropertyBlock = new MaterialPropertyBlock();
	}

	// Token: 0x060021AF RID: 8623 RVA: 0x000B3C40 File Offset: 0x000B1E40
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.materialPropertyBlock.SetColor(ShaderProps._ColorR, this._colorR);
		this.flagRenderer.material = this.flagRenderer.sharedMaterial;
		this.flagRenderer.SetPropertyBlock(this.materialPropertyBlock);
		this.dippedColors = new PlantableObject.AppliedColors[20];
	}

	// Token: 0x060021B0 RID: 8624 RVA: 0x000B3CA0 File Offset: 0x000B1EA0
	private void AssureShaderStuff()
	{
		if (!this.flagRenderer)
		{
			return;
		}
		if (this.materialPropertyBlock == null)
		{
			this.materialPropertyBlock = new MaterialPropertyBlock();
		}
		try
		{
			this.materialPropertyBlock.SetColor(ShaderProps._ColorR, this._colorR);
			this.materialPropertyBlock.SetColor(ShaderProps._ColorG, this._colorG);
		}
		catch
		{
			this.materialPropertyBlock = new MaterialPropertyBlock();
			this.materialPropertyBlock.SetColor(ShaderProps._ColorR, this._colorR);
			this.materialPropertyBlock.SetColor(ShaderProps._ColorG, this._colorG);
		}
		this.flagRenderer.material = this.flagRenderer.sharedMaterial;
		this.flagRenderer.SetPropertyBlock(this.materialPropertyBlock);
	}

	// Token: 0x17000394 RID: 916
	// (get) Token: 0x060021B1 RID: 8625 RVA: 0x000B3D70 File Offset: 0x000B1F70
	// (set) Token: 0x060021B2 RID: 8626 RVA: 0x000B3D78 File Offset: 0x000B1F78
	public Color colorR
	{
		get
		{
			return this._colorR;
		}
		set
		{
			this._colorR = value;
			this.AssureShaderStuff();
		}
	}

	// Token: 0x17000395 RID: 917
	// (get) Token: 0x060021B3 RID: 8627 RVA: 0x000B3D87 File Offset: 0x000B1F87
	// (set) Token: 0x060021B4 RID: 8628 RVA: 0x000B3D8F File Offset: 0x000B1F8F
	public Color colorG
	{
		get
		{
			return this._colorG;
		}
		set
		{
			this._colorG = value;
			this.AssureShaderStuff();
		}
	}

	// Token: 0x17000396 RID: 918
	// (get) Token: 0x060021B5 RID: 8629 RVA: 0x000B3D9E File Offset: 0x000B1F9E
	// (set) Token: 0x060021B6 RID: 8630 RVA: 0x000B3DA6 File Offset: 0x000B1FA6
	public bool planted { get; private set; }

	// Token: 0x060021B7 RID: 8631 RVA: 0x000B3DB0 File Offset: 0x000B1FB0
	public void SetPlanted(bool newPlanted)
	{
		if (this.planted != newPlanted)
		{
			if (newPlanted)
			{
				if (!this.rigidbodyInstance.isKinematic)
				{
					this.rigidbodyInstance.isKinematic = true;
				}
				this.respawnAtTimestamp = Time.time + this.respawnAfterDuration;
			}
			else
			{
				this.respawnAtTimestamp = 0f;
			}
			this.planted = newPlanted;
		}
	}

	// Token: 0x060021B8 RID: 8632 RVA: 0x000B3E08 File Offset: 0x000B2008
	private void AddRed()
	{
		this.AddColor(PlantableObject.AppliedColors.Red);
	}

	// Token: 0x060021B9 RID: 8633 RVA: 0x000B3E11 File Offset: 0x000B2011
	private void AddGreen()
	{
		this.AddColor(PlantableObject.AppliedColors.Blue);
	}

	// Token: 0x060021BA RID: 8634 RVA: 0x000B3E1A File Offset: 0x000B201A
	private void AddBlue()
	{
		this.AddColor(PlantableObject.AppliedColors.Green);
	}

	// Token: 0x060021BB RID: 8635 RVA: 0x000B3E23 File Offset: 0x000B2023
	private void AddBlack()
	{
		this.AddColor(PlantableObject.AppliedColors.Black);
	}

	// Token: 0x060021BC RID: 8636 RVA: 0x000B3E2C File Offset: 0x000B202C
	public void AddColor(PlantableObject.AppliedColors color)
	{
		this.dippedColors[this.currentDipIndex] = color;
		this.currentDipIndex++;
		if (this.currentDipIndex >= this.dippedColors.Length)
		{
			this.currentDipIndex = 0;
		}
		this.UpdateDisplayedDippedColor();
	}

	// Token: 0x060021BD RID: 8637 RVA: 0x000B3E68 File Offset: 0x000B2068
	public void ClearColors()
	{
		for (int i = 0; i < this.dippedColors.Length; i++)
		{
			this.dippedColors[i] = PlantableObject.AppliedColors.None;
		}
		this.currentDipIndex = 0;
		this.UpdateDisplayedDippedColor();
	}

	// Token: 0x060021BE RID: 8638 RVA: 0x000B3EA0 File Offset: 0x000B20A0
	public Color CalculateOutputColor()
	{
		Color color = Color.black;
		int num = 0;
		int num2 = 0;
		foreach (PlantableObject.AppliedColors appliedColors in this.dippedColors)
		{
			if (appliedColors == PlantableObject.AppliedColors.None)
			{
				break;
			}
			switch (appliedColors)
			{
			case PlantableObject.AppliedColors.Red:
				color += Color.red;
				num2++;
				break;
			case PlantableObject.AppliedColors.Green:
				color += Color.green;
				num2++;
				break;
			case PlantableObject.AppliedColors.Blue:
				color += Color.blue;
				num2++;
				break;
			case PlantableObject.AppliedColors.Black:
				num++;
				num2++;
				break;
			}
		}
		if (color == Color.black && num == 0)
		{
			return Color.white;
		}
		float num3 = Mathf.Max(new float[]
		{
			color.r,
			color.g,
			color.b
		});
		if (num3 == 0f)
		{
			return Color.black;
		}
		color /= num3;
		float num4 = (float)num / (float)num2;
		if (num4 > 0f)
		{
			color *= 1f - num4;
		}
		return color;
	}

	// Token: 0x060021BF RID: 8639 RVA: 0x000B3FA9 File Offset: 0x000B21A9
	public void UpdateDisplayedDippedColor()
	{
		this.colorR = this.CalculateOutputColor();
	}

	// Token: 0x060021C0 RID: 8640 RVA: 0x000B3FB7 File Offset: 0x000B21B7
	public override void DropItem()
	{
		base.DropItem();
		if (this.itemState == TransferrableObject.ItemStates.State1 && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x060021C1 RID: 8641 RVA: 0x000B3FE4 File Offset: 0x000B21E4
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		this.itemState = (this.planted ? TransferrableObject.ItemStates.State1 : TransferrableObject.ItemStates.State0);
		if (this.respawnAtTimestamp != 0f && Time.time > this.respawnAtTimestamp)
		{
			this.respawnAtTimestamp = 0f;
			this.ResetToHome();
		}
	}

	// Token: 0x060021C2 RID: 8642 RVA: 0x000B4034 File Offset: 0x000B2234
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.itemState == TransferrableObject.ItemStates.State1 && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x060021C3 RID: 8643 RVA: 0x000B405E File Offset: 0x000B225E
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
	}

	// Token: 0x060021C4 RID: 8644 RVA: 0x000B4068 File Offset: 0x000B2268
	public override bool ShouldBeKinematic()
	{
		return base.ShouldBeKinematic() || this.itemState == TransferrableObject.ItemStates.State1;
	}

	// Token: 0x060021C5 RID: 8645 RVA: 0x000B4080 File Offset: 0x000B2280
	public override void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		base.OnOwnershipTransferred(toPlayer, fromPlayer);
		if (toPlayer == null)
		{
			return;
		}
		if (toPlayer.IsLocal && this.itemState == TransferrableObject.ItemStates.State1)
		{
			this.respawnAtTimestamp = Time.time + this.respawnAfterDuration;
		}
		Action<Color> <>9__1;
		GorillaGameManager.OnInstanceReady(delegate
		{
			VRRig vrrig = GorillaGameManager.instance.FindPlayerVRRig(toPlayer);
			if (vrrig == null)
			{
				return;
			}
			VRRig vrrig2 = vrrig;
			Action<Color> action;
			if ((action = <>9__1) == null)
			{
				action = (<>9__1 = delegate(Color color1)
				{
					this.colorG = color1;
				});
			}
			vrrig2.OnColorInitialized(action);
		});
	}

	// Token: 0x04002C88 RID: 11400
	public PlantablePoint point;

	// Token: 0x04002C89 RID: 11401
	public float respawnAfterDuration;

	// Token: 0x04002C8A RID: 11402
	private float respawnAtTimestamp;

	// Token: 0x04002C8B RID: 11403
	public SkinnedMeshRenderer flagRenderer;

	// Token: 0x04002C8C RID: 11404
	private MaterialPropertyBlock materialPropertyBlock;

	// Token: 0x04002C8D RID: 11405
	[HideInInspector]
	[SerializeReference]
	private Color _colorR;

	// Token: 0x04002C8E RID: 11406
	[HideInInspector]
	[SerializeReference]
	private Color _colorG;

	// Token: 0x04002C90 RID: 11408
	public Transform flagTip;

	// Token: 0x04002C91 RID: 11409
	public PlantableObject.AppliedColors[] dippedColors = new PlantableObject.AppliedColors[20];

	// Token: 0x04002C92 RID: 11410
	public int currentDipIndex;

	// Token: 0x02000537 RID: 1335
	public enum AppliedColors
	{
		// Token: 0x04002C94 RID: 11412
		None,
		// Token: 0x04002C95 RID: 11413
		Red,
		// Token: 0x04002C96 RID: 11414
		Green,
		// Token: 0x04002C97 RID: 11415
		Blue,
		// Token: 0x04002C98 RID: 11416
		Black
	}
}
