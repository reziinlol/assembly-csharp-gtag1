using System;
using UnityEngine;

// Token: 0x020000E6 RID: 230
[RequireComponent(typeof(GameTriggerInteractable))]
public class SIGadgetPumpBlaster : MonoBehaviour, SIGadgetBlasterType
{
	// Token: 0x06000560 RID: 1376 RVA: 0x0001E4A7 File Offset: 0x0001C6A7
	private bool CheckInput()
	{
		return this.blaster.CheckInput();
	}

	// Token: 0x06000561 RID: 1377 RVA: 0x0001E4B4 File Offset: 0x0001C6B4
	private void OnEnable()
	{
		this.blaster = base.GetComponent<SIGadgetBlaster>();
		this.triggerInteractable = base.GetComponent<GameTriggerInteractable>();
		this.strokeLength = (this.pumpFullyClosed.position - this.pumpFullyOpen.position).magnitude;
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x0001E504 File Offset: 0x0001C704
	public void OnUpdateAuthority(float dt)
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState != SIGadgetBlasterState.Idle)
		{
			if (currentState != SIGadgetBlasterState.Pumping)
			{
				return;
			}
			if (!this.triggerInteractable.triggerInteractionActive)
			{
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Idle);
			}
			Vector3 vector = this.pumpFullyOpen.position - this.pumpFullyClosed.position;
			Vector3 vector2 = this.pumpingTransform.position - this.pumpFullyClosed.position;
			if (Vector3.Dot(vector, vector2) < 0f)
			{
				vector2 = Vector3.zero;
			}
			Vector3 vector3 = Vector3.Project(vector2, vector);
			this.pumpHandlePosition.position = this.pumpFullyClosed.position + vector.normalized * Mathf.Clamp(vector3.magnitude, 0f, vector.magnitude);
			if (!this.pumpFullyOpened && vector3.magnitude > (1f - this.pumpThresholdPercent) * this.strokeLength)
			{
				this.pumpFullyOpened = true;
			}
			else if (this.pumpFullyOpen && vector3.magnitude < this.pumpThresholdPercent * this.strokeLength)
			{
				this.pumpFullyOpened = false;
				this.currentPumpChargeAmount = Mathf.Min(this.currentPumpChargeAmount + this.chargePerPump, this.maxPumpCharge);
			}
			if (this.CheckInput() && this.currentPumpChargeAmount > 0f)
			{
				this.AttemptFireProjectile(this.blaster.NextFireId(), this.currentPumpChargeAmount, this.blaster.firingPosition.position, this.blaster.firingPosition.rotation);
			}
		}
		else
		{
			if (this.triggerInteractable.triggerInteractionActive)
			{
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Pumping);
				return;
			}
			if (this.CheckInput() && this.currentPumpChargeAmount > 0f)
			{
				this.AttemptFireProjectile(this.blaster.NextFireId(), this.currentPumpChargeAmount, this.blaster.firingPosition.position, this.blaster.firingPosition.rotation);
				return;
			}
		}
	}

	// Token: 0x06000563 RID: 1379 RVA: 0x0001E700 File Offset: 0x0001C900
	public void OnUpdateRemote(float dt)
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState != SIGadgetBlasterState.Idle && currentState == SIGadgetBlasterState.Pumping)
		{
			Vector3 vector = this.pumpFullyOpen.position - this.pumpFullyClosed.position;
			Vector3 vector2 = this.pumpingTransform.position - this.pumpFullyClosed.position;
			if (Vector3.Dot(vector, vector2) < 0f)
			{
				vector2 = Vector3.zero;
			}
			Vector3 vector3 = Vector3.Project(vector2, vector);
			this.pumpHandlePosition.position = this.pumpFullyClosed.position + vector.normalized * Mathf.Clamp(vector3.magnitude, 0f, vector.magnitude);
			this.currentPumpChargeAmount = Mathf.Min(this.maxPumpCharge, this.currentPumpChargeAmount + Time.deltaTime * this.remotePumpChargePerSecond);
		}
	}

	// Token: 0x06000564 RID: 1380 RVA: 0x0001E7E0 File Offset: 0x0001C9E0
	public void SetStateShared()
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState == SIGadgetBlasterState.Idle)
		{
			this.blaster.blasterSource.clip = this.idleClip;
			this.blaster.blasterSource.volume = this.idleVolume;
			this.pumpingTransform = null;
			return;
		}
		if (currentState != SIGadgetBlasterState.Pumping)
		{
			return;
		}
		GameEntity gameEntity = this.blaster.gameEntity;
		GamePlayer gamePlayer;
		if (GamePlayer.TryGetGamePlayer(gameEntity.AttachedPlayerActorNr, out gamePlayer))
		{
			EHandedness equippedHandedness = gameEntity.EquippedHandedness;
			Transform transform;
			if (equippedHandedness != EHandedness.Left)
			{
				if (equippedHandedness != EHandedness.Right)
				{
					transform = this.pumpingTransform;
				}
				else
				{
					transform = gamePlayer.leftHand;
				}
			}
			else
			{
				transform = gamePlayer.rightHand;
			}
			this.pumpingTransform = transform;
		}
	}

	// Token: 0x06000565 RID: 1381 RVA: 0x0001E884 File Offset: 0x0001CA84
	public void AttemptFireProjectile(int fireId, float pumpChargeAmount, Vector3 position, Quaternion rotation)
	{
		if (pumpChargeAmount <= 0f)
		{
			return;
		}
		if (pumpChargeAmount - this.maxPumpDiff > this.currentPumpChargeAmount)
		{
			return;
		}
		if (this.blaster.projectileCount > this.blaster.maxProjectileCount)
		{
			return;
		}
		if (this.blaster.LocalEquippedOrActivated)
		{
			this.blaster.SendClientToClientRPC(0, new object[]
			{
				fireId,
				position,
				rotation
			});
		}
		this.currentPumpChargeAmount = Mathf.Min(this.maxPumpCharge, pumpChargeAmount);
		this.blaster.firingSource.time = 0f;
		this.blaster.firingSource.Play();
		this.blaster.firingSource.loop = false;
		this.blaster.InstantiateProjectile(this.projectilePrefab, position, rotation, fireId);
		this.currentPumpChargeAmount = 0f;
	}

	// Token: 0x06000566 RID: 1382 RVA: 0x0001E968 File Offset: 0x0001CB68
	public void NetworkFireProjectile(object[] data)
	{
		if (data == null || data.Length != 4)
		{
			return;
		}
		int fireId;
		if (!GameEntityManager.ValidateDataType<int>(data[0], out fireId))
		{
			return;
		}
		float pumpChargeAmount;
		if (!GameEntityManager.ValidateDataType<float>(data[1], out pumpChargeAmount))
		{
			return;
		}
		Vector3 position;
		if (!GameEntityManager.ValidateDataType<Vector3>(data[2], out position))
		{
			return;
		}
		Quaternion rotation;
		if (!GameEntityManager.ValidateDataType<Quaternion>(data[3], out rotation))
		{
			return;
		}
		this.AttemptFireProjectile(fireId, pumpChargeAmount, position, rotation);
	}

	// Token: 0x06000567 RID: 1383 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
	}

	// Token: 0x04000642 RID: 1602
	public SIGadgetBlasterProjectile projectilePrefab;

	// Token: 0x04000643 RID: 1603
	public AudioClip idleClip;

	// Token: 0x04000644 RID: 1604
	public AudioClip cooldownClip;

	// Token: 0x04000645 RID: 1605
	public float idleVolume;

	// Token: 0x04000646 RID: 1606
	public float cooldownVolume;

	// Token: 0x04000647 RID: 1607
	public AudioClip firingClip;

	// Token: 0x04000648 RID: 1608
	public float firingVolume;

	// Token: 0x04000649 RID: 1609
	public ParticleSystem fireFX;

	// Token: 0x0400064A RID: 1610
	public Transform pumpHandlePosition;

	// Token: 0x0400064B RID: 1611
	public Transform pumpFullyClosed;

	// Token: 0x0400064C RID: 1612
	public Transform pumpFullyOpen;

	// Token: 0x0400064D RID: 1613
	private GameTriggerInteractable triggerInteractable;

	// Token: 0x0400064E RID: 1614
	private SIGadgetBlaster blaster;

	// Token: 0x0400064F RID: 1615
	private Transform pumpingTransform;

	// Token: 0x04000650 RID: 1616
	public float currentPumpChargeAmount;

	// Token: 0x04000651 RID: 1617
	public float maxPumpCharge = 1f;

	// Token: 0x04000652 RID: 1618
	public float remotePumpChargePerSecond = 2f;

	// Token: 0x04000653 RID: 1619
	public float maxPumpDiff = 0.5f;

	// Token: 0x04000654 RID: 1620
	private float chargePerPump = 1f;

	// Token: 0x04000655 RID: 1621
	private bool pumpFullyOpened;

	// Token: 0x04000656 RID: 1622
	private float pumpThresholdPercent = 0.1f;

	// Token: 0x04000657 RID: 1623
	private float strokeLength;
}
