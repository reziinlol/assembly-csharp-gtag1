using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using GorillaNetworking;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Subscription.AlarmClocks
{
	// Token: 0x02000F6F RID: 3951
	[DefaultExecutionOrder(10000)]
	public sealed class AlarmClockManager : MonoBehaviour
	{
		// Token: 0x17000949 RID: 2377
		// (get) Token: 0x0600628A RID: 25226 RVA: 0x001FC9FA File Offset: 0x001FABFA
		// (set) Token: 0x0600628B RID: 25227 RVA: 0x001FCA01 File Offset: 0x001FAC01
		public static AlarmClockManager Instance { get; private set; }

		// Token: 0x1700094A RID: 2378
		// (get) Token: 0x0600628C RID: 25228 RVA: 0x001FCA09 File Offset: 0x001FAC09
		// (set) Token: 0x0600628D RID: 25229 RVA: 0x001FCA11 File Offset: 0x001FAC11
		public bool Initialized { get; private set; }

		// Token: 0x1700094B RID: 2379
		// (get) Token: 0x0600628E RID: 25230 RVA: 0x001FCA1A File Offset: 0x001FAC1A
		// (set) Token: 0x0600628F RID: 25231 RVA: 0x001FCA22 File Offset: 0x001FAC22
		public string ActiveKey { get; private set; } = "";

		// Token: 0x06006290 RID: 25232 RVA: 0x001FCA2C File Offset: 0x001FAC2C
		private void Start()
		{
			if (AlarmClockManager.Instance != null)
			{
				Debug.LogError("Duplicate instance of singleton class AlarmClockManager.");
				Object.Destroy(this);
				return;
			}
			if (this._defaultSpawn == null)
			{
				Debug.LogError("No default spawn set in AlarmClockManager.");
				Object.Destroy(this);
				return;
			}
			AlarmClockManager.Instance = this;
			this.ActiveKey = PlayerPrefs.GetString("AlarmClock");
			if (!string.IsNullOrEmpty(this.ActiveKey))
			{
				AlarmClockManager.AlarmClockData alarmClockData = this._clockData.FirstOrDefault((AlarmClockManager.AlarmClockData c) => c.Key == this.ActiveKey);
				if (alarmClockData != null && alarmClockData.SpawnPoint != this._defaultSpawn)
				{
					this._activeClockData = alarmClockData;
					this._teleportTarget = alarmClockData.SpawnPoint;
					base.StartCoroutine(this.PerformWakeUpSequence());
					return;
				}
			}
			this.Initialized = true;
		}

		// Token: 0x06006291 RID: 25233 RVA: 0x001FCAEF File Offset: 0x001FACEF
		private IEnumerator PerformWakeUpSequence()
		{
			while (!GTPlayer.hasInstance)
			{
				yield return null;
			}
			GTPlayer.Instance.disableMovement = true;
			while (!GorillaTagger.hasInstance || !GorillaTagger.Instance.mainCamera)
			{
				yield return null;
			}
			PrivateUIRoom.ForceStartOverlay(PrivateUIRoom.OverlaySource.AlarmClock, this._loadingMessage);
			PersistLog.Log(string.Format("[AC][F{0}] Waiting for game systems", Time.frameCount));
			while (!AlarmClockManager.GameSystemsLoaded())
			{
				yield return null;
			}
			PersistLog.Log(string.Format("[AC][F{0}] Game systems loaded", Time.frameCount));
			if (PlayFabAuthenticator.instance != null && SubscriptionManager.IsLocalSubscribed())
			{
				this.RequestLoadZones();
				yield return null;
				while (!this.AllZonesLoaded())
				{
					while (ZoneManagement.instance.AnyActiveLoadOps())
					{
						yield return null;
					}
					if (!this.AllZonesLoaded())
					{
						PersistLog.Log(string.Format("[AC][F{0}] Missing zones.  Requested:{1} Active: {2}", Time.frameCount, string.Join<GTZone>(", ", this._activeClockData.Zones), string.Join<GTZone>(", ", this.GetActiveZones())));
						this.RequestLoadZones();
						yield return null;
					}
				}
				PersistLog.Log(string.Format("[AC][F{0}] All zones loaded.", Time.frameCount));
				foreach (XSceneRef xsceneRef in this._activeClockData.Objects)
				{
					GameObject gameObject;
					if (xsceneRef.TryResolve(out gameObject))
					{
						gameObject.SetActive(true);
					}
				}
				yield return null;
				GTPlayer.Instance.TeleportTo(this._teleportTarget, true, false);
				yield return null;
				int fixAttempts = 0;
				while ((GTPlayer.Instance.mainCamera.transform.position - this._teleportTarget.position).sqrMagnitude > this._wrongWarpTolerance * this._wrongWarpTolerance)
				{
					int i = fixAttempts + 1;
					fixAttempts = i;
					if (i > 10)
					{
						break;
					}
					PersistLog.Log(string.Format("[AC][F{0}] AlarmClockManager attempting wrong warp fix. (Off by {1:F2})", Time.frameCount, GTPlayer.Instance.mainCamera.transform.position - this._teleportTarget.position));
					GTPlayer.Instance.TeleportTo(this._teleportTarget, true, false);
					yield return null;
				}
				GTPlayer.Instance.disableMovement = false;
				PrivateUIRoom.StopForcedOverlay(PrivateUIRoom.OverlaySource.AlarmClock);
				UnityEvent onWakeUp = this.OnWakeUp;
				if (onWakeUp != null)
				{
					onWakeUp.Invoke();
				}
			}
			else
			{
				if (PlayFabAuthenticator.instance == null)
				{
					PersistLog.Log(string.Format("[AC][F{0}] AlarmClockManager failed wake up because PlayFabAuthenticator was null.", Time.frameCount));
				}
				else if (PlayFabAuthenticator.instance.loginFailed)
				{
					PersistLog.Log(string.Format("[AC][F{0}] AlarmClockManager failed wake up because login failed.", Time.frameCount));
				}
				PersistLog.Log("No subscription.  Clearing clock data.");
				PlayerPrefs.SetString("AlarmClock", "");
				base.StartCoroutine(this.ClearUnsubPlayerData());
			}
			this.Initialized = true;
			GTPlayer.Instance.disableMovement = false;
			PrivateUIRoom.StopForcedOverlay(PrivateUIRoom.OverlaySource.AlarmClock);
			yield break;
		}

		// Token: 0x06006292 RID: 25234 RVA: 0x001FCAFE File Offset: 0x001FACFE
		public static void ToggleAlarmClock(AlarmClock clock)
		{
			AlarmClockManager.Instance.ToggleAlarmClockInternal(clock);
		}

		// Token: 0x06006293 RID: 25235 RVA: 0x001FCB0C File Offset: 0x001FAD0C
		private void ToggleAlarmClockInternal(AlarmClock clock)
		{
			if (this.ActiveKey == clock.Key)
			{
				AlarmClock activeClock = this._activeClock;
				if (activeClock != null)
				{
					UnityEvent onDeactivate = activeClock.OnDeactivate;
					if (onDeactivate != null)
					{
						onDeactivate.Invoke();
					}
				}
				this._activeClock = null;
				this.ActiveKey = "";
			}
			else
			{
				AlarmClock activeClock2 = this._activeClock;
				if (activeClock2 != null)
				{
					UnityEvent onDeactivate2 = activeClock2.OnDeactivate;
					if (onDeactivate2 != null)
					{
						onDeactivate2.Invoke();
					}
				}
				this._activeClock = clock;
				AlarmClock activeClock3 = this._activeClock;
				if (activeClock3 != null)
				{
					UnityEvent onActivate = activeClock3.OnActivate;
					if (onActivate != null)
					{
						onActivate.Invoke();
					}
				}
				this.ActiveKey = clock.Key;
			}
			PlayerPrefs.SetString("AlarmClock", this.ActiveKey);
			Debug.Log("Alarm clock data set to \"" + this.ActiveKey + "\".");
		}

		// Token: 0x06006294 RID: 25236 RVA: 0x001FCBD1 File Offset: 0x001FADD1
		private void OnDestroy()
		{
			if (AlarmClockManager.Instance == this)
			{
				AlarmClockManager.Instance = null;
			}
		}

		// Token: 0x06006295 RID: 25237 RVA: 0x001FCBE8 File Offset: 0x001FADE8
		[UsedImplicitly]
		private bool AllUniqueClockKeys()
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (AlarmClockManager.AlarmClockData alarmClockData in this._clockData)
			{
				if (!hashSet.Add(alarmClockData.Key))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06006296 RID: 25238 RVA: 0x001FCC28 File Offset: 0x001FAE28
		private static bool GameSystemsLoaded()
		{
			return (ZoneManagement.instance && ZoneManagement.instance.Initialized && CosmeticsController.instance && CosmeticsController.instance.v2_isCosmeticPlayFabCatalogDataLoaded && CosmeticsV2Spawner_Dirty.isPrepared && SubscriptionManager.LocalSubscriptionDataInitialized) || (PlayFabAuthenticator.instance != null && PlayFabAuthenticator.instance.loginFailed) || PlayFabAuthenticator.instance == null;
		}

		// Token: 0x06006297 RID: 25239 RVA: 0x001FCCA4 File Offset: 0x001FAEA4
		private bool AllZonesLoaded()
		{
			if (ZoneManagement.instance.AnyActiveLoadOps())
			{
				return false;
			}
			foreach (GTZone gtzone in this._activeClockData.Zones)
			{
				if (!ZoneManagement.instance.IsSceneLoaded(gtzone))
				{
					return false;
				}
				if (!ZoneManagement.IsZoneLoaded(gtzone))
				{
					PersistLog.Log(string.Format("[AC][F{0}] ZoneManagement reports Zone {1} is loaded but SceneManager says no.", Time.frameCount, gtzone));
					return false;
				}
			}
			return true;
		}

		// Token: 0x06006298 RID: 25240 RVA: 0x001FCD18 File Offset: 0x001FAF18
		private List<GTZone> GetActiveZones()
		{
			List<GTZone> list = new List<GTZone>();
			foreach (object obj in Enum.GetValues(typeof(GTZone)))
			{
				GTZone gtzone = (GTZone)obj;
				if (ZoneManagement.IsInZone(gtzone))
				{
					list.Add(gtzone);
				}
			}
			return list;
		}

		// Token: 0x06006299 RID: 25241 RVA: 0x001FCD8C File Offset: 0x001FAF8C
		private void RequestLoadZones()
		{
			PersistLog.Log(string.Format("[AC][F{0}] Requesting zones: {1}", Time.frameCount, string.Join<GTZone>(", ", this._activeClockData.Zones)));
			ZoneManagement.SetActiveZones(this._activeClockData.Zones);
		}

		// Token: 0x0600629A RID: 25242 RVA: 0x001FCDCC File Offset: 0x001FAFCC
		private void StartTracking()
		{
			base.StartCoroutine(this.DoTracking());
		}

		// Token: 0x0600629B RID: 25243 RVA: 0x001FCDDB File Offset: 0x001FAFDB
		private void StopTracking(float delay)
		{
			Debug.Log(string.Format("[AC][F{0}] STOP TRACKING", Time.frameCount));
			this._trackingEndTime = Time.time + delay;
		}

		// Token: 0x0600629C RID: 25244 RVA: 0x001FCE03 File Offset: 0x001FB003
		private IEnumerator DoTracking()
		{
			this._trackingEndTime = float.PositiveInfinity;
			while (Time.time < this._trackingEndTime)
			{
				Debug.Log(string.Format("[AC][F{0}] Pos: {1} Off:[{2}] Distance: {3}[R{4}][C{5}]", new object[]
				{
					Time.frameCount,
					GTPlayer.Instance.transform.position,
					GTPlayer.Instance.LastPosition - this._teleportTarget.position,
					(GTPlayer.Instance.LastPosition - this._teleportTarget.position).magnitude,
					(GTPlayer.Instance.playerRigidBody.position - this._teleportTarget.position).magnitude,
					(GTPlayer.Instance.mainCamera.transform.position - this._teleportTarget.position).magnitude
				}));
				yield return null;
			}
			yield break;
		}

		// Token: 0x0600629D RID: 25245 RVA: 0x001FCE12 File Offset: 0x001FB012
		private IEnumerator ClearUnsubPlayerData()
		{
			PersistLog.Log("No subscription, warping home.");
			PlayerPrefs.SetString("AlarmClock", "");
			GTPlayer.Instance.TeleportTo(this._defaultSpawn, true, false);
			this.Initialized = true;
			yield return null;
			GTPlayer.Instance.disableMovement = false;
			PrivateUIRoom.StopForcedOverlay(PrivateUIRoom.OverlaySource.AlarmClock);
			yield break;
		}

		// Token: 0x04007172 RID: 29042
		public const string SaveDataKey = "AlarmClock";

		// Token: 0x04007175 RID: 29045
		[SerializeField]
		private string _loadingMessage = "";

		// Token: 0x04007176 RID: 29046
		[SerializeField]
		private float _wrongWarpTolerance = 0.1f;

		// Token: 0x04007177 RID: 29047
		[SerializeField]
		private AlarmClockManager.AlarmClockData[] _clockData = new AlarmClockManager.AlarmClockData[0];

		// Token: 0x04007178 RID: 29048
		[SerializeField]
		private Transform _defaultSpawn;

		// Token: 0x04007179 RID: 29049
		public UnityEvent OnWakeUp;

		// Token: 0x0400717A RID: 29050
		private Transform _teleportTarget;

		// Token: 0x0400717C RID: 29052
		private AlarmClockManager.AlarmClockData _activeClockData;

		// Token: 0x0400717D RID: 29053
		[CanBeNull]
		private AlarmClock _activeClock;

		// Token: 0x0400717E RID: 29054
		private float _trackingEndTime;

		// Token: 0x02000F70 RID: 3952
		[Serializable]
		public class AlarmClockData
		{
			// Token: 0x0400717F RID: 29055
			public string Key;

			// Token: 0x04007180 RID: 29056
			public GTZone[] Zones;

			// Token: 0x04007181 RID: 29057
			public XSceneRef[] Objects;

			// Token: 0x04007182 RID: 29058
			public Transform SpawnPoint;
		}
	}
}
