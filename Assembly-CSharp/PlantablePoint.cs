using System;
using UnityEngine;

// Token: 0x02000361 RID: 865
public class PlantablePoint : MonoBehaviour
{
	// Token: 0x0600152D RID: 5421 RVA: 0x00070772 File Offset: 0x0006E972
	private void OnTriggerEnter(Collider other)
	{
		if ((this.floorMask & 1 << other.gameObject.layer) != 0)
		{
			this.plantableObject.SetPlanted(true);
		}
	}

	// Token: 0x0600152E RID: 5422 RVA: 0x0007079E File Offset: 0x0006E99E
	public void OnTriggerExit(Collider other)
	{
		if ((this.floorMask & 1 << other.gameObject.layer) != 0)
		{
			this.plantableObject.SetPlanted(false);
		}
	}

	// Token: 0x04001A01 RID: 6657
	public bool shouldBeSet;

	// Token: 0x04001A02 RID: 6658
	public LayerMask floorMask;

	// Token: 0x04001A03 RID: 6659
	public PlantableObject plantableObject;
}
