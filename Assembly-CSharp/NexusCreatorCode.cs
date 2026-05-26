using System;
using UnityEngine;

// Token: 0x02000484 RID: 1156
[CreateAssetMenu(fileName = "NexusCreatorCode", menuName = "Nexus/NexusCreatorCode")]
public class NexusCreatorCode : ScriptableObject
{
	// Token: 0x17000304 RID: 772
	// (get) Token: 0x06001C2C RID: 7212 RVA: 0x00098BF7 File Offset: 0x00096DF7
	public string Code
	{
		get
		{
			return this.code;
		}
	}

	// Token: 0x17000305 RID: 773
	// (get) Token: 0x06001C2D RID: 7213 RVA: 0x00098BFF File Offset: 0x00096DFF
	public NexusGroupId GroupId
	{
		get
		{
			return this.groupId;
		}
	}

	// Token: 0x0400263E RID: 9790
	[SerializeField]
	private string code;

	// Token: 0x0400263F RID: 9791
	[SerializeField]
	private NexusGroupId groupId;
}
