using System;
using GorillaExtensions;
using GorillaTag;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200051F RID: 1311
public class EdibleHoldable : TransferrableObject
{
	// Token: 0x1700038B RID: 907
	// (get) Token: 0x060020E4 RID: 8420 RVA: 0x000AFE1E File Offset: 0x000AE01E
	// (set) Token: 0x060020E5 RID: 8421 RVA: 0x000AFE26 File Offset: 0x000AE026
	public int lastBiterActorID { get; private set; } = -1;

	// Token: 0x060020E6 RID: 8422 RVA: 0x000AFE2F File Offset: 0x000AE02F
	protected override void Start()
	{
		base.Start();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.previousEdibleState = (EdibleHoldable.EdibleHoldableStates)this.itemState;
		this.lastFullyEatenTime = -this.respawnTime;
		this.iResettableItems = base.GetComponentsInChildren<IResettableItem>(true);
	}

	// Token: 0x060020E7 RID: 8423 RVA: 0x000AFE64 File Offset: 0x000AE064
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		this.lastEatTime = Time.time - this.eatMinimumCooldown;
	}

	// Token: 0x060020E8 RID: 8424 RVA: 0x000AFE80 File Offset: 0x000AE080
	public override void OnActivate()
	{
		base.OnActivate();
	}

	// Token: 0x060020E9 RID: 8425 RVA: 0x000AFE88 File Offset: 0x000AE088
	internal override void OnEnable()
	{
		base.OnEnable();
	}

	// Token: 0x060020EA RID: 8426 RVA: 0x0004B3FC File Offset: 0x000495FC
	internal override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x060020EB RID: 8427 RVA: 0x000AFE90 File Offset: 0x000AE090
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
	}

	// Token: 0x060020EC RID: 8428 RVA: 0x000AFE98 File Offset: 0x000AE098
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return base.OnRelease(zoneReleased, releasingHand) && !base.InHand();
	}

	// Token: 0x060020ED RID: 8429 RVA: 0x000AFEB4 File Offset: 0x000AE0B4
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (this.itemState == TransferrableObject.ItemStates.State3)
		{
			if (Time.time > this.lastFullyEatenTime + this.respawnTime)
			{
				this.itemState = TransferrableObject.ItemStates.State0;
				return;
			}
		}
		else if (Time.time > this.lastEatTime + this.eatMinimumCooldown)
		{
			bool flag = false;
			bool flag2 = false;
			float num = this.biteDistance * this.biteDistance;
			if (!GorillaParent.hasInstance)
			{
				return;
			}
			VRRig vrrig = null;
			VRRig vrrig2 = null;
			for (int i = 0; i < VRRigCache.ActiveRigContainers.Count; i++)
			{
				VRRig rig = VRRigCache.ActiveRigContainers[i].Rig;
				if (!rig.isOfflineVRRig)
				{
					if (rig.head == null || rig.head.rigTarget.IsNull())
					{
						break;
					}
					Transform transform = rig.head.rigTarget.transform;
					if ((transform.position + transform.rotation * this.biteOffset - this.biteSpot.position).sqrMagnitude < num)
					{
						flag = true;
						vrrig2 = rig;
					}
				}
			}
			Transform transform2 = GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform;
			if ((transform2.position + transform2.rotation * this.biteOffset - this.biteSpot.position).sqrMagnitude < num)
			{
				flag = true;
				flag2 = true;
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (flag && !this.inBiteZone && (!flag2 || base.InHand()) && this.itemState != TransferrableObject.ItemStates.State3)
			{
				if (this.itemState == TransferrableObject.ItemStates.State0)
				{
					this.itemState = TransferrableObject.ItemStates.State1;
				}
				else if (this.itemState == TransferrableObject.ItemStates.State1)
				{
					this.itemState = TransferrableObject.ItemStates.State2;
				}
				else if (this.itemState == TransferrableObject.ItemStates.State2)
				{
					this.itemState = TransferrableObject.ItemStates.State3;
				}
				this.lastEatTime = Time.time;
				this.lastFullyEatenTime = Time.time;
			}
			if (flag)
			{
				if (flag2)
				{
					int lastBiterActorID;
					if (!vrrig)
					{
						lastBiterActorID = -1;
					}
					else
					{
						NetPlayer owningNetPlayer = vrrig.OwningNetPlayer;
						lastBiterActorID = ((owningNetPlayer != null) ? owningNetPlayer.ActorNumber : -1);
					}
					this.lastBiterActorID = lastBiterActorID;
					EdibleHoldable.BiteEvent biteEvent = this.onBiteView;
					if (biteEvent != null)
					{
						biteEvent.Invoke(vrrig, (int)this.itemState);
					}
				}
				else
				{
					int lastBiterActorID2;
					if (!vrrig2)
					{
						lastBiterActorID2 = -1;
					}
					else
					{
						NetPlayer owningNetPlayer2 = vrrig2.OwningNetPlayer;
						lastBiterActorID2 = ((owningNetPlayer2 != null) ? owningNetPlayer2.ActorNumber : -1);
					}
					this.lastBiterActorID = lastBiterActorID2;
					EdibleHoldable.BiteEvent biteEvent2 = this.onBiteWorld;
					if (biteEvent2 != null)
					{
						biteEvent2.Invoke(vrrig2, (int)this.itemState);
					}
				}
			}
			this.inBiteZone = flag;
		}
	}

	// Token: 0x060020EE RID: 8430 RVA: 0x000B0124 File Offset: 0x000AE324
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		EdibleHoldable.EdibleHoldableStates itemState = (EdibleHoldable.EdibleHoldableStates)this.itemState;
		if (itemState != this.previousEdibleState)
		{
			this.OnEdibleHoldableStateChange();
		}
		this.previousEdibleState = itemState;
	}

	// Token: 0x060020EF RID: 8431 RVA: 0x000B0154 File Offset: 0x000AE354
	protected virtual void OnEdibleHoldableStateChange()
	{
		float amplitude = GorillaTagger.Instance.tapHapticStrength / 4f;
		float fixedDeltaTime = Time.fixedDeltaTime;
		float volumeScale = 0.08f;
		int num = 0;
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			num = 0;
			if (this.iResettableItems != null)
			{
				foreach (IResettableItem resettableItem in this.iResettableItems)
				{
					if (resettableItem != null)
					{
						resettableItem.ResetToDefaultState();
					}
				}
			}
		}
		else if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			num = 1;
		}
		else if (this.itemState == TransferrableObject.ItemStates.State2)
		{
			num = 2;
		}
		else if (this.itemState == TransferrableObject.ItemStates.State3)
		{
			num = 3;
		}
		int num2 = num - 1;
		if (num2 < 0)
		{
			num2 = this.edibleMeshObjects.Length - 1;
		}
		this.edibleMeshObjects[num2].SetActive(false);
		this.edibleMeshObjects[num].SetActive(true);
		if ((this.itemState != TransferrableObject.ItemStates.State0 && this.onBiteView != null) || this.onBiteWorld != null)
		{
			VRRig vrrig = null;
			float num3 = float.PositiveInfinity;
			for (int j = 0; j < VRRigCache.ActiveRigContainers.Count; j++)
			{
				VRRig rig = VRRigCache.ActiveRigContainers[j].Rig;
				if (rig.head == null || rig.head.rigTarget.IsNull())
				{
					break;
				}
				Transform transform = rig.head.rigTarget.transform;
				float sqrMagnitude = (transform.position + transform.rotation * this.biteOffset - this.biteSpot.position).sqrMagnitude;
				if (sqrMagnitude < num3)
				{
					num3 = sqrMagnitude;
					vrrig = rig;
				}
			}
			if (vrrig.IsNotNull())
			{
				EdibleHoldable.BiteEvent biteEvent = vrrig.isOfflineVRRig ? this.onBiteView : this.onBiteWorld;
				if (biteEvent != null)
				{
					biteEvent.Invoke(vrrig, (int)this.itemState);
				}
				if (vrrig.isOfflineVRRig && this.itemState != TransferrableObject.ItemStates.State0)
				{
					PlayerGameEvents.EatObject(this.interactEventName);
				}
			}
		}
		this.eatSoundSource.GTPlayOneShot(this.eatSounds[num], volumeScale);
		if (this.IsMyItem())
		{
			if (base.InHand())
			{
				GorillaTagger.Instance.StartVibration(base.InLeftHand(), amplitude, fixedDeltaTime);
				return;
			}
			GorillaTagger.Instance.StartVibration(false, amplitude, fixedDeltaTime);
			GorillaTagger.Instance.StartVibration(true, amplitude, fixedDeltaTime);
		}
	}

	// Token: 0x060020F0 RID: 8432 RVA: 0x00023994 File Offset: 0x00021B94
	public override bool CanActivate()
	{
		return true;
	}

	// Token: 0x060020F1 RID: 8433 RVA: 0x00023994 File Offset: 0x00021B94
	public override bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x04002BA3 RID: 11171
	public AudioClip[] eatSounds;

	// Token: 0x04002BA4 RID: 11172
	public GameObject[] edibleMeshObjects;

	// Token: 0x04002BA6 RID: 11174
	public EdibleHoldable.BiteEvent onBiteView;

	// Token: 0x04002BA7 RID: 11175
	public EdibleHoldable.BiteEvent onBiteWorld;

	// Token: 0x04002BA8 RID: 11176
	[DebugReadout]
	public float lastEatTime;

	// Token: 0x04002BA9 RID: 11177
	[DebugReadout]
	public float lastFullyEatenTime;

	// Token: 0x04002BAA RID: 11178
	public float eatMinimumCooldown = 1f;

	// Token: 0x04002BAB RID: 11179
	public float respawnTime = 7f;

	// Token: 0x04002BAC RID: 11180
	public float biteDistance = 0.1666667f;

	// Token: 0x04002BAD RID: 11181
	public Vector3 biteOffset = new Vector3(0f, 0.0208f, 0.171f);

	// Token: 0x04002BAE RID: 11182
	public Transform biteSpot;

	// Token: 0x04002BAF RID: 11183
	public bool inBiteZone;

	// Token: 0x04002BB0 RID: 11184
	public AudioSource eatSoundSource;

	// Token: 0x04002BB1 RID: 11185
	private EdibleHoldable.EdibleHoldableStates previousEdibleState;

	// Token: 0x04002BB2 RID: 11186
	private IResettableItem[] iResettableItems;

	// Token: 0x02000520 RID: 1312
	private enum EdibleHoldableStates
	{
		// Token: 0x04002BB4 RID: 11188
		EatingState0 = 1,
		// Token: 0x04002BB5 RID: 11189
		EatingState1,
		// Token: 0x04002BB6 RID: 11190
		EatingState2 = 4,
		// Token: 0x04002BB7 RID: 11191
		EatingState3 = 8
	}

	// Token: 0x02000521 RID: 1313
	[Serializable]
	public class BiteEvent : UnityEvent<VRRig, int>
	{
	}
}
