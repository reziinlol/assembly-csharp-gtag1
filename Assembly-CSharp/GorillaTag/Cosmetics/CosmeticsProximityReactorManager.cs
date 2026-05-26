using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001268 RID: 4712
	public class CosmeticsProximityReactorManager : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x17000B65 RID: 2917
		// (get) Token: 0x06007615 RID: 30229 RVA: 0x0026AAE0 File Offset: 0x00268CE0
		public static CosmeticsProximityReactorManager Instance
		{
			get
			{
				return CosmeticsProximityReactorManager._instance;
			}
		}

		// Token: 0x17000B66 RID: 2918
		// (get) Token: 0x06007616 RID: 30230 RVA: 0x0026AAE7 File Offset: 0x00268CE7
		public IReadOnlyList<CosmeticsProximityReactor> Cosmetics
		{
			get
			{
				return this.cosmetics;
			}
		}

		// Token: 0x140000C2 RID: 194
		// (add) Token: 0x06007617 RID: 30231 RVA: 0x0026AAF0 File Offset: 0x00268CF0
		// (remove) Token: 0x06007618 RID: 30232 RVA: 0x0026AB24 File Offset: 0x00268D24
		public static event Action<CosmeticsProximityReactor> OnCosmeticRegistered;

		// Token: 0x06007619 RID: 30233 RVA: 0x0026AB57 File Offset: 0x00268D57
		private void Awake()
		{
			if (CosmeticsProximityReactorManager._instance != null && CosmeticsProximityReactorManager._instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			CosmeticsProximityReactorManager._instance = this;
		}

		// Token: 0x0600761A RID: 30234 RVA: 0x00018E08 File Offset: 0x00017008
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x0600761B RID: 30235 RVA: 0x0026AB85 File Offset: 0x00268D85
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			if (CosmeticsProximityReactorManager._instance == this)
			{
				CosmeticsProximityReactorManager._instance = null;
			}
		}

		// Token: 0x0600761C RID: 30236 RVA: 0x0026ABA4 File Offset: 0x00268DA4
		public void Register(CosmeticsProximityReactor cosmetic)
		{
			if (cosmetic == null)
			{
				return;
			}
			if (cosmetic.IsGorillaBody())
			{
				if (!this.gorillaBodyPart.Contains(cosmetic))
				{
					this.gorillaBodyPart.Add(cosmetic);
				}
				return;
			}
			if (!this.cosmetics.Contains(cosmetic))
			{
				this.cosmetics.Add(cosmetic);
				Action<CosmeticsProximityReactor> onCosmeticRegistered = CosmeticsProximityReactorManager.OnCosmeticRegistered;
				if (onCosmeticRegistered != null)
				{
					onCosmeticRegistered(cosmetic);
				}
			}
			IReadOnlyList<string> types = cosmetic.GetTypes();
			for (int i = 0; i < types.Count; i++)
			{
				string text = types[i];
				if (!string.IsNullOrEmpty(text))
				{
					List<CosmeticsProximityReactor> list;
					if (!this.byType.TryGetValue(text, out list))
					{
						list = new List<CosmeticsProximityReactor>();
						this.byType[text] = list;
					}
					if (!list.Contains(cosmetic))
					{
						list.Add(cosmetic);
						this.typeKeysDirty = true;
					}
				}
			}
		}

		// Token: 0x0600761D RID: 30237 RVA: 0x0026AC6C File Offset: 0x00268E6C
		public void Unregister(CosmeticsProximityReactor cosmetic)
		{
			if (cosmetic == null)
			{
				return;
			}
			this.cosmetics.Remove(cosmetic);
			this.gorillaBodyPart.Remove(cosmetic);
			this.matchedFrame.Remove(cosmetic);
			foreach (KeyValuePair<string, List<CosmeticsProximityReactor>> keyValuePair in this.byType)
			{
				if (keyValuePair.Value.Remove(cosmetic))
				{
					this.typeKeysDirty = true;
				}
			}
		}

		// Token: 0x0600761E RID: 30238 RVA: 0x0026AD00 File Offset: 0x00268F00
		public void SliceUpdate()
		{
			if (this.cosmetics.Count == 0)
			{
				return;
			}
			if (this.AnyGroupHasTwo())
			{
				if (this.typeKeysDirty)
				{
					this.RebuildTypeKeysCache();
				}
				if (this.typeKeysCache.Count > 0)
				{
					for (int i = 0; i < this.typeKeysCache.Count; i++)
					{
						string key = this.typeKeysCache[i];
						List<CosmeticsProximityReactor> list;
						if (this.byType.TryGetValue(key, out list) && list != null && list.Count > 0)
						{
							this.ProcessOneGroup(list);
						}
					}
				}
			}
			if (this.gorillaBodyPart.Count > 0)
			{
				for (int j = 0; j < this.cosmetics.Count; j++)
				{
					CosmeticsProximityReactor cosmeticsProximityReactor = this.cosmetics[j];
					if (!(cosmeticsProximityReactor == null))
					{
						if (!cosmeticsProximityReactor.AcceptsAnySource())
						{
							cosmeticsProximityReactor.OnSourceAboveAll();
						}
						else
						{
							bool flag = false;
							Vector3 contact = default(Vector3);
							for (int k = 0; k < this.gorillaBodyPart.Count; k++)
							{
								CosmeticsProximityReactor cosmeticsProximityReactor2 = this.gorillaBodyPart[k];
								if (!(cosmeticsProximityReactor2 == null) && cosmeticsProximityReactor.AcceptsThisSource(cosmeticsProximityReactor2.gorillaBodyParts))
								{
									bool flag2;
									float sourceThresholdFor = cosmeticsProximityReactor.GetSourceThresholdFor(cosmeticsProximityReactor2, out flag2);
									Vector3 vector;
									if (flag2 && CosmeticsProximityReactorManager.AreCollidersWithinThreshold(cosmeticsProximityReactor2, cosmeticsProximityReactor, sourceThresholdFor, out vector))
									{
										cosmeticsProximityReactor.OnSourceBelow(vector, cosmeticsProximityReactor2.gorillaBodyParts, cosmeticsProximityReactor2.GetComponentInParent<VRRig>());
										contact = vector;
										flag = true;
									}
								}
							}
							if (flag)
							{
								cosmeticsProximityReactor.WhileSourceBelow(contact, CosmeticsProximityReactor.GorillaBodyPart.HandLeft | CosmeticsProximityReactor.GorillaBodyPart.HandRight | CosmeticsProximityReactor.GorillaBodyPart.Mouth, (this.gorillaBodyPart[0] != null) ? this.gorillaBodyPart[0].GetComponentInParent<VRRig>() : null);
							}
							else
							{
								cosmeticsProximityReactor.OnSourceAboveAll();
							}
						}
					}
				}
			}
			if (this.typeKeysDirty)
			{
				this.RebuildTypeKeysCache();
			}
			for (int l = 0; l < this.typeKeysCache.Count; l++)
			{
				string key2 = this.typeKeysCache[l];
				List<CosmeticsProximityReactor> list2;
				if (this.byType.TryGetValue(key2, out list2) && list2 != null && list2.Count > 0)
				{
					this.BreakTheBoundForGroup(list2);
				}
			}
		}

		// Token: 0x0600761F RID: 30239 RVA: 0x0026AF0B File Offset: 0x0026910B
		private void ProcessOneGroup(List<CosmeticsProximityReactor> group)
		{
			if (!this.CheckProximity(group))
			{
				this.BreakTheBoundForGroup(group);
			}
		}

		// Token: 0x06007620 RID: 30240 RVA: 0x0026AF20 File Offset: 0x00269120
		private bool CheckProximity(List<CosmeticsProximityReactor> group)
		{
			bool result = false;
			for (int i = 0; i < group.Count; i++)
			{
				CosmeticsProximityReactor cosmeticsProximityReactor = group[i];
				if (!(cosmeticsProximityReactor == null))
				{
					for (int j = i + 1; j < group.Count; j++)
					{
						CosmeticsProximityReactor cosmeticsProximityReactor2 = group[j];
						if (!(cosmeticsProximityReactor2 == null) && !CosmeticsProximityReactorManager.ShouldSkipSameIdPair(cosmeticsProximityReactor, cosmeticsProximityReactor2))
						{
							bool flag;
							float cosmeticPairThresholdWith = cosmeticsProximityReactor.GetCosmeticPairThresholdWith(cosmeticsProximityReactor2, out flag);
							bool flag2;
							float cosmeticPairThresholdWith2 = cosmeticsProximityReactor2.GetCosmeticPairThresholdWith(cosmeticsProximityReactor, out flag2);
							if (flag || flag2)
							{
								float num = float.MaxValue;
								if (flag && cosmeticPairThresholdWith < num)
								{
									num = cosmeticPairThresholdWith;
								}
								if (flag2 && cosmeticPairThresholdWith2 < num)
								{
									num = cosmeticPairThresholdWith2;
								}
								Vector3 contact;
								if (CosmeticsProximityReactorManager.AreCollidersWithinThreshold(cosmeticsProximityReactor, cosmeticsProximityReactor2, num, out contact))
								{
									cosmeticsProximityReactor.OnCosmeticBelowWith(cosmeticsProximityReactor2, contact);
									cosmeticsProximityReactor2.OnCosmeticBelowWith(cosmeticsProximityReactor, contact);
									if (cosmeticsProximityReactor.IsBelow)
									{
										cosmeticsProximityReactor.RefreshAggregateMatched();
										this.matchedFrame[cosmeticsProximityReactor] = Time.frameCount;
										result = true;
									}
									if (cosmeticsProximityReactor2.IsBelow)
									{
										cosmeticsProximityReactor2.RefreshAggregateMatched();
										this.matchedFrame[cosmeticsProximityReactor2] = Time.frameCount;
										result = true;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06007621 RID: 30241 RVA: 0x0026B044 File Offset: 0x00269244
		private void BreakTheBoundForGroup(List<CosmeticsProximityReactor> group)
		{
			for (int i = 0; i < group.Count; i++)
			{
				CosmeticsProximityReactor cosmeticsProximityReactor = group[i];
				int num;
				if (!(cosmeticsProximityReactor == null) && cosmeticsProximityReactor.HasAnyCosmeticMatch() && (!this.matchedFrame.TryGetValue(cosmeticsProximityReactor, out num) || num != Time.frameCount))
				{
					CosmeticsProximityReactor cosmeticsProximityReactor2;
					Vector3 contact;
					if (this.TryFindAnyCosmeticPartner(cosmeticsProximityReactor, out cosmeticsProximityReactor2, out contact))
					{
						cosmeticsProximityReactor.WhileCosmeticBelowWith(cosmeticsProximityReactor2, contact);
						cosmeticsProximityReactor2.WhileCosmeticBelowWith(cosmeticsProximityReactor, contact);
					}
					else
					{
						cosmeticsProximityReactor.OnCosmeticAboveAll();
					}
				}
			}
		}

		// Token: 0x06007622 RID: 30242 RVA: 0x0026B0BC File Offset: 0x002692BC
		private bool TryFindAnyCosmeticPartner(CosmeticsProximityReactor a, out CosmeticsProximityReactor partner, out Vector3 contact)
		{
			partner = null;
			contact = default(Vector3);
			IReadOnlyList<string> types = a.GetTypes();
			for (int i = 0; i < types.Count; i++)
			{
				string text = types[i];
				List<CosmeticsProximityReactor> list;
				if (!string.IsNullOrEmpty(text) && this.byType.TryGetValue(text, out list) && list != null)
				{
					for (int j = 0; j < list.Count; j++)
					{
						CosmeticsProximityReactor cosmeticsProximityReactor = list[j];
						if (!(cosmeticsProximityReactor == null) && !(cosmeticsProximityReactor == a) && !CosmeticsProximityReactorManager.ShouldSkipSameIdPair(a, cosmeticsProximityReactor))
						{
							bool flag;
							float cosmeticPairThresholdWith = a.GetCosmeticPairThresholdWith(cosmeticsProximityReactor, out flag);
							if (flag)
							{
								float threshold = cosmeticPairThresholdWith;
								Vector3 vector;
								if (CosmeticsProximityReactorManager.AreCollidersWithinThreshold(a, cosmeticsProximityReactor, threshold, out vector))
								{
									partner = cosmeticsProximityReactor;
									contact = vector;
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06007623 RID: 30243 RVA: 0x0026B188 File Offset: 0x00269388
		private static bool ShouldSkipSameIdPair(CosmeticsProximityReactor a, CosmeticsProximityReactor b)
		{
			return (a.ignoreSameCosmeticInstances || b.ignoreSameCosmeticInstances) && !string.IsNullOrEmpty(a.PlayFabID) && !string.IsNullOrEmpty(b.PlayFabID) && string.Equals(a.PlayFabID, b.PlayFabID, StringComparison.Ordinal);
		}

		// Token: 0x06007624 RID: 30244 RVA: 0x0026B1D8 File Offset: 0x002693D8
		private static bool AreCollidersWithinThreshold(CosmeticsProximityReactor a, CosmeticsProximityReactor b, float threshold, out Vector3 contactPoint)
		{
			Vector3 vector = (b.collider == null) ? b.transform.position : b.collider.ClosestPoint(a.transform.position);
			Vector3 a2 = (a.collider == null) ? a.transform.position : a.collider.ClosestPoint(vector);
			contactPoint = (a2 + vector) * 0.5f;
			return Vector3.Distance(a2, vector) <= threshold;
		}

		// Token: 0x06007625 RID: 30245 RVA: 0x0026B264 File Offset: 0x00269464
		private bool AnyGroupHasTwo()
		{
			foreach (KeyValuePair<string, List<CosmeticsProximityReactor>> keyValuePair in this.byType)
			{
				List<CosmeticsProximityReactor> value = keyValuePair.Value;
				if (value != null && value.Count >= 2)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06007626 RID: 30246 RVA: 0x0026B2CC File Offset: 0x002694CC
		private void RebuildTypeKeysCache()
		{
			this.typeKeysCache.Clear();
			foreach (KeyValuePair<string, List<CosmeticsProximityReactor>> keyValuePair in this.byType)
			{
				List<CosmeticsProximityReactor> value = keyValuePair.Value;
				if (value != null && value.Count > 0)
				{
					this.typeKeysCache.Add(keyValuePair.Key);
				}
			}
			this.typeKeysDirty = false;
			if (this.groupCursor >= this.typeKeysCache.Count)
			{
				this.groupCursor = 0;
			}
		}

		// Token: 0x040087E4 RID: 34788
		private static CosmeticsProximityReactorManager _instance;

		// Token: 0x040087E5 RID: 34789
		private readonly List<CosmeticsProximityReactor> cosmetics = new List<CosmeticsProximityReactor>();

		// Token: 0x040087E6 RID: 34790
		private readonly List<CosmeticsProximityReactor> gorillaBodyPart = new List<CosmeticsProximityReactor>();

		// Token: 0x040087E8 RID: 34792
		private readonly Dictionary<string, List<CosmeticsProximityReactor>> byType = new Dictionary<string, List<CosmeticsProximityReactor>>(StringComparer.Ordinal);

		// Token: 0x040087E9 RID: 34793
		private readonly Dictionary<CosmeticsProximityReactor, int> matchedFrame = new Dictionary<CosmeticsProximityReactor, int>();

		// Token: 0x040087EA RID: 34794
		private readonly List<string> typeKeysCache = new List<string>();

		// Token: 0x040087EB RID: 34795
		private bool typeKeysDirty;

		// Token: 0x040087EC RID: 34796
		private int groupCursor;

		// Token: 0x040087ED RID: 34797
		internal static readonly List<string> SharedKeysCache = new List<string>();
	}
}
