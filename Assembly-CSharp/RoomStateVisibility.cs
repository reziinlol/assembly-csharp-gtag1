using System;
using UnityEngine;

// Token: 0x02000395 RID: 917
public class RoomStateVisibility : MonoBehaviour
{
	// Token: 0x0600164B RID: 5707 RVA: 0x00081693 File Offset: 0x0007F893
	private void Start()
	{
		this.OnRoomChanged();
		RoomSystem.JoinedRoomEvent += new Action(this.OnRoomChanged);
		RoomSystem.LeftRoomEvent += new Action(this.OnRoomChanged);
	}

	// Token: 0x0600164C RID: 5708 RVA: 0x000816D1 File Offset: 0x0007F8D1
	private void OnDestroy()
	{
		RoomSystem.JoinedRoomEvent -= new Action(this.OnRoomChanged);
		RoomSystem.LeftRoomEvent -= new Action(this.OnRoomChanged);
	}

	// Token: 0x0600164D RID: 5709 RVA: 0x0008170C File Offset: 0x0007F90C
	private void OnRoomChanged()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			base.gameObject.SetActive(this.enableOutOfRoom);
			return;
		}
		if (NetworkSystem.Instance.SessionIsPrivate)
		{
			base.gameObject.SetActive(this.enableInPrivateRoom);
			return;
		}
		base.gameObject.SetActive(this.enableInRoom);
	}

	// Token: 0x04002065 RID: 8293
	[SerializeField]
	private bool enableOutOfRoom;

	// Token: 0x04002066 RID: 8294
	[SerializeField]
	private bool enableInRoom = true;

	// Token: 0x04002067 RID: 8295
	[SerializeField]
	private bool enableInPrivateRoom = true;
}
