using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000114 RID: 276
public class SIGadgetTapTeleporterDeployable : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x060006C6 RID: 1734 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Awake()
	{
	}

	// Token: 0x060006C7 RID: 1735 RVA: 0x00025E2F File Offset: 0x0002402F
	private void OnEnable()
	{
		this.activateTime = Time.time + this.activateDelay;
	}

	// Token: 0x060006C8 RID: 1736 RVA: 0x00025E44 File Offset: 0x00024044
	private void LateUpdate()
	{
		if (Time.time > this.timeToDie && this.gameEntity.IsAuthority())
		{
			if (this.linkedPoint != null)
			{
				this.linkedPoint.ClearLink();
			}
			this.gameEntity.manager.RequestDestroyItem(this.gameEntity.id);
		}
	}

	// Token: 0x060006C9 RID: 1737 RVA: 0x00025EA0 File Offset: 0x000240A0
	public void OnEntityInit()
	{
		int num;
		BitPackUtils.UnpackIntsFromLong(this.gameEntity.createData, out this.selectionId, out num);
		if ((float)num < 0f)
		{
			this.timeToDie = float.PositiveInfinity;
		}
		else
		{
			this.timeToDie = Time.time + (float)num;
		}
		this.UpdateSelectionDisplay();
	}

	// Token: 0x060006CA RID: 1738 RVA: 0x00025EEF File Offset: 0x000240EF
	private void UpdateSelectionDisplay()
	{
		if (this.selectionId == 0)
		{
			this.selectionColorDisplay.material = this.selectionColor1;
			return;
		}
		if (this.selectionId == 1)
		{
			this.selectionColorDisplay.material = this.selectionColor2;
		}
	}

	// Token: 0x060006CB RID: 1739 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060006CC RID: 1740 RVA: 0x00025F28 File Offset: 0x00024128
	public void OnEntityStateChange(long prevState, long newState)
	{
		if (this.gameEntity.IsAuthority())
		{
			return;
		}
		int netId;
		int netId2;
		BitPackUtils.UnpackIntsFromLong(newState, out netId, out netId2);
		GameEntity gameEntityFromNetId = this.gameEntity.manager.GetGameEntityFromNetId(netId);
		if (gameEntityFromNetId != null)
		{
			SIGadgetTapTeleporter component = gameEntityFromNetId.GetComponent<SIGadgetTapTeleporter>();
			this._pad = component;
			this.identifierColor = this._pad.identifierColor;
		}
		GameEntity gameEntityFromNetId2 = this.gameEntity.manager.GetGameEntityFromNetId(netId2);
		if (gameEntityFromNetId2 != null)
		{
			this.linkedPoint = gameEntityFromNetId2.GetComponent<SIGadgetTapTeleporterDeployable>();
			if (this.linkedPoint.linkedPoint == null)
			{
				this.linkedPoint.linkedPoint = this;
				this.linkedPoint._pad = this._pad;
				this.linkedPoint.identifierColor = this.identifierColor;
				this.linkedPoint.UpdateLinkDisplay();
			}
		}
		else
		{
			this.linkedPoint = null;
		}
		this.UpdateLinkDisplay();
	}

	// Token: 0x060006CD RID: 1741 RVA: 0x0002600C File Offset: 0x0002420C
	public void SetLink(SIGadgetTapTeleporter newPad, SIGadgetTapTeleporterDeployable newLink)
	{
		this._pad = newPad;
		this.linkedPoint = newLink;
		this.identifierColor = this._pad.identifierColor;
		int value = -1;
		if (this.linkedPoint != null)
		{
			value = this.linkedPoint.gameEntity.GetNetId();
		}
		this.gameEntity.RequestState(this.gameEntity.id, BitPackUtils.PackIntsIntoLong(this._pad.gameEntity.GetNetId(), value));
		this.UpdateLinkDisplay();
		this.stealth.enabled = this._pad.useStealthTeleporters;
		this.maintainVelocity = this._pad.isVelocityPreserved;
	}

	// Token: 0x060006CE RID: 1742 RVA: 0x000260B2 File Offset: 0x000242B2
	private void ClearLink()
	{
		this.linkedPoint = null;
		this.gameEntity.RequestState(this.gameEntity.id, BitPackUtils.PackIntsIntoLong(this._pad.gameEntity.GetNetId(), -1));
		this.UpdateLinkDisplay();
	}

	// Token: 0x060006CF RID: 1743 RVA: 0x000260F0 File Offset: 0x000242F0
	private void UpdateLinkDisplay()
	{
		Renderer[] array = this.identifierColorDisplay;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material.color = this.identifierColor;
		}
		if (this.linkedPoint != null)
		{
			Vector3 vector = this.linkedPoint.transform.position - base.transform.position;
			this.linkDirectionIndicator.gameObject.SetActive(true);
			this.linkDirectionIndicator.transform.rotation = Quaternion.LookRotation(base.transform.forward, vector.normalized);
			return;
		}
		this.linkDirectionIndicator.gameObject.SetActive(false);
	}

	// Token: 0x060006D0 RID: 1744 RVA: 0x0002619E File Offset: 0x0002439E
	public void TryTeleport()
	{
		if (this.activateTime < Time.time && SIGadgetTapTeleporterDeployable.reteleportTime < Time.time && (!this.requiresSurfaceTapSinceTeleport || GorillaTagger.Instance.hasTappedSurface))
		{
			this.TeleportToLinked();
		}
	}

	// Token: 0x060006D1 RID: 1745 RVA: 0x000261D3 File Offset: 0x000243D3
	private void ResetRetriggerBlock()
	{
		SIGadgetTapTeleporterDeployable.reteleportTime = Time.time + SIGadgetTapTeleporterDeployable.reteleportDelay;
	}

	// Token: 0x060006D2 RID: 1746 RVA: 0x000261E8 File Offset: 0x000243E8
	private void TeleportToLinked()
	{
		if (this.linkedPoint == null || !this.linkedPoint.gameObject.activeSelf)
		{
			return;
		}
		Vector3 position = this.destination.position;
		if (Vector3.Distance(GTPlayer.Instance.transform.position, position) > this.teleportCheckDistance)
		{
			return;
		}
		this.ResetRetriggerBlock();
		if (this.requiresSurfaceTapSinceTeleport)
		{
			GorillaTagger.Instance.ResetTappedSurfaceCheck();
		}
		Vector3 position2 = this.linkedPoint.destination.position;
		Quaternion rotation = GTPlayer.Instance.transform.rotation;
		GTPlayer.Instance.TeleportTo(position2, rotation, this.maintainVelocity, true);
		this.linkedPoint.teleportSoundbank.Play();
	}

	// Token: 0x0400084C RID: 2124
	public GameEntity gameEntity;

	// Token: 0x0400084D RID: 2125
	[SerializeField]
	private Transform destination;

	// Token: 0x0400084E RID: 2126
	[SerializeField]
	private Renderer[] identifierColorDisplay;

	// Token: 0x0400084F RID: 2127
	[SerializeField]
	private Transform linkDirectionIndicator;

	// Token: 0x04000850 RID: 2128
	[SerializeField]
	private Renderer selectionColorDisplay;

	// Token: 0x04000851 RID: 2129
	[SerializeField]
	private Material selectionColor1;

	// Token: 0x04000852 RID: 2130
	[SerializeField]
	private Material selectionColor2;

	// Token: 0x04000853 RID: 2131
	[SerializeField]
	private SoundBankPlayer teleportSoundbank;

	// Token: 0x04000854 RID: 2132
	[SerializeField]
	private SIGameEntityStealthVisibility stealth;

	// Token: 0x04000855 RID: 2133
	[SerializeField]
	private bool requiresSurfaceTapSinceTeleport;

	// Token: 0x04000856 RID: 2134
	private bool maintainVelocity;

	// Token: 0x04000857 RID: 2135
	private int selectionId;

	// Token: 0x04000858 RID: 2136
	private SIGadgetTapTeleporter _pad;

	// Token: 0x04000859 RID: 2137
	private SIGadgetTapTeleporterDeployable linkedPoint;

	// Token: 0x0400085A RID: 2138
	private float activateDelay = 0.3f;

	// Token: 0x0400085B RID: 2139
	private float activateTime;

	// Token: 0x0400085C RID: 2140
	private static float reteleportDelay = 0.3f;

	// Token: 0x0400085D RID: 2141
	private static float reteleportTime;

	// Token: 0x0400085E RID: 2142
	private Color identifierColor;

	// Token: 0x0400085F RID: 2143
	private float timeToDie = -1f;

	// Token: 0x04000860 RID: 2144
	private float teleportCheckDistance = 2f;
}
