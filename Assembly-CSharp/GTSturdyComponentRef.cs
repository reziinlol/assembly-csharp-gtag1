using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200030B RID: 779
[Serializable]
public struct GTSturdyComponentRef<T> where T : Component
{
	// Token: 0x170001F4 RID: 500
	// (get) Token: 0x060013B9 RID: 5049 RVA: 0x0006B2AD File Offset: 0x000694AD
	// (set) Token: 0x060013BA RID: 5050 RVA: 0x0006B2B5 File Offset: 0x000694B5
	public Transform BaseXform
	{
		get
		{
			return this._baseXform;
		}
		set
		{
			this._baseXform = value;
		}
	}

	// Token: 0x170001F5 RID: 501
	// (get) Token: 0x060013BB RID: 5051 RVA: 0x0006B2C0 File Offset: 0x000694C0
	// (set) Token: 0x060013BC RID: 5052 RVA: 0x0006B32F File Offset: 0x0006952F
	public T Value
	{
		get
		{
			if (!this._value)
			{
				return this._value;
			}
			if (string.IsNullOrEmpty(this._relativePath))
			{
				return default(T);
			}
			Transform transform;
			if (!this._baseXform.TryFindByPath(this._relativePath, out transform, false))
			{
				return default(T);
			}
			this._value = transform.GetComponent<T>();
			return this._value;
		}
		set
		{
			this._value = value;
			this._relativePath = ((!value) ? this._baseXform.GetRelativePath(value.transform) : string.Empty);
		}
	}

	// Token: 0x060013BD RID: 5053 RVA: 0x0006B368 File Offset: 0x00069568
	public static implicit operator T(GTSturdyComponentRef<T> sturdyRef)
	{
		return sturdyRef.Value;
	}

	// Token: 0x060013BE RID: 5054 RVA: 0x0006B374 File Offset: 0x00069574
	public static implicit operator GTSturdyComponentRef<T>(T component)
	{
		return new GTSturdyComponentRef<T>
		{
			Value = component
		};
	}

	// Token: 0x04001854 RID: 6228
	[SerializeField]
	private T _value;

	// Token: 0x04001855 RID: 6229
	[SerializeField]
	private string _relativePath;

	// Token: 0x04001856 RID: 6230
	[SerializeField]
	private Transform _baseXform;
}
