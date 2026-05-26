using System;
using GorillaExtensions;
using Steamworks;
using UnityEngine;

// Token: 0x02000C87 RID: 3207
public class MothershipAuthenticator : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06004F95 RID: 20373 RVA: 0x001A5ACD File Offset: 0x001A3CCD
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		if (MothershipAuthenticator.Instance == null)
		{
			MothershipAuthenticator.Instance = null;
		}
	}

	// Token: 0x06004F96 RID: 20374 RVA: 0x001A5AE8 File Offset: 0x001A3CE8
	public void Awake()
	{
		if (MothershipAuthenticator.Instance == null)
		{
			MothershipAuthenticator.Instance = this;
		}
		else if (MothershipAuthenticator.Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		if (!MothershipClientApiUnity.IsEnabled())
		{
			Debug.Log("Mothership is not enabled.");
			return;
		}
		if (MothershipAuthenticator.Instance.SteamAuthenticator == null)
		{
			MothershipAuthenticator.Instance.SteamAuthenticator = MothershipAuthenticator.Instance.gameObject.GetOrAddComponent<SteamAuthenticator>();
		}
		MothershipClientApiUnity.SetLogCallback(delegate(MothershipLogLevel level, string message)
		{
			LogType type;
			switch (level)
			{
			case MothershipLogLevel.INFO:
				type = LogType.Log;
				break;
			case MothershipLogLevel.WARN:
				type = LogType.Warning;
				break;
			case MothershipLogLevel.ERROR:
				type = LogType.Error;
				break;
			default:
				type = LogType.Log;
				break;
			}
			PersistLog.Log(type, message);
		});
		MothershipClientApiUnity.SetAuthRefreshedCallback(delegate(string id)
		{
			this.BeginLoginFlow();
		});
	}

	// Token: 0x06004F97 RID: 20375 RVA: 0x001A5BA0 File Offset: 0x001A3DA0
	public void BeginLoginFlow()
	{
		Debug.Log("making login call");
		this.LogInWithSteam();
	}

	// Token: 0x06004F98 RID: 20376 RVA: 0x001A5BB2 File Offset: 0x001A3DB2
	private void LogInWithInsecure()
	{
		MothershipClientApiUnity.LogInWithInsecure1(this.TestNickname, this.TestAccountId, delegate(LoginResponse LoginResponse)
		{
			Debug.Log("Logged in with Mothership Id " + LoginResponse.MothershipPlayerId);
			MothershipClientApiUnity.OpenNotificationsSocket();
			Action onLoginSuccess = this.OnLoginSuccess;
			if (onLoginSuccess == null)
			{
				return;
			}
			onLoginSuccess();
		}, delegate(MothershipError MothershipError, int errorCode)
		{
			Debug.LogError(string.Format("Failed to log in, error {0} trace ID: {1} status: {2} Mothership error code: {3}", new object[]
			{
				MothershipError.Message,
				MothershipError.TraceId,
				errorCode,
				MothershipError.MothershipErrorCode
			}));
			Action<int> onLoginAttemptFailure = this.OnLoginAttemptFailure;
			if (onLoginAttemptFailure != null)
			{
				onLoginAttemptFailure(1);
			}
			Action<string, string, string> onLoginFailure = this.OnLoginFailure;
			if (onLoginFailure == null)
			{
				return;
			}
			onLoginFailure(MothershipError.Message, MothershipError.MothershipErrorCode, MothershipError.TraceId);
		});
	}

	// Token: 0x06004F99 RID: 20377 RVA: 0x001A5BDE File Offset: 0x001A3DDE
	private void LogInWithSteam()
	{
		MothershipClientApiUnity.StartLoginWithSteam(delegate(PlayerSteamBeginLoginResponse resp)
		{
			Debug.Log(string.Format("Mothership: Steam Login started at {0}", DateTime.Now));
			string nonce = resp.Nonce;
			SteamAuthTicket ticketHandle = HAuthTicket.Invalid;
			Action<LoginResponse> <>9__4;
			Action<MothershipError, int> <>9__5;
			ticketHandle = this.SteamAuthenticator.GetAuthTicketForWebApi(nonce, delegate(string ticket)
			{
				Debug.Log(string.Format("Mothership: Attempting to complete login at {0}", DateTime.Now));
				string nonce = nonce;
				Action<LoginResponse> successAction;
				if ((successAction = <>9__4) == null)
				{
					successAction = (<>9__4 = delegate(LoginResponse successResp)
					{
						ticketHandle.Dispose();
						Debug.Log("Logged in to Mothership with Steam");
						MothershipClientApiUnity.OpenNotificationsSocket();
						Action onLoginSuccess = this.OnLoginSuccess;
						if (onLoginSuccess == null)
						{
							return;
						}
						onLoginSuccess();
					});
				}
				Action<MothershipError, int> errorAction;
				if ((errorAction = <>9__5) == null)
				{
					errorAction = (<>9__5 = delegate(MothershipError MothershipError, int errorCode)
					{
						ticketHandle.Dispose();
						Debug.LogError(string.Format("Couldn't log into Mothership with Steam error {0} trace ID: {1} status: {2} Mothership error code: {3}", new object[]
						{
							MothershipError.Message,
							MothershipError.TraceId,
							errorCode,
							MothershipError.MothershipErrorCode
						}));
						this.loginAttempts++;
						Action<int> onLoginAttemptFailure = this.OnLoginAttemptFailure;
						if (onLoginAttemptFailure != null)
						{
							onLoginAttemptFailure(this.loginAttempts);
						}
						if (MothershipError.StatusCode != 400 && this.loginAttempts < this.MaxLoginAttempts)
						{
							this.LogInWithSteam();
							return;
						}
						Action<string, string, string> onLoginFailure = this.OnLoginFailure;
						if (onLoginFailure == null)
						{
							return;
						}
						onLoginFailure(MothershipError.Message, MothershipError.MothershipErrorCode, MothershipError.TraceId);
					});
				}
				MothershipClientApiUnity.CompleteLoginWithSteam(nonce, ticket, successAction, errorAction);
			}, delegate(EResult error)
			{
				string text = string.Format("Couldn't get an auth ticket for logging into Mothership with Steam: {0}", error);
				Debug.LogError(text);
				Action<int> onLoginAttemptFailure = this.OnLoginAttemptFailure;
				if (onLoginAttemptFailure != null)
				{
					onLoginAttemptFailure(1);
				}
				Action<string, string, string> onLoginFailure = this.OnLoginFailure;
				if (onLoginFailure == null)
				{
					return;
				}
				onLoginFailure(text, "", "");
			});
		}, delegate(MothershipError MothershipError, int errorCode)
		{
			Debug.LogError(string.Format("Couldn't start Mothership auth for Steam error {0} trace ID: {1} status: {2} Mothership error code: {3}", new object[]
			{
				MothershipError.Message,
				MothershipError.TraceId,
				errorCode,
				MothershipError.MothershipErrorCode
			}));
			Action<int> onLoginAttemptFailure = this.OnLoginAttemptFailure;
			if (onLoginAttemptFailure != null)
			{
				onLoginAttemptFailure(1);
			}
			Action<string, string, string> onLoginFailure = this.OnLoginFailure;
			if (onLoginFailure == null)
			{
				return;
			}
			onLoginFailure(MothershipError.Message, MothershipError.MothershipErrorCode, MothershipError.TraceId);
		});
	}

	// Token: 0x06004F9A RID: 20378 RVA: 0x001A5BFE File Offset: 0x001A3DFE
	public void OnEnable()
	{
		if (MothershipClientApiUnity.IsEnabled())
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			this.lastSliceUpdateTime = Time.unscaledTimeAsDouble;
		}
	}

	// Token: 0x06004F9B RID: 20379 RVA: 0x001A5C19 File Offset: 0x001A3E19
	public void OnDisable()
	{
		if (MothershipClientApiUnity.IsEnabled())
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}
	}

	// Token: 0x06004F9C RID: 20380 RVA: 0x001A5C2C File Offset: 0x001A3E2C
	public void SliceUpdate()
	{
		double unscaledTimeAsDouble = Time.unscaledTimeAsDouble;
		float deltaTime = (float)(unscaledTimeAsDouble - this.lastSliceUpdateTime);
		this.lastSliceUpdateTime = unscaledTimeAsDouble;
		MothershipClientApiUnity.Tick(deltaTime);
	}

	// Token: 0x0400615B RID: 24923
	public static volatile MothershipAuthenticator Instance;

	// Token: 0x0400615C RID: 24924
	public MetaAuthenticator MetaAuthenticator;

	// Token: 0x0400615D RID: 24925
	public SteamAuthenticator SteamAuthenticator;

	// Token: 0x0400615E RID: 24926
	public string TestNickname;

	// Token: 0x0400615F RID: 24927
	public string TestAccountId;

	// Token: 0x04006160 RID: 24928
	public bool UseConstantTestAccountId;

	// Token: 0x04006161 RID: 24929
	private int loginAttempts;

	// Token: 0x04006162 RID: 24930
	public int MaxLoginAttempts = 5;

	// Token: 0x04006163 RID: 24931
	public Action OnLoginSuccess;

	// Token: 0x04006164 RID: 24932
	public Action<string, string, string> OnLoginFailure;

	// Token: 0x04006165 RID: 24933
	public Action<int> OnLoginAttemptFailure;

	// Token: 0x04006166 RID: 24934
	private double lastSliceUpdateTime;
}
