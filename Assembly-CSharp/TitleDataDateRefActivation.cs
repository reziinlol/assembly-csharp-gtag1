using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000DEC RID: 3564
public class TitleDataDateRefActivation : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x0600573D RID: 22333 RVA: 0x001C376C File Offset: 0x001C196C
	private void Initialize()
	{
		TitleDataDateRefActivation.<Initialize>d__8 <Initialize>d__;
		<Initialize>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Initialize>d__.<>4__this = this;
		<Initialize>d__.<>1__state = -1;
		<Initialize>d__.<>t__builder.Start<TitleDataDateRefActivation.<Initialize>d__8>(ref <Initialize>d__);
	}

	// Token: 0x0600573E RID: 22334 RVA: 0x001C37A4 File Offset: 0x001C19A4
	private void onTD(string s)
	{
		try
		{
			this.setStartDate(DateTime.Parse(s));
			this.readyState = TitleDataDateRefActivation.ReadyState.Ready;
		}
		catch (Exception ex)
		{
			Debug.Log("TitleDataDateRefActivation :: onTD :: " + ex.Message + " :: " + ex.StackTrace);
			this.readyState = TitleDataDateRefActivation.ReadyState.Crashed;
		}
	}

	// Token: 0x0600573F RID: 22335 RVA: 0x001C3800 File Offset: 0x001C1A00
	public void StartNow(float delay)
	{
		this.setStartDate(GorillaComputer.instance.GetServerTime().AddSeconds((double)delay));
	}

	// Token: 0x06005740 RID: 22336 RVA: 0x001C382C File Offset: 0x001C1A2C
	private void setStartDate(DateTime d)
	{
		this.nodeList.Clear();
		for (int i = 0; i < this.nodes.Length; i++)
		{
			this.nodes[i].Initialize(d);
			this.nodeList.Add(this.nodes[i]);
		}
		this.nodeList.Sort();
		this.activations = 0;
	}

	// Token: 0x06005741 RID: 22337 RVA: 0x001C388A File Offset: 0x001C1A8A
	private void onTDError(PlayFabError error)
	{
		Debug.Log(string.Format("TitleDataDateRefActivation :: onTDError :: {0}", error));
		this.readyState = TitleDataDateRefActivation.ReadyState.Crashed;
	}

	// Token: 0x06005742 RID: 22338 RVA: 0x001C38A3 File Offset: 0x001C1AA3
	private void OnEnable()
	{
		this.Initialize();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06005743 RID: 22339 RVA: 0x00011DE0 File Offset: 0x0000FFE0
	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06005744 RID: 22340 RVA: 0x001C38B4 File Offset: 0x001C1AB4
	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (this.readyState != TitleDataDateRefActivation.ReadyState.Ready || this.nodeList.Count == 0)
		{
			return;
		}
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		if (serverTime.Year < 2000)
		{
			return;
		}
		if (this.tmpStatus != null)
		{
			this.tmpStatus.text = string.Format("action {0} of {1} in {2:g} s", this.activations + 1, this.nodes.Length, this.nodeList[0].ActivationTime - GorillaComputer.instance.GetServerTime());
		}
		if (this.nodeList[0].ActivationTime <= serverTime)
		{
			this.nodeList[0].Activate(serverTime);
			this.nodeList.RemoveAt(0);
			this.activations++;
		}
	}

	// Token: 0x04006728 RID: 26408
	[SerializeField]
	private string titleDataKey;

	// Token: 0x04006729 RID: 26409
	[SerializeField]
	private TitleDataDateRefActivation.TitleDataDateRefActivationTarget[] nodes;

	// Token: 0x0400672A RID: 26410
	[SerializeField]
	private TMP_Text tmpStatus;

	// Token: 0x0400672B RID: 26411
	private TitleDataDateRefActivation.ReadyState readyState;

	// Token: 0x0400672C RID: 26412
	private List<TitleDataDateRefActivation.TitleDataDateRefActivationTarget> nodeList = new List<TitleDataDateRefActivation.TitleDataDateRefActivationTarget>();

	// Token: 0x0400672D RID: 26413
	private int activations;

	// Token: 0x02000DED RID: 3565
	private enum ReadyState
	{
		// Token: 0x0400672F RID: 26415
		None,
		// Token: 0x04006730 RID: 26416
		Initializing,
		// Token: 0x04006731 RID: 26417
		Ready,
		// Token: 0x04006732 RID: 26418
		Crashed
	}

	// Token: 0x02000DEE RID: 3566
	[Serializable]
	private class TitleDataDateRefActivationTarget : IComparable<TitleDataDateRefActivation.TitleDataDateRefActivationTarget>
	{
		// Token: 0x17000839 RID: 2105
		// (get) Token: 0x06005746 RID: 22342 RVA: 0x001C39B0 File Offset: 0x001C1BB0
		public GameObject GameObject
		{
			get
			{
				return this.gameObject;
			}
		}

		// Token: 0x1700083A RID: 2106
		// (get) Token: 0x06005747 RID: 22343 RVA: 0x001C39B8 File Offset: 0x001C1BB8
		public DateTime ActivationTime
		{
			get
			{
				return this.dateTime;
			}
		}

		// Token: 0x06005748 RID: 22344 RVA: 0x001C39C0 File Offset: 0x001C1BC0
		public void Initialize(DateTime refTime)
		{
			this.dateTime = refTime.AddHours((double)this.hrs).AddMinutes((double)this.min).AddSeconds((double)this.sec);
		}

		// Token: 0x06005749 RID: 22345 RVA: 0x001C3A00 File Offset: 0x001C1C00
		public void Activate(DateTime now)
		{
			float late = (float)(now - this.dateTime).TotalSeconds;
			this.Activate(late);
		}

		// Token: 0x0600574A RID: 22346 RVA: 0x001C3A2A File Offset: 0x001C1C2A
		public void Activate()
		{
			this.Activate(0f);
		}

		// Token: 0x0600574B RID: 22347 RVA: 0x001C3A38 File Offset: 0x001C1C38
		private void Activate(float late)
		{
			if (this.gameObject != null)
			{
				this.gameObject.SetActive(this.activationState);
			}
			if (late < 1f)
			{
				UnityEvent unityEvent = this.payload;
				if (unityEvent != null)
				{
					unityEvent.Invoke();
				}
			}
			UnityEvent<float> unityEvent2 = this.persistantPayload;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke(late);
		}

		// Token: 0x0600574C RID: 22348 RVA: 0x001C3A90 File Offset: 0x001C1C90
		int IComparable<TitleDataDateRefActivation.TitleDataDateRefActivationTarget>.CompareTo(TitleDataDateRefActivation.TitleDataDateRefActivationTarget other)
		{
			return (this.hrs * 3600 + this.min * 60 + this.sec).CompareTo(other.hrs * 3600 + other.min * 60 + other.sec);
		}

		// Token: 0x04006733 RID: 26419
		[SerializeField]
		private bool activationState;

		// Token: 0x04006734 RID: 26420
		[SerializeField]
		private GameObject gameObject;

		// Token: 0x04006735 RID: 26421
		[SerializeField]
		private int hrs;

		// Token: 0x04006736 RID: 26422
		[SerializeField]
		private int min;

		// Token: 0x04006737 RID: 26423
		[SerializeField]
		private int sec;

		// Token: 0x04006738 RID: 26424
		[SerializeField]
		private UnityEvent payload;

		// Token: 0x04006739 RID: 26425
		[SerializeField]
		private UnityEvent<float> persistantPayload;

		// Token: 0x0400673A RID: 26426
		private DateTime dateTime = DateTime.MaxValue;
	}
}
