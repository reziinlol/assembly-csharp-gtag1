using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200051E RID: 1310
public class DrumsItem : MonoBehaviour, ISpawnable
{
	// Token: 0x17000389 RID: 905
	// (get) Token: 0x060020D9 RID: 8409 RVA: 0x000AF919 File Offset: 0x000ADB19
	// (set) Token: 0x060020DA RID: 8410 RVA: 0x000AF921 File Offset: 0x000ADB21
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x1700038A RID: 906
	// (get) Token: 0x060020DB RID: 8411 RVA: 0x000AF92A File Offset: 0x000ADB2A
	// (set) Token: 0x060020DC RID: 8412 RVA: 0x000AF932 File Offset: 0x000ADB32
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060020DD RID: 8413 RVA: 0x000AF93C File Offset: 0x000ADB3C
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
		this.leftHandIndicator = GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.rightHandIndicator = GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.sphereRadius = this.leftHandIndicator.GetComponent<SphereCollider>().radius;
		for (int i = 0; i < this.collidersForThisDrum.Length; i++)
		{
			this.collidersForThisDrumList.Add(this.collidersForThisDrum[i]);
		}
		for (int j = 0; j < this.drumsAS.Length; j++)
		{
			this.myRig.AssignDrumToMusicDrums(j + this.onlineOffset, this.drumsAS[j]);
		}
	}

	// Token: 0x060020DE RID: 8414 RVA: 0x000028C5 File Offset: 0x00000AC5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060020DF RID: 8415 RVA: 0x000AF9E4 File Offset: 0x000ADBE4
	private void LateUpdate()
	{
		this.CheckHandHit(ref this.leftHandIn, ref this.leftHandIndicator, true);
		this.CheckHandHit(ref this.rightHandIn, ref this.rightHandIndicator, false);
	}

	// Token: 0x060020E0 RID: 8416 RVA: 0x000AFA0C File Offset: 0x000ADC0C
	private void CheckHandHit(ref bool handIn, ref GorillaTriggerColliderHandIndicator handIndicator, bool isLeftHand)
	{
		this.spherecastSweep = handIndicator.transform.position - handIndicator.lastPosition;
		if (this.spherecastSweep.magnitude < 0.0001f)
		{
			this.spherecastSweep = Vector3.up * 0.0001f;
		}
		for (int i = 0; i < this.collidersHit.Length; i++)
		{
			this.collidersHit[i] = this.nullHit;
		}
		this.collidersHitCount = Physics.SphereCastNonAlloc(handIndicator.lastPosition, this.sphereRadius, this.spherecastSweep.normalized, this.collidersHit, this.spherecastSweep.magnitude, this.drumsTouchable, QueryTriggerInteraction.Collide);
		this.drumHit = false;
		if (this.collidersHitCount > 0)
		{
			this.hitList.Clear();
			for (int j = 0; j < this.collidersHit.Length; j++)
			{
				if (this.collidersHit[j].collider != null && this.collidersForThisDrumList.Contains(this.collidersHit[j].collider) && this.collidersHit[j].collider.gameObject.activeSelf)
				{
					this.hitList.Add(this.collidersHit[j]);
				}
			}
			this.hitList.Sort(new Comparison<RaycastHit>(this.RayCastHitCompare));
			int k = 0;
			while (k < this.hitList.Count)
			{
				this.tempDrum = this.hitList[k].collider.GetComponent<Drum>();
				if (this.tempDrum != null)
				{
					this.drumHit = true;
					if (!handIn && !this.tempDrum.disabler)
					{
						this.DrumHit(this.tempDrum, isLeftHand, handIndicator.currentVelocity.magnitude);
						break;
					}
					break;
				}
				else
				{
					k++;
				}
			}
		}
		if (!this.drumHit & handIn)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration);
		}
		handIn = this.drumHit;
	}

	// Token: 0x060020E1 RID: 8417 RVA: 0x000AFC27 File Offset: 0x000ADE27
	private int RayCastHitCompare(RaycastHit a, RaycastHit b)
	{
		if (a.distance < b.distance)
		{
			return -1;
		}
		if (a.distance == b.distance)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x060020E2 RID: 8418 RVA: 0x000AFC50 File Offset: 0x000ADE50
	public void DrumHit(Drum tempDrumInner, bool isLeftHand, float hitVelocity)
	{
		if (isLeftHand)
		{
			if (this.leftHandIn)
			{
				return;
			}
			this.leftHandIn = true;
		}
		else
		{
			if (this.rightHandIn)
			{
				return;
			}
			this.rightHandIn = true;
		}
		this.volToPlay = Mathf.Max(Mathf.Min(1f, hitVelocity / this.maxDrumVolumeVelocity) * this.maxDrumVolume, this.minDrumVolume);
		if (NetworkSystem.Instance.InRoom)
		{
			if (!this.myRig.isOfflineVRRig)
			{
				NetworkView netView = this.myRig.netView;
				if (netView != null)
				{
					netView.SendRPC("RPC_PlayDrum", RpcTarget.Others, new object[]
					{
						tempDrumInner.myIndex + this.onlineOffset,
						this.volToPlay
					});
				}
			}
			else
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayDrum", RpcTarget.Others, new object[]
				{
					tempDrumInner.myIndex + this.onlineOffset,
					this.volToPlay
				});
			}
		}
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 4f, GorillaTagger.Instance.tapHapticDuration);
		this.drumsAS[tempDrumInner.myIndex].volume = this.maxDrumVolume;
		this.drumsAS[tempDrumInner.myIndex].GTPlayOneShot(this.drumsAS[tempDrumInner.myIndex].clip, this.volToPlay);
	}

	// Token: 0x04002B8A RID: 11146
	[Tooltip("Array of colliders for this specific drum.")]
	public Collider[] collidersForThisDrum;

	// Token: 0x04002B8B RID: 11147
	private List<Collider> collidersForThisDrumList = new List<Collider>();

	// Token: 0x04002B8C RID: 11148
	[Tooltip("AudioSources where each index must match the index given to the corresponding Drum component.")]
	public AudioSource[] drumsAS;

	// Token: 0x04002B8D RID: 11149
	[Tooltip("Max volume a drum can reach.")]
	public float maxDrumVolume = 0.2f;

	// Token: 0x04002B8E RID: 11150
	[Tooltip("Min volume a drum can reach.")]
	public float minDrumVolume = 0.05f;

	// Token: 0x04002B8F RID: 11151
	[Tooltip("Multiplies against actual velocity before capping by min & maxDrumVolume values.")]
	public float maxDrumVolumeVelocity = 1f;

	// Token: 0x04002B90 RID: 11152
	private bool rightHandIn;

	// Token: 0x04002B91 RID: 11153
	private bool leftHandIn;

	// Token: 0x04002B92 RID: 11154
	private float volToPlay;

	// Token: 0x04002B93 RID: 11155
	private GorillaTriggerColliderHandIndicator rightHandIndicator;

	// Token: 0x04002B94 RID: 11156
	private GorillaTriggerColliderHandIndicator leftHandIndicator;

	// Token: 0x04002B95 RID: 11157
	private RaycastHit[] collidersHit = new RaycastHit[20];

	// Token: 0x04002B96 RID: 11158
	private Collider[] actualColliders = new Collider[20];

	// Token: 0x04002B97 RID: 11159
	public LayerMask drumsTouchable;

	// Token: 0x04002B98 RID: 11160
	private float sphereRadius;

	// Token: 0x04002B99 RID: 11161
	private Vector3 spherecastSweep;

	// Token: 0x04002B9A RID: 11162
	private int collidersHitCount;

	// Token: 0x04002B9B RID: 11163
	private List<RaycastHit> hitList = new List<RaycastHit>(20);

	// Token: 0x04002B9C RID: 11164
	private Drum tempDrum;

	// Token: 0x04002B9D RID: 11165
	private bool drumHit;

	// Token: 0x04002B9E RID: 11166
	private RaycastHit nullHit;

	// Token: 0x04002B9F RID: 11167
	public int onlineOffset;

	// Token: 0x04002BA0 RID: 11168
	[Tooltip("VRRig object of the player, used to determine if it is an offline rig.")]
	private VRRig myRig;
}
