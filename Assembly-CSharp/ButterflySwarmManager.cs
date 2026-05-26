using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000223 RID: 547
public class ButterflySwarmManager : MonoBehaviour
{
	// Token: 0x17000154 RID: 340
	// (get) Token: 0x06000E61 RID: 3681 RVA: 0x0004ED2F File Offset: 0x0004CF2F
	// (set) Token: 0x06000E62 RID: 3682 RVA: 0x0004ED37 File Offset: 0x0004CF37
	public float PerchedFlapSpeed { get; private set; }

	// Token: 0x17000155 RID: 341
	// (get) Token: 0x06000E63 RID: 3683 RVA: 0x0004ED40 File Offset: 0x0004CF40
	// (set) Token: 0x06000E64 RID: 3684 RVA: 0x0004ED48 File Offset: 0x0004CF48
	public float PerchedFlapPhase { get; private set; }

	// Token: 0x17000156 RID: 342
	// (get) Token: 0x06000E65 RID: 3685 RVA: 0x0004ED51 File Offset: 0x0004CF51
	// (set) Token: 0x06000E66 RID: 3686 RVA: 0x0004ED59 File Offset: 0x0004CF59
	public float BeeSpeed { get; private set; }

	// Token: 0x17000157 RID: 343
	// (get) Token: 0x06000E67 RID: 3687 RVA: 0x0004ED62 File Offset: 0x0004CF62
	// (set) Token: 0x06000E68 RID: 3688 RVA: 0x0004ED6A File Offset: 0x0004CF6A
	public float BeeMaxTravelTime { get; private set; }

	// Token: 0x17000158 RID: 344
	// (get) Token: 0x06000E69 RID: 3689 RVA: 0x0004ED73 File Offset: 0x0004CF73
	// (set) Token: 0x06000E6A RID: 3690 RVA: 0x0004ED7B File Offset: 0x0004CF7B
	public float BeeAcceleration { get; private set; }

	// Token: 0x17000159 RID: 345
	// (get) Token: 0x06000E6B RID: 3691 RVA: 0x0004ED84 File Offset: 0x0004CF84
	// (set) Token: 0x06000E6C RID: 3692 RVA: 0x0004ED8C File Offset: 0x0004CF8C
	public float BeeJitterStrength { get; private set; }

	// Token: 0x1700015A RID: 346
	// (get) Token: 0x06000E6D RID: 3693 RVA: 0x0004ED95 File Offset: 0x0004CF95
	// (set) Token: 0x06000E6E RID: 3694 RVA: 0x0004ED9D File Offset: 0x0004CF9D
	public float BeeJitterDamping { get; private set; }

	// Token: 0x1700015B RID: 347
	// (get) Token: 0x06000E6F RID: 3695 RVA: 0x0004EDA6 File Offset: 0x0004CFA6
	// (set) Token: 0x06000E70 RID: 3696 RVA: 0x0004EDAE File Offset: 0x0004CFAE
	public float BeeMaxJitterRadius { get; private set; }

	// Token: 0x1700015C RID: 348
	// (get) Token: 0x06000E71 RID: 3697 RVA: 0x0004EDB7 File Offset: 0x0004CFB7
	// (set) Token: 0x06000E72 RID: 3698 RVA: 0x0004EDBF File Offset: 0x0004CFBF
	public float BeeNearDestinationRadius { get; private set; }

	// Token: 0x1700015D RID: 349
	// (get) Token: 0x06000E73 RID: 3699 RVA: 0x0004EDC8 File Offset: 0x0004CFC8
	// (set) Token: 0x06000E74 RID: 3700 RVA: 0x0004EDD0 File Offset: 0x0004CFD0
	public float DestRotationAlignmentSpeed { get; private set; }

	// Token: 0x1700015E RID: 350
	// (get) Token: 0x06000E75 RID: 3701 RVA: 0x0004EDD9 File Offset: 0x0004CFD9
	// (set) Token: 0x06000E76 RID: 3702 RVA: 0x0004EDE1 File Offset: 0x0004CFE1
	public Vector3 TravellingLocalRotationEuler { get; private set; }

	// Token: 0x1700015F RID: 351
	// (get) Token: 0x06000E77 RID: 3703 RVA: 0x0004EDEA File Offset: 0x0004CFEA
	// (set) Token: 0x06000E78 RID: 3704 RVA: 0x0004EDF2 File Offset: 0x0004CFF2
	public Quaternion TravellingLocalRotation { get; private set; }

	// Token: 0x17000160 RID: 352
	// (get) Token: 0x06000E79 RID: 3705 RVA: 0x0004EDFB File Offset: 0x0004CFFB
	// (set) Token: 0x06000E7A RID: 3706 RVA: 0x0004EE03 File Offset: 0x0004D003
	public float AvoidPointRadius { get; private set; }

	// Token: 0x17000161 RID: 353
	// (get) Token: 0x06000E7B RID: 3707 RVA: 0x0004EE0C File Offset: 0x0004D00C
	// (set) Token: 0x06000E7C RID: 3708 RVA: 0x0004EE14 File Offset: 0x0004D014
	public float BeeMinFlowerDuration { get; private set; }

	// Token: 0x17000162 RID: 354
	// (get) Token: 0x06000E7D RID: 3709 RVA: 0x0004EE1D File Offset: 0x0004D01D
	// (set) Token: 0x06000E7E RID: 3710 RVA: 0x0004EE25 File Offset: 0x0004D025
	public float BeeMaxFlowerDuration { get; private set; }

	// Token: 0x17000163 RID: 355
	// (get) Token: 0x06000E7F RID: 3711 RVA: 0x0004EE2E File Offset: 0x0004D02E
	// (set) Token: 0x06000E80 RID: 3712 RVA: 0x0004EE36 File Offset: 0x0004D036
	public Color[] BeeColors { get; private set; }

	// Token: 0x06000E81 RID: 3713 RVA: 0x0004EE40 File Offset: 0x0004D040
	private void Awake()
	{
		this.TravellingLocalRotation = Quaternion.Euler(this.TravellingLocalRotationEuler);
		this.butterflies = new List<AnimatedButterfly>(this.numBees);
		for (int i = 0; i < this.numBees; i++)
		{
			AnimatedButterfly item = default(AnimatedButterfly);
			item.InitVisual(this.beePrefab, this);
			if (this.BeeColors.Length != 0)
			{
				item.SetColor(this.BeeColors[i % this.BeeColors.Length]);
			}
			this.butterflies.Add(item);
		}
	}

	// Token: 0x06000E82 RID: 3714 RVA: 0x0004EEC8 File Offset: 0x0004D0C8
	private void Start()
	{
		foreach (XSceneRef xsceneRef in this.perchSections)
		{
			GameObject gameObject;
			if (xsceneRef.TryResolve(out gameObject))
			{
				List<GameObject> list = new List<GameObject>();
				this.allPerchZones.Add(list);
				foreach (object obj in gameObject.transform)
				{
					Transform transform = (Transform)obj;
					list.Add(transform.gameObject);
				}
			}
		}
		this.OnSeedChange();
		RandomTimedSeedManager.instance.AddCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	// Token: 0x06000E83 RID: 3715 RVA: 0x0004EF88 File Offset: 0x0004D188
	private void OnDestroy()
	{
		RandomTimedSeedManager.instance.RemoveCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	// Token: 0x06000E84 RID: 3716 RVA: 0x0004EFA0 File Offset: 0x0004D1A0
	private void Update()
	{
		for (int i = 0; i < this.butterflies.Count; i++)
		{
			AnimatedButterfly value = this.butterflies[i];
			value.UpdateVisual(RandomTimedSeedManager.instance.currentSyncTime, this);
			this.butterflies[i] = value;
		}
	}

	// Token: 0x06000E85 RID: 3717 RVA: 0x0004EFF0 File Offset: 0x0004D1F0
	private void OnSeedChange()
	{
		SRand srand = new SRand(RandomTimedSeedManager.instance.seed);
		List<List<GameObject>> list = new List<List<GameObject>>(this.allPerchZones.Count);
		for (int i = 0; i < this.allPerchZones.Count; i++)
		{
			List<GameObject> list2 = new List<GameObject>();
			list2.AddRange(this.allPerchZones[i]);
			list.Add(list2);
		}
		List<GameObject> list3 = new List<GameObject>(this.loopSizePerBee);
		List<float> list4 = new List<float>(this.loopSizePerBee);
		for (int j = 0; j < this.butterflies.Count; j++)
		{
			AnimatedButterfly value = this.butterflies[j];
			value.SetFlapSpeed(srand.NextFloat(this.minFlapSpeed, this.maxFlapSpeed));
			list3.Clear();
			list4.Clear();
			this.PickPoints(this.loopSizePerBee, list, ref srand, list3);
			for (int k = 0; k < list3.Count; k++)
			{
				list4.Add(srand.NextFloat(this.BeeMinFlowerDuration, this.BeeMaxFlowerDuration));
			}
			if (list3.Count == 0)
			{
				this.butterflies.Clear();
				return;
			}
			value.InitRoute(list3, list4, this);
			this.butterflies[j] = value;
		}
	}

	// Token: 0x06000E86 RID: 3718 RVA: 0x0004F134 File Offset: 0x0004D334
	private void PickPoints(int n, List<List<GameObject>> pickBuffer, ref SRand rand, List<GameObject> resultBuffer)
	{
		int exclude = rand.NextInt(0, pickBuffer.Count);
		int num = -1;
		int num2 = n - 2;
		while (resultBuffer.Count < n)
		{
			int num3;
			if (resultBuffer.Count < num2)
			{
				num3 = rand.NextIntWithExclusion(0, pickBuffer.Count, num);
			}
			else
			{
				num3 = rand.NextIntWithExclusion2(0, pickBuffer.Count, num, exclude);
			}
			int num4 = 10;
			while (num3 == num || pickBuffer[num3].Count == 0)
			{
				num3 = (num3 + 1) % pickBuffer.Count;
				num4--;
				if (num4 <= 0)
				{
					return;
				}
			}
			num = num3;
			List<GameObject> list = pickBuffer[num];
			while (list.Count == 0)
			{
				num = (num + 1) % pickBuffer.Count;
				list = pickBuffer[num];
			}
			resultBuffer.Add(list[list.Count - 1]);
			list.RemoveAt(list.Count - 1);
		}
	}

	// Token: 0x04001155 RID: 4437
	[SerializeField]
	private XSceneRef[] perchSections;

	// Token: 0x04001156 RID: 4438
	[SerializeField]
	private int loopSizePerBee;

	// Token: 0x04001157 RID: 4439
	[SerializeField]
	private int numBees;

	// Token: 0x04001158 RID: 4440
	[SerializeField]
	private MeshRenderer beePrefab;

	// Token: 0x04001159 RID: 4441
	[SerializeField]
	private float maxFlapSpeed;

	// Token: 0x0400115A RID: 4442
	[SerializeField]
	private float minFlapSpeed;

	// Token: 0x0400116B RID: 4459
	private List<AnimatedButterfly> butterflies;

	// Token: 0x0400116C RID: 4460
	private List<List<GameObject>> allPerchZones = new List<List<GameObject>>();
}
