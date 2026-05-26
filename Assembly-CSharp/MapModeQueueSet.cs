using System;
using UnityEngine;

// Token: 0x02000901 RID: 2305
[CreateAssetMenu(fileName = "MapModeQueueSet", menuName = "Game Settings/Map Mode Queue Set")]
public class MapModeQueueSet : ScriptableObject
{
	// Token: 0x04004CCB RID: 19659
	public string[] maps;

	// Token: 0x04004CCC RID: 19660
	public string[] modes;

	// Token: 0x04004CCD RID: 19661
	public string[] queues;
}
