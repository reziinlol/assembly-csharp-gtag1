using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x02000ACF RID: 2767
[Serializable]
[StructLayout(LayoutKind.Explicit)]
public struct GlobalObjectRef
{
	// Token: 0x060046A5 RID: 18085 RVA: 0x0017E624 File Offset: 0x0017C824
	public static GlobalObjectRef ObjectToRefSlow(Object target)
	{
		return default(GlobalObjectRef);
	}

	// Token: 0x060046A6 RID: 18086 RVA: 0x00035D0D File Offset: 0x00033F0D
	public static Object RefToObjectSlow(GlobalObjectRef @ref)
	{
		return null;
	}

	// Token: 0x04005921 RID: 22817
	[FieldOffset(0)]
	public ulong targetObjectId;

	// Token: 0x04005922 RID: 22818
	[FieldOffset(8)]
	public ulong targetPrefabId;

	// Token: 0x04005923 RID: 22819
	[FieldOffset(16)]
	public Guid assetGUID;

	// Token: 0x04005924 RID: 22820
	[HideInInspector]
	[FieldOffset(32)]
	public int identifierType;

	// Token: 0x04005925 RID: 22821
	[NonSerialized]
	[FieldOffset(32)]
	private GlobalObjectRefType refType;
}
