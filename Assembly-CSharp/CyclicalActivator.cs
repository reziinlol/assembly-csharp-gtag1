using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000093 RID: 147
public class CyclicalActivator : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060003A1 RID: 929 RVA: 0x00011DD7 File Offset: 0x0000FFD7
	private void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x00011DE0 File Offset: 0x0000FFE0
	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x00014ED0 File Offset: 0x000130D0
	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (GorillaComputer.instance == null || GorillaComputer.instance.GetServerTime().Year < 2000)
		{
			return;
		}
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		float nowSeconds = (float)(serverTime.Minute * 60) + ((float)serverTime.Second + (float)serverTime.Millisecond * 0.001f);
		for (int i = 0; i < this.objects.Length; i++)
		{
			this.objects[i].gameObject.SetActive(this.objects[i].schedule.CheckTime(nowSeconds));
		}
	}

	// Token: 0x04000426 RID: 1062
	[SerializeField]
	private CyclicalActivator.CyclicalActivatorObject[] objects;

	// Token: 0x02000094 RID: 148
	[Serializable]
	private class CyclicalActivatorObjectScheduleNode
	{
		// Token: 0x04000427 RID: 1063
		public Vector2 secondsActiveRange;
	}

	// Token: 0x02000095 RID: 149
	[Serializable]
	private class CyclicalActivatorObjectSchedule
	{
		// Token: 0x060003A6 RID: 934 RVA: 0x00014F74 File Offset: 0x00013174
		public bool CheckTime(float nowSeconds)
		{
			nowSeconds %= (float)this.totalSeconds;
			for (int i = 0; i < this.schedule.Length; i++)
			{
				if (this.schedule[i].secondsActiveRange.x <= nowSeconds && this.schedule[i].secondsActiveRange.y > nowSeconds)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04000428 RID: 1064
		[Range(10f, 3599f)]
		public int totalSeconds = 60;

		// Token: 0x04000429 RID: 1065
		public CyclicalActivator.CyclicalActivatorObjectScheduleNode[] schedule;
	}

	// Token: 0x02000096 RID: 150
	[Serializable]
	private class CyclicalActivatorObject
	{
		// Token: 0x0400042A RID: 1066
		public GameObject gameObject;

		// Token: 0x0400042B RID: 1067
		public CyclicalActivator.CyclicalActivatorObjectSchedule schedule;
	}
}
