using System;
using UnityEngine;

// Token: 0x020003C0 RID: 960
public class XSceneRefTarget : MonoBehaviour
{
	// Token: 0x0600170E RID: 5902 RVA: 0x0008590D File Offset: 0x00083B0D
	private void Awake()
	{
		this.Register(false);
	}

	// Token: 0x0600170F RID: 5903 RVA: 0x00085916 File Offset: 0x00083B16
	private void Reset()
	{
		this.UniqueID = XSceneRefTarget.CreateNewID();
	}

	// Token: 0x06001710 RID: 5904 RVA: 0x00085923 File Offset: 0x00083B23
	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			this.Register(false);
		}
	}

	// Token: 0x06001711 RID: 5905 RVA: 0x00085934 File Offset: 0x00083B34
	public void Register(bool force = false)
	{
		if (this.UniqueID == this.lastRegisteredID && !force)
		{
			return;
		}
		if (this.lastRegisteredID != -1)
		{
			XSceneRefGlobalHub.Unregister(this.lastRegisteredID, this);
		}
		XSceneRefGlobalHub.Register(this.UniqueID, this);
		this.lastRegisteredID = this.UniqueID;
	}

	// Token: 0x06001712 RID: 5906 RVA: 0x00085980 File Offset: 0x00083B80
	private void OnDestroy()
	{
		XSceneRefGlobalHub.Unregister(this.UniqueID, this);
	}

	// Token: 0x06001713 RID: 5907 RVA: 0x0008598E File Offset: 0x00083B8E
	private void AssignNewID()
	{
		this.UniqueID = XSceneRefTarget.CreateNewID();
		this.Register(false);
	}

	// Token: 0x06001714 RID: 5908 RVA: 0x000859A4 File Offset: 0x00083BA4
	public static int CreateNewID()
	{
		int num = (int)((DateTime.Now - XSceneRefTarget.epoch).TotalSeconds * 8.0 % 2147483646.0) + 1;
		if (num <= XSceneRefTarget.lastAssignedID)
		{
			XSceneRefTarget.lastAssignedID++;
			return XSceneRefTarget.lastAssignedID;
		}
		XSceneRefTarget.lastAssignedID = num;
		return num;
	}

	// Token: 0x04002244 RID: 8772
	public int UniqueID;

	// Token: 0x04002245 RID: 8773
	[NonSerialized]
	private int lastRegisteredID = -1;

	// Token: 0x04002246 RID: 8774
	private static DateTime epoch = new DateTime(2024, 1, 1);

	// Token: 0x04002247 RID: 8775
	private static int lastAssignedID;
}
