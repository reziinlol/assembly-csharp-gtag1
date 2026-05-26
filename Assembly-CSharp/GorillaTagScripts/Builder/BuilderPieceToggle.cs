using System;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FAE RID: 4014
	public class BuilderPieceToggle : MonoBehaviour, IBuilderPieceFunctional, IBuilderPieceComponent, IBuilderTappable
	{
		// Token: 0x06006441 RID: 25665 RVA: 0x00204FD0 File Offset: 0x002031D0
		private void Awake()
		{
			this.colliders.Clear();
			if (this.toggleType == BuilderPieceToggle.ToggleType.OnTriggerEnter)
			{
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
		}

		// Token: 0x06006442 RID: 25666 RVA: 0x00205088 File Offset: 0x00203288
		private void OnDestroy()
		{
			foreach (BuilderSmallHandTrigger builderSmallHandTrigger in this.handTriggers)
			{
				if (!(builderSmallHandTrigger == null))
				{
					builderSmallHandTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnHandTriggerEntered));
				}
			}
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.bodyTriggers)
			{
				if (!(builderSmallMonkeTrigger == null))
				{
					builderSmallMonkeTrigger.onPlayerEnteredTrigger -= this.OnBodyTriggerEntered;
				}
			}
		}

		// Token: 0x06006443 RID: 25667 RVA: 0x00205108 File Offset: 0x00203308
		private bool CanTap()
		{
			return (!this.onlySmallMonkeTaps || !this.myPiece.GetTable().isTableMutable || (double)VRRigCache.Instance.localRig.Rig.scaleFactor <= 0.99) && this.toggleType == BuilderPieceToggle.ToggleType.OnTap && this.myPiece.state == BuilderPiece.State.AttachedAndPlaced;
		}

		// Token: 0x06006444 RID: 25668 RVA: 0x00205169 File Offset: 0x00203369
		public void OnTapLocal(float tapStrength)
		{
			if (!this.CanTap())
			{
				Debug.Log("BuilderPieceToggle Can't Tap");
				return;
			}
			Debug.Log("Tap Local");
			this.ToggleStateRequest();
		}

		// Token: 0x06006445 RID: 25669 RVA: 0x0020518E File Offset: 0x0020338E
		private bool CanTrigger()
		{
			return this.toggleType == BuilderPieceToggle.ToggleType.OnTriggerEnter && this.myPiece.state == BuilderPiece.State.AttachedAndPlaced;
		}

		// Token: 0x06006446 RID: 25670 RVA: 0x002051A9 File Offset: 0x002033A9
		private void OnHandTriggerEntered()
		{
			if (this.CanTrigger())
			{
				this.ToggleStateRequest();
				return;
			}
			Debug.Log("BuilderPieceToggle Can't Trigger");
		}

		// Token: 0x06006447 RID: 25671 RVA: 0x002051C4 File Offset: 0x002033C4
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
				this.ToggleStateMaster(player.GetPlayerRef());
				return;
			}
			Debug.Log("BuilderPieceToggle Can't Trigger");
		}

		// Token: 0x06006448 RID: 25672 RVA: 0x00205210 File Offset: 0x00203410
		private void ToggleStateRequest()
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			BuilderPieceToggle.ToggleStates toggleStates = (this.toggleState == BuilderPieceToggle.ToggleStates.Off) ? BuilderPieceToggle.ToggleStates.On : BuilderPieceToggle.ToggleStates.Off;
			Debug.Log("BuilderPieceToggle" + string.Format(" Requesting state {0}", toggleStates));
			this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, (byte)toggleStates);
		}

		// Token: 0x06006449 RID: 25673 RVA: 0x00205278 File Offset: 0x00203478
		private void ToggleStateMaster(Player instigator)
		{
			BuilderPieceToggle.ToggleStates toggleStates = (this.toggleState == BuilderPieceToggle.ToggleStates.Off) ? BuilderPieceToggle.ToggleStates.On : BuilderPieceToggle.ToggleStates.Off;
			Debug.Log("BuilderPieceToggle" + string.Format(" Set Master state {0}", toggleStates));
			this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, (byte)toggleStates, instigator, NetworkSystem.Instance.ServerTimestamp);
		}

		// Token: 0x0600644A RID: 25674 RVA: 0x002052E0 File Offset: 0x002034E0
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				Debug.Log("BuilderPieceToggle State Invalid");
				return;
			}
			Debug.Log("BuilderPieceToggle" + string.Format(" State Changed {0}", newState));
			if ((BuilderPieceToggle.ToggleStates)newState != this.toggleState)
			{
				if (newState == 1)
				{
					Debug.Log("BuilderPieceToggle Toggled On");
					UnityEvent toggledOn = this.ToggledOn;
					if (toggledOn != null)
					{
						toggledOn.Invoke();
					}
				}
				else
				{
					Debug.Log("BuilderPieceToggle Toggled Off");
					this.ToggledOff.Invoke();
				}
			}
			this.toggleState = (BuilderPieceToggle.ToggleStates)newState;
		}

		// Token: 0x0600644B RID: 25675 RVA: 0x00205368 File Offset: 0x00203568
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (!this.IsStateValid(newState) || instigator == null)
			{
				Debug.Log("BuilderPieceToggle State Invalid or Player Null");
				return;
			}
			Debug.Log("BuilderPieceToggle" + string.Format(" State Request {0}", newState));
			if (newState != (byte)this.toggleState)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
				return;
			}
			Debug.Log("BuilderPieceToggle Same State");
		}

		// Token: 0x0600644C RID: 25676 RVA: 0x002053F5 File Offset: 0x002035F5
		public bool IsStateValid(byte state)
		{
			Debug.Log(string.Format("Is State Valid? {0}", state));
			return state <= 1;
		}

		// Token: 0x0600644D RID: 25677 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void FunctionalPieceUpdate()
		{
		}

		// Token: 0x0600644E RID: 25678 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		// Token: 0x0600644F RID: 25679 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06006450 RID: 25680 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06006451 RID: 25681 RVA: 0x00205414 File Offset: 0x00203614
		public void OnPieceActivate()
		{
			foreach (Collider collider in this.colliders)
			{
				collider.enabled = true;
			}
		}

		// Token: 0x06006452 RID: 25682 RVA: 0x00205468 File Offset: 0x00203668
		public void OnPieceDeactivate()
		{
			this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			foreach (Collider collider in this.colliders)
			{
				collider.enabled = false;
			}
		}

		// Token: 0x04007310 RID: 29456
		[SerializeField]
		protected BuilderPiece myPiece;

		// Token: 0x04007311 RID: 29457
		[SerializeField]
		private BuilderPieceToggle.ToggleType toggleType;

		// Token: 0x04007312 RID: 29458
		public bool onlySmallMonkeTaps;

		// Token: 0x04007313 RID: 29459
		[SerializeField]
		private BuilderSmallHandTrigger[] handTriggers;

		// Token: 0x04007314 RID: 29460
		[SerializeField]
		private BuilderSmallMonkeTrigger[] bodyTriggers;

		// Token: 0x04007315 RID: 29461
		[SerializeField]
		protected UnityEvent ToggledOn;

		// Token: 0x04007316 RID: 29462
		[SerializeField]
		protected UnityEvent ToggledOff;

		// Token: 0x04007317 RID: 29463
		private List<Collider> colliders = new List<Collider>(5);

		// Token: 0x04007318 RID: 29464
		private BuilderPieceToggle.ToggleStates toggleState;

		// Token: 0x02000FAF RID: 4015
		[Serializable]
		private enum ToggleType
		{
			// Token: 0x0400731A RID: 29466
			OnTap,
			// Token: 0x0400731B RID: 29467
			OnTriggerEnter
		}

		// Token: 0x02000FB0 RID: 4016
		private enum ToggleStates
		{
			// Token: 0x0400731D RID: 29469
			Off,
			// Token: 0x0400731E RID: 29470
			On
		}
	}
}
