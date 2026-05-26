using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000222 RID: 546
public class BeeSwarmManager : MonoBehaviour
{
	// Token: 0x17000148 RID: 328
	// (get) Token: 0x06000E3F RID: 3647 RVA: 0x0004E886 File Offset: 0x0004CA86
	// (set) Token: 0x06000E40 RID: 3648 RVA: 0x0004E88E File Offset: 0x0004CA8E
	public BeePerchPoint BeeHive { get; private set; }

	// Token: 0x17000149 RID: 329
	// (get) Token: 0x06000E41 RID: 3649 RVA: 0x0004E897 File Offset: 0x0004CA97
	// (set) Token: 0x06000E42 RID: 3650 RVA: 0x0004E89F File Offset: 0x0004CA9F
	public float BeeSpeed { get; private set; }

	// Token: 0x1700014A RID: 330
	// (get) Token: 0x06000E43 RID: 3651 RVA: 0x0004E8A8 File Offset: 0x0004CAA8
	// (set) Token: 0x06000E44 RID: 3652 RVA: 0x0004E8B0 File Offset: 0x0004CAB0
	public float BeeMaxTravelTime { get; private set; }

	// Token: 0x1700014B RID: 331
	// (get) Token: 0x06000E45 RID: 3653 RVA: 0x0004E8B9 File Offset: 0x0004CAB9
	// (set) Token: 0x06000E46 RID: 3654 RVA: 0x0004E8C1 File Offset: 0x0004CAC1
	public float BeeAcceleration { get; private set; }

	// Token: 0x1700014C RID: 332
	// (get) Token: 0x06000E47 RID: 3655 RVA: 0x0004E8CA File Offset: 0x0004CACA
	// (set) Token: 0x06000E48 RID: 3656 RVA: 0x0004E8D2 File Offset: 0x0004CAD2
	public float BeeJitterStrength { get; private set; }

	// Token: 0x1700014D RID: 333
	// (get) Token: 0x06000E49 RID: 3657 RVA: 0x0004E8DB File Offset: 0x0004CADB
	// (set) Token: 0x06000E4A RID: 3658 RVA: 0x0004E8E3 File Offset: 0x0004CAE3
	public float BeeJitterDamping { get; private set; }

	// Token: 0x1700014E RID: 334
	// (get) Token: 0x06000E4B RID: 3659 RVA: 0x0004E8EC File Offset: 0x0004CAEC
	// (set) Token: 0x06000E4C RID: 3660 RVA: 0x0004E8F4 File Offset: 0x0004CAF4
	public float BeeMaxJitterRadius { get; private set; }

	// Token: 0x1700014F RID: 335
	// (get) Token: 0x06000E4D RID: 3661 RVA: 0x0004E8FD File Offset: 0x0004CAFD
	// (set) Token: 0x06000E4E RID: 3662 RVA: 0x0004E905 File Offset: 0x0004CB05
	public float BeeNearDestinationRadius { get; private set; }

	// Token: 0x17000150 RID: 336
	// (get) Token: 0x06000E4F RID: 3663 RVA: 0x0004E90E File Offset: 0x0004CB0E
	// (set) Token: 0x06000E50 RID: 3664 RVA: 0x0004E916 File Offset: 0x0004CB16
	public float AvoidPointRadius { get; private set; }

	// Token: 0x17000151 RID: 337
	// (get) Token: 0x06000E51 RID: 3665 RVA: 0x0004E91F File Offset: 0x0004CB1F
	// (set) Token: 0x06000E52 RID: 3666 RVA: 0x0004E927 File Offset: 0x0004CB27
	public float BeeMinFlowerDuration { get; private set; }

	// Token: 0x17000152 RID: 338
	// (get) Token: 0x06000E53 RID: 3667 RVA: 0x0004E930 File Offset: 0x0004CB30
	// (set) Token: 0x06000E54 RID: 3668 RVA: 0x0004E938 File Offset: 0x0004CB38
	public float BeeMaxFlowerDuration { get; private set; }

	// Token: 0x17000153 RID: 339
	// (get) Token: 0x06000E55 RID: 3669 RVA: 0x0004E941 File Offset: 0x0004CB41
	// (set) Token: 0x06000E56 RID: 3670 RVA: 0x0004E949 File Offset: 0x0004CB49
	public float GeneralBuzzRange { get; private set; }

	// Token: 0x06000E57 RID: 3671 RVA: 0x0004E954 File Offset: 0x0004CB54
	private void Awake()
	{
		this.bees = new List<AnimatedBee>(this.numBees);
		for (int i = 0; i < this.numBees; i++)
		{
			AnimatedBee item = default(AnimatedBee);
			item.InitVisual(this.beePrefab, this);
			this.bees.Add(item);
		}
		this.playerCamera = Camera.main.transform;
	}

	// Token: 0x06000E58 RID: 3672 RVA: 0x0004E9B8 File Offset: 0x0004CBB8
	private void Start()
	{
		foreach (XSceneRef xsceneRef in this.flowerSections)
		{
			GameObject gameObject;
			if (xsceneRef.TryResolve(out gameObject))
			{
				foreach (BeePerchPoint item in gameObject.GetComponentsInChildren<BeePerchPoint>())
				{
					this.allPerchPoints.Add(item);
				}
			}
		}
		this.OnSeedChange();
		RandomTimedSeedManager.instance.AddCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	// Token: 0x06000E59 RID: 3673 RVA: 0x0004EA38 File Offset: 0x0004CC38
	private void OnDestroy()
	{
		RandomTimedSeedManager.instance.RemoveCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	// Token: 0x06000E5A RID: 3674 RVA: 0x0004EA50 File Offset: 0x0004CC50
	private void Update()
	{
		Vector3 position = this.playerCamera.transform.position;
		Vector3 position2 = Vector3.zero;
		Vector3 a = Vector3.zero;
		float num = 1f / (float)this.bees.Count;
		float num2 = float.PositiveInfinity;
		float num3 = this.GeneralBuzzRange * this.GeneralBuzzRange;
		int num4 = 0;
		for (int i = 0; i < this.bees.Count; i++)
		{
			AnimatedBee animatedBee = this.bees[i];
			animatedBee.UpdateVisual(RandomTimedSeedManager.instance.currentSyncTime, this);
			Vector3 position3 = animatedBee.visual.transform.position;
			float sqrMagnitude = (position3 - position).sqrMagnitude;
			if (sqrMagnitude < num2)
			{
				position2 = position3;
				num2 = sqrMagnitude;
			}
			if (sqrMagnitude < num3)
			{
				a += position3;
				num4++;
			}
			this.bees[i] = animatedBee;
		}
		this.nearbyBeeBuzz.transform.position = position2;
		if (num4 > 0)
		{
			this.generalBeeBuzz.transform.position = a / (float)num4;
			this.generalBeeBuzz.enabled = true;
			return;
		}
		this.generalBeeBuzz.enabled = false;
	}

	// Token: 0x06000E5B RID: 3675 RVA: 0x0004EB80 File Offset: 0x0004CD80
	private void OnSeedChange()
	{
		SRand srand = new SRand(RandomTimedSeedManager.instance.seed);
		List<BeePerchPoint> pickBuffer = new List<BeePerchPoint>(this.allPerchPoints.Count);
		List<BeePerchPoint> list = new List<BeePerchPoint>(this.loopSizePerBee);
		List<float> list2 = new List<float>(this.loopSizePerBee);
		for (int i = 0; i < this.bees.Count; i++)
		{
			AnimatedBee value = this.bees[i];
			list = new List<BeePerchPoint>(this.loopSizePerBee);
			list2 = new List<float>(this.loopSizePerBee);
			this.PickPoints(this.loopSizePerBee, pickBuffer, this.allPerchPoints, ref srand, list);
			for (int j = 0; j < list.Count; j++)
			{
				list2.Add(srand.NextFloat(this.BeeMinFlowerDuration, this.BeeMaxFlowerDuration));
			}
			value.InitRoute(list, list2, this);
			value.InitRouteTimestamps();
			this.bees[i] = value;
		}
	}

	// Token: 0x06000E5C RID: 3676 RVA: 0x0004EC74 File Offset: 0x0004CE74
	private void PickPoints(int n, List<BeePerchPoint> pickBuffer, List<BeePerchPoint> allPerchPoints, ref SRand rand, List<BeePerchPoint> resultBuffer)
	{
		resultBuffer.Add(this.BeeHive);
		n--;
		int num = 100;
		while (pickBuffer.Count < n && num-- > 0)
		{
			n -= pickBuffer.Count;
			resultBuffer.AddRange(pickBuffer);
			pickBuffer.Clear();
			pickBuffer.AddRange(allPerchPoints);
			rand.Shuffle<BeePerchPoint>(pickBuffer);
		}
		resultBuffer.AddRange(pickBuffer.GetRange(pickBuffer.Count - n, n));
		pickBuffer.RemoveRange(pickBuffer.Count - n, n);
	}

	// Token: 0x06000E5D RID: 3677 RVA: 0x0004ECF5 File Offset: 0x0004CEF5
	public static void RegisterAvoidPoint(GameObject obj)
	{
		BeeSwarmManager.avoidPoints.Add(obj);
	}

	// Token: 0x06000E5E RID: 3678 RVA: 0x0004ED02 File Offset: 0x0004CF02
	public static void UnregisterAvoidPoint(GameObject obj)
	{
		BeeSwarmManager.avoidPoints.Remove(obj);
	}

	// Token: 0x0400113E RID: 4414
	[SerializeField]
	private XSceneRef[] flowerSections;

	// Token: 0x0400113F RID: 4415
	[SerializeField]
	private int loopSizePerBee;

	// Token: 0x04001140 RID: 4416
	[SerializeField]
	private int numBees;

	// Token: 0x04001141 RID: 4417
	[SerializeField]
	private MeshRenderer beePrefab;

	// Token: 0x04001142 RID: 4418
	[SerializeField]
	private AudioSource nearbyBeeBuzz;

	// Token: 0x04001143 RID: 4419
	[SerializeField]
	private AudioSource generalBeeBuzz;

	// Token: 0x04001144 RID: 4420
	private GameObject[] flowerSectionsResolved;

	// Token: 0x04001151 RID: 4433
	private List<AnimatedBee> bees;

	// Token: 0x04001152 RID: 4434
	private Transform playerCamera;

	// Token: 0x04001153 RID: 4435
	private List<BeePerchPoint> allPerchPoints = new List<BeePerchPoint>();

	// Token: 0x04001154 RID: 4436
	public static readonly List<GameObject> avoidPoints = new List<GameObject>();
}
