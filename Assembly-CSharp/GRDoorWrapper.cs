using System;
using UnityEngine;

// Token: 0x020003CD RID: 973
public class GRDoorWrapper : MonoBehaviour
{
	// Token: 0x06001734 RID: 5940 RVA: 0x00085FE8 File Offset: 0x000841E8
	public void ToggleDoor(bool value)
	{
		this.grDoor.SetDoorState(value ? GRDoor.DoorState.Open : GRDoor.DoorState.Closed);
	}

	// Token: 0x0400226E RID: 8814
	[SerializeField]
	private GRDoor grDoor;
}
