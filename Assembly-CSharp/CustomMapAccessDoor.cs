using System;
using UnityEngine;

// Token: 0x02000A2C RID: 2604
public class CustomMapAccessDoor : MonoBehaviour
{
	// Token: 0x06004296 RID: 17046 RVA: 0x00163F5A File Offset: 0x0016215A
	public void OpenDoor()
	{
		if (this.openDoorObject != null)
		{
			this.openDoorObject.SetActive(true);
		}
		if (this.closedDoorObject != null)
		{
			this.closedDoorObject.SetActive(false);
		}
	}

	// Token: 0x06004297 RID: 17047 RVA: 0x00163F90 File Offset: 0x00162190
	public void CloseDoor()
	{
		if (this.openDoorObject != null)
		{
			this.openDoorObject.SetActive(false);
		}
		if (this.closedDoorObject != null)
		{
			this.closedDoorObject.SetActive(true);
		}
	}

	// Token: 0x04005493 RID: 21651
	public GameObject openDoorObject;

	// Token: 0x04005494 RID: 21652
	public GameObject closedDoorObject;
}
