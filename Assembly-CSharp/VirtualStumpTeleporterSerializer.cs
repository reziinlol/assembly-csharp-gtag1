using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000AAF RID: 2735
internal class VirtualStumpTeleporterSerializer : GorillaSerializer
{
	// Token: 0x060045FF RID: 17919 RVA: 0x0017AA4B File Offset: 0x00178C4B
	public void NotifyPlayerTeleporting(short teleporterIdx, AudioSource localPlayerTeleporterAudioSource)
	{
		if ((int)teleporterIdx >= this.teleporters.Count)
		{
			return;
		}
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("ActivateTeleportVFX", true, new object[]
			{
				false,
				teleporterIdx
			});
		}
	}

	// Token: 0x06004600 RID: 17920 RVA: 0x0017AA88 File Offset: 0x00178C88
	public void NotifyPlayerReturning(short teleporterIdx)
	{
		if ((int)teleporterIdx >= this.teleporters.Count)
		{
			return;
		}
		Debug.Log(string.Format("[VRTeleporterSerializer::NotifyPlayerReturning] Sending RPC to activate VFX at idx: {0}", teleporterIdx));
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("ActivateTeleportVFX", true, new object[]
			{
				true,
				teleporterIdx
			});
		}
	}

	// Token: 0x06004601 RID: 17921 RVA: 0x0017AAE4 File Offset: 0x00178CE4
	[PunRPC]
	private void ActivateTeleportVFX(bool returning, short teleporterIdx, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "ActivateTeleportVFX");
		if ((int)teleporterIdx >= this.teleporters.Count)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[13].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		VirtualStumpTeleporter virtualStumpTeleporter = this.teleporters[(int)teleporterIdx];
		if (virtualStumpTeleporter.IsNotNull())
		{
			virtualStumpTeleporter.PlayTeleportEffects(false, !returning, null, false);
		}
	}

	// Token: 0x06004602 RID: 17922 RVA: 0x0017AB74 File Offset: 0x00178D74
	public short GetTeleporterIndex(VirtualStumpTeleporter teleporter)
	{
		short num = 0;
		while ((int)num < this.teleporters.Count)
		{
			if (this.teleporters[(int)num] == teleporter)
			{
				return num;
			}
			num += 1;
		}
		return -1;
	}

	// Token: 0x04005865 RID: 22629
	[SerializeField]
	public List<VirtualStumpTeleporter> teleporters = new List<VirtualStumpTeleporter>();

	// Token: 0x04005866 RID: 22630
	[SerializeField]
	public List<ParticleSystem> teleporterVFX = new List<ParticleSystem>();

	// Token: 0x04005867 RID: 22631
	[SerializeField]
	public List<ParticleSystem> returnVFX = new List<ParticleSystem>();

	// Token: 0x04005868 RID: 22632
	[SerializeField]
	public List<AudioSource> teleportAudioSource = new List<AudioSource>();

	// Token: 0x04005869 RID: 22633
	[SerializeField]
	public List<AudioClip> teleportingPlayerSoundClips = new List<AudioClip>();

	// Token: 0x0400586A RID: 22634
	[SerializeField]
	public List<AudioClip> observerSoundClips = new List<AudioClip>();
}
