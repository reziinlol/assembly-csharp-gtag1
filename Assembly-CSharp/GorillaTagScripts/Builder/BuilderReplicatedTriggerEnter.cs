using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FB6 RID: 4022
	public class BuilderReplicatedTriggerEnter : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x06006480 RID: 25728 RVA: 0x0020621C File Offset: 0x0020441C
		private void Awake()
		{
			this.colliders.Clear();
			foreach (BuilderSmallHandTrigger builderSmallHandTrigger in this.handTriggers)
			{
				builderSmallHandTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnHandTriggerEntered));
				Collider component = builderSmallHandTrigger.GetComponent<Collider>();
				if (component != null)
				{
					this.colliders.Add(component);
				}
			}
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.bodyTriggers)
			{
				builderSmallMonkeTrigger.onPlayerEnteredTrigger += this.OnBodyTriggerEntered;
				Collider component2 = builderSmallMonkeTrigger.GetComponent<Collider>();
				if (component2 != null)
				{
					this.colliders.Add(component2);
				}
			}
		}

		// Token: 0x06006481 RID: 25729 RVA: 0x002062C8 File Offset: 0x002044C8
		private void OnDestroy()
		{
			BuilderSmallHandTrigger[] array = this.handTriggers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].TriggeredEvent.RemoveListener(new UnityAction(this.OnHandTriggerEntered));
			}
			BuilderSmallMonkeTrigger[] array2 = this.bodyTriggers;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].onPlayerEnteredTrigger -= this.OnBodyTriggerEntered;
			}
		}

		// Token: 0x06006482 RID: 25730 RVA: 0x0020632C File Offset: 0x0020452C
		private void PlayTriggerEffects(NetPlayer target)
		{
			UnityEvent onTriggered = this.OnTriggered;
			if (onTriggered != null)
			{
				onTriggered.Invoke();
			}
			if (this.animationOnTrigger != null && this.animationOnTrigger.clip != null)
			{
				this.animationOnTrigger.Rewind();
				this.animationOnTrigger.Play();
			}
			if (this.activateSoundBank != null)
			{
				this.activateSoundBank.Play();
			}
			if (target.IsLocal)
			{
				VRRig rig = VRRigCache.Instance.localRig.Rig;
				if (rig != null)
				{
					float num = 1.5f * rig.scaleFactor;
					if ((rig.transform.position - base.transform.position).sqrMagnitude > num * num)
					{
						return;
					}
					GTPlayer.Instance.SetMaximumSlipThisFrame();
					GTPlayer.Instance.ApplyKnockback(this.knockbackDirection.forward, this.knockbackVelocity * rig.scaleFactor, false);
					GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 2f, Time.fixedDeltaTime);
					GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 2f, Time.fixedDeltaTime);
				}
			}
		}

		// Token: 0x06006483 RID: 25731 RVA: 0x00206465 File Offset: 0x00204665
		private void OnHandTriggerEntered()
		{
			if (this.CanTrigger())
			{
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 1);
			}
		}

		// Token: 0x06006484 RID: 25732 RVA: 0x00206490 File Offset: 0x00204690
		private void OnBodyTriggerEntered(int playerNumber)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(playerNumber);
			if (player == null)
			{
				return;
			}
			if (this.CanTrigger())
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, player.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x06006485 RID: 25733 RVA: 0x002064F3 File Offset: 0x002046F3
		private bool CanTrigger()
		{
			return this.isPieceActive && this.currentState == BuilderReplicatedTriggerEnter.FunctionalState.Idle && Time.time > this.lastTriggerTime + this.triggerCooldown;
		}

		// Token: 0x06006486 RID: 25734 RVA: 0x0020651B File Offset: 0x0020471B
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.currentState = BuilderReplicatedTriggerEnter.FunctionalState.Idle;
		}

		// Token: 0x06006487 RID: 25735 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06006488 RID: 25736 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06006489 RID: 25737 RVA: 0x00206524 File Offset: 0x00204724
		public void OnPieceActivate()
		{
			this.isPieceActive = true;
			foreach (Collider collider in this.colliders)
			{
				collider.enabled = true;
			}
		}

		// Token: 0x0600648A RID: 25738 RVA: 0x0020657C File Offset: 0x0020477C
		public void OnPieceDeactivate()
		{
			this.isPieceActive = false;
			if (this.currentState == BuilderReplicatedTriggerEnter.FunctionalState.TriggerEntered)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
			foreach (Collider collider in this.colliders)
			{
				collider.enabled = false;
			}
		}

		// Token: 0x0600648B RID: 25739 RVA: 0x00206610 File Offset: 0x00204810
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (newState == 1 && this.currentState != BuilderReplicatedTriggerEnter.FunctionalState.TriggerEntered)
			{
				this.lastTriggerTime = Time.time;
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
				this.PlayTriggerEffects(instigator);
			}
			this.currentState = (BuilderReplicatedTriggerEnter.FunctionalState)newState;
		}

		// Token: 0x0600648C RID: 25740 RVA: 0x00206660 File Offset: 0x00204860
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
			if (newState == 1 && this.CanTrigger())
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x0600648D RID: 25741 RVA: 0x00204ACF File Offset: 0x00202CCF
		public bool IsStateValid(byte state)
		{
			return state <= 1;
		}

		// Token: 0x0600648E RID: 25742 RVA: 0x002066BC File Offset: 0x002048BC
		public void FunctionalPieceUpdate()
		{
			if (this.lastTriggerTime + this.triggerCooldown < Time.time)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x0400734D RID: 29517
		[SerializeField]
		protected BuilderPiece myPiece;

		// Token: 0x0400734E RID: 29518
		[Tooltip("How long in seconds to wait between trigger events")]
		[SerializeField]
		protected float triggerCooldown = 0.5f;

		// Token: 0x0400734F RID: 29519
		[SerializeField]
		private BuilderSmallHandTrigger[] handTriggers;

		// Token: 0x04007350 RID: 29520
		[SerializeField]
		private BuilderSmallMonkeTrigger[] bodyTriggers;

		// Token: 0x04007351 RID: 29521
		[Tooltip("Optional Animation to play when triggered")]
		[SerializeField]
		private Animation animationOnTrigger;

		// Token: 0x04007352 RID: 29522
		[Tooltip("Optional Sound to play when triggered")]
		[SerializeField]
		private SoundBankPlayer activateSoundBank;

		// Token: 0x04007353 RID: 29523
		[Tooltip("Knockback the triggering player?")]
		[SerializeField]
		private bool knockbackOnTriggerEnter;

		// Token: 0x04007354 RID: 29524
		[SerializeField]
		private float knockbackVelocity;

		// Token: 0x04007355 RID: 29525
		[Tooltip("uses Forward of the transform provided")]
		[SerializeField]
		private Transform knockbackDirection;

		// Token: 0x04007356 RID: 29526
		private List<Collider> colliders = new List<Collider>(5);

		// Token: 0x04007357 RID: 29527
		private bool isPieceActive;

		// Token: 0x04007358 RID: 29528
		private float lastTriggerTime;

		// Token: 0x04007359 RID: 29529
		private BuilderReplicatedTriggerEnter.FunctionalState currentState;

		// Token: 0x0400735A RID: 29530
		public UnityEvent OnTriggered;

		// Token: 0x02000FB7 RID: 4023
		private enum FunctionalState
		{
			// Token: 0x0400735C RID: 29532
			Idle,
			// Token: 0x0400735D RID: 29533
			TriggerEntered
		}
	}
}
