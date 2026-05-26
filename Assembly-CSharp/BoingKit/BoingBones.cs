using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001354 RID: 4948
	public class BoingBones : BoingReactor
	{
		// Token: 0x06007CA9 RID: 31913 RVA: 0x0028E9AB File Offset: 0x0028CBAB
		protected override void Register()
		{
			BoingManager.Register(this);
		}

		// Token: 0x06007CAA RID: 31914 RVA: 0x0028E9B3 File Offset: 0x0028CBB3
		protected override void Unregister()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x06007CAB RID: 31915 RVA: 0x0028E9BB File Offset: 0x0028CBBB
		protected override void OnUpgrade(Version oldVersion, Version newVersion)
		{
			base.OnUpgrade(oldVersion, newVersion);
			if (oldVersion.Revision < 33)
			{
				this.TwistPropagation = false;
			}
		}

		// Token: 0x06007CAC RID: 31916 RVA: 0x0028E9D7 File Offset: 0x0028CBD7
		public void OnValidate()
		{
			this.RescanBoneChains();
			this.UpdateCollisionRadius();
		}

		// Token: 0x06007CAD RID: 31917 RVA: 0x0028E9E5 File Offset: 0x0028CBE5
		public override void OnEnable()
		{
			base.OnEnable();
			this.RescanBoneChains();
			this.Reboot();
		}

		// Token: 0x06007CAE RID: 31918 RVA: 0x0028E9F9 File Offset: 0x0028CBF9
		public override void OnDisable()
		{
			base.OnDisable();
			this.Restore();
		}

		// Token: 0x06007CAF RID: 31919 RVA: 0x0028EA08 File Offset: 0x0028CC08
		public void RescanBoneChains()
		{
			if (this.BoneChains == null)
			{
				return;
			}
			int num = this.BoneChains.Length;
			if (this.BoneData == null || this.BoneData.Length != num)
			{
				BoingBones.Bone[][] array = new BoingBones.Bone[num][];
				if (this.BoneData != null)
				{
					int i = 0;
					int num2 = Mathf.Min(this.BoneData.Length, num);
					while (i < num2)
					{
						array[i] = this.BoneData[i];
						i++;
					}
				}
				this.BoneData = array;
			}
			Queue<BoingBones.RescanEntry> queue = new Queue<BoingBones.RescanEntry>();
			for (int j = 0; j < num; j++)
			{
				BoingBones.Chain chain = this.BoneChains[j];
				bool flag = false;
				if (this.BoneData[j] == null)
				{
					flag = true;
				}
				if (!flag && chain.m_scannedRoot == null)
				{
					flag = true;
				}
				if (!flag && chain.m_scannedRoot != chain.Root)
				{
					flag = true;
				}
				if (!flag && chain.m_scannedExclusion != null != (chain.Exclusion != null))
				{
					flag = true;
				}
				if (!flag && chain.Exclusion != null)
				{
					if (chain.m_scannedExclusion.Length != chain.Exclusion.Length)
					{
						flag = true;
					}
					else
					{
						for (int k = 0; k < chain.m_scannedExclusion.Length; k++)
						{
							if (!(chain.m_scannedExclusion[k] == chain.Exclusion[k]))
							{
								flag = true;
								break;
							}
						}
					}
				}
				Transform transform = (chain != null) ? chain.Root : null;
				int num3 = (transform != null) ? Codec.HashTransformHierarchy(transform) : -1;
				if (!flag && transform != null && chain.m_hierarchyHash != num3)
				{
					flag = true;
				}
				if (flag)
				{
					if (transform == null)
					{
						this.BoneData[j] = null;
					}
					else
					{
						chain.m_scannedRoot = chain.Root;
						chain.m_scannedExclusion = chain.Exclusion.ToArray<Transform>();
						chain.m_hierarchyHash = num3;
						chain.MaxLengthFromRoot = 0f;
						List<BoingBones.Bone> list = new List<BoingBones.Bone>();
						queue.Enqueue(new BoingBones.RescanEntry(transform, -1, 0f));
						while (queue.Count > 0)
						{
							BoingBones.RescanEntry rescanEntry = queue.Dequeue();
							if (!chain.Exclusion.Contains(rescanEntry.Transform))
							{
								int count = list.Count;
								Transform transform2 = rescanEntry.Transform;
								int[] array2 = new int[transform2.childCount];
								for (int l = 0; l < array2.Length; l++)
								{
									array2[l] = -1;
								}
								int num4 = 0;
								int m = 0;
								int childCount = transform2.childCount;
								while (m < childCount)
								{
									Transform child = transform2.GetChild(m);
									if (!chain.Exclusion.Contains(child))
									{
										float num5 = Vector3.Distance(rescanEntry.Transform.position, child.position);
										float lengthFromRoot = rescanEntry.LengthFromRoot + num5;
										queue.Enqueue(new BoingBones.RescanEntry(child, count, lengthFromRoot));
										num4++;
									}
									m++;
								}
								chain.MaxLengthFromRoot = Mathf.Max(rescanEntry.LengthFromRoot, chain.MaxLengthFromRoot);
								BoingBones.Bone bone = new BoingBones.Bone(transform2, rescanEntry.ParentIndex, rescanEntry.LengthFromRoot);
								if (num4 > 0)
								{
									bone.ChildIndices = array2;
								}
								list.Add(bone);
							}
						}
						for (int n = 0; n < list.Count; n++)
						{
							BoingBones.Bone bone2 = list[n];
							if (bone2.ParentIndex >= 0)
							{
								BoingBones.Bone bone3 = list[bone2.ParentIndex];
								int num6 = 0;
								while (bone3.ChildIndices[num6] >= 0)
								{
									num6++;
								}
								if (num6 < bone3.ChildIndices.Length)
								{
									bone3.ChildIndices[num6] = n;
								}
							}
						}
						if (list.Count != 0)
						{
							float num7 = MathUtil.InvSafe(chain.MaxLengthFromRoot);
							for (int num8 = 0; num8 < list.Count; num8++)
							{
								BoingBones.Bone bone4 = list[num8];
								float t = Mathf.Clamp01(bone4.LengthFromRoot * num7);
								bone4.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, t, chain.CollisionRadiusCustomCurve);
							}
							this.BoneData[j] = list.ToArray();
							this.Reboot(j);
						}
					}
				}
			}
		}

		// Token: 0x06007CB0 RID: 31920 RVA: 0x0028EE24 File Offset: 0x0028D024
		private void UpdateCollisionRadius()
		{
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null)
				{
					float num = MathUtil.InvSafe(chain.MaxLengthFromRoot);
					foreach (BoingBones.Bone bone in array)
					{
						float t = Mathf.Clamp01(bone.LengthFromRoot * num);
						bone.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, t, chain.CollisionRadiusCustomCurve);
					}
				}
			}
		}

		// Token: 0x06007CB1 RID: 31921 RVA: 0x0028EEAC File Offset: 0x0028D0AC
		public override void Reboot()
		{
			base.Reboot();
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				this.Reboot(i);
			}
		}

		// Token: 0x06007CB2 RID: 31922 RVA: 0x0028EEDC File Offset: 0x0028D0DC
		public void Reboot(int iChain)
		{
			BoingBones.Bone[] array = this.BoneData[iChain];
			if (array == null)
			{
				return;
			}
			foreach (BoingBones.Bone bone in array)
			{
				bone.Instance.PositionSpring.Reset(bone.Position);
				bone.Instance.RotationSpring.Reset(bone.Rotation);
				bone.CachedPositionWs = bone.Position;
				bone.CachedPositionLs = bone.Transform.localPosition;
				bone.CachedRotationWs = bone.Rotation;
				bone.CachedRotationLs = bone.Transform.localRotation;
				bone.CachedScaleLs = bone.LocalScale;
			}
			this.CachedTransformValid = true;
		}

		// Token: 0x17000BD0 RID: 3024
		// (get) Token: 0x06007CB3 RID: 31923 RVA: 0x0028EF81 File Offset: 0x0028D181
		internal float MinScale
		{
			get
			{
				return this.m_minScale;
			}
		}

		// Token: 0x06007CB4 RID: 31924 RVA: 0x0028EF8C File Offset: 0x0028D18C
		public override void PrepareExecute()
		{
			base.PrepareExecute();
			this.Params.Bits.SetBit(4, false);
			float fixedDeltaTime = Time.fixedDeltaTime;
			float d = (this.UpdateMode == BoingManager.UpdateMode.FixedUpdate) ? fixedDeltaTime : Time.deltaTime;
			this.m_minScale = Mathf.Min(base.transform.localScale.x, Mathf.Min(base.transform.localScale.y, base.transform.localScale.z));
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null && !(chain.Root == null) && array.Length != 0)
				{
					Vector3 b = chain.Gravity * d;
					float num = 0f;
					foreach (BoingBones.Bone bone in array)
					{
						if (bone.ParentIndex < 0)
						{
							if (!chain.LooseRoot)
							{
								bone.Instance.PositionSpring.Reset(bone.Position);
								bone.Instance.RotationSpring.Reset(bone.Rotation);
							}
							bone.LengthFromRoot = 0f;
						}
						else
						{
							BoingBones.Bone bone2 = array[bone.ParentIndex];
							float num2 = Vector3.Distance(bone.Position, bone2.Position);
							bone.LengthFromRoot = bone2.LengthFromRoot + num2;
							num = Mathf.Max(num, bone.LengthFromRoot);
						}
					}
					float num3 = MathUtil.InvSafe(num);
					foreach (BoingBones.Bone bone3 in array)
					{
						float t = bone3.LengthFromRoot * num3;
						bone3.AnimationBlend = BoingBones.Chain.EvaluateCurve(chain.AnimationBlendCurveType, t, chain.AnimationBlendCustomCurve);
						bone3.LengthStiffness = BoingBones.Chain.EvaluateCurve(chain.LengthStiffnessCurveType, t, chain.LengthStiffnessCustomCurve);
						bone3.LengthStiffnessT = 1f - Mathf.Pow(1f - bone3.LengthStiffness, 30f * fixedDeltaTime);
						bone3.FullyStiffToParentLength = ((bone3.ParentIndex >= 0) ? Vector3.Distance(array[bone3.ParentIndex].Position, bone3.Position) : 0f);
						bone3.PoseStiffness = BoingBones.Chain.EvaluateCurve(chain.PoseStiffnessCurveType, t, chain.PoseStiffnessCustomCurve);
						bone3.BendAngleCap = chain.MaxBendAngleCap * MathUtil.Deg2Rad * BoingBones.Chain.EvaluateCurve(chain.BendAngleCapCurveType, t, chain.BendAngleCapCustomCurve);
						bone3.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, t, chain.CollisionRadiusCustomCurve);
						bone3.SquashAndStretch = BoingBones.Chain.EvaluateCurve(chain.SquashAndStretchCurveType, t, chain.SquashAndStretchCustomCurve);
					}
					Vector3 position = array[0].Position;
					for (int l = 0; l < array.Length; l++)
					{
						BoingBones.Bone bone4 = array[l];
						float t2 = bone4.LengthFromRoot * num3;
						bone4.AnimationBlend = BoingBones.Chain.EvaluateCurve(chain.AnimationBlendCurveType, t2, chain.AnimationBlendCustomCurve);
						bone4.LengthStiffness = BoingBones.Chain.EvaluateCurve(chain.LengthStiffnessCurveType, t2, chain.LengthStiffnessCustomCurve);
						bone4.PoseStiffness = BoingBones.Chain.EvaluateCurve(chain.PoseStiffnessCurveType, t2, chain.PoseStiffnessCustomCurve);
						bone4.BendAngleCap = chain.MaxBendAngleCap * MathUtil.Deg2Rad * BoingBones.Chain.EvaluateCurve(chain.BendAngleCapCurveType, t2, chain.BendAngleCapCustomCurve);
						bone4.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, t2, chain.CollisionRadiusCustomCurve);
						bone4.SquashAndStretch = BoingBones.Chain.EvaluateCurve(chain.SquashAndStretchCurveType, t2, chain.SquashAndStretchCustomCurve);
						if (l > 0)
						{
							BoingBones.Bone bone5 = bone4;
							bone5.Instance.PositionSpring.Velocity = bone5.Instance.PositionSpring.Velocity + b;
						}
						bone4.RotationInverseWs = Quaternion.Inverse(bone4.Rotation);
						bone4.SpringRotationWs = bone4.Instance.RotationSpring.ValueQuat;
						bone4.SpringRotationInverseWs = Quaternion.Inverse(bone4.SpringRotationWs);
						Vector3 vector = bone4.Position;
						Quaternion rotation = bone4.Rotation;
						Vector3 localScale = bone4.LocalScale;
						if (bone4.ParentIndex >= 0)
						{
							BoingBones.Bone bone6 = array[bone4.ParentIndex];
							Vector3 position2 = bone6.Position;
							Vector3 value = bone6.Instance.PositionSpring.Value;
							Vector3 a = bone6.SpringRotationInverseWs * (bone4.Instance.PositionSpring.Value - value);
							Quaternion a2 = bone6.SpringRotationInverseWs * bone4.Instance.RotationSpring.ValueQuat;
							Vector3 position3 = bone4.Position;
							Quaternion rotation2 = bone4.Rotation;
							Vector3 b2 = bone6.RotationInverseWs * (position3 - position2);
							Quaternion b3 = bone6.RotationInverseWs * rotation2;
							float poseStiffness = bone4.PoseStiffness;
							Vector3 point = Vector3.Lerp(a, b2, poseStiffness);
							Quaternion rhs = Quaternion.Slerp(a2, b3, poseStiffness);
							vector = value + bone6.SpringRotationWs * point;
							rotation = bone6.SpringRotationWs * rhs;
							if (bone4.BendAngleCap < MathUtil.Pi - MathUtil.Epsilon)
							{
								Vector3 vector2 = vector - position;
								vector2 = VectorUtil.ClampBend(vector2, position3 - position, bone4.BendAngleCap);
								vector = position + vector2;
							}
						}
						if (chain.ParamsOverride == null)
						{
							bone4.Instance.PrepareExecute(ref this.Params, vector, rotation, localScale, true);
						}
						else
						{
							bone4.Instance.PrepareExecute(ref chain.ParamsOverride.Params, vector, rotation, localScale, true);
						}
					}
				}
			}
		}

		// Token: 0x06007CB5 RID: 31925 RVA: 0x0028F544 File Offset: 0x0028D744
		public void AccumulateTarget(ref BoingEffector.Params effector, float dt)
		{
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null && chain.EffectorReaction)
				{
					foreach (BoingBones.Bone bone in array)
					{
						if (chain.ParamsOverride == null)
						{
							bone.Instance.AccumulateTarget(ref this.Params, ref effector, dt);
						}
						else
						{
							Bits32 bits = chain.ParamsOverride.Params.Bits;
							chain.ParamsOverride.Params.Bits = this.Params.Bits;
							bone.Instance.AccumulateTarget(ref chain.ParamsOverride.Params, ref effector, dt);
							chain.ParamsOverride.Params.Bits = bits;
						}
					}
				}
			}
		}

		// Token: 0x06007CB6 RID: 31926 RVA: 0x0028F62C File Offset: 0x0028D82C
		public void EndAccumulateTargets()
		{
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null)
				{
					foreach (BoingBones.Bone bone in array)
					{
						if (chain.ParamsOverride == null)
						{
							bone.Instance.EndAccumulateTargets(ref this.Params);
						}
						else
						{
							bone.Instance.EndAccumulateTargets(ref chain.ParamsOverride.Params);
						}
					}
				}
			}
		}

		// Token: 0x06007CB7 RID: 31927 RVA: 0x0028F6B0 File Offset: 0x0028D8B0
		public override void Restore()
		{
			if (!this.CachedTransformValid)
			{
				return;
			}
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null)
				{
					for (int j = 0; j < array.Length; j++)
					{
						BoingBones.Bone bone = array[j];
						if (j != 0 || chain.LooseRoot)
						{
							bone.Transform.SetLocalPositionAndRotation(bone.CachedPositionLs, bone.CachedRotationLs);
							bone.Transform.localScale = bone.CachedScaleLs;
						}
					}
				}
			}
		}

		// Token: 0x04008DD9 RID: 36313
		[SerializeField]
		internal BoingBones.Bone[][] BoneData;

		// Token: 0x04008DDA RID: 36314
		public BoingBones.Chain[] BoneChains = new BoingBones.Chain[1];

		// Token: 0x04008DDB RID: 36315
		public bool TwistPropagation = true;

		// Token: 0x04008DDC RID: 36316
		[Range(0.1f, 20f)]
		public float MaxCollisionResolutionSpeed = 3f;

		// Token: 0x04008DDD RID: 36317
		public BoingBoneCollider[] BoingColliders = new BoingBoneCollider[0];

		// Token: 0x04008DDE RID: 36318
		public Collider[] UnityColliders = new Collider[0];

		// Token: 0x04008DDF RID: 36319
		public bool DebugDrawRawBones;

		// Token: 0x04008DE0 RID: 36320
		public bool DebugDrawTargetBones;

		// Token: 0x04008DE1 RID: 36321
		public bool DebugDrawBoingBones;

		// Token: 0x04008DE2 RID: 36322
		public bool DebugDrawFinalBones;

		// Token: 0x04008DE3 RID: 36323
		public bool DebugDrawColliders;

		// Token: 0x04008DE4 RID: 36324
		public bool DebugDrawChainBounds;

		// Token: 0x04008DE5 RID: 36325
		public bool DebugDrawBoneNames;

		// Token: 0x04008DE6 RID: 36326
		public bool DebugDrawLengthFromRoot;

		// Token: 0x04008DE7 RID: 36327
		private float m_minScale = 1f;

		// Token: 0x02001355 RID: 4949
		[Serializable]
		public class Bone
		{
			// Token: 0x17000BD1 RID: 3025
			// (get) Token: 0x06007CB9 RID: 31929 RVA: 0x0028F790 File Offset: 0x0028D990
			internal Vector3 Position
			{
				get
				{
					this.CheckResetFlags();
					if (this.updatedPos)
					{
						this.updatedPos = false;
						this.position = this.Transform.position;
					}
					return this.position;
				}
			}

			// Token: 0x17000BD2 RID: 3026
			// (get) Token: 0x06007CBA RID: 31930 RVA: 0x0028F7BE File Offset: 0x0028D9BE
			internal Quaternion Rotation
			{
				get
				{
					this.CheckResetFlags();
					if (this.updatedRot)
					{
						this.updatedRot = false;
						this.rotation = this.Transform.rotation;
					}
					return this.rotation;
				}
			}

			// Token: 0x17000BD3 RID: 3027
			// (get) Token: 0x06007CBB RID: 31931 RVA: 0x0028F7EC File Offset: 0x0028D9EC
			internal Vector3 LocalScale
			{
				get
				{
					this.CheckResetFlags();
					if (this.updatedScale)
					{
						this.updatedScale = false;
						this.localScale = this.Transform.localScale;
					}
					return this.localScale;
				}
			}

			// Token: 0x06007CBC RID: 31932 RVA: 0x0028F81C File Offset: 0x0028DA1C
			private void CheckResetFlags()
			{
				if (this.Transform.hasChanged)
				{
					this.updatedPos = (this.updatedRot = (this.updatedScale = true));
					this.Transform.hasChanged = false;
				}
			}

			// Token: 0x06007CBD RID: 31933 RVA: 0x0028F85B File Offset: 0x0028DA5B
			internal void UpdateBounds()
			{
				this.Bounds = new Bounds(this.Instance.PositionSpring.Value, 2f * this.CollisionRadius * Vector3.one);
			}

			// Token: 0x06007CBE RID: 31934 RVA: 0x0028F890 File Offset: 0x0028DA90
			internal Bone(Transform transform, int iParent, float lengthFromRoot)
			{
				this.Transform = transform;
				this.RotationInverseWs = Quaternion.identity;
				this.ParentIndex = iParent;
				this.LengthFromRoot = lengthFromRoot;
				this.Instance.Reset();
				this.CachedPositionWs = transform.position;
				this.CachedPositionLs = transform.localPosition;
				this.CachedRotationWs = transform.rotation;
				this.CachedRotationLs = transform.localRotation;
				this.CachedScaleLs = transform.localScale;
				this.AnimationBlend = 0f;
				this.LengthStiffness = 0f;
				this.PoseStiffness = 0f;
				this.BendAngleCap = 180f;
				this.CollisionRadius = 0f;
			}

			// Token: 0x04008DE8 RID: 36328
			internal BoingWork.Params.InstanceData Instance;

			// Token: 0x04008DE9 RID: 36329
			internal Transform Transform;

			// Token: 0x04008DEA RID: 36330
			internal Vector3 ScaleWs;

			// Token: 0x04008DEB RID: 36331
			internal Vector3 CachedScaleLs;

			// Token: 0x04008DEC RID: 36332
			internal Vector3 BlendedPositionWs;

			// Token: 0x04008DED RID: 36333
			internal Vector3 BlendedScaleLs;

			// Token: 0x04008DEE RID: 36334
			internal Vector3 CachedPositionWs;

			// Token: 0x04008DEF RID: 36335
			internal Vector3 CachedPositionLs;

			// Token: 0x04008DF0 RID: 36336
			internal Bounds Bounds;

			// Token: 0x04008DF1 RID: 36337
			internal Quaternion RotationInverseWs;

			// Token: 0x04008DF2 RID: 36338
			internal Quaternion SpringRotationWs;

			// Token: 0x04008DF3 RID: 36339
			internal Quaternion SpringRotationInverseWs;

			// Token: 0x04008DF4 RID: 36340
			internal Quaternion CachedRotationWs;

			// Token: 0x04008DF5 RID: 36341
			internal Quaternion CachedRotationLs;

			// Token: 0x04008DF6 RID: 36342
			internal Quaternion BlendedRotationWs;

			// Token: 0x04008DF7 RID: 36343
			internal Quaternion RotationBackPropDeltaPs;

			// Token: 0x04008DF8 RID: 36344
			internal int ParentIndex;

			// Token: 0x04008DF9 RID: 36345
			internal int[] ChildIndices;

			// Token: 0x04008DFA RID: 36346
			internal float LengthFromRoot;

			// Token: 0x04008DFB RID: 36347
			internal float AnimationBlend;

			// Token: 0x04008DFC RID: 36348
			internal float LengthStiffness;

			// Token: 0x04008DFD RID: 36349
			internal float LengthStiffnessT;

			// Token: 0x04008DFE RID: 36350
			internal float FullyStiffToParentLength;

			// Token: 0x04008DFF RID: 36351
			internal float PoseStiffness;

			// Token: 0x04008E00 RID: 36352
			internal float BendAngleCap;

			// Token: 0x04008E01 RID: 36353
			internal float CollisionRadius;

			// Token: 0x04008E02 RID: 36354
			internal float SquashAndStretch;

			// Token: 0x04008E03 RID: 36355
			private bool updatedPos;

			// Token: 0x04008E04 RID: 36356
			private bool updatedRot;

			// Token: 0x04008E05 RID: 36357
			private bool updatedScale;

			// Token: 0x04008E06 RID: 36358
			private Vector3 position;

			// Token: 0x04008E07 RID: 36359
			private Quaternion rotation;

			// Token: 0x04008E08 RID: 36360
			private Vector3 localScale;
		}

		// Token: 0x02001356 RID: 4950
		[Serializable]
		public class Chain
		{
			// Token: 0x06007CBF RID: 31935 RVA: 0x0028F944 File Offset: 0x0028DB44
			public static float EvaluateCurve(BoingBones.Chain.CurveType type, float t, AnimationCurve curve)
			{
				switch (type)
				{
				case BoingBones.Chain.CurveType.ConstantOne:
					return 1f;
				case BoingBones.Chain.CurveType.ConstantHalf:
					return 0.5f;
				case BoingBones.Chain.CurveType.ConstantZero:
					return 0f;
				case BoingBones.Chain.CurveType.RootOneTailHalf:
					return 1f - 0.5f * Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.RootOneTailZero:
					return 1f - Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.RootHalfTailOne:
					return 0.5f + 0.5f * Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.RootZeroTailOne:
					return Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.Custom:
					return curve.Evaluate(t);
				default:
					return 0f;
				}
			}

			// Token: 0x04008E09 RID: 36361
			[Tooltip("Root Transform object from which to build a chain (or tree if a bone has multiple children) of bouncy boing bones.")]
			public Transform Root;

			// Token: 0x04008E0A RID: 36362
			[Tooltip("List of Transform objects to exclude from chain building.")]
			public Transform[] Exclusion;

			// Token: 0x04008E0B RID: 36363
			[Tooltip("Enable to allow reaction to boing effectors.")]
			public bool EffectorReaction = true;

			// Token: 0x04008E0C RID: 36364
			[Tooltip("Enable to allow root Transform object to be sprung around as well. Otherwise, no effects will be applied to the root Transform object.")]
			public bool LooseRoot;

			// Token: 0x04008E0D RID: 36365
			[Tooltip("Assign a SharedParamsOverride asset to override the parameters for this chain. Useful for chains using different parameters than that of the BoingBones component.")]
			public SharedBoingParams ParamsOverride;

			// Token: 0x04008E0E RID: 36366
			[ConditionalField(null, null, null, null, null, null, null, Label = "Animation Blend", Tooltip = "Animation blend determines each bone's final transform between the original raw transform and its corresponding boing bone. 1.0 means 100% contribution from raw (or animated) transform. 0.0 means 100% contribution from boing bone.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's animation blend:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType AnimationBlendCurveType = BoingBones.Chain.CurveType.RootOneTailZero;

			// Token: 0x04008E0F RID: 36367
			[ConditionalField("AnimationBlendCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve AnimationBlendCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

			// Token: 0x04008E10 RID: 36368
			[ConditionalField(null, null, null, null, null, null, null, Label = "Length Stiffness", Tooltip = "Length stiffness determines how much each target bone (target transform each boing bone is sprung towards) tries to maintain original distance from its parent. 1.0 means 100% distance maintenance. 0.0 means 0% distance maintenance.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's length stiffness:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType LengthStiffnessCurveType;

			// Token: 0x04008E11 RID: 36369
			[ConditionalField("LengthStiffnessCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve LengthStiffnessCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			// Token: 0x04008E12 RID: 36370
			[ConditionalField(null, null, null, null, null, null, null, Label = "Pose Stiffness", Tooltip = "Pose stiffness determines how much each target bone (target transform each boing bone is sprung towards) tries to maintain original transform. 1.0 means 100% original transform maintenance. 0.0 means 0% original transform maintenance.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's pose stiffness:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType PoseStiffnessCurveType;

			// Token: 0x04008E13 RID: 36371
			[ConditionalField("PoseStiffnessCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve PoseStiffnessCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			// Token: 0x04008E14 RID: 36372
			[ConditionalField(null, null, null, null, null, null, null, Label = "Bend Angle Cap", Tooltip = "Maximum bone bend angle cap.", Min = 0f, Max = 180f)]
			public float MaxBendAngleCap = 180f;

			// Token: 0x04008E15 RID: 36373
			[ConditionalField(null, null, null, null, null, null, null, Label = "  Curve Type", Tooltip = "Percentage(0.0 = 0 %; 1.0 = 100 %) of maximum bone bend angle cap.Bend angle cap limits how much each bone can bend relative to the root (in degrees). 1.0 means 100% maximum bend angle cap. 0.0 means 0% maximum bend angle cap.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's pose stiffness:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType BendAngleCapCurveType;

			// Token: 0x04008E16 RID: 36374
			[ConditionalField("BendAngleCapCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "    Custom Curve")]
			public AnimationCurve BendAngleCapCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			// Token: 0x04008E17 RID: 36375
			[ConditionalField(null, null, null, null, null, null, null, Label = "Collision Radius", Tooltip = "Maximum bone collision radius.")]
			public float MaxCollisionRadius = 0.1f;

			// Token: 0x04008E18 RID: 36376
			[ConditionalField(null, null, null, null, null, null, null, Label = "  Curve Type", Tooltip = "Percentage (0.0 = 0%; 1.0 = 100%) of maximum bone collision radius.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's collision radius:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType CollisionRadiusCurveType;

			// Token: 0x04008E19 RID: 36377
			[ConditionalField("CollisionRadiusCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "    Custom Curve")]
			public AnimationCurve CollisionRadiusCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			// Token: 0x04008E1A RID: 36378
			[ConditionalField(null, null, null, null, null, null, null, Label = "Boing Kit Collision", Tooltip = "Enable to allow this chain to collide with Boing Kit's own implementation of lightweight colliders")]
			public bool EnableBoingKitCollision;

			// Token: 0x04008E1B RID: 36379
			[ConditionalField(null, null, null, null, null, null, null, Label = "Unity Collision", Tooltip = "Enable to allow this chain to collide with Unity colliders.")]
			public bool EnableUnityCollision;

			// Token: 0x04008E1C RID: 36380
			[ConditionalField(null, null, null, null, null, null, null, Label = "Inter-Chain Collision", Tooltip = "Enable to allow this chain to collide with other chain (under the same BoingBones component) with inter-chain collision enabled.")]
			public bool EnableInterChainCollision;

			// Token: 0x04008E1D RID: 36381
			public Vector3 Gravity = Vector3.zero;

			// Token: 0x04008E1E RID: 36382
			internal Bounds Bounds;

			// Token: 0x04008E1F RID: 36383
			[ConditionalField(null, null, null, null, null, null, null, Label = "Squash & Stretch", Tooltip = "Percentage (0.0 = 0%; 1.0 = 100%) of each bone's squash & stretch effect. Squash & stretch is the effect of volume preservation by scaling bones based on how compressed or stretched the distances between bones become.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's squash & stretch effect amount:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType SquashAndStretchCurveType = BoingBones.Chain.CurveType.ConstantZero;

			// Token: 0x04008E20 RID: 36384
			[ConditionalField("SquashAndStretchCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve SquashAndStretchCustomCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

			// Token: 0x04008E21 RID: 36385
			[ConditionalField(null, null, null, null, null, null, null, Label = "  Max Squash", Tooltip = "Maximum squash amount. For example, 2.0 means a maximum scale of 200% when squashed.", Min = 1f, Max = 5f)]
			public float MaxSquash = 1.1f;

			// Token: 0x04008E22 RID: 36386
			[ConditionalField(null, null, null, null, null, null, null, Label = "  Max Stretch", Tooltip = "Maximum stretch amount. For example, 2.0 means a minimum scale of 50% when stretched (200% stretched).", Min = 1f, Max = 5f)]
			public float MaxStretch = 2f;

			// Token: 0x04008E23 RID: 36387
			internal Transform m_scannedRoot;

			// Token: 0x04008E24 RID: 36388
			internal Transform[] m_scannedExclusion;

			// Token: 0x04008E25 RID: 36389
			internal int m_hierarchyHash = -1;

			// Token: 0x04008E26 RID: 36390
			internal float MaxLengthFromRoot;

			// Token: 0x02001357 RID: 4951
			public enum CurveType
			{
				// Token: 0x04008E28 RID: 36392
				ConstantOne,
				// Token: 0x04008E29 RID: 36393
				ConstantHalf,
				// Token: 0x04008E2A RID: 36394
				ConstantZero,
				// Token: 0x04008E2B RID: 36395
				RootOneTailHalf,
				// Token: 0x04008E2C RID: 36396
				RootOneTailZero,
				// Token: 0x04008E2D RID: 36397
				RootHalfTailOne,
				// Token: 0x04008E2E RID: 36398
				RootZeroTailOne,
				// Token: 0x04008E2F RID: 36399
				Custom
			}
		}

		// Token: 0x02001358 RID: 4952
		private class RescanEntry
		{
			// Token: 0x06007CC1 RID: 31937 RVA: 0x0028FAF4 File Offset: 0x0028DCF4
			internal RescanEntry(Transform transform, int iParent, float lengthFromRoot)
			{
				this.Transform = transform;
				this.ParentIndex = iParent;
				this.LengthFromRoot = lengthFromRoot;
			}

			// Token: 0x04008E30 RID: 36400
			internal Transform Transform;

			// Token: 0x04008E31 RID: 36401
			internal int ParentIndex;

			// Token: 0x04008E32 RID: 36402
			internal float LengthFromRoot;
		}
	}
}
