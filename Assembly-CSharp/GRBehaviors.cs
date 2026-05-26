using System;
using System.Collections.Generic;

// Token: 0x02000770 RID: 1904
public class GRBehaviors<T> : GRBehaviorsBase where T : Enum
{
	// Token: 0x0600303C RID: 12348 RVA: 0x00106474 File Offset: 0x00104674
	public void AddBehavior(T behavior, GRAbilityBase ability)
	{
		GRBehaviors<T>.BehaviorData item = new GRBehaviors<T>.BehaviorData
		{
			behavior = behavior,
			ability = ability
		};
		this.behaviorData.Add(item);
	}

	// Token: 0x04003DCC RID: 15820
	public List<GRBehaviors<T>.BehaviorData> behaviorData;

	// Token: 0x02000771 RID: 1905
	public class BehaviorData
	{
		// Token: 0x04003DCD RID: 15821
		public T behavior;

		// Token: 0x04003DCE RID: 15822
		public GRAbilityBase ability;
	}
}
