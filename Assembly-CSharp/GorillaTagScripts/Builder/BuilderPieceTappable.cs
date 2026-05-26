using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FAB RID: 4011
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(GorillaSurfaceOverride))]
	public class BuilderPieceTappable : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional, IBuilderTappable
	{
		// Token: 0x06006425 RID: 25637 RVA: 0x00204956 File Offset: 0x00202B56
		public virtual bool CanTap()
		{
			return this.isPieceActive && Time.time > this.lastTapTime + this.tapCooldown;
		}

		// Token: 0x06006426 RID: 25638 RVA: 0x00204976 File Offset: 0x00202B76
		public void OnTapLocal(float tapStrength)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			if (!this.CanTap())
			{
				return;
			}
			this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 1);
		}

		// Token: 0x06006427 RID: 25639 RVA: 0x002049AF File Offset: 0x00202BAF
		public virtual void OnTapReplicated()
		{
			UnityEvent onTapped = this.OnTapped;
			if (onTapped == null)
			{
				return;
			}
			onTapped.Invoke();
		}

		// Token: 0x06006428 RID: 25640 RVA: 0x002049C1 File Offset: 0x00202BC1
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.currentState = BuilderPieceTappable.FunctionalState.Idle;
		}

		// Token: 0x06006429 RID: 25641 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPieceDestroy()
		{
		}

		// Token: 0x0600642A RID: 25642 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x0600642B RID: 25643 RVA: 0x002049CA File Offset: 0x00202BCA
		public void OnPieceActivate()
		{
			this.isPieceActive = true;
		}

		// Token: 0x0600642C RID: 25644 RVA: 0x002049D4 File Offset: 0x00202BD4
		public void OnPieceDeactivate()
		{
			this.isPieceActive = false;
			if (this.currentState == BuilderPieceTappable.FunctionalState.Tap)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x0600642D RID: 25645 RVA: 0x00204A24 File Offset: 0x00202C24
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (newState == 1 && this.currentState != BuilderPieceTappable.FunctionalState.Tap)
			{
				this.lastTapTime = Time.time;
				this.OnTapReplicated();
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
			}
			this.currentState = (BuilderPieceTappable.FunctionalState)newState;
		}

		// Token: 0x0600642E RID: 25646 RVA: 0x00204A74 File Offset: 0x00202C74
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
			if (newState == 1 && this.CanTap())
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x0600642F RID: 25647 RVA: 0x00204ACF File Offset: 0x00202CCF
		public bool IsStateValid(byte state)
		{
			return state <= 1;
		}

		// Token: 0x06006430 RID: 25648 RVA: 0x00204AD8 File Offset: 0x00202CD8
		public void FunctionalPieceUpdate()
		{
			if (this.lastTapTime + this.tapCooldown < Time.time)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x040072FC RID: 29436
		[SerializeField]
		protected BuilderPiece myPiece;

		// Token: 0x040072FD RID: 29437
		[SerializeField]
		protected float tapCooldown = 0.5f;

		// Token: 0x040072FE RID: 29438
		private bool isPieceActive;

		// Token: 0x040072FF RID: 29439
		private float lastTapTime;

		// Token: 0x04007300 RID: 29440
		private BuilderPieceTappable.FunctionalState currentState;

		// Token: 0x04007301 RID: 29441
		[Tooltip("Called on all clients when this collider is tapped by anyone")]
		[SerializeField]
		protected UnityEvent OnTapped;

		// Token: 0x02000FAC RID: 4012
		private enum FunctionalState
		{
			// Token: 0x04007303 RID: 29443
			Idle,
			// Token: 0x04007304 RID: 29444
			Tap
		}
	}
}
