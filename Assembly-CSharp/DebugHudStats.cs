using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using GorillaUtil;
using TMPro;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

// Token: 0x02000D40 RID: 3392
public class DebugHudStats : MonoBehaviour
{
	// Token: 0x170007E5 RID: 2021
	// (get) Token: 0x06005393 RID: 21395 RVA: 0x001B50C2 File Offset: 0x001B32C2
	public static DebugHudStats Instance
	{
		get
		{
			return DebugHudStats._instance;
		}
	}

	// Token: 0x06005394 RID: 21396 RVA: 0x001B50CC File Offset: 0x001B32CC
	private void Awake()
	{
		if (DebugHudStats._instance != null && DebugHudStats._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			DebugHudStats._instance = this;
			this.fixedWeathers = Enum.GetValues(typeof(BetterDayNightManager.WeatherType));
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06005395 RID: 21397 RVA: 0x001B5128 File Offset: 0x001B3328
	private void OnDestroy()
	{
		if (DebugHudStats._instance == this)
		{
			DebugHudStats._instance = null;
			if (this.drawCallsRecorder.Valid)
			{
				this.drawCallsRecorder.Dispose();
			}
			if (this.trisRecorder.Valid)
			{
				this.trisRecorder.Dispose();
			}
		}
	}

	// Token: 0x06005396 RID: 21398 RVA: 0x001B5178 File Offset: 0x001B3378
	private void LateUpdate()
	{
		if (GTPlayerTransform.Instance != null)
		{
			base.transform.LookAt(Camera.main.transform.position, GTPlayerTransform.Instance.GravityUp);
		}
		else
		{
			base.transform.LookAt(Camera.main.transform.position, Vector3.up);
		}
		if (this.currentState == DebugHudStats.State.timeAdjust)
		{
			bool flag = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
			bool flag2 = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
			bool flag3 = ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.5f;
			bool flag4 = ControllerInputPoller.GripFloat(XRNode.RightHand) > 0.5f;
			bool flag5 = ControllerInputPoller.Primary2DAxis(XRNode.LeftHand).x > 0.5f;
			bool flag6 = ControllerInputPoller.Primary2DAxis(XRNode.LeftHand).x < -0.5f;
			bool flag7 = ControllerInputPoller.Primary2DAxis(XRNode.LeftHand).y > 0.5f;
			bool flag8 = ControllerInputPoller.Primary2DAxis(XRNode.LeftHand).y < -0.5f;
			if (this.button1Down && !flag)
			{
				GorillaComputer.instance.AddSeverTime(flag4 ? -60 : 60);
			}
			if (this.button2Down && !flag2)
			{
				GorillaComputer.instance.AddSeverTime(flag4 ? -1 : 5);
			}
			if (this.button3Down && !flag3)
			{
				GorillaComputer.instance.AddSeverTime(flag4 ? -1440 : 1440);
			}
			if (!this.button5Down && flag5)
			{
				this.ChangeTOD(1);
			}
			if (!this.button6Down && flag6)
			{
				this.ChangeTOD(-1);
			}
			if (!this.button7Down && flag7)
			{
				this.ChangeWeather(1);
			}
			if (!this.button8Down && flag8)
			{
				this.ChangeWeather(-1);
			}
			this.button1Down = flag;
			this.button2Down = flag2;
			this.button3Down = flag3;
			this.button5Down = flag5;
			this.button6Down = flag6;
			this.button7Down = flag7;
			this.button8Down = flag8;
		}
		if (this.currentState == DebugHudStats.State.TitleDataMonitor || this.currentState == DebugHudStats.State.ShowLog || this.currentState == DebugHudStats.State.ShowError)
		{
			bool flag9 = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
			bool flag10 = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
			if (this.button1Down && !flag9)
			{
				this.logging.pageToDisplay = ((this.logging.pageToDisplay < this.logging.textInfo.pageCount) ? (this.logging.pageToDisplay + 1) : 1);
				this.updateLogTitle();
			}
			if (this.button2Down && !flag10)
			{
				this.logging.pageToDisplay = ((this.logging.pageToDisplay > 1) ? (this.logging.pageToDisplay - 1) : this.logging.textInfo.pageCount);
				this.updateLogTitle();
			}
			this.button1Down = flag9;
			this.button2Down = flag10;
		}
		bool flag11 = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
		bool flag12 = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
		if ((this.buttonDown && !flag11) || (this.buttonDownBack && !flag12))
		{
			this.NextState(this.buttonDown);
			if (this.currentState == DebugHudStats.State.ShowStats)
			{
				this.distanceMoved = (this.distanceSwam = 0f);
				PlayerGameEvents.OnPlayerMoved += this.OnPlayerMoved;
				PlayerGameEvents.OnPlayerSwam += this.OnPlayerSwam;
			}
			this.text.gameObject.SetActive(this.currentState > DebugHudStats.State.Inactive);
			if (RigidbodyHighlighter.Instance != null)
			{
				RigidbodyHighlighter.Instance.Active = (this.currentState == DebugHudStats.State.ShowRBs);
			}
		}
		this.buttonDown = flag11;
		this.buttonDownBack = flag12;
		if (this.firstAwake == 0f)
		{
			this.firstAwake = Time.time;
		}
		if (this.updateTimer < this.delayUpdateRate)
		{
			this.updateTimer += Time.deltaTime;
			return;
		}
		int num = Mathf.RoundToInt(1f / Time.smoothDeltaTime);
		if (num < DebugHudStats.FPS_THRESHOLD)
		{
			this.lowFps++;
		}
		else
		{
			this.lowFps = 0;
		}
		this.fpsWarning.gameObject.SetActive(this.lowFps > 5 && this.currentState == DebugHudStats.State.Inactive);
		if (this.currentState != DebugHudStats.State.Inactive)
		{
			this.builder.Clear();
			this.builder.Append("gt: ");
			this.builder.Append(GorillaComputer.instance.version);
			this.builder.Append(":");
			this.builder.Append(GorillaComputer.instance.buildCode);
			this.builder.AppendLine(this.spoofIds ? " <color=\"red\">*Spoofing IDs*</color>" : string.Empty);
			num = Mathf.Min(num, 90);
			this.builder.Append((num < DebugHudStats.FPS_THRESHOLD) ? "<color=\"red\">" : "<color=\"white\">");
			this.builder.Append(num);
			this.builder.Append(string.Format(" fps / {0} fps</color> ", DebugHudStats.FPS_THRESHOLD + 1));
			this.builder.AppendLine(string.Format("sfps: {0} (Health: {1})", GorillaTagger.Instance.SmoothedFramerate, GorillaTagger.Instance.FramerateHealth));
			float eyeTextureResolutionScale = XRSettings.eyeTextureResolutionScale;
			float renderViewportScale = XRSettings.renderViewportScale;
			float renderScale = (GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset).renderScale;
			this.builder.AppendLine(string.Format("draw calls: {0} tris: {1} ", this.drawCallsRecorder.LastValue, this.trisRecorder.LastValue) + string.Format("rs: {0}/{1}/{2} ", eyeTextureResolutionScale, renderViewportScale, renderScale));
			if (GorillaComputer.instance != null)
			{
				DateTime serverTime = GorillaComputer.instance.GetServerTime();
				this.builder.AppendLine(string.Format("<color={0}>{1}</color>", (serverTime.Year > 2020) ? "#00FFAA" : "#FF3333", serverTime));
			}
			else
			{
				this.builder.AppendLine("<color=#FF3333>Server Time Unavailable</color>");
			}
			ZoneDef currentNode = GorillaTagger.Instance.offlineVRRig.zoneEntity.currentNode;
			if (currentNode != null)
			{
				this.zones = string.Format("{0}/{1}/{2}", currentNode.gameObject.name.ToUpperInvariant(), currentNode.zoneId, currentNode.subZoneId);
			}
			if (NetworkSystem.Instance.IsMasterClient)
			{
				this.builder.Append("H");
			}
			if (NetworkSystem.Instance.InRoom)
			{
				if (NetworkSystem.Instance.SessionIsPrivate)
				{
					this.builder.Append("Pri ");
				}
				else
				{
					this.builder.Append("Pub ");
				}
			}
			else
			{
				this.builder.Append("DC ");
			}
			this.builder.Append("z: <color=\"green\">");
			this.builder.Append(this.zones);
			this.builder.AppendLine("</color>");
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaGameManager instance = GorillaGameManager.instance;
				if (instance != null)
				{
					GorillaTagCompetitiveManager gorillaTagCompetitiveManager = instance as GorillaTagCompetitiveManager;
					if (gorillaTagCompetitiveManager != null)
					{
						this.builder.Append("Ranked Mode ELO: ");
						this.builder.Append(gorillaTagCompetitiveManager.GetScoring().Progression.GetEloScore().ToString());
						this.builder.Append("  Tier: ");
						this.builder.AppendLine(gorillaTagCompetitiveManager.GetScoring().Progression.GetRankedProgressionTierName());
						RankedMultiplayerScore.PlayerScoreInRound inGameScoreForSelf = gorillaTagCompetitiveManager.GetScoring().GetInGameScoreForSelf();
						this.builder.Append("Tags: ");
						this.builder.Append(inGameScoreForSelf.NumTags.ToString());
						this.builder.Append("  Defense: ");
						this.builder.Append(Mathf.RoundToInt(inGameScoreForSelf.PointsOnDefense).ToString());
						this.builder.Append("  Score: ");
						this.builder.AppendLine(Mathf.RoundToInt(gorillaTagCompetitiveManager.GetScoring().ComputeGameScore(inGameScoreForSelf.NumTags, inGameScoreForSelf.PointsOnDefense)).ToString());
						if (gorillaTagCompetitiveManager.ShowDebugPing)
						{
							this.builder.AppendLine("Server MatchID Ping!");
						}
					}
				}
			}
			switch (this.currentState)
			{
			case DebugHudStats.State.ShowStats:
			{
				this.builder.AppendLine("\nStats:\n");
				Vector3 vector = GTPlayer.Instance.AveragedVelocity;
				Vector3 headCenterPosition = GTPlayer.Instance.HeadCenterPosition;
				float magnitude = vector.magnitude;
				this.groundVelocity = vector;
				this.groundVelocity.y = 0f;
				this.builder.AppendLine(string.Format("v: {0:F1} m/s\t\todo: {1:F2}m\tswam: {2:F2}m", magnitude, this.distanceMoved, this.distanceSwam));
				this.builder.AppendLine(string.Format("ground: {0:F1} m/s\thead: {1:F2}", this.groundVelocity.magnitude, headCenterPosition));
				break;
			}
			case DebugHudStats.State.ShowRBs:
				this.builder.AppendLine("\nRigid Body Locator\n");
				break;
			case DebugHudStats.State.timeAdjust:
				this.builder.AppendLine("\nAdjust Time\n");
				this.builder.AppendLine("Press [A] to advance one hour [+ R Grip to go back one hour]");
				this.builder.AppendLine("Press [B] to advance five minutes [+ R Grip to go back one minute]");
				this.builder.AppendLine("Press [R] Trigger to advance one day [+ R Grip to go back one day]");
				this.builder.AppendLine(string.Format("\nAdjust Environment {0}/{1} : {2} \n", BetterDayNightManager.instance.currentTimeIndex + 1, BetterDayNightManager.instance.timeOfDayRange.Length, BetterDayNightManager.instance.CurrentWeather()));
				this.builder.AppendLine("[L STICK L/R] to change Time Of Day. [L STICK U/D] to change Weather.");
				break;
			case DebugHudStats.State.RecordingMode:
				this.builder.AppendLine("\nMo-Cap Recording:\n");
				break;
			}
			this.text.text = this.builder.ToString();
		}
		this.updateTimer = 0f;
	}

	// Token: 0x06005397 RID: 21399 RVA: 0x001B5B84 File Offset: 0x001B3D84
	private void ChangeTOD(int v)
	{
		int num = (BetterDayNightManager.instance.currentTimeIndex + BetterDayNightManager.instance.timeOfDayRange.Length + v) % BetterDayNightManager.instance.timeOfDayRange.Length;
		BetterDayNightManager.instance.SetTimeOfDay(num);
		BetterDayNightManager.instance.SetOverrideIndex(num);
		BetterDayNightManager.instance.SetFixedWeather((BetterDayNightManager.WeatherType)this.fixedWeathers.GetValue(this.fixedWeatherIndex));
	}

	// Token: 0x06005398 RID: 21400 RVA: 0x001B5BFC File Offset: 0x001B3DFC
	private void ChangeWeather(int v)
	{
		this.fixedWeatherIndex = (this.fixedWeatherIndex + this.fixedWeathers.Length + v) % this.fixedWeathers.Length;
		BetterDayNightManager.instance.SetFixedWeather((BetterDayNightManager.WeatherType)this.fixedWeathers.GetValue(this.fixedWeatherIndex));
	}

	// Token: 0x06005399 RID: 21401 RVA: 0x001B5C54 File Offset: 0x001B3E54
	private void NextState(bool fwd)
	{
		PlayerGameEvents.OnPlayerMoved -= this.OnPlayerMoved;
		PlayerGameEvents.OnPlayerSwam -= this.OnPlayerSwam;
		this.logging.gameObject.SetActive(false);
		this.logging.pageToDisplay = 1;
		if (this.currentState == DebugHudStats.State.timeAdjust)
		{
			BetterDayNightManager.instance.ClearFixedWeather();
		}
		switch (this.currentState)
		{
		case DebugHudStats.State.Inactive:
			this.currentState = (fwd ? DebugHudStats.State.Active : DebugHudStats.State.timeAdjust);
			break;
		case DebugHudStats.State.Active:
			this.currentState = (fwd ? DebugHudStats.State.ShowLog : DebugHudStats.State.Inactive);
			break;
		case DebugHudStats.State.ShowLog:
			this.currentState = (fwd ? DebugHudStats.State.ShowError : DebugHudStats.State.Active);
			break;
		case DebugHudStats.State.ShowError:
			this.currentState = (fwd ? DebugHudStats.State.ShowStats : DebugHudStats.State.ShowLog);
			break;
		case DebugHudStats.State.ShowStats:
			this.currentState = (fwd ? DebugHudStats.State.ShowRBs : DebugHudStats.State.ShowError);
			break;
		case DebugHudStats.State.ShowRBs:
			this.currentState = (fwd ? DebugHudStats.State.TitleDataMonitor : DebugHudStats.State.ShowStats);
			break;
		case DebugHudStats.State.timeAdjust:
			this.currentState = (fwd ? DebugHudStats.State.Inactive : DebugHudStats.State.TitleDataMonitor);
			break;
		case DebugHudStats.State.RecordingMode:
			this.currentState = (fwd ? DebugHudStats.State.Inactive : DebugHudStats.State.timeAdjust);
			break;
		case DebugHudStats.State.TitleDataMonitor:
			this.currentState = (fwd ? DebugHudStats.State.timeAdjust : DebugHudStats.State.ShowRBs);
			break;
		}
		if (this.currentState == DebugHudStats.State.timeAdjust)
		{
			BetterDayNightManager.instance.SetFixedWeather((BetterDayNightManager.WeatherType)this.fixedWeathers.GetValue(this.fixedWeatherIndex));
		}
		this.UpdateLog();
	}

	// Token: 0x0600539A RID: 21402 RVA: 0x001B5DA4 File Offset: 0x001B3FA4
	private void DisplayLog(List<string> log)
	{
		this.logging.gameObject.SetActive(true);
		this.logging.text = string.Empty;
		for (int i = log.Count - 1; i >= 0; i--)
		{
			TMP_Text tmp_Text = this.logging;
			tmp_Text.text = tmp_Text.text + log[i] + "\n";
		}
		this.updateLogTitle();
	}

	// Token: 0x0600539B RID: 21403 RVA: 0x001B5E10 File Offset: 0x001B4010
	private void updateLogTitle()
	{
		DebugHudStats.<updateLogTitle>d__52 <updateLogTitle>d__;
		<updateLogTitle>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<updateLogTitle>d__.<>4__this = this;
		<updateLogTitle>d__.<>1__state = -1;
		<updateLogTitle>d__.<>t__builder.Start<DebugHudStats.<updateLogTitle>d__52>(ref <updateLogTitle>d__);
	}

	// Token: 0x0600539C RID: 21404 RVA: 0x001B5E47 File Offset: 0x001B4047
	private string logTitleFromState(DebugHudStats.State s)
	{
		if (s == DebugHudStats.State.ShowLog)
		{
			return "Debug Log";
		}
		if (s == DebugHudStats.State.ShowError)
		{
			return "Error Log";
		}
		if (s != DebugHudStats.State.TitleDataMonitor)
		{
			return string.Empty;
		}
		return "Title Data Log";
	}

	// Token: 0x0600539D RID: 21405 RVA: 0x001B5E70 File Offset: 0x001B4070
	private string colorFromState(DebugHudStats.State s)
	{
		switch (s)
		{
		case DebugHudStats.State.ShowLog:
			return "\"yellow\"";
		case DebugHudStats.State.ShowError:
			return "\"orange\"";
		case DebugHudStats.State.ShowStats:
			return "\"green\"";
		case DebugHudStats.State.ShowRBs:
			return "\"red\"";
		case DebugHudStats.State.RecordingMode:
			return "\"purple\"";
		case DebugHudStats.State.TitleDataMonitor:
			return "#00ffff";
		}
		return "#ffffff";
	}

	// Token: 0x0600539E RID: 21406 RVA: 0x001B5ECC File Offset: 0x001B40CC
	private void OnPlayerSwam(float distance, float speed)
	{
		if (distance > 0.005f)
		{
			this.distanceSwam += distance;
		}
	}

	// Token: 0x0600539F RID: 21407 RVA: 0x001B5EE4 File Offset: 0x001B40E4
	private void OnPlayerMoved(float distance, float speed)
	{
		if (distance > 0.005f)
		{
			this.distanceMoved += distance;
		}
	}

	// Token: 0x060053A0 RID: 21408 RVA: 0x001B5EFC File Offset: 0x001B40FC
	private void OnEnable()
	{
		Application.logMessageReceived += this.LogMessageReceived;
		PlayFabTitleDataCache.OnValueRetieved = (Action<string, string>)Delegate.Combine(PlayFabTitleDataCache.OnValueRetieved, new Action<string, string>(this.TDValueRetrieved));
		PlayFabTitleDataCache.OnCachedValueRetieved = (Action<string, string>)Delegate.Combine(PlayFabTitleDataCache.OnCachedValueRetieved, new Action<string, string>(this.TDCachedValueRetrieved));
	}

	// Token: 0x060053A1 RID: 21409 RVA: 0x001B5F5C File Offset: 0x001B415C
	private void TDValueRetrieved(string arg1, string arg2)
	{
		this.logTD.Add(string.Format(" >{0:F2}> TitleData[ <color=#ffaaff>{1}</color> ] = {2}", Time.realtimeSinceStartup, arg1, arg2));
		if (this.logTD.Count > 1000)
		{
			this.logTD.RemoveAt(0);
		}
		this.UpdateLog();
	}

	// Token: 0x060053A2 RID: 21410 RVA: 0x001B5FB0 File Offset: 0x001B41B0
	private void TDCachedValueRetrieved(string arg1, string arg2)
	{
		this.logTD.Add(string.Format(" >{0:F2}> TitleData[ <color=#00ffff>{1}</color> ] = {2}", Time.realtimeSinceStartup, arg1, arg2));
		if (this.logTD.Count > 1000)
		{
			this.logTD.RemoveAt(0);
		}
		this.UpdateLog();
	}

	// Token: 0x060053A3 RID: 21411 RVA: 0x001B6004 File Offset: 0x001B4204
	private void OnDisable()
	{
		PlayFabTitleDataCache.OnValueRetieved = (Action<string, string>)Delegate.Remove(PlayFabTitleDataCache.OnValueRetieved, new Action<string, string>(this.TDValueRetrieved));
		PlayFabTitleDataCache.OnCachedValueRetieved = (Action<string, string>)Delegate.Remove(PlayFabTitleDataCache.OnCachedValueRetieved, new Action<string, string>(this.TDCachedValueRetrieved));
		Application.logMessageReceived -= this.LogMessageReceived;
	}

	// Token: 0x060053A4 RID: 21412 RVA: 0x001B6064 File Offset: 0x001B4264
	private void LogMessageReceived(string condition, string stackTrace, LogType type)
	{
		string text = string.Format(" >{0:F2}> {1}{2}</color>", Time.realtimeSinceStartup, this.getColorStringFromLogType(type), condition);
		if (this.pLog != condition)
		{
			this.logMessage.Add(text);
		}
		else
		{
			this.logMessage[this.logMessage.Count - 1] = text;
		}
		this.pLog = condition;
		if (this.logMessage.Count > 100)
		{
			this.logMessage.RemoveAt(0);
		}
		if (type == LogType.Error || type == LogType.Assert || type == LogType.Exception)
		{
			this.logError.Add(text + "\n" + stackTrace);
			if (this.logError.Count > 100)
			{
				this.logError.RemoveAt(0);
			}
		}
		this.UpdateLog();
	}

	// Token: 0x060053A5 RID: 21413 RVA: 0x001B6128 File Offset: 0x001B4328
	private void UpdateLog()
	{
		DebugHudStats.State state = this.currentState;
		if (state == DebugHudStats.State.ShowLog)
		{
			this.DisplayLog(this.logMessage);
			return;
		}
		if (state == DebugHudStats.State.ShowError)
		{
			this.DisplayLog(this.logError);
			return;
		}
		if (state != DebugHudStats.State.TitleDataMonitor)
		{
			return;
		}
		this.DisplayLog(this.logTD);
	}

	// Token: 0x060053A6 RID: 21414 RVA: 0x001B616F File Offset: 0x001B436F
	private string getColorStringFromLogType(LogType type)
	{
		switch (type)
		{
		case LogType.Error:
		case LogType.Assert:
		case LogType.Exception:
			return "<color=\"red\">";
		case LogType.Warning:
			return "<color=\"yellow\">";
		}
		return "<color=\"white\">";
	}

	// Token: 0x060053A7 RID: 21415 RVA: 0x001B61A0 File Offset: 0x001B43A0
	private void OnZoneChanged(ZoneData[] zoneData)
	{
		this.zones = string.Empty;
		for (int i = 0; i < zoneData.Length; i++)
		{
			if (zoneData[i].active)
			{
				this.zones = this.zones + zoneData[i].zone.ToString().ToUpper() + "; ";
			}
		}
	}

	// Token: 0x04006499 RID: 25753
	public static int FPS_THRESHOLD = 89;

	// Token: 0x0400649A RID: 25754
	private static DebugHudStats _instance;

	// Token: 0x0400649B RID: 25755
	[SerializeField]
	public TMP_Text text;

	// Token: 0x0400649C RID: 25756
	[SerializeField]
	public TMP_Text logging;

	// Token: 0x0400649D RID: 25757
	[SerializeField]
	public TMP_Text logPage;

	// Token: 0x0400649E RID: 25758
	[SerializeField]
	private TMP_Text fpsWarning;

	// Token: 0x0400649F RID: 25759
	[SerializeField]
	private float delayUpdateRate = 0.25f;

	// Token: 0x040064A0 RID: 25760
	private float updateTimer;

	// Token: 0x040064A1 RID: 25761
	public float sessionAnytrackingLost;

	// Token: 0x040064A2 RID: 25762
	public float last30SecondsTrackingLost;

	// Token: 0x040064A3 RID: 25763
	private float firstAwake;

	// Token: 0x040064A4 RID: 25764
	private bool leftHandTracked;

	// Token: 0x040064A5 RID: 25765
	private bool rightHandTracked;

	// Token: 0x040064A6 RID: 25766
	private StringBuilder builder;

	// Token: 0x040064A7 RID: 25767
	private Vector3 averagedVelocity;

	// Token: 0x040064A8 RID: 25768
	private Vector3 groundVelocity;

	// Token: 0x040064A9 RID: 25769
	private Vector3 centerHeadPos;

	// Token: 0x040064AA RID: 25770
	private float distanceMoved;

	// Token: 0x040064AB RID: 25771
	private float distanceSwam;

	// Token: 0x040064AC RID: 25772
	private List<string> logMessage = new List<string>();

	// Token: 0x040064AD RID: 25773
	private List<string> logError = new List<string>();

	// Token: 0x040064AE RID: 25774
	private List<string> logTD = new List<string>();

	// Token: 0x040064AF RID: 25775
	private bool buttonDown;

	// Token: 0x040064B0 RID: 25776
	private bool buttonDownBack;

	// Token: 0x040064B1 RID: 25777
	private bool spoofIds;

	// Token: 0x040064B2 RID: 25778
	private int lowFps;

	// Token: 0x040064B3 RID: 25779
	private string zones;

	// Token: 0x040064B4 RID: 25780
	private GroupJoinZoneAB lastGroupJoinZone;

	// Token: 0x040064B5 RID: 25781
	private DebugHudStats.State currentState = DebugHudStats.State.Active;

	// Token: 0x040064B6 RID: 25782
	private ProfilerRecorder drawCallsRecorder;

	// Token: 0x040064B7 RID: 25783
	private ProfilerRecorder trisRecorder;

	// Token: 0x040064B8 RID: 25784
	private string pLog;

	// Token: 0x040064B9 RID: 25785
	private bool button1Down;

	// Token: 0x040064BA RID: 25786
	private bool button2Down;

	// Token: 0x040064BB RID: 25787
	private bool button3Down;

	// Token: 0x040064BC RID: 25788
	private bool button5Down;

	// Token: 0x040064BD RID: 25789
	private bool button6Down;

	// Token: 0x040064BE RID: 25790
	private bool button7Down;

	// Token: 0x040064BF RID: 25791
	private bool button8Down;

	// Token: 0x040064C0 RID: 25792
	[SerializeField]
	private StringTable betaTitleDataOveride;

	// Token: 0x040064C1 RID: 25793
	private Array fixedWeathers;

	// Token: 0x040064C2 RID: 25794
	private int fixedWeatherIndex;

	// Token: 0x02000D41 RID: 3393
	private enum State
	{
		// Token: 0x040064C4 RID: 25796
		Inactive,
		// Token: 0x040064C5 RID: 25797
		Active,
		// Token: 0x040064C6 RID: 25798
		ShowLog,
		// Token: 0x040064C7 RID: 25799
		ShowError,
		// Token: 0x040064C8 RID: 25800
		ShowStats,
		// Token: 0x040064C9 RID: 25801
		ShowRBs,
		// Token: 0x040064CA RID: 25802
		timeAdjust,
		// Token: 0x040064CB RID: 25803
		RecordingMode,
		// Token: 0x040064CC RID: 25804
		TitleDataMonitor
	}
}
