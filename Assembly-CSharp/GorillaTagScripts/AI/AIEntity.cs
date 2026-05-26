using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.AI;

namespace GorillaTagScripts.AI
{
	// Token: 0x02000FE9 RID: 4073
	public class AIEntity : MonoBehaviour
	{
		// Token: 0x060065E3 RID: 26083 RVA: 0x0020DB44 File Offset: 0x0020BD44
		protected void Awake()
		{
			this.navMeshAgent = base.gameObject.GetComponent<NavMeshAgent>();
			this.animator = base.gameObject.GetComponent<Animator>();
			if (this.waypointsContainer != null)
			{
				foreach (Transform item in this.waypointsContainer.GetComponentsInChildren<Transform>())
				{
					this.waypoints.Add(item);
				}
			}
		}

		// Token: 0x060065E4 RID: 26084 RVA: 0x0020DBAC File Offset: 0x0020BDAC
		protected void ChooseRandomTarget()
		{
			int randomTarget = Random.Range(0, VRRigCache.ActiveRigs.Count);
			int num = VRRigCache.ActiveRigContainers.FindIndex((RigContainer x) => x.Rig.creator != null && x.Rig.creator == VRRigCache.ActiveRigContainers[randomTarget].Rig.creator);
			if (num == -1)
			{
				num = Random.Range(0, VRRigCache.ActiveRigs.Count);
			}
			if (num < VRRigCache.ActiveRigContainers.Count)
			{
				this.targetPlayer = VRRigCache.ActiveRigContainers[num].Rig.creator;
				this.followTarget = VRRigCache.ActiveRigContainers[num].Rig.head.rigTarget;
				NavMeshHit navMeshHit;
				this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, out navMeshHit, this.navMeshSampleRange, 1);
				return;
			}
			this.targetPlayer = null;
			this.followTarget = null;
		}

		// Token: 0x060065E5 RID: 26085 RVA: 0x0020DC7C File Offset: 0x0020BE7C
		protected void ChooseClosestTarget()
		{
			VRRig vrrig = null;
			float num = float.MaxValue;
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				VRRig rig = rigContainer.Rig;
				if (rig.head != null && !rig.head.rigTarget.IsNull())
				{
					float sqrMagnitude = (base.transform.position - rig.head.rigTarget.transform.position).sqrMagnitude;
					if (sqrMagnitude < this.minChaseRange * this.minChaseRange && sqrMagnitude < num)
					{
						num = sqrMagnitude;
						vrrig = rig;
					}
				}
			}
			if (vrrig.IsNotNull())
			{
				this.targetPlayer = vrrig.creator;
				this.followTarget = vrrig.head.rigTarget;
				NavMeshHit navMeshHit;
				this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, out navMeshHit, this.navMeshSampleRange, 1);
				return;
			}
			this.targetPlayer = null;
			this.followTarget = null;
		}

		// Token: 0x04007544 RID: 30020
		public GameObject waypointsContainer;

		// Token: 0x04007545 RID: 30021
		public Transform circleCenter;

		// Token: 0x04007546 RID: 30022
		public float circleRadius;

		// Token: 0x04007547 RID: 30023
		public float angularSpeed;

		// Token: 0x04007548 RID: 30024
		public float patrolSpeed;

		// Token: 0x04007549 RID: 30025
		public float fleeSpeed;

		// Token: 0x0400754A RID: 30026
		public NavMeshAgent navMeshAgent;

		// Token: 0x0400754B RID: 30027
		public Animator animator;

		// Token: 0x0400754C RID: 30028
		public float fleeRang;

		// Token: 0x0400754D RID: 30029
		public float fleeSpeedMult;

		// Token: 0x0400754E RID: 30030
		public float minChaseRange;

		// Token: 0x0400754F RID: 30031
		public float attackDistance;

		// Token: 0x04007550 RID: 30032
		public float navMeshSampleRange = 5f;

		// Token: 0x04007551 RID: 30033
		internal readonly List<Transform> waypoints = new List<Transform>();

		// Token: 0x04007552 RID: 30034
		internal float defaultSpeed;

		// Token: 0x04007553 RID: 30035
		public Transform followTarget;

		// Token: 0x04007554 RID: 30036
		public NetPlayer targetPlayer;

		// Token: 0x04007555 RID: 30037
		public bool targetIsOnNavMesh;
	}
}
