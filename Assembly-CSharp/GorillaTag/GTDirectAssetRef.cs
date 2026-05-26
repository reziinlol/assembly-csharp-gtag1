using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x02001131 RID: 4401
	[Serializable]
	public struct GTDirectAssetRef<T> : IEquatable<T> where T : Object
	{
		// Token: 0x17000AA9 RID: 2729
		// (get) Token: 0x06006FC6 RID: 28614 RVA: 0x002481BC File Offset: 0x002463BC
		// (set) Token: 0x06006FC7 RID: 28615 RVA: 0x002481C4 File Offset: 0x002463C4
		public T obj
		{
			get
			{
				return this._obj;
			}
			set
			{
				this._obj = value;
				this.edAssetPath = null;
			}
		}

		// Token: 0x06006FC8 RID: 28616 RVA: 0x002481C4 File Offset: 0x002463C4
		public GTDirectAssetRef(T theObj)
		{
			this._obj = theObj;
			this.edAssetPath = null;
		}

		// Token: 0x06006FC9 RID: 28617 RVA: 0x002481D4 File Offset: 0x002463D4
		public static implicit operator T(GTDirectAssetRef<T> refObject)
		{
			return refObject.obj;
		}

		// Token: 0x06006FCA RID: 28618 RVA: 0x002481E0 File Offset: 0x002463E0
		public static implicit operator GTDirectAssetRef<T>(T other)
		{
			return new GTDirectAssetRef<T>
			{
				obj = other
			};
		}

		// Token: 0x06006FCB RID: 28619 RVA: 0x002481FE File Offset: 0x002463FE
		public bool Equals(T other)
		{
			return this.obj == other;
		}

		// Token: 0x06006FCC RID: 28620 RVA: 0x00248218 File Offset: 0x00246418
		public override bool Equals(object other)
		{
			T t = other as T;
			return t != null && this.Equals(t);
		}

		// Token: 0x06006FCD RID: 28621 RVA: 0x00248242 File Offset: 0x00246442
		public override int GetHashCode()
		{
			if (!(this.obj != null))
			{
				return 0;
			}
			return this.obj.GetHashCode();
		}

		// Token: 0x06006FCE RID: 28622 RVA: 0x00248269 File Offset: 0x00246469
		public static bool operator ==(GTDirectAssetRef<T> left, T right)
		{
			return left.Equals(right);
		}

		// Token: 0x06006FCF RID: 28623 RVA: 0x00248273 File Offset: 0x00246473
		public static bool operator !=(GTDirectAssetRef<T> left, T right)
		{
			return !(left == right);
		}

		// Token: 0x04007FC9 RID: 32713
		[SerializeField]
		[HideInInspector]
		internal T _obj;

		// Token: 0x04007FCA RID: 32714
		[FormerlySerializedAs("assetPath")]
		public string edAssetPath;
	}
}
