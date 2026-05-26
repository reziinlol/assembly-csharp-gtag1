using System;
using UnityEngine;

// Token: 0x02000485 RID: 1157
[CreateAssetMenu(fileName = "NexusGroupId", menuName = "Nexus/NexusGroupId")]
public class NexusGroupId : ScriptableObject
{
	// Token: 0x17000306 RID: 774
	// (get) Token: 0x06001C2F RID: 7215 RVA: 0x00098C07 File Offset: 0x00096E07
	public string Code
	{
		get
		{
			return this.code;
		}
	}

	// Token: 0x04002640 RID: 9792
	[SerializeField]
	private string code;

	// Token: 0x04002641 RID: 9793
	[SerializeField]
	private string sandboxCode;
}
