using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000150 RID: 336
public class SIResource : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060008DB RID: 2267 RVA: 0x00030214 File Offset: 0x0002E414
	private void Awake()
	{
		if (this.myGameEntity == null)
		{
			this.myGameEntity = base.GetComponent<GameEntity>();
		}
		if (this.myGameEntity == null)
		{
			Debug.LogError("missing gameentity reference! bad!", base.gameObject);
			return;
		}
		GameEntity gameEntity = this.myGameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.SetLastGrabbed));
		this._rb = base.GetComponent<Rigidbody>();
		this.myGameEntity.onEntityDestroyed += this.HandleOnDestroyed;
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x000302A4 File Offset: 0x0002E4A4
	public void SliceUpdate()
	{
		if (this.isSleeping || !this.shouldSleep)
		{
			return;
		}
		if (Time.time < this.timeReleased + this.sleepTime)
		{
			return;
		}
		this._rb.isKinematic = true;
		this.isSleeping = true;
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x000302DF File Offset: 0x0002E4DF
	public void SetLastGrabbed()
	{
		this.lastPlayerHeld = SIPlayer.Get(this.myGameEntity.lastHeldByActorNumber);
		if (this.lastPlayerHeld == SIPlayer.LocalPlayer)
		{
			this.localEverGrabbed = true;
		}
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x00030310 File Offset: 0x0002E510
	protected virtual void OnEnable()
	{
		GameEntity gameEntity = this.myGameEntity;
		gameEntity.OnSnapped = (Action)Delegate.Combine(gameEntity.OnSnapped, new Action(this.GrabInitialization));
		GameEntity gameEntity2 = this.myGameEntity;
		gameEntity2.OnGrabbed = (Action)Delegate.Combine(gameEntity2.OnGrabbed, new Action(this.GrabInitialization));
		GameEntity gameEntity3 = this.myGameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.ReleaseInitialization));
		GameEntity gameEntity4 = this.myGameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.ReleaseInitialization));
		this.timeReleased = Time.time;
		this._rb.isKinematic = true;
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x000303D8 File Offset: 0x0002E5D8
	private void OnDisable()
	{
		GameEntity gameEntity = this.myGameEntity;
		gameEntity.OnSnapped = (Action)Delegate.Remove(gameEntity.OnSnapped, new Action(this.GrabInitialization));
		GameEntity gameEntity2 = this.myGameEntity;
		gameEntity2.OnGrabbed = (Action)Delegate.Remove(gameEntity2.OnGrabbed, new Action(this.GrabInitialization));
		GameEntity gameEntity3 = this.myGameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Remove(gameEntity3.OnReleased, new Action(this.ReleaseInitialization));
		GameEntity gameEntity4 = this.myGameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Remove(gameEntity4.OnUnsnapped, new Action(this.ReleaseInitialization));
		SpawnRegion<GameEntity, SIResourceRegion>.RemoveItemFromRegion(this.myGameEntity);
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x00030494 File Offset: 0x0002E694
	public void GrabInitialization()
	{
		this.isSleeping = false;
		this.shouldSleep = false;
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x000304A4 File Offset: 0x0002E6A4
	public void ReleaseInitialization()
	{
		this.shouldSleep = true;
		this.isSleeping = false;
		this.timeReleased = Time.time;
	}

	// Token: 0x060008E2 RID: 2274 RVA: 0x000304BF File Offset: 0x0002E6BF
	public virtual bool CanDeposit()
	{
		return this.lastPlayerHeld != null && this.lastPlayerHeld.gamePlayer.IsLocal() && !this.localDeposited && SIPlayer.LocalPlayer.CanLimitedResourceBeDeposited(this.limitedDepositType);
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x000304FB File Offset: 0x0002E6FB
	public virtual void HandleDepositLocal(SIPlayer depositingPlayer)
	{
		this.localDeposited = true;
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void HandleDepositAuth(SIPlayer depositingPlayer)
	{
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x00030504 File Offset: 0x0002E704
	private void HandleOnDestroyed(GameEntity entity)
	{
		if (!this.localEverGrabbed || this.localDeposited || !entity.manager.IsZoneActive() || !PhotonNetwork.InRoom)
		{
			return;
		}
		if (this.type == SIResource.ResourceType.StrangeWood)
		{
			PlayerGameEvents.MiscEvent("SIHelpOtherCollectStrangeWood", 1);
			return;
		}
		if (this.type == SIResource.ResourceType.WeirdGear)
		{
			PlayerGameEvents.MiscEvent("SIHelpOtherCollectWeirdGears", 1);
			return;
		}
		if (this.type == SIResource.ResourceType.FloppyMetal)
		{
			PlayerGameEvents.MiscEvent("SIHelpOtherCollectFloppyMetal", 1);
			return;
		}
		if (this.type == SIResource.ResourceType.BouncySand)
		{
			PlayerGameEvents.MiscEvent("SIHelpOtherCollectBouncySand", 1);
			return;
		}
		if (this.type == SIResource.ResourceType.VibratingSpring)
		{
			PlayerGameEvents.MiscEvent("SIHelpOtherCollectVibratingSpring", 1);
		}
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x000305A0 File Offset: 0x0002E7A0
	public static List<SIResource.ResourceCost> GetSum(params IList<SIResource.ResourceCost>[] costs)
	{
		List<SIResource.ResourceCost> list = new List<SIResource.ResourceCost>();
		if (costs == null)
		{
			return list;
		}
		foreach (IList<SIResource.ResourceCost> list2 in costs)
		{
			if (list2 != null)
			{
				foreach (SIResource.ResourceCost additiveCost in list2)
				{
					list.AddResourceCost(additiveCost);
				}
			}
		}
		return list;
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x00030614 File Offset: 0x0002E814
	public static List<SIResource.ResourceCost> GetMax(params IList<SIResource.ResourceCost>[] costs)
	{
		List<SIResource.ResourceCost> list = new List<SIResource.ResourceCost>();
		if (costs == null)
		{
			return list;
		}
		for (int i = 0; i < costs.Length; i++)
		{
			foreach (SIResource.ResourceCost resourceCost in costs[i])
			{
				int amount = Mathf.Max(list.GetAmount(resourceCost.type), resourceCost.amount);
				list.SetAmount(resourceCost.type, amount);
			}
		}
		return list;
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x000306A0 File Offset: 0x0002E8A0
	public static bool CategoryCostsMatch(IList<SIResource.ResourceCost> cost1, IList<SIResource.ResourceCost> cost2)
	{
		return cost1.GetCategoryCosts() == cost2.GetCategoryCosts();
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x000306B4 File Offset: 0x0002E8B4
	public static bool CostsAreEqual(IList<SIResource.ResourceCost> cost1, IList<SIResource.ResourceCost> cost2, bool matchOrder = true)
	{
		if (cost1.Count != cost2.Count)
		{
			return false;
		}
		if (!matchOrder)
		{
			foreach (SIResource.ResourceCost resourceCost in cost1)
			{
				if (cost2.GetAmount(resourceCost.type) != resourceCost.amount)
				{
					return false;
				}
			}
			return true;
		}
		for (int i = 0; i < cost1.Count; i++)
		{
			if (!cost1[i].Equals(cost2[i]))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x00030754 File Offset: 0x0002E954
	public static SIResource.ResourceCost[] GenerateCostsFrom(Dictionary<SIResource.ResourceType, int> costDictionary)
	{
		List<SIResource.ResourceCost> list = new List<SIResource.ResourceCost>();
		foreach (KeyValuePair<SIResource.ResourceType, int> keyValuePair in costDictionary)
		{
			list.Add(new SIResource.ResourceCost(keyValuePair.Key, keyValuePair.Value));
		}
		list.Sort();
		return list.ToArray();
	}

	// Token: 0x060008EB RID: 2283 RVA: 0x000307C8 File Offset: 0x0002E9C8
	public static string PrintCost(IEnumerable<SIResource.ResourceCost> costs)
	{
		return "[" + string.Join<SIResource.ResourceCost>(", ", costs) + "]";
	}

	// Token: 0x04000AF4 RID: 2804
	public SIPlayer lastPlayerHeld;

	// Token: 0x04000AF5 RID: 2805
	public GameEntity myGameEntity;

	// Token: 0x04000AF6 RID: 2806
	public SIResource.ResourceType type;

	// Token: 0x04000AF7 RID: 2807
	public SIResource.LimitedDepositType limitedDepositType;

	// Token: 0x04000AF8 RID: 2808
	public bool localDeposited;

	// Token: 0x04000AF9 RID: 2809
	public bool localEverGrabbed;

	// Token: 0x04000AFA RID: 2810
	[Tooltip("The amount of pitch offset allowed during spawn, in degrees.  With this set to 0, item will always spawn aligned with surface.")]
	public float spawnPitchVariance;

	// Token: 0x04000AFB RID: 2811
	public float sleepTime = 10f;

	// Token: 0x04000AFC RID: 2812
	private bool shouldSleep = true;

	// Token: 0x04000AFD RID: 2813
	private bool isSleeping;

	// Token: 0x04000AFE RID: 2814
	private float timeReleased;

	// Token: 0x04000AFF RID: 2815
	private Rigidbody _rb;

	// Token: 0x02000151 RID: 337
	[Serializable]
	public struct ResourceCost : IComparable<SIResource.ResourceCost>, IEquatable<SIResource.ResourceCost>
	{
		// Token: 0x060008ED RID: 2285 RVA: 0x000307FE File Offset: 0x0002E9FE
		public ResourceCost(SIResource.ResourceType type, int amount)
		{
			this.type = type;
			this.amount = amount;
		}

		// Token: 0x060008EE RID: 2286 RVA: 0x00030810 File Offset: 0x0002EA10
		public int CompareTo(SIResource.ResourceCost other)
		{
			int num = this.type.CompareTo(other.type);
			if (num != 0)
			{
				return num;
			}
			return this.amount.CompareTo(other.amount);
		}

		// Token: 0x060008EF RID: 2287 RVA: 0x00030850 File Offset: 0x0002EA50
		public bool Equals(SIResource.ResourceCost other)
		{
			return this.type == other.type && this.amount == other.amount;
		}

		// Token: 0x060008F0 RID: 2288 RVA: 0x00030870 File Offset: 0x0002EA70
		public override bool Equals(object obj)
		{
			if (obj is SIResource.ResourceCost)
			{
				SIResource.ResourceCost other = (SIResource.ResourceCost)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x060008F1 RID: 2289 RVA: 0x00030895 File Offset: 0x0002EA95
		public override int GetHashCode()
		{
			return HashCode.Combine<int, int>((int)this.type, this.amount);
		}

		// Token: 0x060008F2 RID: 2290 RVA: 0x000308A8 File Offset: 0x0002EAA8
		public override string ToString()
		{
			return string.Format("{0}: {1}", this.type.ToString(), this.amount);
		}

		// Token: 0x04000B00 RID: 2816
		public SIResource.ResourceType type;

		// Token: 0x04000B01 RID: 2817
		public int amount;
	}

	// Token: 0x02000152 RID: 338
	public struct ResourceCategoryCost : IComparable<SIResource.ResourceCategoryCost>, IEquatable<SIResource.ResourceCategoryCost>
	{
		// Token: 0x060008F3 RID: 2291 RVA: 0x000308D0 File Offset: 0x0002EAD0
		public ResourceCategoryCost(int techPoints, int misc)
		{
			this.techPoints = techPoints;
			this.misc = misc;
		}

		// Token: 0x060008F4 RID: 2292 RVA: 0x000308E0 File Offset: 0x0002EAE0
		public int CompareTo(SIResource.ResourceCategoryCost other)
		{
			int num = this.techPoints.CompareTo(other.techPoints);
			if (num != 0)
			{
				return num;
			}
			return this.misc.CompareTo(other.misc);
		}

		// Token: 0x060008F5 RID: 2293 RVA: 0x00030915 File Offset: 0x0002EB15
		public bool Equals(SIResource.ResourceCategoryCost other)
		{
			return this.techPoints == other.techPoints && this.misc == other.misc;
		}

		// Token: 0x060008F6 RID: 2294 RVA: 0x00030935 File Offset: 0x0002EB35
		public static bool operator ==(SIResource.ResourceCategoryCost left, SIResource.ResourceCategoryCost right)
		{
			return left.Equals(right);
		}

		// Token: 0x060008F7 RID: 2295 RVA: 0x0003093F File Offset: 0x0002EB3F
		public static bool operator !=(SIResource.ResourceCategoryCost left, SIResource.ResourceCategoryCost right)
		{
			return !left.Equals(right);
		}

		// Token: 0x060008F8 RID: 2296 RVA: 0x0003094C File Offset: 0x0002EB4C
		public static SIResource.ResourceCategoryCost operator +(SIResource.ResourceCategoryCost left, SIResource.ResourceCategoryCost right)
		{
			return new SIResource.ResourceCategoryCost(left.techPoints + right.techPoints, left.misc + right.misc);
		}

		// Token: 0x060008F9 RID: 2297 RVA: 0x0003096D File Offset: 0x0002EB6D
		public static SIResource.ResourceCategoryCost operator -(SIResource.ResourceCategoryCost left, SIResource.ResourceCategoryCost right)
		{
			return new SIResource.ResourceCategoryCost(left.techPoints - right.techPoints, left.misc - right.misc);
		}

		// Token: 0x060008FA RID: 2298 RVA: 0x0003098E File Offset: 0x0002EB8E
		public static SIResource.ResourceCategoryCost operator *(SIResource.ResourceCategoryCost cost, int multiple)
		{
			return new SIResource.ResourceCategoryCost(cost.techPoints * multiple, cost.misc * multiple);
		}

		// Token: 0x060008FB RID: 2299 RVA: 0x000309A5 File Offset: 0x0002EBA5
		public static SIResource.ResourceCategoryCost operator *(int multiple, SIResource.ResourceCategoryCost cost)
		{
			return new SIResource.ResourceCategoryCost(cost.techPoints * multiple, cost.misc * multiple);
		}

		// Token: 0x060008FC RID: 2300 RVA: 0x000309BC File Offset: 0x0002EBBC
		public static SIResource.ResourceCategoryCost Max(SIResource.ResourceCategoryCost left, SIResource.ResourceCategoryCost right)
		{
			return new SIResource.ResourceCategoryCost(Mathf.Max(left.techPoints, right.techPoints), Mathf.Max(left.misc, right.misc));
		}

		// Token: 0x060008FD RID: 2301 RVA: 0x000309E5 File Offset: 0x0002EBE5
		public override int GetHashCode()
		{
			return HashCode.Combine<int, int>(this.techPoints, this.misc);
		}

		// Token: 0x04000B02 RID: 2818
		public int techPoints;

		// Token: 0x04000B03 RID: 2819
		public int misc;
	}

	// Token: 0x02000153 RID: 339
	public enum ResourceType
	{
		// Token: 0x04000B05 RID: 2821
		TechPoint,
		// Token: 0x04000B06 RID: 2822
		StrangeWood,
		// Token: 0x04000B07 RID: 2823
		WeirdGear,
		// Token: 0x04000B08 RID: 2824
		VibratingSpring,
		// Token: 0x04000B09 RID: 2825
		BouncySand,
		// Token: 0x04000B0A RID: 2826
		FloppyMetal,
		// Token: 0x04000B0B RID: 2827
		Count
	}

	// Token: 0x02000154 RID: 340
	public enum LimitedDepositType
	{
		// Token: 0x04000B0D RID: 2829
		None,
		// Token: 0x04000B0E RID: 2830
		MonkeIdol,
		// Token: 0x04000B0F RID: 2831
		Count
	}
}
