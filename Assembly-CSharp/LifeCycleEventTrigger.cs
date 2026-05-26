using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000ADC RID: 2780
public class LifeCycleEventTrigger : MonoBehaviour
{
	// Token: 0x060046F2 RID: 18162 RVA: 0x0017F08F File Offset: 0x0017D28F
	private void Awake()
	{
		UnityEvent onAwake = this._onAwake;
		if (onAwake == null)
		{
			return;
		}
		onAwake.Invoke();
	}

	// Token: 0x060046F3 RID: 18163 RVA: 0x0017F0A1 File Offset: 0x0017D2A1
	private void Start()
	{
		UnityEvent onStart = this._onStart;
		if (onStart == null)
		{
			return;
		}
		onStart.Invoke();
	}

	// Token: 0x060046F4 RID: 18164 RVA: 0x0017F0B3 File Offset: 0x0017D2B3
	private void OnEnable()
	{
		UnityEvent onEnable = this._onEnable;
		if (onEnable == null)
		{
			return;
		}
		onEnable.Invoke();
	}

	// Token: 0x060046F5 RID: 18165 RVA: 0x0017F0C5 File Offset: 0x0017D2C5
	private void OnDisable()
	{
		UnityEvent onDisable = this._onDisable;
		if (onDisable == null)
		{
			return;
		}
		onDisable.Invoke();
	}

	// Token: 0x060046F6 RID: 18166 RVA: 0x0017F0D7 File Offset: 0x0017D2D7
	private void OnDestroy()
	{
		UnityEvent onDestroy = this._onDestroy;
		if (onDestroy == null)
		{
			return;
		}
		onDestroy.Invoke();
	}

	// Token: 0x04005948 RID: 22856
	[SerializeField]
	private UnityEvent _onAwake;

	// Token: 0x04005949 RID: 22857
	[SerializeField]
	private UnityEvent _onStart;

	// Token: 0x0400594A RID: 22858
	[SerializeField]
	private UnityEvent _onEnable;

	// Token: 0x0400594B RID: 22859
	[SerializeField]
	private UnityEvent _onDisable;

	// Token: 0x0400594C RID: 22860
	[SerializeField]
	private UnityEvent _onDestroy;
}
