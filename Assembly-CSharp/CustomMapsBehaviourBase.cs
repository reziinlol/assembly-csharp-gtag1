using System;
using UnityEngine;

// Token: 0x02000A52 RID: 2642
public abstract class CustomMapsBehaviourBase
{
	// Token: 0x060043B3 RID: 17331
	public abstract bool CanExecute();

	// Token: 0x060043B4 RID: 17332
	public abstract void Execute();

	// Token: 0x060043B5 RID: 17333
	public abstract void NetExecute();

	// Token: 0x060043B6 RID: 17334
	public abstract void ResetBehavior();

	// Token: 0x060043B7 RID: 17335
	public abstract bool CanContinueExecuting();

	// Token: 0x060043B8 RID: 17336
	public abstract void OnTriggerEnter(Collider otherCollider);
}
