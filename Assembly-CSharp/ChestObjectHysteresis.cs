using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000518 RID: 1304
public class ChestObjectHysteresis : MonoBehaviour, ISpawnable
{
	// Token: 0x17000385 RID: 901
	// (get) Token: 0x0600209F RID: 8351 RVA: 0x000AF11B File Offset: 0x000AD31B
	// (set) Token: 0x060020A0 RID: 8352 RVA: 0x000AF123 File Offset: 0x000AD323
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000386 RID: 902
	// (get) Token: 0x060020A1 RID: 8353 RVA: 0x000AF12C File Offset: 0x000AD32C
	// (set) Token: 0x060020A2 RID: 8354 RVA: 0x000AF134 File Offset: 0x000AD334
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060020A3 RID: 8355 RVA: 0x000AF140 File Offset: 0x000AD340
	void ISpawnable.OnSpawn(VRRig rig)
	{
		if (!this.angleFollower && (string.IsNullOrEmpty(this.angleFollower_path) || base.transform.TryFindByPath(this.angleFollower_path, out this.angleFollower, false)))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"ChestObjectHysteresis: DEACTIVATING! Could not find `angleFollower` using path: \"",
				this.angleFollower_path,
				"\". For component at: \"",
				this.GetComponentPath(int.MaxValue),
				"\""
			}), this);
			base.gameObject.SetActive(false);
			return;
		}
	}

	// Token: 0x060020A4 RID: 8356 RVA: 0x000028C5 File Offset: 0x00000AC5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060020A5 RID: 8357 RVA: 0x000AF1CE File Offset: 0x000AD3CE
	private void Start()
	{
		this.lastAngleQuat = base.transform.rotation;
		this.currentAngleQuat = base.transform.rotation;
	}

	// Token: 0x060020A6 RID: 8358 RVA: 0x000AF1F2 File Offset: 0x000AD3F2
	private void OnEnable()
	{
		ChestObjectHysteresisManager.RegisterCH(this);
	}

	// Token: 0x060020A7 RID: 8359 RVA: 0x000AF1FA File Offset: 0x000AD3FA
	private void OnDisable()
	{
		ChestObjectHysteresisManager.UnregisterCH(this);
	}

	// Token: 0x060020A8 RID: 8360 RVA: 0x000AF204 File Offset: 0x000AD404
	public void InvokeUpdate()
	{
		this.currentAngleQuat = this.angleFollower.rotation;
		this.angleBetween = Quaternion.Angle(this.currentAngleQuat, this.lastAngleQuat);
		if (this.angleBetween > this.angleHysteresis)
		{
			base.transform.rotation = Quaternion.Slerp(this.currentAngleQuat, this.lastAngleQuat, this.angleHysteresis / this.angleBetween);
			this.lastAngleQuat = base.transform.rotation;
		}
		base.transform.rotation = this.lastAngleQuat;
	}

	// Token: 0x04002B57 RID: 11095
	public float angleHysteresis;

	// Token: 0x04002B58 RID: 11096
	public float angleBetween;

	// Token: 0x04002B59 RID: 11097
	public Transform angleFollower;

	// Token: 0x04002B5A RID: 11098
	[Delayed]
	public string angleFollower_path;

	// Token: 0x04002B5B RID: 11099
	private Quaternion lastAngleQuat;

	// Token: 0x04002B5C RID: 11100
	private Quaternion currentAngleQuat;
}
