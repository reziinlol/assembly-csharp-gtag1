using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x02001139 RID: 4409
	[Serializable]
	public struct XformOffset
	{
		// Token: 0x17000AB5 RID: 2741
		// (get) Token: 0x06006FF8 RID: 28664 RVA: 0x00248CA1 File Offset: 0x00246EA1
		// (set) Token: 0x06006FF9 RID: 28665 RVA: 0x00248CA9 File Offset: 0x00246EA9
		[Tooltip("The rotation of the offset relative to the parent bone.")]
		public Quaternion rot
		{
			get
			{
				return this._rotQuat;
			}
			set
			{
				this._rotQuat = value;
			}
		}

		// Token: 0x06006FFA RID: 28666 RVA: 0x00248CB2 File Offset: 0x00246EB2
		public XformOffset(Vector3 pos, Quaternion rot, Vector3 scale)
		{
			this.pos = pos;
			this._rotQuat = rot;
			this._rotEulerAngles = rot.eulerAngles;
			this.scale = scale;
		}

		// Token: 0x06006FFB RID: 28667 RVA: 0x00248CD6 File Offset: 0x00246ED6
		public XformOffset(Vector3 pos, Vector3 rot, Vector3 scale)
		{
			this.pos = pos;
			this._rotQuat = Quaternion.Euler(rot);
			this._rotEulerAngles = rot;
			this.scale = scale;
		}

		// Token: 0x06006FFC RID: 28668 RVA: 0x00248CF9 File Offset: 0x00246EF9
		public XformOffset(Vector3 pos, Quaternion rot)
		{
			this.pos = pos;
			this._rotQuat = rot;
			this._rotEulerAngles = rot.eulerAngles;
			this.scale = Vector3.one;
		}

		// Token: 0x06006FFD RID: 28669 RVA: 0x00248D21 File Offset: 0x00246F21
		public XformOffset(Vector3 pos, Vector3 rot)
		{
			this.pos = pos;
			this._rotQuat = Quaternion.Euler(rot);
			this._rotEulerAngles = rot;
			this.scale = Vector3.one;
		}

		// Token: 0x06006FFE RID: 28670 RVA: 0x00248D48 File Offset: 0x00246F48
		public XformOffset(Transform parentXform, Transform childXform)
		{
			this.pos = parentXform.InverseTransformPoint(childXform.position);
			this._rotQuat = Quaternion.Inverse(parentXform.rotation) * childXform.rotation;
			this._rotEulerAngles = this._rotQuat.eulerAngles;
			this.scale = childXform.lossyScale.SafeDivide(parentXform.lossyScale);
		}

		// Token: 0x06006FFF RID: 28671 RVA: 0x00248DAC File Offset: 0x00246FAC
		public XformOffset(Matrix4x4 matrix)
		{
			this.pos = matrix.GetPosition();
			this.scale = matrix.lossyScale;
			if (Vector3.Dot(Vector3.Cross(matrix.GetColumn(0), matrix.GetColumn(1)), matrix.GetColumn(2)) < 0f)
			{
				this.scale = -this.scale;
			}
			Matrix4x4 matrix4x = matrix;
			matrix4x.SetColumn(0, matrix4x.GetColumn(0) / this.scale.x);
			matrix4x.SetColumn(1, matrix4x.GetColumn(1) / this.scale.y);
			matrix4x.SetColumn(2, matrix4x.GetColumn(2) / this.scale.z);
			this._rotQuat = Quaternion.LookRotation(matrix4x.GetColumn(2), matrix4x.GetColumn(1));
			this._rotEulerAngles = this._rotQuat.eulerAngles;
		}

		// Token: 0x06007000 RID: 28672 RVA: 0x00248EB4 File Offset: 0x002470B4
		public bool Approx(XformOffset other)
		{
			return this.pos.Approx(other.pos, 1E-05f) && this._rotQuat.Approx(other._rotQuat, 1E-06f) && this.scale.Approx(other.scale, 1E-05f);
		}

		// Token: 0x04007FFB RID: 32763
		[Tooltip("The position of the offset relative to the parent bone.")]
		public Vector3 pos;

		// Token: 0x04007FFC RID: 32764
		[FormerlySerializedAs("_edRotQuat")]
		[FormerlySerializedAs("rot")]
		[HideInInspector]
		[SerializeField]
		private Quaternion _rotQuat;

		// Token: 0x04007FFD RID: 32765
		[FormerlySerializedAs("_edRotEulerAngles")]
		[FormerlySerializedAs("_edRotEuler")]
		[HideInInspector]
		[SerializeField]
		private Vector3 _rotEulerAngles;

		// Token: 0x04007FFE RID: 32766
		[Tooltip("The scale of the offset relative to the parent bone.")]
		public Vector3 scale;

		// Token: 0x04007FFF RID: 32767
		public static readonly XformOffset Identity = new XformOffset
		{
			_rotQuat = Quaternion.identity,
			scale = Vector3.one
		};
	}
}
