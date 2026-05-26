using System;
using UnityEngine;

// Token: 0x02000DC8 RID: 3528
[CreateAssetMenu(fileName = "New TeleportNode Definition", menuName = "Teleportation/TeleportNode Definition", order = 1)]
public class TeleportNodeDefinition : ScriptableObject
{
	// Token: 0x17000825 RID: 2085
	// (get) Token: 0x06005674 RID: 22132 RVA: 0x001C0B19 File Offset: 0x001BED19
	public TeleportNode Forward
	{
		get
		{
			return this.forward;
		}
	}

	// Token: 0x17000826 RID: 2086
	// (get) Token: 0x06005675 RID: 22133 RVA: 0x001C0B21 File Offset: 0x001BED21
	public TeleportNode Backward
	{
		get
		{
			return this.backward;
		}
	}

	// Token: 0x06005676 RID: 22134 RVA: 0x001C0B29 File Offset: 0x001BED29
	public void SetForward(TeleportNode node)
	{
		Debug.Log("registered fwd node " + node.name);
		this.forward = node;
	}

	// Token: 0x06005677 RID: 22135 RVA: 0x001C0B47 File Offset: 0x001BED47
	public void SetBackward(TeleportNode node)
	{
		Debug.Log("registered bkwd node " + node.name);
		this.backward = node;
	}

	// Token: 0x0400665A RID: 26202
	[SerializeField]
	private TeleportNode forward;

	// Token: 0x0400665B RID: 26203
	[SerializeField]
	private TeleportNode backward;
}
