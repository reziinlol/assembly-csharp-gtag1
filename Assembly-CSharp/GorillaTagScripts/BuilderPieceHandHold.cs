using System;
using System.Collections.Generic;
using GorillaLocomotion.Gameplay;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000ECB RID: 3787
	[RequireComponent(typeof(Collider))]
	public class BuilderPieceHandHold : MonoBehaviour, IGorillaGrabable, IBuilderPieceComponent, ITickSystemTick
	{
		// Token: 0x06005D39 RID: 23865 RVA: 0x001D8EE1 File Offset: 0x001D70E1
		private void Initialize()
		{
			if (this.initialized)
			{
				return;
			}
			this.myCollider = base.GetComponent<Collider>();
			this.initialized = true;
		}

		// Token: 0x06005D3A RID: 23866 RVA: 0x001D8EFF File Offset: 0x001D70FF
		public bool IsHandHoldMoving()
		{
			return this.myPiece.IsPieceMoving();
		}

		// Token: 0x06005D3B RID: 23867 RVA: 0x001D8F0C File Offset: 0x001D710C
		public bool MomentaryGrabOnly()
		{
			return this.forceMomentary;
		}

		// Token: 0x06005D3C RID: 23868 RVA: 0x001D8F14 File Offset: 0x001D7114
		public virtual bool CanBeGrabbed(GorillaGrabber grabber)
		{
			return this.myPiece.state == BuilderPiece.State.AttachedAndPlaced && (!this.myPiece.GetTable().isTableMutable || grabber.Player.scale < 0.5f);
		}

		// Token: 0x06005D3D RID: 23869 RVA: 0x001D8F4C File Offset: 0x001D714C
		public void OnGrabbed(GorillaGrabber grabber, out Transform grabbedTransform, out Vector3 localGrabbedPosition)
		{
			this.Initialize();
			grabbedTransform = base.transform;
			Vector3 position = grabber.transform.position;
			localGrabbedPosition = base.transform.InverseTransformPoint(position);
			this.activeGrabbers.Add(grabber);
			this.isGrabbed = true;
			Vector3 vector;
			grabber.Player.AddHandHold(base.transform, localGrabbedPosition, grabber, grabber.IsRightHand, false, out vector);
		}

		// Token: 0x06005D3E RID: 23870 RVA: 0x001D8FB9 File Offset: 0x001D71B9
		public void OnGrabReleased(GorillaGrabber grabber)
		{
			this.Initialize();
			this.activeGrabbers.Remove(grabber);
			this.isGrabbed = (this.activeGrabbers.Count < 1);
			grabber.Player.RemoveHandHold(grabber, grabber.IsRightHand);
		}

		// Token: 0x170008FF RID: 2303
		// (get) Token: 0x06005D3F RID: 23871 RVA: 0x001D8FF4 File Offset: 0x001D71F4
		// (set) Token: 0x06005D40 RID: 23872 RVA: 0x001D8FFC File Offset: 0x001D71FC
		public bool TickRunning { get; set; }

		// Token: 0x06005D41 RID: 23873 RVA: 0x001D9008 File Offset: 0x001D7208
		public void Tick()
		{
			if (!this.isGrabbed)
			{
				return;
			}
			foreach (GorillaGrabber gorillaGrabber in this.activeGrabbers)
			{
				if (gorillaGrabber != null && gorillaGrabber.Player.scale > 0.5f)
				{
					this.OnGrabReleased(gorillaGrabber);
				}
			}
		}

		// Token: 0x06005D42 RID: 23874 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		// Token: 0x06005D43 RID: 23875 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06005D44 RID: 23876 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06005D45 RID: 23877 RVA: 0x001D9080 File Offset: 0x001D7280
		public void OnPieceActivate()
		{
			if (!this.TickRunning && this.myPiece.GetTable().isTableMutable)
			{
				TickSystem<object>.AddCallbackTarget(this);
			}
		}

		// Token: 0x06005D46 RID: 23878 RVA: 0x001D90A4 File Offset: 0x001D72A4
		public void OnPieceDeactivate()
		{
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveCallbackTarget(this);
			}
			foreach (GorillaGrabber grabber in this.activeGrabbers)
			{
				this.OnGrabReleased(grabber);
			}
		}

		// Token: 0x06005D48 RID: 23880 RVA: 0x00014807 File Offset: 0x00012A07
		string IGorillaGrabable.get_name()
		{
			return base.name;
		}

		// Token: 0x04006BCC RID: 27596
		private bool initialized;

		// Token: 0x04006BCD RID: 27597
		private Collider myCollider;

		// Token: 0x04006BCE RID: 27598
		[SerializeField]
		private bool forceMomentary = true;

		// Token: 0x04006BCF RID: 27599
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x04006BD0 RID: 27600
		private List<GorillaGrabber> activeGrabbers = new List<GorillaGrabber>(2);

		// Token: 0x04006BD1 RID: 27601
		private bool isGrabbed;
	}
}
