using System;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200022B RID: 555
public class RCShip : RCHoverboard
{
	// Token: 0x06000EAC RID: 3756 RVA: 0x00050016 File Offset: 0x0004E216
	private byte GetDataB()
	{
		if (!this.hasNetworkSync)
		{
			return 0;
		}
		return this.networkSync.syncedState.dataB;
	}

	// Token: 0x06000EAD RID: 3757 RVA: 0x00050032 File Offset: 0x0004E232
	private void SetDataB(byte b)
	{
		if (this.hasNetworkSync)
		{
			this.networkSync.syncedState.dataB = b;
		}
	}

	// Token: 0x06000EAE RID: 3758 RVA: 0x00050050 File Offset: 0x0004E250
	private void WriteCannonBit(bool toLeft)
	{
		if (!this.hasNetworkSync)
		{
			return;
		}
		byte b = this.GetDataB();
		b = (toLeft ? (b | 1) : ((byte)((int)b & -2)));
		this.SetDataB(b);
	}

	// Token: 0x06000EAF RID: 3759 RVA: 0x00050083 File Offset: 0x0004E283
	private bool ReadCannonBit()
	{
		if (!this.hasNetworkSync)
		{
			return this.cannonToLeft;
		}
		return (this.GetDataB() & 1) > 0;
	}

	// Token: 0x06000EB0 RID: 3760 RVA: 0x0005009F File Offset: 0x0004E29F
	private bool ReadFireFlip()
	{
		return (this.GetDataB() & 2) > 0;
	}

	// Token: 0x06000EB1 RID: 3761 RVA: 0x000500AC File Offset: 0x0004E2AC
	protected override void AuthorityUpdate(float dt)
	{
		base.AuthorityUpdate(dt);
		float trigger = this.activeInput.trigger;
		float num = (float)this.activeInput.buttons;
		if (this.localState == RCVehicle.State.Mobilized && this.localStatePrev != RCVehicle.State.Mobilized)
		{
			this.armedAfterMobilize = false;
			if (trigger >= this.triggerReleaseThreshold)
			{
				this.triggerIsDown = true;
			}
		}
		if (this.localState == RCVehicle.State.Mobilized)
		{
			if (!this.armedAfterMobilize && trigger <= this.triggerReleaseThreshold)
			{
				this.armedAfterMobilize = true;
				this.triggerIsDown = false;
			}
			if (this.armedAfterMobilize)
			{
				if (!this.triggerIsDown && trigger >= this.triggerPressThreshold)
				{
					this.triggerIsDown = true;
					UnityEvent onFire = this.OnFire;
					if (onFire != null)
					{
						onFire.Invoke();
					}
					if (this.hasNetworkSync)
					{
						byte b = this.GetDataB();
						b ^= 2;
						this.SetDataB(b);
						this.lastFireFlip = ((b & 2) > 0);
					}
				}
				else if (this.triggerIsDown && trigger <= this.triggerReleaseThreshold)
				{
					this.triggerIsDown = false;
				}
			}
			if (!this.faceIsDown && num >= this.facePressThreshold)
			{
				this.faceIsDown = true;
				this.cannonToLeft = !this.cannonToLeft;
				this.WriteCannonBit(this.cannonToLeft);
			}
			else if (this.faceIsDown && num <= this.faceReleaseThreshold)
			{
				this.faceIsDown = false;
			}
		}
		else
		{
			if (this.faceIsDown && num <= this.faceReleaseThreshold)
			{
				this.faceIsDown = false;
			}
			this.armedAfterMobilize = false;
			if (this.triggerIsDown && trigger <= this.triggerReleaseThreshold)
			{
				this.triggerIsDown = false;
			}
		}
		if (this.hasNetworkSync)
		{
			byte b2 = this.GetDataB();
			if (this.localState == RCVehicle.State.Mobilized && this.rb != null && this.rb.linearVelocity.sqrMagnitude >= this.movingSpeedThreshold * this.movingSpeedThreshold)
			{
				b2 |= 4;
				this.isMovingShared = true;
			}
			else
			{
				b2 = (byte)((int)b2 & -5);
				this.isMovingShared = false;
			}
			this.SetDataB(b2);
			return;
		}
		this.isMovingShared = (this.localState == RCVehicle.State.Mobilized && this.rb != null && this.rb.linearVelocity.sqrMagnitude >= this.movingSpeedThreshold * this.movingSpeedThreshold);
	}

	// Token: 0x06000EB2 RID: 3762 RVA: 0x000502E8 File Offset: 0x0004E4E8
	protected override void RemoteUpdate(float dt)
	{
		base.RemoteUpdate(dt);
		if (!this.hasNetworkSync)
		{
			return;
		}
		this.cannonToLeft = this.ReadCannonBit();
		bool flag = this.ReadFireFlip();
		if (!base.HasLocalAuthority)
		{
			if (flag != this.lastFireFlip)
			{
				this.lastFireFlip = flag;
				UnityEvent onFire = this.OnFire;
				if (onFire != null)
				{
					onFire.Invoke();
				}
			}
			byte dataB = this.GetDataB();
			this.isMovingShared = ((dataB & 4) > 0);
			return;
		}
		this.lastFireFlip = flag;
		this.isMovingShared = (this.localState == RCVehicle.State.Mobilized && this.rb != null && this.rb.linearVelocity.sqrMagnitude >= this.movingSpeedThreshold * this.movingSpeedThreshold);
	}

	// Token: 0x06000EB3 RID: 3763 RVA: 0x000503A4 File Offset: 0x0004E5A4
	protected override void SharedUpdate(float dt)
	{
		base.SharedUpdate(dt);
		if (this.cannonTransform != null)
		{
			float target = this.cannonToLeft ? this.leftYaw : this.rightYaw;
			Vector3 localEulerAngles = this.cannonTransform.localEulerAngles;
			localEulerAngles.z = Mathf.MoveTowardsAngle(localEulerAngles.z, target, this.cannonYawSpeed * dt);
			this.cannonTransform.localEulerAngles = localEulerAngles;
		}
		if (this.cannonToLeft != this.lastCannonToLeft)
		{
			this.lastCannonToLeft = this.cannonToLeft;
			UnityEvent<bool> onCannonSideChanged = this.OnCannonSideChanged;
			if (onCannonSideChanged != null)
			{
				onCannonSideChanged.Invoke(this.cannonToLeft);
			}
		}
		bool flag = this.localState == RCVehicle.State.Mobilized && this.isMovingShared;
		if (flag != this.lastIsMoving)
		{
			this.lastIsMoving = flag;
			if (flag)
			{
				UnityEvent onMoveStarted = this.OnMoveStarted;
				if (onMoveStarted == null)
				{
					return;
				}
				onMoveStarted.Invoke();
				return;
			}
			else
			{
				UnityEvent onMoveStopped = this.OnMoveStopped;
				if (onMoveStopped == null)
				{
					return;
				}
				onMoveStopped.Invoke();
			}
		}
	}

	// Token: 0x040011A2 RID: 4514
	[Header("RCShip - Events")]
	public UnityEvent OnFire;

	// Token: 0x040011A3 RID: 4515
	public UnityEvent<bool> OnCannonSideChanged;

	// Token: 0x040011A4 RID: 4516
	public UnityEvent OnMoveStarted;

	// Token: 0x040011A5 RID: 4517
	public UnityEvent OnMoveStopped;

	// Token: 0x040011A6 RID: 4518
	[Header("RCShip - Cannon Rotation")]
	[SerializeField]
	private Transform cannonTransform;

	// Token: 0x040011A7 RID: 4519
	[SerializeField]
	private float leftYaw = -45f;

	// Token: 0x040011A8 RID: 4520
	[SerializeField]
	private float rightYaw = 45f;

	// Token: 0x040011A9 RID: 4521
	[SerializeField]
	private float cannonYawSpeed = 240f;

	// Token: 0x040011AA RID: 4522
	[Header("RCShip - Input")]
	[Range(0f, 1f)]
	[SerializeField]
	private float triggerPressThreshold = 0.6f;

	// Token: 0x040011AB RID: 4523
	[Range(0f, 1f)]
	[SerializeField]
	private float triggerReleaseThreshold = 0.1f;

	// Token: 0x040011AC RID: 4524
	[Range(0f, 1f)]
	[SerializeField]
	private float facePressThreshold = 0.6f;

	// Token: 0x040011AD RID: 4525
	[Range(0f, 1f)]
	[SerializeField]
	private float faceReleaseThreshold = 0.1f;

	// Token: 0x040011AE RID: 4526
	[Header("RCShip - Movement Detection")]
	[Tooltip("Minimum speed to consider the ship moving")]
	[SerializeField]
	private float movingSpeedThreshold = 0.05f;

	// Token: 0x040011AF RID: 4527
	private bool prevTriggerDown;

	// Token: 0x040011B0 RID: 4528
	private bool prevFaceDown;

	// Token: 0x040011B1 RID: 4529
	private bool faceIsDown;

	// Token: 0x040011B2 RID: 4530
	private bool triggerIsDown;

	// Token: 0x040011B3 RID: 4531
	private bool armedAfterMobilize;

	// Token: 0x040011B4 RID: 4532
	private bool cannonToLeft;

	// Token: 0x040011B5 RID: 4533
	private const byte CannonLeftBit = 1;

	// Token: 0x040011B6 RID: 4534
	private const byte FireFlipBit = 2;

	// Token: 0x040011B7 RID: 4535
	private const byte MovingBit = 4;

	// Token: 0x040011B8 RID: 4536
	private bool lastFireFlip;

	// Token: 0x040011B9 RID: 4537
	private bool lastCannonToLeft;

	// Token: 0x040011BA RID: 4538
	private bool lastIsMoving;

	// Token: 0x040011BB RID: 4539
	private bool isMovingShared;
}
