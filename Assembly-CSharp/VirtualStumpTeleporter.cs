using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Modio.Mods;
using TMPro;
using UnityEngine;

// Token: 0x02000AAE RID: 2734
public class VirtualStumpTeleporter : MonoBehaviour, IBuildValidation, IGorillaSliceableSimple
{
	// Token: 0x060045E6 RID: 17894 RVA: 0x0017A265 File Offset: 0x00178465
	public bool BuildValidationCheck()
	{
		if (this.netSerializer.IsNull())
		{
			Debug.LogError("VStump Teleporter \"" + base.gameObject.GetPath() + "\" needs a reference to a VirtualStumpTeleporterSerializer for networked FX to function. Check out the teleporter prefabs in arcade or the stump", this);
			return false;
		}
		return true;
	}

	// Token: 0x060045E7 RID: 17895 RVA: 0x0017A298 File Offset: 0x00178498
	public void SliceUpdate()
	{
		if (!this.accessDenied && NetworkSystem.Instance.netState != NetSystemState.Idle && NetworkSystem.Instance.netState != NetSystemState.InGame)
		{
			this.DenyAccess();
		}
		if (this.accessDenied && (NetworkSystem.Instance.netState == NetSystemState.Idle || NetworkSystem.Instance.netState == NetSystemState.InGame) && !UGCPermissionManager.IsUGCDisabled)
		{
			this.AllowAccess();
		}
	}

	// Token: 0x060045E8 RID: 17896 RVA: 0x0017A2FC File Offset: 0x001784FC
	public void OnEnable()
	{
		if (this.netSerializer.IsNull())
		{
			Debug.LogWarning("[VStumpTeleporter.OnEnable] Net Serializer is null for \"" + base.gameObject.GetPath() + "\", networked teleport FX will not function.");
		}
		if (UGCPermissionManager.IsUGCDisabled || (NetworkSystem.Instance.netState != NetSystemState.Idle && NetworkSystem.Instance.netState != NetSystemState.InGame))
		{
			ushort num = VirtualStumpTeleporter.lastLoggingHandsMsgId;
			VirtualStumpTeleporter.lastLoggingHandsMsgId = 1;
			this.DenyAccess();
		}
		else
		{
			ushort num2 = VirtualStumpTeleporter.lastLoggingHandsMsgId;
			VirtualStumpTeleporter.lastLoggingHandsMsgId = 2;
			this.AllowAccess();
		}
		UGCPermissionManager.SubscribeToUGCEnabled(new Action(this.OnUGCEnabled));
		UGCPermissionManager.SubscribeToUGCDisabled(new Action(this.OnUGCDisabled));
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060045E9 RID: 17897 RVA: 0x0017A3A9 File Offset: 0x001785A9
	public void OnDisable()
	{
		this.AllowAccess();
		UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
		UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060045EA RID: 17898 RVA: 0x0017A3DB File Offset: 0x001785DB
	private void OnUGCEnabled()
	{
		this.AllowAccess();
		ushort num = VirtualStumpTeleporter.lastLoggingHandsMsgId;
		VirtualStumpTeleporter.lastLoggingHandsMsgId = 3;
	}

	// Token: 0x060045EB RID: 17899 RVA: 0x0017A3F1 File Offset: 0x001785F1
	private void OnUGCDisabled()
	{
		this.DenyAccess();
		ushort num = VirtualStumpTeleporter.lastLoggingHandsMsgId;
		VirtualStumpTeleporter.lastLoggingHandsMsgId = 4;
	}

	// Token: 0x060045EC RID: 17900 RVA: 0x0017A408 File Offset: 0x00178608
	public void OnTriggerEnter(Collider other)
	{
		if (UGCPermissionManager.IsUGCDisabled || this.accessDenied || this.teleporting || CustomMapManager.WaitingForRoomJoin || CustomMapManager.WaitingForDisconnect)
		{
			return;
		}
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			this.triggerEntryTime = Time.time;
			this.ShowCountdownText();
		}
	}

	// Token: 0x060045ED RID: 17901 RVA: 0x0017A468 File Offset: 0x00178668
	public void OnTriggerStay(Collider other)
	{
		if (UGCPermissionManager.IsUGCDisabled || this.accessDenied)
		{
			return;
		}
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject && this.triggerEntryTime >= 0f)
		{
			this.UpdateCountdownText();
			if (!this.teleporting && this.triggerEntryTime + this.stayInTriggerDuration <= Time.time)
			{
				this.TeleportPlayer();
				this.HideCountdownText();
			}
		}
	}

	// Token: 0x060045EE RID: 17902 RVA: 0x0017A4DC File Offset: 0x001786DC
	public void OnTriggerExit(Collider other)
	{
		if (UGCPermissionManager.IsUGCDisabled || this.accessDenied)
		{
			return;
		}
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			this.triggerEntryTime = -1f;
			this.HideCountdownText();
		}
	}

	// Token: 0x060045EF RID: 17903 RVA: 0x0017A51C File Offset: 0x0017871C
	private void ShowCountdownText()
	{
		if (UGCPermissionManager.IsUGCDisabled || this.accessDenied)
		{
			return;
		}
		if (!this.countdownTexts.IsNullOrEmpty<TMP_Text>())
		{
			int num = 1 + Mathf.FloorToInt(this.stayInTriggerDuration);
			for (int i = 0; i < this.countdownTexts.Length; i++)
			{
				if (!this.countdownTexts[i].IsNull())
				{
					this.countdownTexts[i].text = num.ToString();
					this.countdownTexts[i].gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x060045F0 RID: 17904 RVA: 0x0017A5A0 File Offset: 0x001787A0
	private void HideCountdownText()
	{
		if (!this.countdownTexts.IsNullOrEmpty<TMP_Text>())
		{
			for (int i = 0; i < this.countdownTexts.Length; i++)
			{
				if (!this.countdownTexts[i].IsNull())
				{
					this.countdownTexts[i].text = "";
					this.countdownTexts[i].gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x060045F1 RID: 17905 RVA: 0x0017A604 File Offset: 0x00178804
	private void UpdateCountdownText()
	{
		if (UGCPermissionManager.IsUGCDisabled || this.accessDenied)
		{
			return;
		}
		if (!this.countdownTexts.IsNullOrEmpty<TMP_Text>())
		{
			float f = this.stayInTriggerDuration - (Time.time - this.triggerEntryTime);
			int num = 1 + Mathf.FloorToInt(f);
			for (int i = 0; i < this.countdownTexts.Length; i++)
			{
				if (!this.countdownTexts[i].IsNull())
				{
					this.countdownTexts[i].text = num.ToString();
				}
			}
		}
	}

	// Token: 0x060045F2 RID: 17906 RVA: 0x0017A681 File Offset: 0x00178881
	public void TeleportPlayer()
	{
		if (UGCPermissionManager.IsUGCDisabled || this.accessDenied)
		{
			return;
		}
		if (!this.teleporting)
		{
			this.teleporting = true;
			CustomMapManager.TeleportToVirtualStump(this, new Action<bool>(this.FinishTeleport));
		}
	}

	// Token: 0x060045F3 RID: 17907 RVA: 0x0017A6B4 File Offset: 0x001788B4
	private void FinishTeleport(bool success = true)
	{
		if (this.teleporting)
		{
			this.teleporting = false;
			this.triggerEntryTime = -1f;
		}
	}

	// Token: 0x060045F4 RID: 17908 RVA: 0x0017A6D0 File Offset: 0x001788D0
	private void DenyAccess()
	{
		this.accessDenied = true;
		foreach (GameObject gameObject in this.accessDeniedEnabledObjects)
		{
			gameObject.SetActive(true);
		}
		foreach (GameObject gameObject2 in this.accessDeniedDisabledObjects)
		{
			gameObject2.SetActive(false);
		}
	}

	// Token: 0x060045F5 RID: 17909 RVA: 0x0017A768 File Offset: 0x00178968
	private void AllowAccess()
	{
		if (UGCPermissionManager.IsUGCDisabled)
		{
			return;
		}
		this.accessDenied = false;
		foreach (GameObject gameObject in this.accessDeniedEnabledObjects)
		{
			gameObject.SetActive(false);
		}
		foreach (GameObject gameObject2 in this.accessDeniedDisabledObjects)
		{
			gameObject2.SetActive(true);
		}
	}

	// Token: 0x060045F6 RID: 17910 RVA: 0x0017A808 File Offset: 0x00178A08
	private short GetIndex()
	{
		if (!this.netSerializer.IsNotNull())
		{
			return -1;
		}
		return this.netSerializer.GetTeleporterIndex(this);
	}

	// Token: 0x060045F7 RID: 17911 RVA: 0x0017A825 File Offset: 0x00178A25
	public GTZone GetZone()
	{
		return this.entranceZone;
	}

	// Token: 0x060045F8 RID: 17912 RVA: 0x0017A82D File Offset: 0x00178A2D
	public GorillaNetworkJoinTrigger GetExitVStumpJoinTrigger()
	{
		return this.exitVStumpJoinTrigger;
	}

	// Token: 0x060045F9 RID: 17913 RVA: 0x0017A835 File Offset: 0x00178A35
	public Transform GetReturnTransform()
	{
		return this.returnLocation;
	}

	// Token: 0x060045FA RID: 17914 RVA: 0x0017A83D File Offset: 0x00178A3D
	public long GetAutoLoadMapModId()
	{
		return this.autoLoadMapModId;
	}

	// Token: 0x060045FB RID: 17915 RVA: 0x0017A845 File Offset: 0x00178A45
	public GameModeType GetAutoLoadGamemode()
	{
		return this.autoLoadGamemode;
	}

	// Token: 0x060045FC RID: 17916 RVA: 0x0017A84D File Offset: 0x00178A4D
	public GameModeType GetReturnGamemode()
	{
		return this.forcedGamemodeUponReturn;
	}

	// Token: 0x060045FD RID: 17917 RVA: 0x0017A858 File Offset: 0x00178A58
	public void PlayTeleportEffects(bool forLocalPlayer, bool toVStump, AudioSource vStumpSFXAudioSource = null, bool sendRPC = false)
	{
		if (sendRPC && this.netSerializer.IsNotNull())
		{
			this.netSerializer.NotifyPlayerTeleporting(this.GetIndex(), vStumpSFXAudioSource);
		}
		ParticleSystem particleSystem;
		if (toVStump)
		{
			particleSystem = this.teleportToVStumpVFX;
			if (forLocalPlayer && vStumpSFXAudioSource.IsNotNull() && !this.teleportingPlayerSoundClips.IsNullOrEmpty<AudioClip>())
			{
				vStumpSFXAudioSource.clip = this.teleportingPlayerSoundClips[Random.Range(0, this.teleportingPlayerSoundClips.Count)];
				vStumpSFXAudioSource.Play();
			}
			if (!forLocalPlayer && this.teleporterSFXAudioSource.IsNotNull() && !this.observerSoundClips.IsNullOrEmpty<AudioClip>())
			{
				this.teleporterSFXAudioSource.clip = this.observerSoundClips[Random.Range(0, this.observerSoundClips.Count)];
				this.teleporterSFXAudioSource.Play();
			}
		}
		else
		{
			particleSystem = this.returnFromVStumpVFX;
			if (this.teleporterSFXAudioSource.IsNotNull())
			{
				if (forLocalPlayer && !this.teleportingPlayerSoundClips.IsNullOrEmpty<AudioClip>())
				{
					this.teleporterSFXAudioSource.clip = this.teleportingPlayerSoundClips[Random.Range(0, this.teleportingPlayerSoundClips.Count)];
				}
				else if (!forLocalPlayer && !this.observerSoundClips.IsNullOrEmpty<AudioClip>())
				{
					this.teleporterSFXAudioSource.clip = this.observerSoundClips[Random.Range(0, this.observerSoundClips.Count)];
				}
				this.teleporterSFXAudioSource.Play();
			}
		}
		if (particleSystem.IsNotNull())
		{
			particleSystem.Play();
		}
	}

	// Token: 0x0400584F RID: 22607
	[SerializeField]
	private float stayInTriggerDuration = 3f;

	// Token: 0x04005850 RID: 22608
	[SerializeField]
	private TMP_Text[] countdownTexts;

	// Token: 0x04005851 RID: 22609
	[SerializeField]
	private GameObject[] handHoldObjects;

	// Token: 0x04005852 RID: 22610
	[SerializeField]
	private List<GameObject> accessDeniedDisabledObjects = new List<GameObject>();

	// Token: 0x04005853 RID: 22611
	[SerializeField]
	private List<GameObject> accessDeniedEnabledObjects = new List<GameObject>();

	// Token: 0x04005854 RID: 22612
	[SerializeField]
	private Transform returnLocation;

	// Token: 0x04005855 RID: 22613
	[SerializeField]
	private GTZone entranceZone = GTZone.arcade;

	// Token: 0x04005856 RID: 22614
	[SerializeField]
	private GorillaNetworkJoinTrigger exitVStumpJoinTrigger;

	// Token: 0x04005857 RID: 22615
	[SerializeField]
	private long autoLoadMapModId = ModId.Null;

	// Token: 0x04005858 RID: 22616
	[SerializeField]
	private GameModeType autoLoadGamemode = GameModeType.None;

	// Token: 0x04005859 RID: 22617
	[SerializeField]
	private GameModeType forcedGamemodeUponReturn = GameModeType.None;

	// Token: 0x0400585A RID: 22618
	[SerializeField]
	private ParticleSystem teleportToVStumpVFX;

	// Token: 0x0400585B RID: 22619
	[SerializeField]
	private ParticleSystem returnFromVStumpVFX;

	// Token: 0x0400585C RID: 22620
	[SerializeField]
	private AudioSource teleporterSFXAudioSource;

	// Token: 0x0400585D RID: 22621
	[SerializeField]
	private List<AudioClip> teleportingPlayerSoundClips = new List<AudioClip>();

	// Token: 0x0400585E RID: 22622
	[SerializeField]
	private List<AudioClip> observerSoundClips = new List<AudioClip>();

	// Token: 0x0400585F RID: 22623
	[SerializeField]
	private VirtualStumpTeleporterSerializer netSerializer;

	// Token: 0x04005860 RID: 22624
	private VirtualStumpTeleporterSerializer mySerializer;

	// Token: 0x04005861 RID: 22625
	private bool accessDenied;

	// Token: 0x04005862 RID: 22626
	private bool teleporting;

	// Token: 0x04005863 RID: 22627
	private float triggerEntryTime = -1f;

	// Token: 0x04005864 RID: 22628
	[OnEnterPlay_Set(0)]
	private static ushort lastLoggingHandsMsgId;
}
