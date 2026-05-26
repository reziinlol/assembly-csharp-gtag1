using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viveport
{
	// Token: 0x02000E26 RID: 3622
	public class MainThreadDispatcher : MonoBehaviour
	{
		// Token: 0x06005831 RID: 22577 RVA: 0x001CA43D File Offset: 0x001C863D
		private void Awake()
		{
			if (MainThreadDispatcher.instance == null)
			{
				MainThreadDispatcher.instance = this;
				Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		// Token: 0x06005832 RID: 22578 RVA: 0x001CA460 File Offset: 0x001C8660
		public void Update()
		{
			Queue<Action> obj = MainThreadDispatcher.actions;
			lock (obj)
			{
				while (MainThreadDispatcher.actions.Count > 0)
				{
					MainThreadDispatcher.actions.Dequeue()();
				}
			}
		}

		// Token: 0x06005833 RID: 22579 RVA: 0x001CA4B8 File Offset: 0x001C86B8
		public static MainThreadDispatcher Instance()
		{
			if (MainThreadDispatcher.instance == null)
			{
				throw new Exception("Could not find the MainThreadDispatcher GameObject. Please ensure you have added this script to an empty GameObject in your scene.");
			}
			return MainThreadDispatcher.instance;
		}

		// Token: 0x06005834 RID: 22580 RVA: 0x001CA4D7 File Offset: 0x001C86D7
		private void OnDestroy()
		{
			MainThreadDispatcher.instance = null;
		}

		// Token: 0x06005835 RID: 22581 RVA: 0x001CA4E0 File Offset: 0x001C86E0
		public void Enqueue(IEnumerator action)
		{
			Queue<Action> obj = MainThreadDispatcher.actions;
			lock (obj)
			{
				MainThreadDispatcher.actions.Enqueue(delegate
				{
					this.StartCoroutine(action);
				});
			}
		}

		// Token: 0x06005836 RID: 22582 RVA: 0x001CA544 File Offset: 0x001C8744
		public void Enqueue(Action action)
		{
			this.Enqueue(this.ActionWrapper(action));
		}

		// Token: 0x06005837 RID: 22583 RVA: 0x001CA553 File Offset: 0x001C8753
		public void Enqueue<T1>(Action<T1> action, T1 param1)
		{
			this.Enqueue(this.ActionWrapper<T1>(action, param1));
		}

		// Token: 0x06005838 RID: 22584 RVA: 0x001CA563 File Offset: 0x001C8763
		public void Enqueue<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
		{
			this.Enqueue(this.ActionWrapper<T1, T2>(action, param1, param2));
		}

		// Token: 0x06005839 RID: 22585 RVA: 0x001CA574 File Offset: 0x001C8774
		public void Enqueue<T1, T2, T3>(Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
		{
			this.Enqueue(this.ActionWrapper<T1, T2, T3>(action, param1, param2, param3));
		}

		// Token: 0x0600583A RID: 22586 RVA: 0x001CA587 File Offset: 0x001C8787
		public void Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			this.Enqueue(this.ActionWrapper<T1, T2, T3, T4>(action, param1, param2, param3, param4));
		}

		// Token: 0x0600583B RID: 22587 RVA: 0x001CA59C File Offset: 0x001C879C
		private IEnumerator ActionWrapper(Action action)
		{
			action();
			yield return null;
			yield break;
		}

		// Token: 0x0600583C RID: 22588 RVA: 0x001CA5AB File Offset: 0x001C87AB
		private IEnumerator ActionWrapper<T1>(Action<T1> action, T1 param1)
		{
			action(param1);
			yield return null;
			yield break;
		}

		// Token: 0x0600583D RID: 22589 RVA: 0x001CA5C1 File Offset: 0x001C87C1
		private IEnumerator ActionWrapper<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
		{
			action(param1, param2);
			yield return null;
			yield break;
		}

		// Token: 0x0600583E RID: 22590 RVA: 0x001CA5DE File Offset: 0x001C87DE
		private IEnumerator ActionWrapper<T1, T2, T3>(Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
		{
			action(param1, param2, param3);
			yield return null;
			yield break;
		}

		// Token: 0x0600583F RID: 22591 RVA: 0x001CA603 File Offset: 0x001C8803
		private IEnumerator ActionWrapper<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			action(param1, param2, param3, param4);
			yield return null;
			yield break;
		}

		// Token: 0x040068CC RID: 26828
		private static readonly Queue<Action> actions = new Queue<Action>();

		// Token: 0x040068CD RID: 26829
		private static MainThreadDispatcher instance = null;
	}
}
