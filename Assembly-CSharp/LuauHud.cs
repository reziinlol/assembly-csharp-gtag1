using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GorillaGameModes;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000C59 RID: 3161
public class LuauHud : MonoBehaviour
{
	// Token: 0x1700072B RID: 1835
	// (get) Token: 0x06004E17 RID: 19991 RVA: 0x0019E580 File Offset: 0x0019C780
	public static LuauHud Instance
	{
		get
		{
			return LuauHud._instance;
		}
	}

	// Token: 0x06004E18 RID: 19992 RVA: 0x0019E588 File Offset: 0x0019C788
	private void Awake()
	{
		if (LuauHud._instance != null && LuauHud._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		LuauHud._instance = this;
		this.path = Path.Combine(Application.persistentDataPath, "script.luau");
	}

	// Token: 0x06004E19 RID: 19993 RVA: 0x0019E5D6 File Offset: 0x0019C7D6
	private void OnDestroy()
	{
		if (LuauHud._instance == this)
		{
			LuauHud._instance = null;
		}
	}

	// Token: 0x06004E1A RID: 19994 RVA: 0x0019E5EC File Offset: 0x0019C7EC
	private void Start()
	{
		this.useLuauHud = true;
		DebugHudStats instance = DebugHudStats.Instance;
		instance.enabled = false;
		this.debugHud = instance.gameObject;
		this.text = instance.text;
		this.text.gameObject.SetActive(false);
		this.text.gameObject.transform.Rotate(180f * Vector3.up, Space.World);
		this.builder = new StringBuilder(50);
	}

	// Token: 0x06004E1B RID: 19995 RVA: 0x0019E668 File Offset: 0x0019C868
	private void Update()
	{
		if (!CustomMapLoader.IsDevModeEnabled())
		{
			if (this.showLog && this.useLuauHud)
			{
				this.showLog = false;
				DebugHudStats instance = DebugHudStats.Instance;
				if (instance != null)
				{
					instance.gameObject.SetActive(false);
				}
				this.text.gameObject.SetActive(false);
			}
			return;
		}
		GorillaGameManager instance2 = GorillaGameManager.instance;
		if (instance2 == null || instance2.GameType() != GameModeType.Custom)
		{
			return;
		}
		bool flag = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
		bool flag2 = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
		if (flag != this.buttonDown && this.useLuauHud)
		{
			this.buttonDown = flag;
			if (!this.buttonDown)
			{
				if (!this.text.gameObject.activeInHierarchy)
				{
					DebugHudStats instance3 = DebugHudStats.Instance;
					if (instance3 != null)
					{
						instance3.gameObject.SetActive(true);
					}
					this.text.gameObject.SetActive(true);
					this.showLog = true;
				}
				else
				{
					DebugHudStats instance4 = DebugHudStats.Instance;
					if (instance4 != null)
					{
						instance4.gameObject.SetActive(false);
					}
					this.text.gameObject.SetActive(false);
					this.showLog = false;
				}
			}
		}
		if (!flag || !flag2)
		{
			this.resetTimer = Time.time;
		}
		if (Time.time - this.resetTimer > 2f && CustomGameMode.GameModeInitialized)
		{
			this.RestartLuauScript();
			this.resetTimer = Time.time;
		}
		if (this.useLuauHud && this.showLog)
		{
			this.builder.Clear();
			this.builder.AppendLine();
			for (int i = 0; i < this.luauLogs.Count; i++)
			{
				this.builder.AppendLine(this.luauLogs[i]);
			}
			this.text.text = this.builder.ToString();
		}
	}

	// Token: 0x06004E1C RID: 19996 RVA: 0x0019E820 File Offset: 0x0019CA20
	public void RestartLuauScript()
	{
		this.LuauLog("Restarting Luau Script");
		LuauScriptRunner gameScriptRunner = CustomGameMode.gameScriptRunner;
		if (gameScriptRunner != null && gameScriptRunner.ShouldTick)
		{
			CustomGameMode.StopScript();
		}
		this.script = this.LoadLocalScript();
		if (this.script != "")
		{
			this.LuauLog("Loaded script from: " + this.path);
			this.LuauLog("Loaded Script Text: \n" + this.script);
			CustomGameMode.LuaScript = this.script;
		}
		CustomGameMode.LuaStart();
	}

	// Token: 0x06004E1D RID: 19997 RVA: 0x0019E8AC File Offset: 0x0019CAAC
	public string LoadLocalScript()
	{
		string result = "";
		if (File.Exists(this.path))
		{
			result = File.ReadAllText(this.path);
		}
		return result;
	}

	// Token: 0x06004E1E RID: 19998 RVA: 0x0019E8D9 File Offset: 0x0019CAD9
	public void LuauLog(string log)
	{
		Debug.Log(log);
		this.luauLogs.Add(log);
		if (this.luauLogs.Count > 6)
		{
			this.luauLogs.RemoveAt(0);
		}
	}

	// Token: 0x0400600C RID: 24588
	private bool useLuauHud;

	// Token: 0x0400600D RID: 24589
	private bool buttonDown;

	// Token: 0x0400600E RID: 24590
	private bool showLog;

	// Token: 0x0400600F RID: 24591
	private GameObject debugHud;

	// Token: 0x04006010 RID: 24592
	private TMP_Text text;

	// Token: 0x04006011 RID: 24593
	private StringBuilder builder;

	// Token: 0x04006012 RID: 24594
	private float resetTimer;

	// Token: 0x04006013 RID: 24595
	private string path = "";

	// Token: 0x04006014 RID: 24596
	private string script = "";

	// Token: 0x04006015 RID: 24597
	private static LuauHud _instance;

	// Token: 0x04006016 RID: 24598
	private List<string> luauLogs = new List<string>();
}
