using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameObjectScheduling
{
	// Token: 0x0200132F RID: 4911
	public class GameObjectSchedulerEventDispatcher : MonoBehaviour
	{
		// Token: 0x17000BBE RID: 3006
		// (get) Token: 0x06007BAB RID: 31659 RVA: 0x0028584E File Offset: 0x00283A4E
		public UnityEvent OnScheduledActivation
		{
			get
			{
				return this.onScheduledActivation;
			}
		}

		// Token: 0x17000BBF RID: 3007
		// (get) Token: 0x06007BAC RID: 31660 RVA: 0x00285856 File Offset: 0x00283A56
		public UnityEvent OnScheduledDeactivation
		{
			get
			{
				return this.onScheduledDeactivation;
			}
		}

		// Token: 0x04008CFC RID: 36092
		[SerializeField]
		private UnityEvent onScheduledActivation;

		// Token: 0x04008CFD RID: 36093
		[SerializeField]
		private UnityEvent onScheduledDeactivation;
	}
}
