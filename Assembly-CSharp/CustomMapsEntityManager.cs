using System;
using Fusion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000A53 RID: 2643
[NetworkBehaviourWeaved(0)]
public class CustomMapsEntityManager : GameEntityManager
{
	// Token: 0x060043BA RID: 17338 RVA: 0x0016B5C8 File Offset: 0x001697C8
	private static bool IsOverrideEnabled()
	{
		GorillaServer instance = GorillaServer.Instance;
		return instance != null && instance.CheckIsVStumpGrabbablesFixEnabled();
	}

	// Token: 0x060043BB RID: 17339 RVA: 0x0016B5EE File Offset: 0x001697EE
	public override bool IsPositionInManagerBounds(Vector3 pos)
	{
		return (CustomMapLoader.CanLoadEntities && CustomMapsEntityManager.IsOverrideEnabled()) || base.IsPositionInManagerBounds(pos);
	}

	// Token: 0x060043BC RID: 17340 RVA: 0x0016B608 File Offset: 0x00169808
	protected override bool IsInZone()
	{
		if (!CustomMapLoader.CanLoadEntities || !CustomMapsEntityManager.IsOverrideEnabled())
		{
			return base.IsInZone();
		}
		bool flag = true;
		for (int i = 0; i < this.zoneComponents.Count; i++)
		{
			flag &= this.zoneComponents[i].IsZoneReady();
		}
		return flag;
	}

	// Token: 0x060043BE RID: 17342 RVA: 0x0016B65F File Offset: 0x0016985F
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x060043BF RID: 17343 RVA: 0x0016B66B File Offset: 0x0016986B
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
