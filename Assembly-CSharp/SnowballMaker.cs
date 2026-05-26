using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaTag;
using UnityEngine;

// Token: 0x02000204 RID: 516
public class SnowballMaker : MonoBehaviourPostTick
{
	// Token: 0x1700013C RID: 316
	// (get) Token: 0x06000D8B RID: 3467 RVA: 0x0004A21D File Offset: 0x0004841D
	// (set) Token: 0x06000D8C RID: 3468 RVA: 0x0004A224 File Offset: 0x00048424
	public static SnowballMaker leftHandInstance { get; private set; }

	// Token: 0x1700013D RID: 317
	// (get) Token: 0x06000D8D RID: 3469 RVA: 0x0004A22C File Offset: 0x0004842C
	// (set) Token: 0x06000D8E RID: 3470 RVA: 0x0004A233 File Offset: 0x00048433
	public static SnowballMaker rightHandInstance { get; private set; }

	// Token: 0x1700013E RID: 318
	// (get) Token: 0x06000D8F RID: 3471 RVA: 0x0004A23B File Offset: 0x0004843B
	// (set) Token: 0x06000D90 RID: 3472 RVA: 0x0004A243 File Offset: 0x00048443
	public SnowballThrowable[] snowballs { get; private set; }

	// Token: 0x06000D91 RID: 3473 RVA: 0x0004A24C File Offset: 0x0004844C
	private void Awake()
	{
		if (this.snowballs == null)
		{
			this.snowballs = new SnowballThrowable[0];
		}
		if (this.isLeftHand)
		{
			if (SnowballMaker.leftHandInstance == null)
			{
				SnowballMaker.leftHandInstance = this;
				return;
			}
			Object.Destroy(base.gameObject);
			return;
		}
		else
		{
			if (SnowballMaker.rightHandInstance == null)
			{
				SnowballMaker.rightHandInstance = this;
				return;
			}
			Object.Destroy(base.gameObject);
			return;
		}
	}

	// Token: 0x06000D92 RID: 3474 RVA: 0x0004A2B4 File Offset: 0x000484B4
	private void Start()
	{
		this.handTransform = (this.isLeftHand ? GorillaTagger.Instance.offlineVRRig.myBodyDockPositions.leftHandTransform : GorillaTagger.Instance.offlineVRRig.myBodyDockPositions.rightHandTransform);
	}

	// Token: 0x06000D93 RID: 3475 RVA: 0x0004A2F0 File Offset: 0x000484F0
	internal void SetupThrowables(SnowballThrowable[] newThrowables)
	{
		this.snowballs = newThrowables;
		for (int i = 0; i < this.snowballs.Length; i++)
		{
			if (!(this.snowballs[i] == null))
			{
				for (int j = 0; j < this.snowballs[i].matDataIndexes.Count; j++)
				{
					this.matSnowballLookup.TryAdd(this.snowballs[i].matDataIndexes[j], this.snowballs[i]);
				}
			}
		}
	}

	// Token: 0x06000D94 RID: 3476 RVA: 0x0004A36C File Offset: 0x0004856C
	public override void PostTick()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (BuilderPieceInteractor.instance != null && BuilderPieceInteractor.instance.BlockSnowballCreation())
		{
			return;
		}
		if (!GTPlayer.hasInstance || !EquipmentInteractor.hasInstance || !GorillaTagger.hasInstance || !GorillaTagger.Instance.offlineVRRig)
		{
			return;
		}
		int materialTouchIndex = GTPlayer.Instance.GetMaterialTouchIndex(this.isLeftHand);
		if (materialTouchIndex == 0)
		{
			if (Time.time > this.lastGroundContactTime + this.snowballCreationCooldownTime)
			{
				this.requiresFreshMaterialContact = false;
			}
			return;
		}
		this.lastGroundContactTime = Time.time;
		this.InitializeSnowballFromMatIndex(materialTouchIndex);
		EquipmentInteractor instance = EquipmentInteractor.instance;
		bool flag = (this.isLeftHand ? instance.leftHandHeldEquipment : instance.rightHandHeldEquipment) != null;
		bool flag2 = this.isLeftHand ? instance.isLeftGrabbing : instance.isRightGrabbing;
		bool flag3 = this.isLeftHand ? instance.disableLeftGrab : instance.disableRightGrab;
		bool flag4 = false;
		if (flag2 && !flag3 && !this.requiresFreshMaterialContact)
		{
			int num = -1;
			for (int i = 0; i < this.snowballs.Length; i++)
			{
				SnowballThrowable snowballThrowable = this.snowballs[i];
				if (!(snowballThrowable == null) && snowballThrowable.gameObject.activeSelf)
				{
					num = i;
					break;
				}
			}
			SnowballThrowable snowballThrowable2 = (num > -1) ? this.snowballs[num] : null;
			GrowingSnowballThrowable growingSnowballThrowable = snowballThrowable2 as GrowingSnowballThrowable;
			bool flag5 = this.isLeftHand ? (!ConnectedControllerHandler.Instance.RightValid) : (!ConnectedControllerHandler.Instance.LeftValid);
			if (growingSnowballThrowable != null && (!GrowingSnowballThrowable.twoHandedSnowballGrowing || flag5 || flag4))
			{
				if (snowballThrowable2.matDataIndexes.Contains(materialTouchIndex))
				{
					growingSnowballThrowable.IncreaseSize(1);
					GorillaTagger.Instance.StartVibration(this.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
					this.requiresFreshMaterialContact = true;
					return;
				}
			}
			else if (!flag)
			{
				SnowballThrowable snowballThrowable3;
				if (!this.matSnowballLookup.TryGetValue(materialTouchIndex, out snowballThrowable3))
				{
					return;
				}
				Transform transform = snowballThrowable3.transform;
				Transform transform2 = this.handTransform;
				XformOffset spawnOffset = snowballThrowable3.SpawnOffset;
				snowballThrowable3.SetSnowballActiveLocal(true);
				snowballThrowable3.velocityEstimator = this.velocityEstimator;
				transform.position = transform2.TransformPoint(spawnOffset.pos);
				transform.rotation = transform2.rotation * spawnOffset.rot;
				GorillaTagger.Instance.StartVibration(this.isLeftHand, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				this.requiresFreshMaterialContact = true;
			}
		}
	}

	// Token: 0x06000D95 RID: 3477 RVA: 0x0004A608 File Offset: 0x00048808
	public bool TryCreateSnowball(int materialIndex, out SnowballThrowable result)
	{
		EquipmentInteractor instance = EquipmentInteractor.instance;
		if (this.isLeftHand ? instance.disableLeftGrab : instance.disableRightGrab)
		{
			result = null;
			return false;
		}
		this.InitializeSnowballFromMatIndex(materialIndex);
		foreach (SnowballThrowable snowballThrowable in this.snowballs)
		{
			if (!(snowballThrowable == null) && snowballThrowable.matDataIndexes.Contains(materialIndex))
			{
				Transform transform = snowballThrowable.transform;
				Transform transform2 = this.handTransform;
				XformOffset spawnOffset = snowballThrowable.SpawnOffset;
				snowballThrowable.SetSnowballActiveLocal(true);
				snowballThrowable.velocityEstimator = this.velocityEstimator;
				transform.position = transform2.TransformPoint(spawnOffset.pos);
				transform.rotation = transform2.rotation * spawnOffset.rot;
				GorillaTagger.Instance.StartVibration(this.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				result = snowballThrowable;
				return true;
			}
		}
		result = null;
		return false;
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x0004A70C File Offset: 0x0004890C
	private void InitializeSnowballFromMatIndex(int matIndex)
	{
		string itemName;
		if (CosmeticsV2Spawner_Dirty.GetThrowableIDFromMaterialIndex(this.isLeftHand, matIndex, out itemName))
		{
			VRRig.LocalRig.cosmeticsObjectRegistry.Cosmetic(itemName);
		}
	}

	// Token: 0x0400102D RID: 4141
	public bool isLeftHand;

	// Token: 0x0400102F RID: 4143
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04001030 RID: 4144
	private float snowballCreationCooldownTime = 0.1f;

	// Token: 0x04001031 RID: 4145
	private float lastGroundContactTime;

	// Token: 0x04001032 RID: 4146
	private bool requiresFreshMaterialContact;

	// Token: 0x04001033 RID: 4147
	private Transform handTransform;

	// Token: 0x04001034 RID: 4148
	private Dictionary<int, SnowballThrowable> matSnowballLookup = new Dictionary<int, SnowballThrowable>();

	// Token: 0x04001035 RID: 4149
	private Dictionary<int, SnowballThrowable> snowballByThrowableIndex = new Dictionary<int, SnowballThrowable>();

	// Token: 0x04001036 RID: 4150
	private Dictionary<int, string> snowballPlayfabIdByThrowableIndex = new Dictionary<int, string>();

	// Token: 0x04001037 RID: 4151
	private Dictionary<int, string> snowballPlayfabIdByMaterialIndex = new Dictionary<int, string>();
}
