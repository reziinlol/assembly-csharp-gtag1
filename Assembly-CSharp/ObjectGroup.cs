using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000AE4 RID: 2788
public class ObjectGroup : MonoBehaviour
{
	// Token: 0x06004724 RID: 18212 RVA: 0x0017FD04 File Offset: 0x0017DF04
	private void OnEnable()
	{
		if (this.syncWithGroupState)
		{
			this.SetObjectStates(true);
		}
	}

	// Token: 0x06004725 RID: 18213 RVA: 0x0017FD15 File Offset: 0x0017DF15
	private void OnDisable()
	{
		if (this.syncWithGroupState)
		{
			this.SetObjectStates(false);
		}
	}

	// Token: 0x06004726 RID: 18214 RVA: 0x0017FD28 File Offset: 0x0017DF28
	public void SetObjectStates(bool active)
	{
		int count = this.gameObjects.Count;
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = this.gameObjects[i];
			if (!(gameObject == null))
			{
				gameObject.SetActive(active);
			}
		}
		int count2 = this.behaviours.Count;
		for (int j = 0; j < count2; j++)
		{
			Behaviour behaviour = this.behaviours[j];
			if (!(behaviour == null))
			{
				behaviour.enabled = active;
			}
		}
		int count3 = this.renderers.Count;
		for (int k = 0; k < count3; k++)
		{
			Renderer renderer = this.renderers[k];
			if (!(renderer == null))
			{
				renderer.enabled = active;
			}
		}
		int count4 = this.colliders.Count;
		for (int l = 0; l < count4; l++)
		{
			Collider collider = this.colliders[l];
			if (!(collider == null))
			{
				collider.enabled = active;
			}
		}
	}

	// Token: 0x040059A2 RID: 22946
	public List<GameObject> gameObjects = new List<GameObject>(16);

	// Token: 0x040059A3 RID: 22947
	public List<Behaviour> behaviours = new List<Behaviour>(16);

	// Token: 0x040059A4 RID: 22948
	public List<Renderer> renderers = new List<Renderer>(16);

	// Token: 0x040059A5 RID: 22949
	public List<Collider> colliders = new List<Collider>(16);

	// Token: 0x040059A6 RID: 22950
	public bool syncWithGroupState = true;
}
