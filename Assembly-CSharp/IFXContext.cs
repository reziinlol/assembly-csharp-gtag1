using System;

// Token: 0x02000CCE RID: 3278
public interface IFXContext
{
	// Token: 0x17000794 RID: 1940
	// (get) Token: 0x0600516F RID: 20847
	FXSystemSettings settings { get; }

	// Token: 0x06005170 RID: 20848
	void OnPlayFX();
}
