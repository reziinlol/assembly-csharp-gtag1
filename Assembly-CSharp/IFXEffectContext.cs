using System;

// Token: 0x02000CD2 RID: 3282
public interface IFXEffectContext<T> where T : IFXEffectContextObject
{
	// Token: 0x1700079D RID: 1949
	// (get) Token: 0x0600517E RID: 20862
	T effectContext { get; }

	// Token: 0x1700079E RID: 1950
	// (get) Token: 0x0600517F RID: 20863
	FXSystemSettings settings { get; }
}
