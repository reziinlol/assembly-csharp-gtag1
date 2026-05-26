using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020002BD RID: 701
public class DJScratchSoundPlayer : MonoBehaviour, ISpawnable
{
	// Token: 0x170001BD RID: 445
	// (get) Token: 0x06001212 RID: 4626 RVA: 0x00060D35 File Offset: 0x0005EF35
	// (set) Token: 0x06001213 RID: 4627 RVA: 0x00060D3D File Offset: 0x0005EF3D
	public bool IsSpawned { get; set; }

	// Token: 0x170001BE RID: 446
	// (get) Token: 0x06001214 RID: 4628 RVA: 0x00060D46 File Offset: 0x0005EF46
	// (set) Token: 0x06001215 RID: 4629 RVA: 0x00060D4E File Offset: 0x0005EF4E
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06001216 RID: 4630 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnDespawn()
	{
	}

	// Token: 0x06001217 RID: 4631 RVA: 0x00060D58 File Offset: 0x0005EF58
	private void OnEnable()
	{
		if (this._events.IsNull())
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			NetPlayer netPlayer = (this.myRig != null) ? ((this.myRig.creator != null) ? this.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null;
			if (netPlayer != null)
			{
				this._events.Init(netPlayer);
			}
		}
		this._events.Activate += this.OnPlayEvent;
	}

	// Token: 0x06001218 RID: 4632 RVA: 0x00060DEC File Offset: 0x0005EFEC
	private void OnDisable()
	{
		if (this._events.IsNotNull())
		{
			this._events.Activate -= this.OnPlayEvent;
			this._events.Dispose();
			this._events = null;
		}
	}

	// Token: 0x06001219 RID: 4633 RVA: 0x00060E3A File Offset: 0x0005F03A
	public void OnSpawn(VRRig rig)
	{
		this.myRig = rig;
		if (!rig.isLocal)
		{
			this.scratchTableLeft.enabled = false;
			this.scratchTableRight.enabled = false;
		}
	}

	// Token: 0x0600121A RID: 4634 RVA: 0x00060E63 File Offset: 0x0005F063
	public void Play(ScratchSoundType type, bool isLeft)
	{
		if (this.myRig.isLocal)
		{
			this.PlayLocal(type, isLeft);
			this._events.Activate.RaiseOthers(new object[]
			{
				(int)(type + (isLeft ? 100 : 0))
			});
		}
	}

	// Token: 0x0600121B RID: 4635 RVA: 0x00060EA4 File Offset: 0x0005F0A4
	public void OnPlayEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != target)
		{
			return;
		}
		if (info.senderID != this.myRig.creator.ActorNumber)
		{
			return;
		}
		if (args.Length != 1)
		{
			Debug.LogError(string.Format("Invalid DJ Scratch Event - expected 1 arg, got {0}", args.Length));
			return;
		}
		int num = (int)args[0];
		bool flag = num >= 100;
		if (flag)
		{
			num -= 100;
		}
		ScratchSoundType scratchSoundType = (ScratchSoundType)num;
		if (scratchSoundType < ScratchSoundType.Pause || scratchSoundType > ScratchSoundType.Back)
		{
			return;
		}
		this.PlayLocal(scratchSoundType, flag);
	}

	// Token: 0x0600121C RID: 4636 RVA: 0x00060F1C File Offset: 0x0005F11C
	public void PlayLocal(ScratchSoundType type, bool isLeft)
	{
		switch (type)
		{
		case ScratchSoundType.Pause:
			(isLeft ? this.scratchTableLeft : this.scratchTableRight).PauseTrack();
			this.scratchPause.Play();
			return;
		case ScratchSoundType.Resume:
			(isLeft ? this.scratchTableLeft : this.scratchTableRight).ResumeTrack();
			this.scratchResume.Play();
			return;
		case ScratchSoundType.Forward:
			this.scratchForward.Play();
			(isLeft ? this.scratchTableLeft : this.scratchTableRight).PauseTrack();
			return;
		case ScratchSoundType.Back:
			this.scratchBack.Play();
			(isLeft ? this.scratchTableLeft : this.scratchTableRight).PauseTrack();
			return;
		default:
			return;
		}
	}

	// Token: 0x040015E0 RID: 5600
	[SerializeField]
	private SoundBankPlayer scratchForward;

	// Token: 0x040015E1 RID: 5601
	[SerializeField]
	private SoundBankPlayer scratchBack;

	// Token: 0x040015E2 RID: 5602
	[SerializeField]
	private SoundBankPlayer scratchPause;

	// Token: 0x040015E3 RID: 5603
	[SerializeField]
	private SoundBankPlayer scratchResume;

	// Token: 0x040015E4 RID: 5604
	[SerializeField]
	private DJScratchtable scratchTableLeft;

	// Token: 0x040015E5 RID: 5605
	[SerializeField]
	private DJScratchtable scratchTableRight;

	// Token: 0x040015E6 RID: 5606
	private RubberDuckEvents _events;

	// Token: 0x040015E7 RID: 5607
	private VRRig myRig;
}
