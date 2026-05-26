using System;
using BoingKit;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FA7 RID: 4007
	public class BuilderPieceDoorSwinging : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x06006400 RID: 25600 RVA: 0x00203A9C File Offset: 0x00201C9C
		private void Awake()
		{
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.doorHoldTriggers)
			{
				builderSmallMonkeTrigger.onTriggerFirstEntered += this.OnHoldTriggerEntered;
				builderSmallMonkeTrigger.onTriggerLastExited += this.OnHoldTriggerExited;
			}
			this.frontTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnFrontTriggerEntered));
			this.backTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnBackTriggerEntered));
		}

		// Token: 0x06006401 RID: 25601 RVA: 0x00203B1C File Offset: 0x00201D1C
		private void OnDestroy()
		{
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.doorHoldTriggers)
			{
				builderSmallMonkeTrigger.onTriggerFirstEntered -= this.OnHoldTriggerEntered;
				builderSmallMonkeTrigger.onTriggerLastExited -= this.OnHoldTriggerExited;
			}
			this.frontTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnFrontTriggerEntered));
			this.backTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnBackTriggerEntered));
		}

		// Token: 0x06006402 RID: 25602 RVA: 0x00203B9C File Offset: 0x00201D9C
		private void OnFrontTriggerEntered()
		{
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				if (NetworkSystem.Instance.IsMasterClient)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 7, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 7);
			}
		}

		// Token: 0x06006403 RID: 25603 RVA: 0x00203C10 File Offset: 0x00201E10
		private void OnBackTriggerEntered()
		{
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				if (NetworkSystem.Instance.IsMasterClient)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 3, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 3);
			}
		}

		// Token: 0x06006404 RID: 25604 RVA: 0x00203C84 File Offset: 0x00201E84
		private void OnHoldTriggerEntered()
		{
			this.peopleInHoldOpenVolume = true;
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			BuilderPieceDoorSwinging.SwingingDoorState swingingDoorState = this.currentState;
			if (swingingDoorState != BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				if (swingingDoorState == BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut)
				{
					this.openSound.Play();
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 4, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				if (swingingDoorState != BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn)
				{
					return;
				}
				this.openSound.Play();
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 8, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x06006405 RID: 25605 RVA: 0x00203D34 File Offset: 0x00201F34
		private void OnHoldTriggerExited()
		{
			this.peopleInHoldOpenVolume = false;
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.doorHoldTriggers)
			{
				builderSmallMonkeTrigger.ValidateOverlappingColliders();
				if (builderSmallMonkeTrigger.overlapCount > 0)
				{
					this.peopleInHoldOpenVolume = true;
					break;
				}
			}
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn && !this.peopleInHoldOpenVolume)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 5, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				return;
			}
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut && !this.peopleInHoldOpenVolume)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x06006406 RID: 25606 RVA: 0x00203E08 File Offset: 0x00202008
		private void SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState value)
		{
			bool flag = this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed;
			bool flag2 = value == BuilderPieceDoorSwinging.SwingingDoorState.Closed;
			this.currentState = value;
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				this.frontTrigger.enabled = true;
				this.backTrigger.enabled = true;
			}
			else
			{
				this.frontTrigger.enabled = false;
				this.backTrigger.enabled = false;
			}
			if (flag != flag2)
			{
				if (flag2)
				{
					this.myPiece.GetTable().UnregisterFunctionalPiece(this);
					return;
				}
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
			}
		}

		// Token: 0x06006407 RID: 25607 RVA: 0x00203E90 File Offset: 0x00202090
		private void UpdateDoorStateMaster()
		{
			switch (this.currentState)
			{
			case BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn:
				if (Mathf.Abs(this.doorSpring.Value) < 1f && Mathf.Abs(this.doorSpring.Velocity) < this.doorClosedVelocityMag)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.Closed);
					return;
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenIn:
				if (Time.time - this.tLastOpened > this.timeUntilDoorCloses)
				{
					this.peopleInHoldOpenVolume = false;
					foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.doorHoldTriggers)
					{
						builderSmallMonkeTrigger.ValidateOverlappingColliders();
						if (builderSmallMonkeTrigger.overlapCount > 0)
						{
							this.peopleInHoldOpenVolume = true;
							break;
						}
					}
					if (this.peopleInHoldOpenVolume)
					{
						BuilderPieceDoorSwinging.SwingingDoorState swingingDoorState = (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.OpenIn) ? BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn : BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut;
						this.checkHoldTriggersTime = (double)(Time.time + this.checkHoldTriggersDelay);
						this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, (byte)swingingDoorState, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
						return;
					}
					BuilderPieceDoorSwinging.SwingingDoorState swingingDoorState2 = (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.OpenIn) ? BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn : BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut;
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, (byte)swingingDoorState2, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningOut:
				if (Mathf.Abs(this.doorSpring.Value) > 89f)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.OpenOut);
					return;
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn:
				if ((double)Time.time > this.checkHoldTriggersTime)
				{
					foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger2 in this.doorHoldTriggers)
					{
						builderSmallMonkeTrigger2.ValidateOverlappingColliders();
						if (builderSmallMonkeTrigger2.overlapCount > 0)
						{
							this.peopleInHoldOpenVolume = true;
							break;
						}
					}
					if (this.peopleInHoldOpenVolume)
					{
						this.checkHoldTriggersTime = (double)(Time.time + this.checkHoldTriggersDelay);
						return;
					}
					BuilderPieceDoorSwinging.SwingingDoorState swingingDoorState3 = (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn) ? BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn : BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut;
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, (byte)swingingDoorState3, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningIn:
				if (Mathf.Abs(this.doorSpring.Value) > 89f)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.OpenIn);
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06006408 RID: 25608 RVA: 0x002040D8 File Offset: 0x002022D8
		private void UpdateDoorState()
		{
			switch (this.currentState)
			{
			case BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn:
				if (Mathf.Abs(this.doorSpring.Value) < 1f && Mathf.Abs(this.doorSpring.Velocity) < this.doorClosedVelocityMag)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.Closed);
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenIn:
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningOut:
				if (Mathf.Abs(this.doorSpring.Value) > 89f)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.OpenOut);
					return;
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningIn:
				if (Mathf.Abs(this.doorSpring.Value) > 89f)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.OpenIn);
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06006409 RID: 25609 RVA: 0x00204188 File Offset: 0x00202388
		private void CloseDoor()
		{
			switch (this.currentState)
			{
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut:
				this.closeSound.Play();
				this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut);
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn:
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningIn:
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenIn:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn:
				this.closeSound.Play();
				this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn);
				return;
			default:
				return;
			}
		}

		// Token: 0x0600640A RID: 25610 RVA: 0x002041E6 File Offset: 0x002023E6
		private void OpenDoor(bool openIn)
		{
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				this.tLastOpened = Time.time;
				this.openSound.Play();
				this.SetDoorState(openIn ? BuilderPieceDoorSwinging.SwingingDoorState.OpeningIn : BuilderPieceDoorSwinging.SwingingDoorState.OpeningOut);
			}
		}

		// Token: 0x0600640B RID: 25611 RVA: 0x00204214 File Offset: 0x00202414
		private void UpdateDoorAnimation()
		{
			switch (this.currentState)
			{
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut:
				this.doorSpring.TrackDampingRatio(-90f, 3.1415927f * this.doorOpenSpeed, 1f, Time.deltaTime);
				this.doorTransform.localRotation = Quaternion.Euler(this.rotateAxis * this.doorSpring.Value);
				if (this.isDoubleDoor && this.doorTransformB != null)
				{
					this.doorTransformB.localRotation = Quaternion.Euler(this.rotateAxisB * this.doorSpring.Value);
					return;
				}
				return;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenIn:
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningIn:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn:
				this.doorSpring.TrackDampingRatio(90f, 3.1415927f * this.doorOpenSpeed, 1f, Time.deltaTime);
				this.doorTransform.localRotation = Quaternion.Euler(this.rotateAxis * this.doorSpring.Value);
				if (this.isDoubleDoor && this.doorTransformB != null)
				{
					this.doorTransformB.localRotation = Quaternion.Euler(this.rotateAxisB * this.doorSpring.Value);
					return;
				}
				return;
			}
			this.doorSpring.TrackDampingRatio(0f, 3.1415927f * this.doorCloseSpeed, this.dampingRatio, Time.deltaTime);
			this.doorTransform.localRotation = Quaternion.Euler(this.rotateAxis * this.doorSpring.Value);
			if (this.isDoubleDoor && this.doorTransformB != null)
			{
				this.doorTransformB.localRotation = Quaternion.Euler(this.rotateAxisB * this.doorSpring.Value);
			}
		}

		// Token: 0x0600640C RID: 25612 RVA: 0x00204404 File Offset: 0x00202604
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.tLastOpened = 0f;
			this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.Closed);
			this.doorSpring.Reset();
			Collider[] array = this.triggerVolumes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
		}

		// Token: 0x0600640D RID: 25613 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPieceDestroy()
		{
		}

		// Token: 0x0600640E RID: 25614 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x0600640F RID: 25615 RVA: 0x0020444C File Offset: 0x0020264C
		public void OnPieceActivate()
		{
			Collider[] array = this.triggerVolumes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
		}

		// Token: 0x06006410 RID: 25616 RVA: 0x00204478 File Offset: 0x00202678
		public void OnPieceDeactivate()
		{
			Collider[] array = this.triggerVolumes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			this.myPiece.functionalPieceState = 0;
			this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.Closed);
			this.doorSpring.Reset();
			this.doorTransform.localRotation = Quaternion.Euler(this.rotateAxis * this.doorSpring.Value);
			if (this.isDoubleDoor && this.doorTransformB != null)
			{
				this.doorTransformB.localRotation = Quaternion.Euler(this.rotateAxisB * this.doorSpring.Value);
			}
		}

		// Token: 0x06006411 RID: 25617 RVA: 0x00204524 File Offset: 0x00202724
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			switch (newState)
			{
			case 1:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.OpenOut || this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut)
				{
					this.CloseDoor();
				}
				break;
			case 3:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
				{
					this.OpenDoor(false);
				}
				break;
			case 4:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut)
				{
					this.openSound.Play();
				}
				break;
			case 5:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.OpenIn || this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn)
				{
					this.CloseDoor();
				}
				break;
			case 7:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
				{
					this.OpenDoor(true);
				}
				break;
			case 8:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn)
				{
					this.openSound.Play();
				}
				break;
			}
			this.SetDoorState((BuilderPieceDoorSwinging.SwingingDoorState)newState);
		}

		// Token: 0x06006412 RID: 25618 RVA: 0x002045F4 File Offset: 0x002027F4
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (this.IsStateValid(newState) && instigator != null && (newState == 7 || newState == 3) && this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x06006413 RID: 25619 RVA: 0x00204652 File Offset: 0x00202852
		public bool IsStateValid(byte state)
		{
			return state <= 8;
		}

		// Token: 0x06006414 RID: 25620 RVA: 0x0020465C File Offset: 0x0020285C
		public void FunctionalPieceUpdate()
		{
			if (this.myPiece != null && this.myPiece.state == BuilderPiece.State.AttachedAndPlaced)
			{
				if (!NetworkSystem.Instance.InRoom && this.currentState != BuilderPieceDoorSwinging.SwingingDoorState.Closed)
				{
					this.CloseDoor();
				}
				else if (NetworkSystem.Instance.IsMasterClient)
				{
					this.UpdateDoorStateMaster();
				}
				else
				{
					this.UpdateDoorState();
				}
				this.UpdateDoorAnimation();
			}
		}

		// Token: 0x040072D4 RID: 29396
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x040072D5 RID: 29397
		[SerializeField]
		private Vector3 rotateAxis = Vector3.up;

		// Token: 0x040072D6 RID: 29398
		[SerializeField]
		private Transform doorTransform;

		// Token: 0x040072D7 RID: 29399
		[SerializeField]
		private Collider[] triggerVolumes;

		// Token: 0x040072D8 RID: 29400
		[SerializeField]
		private BuilderSmallMonkeTrigger[] doorHoldTriggers;

		// Token: 0x040072D9 RID: 29401
		[SerializeField]
		private BuilderSmallHandTrigger frontTrigger;

		// Token: 0x040072DA RID: 29402
		[SerializeField]
		private BuilderSmallHandTrigger backTrigger;

		// Token: 0x040072DB RID: 29403
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x040072DC RID: 29404
		[SerializeField]
		private SoundBankPlayer openSound;

		// Token: 0x040072DD RID: 29405
		[SerializeField]
		private SoundBankPlayer closeSound;

		// Token: 0x040072DE RID: 29406
		[SerializeField]
		private float doorOpenSpeed = 1f;

		// Token: 0x040072DF RID: 29407
		[SerializeField]
		private float doorCloseSpeed = 1f;

		// Token: 0x040072E0 RID: 29408
		[SerializeField]
		[Range(1.5f, 10f)]
		private float timeUntilDoorCloses = 3f;

		// Token: 0x040072E1 RID: 29409
		[SerializeField]
		private float doorClosedVelocityMag = 30f;

		// Token: 0x040072E2 RID: 29410
		[SerializeField]
		private float dampingRatio = 0.5f;

		// Token: 0x040072E3 RID: 29411
		[Header("Double Door Settings")]
		[SerializeField]
		private bool isDoubleDoor;

		// Token: 0x040072E4 RID: 29412
		[SerializeField]
		private Vector3 rotateAxisB = Vector3.down;

		// Token: 0x040072E5 RID: 29413
		[SerializeField]
		private Transform doorTransformB;

		// Token: 0x040072E6 RID: 29414
		private BuilderPieceDoorSwinging.SwingingDoorState currentState;

		// Token: 0x040072E7 RID: 29415
		private float tLastOpened;

		// Token: 0x040072E8 RID: 29416
		private FloatSpring doorSpring;

		// Token: 0x040072E9 RID: 29417
		private bool peopleInHoldOpenVolume;

		// Token: 0x040072EA RID: 29418
		private double checkHoldTriggersTime;

		// Token: 0x040072EB RID: 29419
		private float checkHoldTriggersDelay = 3f;

		// Token: 0x040072EC RID: 29420
		private int pushDirection = 1;

		// Token: 0x02000FA8 RID: 4008
		private enum SwingingDoorState
		{
			// Token: 0x040072EE RID: 29422
			Closed,
			// Token: 0x040072EF RID: 29423
			ClosingOut,
			// Token: 0x040072F0 RID: 29424
			OpenOut,
			// Token: 0x040072F1 RID: 29425
			OpeningOut,
			// Token: 0x040072F2 RID: 29426
			HeldOpenOut,
			// Token: 0x040072F3 RID: 29427
			ClosingIn,
			// Token: 0x040072F4 RID: 29428
			OpenIn,
			// Token: 0x040072F5 RID: 29429
			OpeningIn,
			// Token: 0x040072F6 RID: 29430
			HeldOpenIn
		}
	}
}
