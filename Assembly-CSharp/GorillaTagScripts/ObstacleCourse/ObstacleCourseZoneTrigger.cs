using System;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000F7F RID: 3967
	public class ObstacleCourseZoneTrigger : MonoBehaviour
	{
		// Token: 0x140000A4 RID: 164
		// (add) Token: 0x06006315 RID: 25365 RVA: 0x001FE734 File Offset: 0x001FC934
		// (remove) Token: 0x06006316 RID: 25366 RVA: 0x001FE76C File Offset: 0x001FC96C
		public event ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerEnter;

		// Token: 0x140000A5 RID: 165
		// (add) Token: 0x06006317 RID: 25367 RVA: 0x001FE7A4 File Offset: 0x001FC9A4
		// (remove) Token: 0x06006318 RID: 25368 RVA: 0x001FE7DC File Offset: 0x001FC9DC
		public event ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerExit;

		// Token: 0x06006319 RID: 25369 RVA: 0x001FE811 File Offset: 0x001FCA11
		private void OnTriggerEnter(Collider other)
		{
			if (!other.GetComponent<SphereCollider>())
			{
				return;
			}
			if (other.attachedRigidbody.gameObject.CompareTag("GorillaPlayer"))
			{
				ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent onPlayerTriggerEnter = this.OnPlayerTriggerEnter;
				if (onPlayerTriggerEnter == null)
				{
					return;
				}
				onPlayerTriggerEnter(other);
			}
		}

		// Token: 0x0600631A RID: 25370 RVA: 0x001FE849 File Offset: 0x001FCA49
		private void OnTriggerExit(Collider other)
		{
			if (!other.GetComponent<SphereCollider>())
			{
				return;
			}
			if (other.attachedRigidbody.gameObject.CompareTag("GorillaPlayer"))
			{
				ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent onPlayerTriggerExit = this.OnPlayerTriggerExit;
				if (onPlayerTriggerExit == null)
				{
					return;
				}
				onPlayerTriggerExit(other);
			}
		}

		// Token: 0x040071C3 RID: 29123
		public LayerMask bodyLayer;

		// Token: 0x02000F80 RID: 3968
		// (Invoke) Token: 0x0600631D RID: 25373
		public delegate void ObstacleCourseTriggerEvent(Collider collider);
	}
}
