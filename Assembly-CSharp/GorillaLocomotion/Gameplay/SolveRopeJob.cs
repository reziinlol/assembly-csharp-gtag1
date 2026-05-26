using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020010F9 RID: 4345
	[BurstCompile]
	public struct SolveRopeJob : IJob
	{
		// Token: 0x06006D66 RID: 28006 RVA: 0x0023BF84 File Offset: 0x0023A184
		public void Execute()
		{
			this.Simulate();
			for (int i = 0; i < 20; i++)
			{
				this.ApplyConstraint();
			}
		}

		// Token: 0x06006D67 RID: 28007 RVA: 0x0023BFAC File Offset: 0x0023A1AC
		private void Simulate()
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				BurstRopeNode burstRopeNode = this.nodes[i];
				Vector3 b = burstRopeNode.curPos - burstRopeNode.lastPos;
				burstRopeNode.lastPos = burstRopeNode.curPos;
				Vector3 vector = burstRopeNode.curPos + b;
				vector += this.gravity * this.fixedDeltaTime;
				burstRopeNode.curPos = vector;
				this.nodes[i] = burstRopeNode;
			}
		}

		// Token: 0x06006D68 RID: 28008 RVA: 0x0023C038 File Offset: 0x0023A238
		private void ApplyConstraint()
		{
			BurstRopeNode value = this.nodes[0];
			value.curPos = this.rootPos;
			this.nodes[0] = value;
			for (int i = 0; i < this.nodes.Length - 1; i++)
			{
				BurstRopeNode burstRopeNode = this.nodes[i];
				BurstRopeNode burstRopeNode2 = this.nodes[i + 1];
				float magnitude = (burstRopeNode.curPos - burstRopeNode2.curPos).magnitude;
				float d = Mathf.Abs(magnitude - this.nodeDistance);
				Vector3 a = Vector3.zero;
				if (magnitude > this.nodeDistance)
				{
					a = (burstRopeNode.curPos - burstRopeNode2.curPos).normalized;
				}
				else if (magnitude < this.nodeDistance)
				{
					a = (burstRopeNode2.curPos - burstRopeNode.curPos).normalized;
				}
				Vector3 a2 = a * d;
				burstRopeNode.curPos -= a2 * 0.5f;
				burstRopeNode2.curPos += a2 * 0.5f;
				this.nodes[i] = burstRopeNode;
				this.nodes[i + 1] = burstRopeNode2;
			}
		}

		// Token: 0x04007E68 RID: 32360
		[ReadOnly]
		public float fixedDeltaTime;

		// Token: 0x04007E69 RID: 32361
		[WriteOnly]
		public NativeArray<BurstRopeNode> nodes;

		// Token: 0x04007E6A RID: 32362
		[ReadOnly]
		public Vector3 gravity;

		// Token: 0x04007E6B RID: 32363
		[ReadOnly]
		public Vector3 rootPos;

		// Token: 0x04007E6C RID: 32364
		[ReadOnly]
		public float nodeDistance;
	}
}
