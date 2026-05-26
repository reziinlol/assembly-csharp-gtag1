using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200057F RID: 1407
[Serializable]
public class FlagEvents<T> where T : Enum
{
	// Token: 0x060023A6 RID: 9126 RVA: 0x000BFEB8 File Offset: 0x000BE0B8
	public void InvokeAll(T test, bool isLocal = false)
	{
		int num = Convert.ToInt32(test);
		for (int i = 0; i < this.list.Length; i++)
		{
			if ((num & this.list[i].flagsAsInt) != 0 && (!this.list[i].runOnlyLocally || isLocal))
			{
				UnityEvent anyFlagTrue = this.list[i].anyFlagTrue;
				if (anyFlagTrue != null)
				{
					anyFlagTrue.Invoke();
				}
			}
		}
	}

	// Token: 0x04002EC8 RID: 11976
	[SerializeField]
	private FlagEvents<T>.FlagEvent[] list;

	// Token: 0x02000580 RID: 1408
	[Serializable]
	private class FlagEvent : ISerializationCallbackReceiver
	{
		// Token: 0x170003BF RID: 959
		// (get) Token: 0x060023A8 RID: 9128 RVA: 0x000BFF21 File Offset: 0x000BE121
		private string FlagsLabel
		{
			get
			{
				return typeof(T).Name;
			}
		}

		// Token: 0x060023A9 RID: 9129 RVA: 0x000BFF32 File Offset: 0x000BE132
		public void OnBeforeSerialize()
		{
			this.flagsAsInt = Convert.ToInt32(this.flags);
		}

		// Token: 0x060023AA RID: 9130 RVA: 0x000BFF4A File Offset: 0x000BE14A
		public void OnAfterDeserialize()
		{
			this.flags = (T)((object)this.flagsAsInt);
		}

		// Token: 0x04002EC9 RID: 11977
		public string debugName = "Any flag true";

		// Token: 0x04002ECA RID: 11978
		[Tooltip("Check this box if only the local player is supposed to run this event.")]
		public bool runOnlyLocally;

		// Token: 0x04002ECB RID: 11979
		private T flags;

		// Token: 0x04002ECC RID: 11980
		[HideInInspector]
		public int flagsAsInt;

		// Token: 0x04002ECD RID: 11981
		public UnityEvent anyFlagTrue;
	}
}
