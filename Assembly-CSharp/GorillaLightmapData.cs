using System;
using UnityEngine;

// Token: 0x02000879 RID: 2169
public class GorillaLightmapData : MonoBehaviour
{
	// Token: 0x0600388B RID: 14475 RVA: 0x001353A0 File Offset: 0x001335A0
	public void Awake()
	{
		this.lights = new Color[this.lightTextures.Length][];
		this.dirs = new Color[this.dirTextures.Length][];
		for (int i = 0; i < this.dirTextures.Length; i++)
		{
			float value = Random.value;
			Debug.Log(value.ToString() + " before load " + Time.realtimeSinceStartup.ToString());
			this.dirs[i] = this.dirTextures[i].GetPixels();
			this.lights[i] = this.lightTextures[i].GetPixels();
			Debug.Log(value.ToString() + " after load " + Time.realtimeSinceStartup.ToString());
		}
	}

	// Token: 0x0400488F RID: 18575
	[SerializeField]
	public Texture2D[] dirTextures;

	// Token: 0x04004890 RID: 18576
	[SerializeField]
	public Texture2D[] lightTextures;

	// Token: 0x04004891 RID: 18577
	public Color[][] lights;

	// Token: 0x04004892 RID: 18578
	public Color[][] dirs;

	// Token: 0x04004893 RID: 18579
	public bool done;
}
