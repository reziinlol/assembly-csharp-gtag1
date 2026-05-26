using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000587 RID: 1415
public class MoodRing : MonoBehaviour, ISpawnable
{
	// Token: 0x170003C3 RID: 963
	// (get) Token: 0x060023D1 RID: 9169 RVA: 0x000C069F File Offset: 0x000BE89F
	// (set) Token: 0x060023D2 RID: 9170 RVA: 0x000C06A7 File Offset: 0x000BE8A7
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170003C4 RID: 964
	// (get) Token: 0x060023D3 RID: 9171 RVA: 0x000C06B0 File Offset: 0x000BE8B0
	// (set) Token: 0x060023D4 RID: 9172 RVA: 0x000C06B8 File Offset: 0x000BE8B8
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060023D5 RID: 9173 RVA: 0x000028C5 File Offset: 0x00000AC5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060023D6 RID: 9174 RVA: 0x000C06C1 File Offset: 0x000BE8C1
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x060023D7 RID: 9175 RVA: 0x000C06CC File Offset: 0x000BE8CC
	private void Update()
	{
		if ((this.attachedToLeftHand ? this.myRig.leftIndex.calcT : this.myRig.rightIndex.calcT) > 0.5f)
		{
			if (!this.isCycling)
			{
				this.animRedValue = this.myRig.playerColor.r;
				this.animGreenValue = this.myRig.playerColor.g;
				this.animBlueValue = this.myRig.playerColor.b;
			}
			this.isCycling = true;
			this.RainbowCycle(ref this.animRedValue, ref this.animGreenValue, ref this.animBlueValue);
			this.myRig.InitializeNoobMaterialLocal(this.animRedValue, this.animGreenValue, this.animBlueValue);
			return;
		}
		if (this.isCycling)
		{
			this.isCycling = false;
			if (this.myRig.isOfflineVRRig)
			{
				this.animRedValue = Mathf.Round(this.animRedValue * 9f) / 9f;
				this.animGreenValue = Mathf.Round(this.animGreenValue * 9f) / 9f;
				this.animBlueValue = Mathf.Round(this.animBlueValue * 9f) / 9f;
				GorillaTagger.Instance.UpdateColor(this.animRedValue, this.animGreenValue, this.animBlueValue);
				if (NetworkSystem.Instance.InRoom)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
					{
						this.animRedValue,
						this.animGreenValue,
						this.animBlueValue
					});
				}
				PlayerPrefs.SetFloat("redValue", this.animRedValue);
				PlayerPrefs.SetFloat("greenValue", this.animGreenValue);
				PlayerPrefs.SetFloat("blueValue", this.animBlueValue);
				PlayerPrefs.Save();
			}
		}
	}

	// Token: 0x060023D8 RID: 9176 RVA: 0x000C08B0 File Offset: 0x000BEAB0
	private void RainbowCycle(ref float r, ref float g, ref float b)
	{
		float num = this.furCycleSpeed * Time.deltaTime;
		if (r == 1f)
		{
			if (b > 0f)
			{
				b = Mathf.Clamp01(b - num);
				return;
			}
			if (g < 1f)
			{
				g = Mathf.Clamp01(g + num);
				return;
			}
			r = Mathf.Clamp01(r - num);
			return;
		}
		else if (g == 1f)
		{
			if (r > 0f)
			{
				r = Mathf.Clamp01(r - num);
				return;
			}
			if (b < 1f)
			{
				b = Mathf.Clamp01(b + num);
				return;
			}
			g = Mathf.Clamp01(g - num);
			return;
		}
		else
		{
			if (b != 1f)
			{
				r = Mathf.Clamp01(r + num);
				return;
			}
			if (g > 0f)
			{
				g = Mathf.Clamp01(g - num);
				return;
			}
			if (r < 1f)
			{
				r = Mathf.Clamp01(r + num);
				return;
			}
			b = Mathf.Clamp01(b - num);
			return;
		}
	}

	// Token: 0x04002EF8 RID: 12024
	[SerializeField]
	private bool attachedToLeftHand;

	// Token: 0x04002EF9 RID: 12025
	private VRRig myRig;

	// Token: 0x04002EFA RID: 12026
	[SerializeField]
	private float rotationSpeed;

	// Token: 0x04002EFB RID: 12027
	[SerializeField]
	private float furCycleSpeed;

	// Token: 0x04002EFC RID: 12028
	private float nextFurCycleTimestamp;

	// Token: 0x04002EFD RID: 12029
	private float animRedValue;

	// Token: 0x04002EFE RID: 12030
	private float animGreenValue;

	// Token: 0x04002EFF RID: 12031
	private float animBlueValue;

	// Token: 0x04002F00 RID: 12032
	private bool isCycling;
}
