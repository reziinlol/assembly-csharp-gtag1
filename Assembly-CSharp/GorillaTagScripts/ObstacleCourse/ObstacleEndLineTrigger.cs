using System;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000F81 RID: 3969
	public class ObstacleEndLineTrigger : MonoBehaviour
	{
		// Token: 0x140000A6 RID: 166
		// (add) Token: 0x06006320 RID: 25376 RVA: 0x001FE884 File Offset: 0x001FCA84
		// (remove) Token: 0x06006321 RID: 25377 RVA: 0x001FE8BC File Offset: 0x001FCABC
		public event ObstacleEndLineTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerEnter;

		// Token: 0x06006322 RID: 25378 RVA: 0x001FE8F4 File Offset: 0x001FCAF4
		private void OnTriggerEnter(Collider other)
		{
			VRRig vrrig;
			if (other.attachedRigidbody.gameObject.TryGetComponent<VRRig>(out vrrig))
			{
				ObstacleEndLineTrigger.ObstacleCourseTriggerEvent onPlayerTriggerEnter = this.OnPlayerTriggerEnter;
				if (onPlayerTriggerEnter == null)
				{
					return;
				}
				onPlayerTriggerEnter(vrrig);
			}
		}

		// Token: 0x02000F82 RID: 3970
		// (Invoke) Token: 0x06006325 RID: 25381
		public delegate void ObstacleCourseTriggerEvent(VRRig vrrig);
	}
}
