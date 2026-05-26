using System;
using System.Collections.Generic;
using System.Diagnostics;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.UI;

namespace GorillaTag
{
	// Token: 0x0200114E RID: 4430
	[DefaultExecutionOrder(2000)]
	public class StaticLodManager : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x06007042 RID: 28738 RVA: 0x00249825 File Offset: 0x00247A25
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
			this.mainCamera = Camera.main;
			this.hasMainCamera = (this.mainCamera != null);
		}

		// Token: 0x06007043 RID: 28739 RVA: 0x00011DE0 File Offset: 0x0000FFE0
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x06007044 RID: 28740 RVA: 0x0024984C File Offset: 0x00247A4C
		public static int Register(StaticLodGroup lodGroup)
		{
			if (lodGroup == null)
			{
				return -1;
			}
			int count;
			if (StaticLodManager.freeSlots.TryPop(out count))
			{
				StaticLodManager.groupMonoBehaviours[count] = lodGroup;
				StaticLodManager.groupInfos[count] = default(StaticLodManager.GroupInfo);
			}
			else
			{
				count = StaticLodManager.groupMonoBehaviours.Count;
				StaticLodManager.groupMonoBehaviours.Add(lodGroup);
				StaticLodManager.groupInfos.Add(default(StaticLodManager.GroupInfo));
			}
			StaticLodManager._groupInstId_to_index[lodGroup.GetInstanceID()] = count;
			StaticLodManager.GroupInfo groupInfo = StaticLodManager.groupInfos[count];
			groupInfo.isLoaded = true;
			groupInfo.componentEnabled = lodGroup.isActiveAndEnabled;
			groupInfo.uiEnabled = true;
			groupInfo.collidersEnabled = true;
			groupInfo.uiEnableDistanceSq = lodGroup.uiFadeDistanceMax * lodGroup.uiFadeDistanceMax;
			groupInfo.collisionEnableDistanceSq = lodGroup.collisionEnableDistance * lodGroup.collisionEnableDistance;
			StaticLodManager.groupInfos[count] = groupInfo;
			StaticLodManager._TryAddMembersToLodGroup(true, count);
			groupInfo = StaticLodManager.groupInfos[count];
			if (Mathf.Approximately(groupInfo.radiusSq, 0f))
			{
				groupInfo.bounds = new Bounds(lodGroup.transform.position, Vector3.one * 0.01f);
				groupInfo.center = groupInfo.bounds.center;
				groupInfo.radiusSq = groupInfo.bounds.extents.sqrMagnitude;
				StaticLodManager.groupInfos[count] = groupInfo;
			}
			return count;
		}

		// Token: 0x06007045 RID: 28741 RVA: 0x002499BC File Offset: 0x00247BBC
		public static int OldRegister(StaticLodGroup lodGroup)
		{
			StaticLodGroupExcluder componentInParent = lodGroup.GetComponentInParent<StaticLodGroupExcluder>();
			List<Graphic> list;
			int num;
			using (lodGroup.GTGetComponentsListPool(true, out list))
			{
				for (int i = list.Count - 1; i >= 0; i--)
				{
					StaticLodGroupExcluder componentInParent2 = list[i].GetComponentInParent<StaticLodGroupExcluder>(true);
					if (componentInParent2 != null && componentInParent2 != componentInParent)
					{
						list.RemoveAt(i);
					}
				}
				Graphic[] array = list.ToArray();
				List<Renderer> list2;
				using (lodGroup.GTGetComponentsListPool(true, out list2))
				{
					for (int j = list2.Count - 1; j >= 0; j--)
					{
						num = list2[j].gameObject.layer;
						if ((num != 5 && num != 18) || !list2[j].enabled)
						{
							list2.RemoveAt(j);
						}
						else
						{
							StaticLodGroupExcluder componentInParent3 = list[j].GetComponentInParent<StaticLodGroupExcluder>(true);
							if (componentInParent3 != null && componentInParent3 != componentInParent)
							{
								list2.RemoveAt(j);
							}
						}
					}
					Renderer[] array2 = list2.ToArray();
					List<Collider> list3;
					using (lodGroup.GTGetComponentsListPool(true, out list3))
					{
						for (int k = 0; k < list3.Count; k++)
						{
							Collider collider = list3[k];
							if (!collider.gameObject.IsOnLayer(UnityLayer.GorillaInteractable))
							{
								list3.RemoveAt(k);
							}
							else
							{
								StaticLodGroupExcluder componentInParent4 = collider.GetComponentInParent<StaticLodGroupExcluder>();
								if (componentInParent4 != null && componentInParent4 != componentInParent)
								{
									list3.RemoveAt(k);
								}
							}
						}
						Collider[] array3 = list3.ToArray();
						Bounds bounds = (array2.Length != 0) ? array2[0].bounds : ((array3.Length != 0) ? array3[0].bounds : ((array.Length != 0) ? new Bounds(array[0].transform.position, Vector3.one * 0.01f) : new Bounds(lodGroup.transform.position, Vector3.one * 0.01f)));
						for (int l = 0; l < array.Length; l++)
						{
							bounds.Encapsulate(array[l].transform.position);
						}
						for (int m = 0; m < array2.Length; m++)
						{
							bounds.Encapsulate(array2[m].bounds);
						}
						for (int n = 0; n < array3.Length; n++)
						{
							bounds.Encapsulate(array3[n].bounds);
						}
						StaticLodManager.GroupInfo groupInfo = new StaticLodManager.GroupInfo
						{
							isLoaded = true,
							componentEnabled = lodGroup.isActiveAndEnabled,
							center = bounds.center,
							radiusSq = bounds.extents.sqrMagnitude,
							uiEnabled = true,
							uiEnableDistanceSq = lodGroup.uiFadeDistanceMax * lodGroup.uiFadeDistanceMax,
							uiGraphics = array,
							renderers = array2,
							collidersEnabled = true,
							collisionEnableDistanceSq = lodGroup.collisionEnableDistance * lodGroup.collisionEnableDistance,
							interactableColliders = array3
						};
						int count;
						if (StaticLodManager.freeSlots.TryPop(out count))
						{
							StaticLodManager.groupMonoBehaviours[count] = lodGroup;
							StaticLodManager.groupInfos[count] = groupInfo;
						}
						else
						{
							count = StaticLodManager.groupMonoBehaviours.Count;
							StaticLodManager.groupMonoBehaviours.Add(lodGroup);
							StaticLodManager.groupInfos.Add(groupInfo);
						}
						StaticLodManager._groupInstId_to_index[lodGroup.GetInstanceID()] = count;
						num = count;
					}
				}
			}
			return num;
		}

		// Token: 0x06007046 RID: 28742 RVA: 0x00249D7C File Offset: 0x00247F7C
		public static void Unregister(int lodGroupIndex)
		{
			StaticLodGroup staticLodGroup = StaticLodManager.groupMonoBehaviours[lodGroupIndex];
			if (staticLodGroup != null)
			{
				StaticLodManager._groupInstId_to_index.Remove(staticLodGroup.GetInstanceID());
			}
			StaticLodManager.groupMonoBehaviours[lodGroupIndex] = null;
			StaticLodManager.groupInfos[lodGroupIndex] = default(StaticLodManager.GroupInfo);
			StaticLodManager.freeSlots.Push(lodGroupIndex);
		}

		// Token: 0x06007047 RID: 28743 RVA: 0x00249DDC File Offset: 0x00247FDC
		public static bool TryAddLateInstantiatedMembers(GameObject root)
		{
			StaticLodGroup componentInParent = root.GetComponentInParent<StaticLodGroup>(true);
			if (componentInParent == null)
			{
				return false;
			}
			int groupIndex;
			if (!StaticLodManager._groupInstId_to_index.TryGetValue(componentInParent.GetInstanceID(), out groupIndex))
			{
				return false;
			}
			if (componentInParent.gameObject != root)
			{
				StaticLodGroupExcluder componentInParent2 = root.GetComponentInParent<StaticLodGroupExcluder>(true);
				if (componentInParent2 != null && componentInParent.transform.GetDepth() < componentInParent2.transform.GetDepth())
				{
					return false;
				}
			}
			return StaticLodManager._TryAddMembersToLodGroup(false, groupIndex);
		}

		// Token: 0x06007048 RID: 28744 RVA: 0x00249E54 File Offset: 0x00248054
		private static bool _TryAddMembersToLodGroup(bool isNew, int groupIndex)
		{
			bool flag = false;
			StaticLodGroup lodGroup = StaticLodManager.groupMonoBehaviours[groupIndex];
			StaticLodManager.GroupInfo value = StaticLodManager.groupInfos[groupIndex];
			bool result = flag | StaticLodManager._TryAddComponentsToGroup<Collider>(lodGroup, ref value, ref value.interactableColliders, (Collider coll) => coll.gameObject.IsOnLayer(UnityLayer.GorillaInteractable), (Collider coll) => coll.bounds) | StaticLodManager._TryAddComponentsToGroup<Renderer>(lodGroup, ref value, ref value.renderers, delegate(Renderer rend)
			{
				int layer = rend.gameObject.layer;
				return (layer == 5 || layer == 18) && rend.enabled;
			}, (Renderer rend) => rend.bounds) | StaticLodManager._TryAddComponentsToGroup<Graphic>(lodGroup, ref value, ref value.uiGraphics, (Graphic _) => true, (Graphic gfx) => new Bounds(gfx.transform.position, Vector3.one * 0.01f));
			StaticLodManager.groupInfos[groupIndex] = value;
			return result;
		}

		// Token: 0x06007049 RID: 28745 RVA: 0x00249F70 File Offset: 0x00248170
		private static bool _TryAddComponentsToGroup<T>(StaticLodGroup lodGroup, ref StaticLodManager.GroupInfo ref_groupInfo, ref T[] ref_components, Predicate<T> includeIf, StaticLodManager._GetBoundsDelegate<T> getBounds) where T : Component
		{
			List<T> componentsInChildrenUntil = lodGroup.GetComponentsInChildrenUntil(true, false, 64);
			for (int i = componentsInChildrenUntil.Count - 1; i >= 0; i--)
			{
				if (!includeIf(componentsInChildrenUntil[i]))
				{
					componentsInChildrenUntil.RemoveAt(i);
				}
			}
			if (componentsInChildrenUntil.Count == 0)
			{
				if (ref_components == null)
				{
					ref_components = Array.Empty<T>();
				}
				return false;
			}
			T[] array = ref_components;
			int num = (array != null) ? array.Length : 0;
			if (num == 0)
			{
				ref_components = componentsInChildrenUntil.ToArray();
			}
			else
			{
				Array.Resize<T>(ref ref_components, num + componentsInChildrenUntil.Count);
				for (int j = num; j < ref_components.Length; j++)
				{
					ref_components[j] = componentsInChildrenUntil[j - num];
				}
			}
			if (Mathf.Approximately(ref_groupInfo.radiusSq, 0f))
			{
				ref_groupInfo.bounds = getBounds(ref_components[0]);
			}
			for (int k = num; k < ref_components.Length; k++)
			{
				ref_groupInfo.bounds.Encapsulate(getBounds(ref_components[k]));
			}
			ref_groupInfo.center = ref_groupInfo.bounds.center;
			ref_groupInfo.radiusSq = ref_groupInfo.bounds.extents.sqrMagnitude;
			return true;
		}

		// Token: 0x0600704A RID: 28746 RVA: 0x000028C5 File Offset: 0x00000AC5
		[Conditional("UNITY_EDITOR")]
		private static void _EdAddPathsToGroup<T>(T[] components, ref string[] ref_edDebugPaths) where T : Component
		{
		}

		// Token: 0x0600704B RID: 28747 RVA: 0x0024A090 File Offset: 0x00248290
		public static void SetEnabled(int index, bool enable)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (StaticLodManager.groupInfos == null || index < 0 || index >= StaticLodManager.groupInfos.Count)
			{
				return;
			}
			StaticLodManager.GroupInfo value = StaticLodManager.groupInfos[index];
			value.componentEnabled = enable;
			StaticLodManager.groupInfos[index] = value;
		}

		// Token: 0x0600704C RID: 28748 RVA: 0x0024A0E0 File Offset: 0x002482E0
		public void SliceUpdate()
		{
			if (!this.hasMainCamera)
			{
				return;
			}
			Vector3 position = this.mainCamera.transform.position;
			for (int i = 0; i < StaticLodManager.groupInfos.Count; i++)
			{
				StaticLodManager.GroupInfo groupInfo = StaticLodManager.groupInfos[i];
				if (groupInfo.isLoaded && groupInfo.componentEnabled)
				{
					float num = Mathf.Max(0f, (groupInfo.center - position).sqrMagnitude - groupInfo.radiusSq);
					float num2 = groupInfo.uiEnabled ? 0.010000001f : 0f;
					bool flag = num < groupInfo.uiEnableDistanceSq + num2;
					if (flag != groupInfo.uiEnabled)
					{
						for (int j = 0; j < groupInfo.uiGraphics.Length; j++)
						{
							Graphic graphic = groupInfo.uiGraphics[j];
							if (!(graphic == null))
							{
								graphic.enabled = flag;
							}
						}
						for (int k = 0; k < groupInfo.renderers.Length; k++)
						{
							Renderer renderer = groupInfo.renderers[k];
							if (!(renderer == null))
							{
								renderer.enabled = flag;
							}
						}
					}
					groupInfo.uiEnabled = flag;
					num2 = (groupInfo.collidersEnabled ? 0.010000001f : 0f);
					bool flag2 = num < groupInfo.collisionEnableDistanceSq + num2;
					if (flag2 != groupInfo.collidersEnabled)
					{
						for (int l = 0; l < groupInfo.interactableColliders.Length; l++)
						{
							if (!(groupInfo.interactableColliders[l] == null))
							{
								groupInfo.interactableColliders[l].enabled = flag2;
							}
						}
					}
					groupInfo.collidersEnabled = flag2;
					StaticLodManager.groupInfos[i] = groupInfo;
				}
			}
		}

		// Token: 0x0400801B RID: 32795
		[OnEnterPlay_Clear]
		private static readonly List<StaticLodGroup> groupMonoBehaviours = new List<StaticLodGroup>(256);

		// Token: 0x0400801C RID: 32796
		[OnEnterPlay_Clear]
		private static readonly Dictionary<int, int> _groupInstId_to_index = new Dictionary<int, int>(256);

		// Token: 0x0400801D RID: 32797
		[DebugReadout]
		[OnEnterPlay_Clear]
		private static readonly List<StaticLodManager.GroupInfo> groupInfos = new List<StaticLodManager.GroupInfo>(256);

		// Token: 0x0400801E RID: 32798
		[OnEnterPlay_Clear]
		private static readonly Stack<int> freeSlots = new Stack<int>();

		// Token: 0x0400801F RID: 32799
		private Camera mainCamera;

		// Token: 0x04008020 RID: 32800
		private bool hasMainCamera;

		// Token: 0x0200114F RID: 4431
		private struct GroupInfo
		{
			// Token: 0x04008021 RID: 32801
			public bool isLoaded;

			// Token: 0x04008022 RID: 32802
			public bool componentEnabled;

			// Token: 0x04008023 RID: 32803
			public Vector3 center;

			// Token: 0x04008024 RID: 32804
			public float radiusSq;

			// Token: 0x04008025 RID: 32805
			public Bounds bounds;

			// Token: 0x04008026 RID: 32806
			public bool uiEnabled;

			// Token: 0x04008027 RID: 32807
			public float uiEnableDistanceSq;

			// Token: 0x04008028 RID: 32808
			public Graphic[] uiGraphics;

			// Token: 0x04008029 RID: 32809
			public Renderer[] renderers;

			// Token: 0x0400802A RID: 32810
			public bool collidersEnabled;

			// Token: 0x0400802B RID: 32811
			public float collisionEnableDistanceSq;

			// Token: 0x0400802C RID: 32812
			public Collider[] interactableColliders;
		}

		// Token: 0x02001150 RID: 4432
		// (Invoke) Token: 0x06007050 RID: 28752
		private delegate Bounds _GetBoundsDelegate<in T>(T t) where T : Component;
	}
}
