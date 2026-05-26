using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020003B5 RID: 949
public class ZoneManagement : MonoBehaviour
{
	// Token: 0x14000031 RID: 49
	// (add) Token: 0x060016CF RID: 5839 RVA: 0x0008496C File Offset: 0x00082B6C
	// (remove) Token: 0x060016D0 RID: 5840 RVA: 0x000849A0 File Offset: 0x00082BA0
	public static event ZoneManagement.ZoneChangeEvent OnZoneChange;

	// Token: 0x17000234 RID: 564
	// (get) Token: 0x060016D1 RID: 5841 RVA: 0x000849D3 File Offset: 0x00082BD3
	// (set) Token: 0x060016D2 RID: 5842 RVA: 0x000849DB File Offset: 0x00082BDB
	public bool hasInstance { get; private set; }

	// Token: 0x17000235 RID: 565
	// (get) Token: 0x060016D3 RID: 5843 RVA: 0x000849E4 File Offset: 0x00082BE4
	// (set) Token: 0x060016D4 RID: 5844 RVA: 0x000849EC File Offset: 0x00082BEC
	public bool Initialized { get; private set; }

	// Token: 0x060016D5 RID: 5845 RVA: 0x000849F5 File Offset: 0x00082BF5
	private void Awake()
	{
		if (ZoneManagement.instance == null)
		{
			this.Initialize();
			return;
		}
		if (ZoneManagement.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060016D6 RID: 5846 RVA: 0x00084A23 File Offset: 0x00082C23
	public static void SetActiveZone(GTZone zone)
	{
		ZoneManagement.SetActiveZones(new GTZone[]
		{
			zone
		});
	}

	// Token: 0x060016D7 RID: 5847 RVA: 0x00084A34 File Offset: 0x00082C34
	public static void SetActiveZones(GTZone[] zones)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		if (zones == null || zones.Length == 0)
		{
			return;
		}
		ZoneManagement.instance.SetZones(zones);
		Action action = ZoneManagement.instance.onZoneChanged;
		if (action != null)
		{
			action();
		}
		if (ZoneManagement.OnZoneChange != null)
		{
			ZoneManagement.OnZoneChange(ZoneManagement.instance.zones);
		}
	}

	// Token: 0x060016D8 RID: 5848 RVA: 0x00084A98 File Offset: 0x00082C98
	public static bool IsInZone(GTZone zone)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		ZoneData zoneData = ZoneManagement.instance.GetZoneData(zone);
		return zoneData != null && zoneData.active;
	}

	// Token: 0x060016D9 RID: 5849 RVA: 0x00084AD0 File Offset: 0x00082CD0
	public static bool IsZoneLoaded(GTZone zone)
	{
		if (!ZoneManagement.instance)
		{
			ZoneManagement.FindInstance();
		}
		ZoneData zoneData = ZoneManagement.instance.GetZoneData(zone);
		return zoneData != null && zoneData.active && SceneManager.GetSceneByName(zoneData.sceneName).isLoaded;
	}

	// Token: 0x060016DA RID: 5850 RVA: 0x00084B1A File Offset: 0x00082D1A
	public GameObject GetPrimaryGameObject(GTZone zone)
	{
		return this.GetZoneData(zone).rootGameObjects[0];
	}

	// Token: 0x060016DB RID: 5851 RVA: 0x00084B2A File Offset: 0x00082D2A
	public static void AddSceneToForceStayLoaded(string sceneName)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		ZoneManagement.instance.sceneForceStayLoaded.Add(sceneName);
	}

	// Token: 0x060016DC RID: 5852 RVA: 0x00084B4F File Offset: 0x00082D4F
	public static void RemoveSceneFromForceStayLoaded(string sceneName)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		ZoneManagement.instance.sceneForceStayLoaded.Remove(sceneName);
	}

	// Token: 0x060016DD RID: 5853 RVA: 0x00084B74 File Offset: 0x00082D74
	public static void FindInstance()
	{
		ZoneManagement zoneManagement = Object.FindAnyObjectByType<ZoneManagement>();
		if (zoneManagement == null)
		{
			throw new NullReferenceException("Unable to find ZoneManagement object in scene.");
		}
		Debug.LogWarning("ZoneManagement accessed before MonoBehaviour awake function called; consider delaying zone management functions to avoid FindObject lookup.");
		zoneManagement.Initialize();
	}

	// Token: 0x060016DE RID: 5854 RVA: 0x00084BA0 File Offset: 0x00082DA0
	public bool IsSceneLoaded(GTZone gtZone)
	{
		foreach (ZoneData zoneData in this.zones)
		{
			if (zoneData.zone == gtZone && this.scenesLoaded.Contains(zoneData.sceneName))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060016DF RID: 5855 RVA: 0x00084BE8 File Offset: 0x00082DE8
	public bool IsZoneActive(GTZone zone)
	{
		ZoneData zoneData = this.GetZoneData(zone);
		return zoneData != null && zoneData.active;
	}

	// Token: 0x060016E0 RID: 5856 RVA: 0x00084C08 File Offset: 0x00082E08
	public HashSet<string> GetAllLoadedScenes()
	{
		return this.scenesLoaded;
	}

	// Token: 0x060016E1 RID: 5857 RVA: 0x00084C10 File Offset: 0x00082E10
	public bool IsSceneLoaded(string sceneName)
	{
		return this.scenesLoaded.Contains(sceneName);
	}

	// Token: 0x060016E2 RID: 5858 RVA: 0x00084C20 File Offset: 0x00082E20
	private void Initialize()
	{
		ZoneManagement.instance = this;
		this.hasInstance = true;
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		List<GameObject> list = new List<GameObject>(8);
		for (int i = 0; i < this.zones.Length; i++)
		{
			list.Clear();
			ZoneData zoneData = this.zones[i];
			if (zoneData != null && zoneData.rootGameObjects != null)
			{
				hashSet.UnionWith(zoneData.rootGameObjects);
				for (int j = 0; j < zoneData.rootGameObjects.Length; j++)
				{
					GameObject gameObject = zoneData.rootGameObjects[j];
					if (!(gameObject == null))
					{
						list.Add(gameObject);
					}
				}
				hashSet.UnionWith(list);
			}
		}
		this.allObjects = hashSet.ToArray<GameObject>();
		this.objectActivationState = new bool[this.allObjects.Length];
		ZoneManagement.AddSceneToForceStayLoaded("City");
		this.Initialized = true;
	}

	// Token: 0x060016E3 RID: 5859 RVA: 0x00084CEC File Offset: 0x00082EEC
	private void SetZones(GTZone[] newActiveZones)
	{
		for (int i = 0; i < this.objectActivationState.Length; i++)
		{
			this.objectActivationState[i] = false;
		}
		this.activeZones.Clear();
		for (int j = 0; j < newActiveZones.Length; j++)
		{
			this.activeZones.Add(newActiveZones[j]);
		}
		this.scenesRequested.Clear();
		this.scenesRequested.Add("GorillaTag");
		float num = 0f;
		for (int k = 0; k < this.zones.Length; k++)
		{
			ZoneData zoneData = this.zones[k];
			if (zoneData != null)
			{
				if (zoneData.rootGameObjects == null || !newActiveZones.Contains(zoneData.zone))
				{
					zoneData.active = false;
				}
				else
				{
					zoneData.active = true;
					num = Mathf.Max(num, zoneData.CameraFarClipPlane);
					if (!string.IsNullOrEmpty(zoneData.sceneName))
					{
						this.scenesRequested.Add(zoneData.sceneName);
					}
					foreach (GameObject x in zoneData.rootGameObjects)
					{
						if (!(x == null))
						{
							for (int m = 0; m < this.allObjects.Length; m++)
							{
								if (x == this.allObjects[m])
								{
									this.objectActivationState[m] = true;
									break;
								}
							}
						}
					}
				}
			}
		}
		if (this.mainCamera == null)
		{
			this.mainCamera = Camera.main;
		}
		this.mainCamera.farClipPlane = num;
		int loadedSceneCount = SceneManager.loadedSceneCount;
		for (int n = 0; n < loadedSceneCount; n++)
		{
			this.scenesLoaded.Add(SceneManager.GetSceneAt(n).name);
		}
		foreach (string text in this.scenesRequested)
		{
			if (this.scenesLoaded.Add(text))
			{
				AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(text, LoadSceneMode.Additive);
				this._scenes_to_loadOps[text] = asyncOperation;
				asyncOperation.completed += this.HandleOnSceneLoadCompleted;
			}
		}
		this.scenesToUnload.Clear();
		foreach (string item in this.scenesLoaded)
		{
			if (!this.scenesRequested.Contains(item) && !this.sceneForceStayLoaded.Contains(item))
			{
				this.scenesToUnload.Add(item);
			}
		}
		foreach (string text2 in this.scenesToUnload)
		{
			this.scenesLoaded.Remove(text2);
			AsyncOperation value = SceneManager.UnloadSceneAsync(text2);
			this._scenes_to_unloadOps[text2] = value;
		}
		for (int num2 = 0; num2 < this.objectActivationState.Length; num2++)
		{
			if (!(this.allObjects[num2] == null))
			{
				this.allObjects[num2].SetActive(this.objectActivationState[num2]);
			}
		}
	}

	// Token: 0x060016E4 RID: 5860 RVA: 0x00085034 File Offset: 0x00083234
	private void HandleOnSceneLoadCompleted(AsyncOperation thisLoadOp)
	{
		foreach (KeyValuePair<string, AsyncOperation> keyValuePair in this._scenes_to_loadOps)
		{
			string text;
			AsyncOperation asyncOperation;
			keyValuePair.Deconstruct(out text, out asyncOperation);
			string str = text;
			AsyncOperation asyncOperation2 = asyncOperation;
			if (asyncOperation2 == null)
			{
				Debug.LogError("ERROR!!!  HandleOnSceneLoadCompleted: Why is `loadOp` null in `_scenes_to_loadOps` for scene \"" + str + "\"?????");
			}
			else if (!asyncOperation2.isDone)
			{
				return;
			}
		}
		foreach (KeyValuePair<string, AsyncOperation> keyValuePair in this._scenes_to_unloadOps)
		{
			string text;
			AsyncOperation asyncOperation;
			keyValuePair.Deconstruct(out text, out asyncOperation);
			string str2 = text;
			AsyncOperation asyncOperation3 = asyncOperation;
			if (asyncOperation3 == null)
			{
				Debug.LogError("ERROR!!!  HandleOnSceneLoadCompleted: Why is `unloadOps` null in `_scenes_to_unloadOps` for scene \"" + str2 + "\"?????");
			}
			else if (!asyncOperation3.isDone)
			{
				return;
			}
		}
		Action onSceneLoadsCompleted = this.OnSceneLoadsCompleted;
		if (onSceneLoadsCompleted == null)
		{
			return;
		}
		onSceneLoadsCompleted();
	}

	// Token: 0x060016E5 RID: 5861 RVA: 0x00085140 File Offset: 0x00083340
	public bool AnyActiveLoadOps()
	{
		return this._scenes_to_loadOps.Values.Any((AsyncOperation op) => !op.isDone);
	}

	// Token: 0x060016E6 RID: 5862 RVA: 0x00085174 File Offset: 0x00083374
	private ZoneData GetZoneData(GTZone zone)
	{
		for (int i = 0; i < this.zones.Length; i++)
		{
			if (this.zones[i].zone == zone)
			{
				return this.zones[i];
			}
		}
		return null;
	}

	// Token: 0x060016E7 RID: 5863 RVA: 0x000851AE File Offset: 0x000833AE
	public string GetSceneNameForZone(GTZone zone)
	{
		ZoneData zoneData = this.GetZoneData(zone);
		if (zoneData == null)
		{
			return null;
		}
		return zoneData.sceneName;
	}

	// Token: 0x060016E8 RID: 5864 RVA: 0x000851C2 File Offset: 0x000833C2
	public static bool IsValidZoneInt(int zoneInt)
	{
		return zoneInt >= 11 && zoneInt <= 24;
	}

	// Token: 0x04002208 RID: 8712
	private const string preLog = "[GT/ZoneManagement]  ";

	// Token: 0x04002209 RID: 8713
	private const string preErr = "ERROR!!!  ";

	// Token: 0x0400220A RID: 8714
	private const string preErrBeta = "(beta only log)  ";

	// Token: 0x0400220C RID: 8716
	public static ZoneManagement instance;

	// Token: 0x0400220F RID: 8719
	[SerializeField]
	private ZoneData[] zones;

	// Token: 0x04002210 RID: 8720
	private GameObject[] allObjects;

	// Token: 0x04002211 RID: 8721
	private bool[] objectActivationState;

	// Token: 0x04002212 RID: 8722
	public Action onZoneChanged;

	// Token: 0x04002213 RID: 8723
	public Action OnSceneLoadsCompleted;

	// Token: 0x04002214 RID: 8724
	public List<GTZone> activeZones = new List<GTZone>(20);

	// Token: 0x04002215 RID: 8725
	private HashSet<string> scenesLoaded = new HashSet<string>();

	// Token: 0x04002216 RID: 8726
	private HashSet<string> scenesRequested = new HashSet<string>();

	// Token: 0x04002217 RID: 8727
	private HashSet<string> sceneForceStayLoaded = new HashSet<string>(8);

	// Token: 0x04002218 RID: 8728
	private List<string> scenesToUnload = new List<string>();

	// Token: 0x04002219 RID: 8729
	private Dictionary<string, AsyncOperation> _scenes_to_loadOps = new Dictionary<string, AsyncOperation>(32);

	// Token: 0x0400221A RID: 8730
	private Dictionary<string, AsyncOperation> _scenes_to_unloadOps = new Dictionary<string, AsyncOperation>(32);

	// Token: 0x0400221B RID: 8731
	private Camera mainCamera;

	// Token: 0x020003B6 RID: 950
	// (Invoke) Token: 0x060016EB RID: 5867
	public delegate void ZoneChangeEvent(ZoneData[] zones);
}
