using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000AF0 RID: 2800
public class SubEmitterListener : MonoBehaviour
{
	// Token: 0x0600479F RID: 18335 RVA: 0x00180FE8 File Offset: 0x0017F1E8
	private void OnEnable()
	{
		if (this.target == null)
		{
			this.Disable();
			return;
		}
		ParticleSystem.SubEmittersModule subEmitters = this.target.subEmitters;
		if (this.subEmitterIndex < 0)
		{
			this.subEmitterIndex = 0;
		}
		this._canListen = (subEmitters.subEmittersCount > 0 && this.subEmitterIndex <= subEmitters.subEmittersCount - 1);
		if (!this._canListen)
		{
			this.Disable();
			return;
		}
		this.subEmitter = this.target.subEmitters.GetSubEmitterSystem(this.subEmitterIndex);
		ParticleSystem.MainModule main = this.subEmitter.main;
		this.interval = main.startLifetime.constantMax * main.startLifetimeMultiplier;
	}

	// Token: 0x060047A0 RID: 18336 RVA: 0x001810A4 File Offset: 0x0017F2A4
	private void OnDisable()
	{
		this._listenOnce = false;
		this._listening = false;
	}

	// Token: 0x060047A1 RID: 18337 RVA: 0x001810B4 File Offset: 0x0017F2B4
	public void ListenStart()
	{
		if (this._listening)
		{
			return;
		}
		if (this._canListen)
		{
			this.Enable();
			this._listening = true;
		}
	}

	// Token: 0x060047A2 RID: 18338 RVA: 0x001810D4 File Offset: 0x0017F2D4
	public void ListenStop()
	{
		this.Disable();
	}

	// Token: 0x060047A3 RID: 18339 RVA: 0x001810DC File Offset: 0x0017F2DC
	public void ListenOnce()
	{
		if (this._listening)
		{
			return;
		}
		this.Enable();
		if (this._canListen)
		{
			this.Enable();
			this._listenOnce = true;
			this._listening = true;
		}
	}

	// Token: 0x060047A4 RID: 18340 RVA: 0x0018110C File Offset: 0x0017F30C
	private void Update()
	{
		if (!this._canListen)
		{
			return;
		}
		if (!this._listening)
		{
			return;
		}
		if (this.subEmitter.particleCount > 0 && this._sinceLastEmit >= this.interval * this.intervalScale)
		{
			this._sinceLastEmit = 0f;
			this.OnSubEmit();
			if (this._listenOnce)
			{
				this.Disable();
			}
		}
	}

	// Token: 0x060047A5 RID: 18341 RVA: 0x00181177 File Offset: 0x0017F377
	protected virtual void OnSubEmit()
	{
		UnityEvent unityEvent = this.onSubEmit;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x060047A6 RID: 18342 RVA: 0x00181189 File Offset: 0x0017F389
	public void Enable()
	{
		if (!base.enabled)
		{
			base.enabled = true;
		}
	}

	// Token: 0x060047A7 RID: 18343 RVA: 0x0018119A File Offset: 0x0017F39A
	public void Disable()
	{
		if (base.enabled)
		{
			base.enabled = false;
		}
	}

	// Token: 0x040059CC RID: 22988
	public ParticleSystem target;

	// Token: 0x040059CD RID: 22989
	public ParticleSystem subEmitter;

	// Token: 0x040059CE RID: 22990
	public int subEmitterIndex;

	// Token: 0x040059CF RID: 22991
	public UnityEvent onSubEmit;

	// Token: 0x040059D0 RID: 22992
	public float intervalScale = 1f;

	// Token: 0x040059D1 RID: 22993
	public float interval;

	// Token: 0x040059D2 RID: 22994
	[NonSerialized]
	private bool _canListen;

	// Token: 0x040059D3 RID: 22995
	[NonSerialized]
	private bool _listening;

	// Token: 0x040059D4 RID: 22996
	[NonSerialized]
	private bool _listenOnce;

	// Token: 0x040059D5 RID: 22997
	[NonSerialized]
	private TimeSince _sinceLastEmit;
}
