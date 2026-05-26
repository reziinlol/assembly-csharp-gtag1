using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Subscription.AlarmClocks
{
	// Token: 0x02000F6D RID: 3949
	public sealed class AlarmClock : MonoBehaviour
	{
		// Token: 0x17000945 RID: 2373
		// (get) Token: 0x0600627A RID: 25210 RVA: 0x001FC790 File Offset: 0x001FA990
		public string Key
		{
			get
			{
				return this._key;
			}
		}

		// Token: 0x17000946 RID: 2374
		// (get) Token: 0x0600627B RID: 25211 RVA: 0x001FC798 File Offset: 0x001FA998
		// (set) Token: 0x0600627C RID: 25212 RVA: 0x001FC7A0 File Offset: 0x001FA9A0
		public bool Initialized { get; private set; }

		// Token: 0x0600627D RID: 25213 RVA: 0x001FC7AC File Offset: 0x001FA9AC
		private void OnEnable()
		{
			this._button.onPressButton.AddListener(new UnityAction(this.OnButtonPressed));
			this.OnActivate.AddListener(new UnityAction(this.OnActivateCallback));
			this.OnDeactivate.AddListener(new UnityAction(this.OnDeactivateCallback));
			base.StartCoroutine(this.ActivateCoroutine());
		}

		// Token: 0x0600627E RID: 25214 RVA: 0x001FC810 File Offset: 0x001FAA10
		private IEnumerator ActivateCoroutine()
		{
			while (AlarmClockManager.Instance == null || !AlarmClockManager.Instance.Initialized)
			{
				yield return null;
			}
			if (AlarmClockManager.Instance.ActiveKey == this._key)
			{
				this.OnActivateCallback();
			}
			else
			{
				this.OnDeactivateCallback();
			}
			this.Initialized = true;
			yield break;
		}

		// Token: 0x0600627F RID: 25215 RVA: 0x001FC820 File Offset: 0x001FAA20
		private void OnDisable()
		{
			this._button.onPressButton.RemoveListener(new UnityAction(this.OnButtonPressed));
			this.OnActivate.RemoveListener(new UnityAction(this.OnActivateCallback));
			this.OnDeactivate.RemoveListener(new UnityAction(this.OnDeactivateCallback));
			base.StopAllCoroutines();
		}

		// Token: 0x06006280 RID: 25216 RVA: 0x001FC87D File Offset: 0x001FAA7D
		private void OnButtonPressed()
		{
			if (!this.Initialized)
			{
				return;
			}
			if (Time.time < this._lastTouchTime + 0.25f)
			{
				return;
			}
			if (!SubscriptionManager.IsLocalSubscribed())
			{
				return;
			}
			this._lastTouchTime = Time.time;
			AlarmClockManager.ToggleAlarmClock(this);
		}

		// Token: 0x06006281 RID: 25217 RVA: 0x001FC8B5 File Offset: 0x001FAAB5
		private void OnActivateCallback()
		{
			this._alarmClockOff.SetActive(false);
			this._button.buttonRenderer.material.color = Color.red;
		}

		// Token: 0x06006282 RID: 25218 RVA: 0x001FC8E0 File Offset: 0x001FAAE0
		private void OnDeactivateCallback()
		{
			this._alarmClockOff.SetActive(true);
			this._button.buttonRenderer.material.color = (SubscriptionManager.IsLocalSubscribed() ? Color.white : new Color(0.33f, 0.33f, 0.33f));
		}

		// Token: 0x04007165 RID: 29029
		private const float TouchDebouncePeriod = 0.25f;

		// Token: 0x04007166 RID: 29030
		[SerializeField]
		private string _key;

		// Token: 0x04007167 RID: 29031
		[SerializeField]
		private GorillaPressableButton _button;

		// Token: 0x04007168 RID: 29032
		[SerializeField]
		private GameObject _alarmClockOff;

		// Token: 0x04007169 RID: 29033
		[SerializeField]
		private float _onTime = 1f;

		// Token: 0x0400716A RID: 29034
		[SerializeField]
		private float _offTime = 0.2f;

		// Token: 0x0400716B RID: 29035
		public UnityEvent OnActivate;

		// Token: 0x0400716C RID: 29036
		public UnityEvent OnDeactivate;

		// Token: 0x0400716D RID: 29037
		private float _lastTouchTime = float.MinValue;
	}
}
