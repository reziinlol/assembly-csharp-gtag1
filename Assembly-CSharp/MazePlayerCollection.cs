using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A3 RID: 419
public class MazePlayerCollection : MonoBehaviour
{
	// Token: 0x06000B43 RID: 2883 RVA: 0x0003C1EC File Offset: 0x0003A3EC
	private void Start()
	{
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x0003C20F File Offset: 0x0003A40F
	private void OnDestroy()
	{
		NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerLeftRoom;
	}

	// Token: 0x06000B45 RID: 2885 RVA: 0x0003C234 File Offset: 0x0003A434
	public void OnTriggerEnter(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (!this.containedRigs.Contains(component))
		{
			this.containedRigs.Add(component);
		}
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x0003C284 File Offset: 0x0003A484
	public void OnTriggerExit(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (this.containedRigs.Contains(component))
		{
			this.containedRigs.Remove(component);
		}
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x0003C2D8 File Offset: 0x0003A4D8
	public void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		this.containedRigs.RemoveAll((VRRig r) => ((r != null) ? r.creator : null) == null || r.creator == otherPlayer);
	}

	// Token: 0x04000D8B RID: 3467
	public List<VRRig> containedRigs = new List<VRRig>();

	// Token: 0x04000D8C RID: 3468
	public List<MonkeyeAI> monkeyeAis = new List<MonkeyeAI>();
}
