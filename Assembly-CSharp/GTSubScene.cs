using System;
using UnityEngine;

// Token: 0x02000DB6 RID: 3510
public class GTSubScene : ScriptableObject
{
	// Token: 0x06005603 RID: 22019 RVA: 0x001BF95C File Offset: 0x001BDB5C
	public void SwitchToScene(int index)
	{
		this.scenes[index].LoadAsync();
	}

	// Token: 0x06005604 RID: 22020 RVA: 0x001BF96C File Offset: 0x001BDB6C
	public void SwitchToScene(GTScene scene)
	{
		for (int i = 0; i < this.scenes.Length; i++)
		{
			GTScene gtscene = this.scenes[i];
			if (!(scene == gtscene))
			{
				gtscene.UnloadAsync();
			}
		}
		scene.LoadAsync();
	}

	// Token: 0x06005605 RID: 22021 RVA: 0x001BF9AC File Offset: 0x001BDBAC
	public void LoadAll()
	{
		for (int i = 0; i < this.scenes.Length; i++)
		{
			this.scenes[i].LoadAsync();
		}
	}

	// Token: 0x06005606 RID: 22022 RVA: 0x001BF9DC File Offset: 0x001BDBDC
	public void UnloadAll()
	{
		for (int i = 0; i < this.scenes.Length; i++)
		{
			this.scenes[i].UnloadAsync();
		}
	}

	// Token: 0x04006607 RID: 26119
	[DragDropScenes]
	public GTScene[] scenes = new GTScene[0];
}
