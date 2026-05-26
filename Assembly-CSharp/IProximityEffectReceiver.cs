using System;

// Token: 0x020002ED RID: 749
public interface IProximityEffectReceiver
{
	// Token: 0x06001313 RID: 4883
	void OnProximityCalculated(float distance, float alignment, float parallel);
}
