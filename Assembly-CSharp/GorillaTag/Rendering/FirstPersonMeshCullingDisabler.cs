using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x020011FE RID: 4606
	public class FirstPersonMeshCullingDisabler : MonoBehaviour
	{
		// Token: 0x0600738A RID: 29578 RVA: 0x00258BC4 File Offset: 0x00256DC4
		protected void Awake()
		{
			MeshFilter[] componentsInChildren = base.GetComponentsInChildren<MeshFilter>();
			if (componentsInChildren == null)
			{
				return;
			}
			this.meshes = new Mesh[componentsInChildren.Length];
			this.xforms = new Transform[componentsInChildren.Length];
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				this.meshes[i] = componentsInChildren[i].mesh;
				this.xforms[i] = componentsInChildren[i].transform;
			}
		}

		// Token: 0x0600738B RID: 29579 RVA: 0x00258C28 File Offset: 0x00256E28
		protected void OnEnable()
		{
			Camera main = Camera.main;
			if (main == null)
			{
				return;
			}
			Transform transform = main.transform;
			Vector3 position = transform.position;
			Vector3 a = Vector3.Normalize(transform.forward);
			float nearClipPlane = main.nearClipPlane;
			float d = (main.farClipPlane - nearClipPlane) / 2f + nearClipPlane;
			Vector3 position2 = position + a * d;
			for (int i = 0; i < this.meshes.Length; i++)
			{
				Vector3 center = this.xforms[i].InverseTransformPoint(position2);
				this.meshes[i].bounds = new Bounds(center, Vector3.one);
			}
		}

		// Token: 0x040083E4 RID: 33764
		private Mesh[] meshes;

		// Token: 0x040083E5 RID: 33765
		private Transform[] xforms;
	}
}
