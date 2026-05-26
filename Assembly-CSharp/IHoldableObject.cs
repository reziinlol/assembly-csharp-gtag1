using System;
using UnityEngine;

// Token: 0x02000526 RID: 1318
public interface IHoldableObject
{
	// Token: 0x1700038F RID: 911
	// (get) Token: 0x06002110 RID: 8464
	GameObject gameObject { get; }

	// Token: 0x17000390 RID: 912
	// (get) Token: 0x06002111 RID: 8465
	// (set) Token: 0x06002112 RID: 8466
	string name { get; set; }

	// Token: 0x17000391 RID: 913
	// (get) Token: 0x06002113 RID: 8467
	bool TwoHanded { get; }

	// Token: 0x06002114 RID: 8468
	void OnHover(InteractionPoint pointHovered, GameObject hoveringHand);

	// Token: 0x06002115 RID: 8469
	void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand);

	// Token: 0x06002116 RID: 8470
	bool OnRelease(DropZone zoneReleased, GameObject releasingHand);

	// Token: 0x06002117 RID: 8471
	void DropItemCleanup();
}
