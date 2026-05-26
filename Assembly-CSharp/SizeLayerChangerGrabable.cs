using System;
using GorillaLocomotion.Gameplay;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200090D RID: 2317
public class SizeLayerChangerGrabable : MonoBehaviour, IGorillaGrabable
{
	// Token: 0x06003C94 RID: 15508 RVA: 0x00149E7A File Offset: 0x0014807A
	public bool MomentaryGrabOnly()
	{
		return this.momentaryGrabOnly;
	}

	// Token: 0x06003C95 RID: 15509 RVA: 0x00023994 File Offset: 0x00021B94
	bool IGorillaGrabable.CanBeGrabbed(GorillaGrabber grabber)
	{
		return true;
	}

	// Token: 0x06003C96 RID: 15510 RVA: 0x00149E84 File Offset: 0x00148084
	void IGorillaGrabable.OnGrabbed(GorillaGrabber g, out Transform grabbedObject, out Vector3 grabbedLocalPosiiton)
	{
		if (this.grabChangesSizeLayer)
		{
			RigContainer rigContainer;
			VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer);
			rigContainer.Rig.sizeManager.currentSizeLayerMaskValue = this.grabbedSizeLayerMask.Mask;
		}
		grabbedObject = base.transform;
		grabbedLocalPosiiton = base.transform.InverseTransformPoint(g.transform.position);
	}

	// Token: 0x06003C97 RID: 15511 RVA: 0x00149EEC File Offset: 0x001480EC
	void IGorillaGrabable.OnGrabReleased(GorillaGrabber g)
	{
		if (this.releaseChangesSizeLayer)
		{
			RigContainer rigContainer;
			VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer);
			rigContainer.Rig.sizeManager.currentSizeLayerMaskValue = this.releasedSizeLayerMask.Mask;
		}
	}

	// Token: 0x06003C99 RID: 15513 RVA: 0x00014807 File Offset: 0x00012A07
	string IGorillaGrabable.get_name()
	{
		return base.name;
	}

	// Token: 0x04004D3C RID: 19772
	[SerializeField]
	private bool grabChangesSizeLayer = true;

	// Token: 0x04004D3D RID: 19773
	[SerializeField]
	private bool releaseChangesSizeLayer = true;

	// Token: 0x04004D3E RID: 19774
	[SerializeField]
	private SizeLayerMask grabbedSizeLayerMask;

	// Token: 0x04004D3F RID: 19775
	[SerializeField]
	private SizeLayerMask releasedSizeLayerMask;

	// Token: 0x04004D40 RID: 19776
	[SerializeField]
	private bool momentaryGrabOnly = true;
}
