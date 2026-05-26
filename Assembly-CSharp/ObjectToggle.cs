using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000AE5 RID: 2789
public class ObjectToggle : MonoBehaviour
{
	// Token: 0x06004728 RID: 18216 RVA: 0x0017FE7A File Offset: 0x0017E07A
	public void Toggle(bool initialState = true)
	{
		if (this._toggled == null)
		{
			if (initialState)
			{
				this.Enable();
				return;
			}
			this.Disable();
			return;
		}
		else
		{
			if (this._toggled.Value)
			{
				this.Disable();
				return;
			}
			this.Enable();
			return;
		}
	}

	// Token: 0x06004729 RID: 18217 RVA: 0x0017FEB4 File Offset: 0x0017E0B4
	public void Enable()
	{
		if (this.objectsToToggle == null)
		{
			return;
		}
		for (int i = 0; i < this.objectsToToggle.Count; i++)
		{
			GameObject gameObject = this.objectsToToggle[i];
			if (!(gameObject == null))
			{
				if (this._ignoreHierarchyState)
				{
					gameObject.SetActive(true);
				}
				else if (!gameObject.activeInHierarchy)
				{
					gameObject.SetActive(true);
				}
			}
		}
		this._toggled = new bool?(true);
	}

	// Token: 0x0600472A RID: 18218 RVA: 0x0017FF24 File Offset: 0x0017E124
	public void Disable()
	{
		if (this.objectsToToggle == null)
		{
			return;
		}
		for (int i = 0; i < this.objectsToToggle.Count; i++)
		{
			GameObject gameObject = this.objectsToToggle[i];
			if (!(gameObject == null))
			{
				if (this._ignoreHierarchyState)
				{
					gameObject.SetActive(false);
				}
				else if (gameObject.activeInHierarchy)
				{
					gameObject.SetActive(false);
				}
			}
		}
		this._toggled = new bool?(false);
	}

	// Token: 0x040059A7 RID: 22951
	public List<GameObject> objectsToToggle = new List<GameObject>();

	// Token: 0x040059A8 RID: 22952
	[SerializeField]
	private bool _ignoreHierarchyState;

	// Token: 0x040059A9 RID: 22953
	[NonSerialized]
	private bool? _toggled;
}
