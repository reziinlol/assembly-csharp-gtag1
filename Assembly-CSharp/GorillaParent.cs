using System;
using UnityEngine;

// Token: 0x02000887 RID: 2183
public class GorillaParent : MonoBehaviour
{
	// Token: 0x060038F8 RID: 14584 RVA: 0x00137518 File Offset: 0x00135718
	public void Awake()
	{
		if (GorillaParent.instance == null)
		{
			GorillaParent.instance = this;
			GorillaParent.hasInstance = true;
			return;
		}
		if (GorillaParent.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
	}

	// Token: 0x060038F9 RID: 14585 RVA: 0x00137553 File Offset: 0x00135753
	protected void OnDestroy()
	{
		if (GorillaParent.instance == this)
		{
			GorillaParent.hasInstance = false;
			GorillaParent.instance = null;
		}
	}

	// Token: 0x060038FA RID: 14586 RVA: 0x00137572 File Offset: 0x00135772
	public static void ReplicatedClientReady()
	{
		GorillaParent.replicatedClientReady = true;
		Action action = GorillaParent.onReplicatedClientReady;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x060038FB RID: 14587 RVA: 0x00137589 File Offset: 0x00135789
	public static void OnReplicatedClientReady(Action action)
	{
		if (GorillaParent.replicatedClientReady)
		{
			action();
			return;
		}
		GorillaParent.onReplicatedClientReady = (Action)Delegate.Combine(GorillaParent.onReplicatedClientReady, action);
	}

	// Token: 0x040048FC RID: 18684
	[OnEnterPlay_SetNull]
	public static volatile GorillaParent instance;

	// Token: 0x040048FD RID: 18685
	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	// Token: 0x040048FE RID: 18686
	private int i;

	// Token: 0x040048FF RID: 18687
	private static bool replicatedClientReady;

	// Token: 0x04004900 RID: 18688
	private static Action onReplicatedClientReady;
}
