using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Critters.Scripts
{
	// Token: 0x0200131F RID: 4895
	public class CrittersFoodDispenser : CrittersActor
	{
		// Token: 0x06007B5D RID: 31581 RVA: 0x00284702 File Offset: 0x00282902
		public override void Initialize()
		{
			base.Initialize();
			this.heldByPlayer = false;
		}

		// Token: 0x06007B5E RID: 31582 RVA: 0x00284711 File Offset: 0x00282911
		public override void GrabbedBy(CrittersActor grabbingActor, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
		{
			base.GrabbedBy(grabbingActor, positionOverride, localRotation, localOffset, disableGrabbing);
			this.heldByPlayer = grabbingActor.isOnPlayer;
		}

		// Token: 0x06007B5F RID: 31583 RVA: 0x0028472C File Offset: 0x0028292C
		protected override void RemoteGrabbedBy(CrittersActor grabbingActor)
		{
			base.RemoteGrabbedBy(grabbingActor);
			this.heldByPlayer = grabbingActor.isOnPlayer;
		}

		// Token: 0x06007B60 RID: 31584 RVA: 0x00284741 File Offset: 0x00282941
		public override void Released(bool keepWorldPosition, Quaternion rotation = default(Quaternion), Vector3 position = default(Vector3), Vector3 impulseVelocity = default(Vector3), Vector3 impulseAngularVelocity = default(Vector3))
		{
			base.Released(keepWorldPosition, rotation, position, impulseVelocity, impulseAngularVelocity);
			this.heldByPlayer = false;
		}

		// Token: 0x06007B61 RID: 31585 RVA: 0x00284757 File Offset: 0x00282957
		protected override void HandleRemoteReleased()
		{
			base.HandleRemoteReleased();
			this.heldByPlayer = false;
		}

		// Token: 0x04008CBE RID: 36030
		[FormerlySerializedAs("isHeldByPlayer")]
		public bool heldByPlayer;
	}
}
