using System;
using System.Collections;
using System.Linq;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using UnityEngine;

// Token: 0x0200087E RID: 2174
public class Gorillanalytics : MonoBehaviour
{
	// Token: 0x0600389F RID: 14495 RVA: 0x00135872 File Offset: 0x00133A72
	private IEnumerator Start()
	{
		PlayFabTitleDataCache.Instance.GetTitleData("GorillanalyticsChance", delegate(string s)
		{
			double num;
			if (double.TryParse(s, out num))
			{
				this.oneOverChance = num;
			}
		}, delegate(PlayFabError e)
		{
		}, false);
		for (;;)
		{
			yield return new WaitForSecondsRealtime(this.interval);
			if ((double)Random.Range(0f, 1f) < 1.0 / this.oneOverChance && PlayFabClientAPI.IsClientLoggedIn())
			{
				this.UploadGorillanalytics();
			}
		}
		yield break;
	}

	// Token: 0x060038A0 RID: 14496 RVA: 0x00135884 File Offset: 0x00133A84
	private void UploadGorillanalytics()
	{
		try
		{
			string map;
			string mode;
			string queue;
			this.GetMapModeQueue(out map, out mode, out queue);
			Vector3 position = GTPlayer.Instance.headCollider.transform.position;
			Vector3 averagedVelocity = GTPlayer.Instance.AveragedVelocity;
			this.uploadData.version = NetworkSystemConfig.AppVersion;
			this.uploadData.upload_chance = this.oneOverChance;
			this.uploadData.map = map;
			this.uploadData.mode = mode;
			this.uploadData.queue = queue;
			this.uploadData.player_count = (int)(PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.PlayerCount : 0);
			this.uploadData.pos_x = position.x;
			this.uploadData.pos_y = position.y;
			this.uploadData.pos_z = position.z;
			this.uploadData.vel_x = averagedVelocity.x;
			this.uploadData.vel_y = averagedVelocity.y;
			this.uploadData.vel_z = averagedVelocity.z;
			this.uploadData.cosmetics_owned = string.Join(";", from c in CosmeticsController.instance.unlockedCosmetics
			select c.itemName);
			this.uploadData.cosmetics_worn = string.Join(";", from c in CosmeticsController.instance.currentWornSet.items
			select c.itemName);
			GorillaServer.Instance.UploadGorillanalytics(this.uploadData);
			GorillaTelemetry.EnqueueTelemetryEvent("periodic_player_state", this.uploadData, null);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	// Token: 0x060038A1 RID: 14497 RVA: 0x00135A64 File Offset: 0x00133C64
	private void GetMapModeQueue(out string map, out string mode, out string queue)
	{
		if (!PhotonNetwork.InRoom)
		{
			map = "none";
			mode = "none";
			queue = "none";
			return;
		}
		object obj = null;
		Room currentRoom = PhotonNetwork.CurrentRoom;
		if (currentRoom != null)
		{
			currentRoom.CustomProperties.TryGetValue("gameMode", out obj);
		}
		GameModeString gameModeString = GameModeString.FromString(((obj != null) ? obj.ToString() : null) ?? "");
		GTZone gtzone = GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone;
		if (gtzone == GTZone.cityNoBuildings || gtzone == GTZone.cityWithSkyJungle || gtzone == GTZone.mall)
		{
			gtzone = GTZone.city;
		}
		if (gtzone == GTZone.tutorial)
		{
			gtzone = GTZone.forest;
		}
		if (gtzone == GTZone.ghostReactorTunnel)
		{
			gtzone = GTZone.ghostReactor;
		}
		map = gtzone.ToString().ToLower();
		if (NetworkSystem.Instance.SessionIsPrivate)
		{
			map += "private";
		}
		mode = ((gameModeString != null) ? gameModeString.gameType.ToUpper() : null);
		if (mode.IsNullOrEmpty())
		{
			mode = "none";
		}
		queue = ((gameModeString != null) ? gameModeString.queue.ToUpper() : null);
		if (queue.IsNullOrEmpty())
		{
			queue = "none";
		}
	}

	// Token: 0x040048AF RID: 18607
	public float interval = 60f;

	// Token: 0x040048B0 RID: 18608
	public double oneOverChance = 4320.0;

	// Token: 0x040048B1 RID: 18609
	public PhotonNetworkController photonNetworkController;

	// Token: 0x040048B2 RID: 18610
	public GameModeZoneMapping gameModeData;

	// Token: 0x040048B3 RID: 18611
	private readonly Gorillanalytics.UploadData uploadData = new Gorillanalytics.UploadData();

	// Token: 0x040048B4 RID: 18612
	public const string GORILLANALYTICS_EVENT_NAME = "periodic_player_state";

	// Token: 0x0200087F RID: 2175
	private class UploadData
	{
		// Token: 0x040048B5 RID: 18613
		public string version;

		// Token: 0x040048B6 RID: 18614
		public double upload_chance;

		// Token: 0x040048B7 RID: 18615
		public string map;

		// Token: 0x040048B8 RID: 18616
		public string mode;

		// Token: 0x040048B9 RID: 18617
		public string queue;

		// Token: 0x040048BA RID: 18618
		public int player_count;

		// Token: 0x040048BB RID: 18619
		public float pos_x;

		// Token: 0x040048BC RID: 18620
		public float pos_y;

		// Token: 0x040048BD RID: 18621
		public float pos_z;

		// Token: 0x040048BE RID: 18622
		public float vel_x;

		// Token: 0x040048BF RID: 18623
		public float vel_y;

		// Token: 0x040048C0 RID: 18624
		public float vel_z;

		// Token: 0x040048C1 RID: 18625
		public string cosmetics_owned;

		// Token: 0x040048C2 RID: 18626
		public string cosmetics_worn;
	}
}
