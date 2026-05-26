using System;
using UnityEngine;

// Token: 0x020004E9 RID: 1257
public class TransformReset : MonoBehaviour
{
	// Token: 0x06001E83 RID: 7811 RVA: 0x000A2DD0 File Offset: 0x000A0FD0
	private void Awake()
	{
		Transform[] componentsInChildren = base.GetComponentsInChildren<Transform>();
		this.transformList = new TransformReset.OriginalGameObjectTransform[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			this.transformList[i] = new TransformReset.OriginalGameObjectTransform(componentsInChildren[i]);
		}
		this.ResetTransforms();
	}

	// Token: 0x06001E84 RID: 7812 RVA: 0x000A2E1C File Offset: 0x000A101C
	public void ReturnTransforms()
	{
		foreach (TransformReset.OriginalGameObjectTransform originalGameObjectTransform in this.tempTransformList)
		{
			originalGameObjectTransform.thisTransform.position = originalGameObjectTransform.thisPosition;
			originalGameObjectTransform.thisTransform.rotation = originalGameObjectTransform.thisRotation;
		}
	}

	// Token: 0x06001E85 RID: 7813 RVA: 0x000A2E6C File Offset: 0x000A106C
	public void SetScale(float ratio)
	{
		foreach (TransformReset.OriginalGameObjectTransform originalGameObjectTransform in this.transformList)
		{
			originalGameObjectTransform.thisTransform.localScale *= ratio;
		}
	}

	// Token: 0x06001E86 RID: 7814 RVA: 0x000A2EB0 File Offset: 0x000A10B0
	public void ResetTransforms()
	{
		this.tempTransformList = new TransformReset.OriginalGameObjectTransform[this.transformList.Length];
		for (int i = 0; i < this.transformList.Length; i++)
		{
			this.tempTransformList[i] = new TransformReset.OriginalGameObjectTransform(this.transformList[i].thisTransform);
		}
		foreach (TransformReset.OriginalGameObjectTransform originalGameObjectTransform in this.transformList)
		{
			originalGameObjectTransform.thisTransform.position = originalGameObjectTransform.thisPosition;
			originalGameObjectTransform.thisTransform.rotation = originalGameObjectTransform.thisRotation;
		}
	}

	// Token: 0x040028C5 RID: 10437
	private TransformReset.OriginalGameObjectTransform[] transformList;

	// Token: 0x040028C6 RID: 10438
	private TransformReset.OriginalGameObjectTransform[] tempTransformList;

	// Token: 0x020004EA RID: 1258
	private struct OriginalGameObjectTransform
	{
		// Token: 0x06001E88 RID: 7816 RVA: 0x000A2F48 File Offset: 0x000A1148
		public OriginalGameObjectTransform(Transform constructionTransform)
		{
			this._thisTransform = constructionTransform;
			this._thisPosition = constructionTransform.position;
			this._thisRotation = constructionTransform.rotation;
		}

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06001E89 RID: 7817 RVA: 0x000A2F69 File Offset: 0x000A1169
		// (set) Token: 0x06001E8A RID: 7818 RVA: 0x000A2F71 File Offset: 0x000A1171
		public Transform thisTransform
		{
			get
			{
				return this._thisTransform;
			}
			set
			{
				this._thisTransform = value;
			}
		}

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x06001E8B RID: 7819 RVA: 0x000A2F7A File Offset: 0x000A117A
		// (set) Token: 0x06001E8C RID: 7820 RVA: 0x000A2F82 File Offset: 0x000A1182
		public Vector3 thisPosition
		{
			get
			{
				return this._thisPosition;
			}
			set
			{
				this._thisPosition = value;
			}
		}

		// Token: 0x17000332 RID: 818
		// (get) Token: 0x06001E8D RID: 7821 RVA: 0x000A2F8B File Offset: 0x000A118B
		// (set) Token: 0x06001E8E RID: 7822 RVA: 0x000A2F93 File Offset: 0x000A1193
		public Quaternion thisRotation
		{
			get
			{
				return this._thisRotation;
			}
			set
			{
				this._thisRotation = value;
			}
		}

		// Token: 0x040028C7 RID: 10439
		private Transform _thisTransform;

		// Token: 0x040028C8 RID: 10440
		private Vector3 _thisPosition;

		// Token: 0x040028C9 RID: 10441
		private Quaternion _thisRotation;
	}
}
