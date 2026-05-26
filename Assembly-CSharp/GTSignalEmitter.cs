using System;
using UnityEngine;

// Token: 0x020008D3 RID: 2259
public class GTSignalEmitter : MonoBehaviour
{
	// Token: 0x06003B0F RID: 15119 RVA: 0x00143ED3 File Offset: 0x001420D3
	public virtual void Emit()
	{
		GTSignal.Emit(this.emitMode, this.signal, Array.Empty<object>());
	}

	// Token: 0x06003B10 RID: 15120 RVA: 0x00143EF0 File Offset: 0x001420F0
	public virtual void Emit(int targetActor)
	{
		GTSignal.Emit(targetActor, this.signal, Array.Empty<object>());
	}

	// Token: 0x06003B11 RID: 15121 RVA: 0x00143F08 File Offset: 0x00142108
	public virtual void Emit(params object[] data)
	{
		GTSignal.Emit(this.emitMode, this.signal, data);
	}

	// Token: 0x04004B72 RID: 19314
	[Space]
	public GTSignalID signal;

	// Token: 0x04004B73 RID: 19315
	public GTSignal.EmitMode emitMode;
}
