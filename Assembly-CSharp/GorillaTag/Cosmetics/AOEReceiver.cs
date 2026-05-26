using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001228 RID: 4648
	public class AOEReceiver : MonoBehaviour
	{
		// Token: 0x0600744E RID: 29774 RVA: 0x00261273 File Offset: 0x0025F473
		public void ReceiveAOE(in AOEReceiver.AOEContext AOEContext)
		{
			if (!this.enabledForAOE)
			{
				return;
			}
			AOEContextEvent onAOEReceived = this.OnAOEReceived;
			if (onAOEReceived == null)
			{
				return;
			}
			onAOEReceived.Invoke(AOEContext);
		}

		// Token: 0x0400857E RID: 34174
		public AOEContextEvent OnAOEReceived;

		// Token: 0x0400857F RID: 34175
		[Tooltip("Quick toggle to disable receiving without disabling the GameObject.")]
		[SerializeField]
		private bool enabledForAOE = true;

		// Token: 0x02001229 RID: 4649
		[Serializable]
		public struct AOEContext
		{
			// Token: 0x04008580 RID: 34176
			public Vector3 origin;

			// Token: 0x04008581 RID: 34177
			public float radius;

			// Token: 0x04008582 RID: 34178
			public GameObject instigator;

			// Token: 0x04008583 RID: 34179
			public float baseStrength;

			// Token: 0x04008584 RID: 34180
			public float finalStrength;

			// Token: 0x04008585 RID: 34181
			public float distance;

			// Token: 0x04008586 RID: 34182
			public float normalizedDistance;
		}
	}
}
