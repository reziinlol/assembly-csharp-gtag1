using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020000B3 RID: 179
public class EyeScannableMono : MonoBehaviour, IEyeScannable
{
	// Token: 0x1400000D RID: 13
	// (add) Token: 0x0600044F RID: 1103 RVA: 0x00018F40 File Offset: 0x00017140
	// (remove) Token: 0x06000450 RID: 1104 RVA: 0x00018F78 File Offset: 0x00017178
	public event Action OnDataChange;

	// Token: 0x17000049 RID: 73
	// (get) Token: 0x06000451 RID: 1105 RVA: 0x00018FAD File Offset: 0x000171AD
	int IEyeScannable.scannableId
	{
		get
		{
			return base.GetInstanceID();
		}
	}

	// Token: 0x1700004A RID: 74
	// (get) Token: 0x06000452 RID: 1106 RVA: 0x00018FB5 File Offset: 0x000171B5
	Vector3 IEyeScannable.Position
	{
		get
		{
			return base.transform.position - this._initialPosition + this._bounds.center;
		}
	}

	// Token: 0x1700004B RID: 75
	// (get) Token: 0x06000453 RID: 1107 RVA: 0x00018FDD File Offset: 0x000171DD
	Bounds IEyeScannable.Bounds
	{
		get
		{
			return this._bounds;
		}
	}

	// Token: 0x1700004C RID: 76
	// (get) Token: 0x06000454 RID: 1108 RVA: 0x00018FE5 File Offset: 0x000171E5
	IList<KeyValueStringPair> IEyeScannable.Entries
	{
		get
		{
			return this.data.Entries;
		}
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x00018FF2 File Offset: 0x000171F2
	private void Awake()
	{
		this.RecalculateBounds();
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x00018FFA File Offset: 0x000171FA
	public void OnEnable()
	{
		this.RecalculateBoundsLater();
		EyeScannerMono.Register(this);
	}

	// Token: 0x06000457 RID: 1111 RVA: 0x0001464D File Offset: 0x0001284D
	public void OnDisable()
	{
		EyeScannerMono.Unregister(this);
	}

	// Token: 0x06000458 RID: 1112 RVA: 0x00019008 File Offset: 0x00017208
	private void RecalculateBoundsLater()
	{
		EyeScannableMono.<RecalculateBoundsLater>d__17 <RecalculateBoundsLater>d__;
		<RecalculateBoundsLater>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RecalculateBoundsLater>d__.<>4__this = this;
		<RecalculateBoundsLater>d__.<>1__state = -1;
		<RecalculateBoundsLater>d__.<>t__builder.Start<EyeScannableMono.<RecalculateBoundsLater>d__17>(ref <RecalculateBoundsLater>d__);
	}

	// Token: 0x06000459 RID: 1113 RVA: 0x00019040 File Offset: 0x00017240
	private void RecalculateBounds()
	{
		this._initialPosition = base.transform.position;
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		this._bounds = default(Bounds);
		if (componentsInChildren.Length == 0)
		{
			this._bounds.center = base.transform.position;
			this._bounds.Expand(1f);
			return;
		}
		this._bounds = componentsInChildren[0].bounds;
		for (int i = 1; i < componentsInChildren.Length; i++)
		{
			this._bounds.Encapsulate(componentsInChildren[i].bounds);
		}
	}

	// Token: 0x040004B7 RID: 1207
	[SerializeField]
	private KeyValuePairSet data;

	// Token: 0x040004B8 RID: 1208
	private Bounds _bounds;

	// Token: 0x040004B9 RID: 1209
	private Vector3 _initialPosition;
}
