using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200079F RID: 1951
public class GRFirstTimeUserExperience : MonoBehaviour
{
	// Token: 0x060031E3 RID: 12771 RVA: 0x00102C83 File Offset: 0x00100E83
	[ContextMenu("Set Player Pref")]
	private void RemovePlayerPref()
	{
		PlayerPrefs.SetString("spawnInWrongStump", "flagged");
		PlayerPrefs.Save();
	}

	// Token: 0x060031E4 RID: 12772 RVA: 0x00111EBC File Offset: 0x001100BC
	private void OnEnable()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.flickerSphere.SetActive(false);
		this.logoQuad.SetActive(false);
		this.flickerSphereOrigParent = this.flickerSphere.transform.parent;
		GameLightingManager.instance.SetCustomDynamicLightingEnabled(true);
		this.playerLight = GorillaTagger.Instance.mainCamera.GetComponentInChildren<GameLight>(true);
		this.playerLight.gameObject.SetActive(true);
		this.ChangeState(GRFirstTimeUserExperience.TransitionState.Waiting);
	}

	// Token: 0x060031E5 RID: 12773 RVA: 0x00111F40 File Offset: 0x00110140
	public void ChangeState(GRFirstTimeUserExperience.TransitionState state)
	{
		this.transitionState = state;
		switch (state)
		{
		case GRFirstTimeUserExperience.TransitionState.Waiting:
			this.stateStartTime = Time.time;
			return;
		case GRFirstTimeUserExperience.TransitionState.Flicker:
			this.transitionState = GRFirstTimeUserExperience.TransitionState.Flicker;
			this.flickerSphere.transform.SetParent(GTPlayer.Instance.headCollider.transform, false);
			this.flickerSphere.SetActive(true);
			this.logoQuad.SetActive(false);
			this.stateStartTime = Time.time;
			return;
		case GRFirstTimeUserExperience.TransitionState.Logo:
			this.stateStartTime = Time.time;
			this.flickerSphere.SetActive(true);
			this.logoQuad.SetActive(true);
			return;
		case GRFirstTimeUserExperience.TransitionState.ZoneLoad:
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.OnSceneLoadsCompleted = (Action)Delegate.Combine(instance.OnSceneLoadsCompleted, new Action(this.OnZoneLoadComplete));
			ZoneManagement.SetActiveZone(this.teleportZone);
			return;
		}
		case GRFirstTimeUserExperience.TransitionState.Teleport:
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this.joinRoomTrigger, JoinType.Solo, null, false);
			GTPlayer.Instance.TeleportTo(this.teleportLocation.position, this.teleportLocation.rotation, false, false);
			GTPlayer.Instance.InitializeValues();
			this.stateStartTime = Time.time;
			return;
		case GRFirstTimeUserExperience.TransitionState.Exit:
			this.flickerSphere.transform.SetParent(this.flickerSphereOrigParent, false);
			this.flickerSphere.SetActive(false);
			this.logoQuad.SetActive(false);
			this.rootObject.SetActive(false);
			GorillaTagger.Instance.mainCamera.GetComponentInChildren<GameLight>(true).gameObject.SetActive(false);
			return;
		default:
			return;
		}
	}

	// Token: 0x060031E6 RID: 12774 RVA: 0x001120C3 File Offset: 0x001102C3
	private void OnZoneLoadComplete()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.OnSceneLoadsCompleted = (Action)Delegate.Remove(instance.OnSceneLoadsCompleted, new Action(this.OnZoneLoadComplete));
		this.ChangeState(GRFirstTimeUserExperience.TransitionState.Teleport);
	}

	// Token: 0x060031E7 RID: 12775 RVA: 0x001120F4 File Offset: 0x001102F4
	public void InterruptWaitingTimer()
	{
		this.stateStartTime = -1f;
		for (int i = 0; i < this.delayObjects.Count; i++)
		{
			this.delayObjects[i].enabledTime = this.stateStartTime;
		}
	}

	// Token: 0x060031E8 RID: 12776 RVA: 0x0011213C File Offset: 0x0011033C
	private void Update()
	{
		switch (this.transitionState)
		{
		case GRFirstTimeUserExperience.TransitionState.Waiting:
			if (PrivateUIRoom.GetInOverlay())
			{
				if (this.stateStartTime >= 0f)
				{
					this.InterruptWaitingTimer();
				}
			}
			else if (this.stateStartTime < 0f)
			{
				this.stateStartTime = Time.time;
			}
			if (this.stateStartTime >= 0f && Time.time - this.stateStartTime >= this.transitionDelay)
			{
				this.ChangeState(GRFirstTimeUserExperience.TransitionState.Flicker);
				return;
			}
			break;
		case GRFirstTimeUserExperience.TransitionState.Flicker:
		{
			float num = Time.time - this.stateStartTime;
			if (this.stateStartTime >= 0f && num >= this.flickerDuration)
			{
				this.ChangeState(GRFirstTimeUserExperience.TransitionState.Logo);
				return;
			}
			bool flag = this.flickerTimeline.Evaluate(num / this.flickerDuration) < 0f;
			this.flickerSphere.SetActive(flag);
			if (flag && !this.flickerLightWasOff)
			{
				if (this.audioSource != null && this.flickerAudioCount < this.flickerAudio.Count && this.flickerAudio[this.flickerAudioCount] != null)
				{
					this.audioSource.PlayOneShot(this.flickerAudio[this.flickerAudioCount]);
				}
				this.flickerAudioCount++;
			}
			this.flickerLightWasOff = flag;
			return;
		}
		case GRFirstTimeUserExperience.TransitionState.Logo:
			if (this.stateStartTime >= 0f && Time.time - this.stateStartTime >= this.logoDisplayTime)
			{
				this.ChangeState(GRFirstTimeUserExperience.TransitionState.ZoneLoad);
				return;
			}
			break;
		case GRFirstTimeUserExperience.TransitionState.ZoneLoad:
			break;
		case GRFirstTimeUserExperience.TransitionState.Teleport:
			if (this.stateStartTime >= 0f && Time.time - this.stateStartTime >= this.teleportSettleTime)
			{
				this.ChangeState(GRFirstTimeUserExperience.TransitionState.Exit);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x040040B6 RID: 16566
	public Transform spawnPoint;

	// Token: 0x040040B7 RID: 16567
	public GameObject rootObject;

	// Token: 0x040040B8 RID: 16568
	public GameObject flickerSphere;

	// Token: 0x040040B9 RID: 16569
	public GameObject logoQuad;

	// Token: 0x040040BA RID: 16570
	public AnimationCurve flickerTimeline;

	// Token: 0x040040BB RID: 16571
	public float flickerDuration = 3f;

	// Token: 0x040040BC RID: 16572
	public GTZone teleportZone = GTZone.none;

	// Token: 0x040040BD RID: 16573
	public Transform teleportLocation;

	// Token: 0x040040BE RID: 16574
	public float transitionDelay = 60f;

	// Token: 0x040040BF RID: 16575
	public float logoDisplayTime = 4f;

	// Token: 0x040040C0 RID: 16576
	public float teleportSettleTime = 1f;

	// Token: 0x040040C1 RID: 16577
	public GorillaNetworkJoinTrigger joinRoomTrigger;

	// Token: 0x040040C2 RID: 16578
	public List<AudioClip> flickerAudio = new List<AudioClip>();

	// Token: 0x040040C3 RID: 16579
	public List<DisableGameObjectDelayed> delayObjects;

	// Token: 0x040040C4 RID: 16580
	private Transform flickerSphereOrigParent;

	// Token: 0x040040C5 RID: 16581
	private float stateStartTime = -1f;

	// Token: 0x040040C6 RID: 16582
	private bool flickerLightWasOff;

	// Token: 0x040040C7 RID: 16583
	private int flickerAudioCount;

	// Token: 0x040040C8 RID: 16584
	private AudioSource audioSource;

	// Token: 0x040040C9 RID: 16585
	private GRFirstTimeUserExperience.TransitionState transitionState;

	// Token: 0x040040CA RID: 16586
	public GameLight playerLight;

	// Token: 0x020007A0 RID: 1952
	public enum TransitionState
	{
		// Token: 0x040040CC RID: 16588
		Waiting,
		// Token: 0x040040CD RID: 16589
		Flicker,
		// Token: 0x040040CE RID: 16590
		Logo,
		// Token: 0x040040CF RID: 16591
		ZoneLoad,
		// Token: 0x040040D0 RID: 16592
		Teleport,
		// Token: 0x040040D1 RID: 16593
		Exit
	}
}
