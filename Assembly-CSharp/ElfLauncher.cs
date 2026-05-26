using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002BF RID: 703
public class ElfLauncher : MonoBehaviour
{
	// Token: 0x06001225 RID: 4645 RVA: 0x00061318 File Offset: 0x0005F518
	private void OnEnable()
	{
		if (this._events == null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			NetPlayer netPlayer = (this.parentHoldable.myOnlineRig != null) ? this.parentHoldable.myOnlineRig.creator : ((this.parentHoldable.myRig != null) ? ((this.parentHoldable.myRig.creator != null) ? this.parentHoldable.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
			if (netPlayer != null)
			{
				this.m_player = netPlayer;
				this._events.Init(netPlayer);
			}
			else
			{
				Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
			}
		}
		if (this._events != null)
		{
			this._events.Activate += this.ShootShared;
		}
	}

	// Token: 0x06001226 RID: 4646 RVA: 0x00061404 File Offset: 0x0005F604
	private void OnDisable()
	{
		if (this._events != null)
		{
			this._events.Activate -= this.ShootShared;
			this._events.Dispose();
			this._events = null;
			this.m_player = null;
		}
	}

	// Token: 0x06001227 RID: 4647 RVA: 0x0006145C File Offset: 0x0005F65C
	private void Awake()
	{
		this._events = base.GetComponent<RubberDuckEvents>();
		this.elfProjectileHash = PoolUtils.GameObjHashCode(this.elfProjectilePrefab);
		TransferrableObjectHoldablePart_Crank[] array = this.cranks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetOnCrankedCallback(new Action<float>(this.OnCranked));
		}
	}

	// Token: 0x06001228 RID: 4648 RVA: 0x000614B0 File Offset: 0x0005F6B0
	private void OnCranked(float deltaAngle)
	{
		this.currentShootCrankAmount += deltaAngle;
		if (Mathf.Abs(this.currentShootCrankAmount) > this.crankShootThreshold)
		{
			this.currentShootCrankAmount = 0f;
			this.Shoot();
		}
		this.currentClickCrankAmount += deltaAngle;
		if (Mathf.Abs(this.currentClickCrankAmount) > this.crankClickThreshold)
		{
			this.currentClickCrankAmount = 0f;
			this.crankClickAudio.Play();
		}
	}

	// Token: 0x06001229 RID: 4649 RVA: 0x00061528 File Offset: 0x0005F728
	private void Shoot()
	{
		if (this.parentHoldable.IsLocalObject())
		{
			GorillaTagger.Instance.StartVibration(true, this.shootHapticStrength, this.shootHapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.shootHapticStrength, this.shootHapticDuration);
			if (PhotonNetwork.InRoom)
			{
				this._events.Activate.RaiseAll(new object[]
				{
					this.muzzle.transform.position,
					this.muzzle.transform.forward
				});
				return;
			}
			this.ShootShared(this.muzzle.transform.position, this.muzzle.transform.forward);
		}
	}

	// Token: 0x0600122A RID: 4650 RVA: 0x000615E8 File Offset: 0x0005F7E8
	private void ShootShared(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (args.Length != 2)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		VRRig ownerRig = this.parentHoldable.ownerRig;
		if (info.senderID != ownerRig.creator.ActorNumber)
		{
			return;
		}
		if (args.Length == 2)
		{
			object obj = args[0];
			if (obj is Vector3)
			{
				Vector3 vector = (Vector3)obj;
				obj = args[1];
				if (obj is Vector3)
				{
					Vector3 direction = (Vector3)obj;
					float num = 10000f;
					if (vector.IsValid(num))
					{
						float num2 = 10000f;
						if (direction.IsValid(num2))
						{
							if (!FXSystem.CheckCallSpam(ownerRig.fxSettings, 4, info.SentServerTime) || !ownerRig.IsPositionInRange(vector, 6f))
							{
								return;
							}
							this.ShootShared(vector, direction);
							return;
						}
					}
				}
			}
		}
	}

	// Token: 0x0600122B RID: 4651 RVA: 0x000616A0 File Offset: 0x0005F8A0
	protected virtual void ShootShared(Vector3 origin, Vector3 direction)
	{
		this.shootAudio.Play();
		Vector3 lossyScale = base.transform.lossyScale;
		GameObject gameObject = ObjectPools.instance.Instantiate(this.elfProjectileHash, true);
		gameObject.transform.position = origin;
		gameObject.transform.rotation = Quaternion.LookRotation(direction);
		gameObject.transform.localScale = lossyScale;
		gameObject.GetComponent<Rigidbody>().linearVelocity = direction * this.muzzleVelocity * lossyScale.x;
	}

	// Token: 0x040015FB RID: 5627
	[SerializeField]
	protected TransferrableObject parentHoldable;

	// Token: 0x040015FC RID: 5628
	[SerializeField]
	private TransferrableObjectHoldablePart_Crank[] cranks;

	// Token: 0x040015FD RID: 5629
	[SerializeField]
	private float crankShootThreshold = 360f;

	// Token: 0x040015FE RID: 5630
	[SerializeField]
	private float crankClickThreshold = 30f;

	// Token: 0x040015FF RID: 5631
	[SerializeField]
	private Transform muzzle;

	// Token: 0x04001600 RID: 5632
	[SerializeField]
	private GameObject elfProjectilePrefab;

	// Token: 0x04001601 RID: 5633
	protected int elfProjectileHash;

	// Token: 0x04001602 RID: 5634
	[SerializeField]
	protected float muzzleVelocity = 10f;

	// Token: 0x04001603 RID: 5635
	[SerializeField]
	private SoundBankPlayer crankClickAudio;

	// Token: 0x04001604 RID: 5636
	[SerializeField]
	protected SoundBankPlayer shootAudio;

	// Token: 0x04001605 RID: 5637
	[SerializeField]
	private float shootHapticStrength;

	// Token: 0x04001606 RID: 5638
	[SerializeField]
	private float shootHapticDuration;

	// Token: 0x04001607 RID: 5639
	private RubberDuckEvents _events;

	// Token: 0x04001608 RID: 5640
	private float currentShootCrankAmount;

	// Token: 0x04001609 RID: 5641
	private float currentClickCrankAmount;

	// Token: 0x0400160A RID: 5642
	private NetPlayer m_player;
}
