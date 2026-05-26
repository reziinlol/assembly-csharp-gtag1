using System;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x020005A3 RID: 1443
public class DebugAutoNamePlayer : MonoBehaviour
{
	// Token: 0x06002490 RID: 9360 RVA: 0x000028C5 File Offset: 0x00000AC5
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Init()
	{
	}

	// Token: 0x06002491 RID: 9361 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnEnable()
	{
	}

	// Token: 0x06002492 RID: 9362 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnDisable()
	{
	}

	// Token: 0x06002493 RID: 9363 RVA: 0x000C42EC File Offset: 0x000C24EC
	private void Update()
	{
	}

	// Token: 0x06002494 RID: 9364 RVA: 0x000C42F9 File Offset: 0x000C24F9
	private void OnRoomJoined()
	{
		this.m_lastZone = DebugAutoNamePlayer.GetPrimaryZone();
		this.m_lastIsZoneAuthority = DebugAutoNamePlayer.GetIsZoneAuthority(this.m_lastZone);
		this.m_joinDelayTimer = 2f;
		this.ApplyAutoName();
	}

	// Token: 0x06002495 RID: 9365 RVA: 0x000C4328 File Offset: 0x000C2528
	private void OnPlayersChanged()
	{
		if (!RoomSystem.JoinedRoom)
		{
			return;
		}
		this.ApplyAutoName();
	}

	// Token: 0x06002496 RID: 9366 RVA: 0x000C4338 File Offset: 0x000C2538
	private void OnZoneChange(ZoneData[] zones)
	{
		if (!RoomSystem.JoinedRoom)
		{
			return;
		}
		this.m_lastZone = DebugAutoNamePlayer.GetPrimaryZone();
		this.m_lastIsZoneAuthority = DebugAutoNamePlayer.GetIsZoneAuthority(this.m_lastZone);
		this.ApplyAutoName();
	}

	// Token: 0x06002497 RID: 9367 RVA: 0x000C4364 File Offset: 0x000C2564
	private void ApplyAutoName()
	{
		string platformCode = DebugAutoNamePlayer.GetPlatformCode();
		int localPlayerID = NetworkSystem.Instance.LocalPlayerID;
		string text = NetworkSystem.Instance.IsMasterClient ? "MC" : "C";
		GTZone primaryZone = DebugAutoNamePlayer.GetPrimaryZone();
		string text2 = DebugAutoNamePlayer.GetIsZoneAuthority(primaryZone) ? "ZA" : "Z";
		string text3 = primaryZone.ToString().ToUpper();
		string text4 = string.Format("{0}_{1}_{2}_{3}_{4}", new object[]
		{
			platformCode,
			localPlayerID,
			text,
			text2,
			text3
		});
		if (text4.Length > 20)
		{
			text4 = text4.Substring(0, 20);
		}
		NetworkSystem.Instance.SetMyNickName(text4);
		if (GorillaComputer.instance != null)
		{
			GorillaComputer.instance.currentName = text4;
			GorillaComputer.instance.savedName = text4;
			GorillaComputer.instance.SetLocalNameTagText(text4);
		}
		if (NetworkSystem.Instance.InRoom)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
			{
				PlayerPrefs.GetFloat("redValue", 0f),
				PlayerPrefs.GetFloat("greenValue", 0f),
				PlayerPrefs.GetFloat("blueValue", 0f)
			});
		}
	}

	// Token: 0x06002498 RID: 9368 RVA: 0x000C44C0 File Offset: 0x000C26C0
	private static GTZone GetPrimaryZone()
	{
		ZoneManagement instance = ZoneManagement.instance;
		if (instance != null && instance.activeZones.Count > 0)
		{
			return instance.activeZones[0];
		}
		return GTZone.forest;
	}

	// Token: 0x06002499 RID: 9369 RVA: 0x000C44F8 File Offset: 0x000C26F8
	private static bool GetIsZoneAuthority(GTZone zone)
	{
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(zone);
		if (managerForZone == null)
		{
			return false;
		}
		NetPlayer localPlayer = NetworkSystem.Instance.LocalPlayer;
		return localPlayer != null && managerForZone.IsAuthorityPlayer(localPlayer);
	}

	// Token: 0x0600249A RID: 9370 RVA: 0x000C452E File Offset: 0x000C272E
	private static string GetPlatformCode()
	{
		return "ST";
	}

	// Token: 0x04002FF3 RID: 12275
	private float m_authorityPollTimer;

	// Token: 0x04002FF4 RID: 12276
	private float m_joinDelayTimer;

	// Token: 0x04002FF5 RID: 12277
	private bool m_lastIsZoneAuthority;

	// Token: 0x04002FF6 RID: 12278
	private GTZone m_lastZone;
}
