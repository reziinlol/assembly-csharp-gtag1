using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000F13 RID: 3859
	public class GorillaTimer : MonoBehaviourPun
	{
		// Token: 0x0600604D RID: 24653 RVA: 0x001F0A3D File Offset: 0x001EEC3D
		private void Awake()
		{
			this.ResetTimer();
		}

		// Token: 0x0600604E RID: 24654 RVA: 0x001F0A45 File Offset: 0x001EEC45
		public void StartTimer()
		{
			this.startTimer = true;
			UnityEvent<GorillaTimer> unityEvent = this.onTimerStarted;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}

		// Token: 0x0600604F RID: 24655 RVA: 0x001F0A5F File Offset: 0x001EEC5F
		public IEnumerator DelayedReStartTimer(float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			this.RestartTimer();
			yield break;
		}

		// Token: 0x06006050 RID: 24656 RVA: 0x001F0A75 File Offset: 0x001EEC75
		private void StopTimer()
		{
			this.startTimer = false;
			UnityEvent<GorillaTimer> unityEvent = this.onTimerStopped;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}

		// Token: 0x06006051 RID: 24657 RVA: 0x001F0A8F File Offset: 0x001EEC8F
		private void ResetTimer()
		{
			this.passedTime = 0f;
		}

		// Token: 0x06006052 RID: 24658 RVA: 0x001F0A9C File Offset: 0x001EEC9C
		public void RestartTimer()
		{
			if (this.useRandomDuration)
			{
				this.SetTimerDuration(Random.Range(this.randTimeMin, this.randTimeMax));
			}
			this.ResetTimer();
			this.StartTimer();
		}

		// Token: 0x06006053 RID: 24659 RVA: 0x001F0AC9 File Offset: 0x001EECC9
		public void SetTimerDuration(float timer)
		{
			this.timerDuration = timer;
		}

		// Token: 0x06006054 RID: 24660 RVA: 0x001F0AD2 File Offset: 0x001EECD2
		public void InvokeUpdate()
		{
			if (this.startTimer)
			{
				this.passedTime += Time.deltaTime;
			}
			if (this.startTimer && this.passedTime >= this.timerDuration)
			{
				this.StopTimer();
				this.ResetTimer();
			}
		}

		// Token: 0x06006055 RID: 24661 RVA: 0x001F0B10 File Offset: 0x001EED10
		public float GetPassedTime()
		{
			return this.passedTime;
		}

		// Token: 0x06006056 RID: 24662 RVA: 0x001F0B18 File Offset: 0x001EED18
		public void SetPassedTime(float time)
		{
			this.passedTime = time;
		}

		// Token: 0x06006057 RID: 24663 RVA: 0x001F0B21 File Offset: 0x001EED21
		public float GetRemainingTime()
		{
			return this.timerDuration - this.passedTime;
		}

		// Token: 0x06006058 RID: 24664 RVA: 0x001F0B30 File Offset: 0x001EED30
		public void OnEnable()
		{
			GorillaTimerManager.RegisterGorillaTimer(this);
		}

		// Token: 0x06006059 RID: 24665 RVA: 0x001F0B38 File Offset: 0x001EED38
		public void OnDisable()
		{
			GorillaTimerManager.UnregisterGorillaTimer(this);
		}

		// Token: 0x04006EDA RID: 28378
		[SerializeField]
		private float timerDuration;

		// Token: 0x04006EDB RID: 28379
		[SerializeField]
		private bool useRandomDuration;

		// Token: 0x04006EDC RID: 28380
		[SerializeField]
		private float randTimeMin;

		// Token: 0x04006EDD RID: 28381
		[SerializeField]
		private float randTimeMax;

		// Token: 0x04006EDE RID: 28382
		private float passedTime;

		// Token: 0x04006EDF RID: 28383
		private bool startTimer;

		// Token: 0x04006EE0 RID: 28384
		private bool resetTimer;

		// Token: 0x04006EE1 RID: 28385
		public UnityEvent<GorillaTimer> onTimerStarted;

		// Token: 0x04006EE2 RID: 28386
		public UnityEvent<GorillaTimer> onTimerStopped;
	}
}
