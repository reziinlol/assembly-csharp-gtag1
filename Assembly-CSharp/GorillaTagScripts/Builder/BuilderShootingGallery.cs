using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FBA RID: 4026
	public class BuilderShootingGallery : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x0600649E RID: 25758 RVA: 0x00206F28 File Offset: 0x00205128
		private void Awake()
		{
			foreach (Collider collider in this.colliders)
			{
				collider.contactOffset = 0.0001f;
			}
			this.wheelHitNotifier.OnProjectileHit += this.OnWheelHit;
			this.cowboyHitNotifier.OnProjectileHit += this.OnCowboyHit;
		}

		// Token: 0x0600649F RID: 25759 RVA: 0x00206FAC File Offset: 0x002051AC
		private void OnDestroy()
		{
			this.wheelHitNotifier.OnProjectileHit -= this.OnWheelHit;
			this.cowboyHitNotifier.OnProjectileHit -= this.OnCowboyHit;
		}

		// Token: 0x060064A0 RID: 25760 RVA: 0x00206FDC File Offset: 0x002051DC
		private void OnWheelHit(SlingshotProjectile projectile, Collision collision)
		{
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			if (projectile.projectileOwner == null || projectile.projectileOwner != NetworkSystem.Instance.LocalPlayer)
			{
				return;
			}
			if (this.lastHitTime + (double)this.hitCooldown < (double)Time.time)
			{
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 1);
			}
		}

		// Token: 0x060064A1 RID: 25761 RVA: 0x0020704C File Offset: 0x0020524C
		private void OnCowboyHit(SlingshotProjectile projectile, Collision collision)
		{
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			if (projectile.projectileOwner == null || projectile.projectileOwner != NetworkSystem.Instance.LocalPlayer)
			{
				return;
			}
			if (this.lastHitTime + (double)this.hitCooldown < (double)Time.time)
			{
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 2);
			}
		}

		// Token: 0x060064A2 RID: 25762 RVA: 0x002070BC File Offset: 0x002052BC
		private void CowboyHitEffects()
		{
			if (this.cowboyHitSound != null)
			{
				this.cowboyHitSound.Play();
			}
			if (this.cowboyHitAnimation != null && this.cowboyHitAnimation.clip != null)
			{
				this.cowboyHitAnimation.Play();
			}
		}

		// Token: 0x060064A3 RID: 25763 RVA: 0x00207110 File Offset: 0x00205310
		private void WheelHitEffects()
		{
			if (this.wheelHitSound != null)
			{
				this.wheelHitSound.Play();
			}
			if (this.wheelHitAnimation != null && this.wheelHitAnimation.clip != null)
			{
				this.wheelHitAnimation.Play();
			}
		}

		// Token: 0x060064A4 RID: 25764 RVA: 0x00207164 File Offset: 0x00205364
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.currentState = BuilderShootingGallery.FunctionalState.Idle;
			this.cowboyInitLocalPos = this.cowboyTransform.transform.localPosition;
			this.cowboyInitLocalRotation = this.cowboyTransform.transform.localRotation;
			this.wheelInitLocalRot = this.wheelTransform.transform.localRotation;
			this.distance = Vector3.Distance(this.cowboyStart.position, this.cowboyEnd.position);
			this.cowboyCycleDuration = this.distance / (this.cowboyVelocity * this.myPiece.GetScale());
			this.wheelCycleDuration = 1f / this.wheelVelocity;
		}

		// Token: 0x060064A5 RID: 25765 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPieceDestroy()
		{
		}

		// Token: 0x060064A6 RID: 25766 RVA: 0x0020720C File Offset: 0x0020540C
		public void OnPiecePlacementDeserialized()
		{
			if (!this.activated && this.myPiece.state == BuilderPiece.State.AttachedAndPlaced)
			{
				this.myPiece.GetTable().RegisterFunctionalPieceFixedUpdate(this);
				this.activated = true;
			}
		}

		// Token: 0x060064A7 RID: 25767 RVA: 0x0020723C File Offset: 0x0020543C
		public void OnPieceActivate()
		{
			this.cowboyTransform.SetLocalPositionAndRotation(this.cowboyInitLocalPos, this.cowboyInitLocalRotation);
			this.wheelTransform.SetLocalPositionAndRotation(this.wheelTransform.localPosition, this.wheelInitLocalRot);
			if (!this.activated)
			{
				this.myPiece.GetTable().RegisterFunctionalPieceFixedUpdate(this);
				this.activated = true;
			}
		}

		// Token: 0x060064A8 RID: 25768 RVA: 0x0020729C File Offset: 0x0020549C
		public void OnPieceDeactivate()
		{
			if (this.currentState != BuilderShootingGallery.FunctionalState.Idle)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
			if (this.activated)
			{
				this.myPiece.GetTable().UnregisterFunctionalPieceFixedUpdate(this);
				this.activated = false;
			}
			this.cowboyTransform.SetLocalPositionAndRotation(this.cowboyInitLocalPos, this.cowboyInitLocalRotation);
			this.wheelTransform.SetLocalPositionAndRotation(this.wheelTransform.localPosition, this.wheelInitLocalRot);
		}

		// Token: 0x060064A9 RID: 25769 RVA: 0x00207338 File Offset: 0x00205538
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (instigator == null)
			{
				return;
			}
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (newState == 1 && this.currentState == BuilderShootingGallery.FunctionalState.Idle)
			{
				this.lastHitTime = (double)Time.time;
				this.WheelHitEffects();
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
			}
			else if (newState == 2 && this.currentState == BuilderShootingGallery.FunctionalState.Idle)
			{
				this.lastHitTime = (double)Time.time;
				this.CowboyHitEffects();
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
			}
			this.currentState = (BuilderShootingGallery.FunctionalState)newState;
		}

		// Token: 0x060064AA RID: 25770 RVA: 0x002073BC File Offset: 0x002055BC
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (!this.IsStateValid(newState) || instigator == null)
			{
				return;
			}
			if (this.lastHitTime + (double)this.hitCooldown < (double)Time.time)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x060064AB RID: 25771 RVA: 0x00207421 File Offset: 0x00205621
		public bool IsStateValid(byte state)
		{
			return state <= 2;
		}

		// Token: 0x060064AC RID: 25772 RVA: 0x0020742C File Offset: 0x0020562C
		public void FunctionalPieceUpdate()
		{
			if (this.lastHitTime + (double)this.hitCooldown < (double)Time.time)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x060064AD RID: 25773 RVA: 0x00207480 File Offset: 0x00205680
		public void FunctionalPieceFixedUpdate()
		{
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			this.currT = this.CowboyCycleCompletionPercent();
			this.currForward = this.IsEvenCycle();
			float time = this.currForward ? this.currT : (1f - this.currT);
			float num = this.WheelCycleCompletionPercent();
			float t = this.cowboyCurve.Evaluate(time);
			this.cowboyTransform.localPosition = Vector3.Lerp(this.cowboyStart.localPosition, this.cowboyEnd.localPosition, t);
			Quaternion localRotation = Quaternion.AngleAxis(num * 360f, Vector3.right);
			this.wheelTransform.localRotation = localRotation;
		}

		// Token: 0x060064AE RID: 25774 RVA: 0x00207527 File Offset: 0x00205727
		private long NetworkTimeMs()
		{
			if (PhotonNetwork.InRoom)
			{
				return (long)((ulong)(PhotonNetwork.ServerTimestamp + int.MinValue));
			}
			return (long)(Time.time * 1000f);
		}

		// Token: 0x060064AF RID: 25775 RVA: 0x00207549 File Offset: 0x00205749
		private long CowboyCycleLengthMs()
		{
			return (long)(this.cowboyCycleDuration * 1000f);
		}

		// Token: 0x060064B0 RID: 25776 RVA: 0x00207558 File Offset: 0x00205758
		private long WheelCycleLengthMs()
		{
			return (long)(this.wheelCycleDuration * 1000f);
		}

		// Token: 0x060064B1 RID: 25777 RVA: 0x00207568 File Offset: 0x00205768
		public double CowboyPlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.CowboyCycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x060064B2 RID: 25778 RVA: 0x00207594 File Offset: 0x00205794
		public double WheelPlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.WheelCycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x060064B3 RID: 25779 RVA: 0x002075BF File Offset: 0x002057BF
		public int CowboyCycleCount()
		{
			return (int)(this.NetworkTimeMs() / this.CowboyCycleLengthMs());
		}

		// Token: 0x060064B4 RID: 25780 RVA: 0x002075CF File Offset: 0x002057CF
		public float CowboyCycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.CowboyPlatformTime() / (double)this.cowboyCycleDuration), 0f, 1f);
		}

		// Token: 0x060064B5 RID: 25781 RVA: 0x002075EF File Offset: 0x002057EF
		public float WheelCycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.WheelPlatformTime() / (double)this.wheelCycleDuration), 0f, 1f);
		}

		// Token: 0x060064B6 RID: 25782 RVA: 0x0020760F File Offset: 0x0020580F
		public bool IsEvenCycle()
		{
			return this.CowboyCycleCount() % 2 == 0;
		}

		// Token: 0x04007383 RID: 29571
		public BuilderPiece myPiece;

		// Token: 0x04007384 RID: 29572
		[SerializeField]
		private Transform wheelTransform;

		// Token: 0x04007385 RID: 29573
		[SerializeField]
		private Transform cowboyTransform;

		// Token: 0x04007386 RID: 29574
		[SerializeField]
		private SlingshotProjectileHitNotifier wheelHitNotifier;

		// Token: 0x04007387 RID: 29575
		[SerializeField]
		private SlingshotProjectileHitNotifier cowboyHitNotifier;

		// Token: 0x04007388 RID: 29576
		[SerializeField]
		protected List<Collider> colliders;

		// Token: 0x04007389 RID: 29577
		[SerializeField]
		protected SoundBankPlayer wheelHitSound;

		// Token: 0x0400738A RID: 29578
		[SerializeField]
		protected Animation wheelHitAnimation;

		// Token: 0x0400738B RID: 29579
		[SerializeField]
		protected SoundBankPlayer cowboyHitSound;

		// Token: 0x0400738C RID: 29580
		[SerializeField]
		private Animation cowboyHitAnimation;

		// Token: 0x0400738D RID: 29581
		[SerializeField]
		private float hitCooldown = 1f;

		// Token: 0x0400738E RID: 29582
		private double lastHitTime;

		// Token: 0x0400738F RID: 29583
		private BuilderShootingGallery.FunctionalState currentState;

		// Token: 0x04007390 RID: 29584
		private bool activated;

		// Token: 0x04007391 RID: 29585
		[SerializeField]
		private float cowboyVelocity;

		// Token: 0x04007392 RID: 29586
		[SerializeField]
		private Transform cowboyStart;

		// Token: 0x04007393 RID: 29587
		[SerializeField]
		private Transform cowboyEnd;

		// Token: 0x04007394 RID: 29588
		[SerializeField]
		private AnimationCurve cowboyCurve;

		// Token: 0x04007395 RID: 29589
		[SerializeField]
		private float wheelVelocity;

		// Token: 0x04007396 RID: 29590
		private Quaternion cowboyInitLocalRotation = Quaternion.identity;

		// Token: 0x04007397 RID: 29591
		private Vector3 cowboyInitLocalPos = Vector3.zero;

		// Token: 0x04007398 RID: 29592
		private Quaternion wheelInitLocalRot = Quaternion.identity;

		// Token: 0x04007399 RID: 29593
		private float cowboyCycleDuration;

		// Token: 0x0400739A RID: 29594
		private float wheelCycleDuration;

		// Token: 0x0400739B RID: 29595
		private float distance;

		// Token: 0x0400739C RID: 29596
		private float currT;

		// Token: 0x0400739D RID: 29597
		private bool currForward;

		// Token: 0x0400739E RID: 29598
		private float dtSinceServerUpdate;

		// Token: 0x0400739F RID: 29599
		private int lastServerTimeStamp;

		// Token: 0x040073A0 RID: 29600
		private float rotateStartAmt;

		// Token: 0x040073A1 RID: 29601
		private float rotateAmt;

		// Token: 0x02000FBB RID: 4027
		private enum FunctionalState
		{
			// Token: 0x040073A3 RID: 29603
			Idle,
			// Token: 0x040073A4 RID: 29604
			HitWheel,
			// Token: 0x040073A5 RID: 29605
			HitCowboy
		}
	}
}
