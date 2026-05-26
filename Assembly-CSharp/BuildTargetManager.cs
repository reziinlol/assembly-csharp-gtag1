using System;
using UnityEngine;

// Token: 0x02000D0B RID: 3339
public class BuildTargetManager : MonoBehaviour
{
	// Token: 0x060052B3 RID: 21171 RVA: 0x001B233F File Offset: 0x001B053F
	public string GetPath()
	{
		return this.path;
	}

	// Token: 0x040063C8 RID: 25544
	public BuildTargetManager.BuildTowards newBuildTarget;

	// Token: 0x040063C9 RID: 25545
	public bool isBeta;

	// Token: 0x040063CA RID: 25546
	public bool isQA;

	// Token: 0x040063CB RID: 25547
	public bool spoofIDs;

	// Token: 0x040063CC RID: 25548
	public bool spoofChild;

	// Token: 0x040063CD RID: 25549
	public bool enableAllCosmetics;

	// Token: 0x040063CE RID: 25550
	public OVRManager ovrManager;

	// Token: 0x040063CF RID: 25551
	private string path = "Assets/csc.rsp";

	// Token: 0x040063D0 RID: 25552
	public BuildTargetManager.BuildTowards currentBuildTargetDONOTCHANGE;

	// Token: 0x040063D1 RID: 25553
	public GorillaTagger gorillaTagger;

	// Token: 0x040063D2 RID: 25554
	public GameObject[] betaDisableObjects;

	// Token: 0x040063D3 RID: 25555
	public GameObject[] betaEnableObjects;

	// Token: 0x040063D4 RID: 25556
	public BuildTargetManager.NetworkBackend networkBackend;

	// Token: 0x02000D0C RID: 3340
	public enum BuildTowards
	{
		// Token: 0x040063D6 RID: 25558
		Steam,
		// Token: 0x040063D7 RID: 25559
		OculusPC,
		// Token: 0x040063D8 RID: 25560
		Quest,
		// Token: 0x040063D9 RID: 25561
		Viveport
	}

	// Token: 0x02000D0D RID: 3341
	public enum NetworkBackend
	{
		// Token: 0x040063DB RID: 25563
		Pun,
		// Token: 0x040063DC RID: 25564
		Fusion
	}
}
