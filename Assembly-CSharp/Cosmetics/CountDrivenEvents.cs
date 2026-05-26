using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Cosmetics
{
	// Token: 0x0200112B RID: 4395
	public class CountDrivenEvents : MonoBehaviour
	{
		// Token: 0x17000AA3 RID: 2723
		// (get) Token: 0x06006F9B RID: 28571 RVA: 0x002473F6 File Offset: 0x002455F6
		public int CurrentCount
		{
			get
			{
				return this.currentCount;
			}
		}

		// Token: 0x06006F9C RID: 28572 RVA: 0x00247400 File Offset: 0x00245600
		private void OnEnable()
		{
			if (this.myRig == null)
			{
				this.myRig = base.GetComponentInParent<VRRig>();
			}
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			}
			NetPlayer netPlayer = (this.myRig != null) ? (this.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : NetworkSystem.Instance.LocalPlayer;
			if (netPlayer != null)
			{
				this._events.Init(netPlayer);
			}
			if (this._events != null)
			{
				this._events.Activate.reliable = true;
				this._events.Deactivate.reliable = true;
				this._events.Activate += this.OnCountChanged_SharedEvent;
				this._events.Deactivate += this.OnCountReached_SharedEvent;
			}
			if (this.evaluateOnEnable)
			{
				this.CheckTriggers(this.currentCount, this.currentCount);
			}
		}

		// Token: 0x06006F9D RID: 28573 RVA: 0x00247518 File Offset: 0x00245718
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.OnCountChanged_SharedEvent;
				this._events.Deactivate -= this.OnCountReached_SharedEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06006F9E RID: 28574 RVA: 0x0024758C File Offset: 0x0024578C
		private void OnValidate()
		{
			if (this.triggers == null)
			{
				return;
			}
			for (int i = 0; i < this.triggers.Count; i++)
			{
				if (this.triggers[i].triggerCount < 0)
				{
					this.triggers[i].triggerCount = 0;
				}
			}
		}

		// Token: 0x06006F9F RID: 28575 RVA: 0x002475DE File Offset: 0x002457DE
		public void Increment()
		{
			this.SetCount(this.currentCount + 1);
		}

		// Token: 0x06006FA0 RID: 28576 RVA: 0x002475EE File Offset: 0x002457EE
		public void Decrement()
		{
			this.SetCount(this.currentCount - 1);
		}

		// Token: 0x06006FA1 RID: 28577 RVA: 0x00247600 File Offset: 0x00245800
		public void SetCount(int newCount)
		{
			if (this.myRig != null && !this.myRig.isLocal)
			{
				return;
			}
			int num = this.currentCount;
			if (this.wrapCount)
			{
				int highestTriggerCount = this.GetHighestTriggerCount();
				if (highestTriggerCount > 0)
				{
					int num2 = highestTriggerCount + 1;
					newCount = (newCount % num2 + num2) % num2;
				}
				else if (newCount < 0)
				{
					newCount = 0;
				}
			}
			else if (newCount < 0)
			{
				newCount = 0;
			}
			if (newCount == num)
			{
				return;
			}
			bool flag = false;
			this.currentCount = newCount;
			UnityEvent<int> unityEvent = this.onCountChanged;
			if (unityEvent != null)
			{
				unityEvent.Invoke(this.currentCount);
			}
			UnityEvent<int> unityEvent2 = this.onCountChangedShared;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke(this.currentCount);
			}
			if (this.currentCount > num)
			{
				UnityEvent<int> unityEvent3 = this.onCountIncreased;
				if (unityEvent3 != null)
				{
					unityEvent3.Invoke(this.currentCount);
				}
				UnityEvent<int> unityEvent4 = this.onCountIncreasedShared;
				if (unityEvent4 != null)
				{
					unityEvent4.Invoke(this.currentCount);
				}
				flag = true;
			}
			else if (this.currentCount < num)
			{
				UnityEvent<int> unityEvent5 = this.onCountDecreased;
				if (unityEvent5 != null)
				{
					unityEvent5.Invoke(this.currentCount);
				}
				UnityEvent<int> unityEvent6 = this.onCountDecreasedShared;
				if (unityEvent6 != null)
				{
					unityEvent6.Invoke(this.currentCount);
				}
			}
			this.CheckTriggers(num, this.currentCount);
			if (this.currentCount == 0)
			{
				UnityEvent unityEvent7 = this.onCountResetToZero;
				if (unityEvent7 != null)
				{
					unityEvent7.Invoke();
				}
				UnityEvent unityEvent8 = this.onCountResetToZeroShared;
				if (unityEvent8 != null)
				{
					unityEvent8.Invoke();
				}
			}
			int highestTriggerCount2 = this.GetHighestTriggerCount();
			if (highestTriggerCount2 > 0 && this.currentCount == highestTriggerCount2)
			{
				UnityEvent unityEvent9 = this.onReachedMaxTrigger;
				if (unityEvent9 != null)
				{
					unityEvent9.Invoke();
				}
				UnityEvent unityEvent10 = this.onReachedMaxTriggerShared;
				if (unityEvent10 != null)
				{
					unityEvent10.Invoke();
				}
			}
			if (this.syncAllEvents && PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				object[] args = new object[]
				{
					flag,
					this.currentCount
				};
				this._events.Activate.RaiseOthers(args);
			}
		}

		// Token: 0x06006FA2 RID: 28578 RVA: 0x002477E4 File Offset: 0x002459E4
		[Tooltip("Resets all 'triggerOnce' flags, allowing one-time triggers to fire again.\n\nUse this when restarting a sequence, resetting an object,\nor testing trigger behavior multiple times in play mode.")]
		public void ResetTriggers()
		{
			for (int i = 0; i < this.triggers.Count; i++)
			{
				this.triggers[i].hasTriggered = false;
			}
		}

		// Token: 0x06006FA3 RID: 28579 RVA: 0x0024781C File Offset: 0x00245A1C
		private int GetHighestTriggerCount()
		{
			int num = 0;
			for (int i = 0; i < this.triggers.Count; i++)
			{
				if (this.triggers[i].triggerCount > num)
				{
					num = this.triggers[i].triggerCount;
				}
			}
			return num;
		}

		// Token: 0x06006FA4 RID: 28580 RVA: 0x00247868 File Offset: 0x00245A68
		private void CheckTriggers(int oldCount, int newCount)
		{
			if (this.myRig != null && !this.myRig.isLocal)
			{
				return;
			}
			for (int i = 0; i < this.triggers.Count; i++)
			{
				CountDrivenEvents.CountTrigger countTrigger = this.triggers[i];
				if (!countTrigger.triggerOnce || !countTrigger.hasTriggered)
				{
					bool flag = false;
					if (this.wrapCount)
					{
						if (newCount == countTrigger.triggerCount)
						{
							flag = true;
						}
					}
					else if (oldCount < countTrigger.triggerCount && newCount >= countTrigger.triggerCount)
					{
						flag = true;
					}
					else if (oldCount > countTrigger.triggerCount && newCount <= countTrigger.triggerCount)
					{
						flag = true;
					}
					else if (oldCount == newCount && newCount == countTrigger.triggerCount)
					{
						flag = true;
					}
					if (flag)
					{
						UnityEvent onCountReached = countTrigger.onCountReached;
						if (onCountReached != null)
						{
							onCountReached.Invoke();
						}
						UnityEvent onCountReachedShared = countTrigger.onCountReachedShared;
						if (onCountReachedShared != null)
						{
							onCountReachedShared.Invoke();
						}
						if (this.syncAllEvents && PhotonNetwork.InRoom && this._events != null && this._events.Deactivate != null)
						{
							object[] args = new object[]
							{
								i
							};
							this._events.Deactivate.RaiseOthers(args);
						}
						if (countTrigger.triggerOnce)
						{
							countTrigger.hasTriggered = true;
						}
					}
				}
			}
		}

		// Token: 0x06006FA5 RID: 28581 RVA: 0x002479A8 File Offset: 0x00245BA8
		private void OnCountChanged_SharedEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target || info.senderID != this.myRig.creator.ActorNumber)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "OnCountChanged_SharedEvent");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			if (args.Length != 2)
			{
				return;
			}
			object obj = args[0];
			if (!(obj is bool))
			{
				return;
			}
			bool flag = (bool)obj;
			obj = args[1];
			if (!(obj is int))
			{
				return;
			}
			int num = (int)obj;
			UnityEvent<int> unityEvent = this.onCountChangedShared;
			if (unityEvent != null)
			{
				unityEvent.Invoke(num);
			}
			if (flag)
			{
				UnityEvent<int> unityEvent2 = this.onCountIncreasedShared;
				if (unityEvent2 != null)
				{
					unityEvent2.Invoke(num);
				}
			}
			else
			{
				UnityEvent<int> unityEvent3 = this.onCountDecreasedShared;
				if (unityEvent3 != null)
				{
					unityEvent3.Invoke(num);
				}
			}
			int highestTriggerCount = this.GetHighestTriggerCount();
			if (num != 0)
			{
				if (highestTriggerCount > 0 && num == highestTriggerCount)
				{
					UnityEvent unityEvent4 = this.onReachedMaxTriggerShared;
					if (unityEvent4 == null)
					{
						return;
					}
					unityEvent4.Invoke();
				}
				return;
			}
			UnityEvent unityEvent5 = this.onCountResetToZeroShared;
			if (unityEvent5 == null)
			{
				return;
			}
			unityEvent5.Invoke();
		}

		// Token: 0x06006FA6 RID: 28582 RVA: 0x00247A94 File Offset: 0x00245C94
		private void OnCountReached_SharedEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target || info.senderID != this.myRig.creator.ActorNumber)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "OnCountReached_SharedEvent");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			if (args.Length != 1)
			{
				return;
			}
			object obj = args[0];
			if (!(obj is int))
			{
				return;
			}
			int num = (int)obj;
			if (num < 0 || num >= this.triggers.Count)
			{
				return;
			}
			UnityEvent onCountReachedShared = this.triggers[num].onCountReachedShared;
			if (onCountReachedShared == null)
			{
				return;
			}
			onCountReachedShared.Invoke();
		}

		// Token: 0x04007F9B RID: 32667
		[Header("Network")]
		[SerializeField]
		private bool syncAllEvents;

		// Token: 0x04007F9C RID: 32668
		[Header("General Settings")]
		[Tooltip("If true, triggers will be evaluated once on enable using the initial count.")]
		[SerializeField]
		private bool evaluateOnEnable;

		// Token: 0x04007F9D RID: 32669
		[Tooltip("If enabled, the counter value will loop between 0 and the highest triggerCount.")]
		[SerializeField]
		private bool wrapCount;

		// Token: 0x04007F9E RID: 32670
		[Header("Count Triggers")]
		[SerializeField]
		private List<CountDrivenEvents.CountTrigger> triggers = new List<CountDrivenEvents.CountTrigger>();

		// Token: 0x04007F9F RID: 32671
		[Header("Local and Networked Events")]
		public UnityEvent<int> onCountChanged;

		// Token: 0x04007FA0 RID: 32672
		public UnityEvent<int> onCountChangedShared;

		// Token: 0x04007FA1 RID: 32673
		public UnityEvent<int> onCountIncreased;

		// Token: 0x04007FA2 RID: 32674
		public UnityEvent<int> onCountIncreasedShared;

		// Token: 0x04007FA3 RID: 32675
		public UnityEvent<int> onCountDecreased;

		// Token: 0x04007FA4 RID: 32676
		public UnityEvent<int> onCountDecreasedShared;

		// Token: 0x04007FA5 RID: 32677
		public UnityEvent onCountResetToZero;

		// Token: 0x04007FA6 RID: 32678
		public UnityEvent onCountResetToZeroShared;

		// Token: 0x04007FA7 RID: 32679
		public UnityEvent onReachedMaxTrigger;

		// Token: 0x04007FA8 RID: 32680
		public UnityEvent onReachedMaxTriggerShared;

		// Token: 0x04007FA9 RID: 32681
		[Header("Debug - Counter Settings")]
		[SerializeField]
		private int currentCount;

		// Token: 0x04007FAA RID: 32682
		private RubberDuckEvents _events;

		// Token: 0x04007FAB RID: 32683
		private VRRig myRig;

		// Token: 0x04007FAC RID: 32684
		private CallLimiter callLimiter = new CallLimiter(10, 1f, 0.5f);

		// Token: 0x0200112C RID: 4396
		[Serializable]
		public class CountTrigger
		{
			// Token: 0x04007FAD RID: 32685
			[Tooltip("The count value that triggers this event")]
			public int triggerCount;

			// Token: 0x04007FAE RID: 32686
			[Tooltip("Events to invoke when count reaches this value")]
			public UnityEvent onCountReached;

			// Token: 0x04007FAF RID: 32687
			public UnityEvent onCountReachedShared;

			// Token: 0x04007FB0 RID: 32688
			[Tooltip("Should this trigger fire every time the count passes through this value, or only once?")]
			public bool triggerOnce;

			// Token: 0x04007FB1 RID: 32689
			[NonSerialized]
			public bool hasTriggered;
		}
	}
}
