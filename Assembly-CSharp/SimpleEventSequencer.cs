using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000DAB RID: 3499
public class SimpleEventSequencer : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060055C3 RID: 21955 RVA: 0x001BF103 File Offset: 0x001BD303
	private void StartSequence()
	{
		this.StartSequenceDelayed(0f);
	}

	// Token: 0x060055C4 RID: 21956 RVA: 0x001BF110 File Offset: 0x001BD310
	public void StartSequenceDelayed(float delay)
	{
		SimpleEventSequencer.<StartSequenceDelayed>d__11 <StartSequenceDelayed>d__;
		<StartSequenceDelayed>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<StartSequenceDelayed>d__.<>4__this = this;
		<StartSequenceDelayed>d__.delay = delay;
		<StartSequenceDelayed>d__.<>1__state = -1;
		<StartSequenceDelayed>d__.<>t__builder.Start<SimpleEventSequencer.<StartSequenceDelayed>d__11>(ref <StartSequenceDelayed>d__);
	}

	// Token: 0x060055C5 RID: 21957 RVA: 0x001BF14F File Offset: 0x001BD34F
	private void startSequenceImmediate()
	{
		this.startTime = Time.time;
		this.idx = 0;
	}

	// Token: 0x060055C6 RID: 21958 RVA: 0x001BF163 File Offset: 0x001BD363
	private void startSequenceFrom(int i)
	{
		this.startTime = Time.time;
		this.idx = i;
	}

	// Token: 0x060055C7 RID: 21959 RVA: 0x001BF177 File Offset: 0x001BD377
	private void stop(int i)
	{
		this.idx = -1;
	}

	// Token: 0x060055C8 RID: 21960 RVA: 0x001BF180 File Offset: 0x001BD380
	private void Awake()
	{
		this.enabledNodes.Clear();
		for (int i = 0; i < this.nodes.Length; i++)
		{
			if (this.nodes[i].Enabled)
			{
				this.enabledNodes.Add(this.nodes[i]);
			}
		}
	}

	// Token: 0x060055C9 RID: 21961 RVA: 0x001BF1CD File Offset: 0x001BD3CD
	private void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		if (this.startOnEnable)
		{
			this.StartSequence();
		}
	}

	// Token: 0x060055CA RID: 21962 RVA: 0x00011DE0 File Offset: 0x0000FFE0
	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060055CB RID: 21963 RVA: 0x001BF1E4 File Offset: 0x001BD3E4
	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (this.idx < 0 || this.idx == this.enabledNodes.Count)
		{
			return;
		}
		if (Time.time >= this.startTime + this.enabledNodes[this.idx].Time)
		{
			UnityEvent unityEvent = this.enabledNodes[this.idx].UnityEvent;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.startTime = Time.time;
			this.idx++;
			if (this.idx == this.enabledNodes.Count)
			{
				SimpleEventSequencer.OnCompleteAction onCompleteAction = this.onComplete;
				if (onCompleteAction != SimpleEventSequencer.OnCompleteAction.Disable)
				{
					if (onCompleteAction == SimpleEventSequencer.OnCompleteAction.Repeat)
					{
						this.StartSequenceDelayed(this.enabledNodes[this.idx - 1].Time);
						return;
					}
				}
				else
				{
					base.gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x060055CC RID: 21964 RVA: 0x001BF2BC File Offset: 0x001BD4BC
	private void onValueChanged()
	{
		float num = 0f;
		for (int i = 0; i < this.nodes.Length; i++)
		{
			if (this.nodes[i].Enabled)
			{
				num += this.nodes[i].Time;
			}
			this.nodes[i].TotalTime = num;
			this.nodes[i].onValueChanged();
		}
	}

	// Token: 0x060055CD RID: 21965 RVA: 0x001BF31C File Offset: 0x001BD51C
	public void SetOnCompleteActionDisable()
	{
		this.onComplete = SimpleEventSequencer.OnCompleteAction.Disable;
	}

	// Token: 0x060055CE RID: 21966 RVA: 0x001BF325 File Offset: 0x001BD525
	public void SetOnCompleteActionRepeat()
	{
		this.onComplete = SimpleEventSequencer.OnCompleteAction.Repeat;
	}

	// Token: 0x060055CF RID: 21967 RVA: 0x001BF32E File Offset: 0x001BD52E
	public void ClearOnCompleteAction()
	{
		this.onComplete = SimpleEventSequencer.OnCompleteAction.None;
	}

	// Token: 0x060055D0 RID: 21968 RVA: 0x001BF337 File Offset: 0x001BD537
	public void TempAudio(string text)
	{
		Debug.Log("SimpleEventSequencer :: " + base.name + " :: TempAudio :: " + text);
	}

	// Token: 0x060055D1 RID: 21969 RVA: 0x001BF354 File Offset: 0x001BD554
	public void TempVFX(string text)
	{
		Debug.Log("SimpleEventSequencer :: " + base.name + " :: TempVFX :: " + text);
	}

	// Token: 0x060055D2 RID: 21970 RVA: 0x001BF371 File Offset: 0x001BD571
	public void Temp(string text)
	{
		Debug.Log("SimpleEventSequencer :: " + base.name + " :: Temp :: " + text);
	}

	// Token: 0x060055D3 RID: 21971 RVA: 0x001BF38E File Offset: 0x001BD58E
	public void DebugLog(string text)
	{
		Debug.Log("SimpleEventSequencer :: " + base.name + " :: DEBUG :: " + text);
	}

	// Token: 0x040065DF RID: 26079
	[SerializeField]
	private SimpleEventSequencer.SimpleEventSequencerNode[] nodes;

	// Token: 0x040065E0 RID: 26080
	[SerializeField]
	private bool startOnEnable = true;

	// Token: 0x040065E1 RID: 26081
	[SerializeField]
	private SimpleEventSequencer.OnCompleteAction onComplete = SimpleEventSequencer.OnCompleteAction.Disable;

	// Token: 0x040065E2 RID: 26082
	[SerializeField]
	private ServerTimeSyncRule serverTimeSync;

	// Token: 0x040065E3 RID: 26083
	private float startTime;

	// Token: 0x040065E4 RID: 26084
	private int idx = -1;

	// Token: 0x040065E5 RID: 26085
	private List<SimpleEventSequencer.SimpleEventSequencerNode> enabledNodes = new List<SimpleEventSequencer.SimpleEventSequencerNode>();

	// Token: 0x040065E6 RID: 26086
	private SimpleEventSequencer.SimpleEventSequencerNode activeNode;

	// Token: 0x02000DAC RID: 3500
	[Serializable]
	private class SimpleEventSequencerNode
	{
		// Token: 0x17000804 RID: 2052
		// (get) Token: 0x060055D5 RID: 21973 RVA: 0x001BF3D3 File Offset: 0x001BD5D3
		private string nameTrim
		{
			get
			{
				if (this.name.Length <= 33)
				{
					return this.name;
				}
				return this.name.Substring(0, 30) + "...";
			}
		}

		// Token: 0x17000805 RID: 2053
		// (get) Token: 0x060055D6 RID: 21974 RVA: 0x001BF403 File Offset: 0x001BD603
		private string notesTrim
		{
			get
			{
				if (this.notes.Length <= 50)
				{
					return this.notes;
				}
				return this.notes.Substring(0, 47) + "...";
			}
		}

		// Token: 0x17000806 RID: 2054
		// (get) Token: 0x060055D7 RID: 21975 RVA: 0x001BF433 File Offset: 0x001BD633
		public float Time
		{
			get
			{
				return this.time;
			}
		}

		// Token: 0x17000807 RID: 2055
		// (get) Token: 0x060055D8 RID: 21976 RVA: 0x001BF43B File Offset: 0x001BD63B
		public UnityEvent UnityEvent
		{
			get
			{
				return this.unityEvent;
			}
		}

		// Token: 0x17000808 RID: 2056
		// (get) Token: 0x060055D9 RID: 21977 RVA: 0x001BF443 File Offset: 0x001BD643
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		// Token: 0x17000809 RID: 2057
		// (set) Token: 0x060055DA RID: 21978 RVA: 0x001BF44B File Offset: 0x001BD64B
		public float TotalTime
		{
			set
			{
				this.totalTime = value;
			}
		}

		// Token: 0x1700080A RID: 2058
		// (get) Token: 0x060055DB RID: 21979 RVA: 0x001BF454 File Offset: 0x001BD654
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
		}

		// Token: 0x060055DC RID: 21980 RVA: 0x001BF45C File Offset: 0x001BD65C
		public void onValueChanged()
		{
			if (this.enabled)
			{
				this.fancyName = string.Format("T+{0} ({1}) : {2}", this.totalTime, this.time, this.nameTrim);
				return;
			}
			this.fancyName = string.Format("Skip ({0}) : {1}", this.time, this.nameTrim);
		}

		// Token: 0x040065E7 RID: 26087
		[Tooltip("Uncheck to skip this node")]
		[SerializeField]
		private bool enabled = true;

		// Token: 0x040065E8 RID: 26088
		[Tooltip("Seconds after the previous node's events are dispatched")]
		[SerializeField]
		private float time;

		// Token: 0x040065E9 RID: 26089
		[Tooltip("This is just for legibilty. Doesn't matter what you name it.")]
		[SerializeField]
		private string name = "New Node";

		// Token: 0x040065EA RID: 26090
		[SerializeField]
		private UnityEvent unityEvent;

		// Token: 0x040065EB RID: 26091
		[SerializeField]
		[TextArea(5, 10)]
		private string notes = "Notes";

		// Token: 0x040065EC RID: 26092
		private string fancyName = "New Node";

		// Token: 0x040065ED RID: 26093
		private float totalTime;
	}

	// Token: 0x02000DAD RID: 3501
	private enum OnCompleteAction
	{
		// Token: 0x040065EF RID: 26095
		None,
		// Token: 0x040065F0 RID: 26096
		Disable,
		// Token: 0x040065F1 RID: 26097
		Repeat
	}
}
