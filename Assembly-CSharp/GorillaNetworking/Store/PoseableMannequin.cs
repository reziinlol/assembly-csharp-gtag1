using System;
using System.Collections;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaNetworking.Store
{
	// Token: 0x020010AC RID: 4268
	public class PoseableMannequin : MonoBehaviour
	{
		// Token: 0x06006B13 RID: 27411 RVA: 0x0022A5C9 File Offset: 0x002287C9
		public void Start()
		{
			if (this.skinnedMeshRenderer)
			{
				this.skinnedMeshRenderer.gameObject.SetActive(false);
			}
			if (this.staticGorillaMesh)
			{
				this.staticGorillaMesh.gameObject.SetActive(true);
			}
		}

		// Token: 0x06006B14 RID: 27412 RVA: 0x0016D2F3 File Offset: 0x0016B4F3
		private string GetPrefabPathFromCurrentPrefabStage()
		{
			return "";
		}

		// Token: 0x06006B15 RID: 27413 RVA: 0x0016D2F3 File Offset: 0x0016B4F3
		private string GetMeshPathFromPrefabPath(string prefabPath)
		{
			return "";
		}

		// Token: 0x06006B16 RID: 27414 RVA: 0x0022A607 File Offset: 0x00228807
		public void BakeSkinnedMesh()
		{
			this.BakeAndSaveMeshInPath(this.GetMeshPathFromPrefabPath(this.GetPrefabPathFromCurrentPrefabStage()));
		}

		// Token: 0x06006B17 RID: 27415 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void BakeAndSaveMeshInPath(string meshPath)
		{
		}

		// Token: 0x06006B18 RID: 27416 RVA: 0x0022A61B File Offset: 0x0022881B
		private void UpdateStaticMeshMannequin()
		{
			this.staticGorillaMesh.sharedMesh = this.BakedColliderMesh;
			this.staticGorillaMeshRenderer.sharedMaterials = this.skinnedMeshRenderer.sharedMaterials;
			this.staticGorillaMeshCollider.sharedMesh = this.BakedColliderMesh;
		}

		// Token: 0x06006B19 RID: 27417 RVA: 0x0022A655 File Offset: 0x00228855
		private void UpdateSkinnedMeshCollider()
		{
			this.skinnedMeshCollider.sharedMesh = this.BakedColliderMesh;
		}

		// Token: 0x06006B1A RID: 27418 RVA: 0x0022A668 File Offset: 0x00228868
		public void UpdateGTPosRotConstraints()
		{
			GTPosRotConstraints[] array = this.cosmeticConstraints;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].constraints.ForEach(delegate(GorillaPosRotConstraint c)
				{
					c.follower.rotation = c.source.rotation;
					c.follower.position = c.source.position;
				});
			}
		}

		// Token: 0x06006B1B RID: 27419 RVA: 0x0022A6B8 File Offset: 0x002288B8
		private void HookupCosmeticConstraints()
		{
			this.cosmeticConstraints = base.GetComponentsInChildren<GTPosRotConstraints>();
			foreach (GTPosRotConstraints gtposRotConstraints in this.cosmeticConstraints)
			{
				for (int j = 0; j < gtposRotConstraints.constraints.Length; j++)
				{
					gtposRotConstraints.constraints[j].source = this.FindBone(gtposRotConstraints.constraints[j].follower.name);
				}
			}
		}

		// Token: 0x06006B1C RID: 27420 RVA: 0x0022A72C File Offset: 0x0022892C
		private Transform FindBone(string boneName)
		{
			foreach (Transform transform in this.skinnedMeshRenderer.bones)
			{
				if (transform.name == boneName)
				{
					return transform;
				}
			}
			return null;
		}

		// Token: 0x06006B1D RID: 27421 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void CreasteTestClip()
		{
		}

		// Token: 0x06006B1E RID: 27422 RVA: 0x0022A768 File Offset: 0x00228968
		public void SerializeVRRig()
		{
			base.StartCoroutine(this.SaveLocalPlayerPose());
		}

		// Token: 0x06006B1F RID: 27423 RVA: 0x0022A777 File Offset: 0x00228977
		public IEnumerator SaveLocalPlayerPose()
		{
			yield return null;
			yield break;
		}

		// Token: 0x06006B20 RID: 27424 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void SerializeOutBonesFromSkinnedMesh(SkinnedMeshRenderer paramSkinnedMeshRenderer)
		{
		}

		// Token: 0x06006B21 RID: 27425 RVA: 0x0022A780 File Offset: 0x00228980
		public void SetCurvesForBone(SkinnedMeshRenderer paramSkinnedMeshRenderer, AnimationClip clip, Transform bone)
		{
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.x)
			};
			Keyframe[] keys2 = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.y)
			};
			Keyframe[] keys3 = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.z)
			};
			Keyframe[] keys4 = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.w)
			};
			AnimationCurve curve = new AnimationCurve(keys);
			AnimationCurve curve2 = new AnimationCurve(keys2);
			AnimationCurve curve3 = new AnimationCurve(keys3);
			AnimationCurve curve4 = new AnimationCurve(keys4);
			string relativePath = "";
			string b = bone.name.Replace("_new", "");
			foreach (Transform transform in this.skinnedMeshRenderer.bones)
			{
				if (transform.name == b)
				{
					relativePath = transform.GetPath(this.skinnedMeshRenderer.transform.parent).TrimStart('/');
					break;
				}
			}
			clip.SetCurve(relativePath, typeof(Transform), "m_LocalRotation.x", curve);
			clip.SetCurve(relativePath, typeof(Transform), "m_LocalRotation.y", curve2);
			clip.SetCurve(relativePath, typeof(Transform), "m_LocalRotation.z", curve3);
			clip.SetCurve(relativePath, typeof(Transform), "m_LocalRotation.w", curve4);
		}

		// Token: 0x06006B22 RID: 27426 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void UpdatePrefabWithAnimationClip(string AnimationFileName)
		{
		}

		// Token: 0x06006B23 RID: 27427 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void LoadPoseOntoMannequin(AnimationClip clip, float frameTime = 0f)
		{
		}

		// Token: 0x06006B24 RID: 27428 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnValidate()
		{
		}

		// Token: 0x04007B34 RID: 31540
		public SkinnedMeshRenderer skinnedMeshRenderer;

		// Token: 0x04007B35 RID: 31541
		[FormerlySerializedAs("meshCollider")]
		public MeshCollider skinnedMeshCollider;

		// Token: 0x04007B36 RID: 31542
		public GTPosRotConstraints[] cosmeticConstraints;

		// Token: 0x04007B37 RID: 31543
		public Mesh BakedColliderMesh;

		// Token: 0x04007B38 RID: 31544
		[SerializeField]
		[FormerlySerializedAs("liveAssetPath")]
		protected string prefabAssetPath;

		// Token: 0x04007B39 RID: 31545
		[SerializeField]
		protected string prefabFolderPath;

		// Token: 0x04007B3A RID: 31546
		[SerializeField]
		protected string prefabAssetName;

		// Token: 0x04007B3B RID: 31547
		public MeshFilter staticGorillaMesh;

		// Token: 0x04007B3C RID: 31548
		public MeshCollider staticGorillaMeshCollider;

		// Token: 0x04007B3D RID: 31549
		public MeshRenderer staticGorillaMeshRenderer;
	}
}
