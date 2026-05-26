using System;
using UnityEngine;

// Token: 0x020007CE RID: 1998
public class GRScannable : MonoBehaviour
{
	// Token: 0x060032F6 RID: 13046 RVA: 0x0011739F File Offset: 0x0011559F
	public virtual void Start()
	{
		if (this.gameEntity == null)
		{
			this.gameEntity = base.GetComponent<GameEntity>();
		}
	}

	// Token: 0x060032F7 RID: 13047 RVA: 0x001173BB File Offset: 0x001155BB
	public virtual string GetTitleText(GhostReactor reactor)
	{
		return this.titleText;
	}

	// Token: 0x060032F8 RID: 13048 RVA: 0x001173C3 File Offset: 0x001155C3
	public virtual string GetBodyText(GhostReactor reactor)
	{
		return this.bodyText;
	}

	// Token: 0x060032F9 RID: 13049 RVA: 0x001173CB File Offset: 0x001155CB
	public virtual string GetAnnotationText(GhostReactor reactor)
	{
		return this.annotationText;
	}

	// Token: 0x04004235 RID: 16949
	public GameEntity gameEntity;

	// Token: 0x04004236 RID: 16950
	[SerializeField]
	protected string titleText;

	// Token: 0x04004237 RID: 16951
	[SerializeField]
	protected string bodyText;

	// Token: 0x04004238 RID: 16952
	[SerializeField]
	protected string annotationText;
}
