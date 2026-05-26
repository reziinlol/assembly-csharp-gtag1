using System;

// Token: 0x02000CD0 RID: 3280
public interface IFXContextParems<T> where T : FXSArgs
{
	// Token: 0x17000795 RID: 1941
	// (get) Token: 0x06005172 RID: 20850
	FXSystemSettings settings { get; }

	// Token: 0x06005173 RID: 20851
	void OnPlayFX(T parems);
}
