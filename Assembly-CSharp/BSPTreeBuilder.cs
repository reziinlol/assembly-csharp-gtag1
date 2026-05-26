using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000E17 RID: 3607
public static class BSPTreeBuilder
{
	// Token: 0x060057EA RID: 22506 RVA: 0x001C83D4 File Offset: 0x001C65D4
	public static SerializableBSPTree BuildTree(ZoneDef[] zones)
	{
		List<BSPTreeBuilder.BoxMetadata> list = new List<BSPTreeBuilder.BoxMetadata>();
		List<MatrixZonePair> list2 = new List<MatrixZonePair>();
		new List<BoxCollider>();
		for (int i = 0; i < zones.Length; i++)
		{
			ZoneDef zoneDef = zones[i];
			List<BoxCollider> list3 = new List<BoxCollider>();
			zoneDef.GetComponents<BoxCollider>(list3);
			list3.AddRange(zoneDef.transform.GetComponentsInChildren<BoxCollider>());
			Debug.Log(string.Format("SerializableBSPTree zone {0}/{1} box count {2}", zoneDef.zoneId, zoneDef.subZoneId, list3.Count));
			foreach (BoxCollider boxCollider in list3)
			{
				int count = list2.Count;
				int zoneIndex = Array.IndexOf<ZoneDef>(zones.ToArray<ZoneDef>(), zoneDef);
				list2.Add(new MatrixZonePair
				{
					matrix = BoxColliderUtils.GetWorldToNormalizedBoxMatrix(boxCollider),
					zoneIndex = zoneIndex
				});
				list.Add(new BSPTreeBuilder.BoxMetadata(boxCollider, zoneDef, count, zones.Length - i));
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		List<SerializableBSPNode> list4 = new List<SerializableBSPNode>();
		List<MatrixBSPNode> list5 = new List<MatrixBSPNode>();
		Dictionary<ValueTuple<int, int>, int> matrixNodeCache = new Dictionary<ValueTuple<int, int>, int>();
		int num = 0;
		list5.Add(new MatrixBSPNode
		{
			matrixIndex = -1,
			outsideChildIndex = 0
		});
		global::BoundsInt bounds = BSPTreeBuilder.CalculateWorldBounds(list);
		int num2 = BSPTreeBuilder.BuildTreeRecursive(zones.ToArray<ZoneDef>(), list, bounds, 0, SerializableBSPNode.Axis.X, list4, list5, matrixNodeCache, ref num);
		BSPTreeBuilder.CleanupUnreferencedMatrices(list5, list2);
		List<SerializableBSPNode> list6 = new List<SerializableBSPNode>(list4);
		int count2 = list6.Count;
		for (int j = 0; j < list5.Count; j++)
		{
			MatrixBSPNode matrixBSPNode = list5[j];
			if (matrixBSPNode.matrixIndex < 0)
			{
				SerializableBSPNode serializableBSPNode = new SerializableBSPNode
				{
					axis = SerializableBSPNode.Axis.Zone,
					splitValue = 0f,
					leftChildIndex = (short)matrixBSPNode.outsideChildIndex,
					rightChildIndex = 0
				};
				SerializableBSPNode item = serializableBSPNode;
				list6.Add(item);
			}
			else
			{
				bool flag = matrixBSPNode.outsideChildIndex >= 0;
				SerializableBSPNode serializableBSPNode = new SerializableBSPNode
				{
					axis = (flag ? SerializableBSPNode.Axis.MatrixFinal : SerializableBSPNode.Axis.MatrixChain),
					splitValue = 0f,
					leftChildIndex = (short)matrixBSPNode.matrixIndex,
					rightChildIndex = (short)(flag ? matrixBSPNode.outsideChildIndex : (count2 - matrixBSPNode.outsideChildIndex))
				};
				SerializableBSPNode item2 = serializableBSPNode;
				list6.Add(item2);
			}
		}
		for (int k = 0; k < list6.Count; k++)
		{
			SerializableBSPNode serializableBSPNode2 = list6[k];
			bool flag2 = false;
			SerializableBSPNode.Axis axis = serializableBSPNode2.axis;
			if (axis <= SerializableBSPNode.Axis.Z)
			{
				if (serializableBSPNode2.leftChildIndex < 0)
				{
					serializableBSPNode2.leftChildIndex = (short)(count2 - (int)serializableBSPNode2.leftChildIndex);
					flag2 = true;
				}
				if (serializableBSPNode2.rightChildIndex < 0)
				{
					serializableBSPNode2.rightChildIndex = (short)(count2 - (int)serializableBSPNode2.rightChildIndex);
					flag2 = true;
				}
			}
			if (flag2)
			{
				list6[k] = serializableBSPNode2;
			}
		}
		if (num2 < 0)
		{
			num2 = count2 - num2;
		}
		SerializableBSPTree serializableBSPTree = new SerializableBSPTree();
		serializableBSPTree.nodes = list6.ToArray();
		serializableBSPTree.matrices = list2.ToArray();
		serializableBSPTree.zones = zones.ToArray<ZoneDef>();
		serializableBSPTree.rootIndex = num2;
		int num3 = serializableBSPTree.nodes.Length * 12;
		int num4 = serializableBSPTree.matrices.Length * 68;
		int num5 = num3 + num4;
		Debug.Log(string.Format("Unified BSP Tree generated: {0} total nodes ({1} spatial + {2} matrix) ({3} bytes), {4} matrix-zone pairs ({5} bytes). Matrix nodes deduplicated: {6}. Total: {7} bytes", new object[]
		{
			serializableBSPTree.nodes.Length,
			list4.Count,
			list5.Count,
			num3,
			serializableBSPTree.matrices.Length,
			num4,
			num,
			num5
		}));
		return serializableBSPTree;
	}

	// Token: 0x060057EB RID: 22507 RVA: 0x001C87B4 File Offset: 0x001C69B4
	private static int BuildTreeRecursive(ZoneDef[] zones, List<BSPTreeBuilder.BoxMetadata> boxes, global::BoundsInt bounds, int depth, SerializableBSPNode.Axis axis, List<SerializableBSPNode> nodeList, List<MatrixBSPNode> matrixNodeList, [TupleElementNames(new string[]
	{
		"matrixIndex",
		"outsideIndex"
	})] Dictionary<ValueTuple<int, int>, int> matrixNodeCache, ref int matrixNodeCacheHits)
	{
		Debug.Log(string.Format("Building node at depth {0} with {1} boxes, total nodes so far: {2}", depth, boxes.Count, nodeList.Count));
		int count = nodeList.Count;
		if (bounds.Contains(BSPTreeBuilder.testPoint))
		{
		}
		List<BSPTreeBuilder.BoxMetadata> list = new List<BSPTreeBuilder.BoxMetadata>();
		int num = -1;
		foreach (BSPTreeBuilder.BoxMetadata boxMetadata in boxes)
		{
			if (boxMetadata.bounds.GetIntersection(bounds) == bounds && BoxColliderUtils.DoesBoxContainRegion(boxMetadata.box, bounds))
			{
				list.Add(boxMetadata);
				num = Mathf.Max(new int[]
				{
					boxMetadata.priority
				});
			}
		}
		if (list.Count > 1)
		{
			foreach (BSPTreeBuilder.BoxMetadata boxMetadata2 in list)
			{
				if (boxMetadata2.priority < num)
				{
					boxes.Remove(boxMetadata2);
				}
				else if (boxMetadata2.priority == num)
				{
					num++;
				}
			}
		}
		bool flag = true;
		for (int i = 1; i < boxes.Count; i++)
		{
			BSPTreeBuilder.BoxMetadata boxMetadata3 = boxes[i];
			if (boxMetadata3.zone != boxes[0].zone)
			{
				flag = false;
				break;
			}
			if (boxMetadata3.bounds.GetIntersection(bounds) == bounds)
			{
				list.Add(boxMetadata3);
			}
		}
		if (flag || boxes.Count == 1)
		{
			return BSPTreeBuilder.CreateMatrixNodeTree(zones, boxes, matrixNodeList, bounds, matrixNodeCache, ref matrixNodeCacheHits);
		}
		if (depth >= 15)
		{
			Debug.LogWarning(string.Format("Maximum depth {0} reached with {1} boxes, creating matrix node tree", 15, boxes.Count));
			return BSPTreeBuilder.CreateMatrixNodeTree(zones, boxes, matrixNodeList, bounds, matrixNodeCache, ref matrixNodeCacheHits);
		}
		if (nodeList.Count >= 650)
		{
			Debug.LogWarning(string.Format("Maximum nodes {0} reached, creating matrix node tree", 650));
			return BSPTreeBuilder.CreateMatrixNodeTree(zones, boxes, matrixNodeList, bounds, matrixNodeCache, ref matrixNodeCacheHits);
		}
		if (boxes.Count <= 10)
		{
			Debug.Log(string.Format("Creating matrix node tree with {0} boxes at depth {1}", boxes.Count, depth));
			return BSPTreeBuilder.CreateMatrixNodeTree(zones, boxes, matrixNodeList, bounds, matrixNodeCache, ref matrixNodeCacheHits);
		}
		SerializableBSPNode serializableBSPNode = new SerializableBSPNode
		{
			axis = axis,
			leftChildIndex = -1,
			rightChildIndex = -1
		};
		nodeList.Add(serializableBSPNode);
		int num2;
		SerializableBSPNode.Axis axis2 = BSPTreeBuilder.FindBestAxis(boxes, bounds, axis, out num2);
		serializableBSPNode.axis = axis2;
		serializableBSPNode.splitValue = (float)num2 / 1000f;
		Debug.Log(string.Format("Best axis: {0}, split value: {1}", axis2, num2));
		global::BoundsInt boundsInt = bounds;
		global::BoundsInt boundsInt2 = bounds;
		switch (axis2)
		{
		case SerializableBSPNode.Axis.X:
			boundsInt.SetMinMax(bounds.min, new Vector3Int(num2, bounds.max.y, bounds.max.z));
			boundsInt2.SetMinMax(new Vector3Int(num2, bounds.min.y, bounds.min.z), bounds.max);
			break;
		case SerializableBSPNode.Axis.Y:
			boundsInt.SetMinMax(bounds.min, new Vector3Int(bounds.max.x, num2, bounds.max.z));
			boundsInt2.SetMinMax(new Vector3Int(bounds.min.x, num2, bounds.min.z), bounds.max);
			break;
		case SerializableBSPNode.Axis.Z:
			boundsInt.SetMinMax(bounds.min, new Vector3Int(bounds.max.x, bounds.max.y, num2));
			boundsInt2.SetMinMax(new Vector3Int(bounds.min.x, bounds.min.y, num2), bounds.max);
			break;
		}
		List<BSPTreeBuilder.BoxMetadata> effectiveBoxes = BSPTreeBuilder.GetEffectiveBoxes(boxes, boundsInt);
		IEnumerable<BSPTreeBuilder.BoxMetadata> effectiveBoxes2 = BSPTreeBuilder.GetEffectiveBoxes(boxes, boundsInt2);
		List<BSPTreeBuilder.BoxMetadata> list2 = new List<BSPTreeBuilder.BoxMetadata>(effectiveBoxes);
		List<BSPTreeBuilder.BoxMetadata> list3 = new List<BSPTreeBuilder.BoxMetadata>(effectiveBoxes2);
		Debug.Log(string.Format("Split result: leftBoxes={0}, rightBoxes={1}", list2.Count, list3.Count));
		if (list2.Count == 0 || list3.Count == 0)
		{
			Debug.Log(string.Format("No valid split found, creating matrix node tree with {0} boxes", boxes.Count));
			return BSPTreeBuilder.CreateMatrixNodeTree(zones, boxes, matrixNodeList, bounds, matrixNodeCache, ref matrixNodeCacheHits);
		}
		SerializableBSPNode.Axis nextAxis = BSPTreeBuilder.GetNextAxis(axis);
		serializableBSPNode.leftChildIndex = (short)BSPTreeBuilder.BuildTreeRecursive(zones, list2, boundsInt, depth + 1, nextAxis, nodeList, matrixNodeList, matrixNodeCache, ref matrixNodeCacheHits);
		serializableBSPNode.rightChildIndex = (short)BSPTreeBuilder.BuildTreeRecursive(zones, list3, boundsInt2, depth + 1, nextAxis, nodeList, matrixNodeList, matrixNodeCache, ref matrixNodeCacheHits);
		nodeList[count] = serializableBSPNode;
		return count;
	}

	// Token: 0x060057EC RID: 22508 RVA: 0x001C8C88 File Offset: 0x001C6E88
	private static SerializableBSPNode.Axis FindBestAxis(List<BSPTreeBuilder.BoxMetadata> boxes, global::BoundsInt bounds, SerializableBSPNode.Axis preferredAxis, out int bestSplitValue)
	{
		SerializableBSPNode.Axis[] array = new SerializableBSPNode.Axis[]
		{
			preferredAxis,
			BSPTreeBuilder.GetNextAxis(preferredAxis),
			BSPTreeBuilder.GetNextAxis(BSPTreeBuilder.GetNextAxis(preferredAxis))
		};
		SerializableBSPNode.Axis axis = preferredAxis;
		int num = int.MaxValue;
		bestSplitValue = BSPTreeBuilder.GetFallbackSplit(bounds, preferredAxis);
		foreach (SerializableBSPNode.Axis axis2 in array)
		{
			int num3;
			int num2 = BSPTreeBuilder.FindOptimalSplit(boxes, bounds, axis2, out num3);
			Debug.Log(string.Format("Axis {0}: split={1:F3}, score={2}", axis2, num2, num3));
			if (num3 < num)
			{
				num = num3;
				axis = axis2;
				bestSplitValue = num2;
			}
		}
		Debug.Log(string.Format("Selected axis {0} with score {1}", axis, num));
		return axis;
	}

	// Token: 0x060057ED RID: 22509 RVA: 0x001C8D38 File Offset: 0x001C6F38
	private static int EvaluateBestSplit(List<BSPTreeBuilder.BoxMetadata> boxes, global::BoundsInt bounds, SerializableBSPNode.Axis axis, int splitValue)
	{
		global::BoundsInt boundsInt = bounds;
		global::BoundsInt boundsInt2 = bounds;
		switch (axis)
		{
		case SerializableBSPNode.Axis.X:
			boundsInt.SetMinMax(bounds.min, new Vector3Int(splitValue, bounds.max.y, bounds.max.z));
			boundsInt2.SetMinMax(new Vector3Int(splitValue, bounds.min.y, bounds.min.z), bounds.max);
			break;
		case SerializableBSPNode.Axis.Y:
			boundsInt.SetMinMax(bounds.min, new Vector3Int(bounds.max.x, splitValue, bounds.max.z));
			boundsInt2.SetMinMax(new Vector3Int(bounds.min.x, splitValue, bounds.min.z), bounds.max);
			break;
		case SerializableBSPNode.Axis.Z:
			boundsInt.SetMinMax(bounds.min, new Vector3Int(bounds.max.x, bounds.max.y, splitValue));
			boundsInt2.SetMinMax(new Vector3Int(bounds.min.x, bounds.min.y, splitValue), bounds.max);
			break;
		}
		return BSPTreeBuilder.EvaluateSplit(boxes, splitValue, axis, bounds);
	}

	// Token: 0x060057EE RID: 22510 RVA: 0x001C8E74 File Offset: 0x001C7074
	private static int FindOptimalSplit(List<BSPTreeBuilder.BoxMetadata> boxes, global::BoundsInt bounds, SerializableBSPNode.Axis axis, out int bestScore)
	{
		List<int> list = new List<int>();
		foreach (BSPTreeBuilder.BoxMetadata boxMetadata in boxes)
		{
			global::BoundsInt bounds2 = boxMetadata.bounds;
			switch (axis)
			{
			case SerializableBSPNode.Axis.X:
				list.Add(bounds2.min.x);
				list.Add(bounds2.max.x);
				break;
			case SerializableBSPNode.Axis.Y:
				list.Add(bounds2.min.y);
				list.Add(bounds2.max.y);
				break;
			case SerializableBSPNode.Axis.Z:
				list.Add(bounds2.min.z);
				list.Add(bounds2.max.z);
				break;
			}
		}
		list = (from x in list.Distinct<int>()
		orderby x
		select x).ToList<int>();
		int num = BSPTreeBuilder.GetFallbackSplit(bounds, axis);
		bestScore = int.MaxValue;
		Debug.Log(string.Format("Evaluating {0} split candidates for {1} axis", list.Count, axis));
		int axisValue = BSPTreeBuilder.GetAxisValue(bounds.min, axis);
		int axisValue2 = BSPTreeBuilder.GetAxisValue(bounds.max, axis);
		foreach (int num2 in list)
		{
			if (num2 > axisValue && num2 < axisValue2)
			{
				int num3 = BSPTreeBuilder.EvaluateSplit(boxes, num2, axis, bounds);
				if (num3 < bestScore)
				{
					bestScore = num3;
					num = num2;
				}
			}
		}
		Debug.Log(string.Format("Best split: {0} with score {1}", num, bestScore));
		return num;
	}

	// Token: 0x060057EF RID: 22511 RVA: 0x001C9044 File Offset: 0x001C7244
	private static int GetFallbackSplit(global::BoundsInt bounds, SerializableBSPNode.Axis axis)
	{
		switch (axis)
		{
		case SerializableBSPNode.Axis.X:
			return bounds.center.x;
		case SerializableBSPNode.Axis.Y:
			return bounds.center.y;
		case SerializableBSPNode.Axis.Z:
			return bounds.center.z;
		default:
			return 0;
		}
	}

	// Token: 0x060057F0 RID: 22512 RVA: 0x001C9098 File Offset: 0x001C7298
	private static int EvaluateSplit(List<BSPTreeBuilder.BoxMetadata> boxes, int splitValue, SerializableBSPNode.Axis axis, global::BoundsInt bounds)
	{
		global::BoundsInt region = bounds;
		global::BoundsInt region2 = bounds;
		switch (axis)
		{
		case SerializableBSPNode.Axis.X:
			region.SetMinMax(bounds.min, new Vector3Int(splitValue - 1, bounds.max.y, bounds.max.z));
			region2.SetMinMax(new Vector3Int(splitValue + 1, bounds.min.y, bounds.min.z), bounds.max);
			break;
		case SerializableBSPNode.Axis.Y:
			region.SetMinMax(bounds.min, new Vector3Int(bounds.max.x, splitValue - 1, bounds.max.z));
			region2.SetMinMax(new Vector3Int(bounds.min.x, splitValue + 1, bounds.min.z), bounds.max);
			break;
		case SerializableBSPNode.Axis.Z:
			region.SetMinMax(bounds.min, new Vector3Int(bounds.max.x, bounds.max.y, splitValue - 1));
			region2.SetMinMax(new Vector3Int(bounds.min.x, bounds.min.y, splitValue + 1), bounds.max);
			break;
		}
		List<BSPTreeBuilder.BoxMetadata> effectiveBoxes = BSPTreeBuilder.GetEffectiveBoxes(boxes, region);
		List<BSPTreeBuilder.BoxMetadata> effectiveBoxes2 = BSPTreeBuilder.GetEffectiveBoxes(boxes, region2);
		int count = effectiveBoxes.Count;
		int count2 = effectiveBoxes2.Count;
		int count3 = boxes.Count;
		int num = count3 - count;
		int num2 = count3 - count2;
		Debug.Log(string.Format("  Split evaluation: {0} total -> L:{1} (eliminated:{2}), R:{3} (eliminated:{4})", new object[]
		{
			count3,
			count,
			num,
			count2,
			num2
		}));
		if (count == 0 || count2 == 0)
		{
			return 1000;
		}
		return -((num + 1) * (num2 + 1));
	}

	// Token: 0x060057F1 RID: 22513 RVA: 0x001C9264 File Offset: 0x001C7464
	private static List<BSPTreeBuilder.BoxMetadata> GetEffectiveBoxes(List<BSPTreeBuilder.BoxMetadata> boxes, global::BoundsInt region)
	{
		List<BSPTreeBuilder.BoxMetadata> list = new List<BSPTreeBuilder.BoxMetadata>();
		List<BSPTreeBuilder.BoxMetadata> list2 = new List<BSPTreeBuilder.BoxMetadata>();
		foreach (BSPTreeBuilder.BoxMetadata boxMetadata in boxes)
		{
			if (boxMetadata.bounds.Intersects(region))
			{
				list2.Add(boxMetadata);
			}
		}
		foreach (BSPTreeBuilder.BoxMetadata boxMetadata2 in list2)
		{
			bool flag = false;
			global::BoundsInt intersection = boxMetadata2.bounds.GetIntersection(region);
			foreach (BSPTreeBuilder.BoxMetadata boxMetadata3 in list2)
			{
				if (boxMetadata3 != boxMetadata2 && boxMetadata3.priority > boxMetadata2.priority && boxMetadata3.bounds.GetIntersection(region).Contains(intersection) && BoxColliderUtils.DoesBoxContainBox(boxMetadata3.box, boxMetadata2.box))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(boxMetadata2);
			}
		}
		return list;
	}

	// Token: 0x060057F2 RID: 22514 RVA: 0x001C93B8 File Offset: 0x001C75B8
	private static List<BSPTreeBuilder.BoxMetadata> GetEffectiveSpanningBoxes(List<BSPTreeBuilder.BoxMetadata> boxes, global::BoundsInt leftBounds, global::BoundsInt rightBounds)
	{
		List<BSPTreeBuilder.BoxMetadata> list = new List<BSPTreeBuilder.BoxMetadata>();
		Dictionary<BSPTreeBuilder.BoxMetadata, global::BoundsInt> dictionary = new Dictionary<BSPTreeBuilder.BoxMetadata, global::BoundsInt>();
		foreach (BSPTreeBuilder.BoxMetadata boxMetadata in boxes)
		{
			dictionary[boxMetadata] = boxMetadata.bounds;
		}
		foreach (BSPTreeBuilder.BoxMetadata boxMetadata2 in boxes)
		{
			global::BoundsInt boundsInt = dictionary[boxMetadata2];
			if (boundsInt.Intersects(leftBounds) && boundsInt.Intersects(rightBounds))
			{
				list.Add(boxMetadata2);
			}
		}
		return list;
	}

	// Token: 0x060057F3 RID: 22515 RVA: 0x001C9478 File Offset: 0x001C7678
	private static SerializableBSPNode.Axis GetNextAxis(SerializableBSPNode.Axis currentAxis)
	{
		switch (currentAxis)
		{
		case SerializableBSPNode.Axis.X:
			return SerializableBSPNode.Axis.Y;
		case SerializableBSPNode.Axis.Y:
			return SerializableBSPNode.Axis.Z;
		case SerializableBSPNode.Axis.Z:
			return SerializableBSPNode.Axis.X;
		default:
			return SerializableBSPNode.Axis.X;
		}
	}

	// Token: 0x060057F4 RID: 22516 RVA: 0x001C9495 File Offset: 0x001C7695
	private static int GetAxisValue(Vector3Int point, SerializableBSPNode.Axis axis)
	{
		switch (axis)
		{
		case SerializableBSPNode.Axis.X:
			return point.x;
		case SerializableBSPNode.Axis.Y:
			return point.y;
		case SerializableBSPNode.Axis.Z:
			return point.z;
		default:
			return 0;
		}
	}

	// Token: 0x060057F5 RID: 22517 RVA: 0x001C94C4 File Offset: 0x001C76C4
	private static global::BoundsInt CalculateWorldBounds(List<BSPTreeBuilder.BoxMetadata> boxes)
	{
		if (boxes.Count == 0)
		{
			return default(global::BoundsInt);
		}
		global::BoundsInt bounds = boxes[0].bounds;
		for (int i = 1; i < boxes.Count; i++)
		{
			bounds.Encapsulate(boxes[i].bounds);
		}
		return bounds;
	}

	// Token: 0x060057F6 RID: 22518 RVA: 0x001C9518 File Offset: 0x001C7718
	private static float CalculateIntersectionVolume(global::BoundsInt box, global::BoundsInt region)
	{
		if (!box.Intersects(region))
		{
			return 0f;
		}
		return box.GetIntersection(region).VolumeFloat();
	}

	// Token: 0x060057F7 RID: 22519 RVA: 0x001C9548 File Offset: 0x001C7748
	private static int CreateMatrixNodeTree(ZoneDef[] zones, List<BSPTreeBuilder.BoxMetadata> boxes, List<MatrixBSPNode> matrixNodeList, global::BoundsInt bounds, [TupleElementNames(new string[]
	{
		"matrixIndex",
		"outsideIndex"
	})] Dictionary<ValueTuple<int, int>, int> matrixNodeCache, ref int matrixNodeCacheHits)
	{
		if (boxes.Count == 0)
		{
			Debug.LogWarning("Cannot create matrix node tree with no boxes - returning zone 0");
			return 0;
		}
		List<BSPTreeBuilder.BoxMetadata> list = new List<BSPTreeBuilder.BoxMetadata>();
		List<BSPTreeBuilder.BoxMetadata> list2 = new List<BSPTreeBuilder.BoxMetadata>();
		foreach (BSPTreeBuilder.BoxMetadata boxMetadata in boxes)
		{
			if (boxMetadata.bounds.GetIntersection(bounds) == bounds && BoxColliderUtils.DoesBoxContainRegion(boxMetadata.box, bounds))
			{
				list.Add(boxMetadata);
			}
			else
			{
				list2.Add(boxMetadata);
			}
		}
		if (list.Count <= 0)
		{
			List<BSPTreeBuilder.BoxMetadata> boxes2 = BSPTreeBuilder.SortBoxesByPriority(boxes);
			return BSPTreeBuilder.CreateSequentialMatrixNodes(zones, boxes2, matrixNodeList, 0, zones, matrixNodeCache, ref matrixNodeCacheHits);
		}
		ZoneDef zone = list[0].zone;
		int priority = list[0].priority;
		foreach (BSPTreeBuilder.BoxMetadata boxMetadata2 in list)
		{
			if (boxMetadata2.priority > priority)
			{
				priority = boxMetadata2.priority;
				zone = boxMetadata2.zone;
			}
		}
		for (int i = list2.Count - 1; i >= 0; i--)
		{
			if (list2[i].priority < priority)
			{
				list2.RemoveAt(i);
			}
		}
		if (list2.Count == 0)
		{
			MatrixBSPNode matrixNode = default(MatrixBSPNode);
			matrixNode.matrixIndex = -1;
			int outsideChildIndex = Array.IndexOf<ZoneDef>(zones, zone);
			matrixNode.outsideChildIndex = outsideChildIndex;
			return BSPTreeBuilder.AddMatrixNodeWithCache(matrixNode, matrixNodeList, matrixNodeCache, ref matrixNodeCacheHits);
		}
		List<BSPTreeBuilder.BoxMetadata> list3 = BSPTreeBuilder.SortBoxesByPriority(list2);
		foreach (BSPTreeBuilder.BoxMetadata boxMetadata3 in list)
		{
			if (boxMetadata3.zone == zone)
			{
				list3.Add(boxMetadata3);
				break;
			}
		}
		return BSPTreeBuilder.CreateSequentialMatrixNodes(zones, list3, matrixNodeList, 0, zones, matrixNodeCache, ref matrixNodeCacheHits);
	}

	// Token: 0x060057F8 RID: 22520 RVA: 0x001C9750 File Offset: 0x001C7950
	private static int CreateSequentialMatrixNodes(ZoneDef[] zones, List<BSPTreeBuilder.BoxMetadata> boxes, List<MatrixBSPNode> matrixNodeList, int boxIndex, ZoneDef[] allZones, [TupleElementNames(new string[]
	{
		"matrixIndex",
		"outsideIndex"
	})] Dictionary<ValueTuple<int, int>, int> matrixNodeCache, ref int matrixNodeCacheHits)
	{
		if (boxIndex == 0 && boxes.Count > 1)
		{
			while (boxes.Count > 1 && boxes[boxes.Count - 1].zone == boxes[boxes.Count - 2].zone)
			{
				boxes.RemoveAt(boxes.Count - 1);
			}
		}
		if (boxIndex >= boxes.Count)
		{
			return 0;
		}
		BSPTreeBuilder.BoxMetadata boxMetadata = boxes[boxIndex];
		if (boxIndex == boxes.Count - 1)
		{
			MatrixBSPNode matrixNode = default(MatrixBSPNode);
			matrixNode.matrixIndex = -1;
			int outsideChildIndex = Array.IndexOf<ZoneDef>(allZones, boxMetadata.zone);
			matrixNode.outsideChildIndex = outsideChildIndex;
			return BSPTreeBuilder.AddMatrixNodeWithCache(matrixNode, matrixNodeList, matrixNodeCache, ref matrixNodeCacheHits);
		}
		MatrixBSPNode matrixNode2 = default(MatrixBSPNode);
		matrixNode2.matrixIndex = boxMetadata.matrixIndex;
		int outsideChildIndex2 = BSPTreeBuilder.CreateSequentialMatrixNodes(zones, boxes, matrixNodeList, boxIndex + 1, allZones, matrixNodeCache, ref matrixNodeCacheHits);
		matrixNode2.outsideChildIndex = outsideChildIndex2;
		return BSPTreeBuilder.AddMatrixNodeWithCache(matrixNode2, matrixNodeList, matrixNodeCache, ref matrixNodeCacheHits);
	}

	// Token: 0x060057F9 RID: 22521 RVA: 0x001C983C File Offset: 0x001C7A3C
	private static int AddMatrixNodeWithCache(MatrixBSPNode matrixNode, List<MatrixBSPNode> matrixNodeList, [TupleElementNames(new string[]
	{
		"matrixIndex",
		"outsideIndex"
	})] Dictionary<ValueTuple<int, int>, int> matrixNodeCache, ref int matrixNodeCacheHits)
	{
		ValueTuple<int, int> key = new ValueTuple<int, int>(matrixNode.matrixIndex, matrixNode.outsideChildIndex);
		int num;
		if (matrixNodeCache.TryGetValue(key, out num))
		{
			matrixNodeCacheHits++;
			return -num;
		}
		int count = matrixNodeList.Count;
		matrixNodeList.Add(matrixNode);
		matrixNodeCache[key] = count;
		return -count;
	}

	// Token: 0x060057FA RID: 22522 RVA: 0x001C9888 File Offset: 0x001C7A88
	private static List<BSPTreeBuilder.BoxMetadata> SortBoxesByPriority(List<BSPTreeBuilder.BoxMetadata> boxes)
	{
		List<BSPTreeBuilder.BoxMetadata> list = new List<BSPTreeBuilder.BoxMetadata>(boxes);
		list.Sort((BSPTreeBuilder.BoxMetadata a, BSPTreeBuilder.BoxMetadata b) => b.priority.CompareTo(a.priority));
		return list;
	}

	// Token: 0x060057FB RID: 22523 RVA: 0x001C98B8 File Offset: 0x001C7AB8
	private static void CleanupUnreferencedMatrices(List<MatrixBSPNode> matrixNodeList, List<MatrixZonePair> matricesList)
	{
		HashSet<int> hashSet = new HashSet<int>();
		foreach (MatrixBSPNode matrixBSPNode in matrixNodeList)
		{
			if (matrixBSPNode.matrixIndex >= 0)
			{
				hashSet.Add(matrixBSPNode.matrixIndex);
			}
		}
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		List<MatrixZonePair> list = new List<MatrixZonePair>();
		for (int i = 0; i < matricesList.Count; i++)
		{
			if (hashSet.Contains(i))
			{
				dictionary[i] = list.Count;
				list.Add(matricesList[i]);
			}
		}
		for (int j = 0; j < matrixNodeList.Count; j++)
		{
			MatrixBSPNode matrixBSPNode2 = matrixNodeList[j];
			int matrixIndex;
			if (dictionary.TryGetValue(matrixBSPNode2.matrixIndex, out matrixIndex))
			{
				matrixBSPNode2.matrixIndex = matrixIndex;
				matrixNodeList[j] = matrixBSPNode2;
			}
		}
		int count = matricesList.Count;
		matricesList.Clear();
		matricesList.AddRange(list);
		int num = count - list.Count;
		if (num > 0)
		{
			Debug.Log(string.Format("Cleaned up {0} unreferenced matrices. Matrices reduced from {1} to {2}", num, count, list.Count));
		}
	}

	// Token: 0x0400689C RID: 26780
	private const int MAX_ZONES_PER_LEAF = 10;

	// Token: 0x0400689D RID: 26781
	private const int MAX_DEPTH = 15;

	// Token: 0x0400689E RID: 26782
	private const int MAX_NODES = 650;

	// Token: 0x0400689F RID: 26783
	private static Vector3 testPoint = new Vector3(60f, 49f, -98f);

	// Token: 0x02000E18 RID: 3608
	public class BoxMetadata
	{
		// Token: 0x060057FD RID: 22525 RVA: 0x001C9A0F File Offset: 0x001C7C0F
		public BoxMetadata(BoxCollider boxCollider, ZoneDef zoneData, int matrixIdx, int priority)
		{
			this.box = boxCollider;
			this.zone = zoneData;
			this.matrixIndex = matrixIdx;
			this.bounds = global::BoundsInt.FromBounds(boxCollider.bounds);
			this.priority = priority;
		}

		// Token: 0x060057FE RID: 22526 RVA: 0x001C9A48 File Offset: 0x001C7C48
		public bool ContainsPoint(Vector3 worldPoint)
		{
			Vector3 vector = this.box.transform.InverseTransformPoint(worldPoint);
			Vector3 size = this.box.size;
			Vector3 center = this.box.center;
			Vector3 vector2 = center - size * 0.5f;
			Vector3 vector3 = center + size * 0.5f;
			return vector.x >= vector2.x && vector.x <= vector3.x && vector.y >= vector2.y && vector.y <= vector3.y && vector.z >= vector2.z && vector.z <= vector3.z;
		}

		// Token: 0x060057FF RID: 22527 RVA: 0x001C9AFA File Offset: 0x001C7CFA
		public global::BoundsInt GetWorldBounds()
		{
			return this.bounds;
		}

		// Token: 0x040068A0 RID: 26784
		public BoxCollider box;

		// Token: 0x040068A1 RID: 26785
		public ZoneDef zone;

		// Token: 0x040068A2 RID: 26786
		public int matrixIndex;

		// Token: 0x040068A3 RID: 26787
		public int priority;

		// Token: 0x040068A4 RID: 26788
		public readonly global::BoundsInt bounds;
	}
}
