using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000B6 RID: 182
public interface IEyeScannable
{
	// Token: 0x17000052 RID: 82
	// (get) Token: 0x06000473 RID: 1139
	int scannableId { get; }

	// Token: 0x17000053 RID: 83
	// (get) Token: 0x06000474 RID: 1140
	Vector3 Position { get; }

	// Token: 0x17000054 RID: 84
	// (get) Token: 0x06000475 RID: 1141
	Bounds Bounds { get; }

	// Token: 0x17000055 RID: 85
	// (get) Token: 0x06000476 RID: 1142
	IList<KeyValueStringPair> Entries { get; }

	// Token: 0x06000477 RID: 1143
	void OnEnable();

	// Token: 0x06000478 RID: 1144
	void OnDisable();

	// Token: 0x1400000E RID: 14
	// (add) Token: 0x06000479 RID: 1145
	// (remove) Token: 0x0600047A RID: 1146
	event Action OnDataChange;
}
