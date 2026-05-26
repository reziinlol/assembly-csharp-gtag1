using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012A2 RID: 4770
	[RequireComponent(typeof(NetworkedRandomProvider))]
	public class RandomWeightedOutput : MonoBehaviour
	{
		// Token: 0x06007769 RID: 30569 RVA: 0x002727B3 File Offset: 0x002709B3
		private void Awake()
		{
			if (this.networkProvider == null)
			{
				this.networkProvider = base.GetComponentInParent<NetworkedRandomProvider>();
			}
		}

		// Token: 0x0600776A RID: 30570 RVA: 0x002727D0 File Offset: 0x002709D0
		public void PickNextRandom()
		{
			int deterministicPickIndex = this.GetDeterministicPickIndex();
			if (deterministicPickIndex >= 0)
			{
				UnityEvent onPick = this.outputs[deterministicPickIndex].onPick;
				if (onPick != null)
				{
					onPick.Invoke();
				}
				UnityEvent<int> unityEvent = this.onAnyPick;
				if (unityEvent != null)
				{
					unityEvent.Invoke(deterministicPickIndex);
				}
				if (this.debugLog)
				{
					Debug.Log(string.Format("[RandomWeightedOutput] Picked '{0}' (idx={1})", this.outputs[deterministicPickIndex].name, deterministicPickIndex));
				}
			}
		}

		// Token: 0x0600776B RID: 30571 RVA: 0x00272844 File Offset: 0x00270A44
		private int GetDeterministicPickIndex()
		{
			if (this.networkProvider == null)
			{
				return -1;
			}
			List<int> list = new List<int>(this.outputs.Count);
			for (int i = 0; i < this.outputs.Count; i++)
			{
				RandomWeightedOutput.WeightedOutput weightedOutput = this.outputs[i];
				if (weightedOutput != null && weightedOutput.enabled && weightedOutput.weight > 0f)
				{
					list.Add(i);
				}
			}
			if (list.Count == 0)
			{
				return -1;
			}
			double num = 0.0;
			foreach (int index in list)
			{
				num += (double)this.outputs[index].weight;
			}
			if (num <= 0.0)
			{
				return list[0];
			}
			double num2 = (double)this.networkProvider.GetSelectedAsFloat() * num;
			double num3 = 0.0;
			for (int j = 0; j < list.Count; j++)
			{
				int num4 = list[j];
				num3 += (double)this.outputs[num4].weight;
				if (num2 < num3)
				{
					return num4;
				}
			}
			List<int> list2 = list;
			return list2[list2.Count - 1];
		}

		// Token: 0x040089D0 RID: 35280
		[Header("Network Provider")]
		[Tooltip("For best result, pick Float01 or Double01 as the output mode in your NetworkedRandomProvider")]
		[SerializeField]
		private NetworkedRandomProvider networkProvider;

		// Token: 0x040089D1 RID: 35281
		[Header("Weighted Outputs")]
		[SerializeField]
		private List<RandomWeightedOutput.WeightedOutput> outputs = new List<RandomWeightedOutput.WeightedOutput>();

		// Token: 0x040089D2 RID: 35282
		[Header("Event")]
		[SerializeField]
		public UnityEvent<int> onAnyPick = new UnityEvent<int>();

		// Token: 0x040089D3 RID: 35283
		[SerializeField]
		private bool debugLog;

		// Token: 0x020012A3 RID: 4771
		[Serializable]
		public class WeightedOutput
		{
			// Token: 0x040089D4 RID: 35284
			[SerializeField]
			public string name = "Event";

			// Token: 0x040089D5 RID: 35285
			[SerializeField]
			[Range(0f, 100f)]
			public float weight = 1f;

			// Token: 0x040089D6 RID: 35286
			[SerializeField]
			public bool enabled = true;

			// Token: 0x040089D7 RID: 35287
			[SerializeField]
			public UnityEvent onPick = new UnityEvent();
		}
	}
}
