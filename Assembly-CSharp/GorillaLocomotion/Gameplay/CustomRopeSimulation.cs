using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020010F7 RID: 4343
	public class CustomRopeSimulation : MonoBehaviour
	{
		// Token: 0x06006D62 RID: 28002 RVA: 0x0023BDA4 File Offset: 0x00239FA4
		private void Start()
		{
			Vector3 position = base.transform.position;
			for (int i = 0; i < this.nodeCount; i++)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.ropeNodePrefab);
				gameObject.transform.parent = base.transform;
				gameObject.transform.position = position;
				this.nodes.Add(gameObject.transform);
				position.y -= this.nodeDistance;
			}
			this.nodes[this.nodes.Count - 1].GetComponentInChildren<Renderer>().enabled = false;
			this.burstNodes = new NativeArray<BurstRopeNode>(this.nodes.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		}

		// Token: 0x06006D63 RID: 28003 RVA: 0x0023BE54 File Offset: 0x0023A054
		private void OnDestroy()
		{
			this.burstNodes.Dispose();
		}

		// Token: 0x06006D64 RID: 28004 RVA: 0x0023BE64 File Offset: 0x0023A064
		private void Update()
		{
			new SolveRopeJob
			{
				fixedDeltaTime = Time.deltaTime,
				gravity = this.gravity,
				nodes = this.burstNodes,
				nodeDistance = this.nodeDistance,
				rootPos = base.transform.position
			}.Run<SolveRopeJob>();
			for (int i = 0; i < this.burstNodes.Length; i++)
			{
				this.nodes[i].position = this.burstNodes[i].curPos;
				if (i > 0)
				{
					Vector3 a = this.burstNodes[i - 1].curPos - this.burstNodes[i].curPos;
					this.nodes[i].up = -a;
				}
			}
		}

		// Token: 0x04007E60 RID: 32352
		private List<Transform> nodes = new List<Transform>();

		// Token: 0x04007E61 RID: 32353
		[SerializeField]
		private GameObject ropeNodePrefab;

		// Token: 0x04007E62 RID: 32354
		[SerializeField]
		private int nodeCount = 10;

		// Token: 0x04007E63 RID: 32355
		[SerializeField]
		private float nodeDistance = 0.4f;

		// Token: 0x04007E64 RID: 32356
		[SerializeField]
		private Vector3 gravity = new Vector3(0f, -9.81f, 0f);

		// Token: 0x04007E65 RID: 32357
		private NativeArray<BurstRopeNode> burstNodes;
	}
}
