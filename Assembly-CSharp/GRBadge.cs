using System;
using System.Collections;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000743 RID: 1859
public class GRBadge : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x06002F26 RID: 12070 RVA: 0x0010109C File Offset: 0x000FF29C
	public void OnEntityInit()
	{
		this.gameEntity.manager.ghostReactorManager.reactor.employeeBadges.LinkBadgeToDispenser(this, (long)((int)this.gameEntity.createData));
	}

	// Token: 0x06002F27 RID: 12071 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002F28 RID: 12072 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002F29 RID: 12073 RVA: 0x001010CC File Offset: 0x000FF2CC
	private void OnDestroy()
	{
		GhostReactor ghostReactor = GhostReactor.Get(this.gameEntity);
		if (ghostReactor != null && ghostReactor.employeeBadges != null)
		{
			ghostReactor.employeeBadges.RemoveBadge(this);
		}
	}

	// Token: 0x06002F2A RID: 12074 RVA: 0x00101108 File Offset: 0x000FF308
	public void Setup(NetPlayer player, int index)
	{
		this.gameEntity.onlyGrabActorNumber = player.ActorNumber;
		this.dispenserIndex = index;
		this.actorNr = player.ActorNumber;
		GRPlayer grplayer = GRPlayer.Get(player.ActorNumber);
		bool flag = (int)this.gameEntity.GetState() == 1;
		if (player.IsLocal)
		{
			flag |= (Time.timeAsDouble < grplayer.lastLeftWithBadgeAttachedTime + 60.0);
		}
		if (grplayer != null && flag)
		{
			base.transform.position = grplayer.badgeBodyAnchor.position;
			grplayer.AttachBadge(this);
		}
		this.RefreshText(player);
	}

	// Token: 0x06002F2B RID: 12075 RVA: 0x001011A8 File Offset: 0x000FF3A8
	public void RefreshText(NetPlayer player)
	{
		this.playerName.text = player.SanitizedNickName;
		GRPlayer grplayer = GRPlayer.Get(player.ActorNumber);
		if (grplayer != null && this.lastRedeemedPoints != grplayer.CurrentProgression.redeemedPoints)
		{
			this.lastRedeemedPoints = grplayer.CurrentProgression.redeemedPoints;
			this.playerTitle.text = GhostReactorProgression.GetTitleName(grplayer.CurrentProgression.redeemedPoints);
			this.playerLevel.text = GhostReactorProgression.GetGrade(grplayer.CurrentProgression.redeemedPoints).ToString();
		}
	}

	// Token: 0x06002F2C RID: 12076 RVA: 0x00101240 File Offset: 0x000FF440
	public void Hide()
	{
		this.badgeMesh.enabled = false;
		this.playerName.gameObject.SetActive(false);
		this.playerTitle.gameObject.SetActive(false);
		this.playerLevel.gameObject.SetActive(false);
	}

	// Token: 0x06002F2D RID: 12077 RVA: 0x0010128C File Offset: 0x000FF48C
	public void UnHide()
	{
		this.badgeMesh.enabled = true;
		this.playerName.gameObject.SetActive(true);
		this.playerTitle.gameObject.SetActive(true);
		this.playerLevel.gameObject.SetActive(true);
	}

	// Token: 0x06002F2E RID: 12078 RVA: 0x001012D8 File Offset: 0x000FF4D8
	public bool IsAttachedToPlayer()
	{
		return (int)this.gameEntity.GetState() == 1;
	}

	// Token: 0x06002F2F RID: 12079 RVA: 0x001012EC File Offset: 0x000FF4EC
	public void StartRetracting()
	{
		this.gameEntity.RequestState(this.gameEntity.id, 1L);
		this.PlayAttachFx();
		if (this.retractCoroutine != null)
		{
			base.StopCoroutine(this.retractCoroutine);
		}
		this.retractCoroutine = base.StartCoroutine(this.RetractCoroutine());
	}

	// Token: 0x06002F30 RID: 12080 RVA: 0x0010133D File Offset: 0x000FF53D
	private IEnumerator RetractCoroutine()
	{
		base.transform.localRotation = Quaternion.identity;
		Vector3 vector = base.transform.localPosition;
		for (float sqrMagnitude = vector.sqrMagnitude; sqrMagnitude > 1E-05f; sqrMagnitude = vector.sqrMagnitude)
		{
			vector = Vector3.MoveTowards(vector, Vector3.zero, this.retractSpeed * Time.deltaTime);
			base.transform.localPosition = vector;
			yield return null;
			vector = base.transform.localPosition;
		}
		base.transform.localPosition = Vector3.zero;
		yield break;
	}

	// Token: 0x06002F31 RID: 12081 RVA: 0x0010134C File Offset: 0x000FF54C
	private void PlayAttachFx()
	{
		if (this.audioSource != null)
		{
			this.audioSource.volume = this.badgeAttachSoundVolume;
			this.audioSource.clip = this.badgeAttachSound;
			this.audioSource.Play();
		}
	}

	// Token: 0x04003C81 RID: 15489
	private const float RESTORE_BADGE_TO_DOCK_WINDOW = 60f;

	// Token: 0x04003C82 RID: 15490
	[SerializeField]
	private GameEntity gameEntity;

	// Token: 0x04003C83 RID: 15491
	[SerializeField]
	public TMP_Text playerName;

	// Token: 0x04003C84 RID: 15492
	[SerializeField]
	public TMP_Text playerTitle;

	// Token: 0x04003C85 RID: 15493
	[SerializeField]
	public TMP_Text playerLevel;

	// Token: 0x04003C86 RID: 15494
	[SerializeField]
	private MeshRenderer badgeMesh;

	// Token: 0x04003C87 RID: 15495
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003C88 RID: 15496
	[SerializeField]
	private float retractSpeed = 4f;

	// Token: 0x04003C89 RID: 15497
	[SerializeField]
	private AudioClip badgeAttachSound;

	// Token: 0x04003C8A RID: 15498
	[SerializeField]
	private float badgeAttachSoundVolume;

	// Token: 0x04003C8B RID: 15499
	[SerializeField]
	public int dispenserIndex;

	// Token: 0x04003C8C RID: 15500
	public int actorNr;

	// Token: 0x04003C8D RID: 15501
	private Coroutine retractCoroutine;

	// Token: 0x04003C8E RID: 15502
	private int lastRedeemedPoints = -1;

	// Token: 0x02000744 RID: 1860
	public enum BadgeState
	{
		// Token: 0x04003C90 RID: 15504
		AtDispenser,
		// Token: 0x04003C91 RID: 15505
		WithPlayer
	}
}
