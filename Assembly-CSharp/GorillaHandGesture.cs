using System;
using UnityEngine;

// Token: 0x020002CE RID: 718
[CreateAssetMenu(fileName = "New Hand Gesture", menuName = "Gorilla/Hand Gesture")]
public class GorillaHandGesture : ScriptableObject
{
	// Token: 0x170001C4 RID: 452
	// (get) Token: 0x0600125E RID: 4702 RVA: 0x00062590 File Offset: 0x00060790
	// (set) Token: 0x0600125F RID: 4703 RVA: 0x0006259F File Offset: 0x0006079F
	public GestureHandNode hand
	{
		get
		{
			return (GestureHandNode)this.nodes[0];
		}
		set
		{
			this.nodes[0] = value;
		}
	}

	// Token: 0x170001C5 RID: 453
	// (get) Token: 0x06001260 RID: 4704 RVA: 0x000625AA File Offset: 0x000607AA
	// (set) Token: 0x06001261 RID: 4705 RVA: 0x000625B4 File Offset: 0x000607B4
	public GestureNode palm
	{
		get
		{
			return this.nodes[1];
		}
		set
		{
			this.nodes[1] = value;
		}
	}

	// Token: 0x170001C6 RID: 454
	// (get) Token: 0x06001262 RID: 4706 RVA: 0x000625BF File Offset: 0x000607BF
	// (set) Token: 0x06001263 RID: 4707 RVA: 0x000625C9 File Offset: 0x000607C9
	public GestureNode wrist
	{
		get
		{
			return this.nodes[2];
		}
		set
		{
			this.nodes[2] = value;
		}
	}

	// Token: 0x170001C7 RID: 455
	// (get) Token: 0x06001264 RID: 4708 RVA: 0x000625D4 File Offset: 0x000607D4
	// (set) Token: 0x06001265 RID: 4709 RVA: 0x000625DE File Offset: 0x000607DE
	public GestureNode digits
	{
		get
		{
			return this.nodes[3];
		}
		set
		{
			this.nodes[3] = value;
		}
	}

	// Token: 0x170001C8 RID: 456
	// (get) Token: 0x06001266 RID: 4710 RVA: 0x000625E9 File Offset: 0x000607E9
	// (set) Token: 0x06001267 RID: 4711 RVA: 0x000625F8 File Offset: 0x000607F8
	public GestureDigitNode thumb
	{
		get
		{
			return (GestureDigitNode)this.nodes[4];
		}
		set
		{
			this.nodes[4] = value;
		}
	}

	// Token: 0x170001C9 RID: 457
	// (get) Token: 0x06001268 RID: 4712 RVA: 0x00062603 File Offset: 0x00060803
	// (set) Token: 0x06001269 RID: 4713 RVA: 0x00062612 File Offset: 0x00060812
	public GestureDigitNode index
	{
		get
		{
			return (GestureDigitNode)this.nodes[5];
		}
		set
		{
			this.nodes[5] = value;
		}
	}

	// Token: 0x170001CA RID: 458
	// (get) Token: 0x0600126A RID: 4714 RVA: 0x0006261D File Offset: 0x0006081D
	// (set) Token: 0x0600126B RID: 4715 RVA: 0x0006262C File Offset: 0x0006082C
	public GestureDigitNode middle
	{
		get
		{
			return (GestureDigitNode)this.nodes[6];
		}
		set
		{
			this.nodes[6] = value;
		}
	}

	// Token: 0x0600126C RID: 4716 RVA: 0x00062637 File Offset: 0x00060837
	private static GestureNode[] InitNodes()
	{
		return new GestureNode[]
		{
			new GestureHandNode(),
			new GestureNode(),
			new GestureNode(),
			new GestureNode(),
			new GestureDigitNode(),
			new GestureDigitNode(),
			new GestureDigitNode()
		};
	}

	// Token: 0x04001677 RID: 5751
	public bool track = true;

	// Token: 0x04001678 RID: 5752
	public GestureNode[] nodes = GorillaHandGesture.InitNodes();
}
