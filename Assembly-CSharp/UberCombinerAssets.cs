using System;
using UnityEngine;

// Token: 0x02000DF7 RID: 3575
public class UberCombinerAssets : ScriptableObject
{
	// Token: 0x1700083D RID: 2109
	// (get) Token: 0x06005780 RID: 22400 RVA: 0x001C4FC7 File Offset: 0x001C31C7
	public static UberCombinerAssets Instance
	{
		get
		{
			UberCombinerAssets.gInstance == null;
			return UberCombinerAssets.gInstance;
		}
	}

	// Token: 0x06005781 RID: 22401 RVA: 0x001C4FDA File Offset: 0x001C31DA
	private void OnEnable()
	{
		this.Setup();
	}

	// Token: 0x06005782 RID: 22402 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Setup()
	{
	}

	// Token: 0x06005783 RID: 22403 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void ClearMaterialAssets()
	{
	}

	// Token: 0x06005784 RID: 22404 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void ClearPrefabAssets()
	{
	}

	// Token: 0x04006782 RID: 26498
	[SerializeField]
	private Object _rootFolder;

	// Token: 0x04006783 RID: 26499
	[SerializeField]
	private Object _resourcesFolder;

	// Token: 0x04006784 RID: 26500
	[SerializeField]
	private Object _materialsFolder;

	// Token: 0x04006785 RID: 26501
	[SerializeField]
	private Object _prefabsFolder;

	// Token: 0x04006786 RID: 26502
	[Space]
	public Object MeshBakerDefaultCustomizer;

	// Token: 0x04006787 RID: 26503
	public Material ReferenceUberMaterial;

	// Token: 0x04006788 RID: 26504
	public Shader TextureArrayCapableShader;

	// Token: 0x04006789 RID: 26505
	[Space]
	public string RootFolderPath;

	// Token: 0x0400678A RID: 26506
	public string ResourcesFolderPath;

	// Token: 0x0400678B RID: 26507
	public string MaterialsFolderPath;

	// Token: 0x0400678C RID: 26508
	public string PrefabsFolderPath;

	// Token: 0x0400678D RID: 26509
	private static UberCombinerAssets gInstance;
}
