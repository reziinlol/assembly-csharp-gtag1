using System;
using System.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;

// Token: 0x02000DA8 RID: 3496
public class GraphicsStateCollectionManager : MonoBehaviour
{
	// Token: 0x060055B6 RID: 21942 RVA: 0x001BED0C File Offset: 0x001BCF0C
	private GraphicsStateCollection FindExistingCollection()
	{
		for (int i = 0; i < this.collections.Length; i++)
		{
			if (this.collections[i] != null && this.collections[i].runtimePlatform == Application.platform && this.collections[i].graphicsDeviceType == SystemInfo.graphicsDeviceType && this.collections[i].qualityLevelName == QualitySettings.names[QualitySettings.GetQualityLevel()])
			{
				return this.collections[i];
			}
		}
		return null;
	}

	// Token: 0x060055B7 RID: 21943 RVA: 0x001BED90 File Offset: 0x001BCF90
	private void Awake()
	{
		if (GraphicsStateCollectionManager.Instance != null && GraphicsStateCollectionManager.Instance != this)
		{
			Debug.LogError("Only one instance of GraphicsStateCollectionManager is allowed!");
			Object.Destroy(base.gameObject);
			return;
		}
		GraphicsStateCollectionManager.Instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x060055B8 RID: 21944 RVA: 0x001BEDE0 File Offset: 0x001BCFE0
	private void Start()
	{
		if (this.mode == GraphicsStateCollectionManager.Mode.Tracing)
		{
			this.m_GraphicsStateCollection = this.FindExistingCollection();
			if (this.m_GraphicsStateCollection != null)
			{
				this.m_OutputCollectionName = "SharedAssets/GraphicsStateCollections/" + this.m_GraphicsStateCollection.name;
			}
			else
			{
				int qualityLevel = QualitySettings.GetQualityLevel();
				string text = QualitySettings.names[qualityLevel];
				text = text.Replace(" ", "");
				this.m_OutputCollectionName = string.Concat(new object[]
				{
					"SharedAssets/GraphicsStateCollections/",
					"GfxState_",
					Application.platform,
					"_",
					SystemInfo.graphicsDeviceType.ToString(),
					"_",
					text
				});
				this.m_GraphicsStateCollection = new GraphicsStateCollection();
			}
			Debug.Log("Tracing started for GraphicsStateCollection by Scene '" + SceneManager.GetActiveScene().name + "'.");
			this.m_GraphicsStateCollection.BeginTrace();
			this._autoSaveRoutine = base.StartCoroutine(this.AutoSaveRoutine());
			return;
		}
		GraphicsStateCollection graphicsStateCollection = this.FindExistingCollection();
		if (graphicsStateCollection != null)
		{
			Debug.Log(string.Concat(new string[]
			{
				"Scene '",
				SceneManager.GetActiveScene().name,
				"' started warming up ",
				graphicsStateCollection.totalGraphicsStateCount.ToString(),
				" GraphicsState entries."
			}));
			graphicsStateCollection.WarmUp(default(JobHandle));
		}
	}

	// Token: 0x060055B9 RID: 21945 RVA: 0x001BEF64 File Offset: 0x001BD164
	private void OnApplicationFocus(bool focus)
	{
		if (!focus && this.mode == GraphicsStateCollectionManager.Mode.Tracing && this.m_GraphicsStateCollection != null)
		{
			Debug.Log("Focus changed. Sending collection to Editor with " + this.m_GraphicsStateCollection.totalGraphicsStateCount.ToString() + " GraphicsState entries.");
			this.m_GraphicsStateCollection.SendToEditor(this.m_OutputCollectionName);
		}
	}

	// Token: 0x060055BA RID: 21946 RVA: 0x001BEFC4 File Offset: 0x001BD1C4
	private void OnDestroy()
	{
		if (this._autoSaveRoutine != null)
		{
			base.StopCoroutine(this._autoSaveRoutine);
		}
		if (this.mode == GraphicsStateCollectionManager.Mode.Tracing && this.m_GraphicsStateCollection != null)
		{
			this.m_GraphicsStateCollection.EndTrace();
			Debug.Log("Sending collection to Editor with " + this.m_GraphicsStateCollection.totalGraphicsStateCount.ToString() + " GraphicsState entries.");
			this.m_GraphicsStateCollection.SendToEditor(this.m_OutputCollectionName);
		}
	}

	// Token: 0x060055BB RID: 21947 RVA: 0x001BF03F File Offset: 0x001BD23F
	private IEnumerator AutoSaveRoutine()
	{
		for (;;)
		{
			yield return new WaitForSeconds(5f);
			if (this.mode == GraphicsStateCollectionManager.Mode.Tracing && this.m_GraphicsStateCollection != null)
			{
				Debug.Log("Auto-saving collection with " + this.m_GraphicsStateCollection.totalGraphicsStateCount.ToString() + " GraphicsState entries.");
				this.m_GraphicsStateCollection.SendToEditor(this.m_OutputCollectionName);
			}
		}
		yield break;
	}

	// Token: 0x040065D2 RID: 26066
	public GraphicsStateCollectionManager.Mode mode;

	// Token: 0x040065D3 RID: 26067
	public static GraphicsStateCollectionManager Instance;

	// Token: 0x040065D4 RID: 26068
	public GraphicsStateCollection[] collections;

	// Token: 0x040065D5 RID: 26069
	private const string k_CollectionFolderPath = "SharedAssets/GraphicsStateCollections/";

	// Token: 0x040065D6 RID: 26070
	private string m_OutputCollectionName;

	// Token: 0x040065D7 RID: 26071
	private GraphicsStateCollection m_GraphicsStateCollection;

	// Token: 0x040065D8 RID: 26072
	private Coroutine _autoSaveRoutine;

	// Token: 0x02000DA9 RID: 3497
	public enum Mode
	{
		// Token: 0x040065DA RID: 26074
		Tracing,
		// Token: 0x040065DB RID: 26075
		WarmUp
	}
}
