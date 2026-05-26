using System;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x020001B8 RID: 440
public class VODTarget : ObservableBehavior, IBuildValidation
{
	// Token: 0x1700011B RID: 283
	// (get) Token: 0x06000BC1 RID: 3009 RVA: 0x000404EE File Offset: 0x0003E6EE
	public VODTarget.VODTargetAudioSettings AudioSettings
	{
		get
		{
			return this.audioSettings;
		}
	}

	// Token: 0x1700011C RID: 284
	// (get) Token: 0x06000BC2 RID: 3010 RVA: 0x000404F6 File Offset: 0x0003E6F6
	public Renderer Renderer
	{
		get
		{
			return this.targetRenderer;
		}
	}

	// Token: 0x1700011D RID: 285
	// (get) Token: 0x06000BC3 RID: 3011 RVA: 0x000404FE File Offset: 0x0003E6FE
	public Material StandbyOverride
	{
		get
		{
			return this.standbyOverride;
		}
	}

	// Token: 0x1700011E RID: 286
	// (get) Token: 0x06000BC4 RID: 3012 RVA: 0x00040506 File Offset: 0x0003E706
	public VODPlayer.VODStream.VODStreamChannel[] Channel
	{
		get
		{
			if (this.channel.Length != 0)
			{
				return this.channel;
			}
			return new VODPlayer.VODStream.VODStreamChannel[1];
		}
	}

	// Token: 0x06000BC5 RID: 3013 RVA: 0x0004051E File Offset: 0x0003E71E
	public void SetNext(VODPlayer.VODNextStreamData data)
	{
		this.upNextData = data;
	}

	// Token: 0x06000BC6 RID: 3014 RVA: 0x00040527 File Offset: 0x0003E727
	public void ClearNext()
	{
		this.upNextData = default(VODPlayer.VODNextStreamData);
	}

	// Token: 0x06000BC7 RID: 3015 RVA: 0x00040538 File Offset: 0x0003E738
	public bool VerifyChannel(VODPlayer.VODStream.VODStreamChannel ch)
	{
		if (this.channel.Length == 0 && ch == VODPlayer.VODStream.VODStreamChannel.DEFAULT)
		{
			return true;
		}
		for (int i = 0; i < this.channel.Length; i++)
		{
			if (this.channel[i] == ch)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000BC8 RID: 3016 RVA: 0x00040574 File Offset: 0x0003E774
	protected override void OnLostObservable()
	{
		if (!this.staticScreen.activeInHierarchy && VODTarget.AlertDisabled != null)
		{
			VODTarget.AlertDisabled(this);
		}
	}

	// Token: 0x06000BC9 RID: 3017 RVA: 0x00040595 File Offset: 0x0003E795
	protected override void OnBecameObservable()
	{
		if (!this.staticScreen.activeInHierarchy && VODTarget.AlertEnabled != null)
		{
			VODTarget.AlertEnabled(this);
		}
	}

	// Token: 0x06000BCA RID: 3018 RVA: 0x000405B6 File Offset: 0x0003E7B6
	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.targetRenderer == null)
		{
			Debug.LogError("VODTarget " + base.name + " must set a Target Renderer");
			return false;
		}
		return true;
	}

	// Token: 0x06000BCB RID: 3019 RVA: 0x000405E3 File Offset: 0x0003E7E3
	private void Start()
	{
		this.targetRenderer.material = ((this.standbyOverride == null) ? VODPlayer.StandbyMaterial : this.standbyOverride);
	}

	// Token: 0x06000BCC RID: 3020 RVA: 0x0004060B File Offset: 0x0003E80B
	protected override void UnityOnEnable()
	{
		VODPlayer.OnCrash = (Action)Delegate.Combine(VODPlayer.OnCrash, new Action(this.VODPlayer_OnCrash));
		if (VODPlayer.state == VODPlayer.State.CRASHED)
		{
			this.staticScreen.SetActive(true);
		}
	}

	// Token: 0x06000BCD RID: 3021 RVA: 0x00040641 File Offset: 0x0003E841
	protected override void UnityOnDisable()
	{
		VODPlayer.OnCrash = (Action)Delegate.Remove(VODPlayer.OnCrash, new Action(this.VODPlayer_OnCrash));
	}

	// Token: 0x06000BCE RID: 3022 RVA: 0x00040641 File Offset: 0x0003E841
	private void OnDestroy()
	{
		VODPlayer.OnCrash = (Action)Delegate.Remove(VODPlayer.OnCrash, new Action(this.VODPlayer_OnCrash));
	}

	// Token: 0x06000BCF RID: 3023 RVA: 0x00040663 File Offset: 0x0003E863
	private void VODPlayer_OnCrash()
	{
		this.staticScreen.SetActive(true);
	}

	// Token: 0x06000BD0 RID: 3024 RVA: 0x00040674 File Offset: 0x0003E874
	protected override void ObservableSliceUpdate()
	{
		if (this.upNextData.Title.IsNullOrEmpty())
		{
			if (this.upNext.text.Length > 0)
			{
				this.upNext.text = string.Empty;
			}
			return;
		}
		if (GorillaComputer.instance == null)
		{
			return;
		}
		TimeSpan timeSpan = this.upNextData.StartTime - GorillaComputer.instance.GetServerTime();
		this.upNext.text = string.Format("next: {0} - {1:00}:{2:00}", this.upNextData.Title, timeSpan.Minutes, timeSpan.Seconds);
	}

	// Token: 0x06000BD1 RID: 3025 RVA: 0x0004071C File Offset: 0x0003E91C
	public void ShowStatic(bool on)
	{
		this.staticScreen.SetActive(on);
		if (on)
		{
			if (this.observable && VODTarget.AlertDisabled != null)
			{
				VODTarget.AlertDisabled(this);
				return;
			}
		}
		else if (this.observable && VODTarget.AlertEnabled != null)
		{
			VODTarget.AlertEnabled(this);
		}
	}

	// Token: 0x04000E51 RID: 3665
	[SerializeField]
	private Renderer targetRenderer;

	// Token: 0x04000E52 RID: 3666
	[SerializeField]
	private Material standbyOverride;

	// Token: 0x04000E53 RID: 3667
	[SerializeField]
	private VODTarget.VODTargetAudioSettings audioSettings;

	// Token: 0x04000E54 RID: 3668
	[SerializeField]
	private TMP_Text upNext;

	// Token: 0x04000E55 RID: 3669
	[SerializeField]
	private VODPlayer.VODStream.VODStreamChannel[] channel;

	// Token: 0x04000E56 RID: 3670
	[SerializeField]
	private GameObject staticScreen;

	// Token: 0x04000E57 RID: 3671
	public static Action<VODTarget> AlertEnabled;

	// Token: 0x04000E58 RID: 3672
	public static Action<VODTarget> AlertDisabled;

	// Token: 0x04000E59 RID: 3673
	private VODPlayer.VODNextStreamData upNextData;

	// Token: 0x020001B9 RID: 441
	[Serializable]
	public class VODTargetAudioSettings
	{
		// Token: 0x04000E5A RID: 3674
		[Range(0f, 1f)]
		public float volume;

		// Token: 0x04000E5B RID: 3675
		[Range(0f, 5f)]
		public float dopplerLevel = 1f;

		// Token: 0x04000E5C RID: 3676
		[Range(0f, 360f)]
		public float spread;

		// Token: 0x04000E5D RID: 3677
		public AudioRolloffMode rolloffMode;

		// Token: 0x04000E5E RID: 3678
		public float minDistance = 0.5f;

		// Token: 0x04000E5F RID: 3679
		public float maxDistance = 5f;
	}
}
