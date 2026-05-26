using System;
using UnityEngine;

// Token: 0x02000CDC RID: 3292
internal abstract class RPCNetworkBase : MonoBehaviour
{
	// Token: 0x06005198 RID: 20888
	public abstract void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler);
}
