using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000DB4 RID: 3508
[Serializable]
public class GTScene : IEquatable<GTScene>
{
	// Token: 0x1700080B RID: 2059
	// (get) Token: 0x060055EC RID: 21996 RVA: 0x001BF7A8 File Offset: 0x001BD9A8
	public string alias
	{
		get
		{
			return this._alias;
		}
	}

	// Token: 0x1700080C RID: 2060
	// (get) Token: 0x060055ED RID: 21997 RVA: 0x001BF7B0 File Offset: 0x001BD9B0
	public string name
	{
		get
		{
			return this._name;
		}
	}

	// Token: 0x1700080D RID: 2061
	// (get) Token: 0x060055EE RID: 21998 RVA: 0x001BF7B8 File Offset: 0x001BD9B8
	public string path
	{
		get
		{
			return this._path;
		}
	}

	// Token: 0x1700080E RID: 2062
	// (get) Token: 0x060055EF RID: 21999 RVA: 0x001BF7C0 File Offset: 0x001BD9C0
	public string guid
	{
		get
		{
			return this._guid;
		}
	}

	// Token: 0x1700080F RID: 2063
	// (get) Token: 0x060055F0 RID: 22000 RVA: 0x001BF7C8 File Offset: 0x001BD9C8
	public int buildIndex
	{
		get
		{
			return this._buildIndex;
		}
	}

	// Token: 0x17000810 RID: 2064
	// (get) Token: 0x060055F1 RID: 22001 RVA: 0x001BF7D0 File Offset: 0x001BD9D0
	public bool includeInBuild
	{
		get
		{
			return this._includeInBuild;
		}
	}

	// Token: 0x17000811 RID: 2065
	// (get) Token: 0x060055F2 RID: 22002 RVA: 0x001BF7D8 File Offset: 0x001BD9D8
	public bool isLoaded
	{
		get
		{
			return SceneManager.GetSceneByBuildIndex(this._buildIndex).isLoaded;
		}
	}

	// Token: 0x17000812 RID: 2066
	// (get) Token: 0x060055F3 RID: 22003 RVA: 0x001BF7F8 File Offset: 0x001BD9F8
	public bool hasAlias
	{
		get
		{
			return !string.IsNullOrWhiteSpace(this._alias);
		}
	}

	// Token: 0x060055F4 RID: 22004 RVA: 0x001BF808 File Offset: 0x001BDA08
	public GTScene(string name, string path, string guid, int buildIndex, bool includeInBuild)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentNullException("name");
		}
		if (string.IsNullOrWhiteSpace(path))
		{
			throw new ArgumentNullException("path");
		}
		if (string.IsNullOrWhiteSpace(guid))
		{
			throw new ArgumentNullException("guid");
		}
		this._name = name;
		this._path = path;
		this._guid = guid;
		this._buildIndex = buildIndex;
		this._includeInBuild = includeInBuild;
	}

	// Token: 0x060055F5 RID: 22005 RVA: 0x001BF879 File Offset: 0x001BDA79
	public override int GetHashCode()
	{
		return this._guid.GetHashCode();
	}

	// Token: 0x060055F6 RID: 22006 RVA: 0x001BF886 File Offset: 0x001BDA86
	public override string ToString()
	{
		return this.ToJson(false);
	}

	// Token: 0x060055F7 RID: 22007 RVA: 0x001BF88F File Offset: 0x001BDA8F
	public bool Equals(GTScene other)
	{
		return this._guid.Equals(other._guid) && this._name == other._name && this._path == other._path;
	}

	// Token: 0x060055F8 RID: 22008 RVA: 0x001BF8CC File Offset: 0x001BDACC
	public override bool Equals(object obj)
	{
		GTScene gtscene = obj as GTScene;
		return gtscene != null && this.Equals(gtscene);
	}

	// Token: 0x060055F9 RID: 22009 RVA: 0x001BF8EC File Offset: 0x001BDAEC
	public static bool operator ==(GTScene x, GTScene y)
	{
		return x.Equals(y);
	}

	// Token: 0x060055FA RID: 22010 RVA: 0x001BF8F5 File Offset: 0x001BDAF5
	public static bool operator !=(GTScene x, GTScene y)
	{
		return !x.Equals(y);
	}

	// Token: 0x060055FB RID: 22011 RVA: 0x001BF901 File Offset: 0x001BDB01
	public void LoadAsync()
	{
		if (this.isLoaded)
		{
			return;
		}
		SceneManager.LoadSceneAsync(this._buildIndex, LoadSceneMode.Additive);
	}

	// Token: 0x060055FC RID: 22012 RVA: 0x001BF919 File Offset: 0x001BDB19
	public void UnloadAsync()
	{
		if (!this.isLoaded)
		{
			return;
		}
		SceneManager.UnloadSceneAsync(this._buildIndex, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
	}

	// Token: 0x060055FD RID: 22013 RVA: 0x00035D0D File Offset: 0x00033F0D
	public static GTScene FromAsset(object sceneAsset)
	{
		return null;
	}

	// Token: 0x060055FE RID: 22014 RVA: 0x00035D0D File Offset: 0x00033F0D
	public static GTScene From(object editorBuildSettingsScene)
	{
		return null;
	}

	// Token: 0x04006601 RID: 26113
	[SerializeField]
	private string _alias;

	// Token: 0x04006602 RID: 26114
	[SerializeField]
	private string _name;

	// Token: 0x04006603 RID: 26115
	[SerializeField]
	private string _path;

	// Token: 0x04006604 RID: 26116
	[SerializeField]
	private string _guid;

	// Token: 0x04006605 RID: 26117
	[SerializeField]
	private int _buildIndex;

	// Token: 0x04006606 RID: 26118
	[SerializeField]
	private bool _includeInBuild;
}
