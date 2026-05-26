using System;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000C1 RID: 193
public class HoldableLighterCosmetic : MonoBehaviour
{
	// Token: 0x060004BC RID: 1212 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnEnable()
	{
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x0001A6C1 File Offset: 0x000188C1
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x0001A6DB File Offset: 0x000188DB
	private bool IsMyItem()
	{
		return this.rig != null && this.rig.isOfflineVRRig;
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x0001A6F8 File Offset: 0x000188F8
	private void DebugPull()
	{
		this.TriggerPulled();
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x0001A700 File Offset: 0x00018900
	private void DebugRelease()
	{
		this.TriggerReleased();
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x0001A708 File Offset: 0x00018908
	public void TriggerPulled()
	{
		this.triggerHeld = true;
		if (this.OwnerID == 0)
		{
			this.TrySetID();
		}
		double time = PhotonNetwork.Time;
		switch (this.GetResultAtTime(time, this.OwnerID))
		{
		case HoldableLighterCosmetic.LighterResult.Flicker:
		{
			UnityEvent onFlicker = this.OnFlicker;
			if (onFlicker != null)
			{
				onFlicker.Invoke();
			}
			if (this.parentTransferable.IsMyItem())
			{
				GorillaTagger.Instance.StartVibration(this.parentTransferable.InLeftHand(), 0.1f, 0.1f);
				return;
			}
			break;
		}
		case HoldableLighterCosmetic.LighterResult.Light:
		{
			UnityEvent onLight = this.OnLight;
			if (onLight != null)
			{
				onLight.Invoke();
			}
			if (this.parentTransferable.IsMyItem())
			{
				GorillaTagger.Instance.StartVibration(this.parentTransferable.InLeftHand(), 0.1f, 0.1f);
				return;
			}
			break;
		}
		case HoldableLighterCosmetic.LighterResult.Explode:
		{
			UnityEvent onExplode = this.OnExplode;
			if (onExplode != null)
			{
				onExplode.Invoke();
			}
			if (this.parentTransferable.IsMyItem())
			{
				GorillaTagger.Instance.StartVibration(this.parentTransferable.InLeftHand(), 0.75f, 0.5f);
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x0001A810 File Offset: 0x00018A10
	private HoldableLighterCosmetic.LighterResult GetResultAtTime(double photonTime, int seed)
	{
		int num = (int)Math.Floor(photonTime);
		float num2 = (float)new Random(seed ^ num).NextDouble();
		if (num2 < this.explodeWeight)
		{
			return HoldableLighterCosmetic.LighterResult.Explode;
		}
		if (num2 < this.explodeWeight + this.lightWeight)
		{
			return HoldableLighterCosmetic.LighterResult.Light;
		}
		return HoldableLighterCosmetic.LighterResult.Flicker;
	}

	// Token: 0x060004C3 RID: 1219 RVA: 0x0001A852 File Offset: 0x00018A52
	public void TriggerReleased()
	{
		this.triggerHeld = false;
		UnityEvent onTriggerRelease = this.OnTriggerRelease;
		if (onTriggerRelease == null)
		{
			return;
		}
		onTriggerRelease.Invoke();
	}

	// Token: 0x060004C4 RID: 1220 RVA: 0x0001A86C File Offset: 0x00018A6C
	private void TrySetID()
	{
		if (this.parentTransferable.IsLocalObject())
		{
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			if (instance != null)
			{
				string playFabPlayerId = instance.GetPlayFabPlayerId();
				Type type = base.GetType();
				this.OwnerID = (playFabPlayerId + ((type != null) ? type.ToString() : null)).GetStaticHash();
				return;
			}
		}
		else if (this.parentTransferable.targetRig != null && this.parentTransferable.targetRig.creator != null)
		{
			string userId = this.parentTransferable.targetRig.creator.UserId;
			Type type2 = base.GetType();
			this.OwnerID = (userId + ((type2 != null) ? type2.ToString() : null)).GetStaticHash();
		}
	}

	// Token: 0x04000538 RID: 1336
	private int OwnerID;

	// Token: 0x04000539 RID: 1337
	[Header("Weights (0 to 1 total)")]
	[Range(0f, 1f)]
	public float flickerWeight = 0.5f;

	// Token: 0x0400053A RID: 1338
	[Range(0f, 1f)]
	public float lightWeight = 0.3f;

	// Token: 0x0400053B RID: 1339
	[Range(0f, 1f)]
	public float explodeWeight = 0.2f;

	// Token: 0x0400053C RID: 1340
	[Header("Unity Events")]
	public UnityEvent OnFlicker;

	// Token: 0x0400053D RID: 1341
	public UnityEvent OnLight;

	// Token: 0x0400053E RID: 1342
	public UnityEvent OnExplode;

	// Token: 0x0400053F RID: 1343
	public UnityEvent OnTriggerRelease;

	// Token: 0x04000540 RID: 1344
	private HoldableLighterCosmetic.LighterResult[] resultTimeline;

	// Token: 0x04000541 RID: 1345
	private bool triggerHeld;

	// Token: 0x04000542 RID: 1346
	private float lastCheckTime;

	// Token: 0x04000543 RID: 1347
	private VRRig rig;

	// Token: 0x04000544 RID: 1348
	private TransferrableObject parentTransferable;

	// Token: 0x020000C2 RID: 194
	public enum LighterResult
	{
		// Token: 0x04000546 RID: 1350
		Flicker,
		// Token: 0x04000547 RID: 1351
		Light,
		// Token: 0x04000548 RID: 1352
		Explode
	}
}
