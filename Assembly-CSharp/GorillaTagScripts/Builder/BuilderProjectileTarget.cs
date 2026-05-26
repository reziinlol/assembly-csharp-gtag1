using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FB5 RID: 4021
	public class BuilderProjectileTarget : MonoBehaviour, IBuilderPieceFunctional
	{
		// Token: 0x06006475 RID: 25717 RVA: 0x00205FA0 File Offset: 0x002041A0
		private void Awake()
		{
			this.hitNotifier.OnProjectileHit += this.OnProjectileHit;
			foreach (Collider collider in this.colliders)
			{
				collider.contactOffset = 0.0001f;
			}
		}

		// Token: 0x06006476 RID: 25718 RVA: 0x0020600C File Offset: 0x0020420C
		private void OnDestroy()
		{
			this.hitNotifier.OnProjectileHit -= this.OnProjectileHit;
		}

		// Token: 0x06006477 RID: 25719 RVA: 0x00206025 File Offset: 0x00204225
		private void OnDisable()
		{
			this.hitCount = 0;
			if (this.scoreText != null)
			{
				this.scoreText.text = this.hitCount.ToString("D2");
			}
		}

		// Token: 0x06006478 RID: 25720 RVA: 0x00206058 File Offset: 0x00204258
		private void OnProjectileHit(SlingshotProjectile projectile, Collision collision)
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
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 11);
			}
		}

		// Token: 0x06006479 RID: 25721 RVA: 0x002060C6 File Offset: 0x002042C6
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
			if (newState == 11)
			{
				return;
			}
			this.lastHitTime = (double)Time.time;
			this.hitCount = Mathf.Clamp((int)newState, 0, 10);
			this.PlayHitEffects();
		}

		// Token: 0x0600647A RID: 25722 RVA: 0x00206100 File Offset: 0x00204300
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (instigator == null)
			{
				return;
			}
			if (newState != 11)
			{
				return;
			}
			this.hitCount++;
			this.hitCount %= 11;
			this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, (byte)this.hitCount, instigator.GetPlayerRef(), timeStamp);
		}

		// Token: 0x0600647B RID: 25723 RVA: 0x00206179 File Offset: 0x00204379
		public bool IsStateValid(byte state)
		{
			return state <= 11;
		}

		// Token: 0x0600647C RID: 25724 RVA: 0x00206184 File Offset: 0x00204384
		private void PlayHitEffects()
		{
			if (this.hitSoundbank != null)
			{
				this.hitSoundbank.Play();
			}
			if (this.hitAnimation != null && this.hitAnimation.clip != null)
			{
				this.hitAnimation.Play();
			}
			if (this.scoreText != null)
			{
				this.scoreText.text = this.hitCount.ToString("D2");
			}
		}

		// Token: 0x0600647D RID: 25725 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void FunctionalPieceUpdate()
		{
		}

		// Token: 0x0600647E RID: 25726 RVA: 0x00206200 File Offset: 0x00204400
		public float GetInteractionDistace()
		{
			return 20f;
		}

		// Token: 0x04007342 RID: 29506
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x04007343 RID: 29507
		[SerializeField]
		private SlingshotProjectileHitNotifier hitNotifier;

		// Token: 0x04007344 RID: 29508
		[SerializeField]
		protected float hitCooldown = 2f;

		// Token: 0x04007345 RID: 29509
		[Tooltip("Optional Sounds to play on hit")]
		[SerializeField]
		protected SoundBankPlayer hitSoundbank;

		// Token: 0x04007346 RID: 29510
		[Tooltip("Optional Sounds to play on hit")]
		[SerializeField]
		protected Animation hitAnimation;

		// Token: 0x04007347 RID: 29511
		[SerializeField]
		protected List<Collider> colliders;

		// Token: 0x04007348 RID: 29512
		[SerializeField]
		private TMP_Text scoreText;

		// Token: 0x04007349 RID: 29513
		private double lastHitTime;

		// Token: 0x0400734A RID: 29514
		private int hitCount;

		// Token: 0x0400734B RID: 29515
		private const byte MAX_SCORE = 10;

		// Token: 0x0400734C RID: 29516
		private const byte HIT = 11;
	}
}
