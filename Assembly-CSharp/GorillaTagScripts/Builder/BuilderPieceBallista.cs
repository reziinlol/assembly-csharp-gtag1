using System;
using System.Collections;
using System.Collections.Generic;
using CjLib;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FA2 RID: 4002
	public class BuilderPieceBallista : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x060063D0 RID: 25552 RVA: 0x00201E38 File Offset: 0x00200038
		private void Awake()
		{
			this.animator.SetFloat(this.pitchParamHash, this.pitch);
			this.appliedAnimatorPitch = this.pitch;
			this.launchDirection = this.launchEnd.position - this.launchStart.position;
			this.launchRampDistance = this.launchDirection.magnitude;
			this.launchDirection /= this.launchRampDistance;
			this.playerPullInRate = Mathf.Exp(this.playerMagnetismStrength);
			if (this.handTrigger != null)
			{
				this.handTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnHandTriggerPressed));
			}
			this.hasLaunchParticles = (this.launchParticles != null);
		}

		// Token: 0x060063D1 RID: 25553 RVA: 0x00201EFE File Offset: 0x002000FE
		private void OnDestroy()
		{
			if (this.handTrigger != null)
			{
				this.handTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnHandTriggerPressed));
			}
		}

		// Token: 0x060063D2 RID: 25554 RVA: 0x00201F2A File Offset: 0x0020012A
		private void OnHandTriggerPressed()
		{
			if (this.autoLaunch)
			{
				return;
			}
			if (this.ballistaState == BuilderPieceBallista.BallistaState.PlayerInTrigger)
			{
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 4);
			}
		}

		// Token: 0x060063D3 RID: 25555 RVA: 0x00201F60 File Offset: 0x00200160
		private void UpdateStateMaster()
		{
			if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
			switch (this.ballistaState)
			{
			case BuilderPieceBallista.BallistaState.Idle:
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				return;
			case BuilderPieceBallista.BallistaState.Loading:
				if (currentAnimatorStateInfo.shortNameHash == this.loadStateHash && (double)Time.time > this.loadCompleteTime)
				{
					if (this.playerInTrigger && this.playerRigInTrigger != null && (this.launchBigMonkes || (double)this.playerRigInTrigger.scaleFactor < 0.99))
					{
						this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 3, this.playerRigInTrigger.Creator.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
						return;
					}
					this.playerInTrigger = false;
					this.playerRigInTrigger = null;
					this.ballistaState = BuilderPieceBallista.BallistaState.WaitingForTrigger;
					return;
				}
				break;
			case BuilderPieceBallista.BallistaState.WaitingForTrigger:
				if (!this.playerInTrigger || this.playerRigInTrigger == null || (!this.launchBigMonkes && this.playerRigInTrigger.scaleFactor >= 0.99f))
				{
					this.playerInTrigger = false;
					this.playerRigInTrigger = null;
					return;
				}
				if (this.playerInTrigger)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 3, this.playerRigInTrigger.Creator.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			case BuilderPieceBallista.BallistaState.PlayerInTrigger:
				if (!this.playerInTrigger || this.playerRigInTrigger == null || (!this.launchBigMonkes && this.playerRigInTrigger.scaleFactor >= 0.99f))
				{
					this.playerInTrigger = false;
					this.playerRigInTrigger = null;
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 2, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				if (this.autoLaunch && (double)Time.time > this.enteredTriggerTime + (double)this.autoLaunchDelay)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 4, this.playerRigInTrigger.Creator.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			case BuilderPieceBallista.BallistaState.PrepareForLaunch:
			case BuilderPieceBallista.BallistaState.PrepareForLaunchLocal:
			{
				if (!this.playerInTrigger || this.playerRigInTrigger == null || (!this.launchBigMonkes && this.playerRigInTrigger.scaleFactor >= 0.99f))
				{
					this.playerInTrigger = false;
					this.playerRigInTrigger = null;
					this.ResetFlags();
					this.myPiece.functionalPieceState = 0;
					this.ballistaState = BuilderPieceBallista.BallistaState.Idle;
					return;
				}
				Vector3 playerBodyCenterPosition = this.GetPlayerBodyCenterPosition(this.playerRigInTrigger.transform, this.playerRigInTrigger.scaleFactor);
				Vector3 b = Vector3.Dot(playerBodyCenterPosition - this.launchStart.position, this.launchDirection) * this.launchDirection + this.launchStart.position;
				Vector3 b2 = playerBodyCenterPosition - b;
				if (Vector3.Lerp(Vector3.zero, b2, Mathf.Exp(-this.playerPullInRate * Time.deltaTime)).sqrMagnitude < this.playerReadyToFireDist * this.myPiece.GetScale() * this.playerReadyToFireDist * this.myPiece.GetScale())
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 6, this.playerRigInTrigger.Creator.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			}
			case BuilderPieceBallista.BallistaState.Launching:
			case BuilderPieceBallista.BallistaState.LaunchingLocal:
				if (currentAnimatorStateInfo.shortNameHash == this.idleStateHash)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x060063D4 RID: 25556 RVA: 0x0020236F File Offset: 0x0020056F
		private void ResetFlags()
		{
			this.playerLaunched = false;
			this.loadCompleteTime = double.MaxValue;
		}

		// Token: 0x060063D5 RID: 25557 RVA: 0x00202388 File Offset: 0x00200588
		private void UpdatePlayerPosition()
		{
			if (this.ballistaState != BuilderPieceBallista.BallistaState.PrepareForLaunchLocal && this.ballistaState != BuilderPieceBallista.BallistaState.LaunchingLocal)
			{
				return;
			}
			float deltaTime = Time.deltaTime;
			GTPlayer instance = GTPlayer.Instance;
			Vector3 playerBodyCenterPosition = this.GetPlayerBodyCenterPosition(instance.headCollider.transform, instance.scale);
			Vector3 lhs = playerBodyCenterPosition - this.launchStart.position;
			BuilderPieceBallista.BallistaState ballistaState = this.ballistaState;
			if (ballistaState == BuilderPieceBallista.BallistaState.PrepareForLaunchLocal)
			{
				Vector3 b = Vector3.Dot(lhs, this.launchDirection) * this.launchDirection + this.launchStart.position;
				Vector3 b2 = playerBodyCenterPosition - b;
				Vector3 a = Vector3.Lerp(Vector3.zero, b2, Mathf.Exp(-this.playerPullInRate * deltaTime));
				instance.transform.position = instance.transform.position + (a - b2);
				instance.SetPlayerVelocity(Vector3.zero);
				instance.SetMaximumSlipThisFrame();
				return;
			}
			if (ballistaState != BuilderPieceBallista.BallistaState.LaunchingLocal)
			{
				return;
			}
			if (!this.playerLaunched)
			{
				float num = Vector3.Dot(this.launchBone.position - this.launchStart.position, this.launchDirection) / this.launchRampDistance;
				float b3 = Vector3.Dot(lhs, this.launchDirection) / this.launchRampDistance;
				float num2 = 0.25f * this.myPiece.GetScale() / this.launchRampDistance;
				float num3 = Mathf.Max(num + num2, b3);
				float d = num3 * this.launchRampDistance;
				Vector3 a2 = this.launchDirection * d + this.launchStart.position;
				instance.transform.position + (a2 - playerBodyCenterPosition);
				instance.transform.position = instance.transform.position + (a2 - playerBodyCenterPosition);
				instance.SetPlayerVelocity(Vector3.zero);
				instance.SetMaximumSlipThisFrame();
				if (num3 >= 1f)
				{
					this.playerLaunched = true;
					this.launchedTime = (double)Time.time;
					instance.SetPlayerVelocity(this.launchSpeed * this.myPiece.GetScale() * this.launchDirection);
					instance.SetMaximumSlipThisFrame();
					return;
				}
			}
			else if ((double)Time.time < this.launchedTime + (double)this.slipOverrideDuration)
			{
				instance.SetMaximumSlipThisFrame();
			}
		}

		// Token: 0x060063D6 RID: 25558 RVA: 0x002025C0 File Offset: 0x002007C0
		private Vector3 GetPlayerBodyCenterPosition(Transform headTransform, float playerScale)
		{
			return headTransform.position + Quaternion.Euler(0f, headTransform.rotation.eulerAngles.y, 0f) * new Vector3(0f, 0f, this.playerBodyOffsetFromHead.z * playerScale) + Vector3.down * (this.playerBodyOffsetFromHead.y * playerScale);
		}

		// Token: 0x060063D7 RID: 25559 RVA: 0x00202638 File Offset: 0x00200838
		private void OnTriggerEnter(Collider other)
		{
			if (this.playerRigInTrigger != null)
			{
				return;
			}
			if (other.GetComponent<CapsuleCollider>() == null)
			{
				return;
			}
			if (other.attachedRigidbody == null)
			{
				return;
			}
			VRRig vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (vrrig == null)
			{
				if (!(GTPlayer.Instance.bodyCollider == other))
				{
					return;
				}
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (!this.launchBigMonkes && (double)vrrig.scaleFactor > 0.99)
			{
				return;
			}
			this.playerRigInTrigger = vrrig;
			this.playerInTrigger = true;
		}

		// Token: 0x060063D8 RID: 25560 RVA: 0x002026D8 File Offset: 0x002008D8
		private void OnTriggerExit(Collider other)
		{
			if (this.playerRigInTrigger == null || !this.playerInTrigger)
			{
				return;
			}
			if (other.GetComponent<CapsuleCollider>() == null)
			{
				return;
			}
			if (other.attachedRigidbody == null)
			{
				return;
			}
			VRRig vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (vrrig == null)
			{
				if (!(GTPlayer.Instance.bodyCollider == other))
				{
					return;
				}
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (this.playerRigInTrigger.Equals(vrrig))
			{
				this.playerInTrigger = false;
				this.playerRigInTrigger = null;
			}
		}

		// Token: 0x060063D9 RID: 25561 RVA: 0x00202770 File Offset: 0x00200970
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			if (!this.myPiece.GetTable().isTableMutable)
			{
				this.launchBigMonkes = true;
			}
			this.ballistaState = BuilderPieceBallista.BallistaState.Idle;
			this.playerInTrigger = false;
			this.playerRigInTrigger = null;
			this.playerLaunched = false;
		}

		// Token: 0x060063DA RID: 25562 RVA: 0x002027A7 File Offset: 0x002009A7
		public void OnPieceDestroy()
		{
			this.myPiece.functionalPieceState = 0;
			this.ballistaState = BuilderPieceBallista.BallistaState.Idle;
		}

		// Token: 0x060063DB RID: 25563 RVA: 0x002027BC File Offset: 0x002009BC
		public void OnPiecePlacementDeserialized()
		{
			this.launchDirection = this.launchEnd.position - this.launchStart.position;
			this.launchRampDistance = this.launchDirection.magnitude;
			this.launchDirection /= this.launchRampDistance;
		}

		// Token: 0x060063DC RID: 25564 RVA: 0x00202814 File Offset: 0x00200A14
		public void OnPieceActivate()
		{
			foreach (Collider collider in this.triggers)
			{
				collider.enabled = true;
			}
			this.animator.SetFloat(this.pitchParamHash, this.pitch);
			this.appliedAnimatorPitch = this.pitch;
			this.launchDirection = this.launchEnd.position - this.launchStart.position;
			this.launchRampDistance = this.launchDirection.magnitude;
			this.launchDirection /= this.launchRampDistance;
			this.myPiece.GetTable().RegisterFunctionalPiece(this);
		}

		// Token: 0x060063DD RID: 25565 RVA: 0x002028E4 File Offset: 0x00200AE4
		public void OnPieceDeactivate()
		{
			foreach (Collider collider in this.triggers)
			{
				collider.enabled = false;
			}
			if (this.hasLaunchParticles)
			{
				this.launchParticles.Stop();
				this.launchParticles.Clear();
			}
			this.myPiece.functionalPieceState = 0;
			this.ballistaState = BuilderPieceBallista.BallistaState.Idle;
			this.playerInTrigger = false;
			this.playerRigInTrigger = null;
			this.ResetFlags();
			this.myPiece.GetTable().UnregisterFunctionalPiece(this);
		}

		// Token: 0x060063DE RID: 25566 RVA: 0x0020298C File Offset: 0x00200B8C
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
			if ((BuilderPieceBallista.BallistaState)newState == this.ballistaState)
			{
				return;
			}
			if (newState == 4)
			{
				if (this.ballistaState == BuilderPieceBallista.BallistaState.PlayerInTrigger && this.playerInTrigger && this.playerRigInTrigger != null)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 4, this.playerRigInTrigger.Creator.GetPlayerRef(), timeStamp);
					return;
				}
			}
			else
			{
				Debug.LogWarning("BuilderPiece Ballista unexpected state request for " + newState.ToString());
			}
		}

		// Token: 0x060063DF RID: 25567 RVA: 0x00202A2C File Offset: 0x00200C2C
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			BuilderPieceBallista.BallistaState ballistaState = (BuilderPieceBallista.BallistaState)newState;
			if (ballistaState == this.ballistaState)
			{
				return;
			}
			switch (newState)
			{
			case 0:
				this.ResetFlags();
				goto IL_2C2;
			case 1:
				this.ResetFlags();
				foreach (Collider collider in this.disableWhileLaunching)
				{
					collider.enabled = true;
				}
				if (this.ballistaState == BuilderPieceBallista.BallistaState.Launching || this.ballistaState == BuilderPieceBallista.BallistaState.LaunchingLocal)
				{
					this.loadCompleteTime = (double)(Time.time + this.reloadDelay);
					if (this.loadSFX != null)
					{
						this.loadSFX.Play();
					}
				}
				else
				{
					this.loadCompleteTime = (double)(Time.time + this.loadTime);
				}
				this.animator.SetTrigger(this.loadTriggerHash);
				goto IL_2C2;
			case 2:
			case 5:
				goto IL_2C2;
			case 3:
				this.enteredTriggerTime = (double)Time.time;
				if (this.autoLaunch && this.cockSFX != null)
				{
					this.cockSFX.Play();
					goto IL_2C2;
				}
				goto IL_2C2;
			case 4:
			{
				this.playerLaunched = false;
				if (!this.autoLaunch && this.cockSFX != null)
				{
					this.cockSFX.Play();
				}
				if (!instigator.IsLocal)
				{
					goto IL_2C2;
				}
				GTPlayer instance = GTPlayer.Instance;
				if (Vector3.Distance(this.GetPlayerBodyCenterPosition(instance.headCollider.transform, instance.scale), this.launchStart.position) > this.prepareForLaunchDistance * this.myPiece.GetScale() || (!this.launchBigMonkes && (double)GorillaTagger.Instance.offlineVRRig.scaleFactor >= 0.99))
				{
					goto IL_2C2;
				}
				ballistaState = BuilderPieceBallista.BallistaState.PrepareForLaunchLocal;
				using (List<Collider>.Enumerator enumerator = this.disableWhileLaunching.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Collider collider2 = enumerator.Current;
						collider2.enabled = false;
					}
					goto IL_2C2;
				}
				break;
			}
			case 6:
				break;
			default:
				goto IL_2C2;
			}
			this.playerLaunched = false;
			this.animator.SetTrigger(this.fireTriggerHash);
			if (this.launchSFX != null)
			{
				this.launchSFX.Play();
			}
			if (this.hasLaunchParticles)
			{
				this.launchParticles.Play();
			}
			if (this.debugDrawTrajectoryOnLaunch)
			{
				base.StartCoroutine(this.DebugDrawTrajectory(8f));
			}
			if (instigator.IsLocal && this.ballistaState == BuilderPieceBallista.BallistaState.PrepareForLaunchLocal)
			{
				ballistaState = BuilderPieceBallista.BallistaState.LaunchingLocal;
				GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 4f);
				GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 4f);
			}
			IL_2C2:
			this.ballistaState = ballistaState;
		}

		// Token: 0x060063E0 RID: 25568 RVA: 0x00202D20 File Offset: 0x00200F20
		public bool IsStateValid(byte state)
		{
			return state < 8;
		}

		// Token: 0x060063E1 RID: 25569 RVA: 0x00202D26 File Offset: 0x00200F26
		public void FunctionalPieceUpdate()
		{
			if (this.myPiece == null || this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			if (NetworkSystem.Instance.IsMasterClient)
			{
				this.UpdateStateMaster();
			}
			this.UpdatePlayerPosition();
		}

		// Token: 0x060063E2 RID: 25570 RVA: 0x00202D5C File Offset: 0x00200F5C
		private void UpdatePredictionLine()
		{
			float d = 0.033333335f;
			Vector3 vector = this.launchEnd.position;
			Vector3 a = (this.launchEnd.position - this.launchStart.position).normalized * this.launchSpeed;
			for (int i = 0; i < 240; i++)
			{
				this.predictionLinePoints[i] = vector;
				vector += a * d;
				a += Vector3.down * 9.8f * d;
			}
		}

		// Token: 0x060063E3 RID: 25571 RVA: 0x00202DF6 File Offset: 0x00200FF6
		private IEnumerator DebugDrawTrajectory(float duration)
		{
			this.UpdatePredictionLine();
			float startTime = Time.time;
			while (Time.time < startTime + duration)
			{
				DebugUtil.DrawLine(this.launchStart.position, this.launchEnd.position, Color.yellow, true);
				DebugUtil.DrawLines(this.predictionLinePoints, Color.yellow, true);
				yield return null;
			}
			yield break;
		}

		// Token: 0x0400727A RID: 29306
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x0400727B RID: 29307
		[SerializeField]
		private List<Collider> triggers;

		// Token: 0x0400727C RID: 29308
		[SerializeField]
		private List<Collider> disableWhileLaunching;

		// Token: 0x0400727D RID: 29309
		[Tooltip("Trigger to start the launch if not autoLaunch")]
		[SerializeField]
		private BuilderSmallHandTrigger handTrigger;

		// Token: 0x0400727E RID: 29310
		[Tooltip("Should the player launch without a hand trigger press")]
		[SerializeField]
		private bool autoLaunch;

		// Token: 0x0400727F RID: 29311
		[SerializeField]
		private float autoLaunchDelay = 0.75f;

		// Token: 0x04007280 RID: 29312
		private double enteredTriggerTime;

		// Token: 0x04007281 RID: 29313
		public Animator animator;

		// Token: 0x04007282 RID: 29314
		public Transform launchStart;

		// Token: 0x04007283 RID: 29315
		public Transform launchEnd;

		// Token: 0x04007284 RID: 29316
		public Transform launchBone;

		// Token: 0x04007285 RID: 29317
		[SerializeField]
		private SoundBankPlayer loadSFX;

		// Token: 0x04007286 RID: 29318
		[SerializeField]
		private SoundBankPlayer launchSFX;

		// Token: 0x04007287 RID: 29319
		[SerializeField]
		private SoundBankPlayer cockSFX;

		// Token: 0x04007288 RID: 29320
		[SerializeField]
		private ParticleSystem launchParticles;

		// Token: 0x04007289 RID: 29321
		private bool hasLaunchParticles;

		// Token: 0x0400728A RID: 29322
		public float reloadDelay = 1f;

		// Token: 0x0400728B RID: 29323
		public float loadTime = 1.933f;

		// Token: 0x0400728C RID: 29324
		public float slipOverrideDuration = 0.1f;

		// Token: 0x0400728D RID: 29325
		private double launchedTime;

		// Token: 0x0400728E RID: 29326
		public float playerMagnetismStrength = 3f;

		// Token: 0x0400728F RID: 29327
		[Tooltip("Speed will be scaled by piece scale")]
		public float launchSpeed = 20f;

		// Token: 0x04007290 RID: 29328
		[Range(0f, 1f)]
		public float pitch;

		// Token: 0x04007291 RID: 29329
		private bool debugDrawTrajectoryOnLaunch;

		// Token: 0x04007292 RID: 29330
		private int loadTriggerHash = Animator.StringToHash("Load");

		// Token: 0x04007293 RID: 29331
		private int fireTriggerHash = Animator.StringToHash("Fire");

		// Token: 0x04007294 RID: 29332
		private int pitchParamHash = Animator.StringToHash("Pitch");

		// Token: 0x04007295 RID: 29333
		private int idleStateHash = Animator.StringToHash("Idle");

		// Token: 0x04007296 RID: 29334
		private int loadStateHash = Animator.StringToHash("Load");

		// Token: 0x04007297 RID: 29335
		private int fireStateHash = Animator.StringToHash("Fire");

		// Token: 0x04007298 RID: 29336
		private bool playerInTrigger;

		// Token: 0x04007299 RID: 29337
		private VRRig playerRigInTrigger;

		// Token: 0x0400729A RID: 29338
		private bool playerLaunched;

		// Token: 0x0400729B RID: 29339
		private float playerReadyToFireDist = 1.6667f;

		// Token: 0x0400729C RID: 29340
		private float prepareForLaunchDistance = 2.5f;

		// Token: 0x0400729D RID: 29341
		private Vector3 launchDirection;

		// Token: 0x0400729E RID: 29342
		private float launchRampDistance;

		// Token: 0x0400729F RID: 29343
		private float playerPullInRate;

		// Token: 0x040072A0 RID: 29344
		private float appliedAnimatorPitch;

		// Token: 0x040072A1 RID: 29345
		private bool launchBigMonkes;

		// Token: 0x040072A2 RID: 29346
		private Vector3 playerBodyOffsetFromHead = new Vector3(0f, -0.4f, -0.15f);

		// Token: 0x040072A3 RID: 29347
		private double loadCompleteTime;

		// Token: 0x040072A4 RID: 29348
		private BuilderPieceBallista.BallistaState ballistaState;

		// Token: 0x040072A5 RID: 29349
		private const int predictionLineSamples = 240;

		// Token: 0x040072A6 RID: 29350
		private Vector3[] predictionLinePoints = new Vector3[240];

		// Token: 0x02000FA3 RID: 4003
		private enum BallistaState
		{
			// Token: 0x040072A8 RID: 29352
			Idle,
			// Token: 0x040072A9 RID: 29353
			Loading,
			// Token: 0x040072AA RID: 29354
			WaitingForTrigger,
			// Token: 0x040072AB RID: 29355
			PlayerInTrigger,
			// Token: 0x040072AC RID: 29356
			PrepareForLaunch,
			// Token: 0x040072AD RID: 29357
			PrepareForLaunchLocal,
			// Token: 0x040072AE RID: 29358
			Launching,
			// Token: 0x040072AF RID: 29359
			LaunchingLocal,
			// Token: 0x040072B0 RID: 29360
			Count
		}
	}
}
