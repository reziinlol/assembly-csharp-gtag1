using System;
using UnityEngine;

// Token: 0x02000683 RID: 1667
public class FingerFlexReactor : MonoBehaviour
{
	// Token: 0x06002987 RID: 10631 RVA: 0x000E0004 File Offset: 0x000DE204
	private void Setup()
	{
		this._rig = base.GetComponentInParent<VRRig>();
		if (!this._rig)
		{
			return;
		}
		this._fingers = new VRMap[]
		{
			this._rig.leftThumb,
			this._rig.leftIndex,
			this._rig.leftMiddle,
			this._rig.rightThumb,
			this._rig.rightIndex,
			this._rig.rightMiddle
		};
	}

	// Token: 0x06002988 RID: 10632 RVA: 0x000E008B File Offset: 0x000DE28B
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x06002989 RID: 10633 RVA: 0x000E0093 File Offset: 0x000DE293
	private void FixedUpdate()
	{
		this.UpdateBlendShapes();
	}

	// Token: 0x0600298A RID: 10634 RVA: 0x000E009C File Offset: 0x000DE29C
	public void UpdateBlendShapes()
	{
		if (!this._rig)
		{
			return;
		}
		if (this._blendShapeTargets == null || this._fingers == null)
		{
			return;
		}
		if (this._blendShapeTargets.Length == 0 || this._fingers.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this._blendShapeTargets.Length; i++)
		{
			FingerFlexReactor.BlendShapeTarget blendShapeTarget = this._blendShapeTargets[i];
			if (blendShapeTarget != null)
			{
				int sourceFinger = (int)blendShapeTarget.sourceFinger;
				if (sourceFinger != -1)
				{
					SkinnedMeshRenderer targetRenderer = blendShapeTarget.targetRenderer;
					if (targetRenderer)
					{
						float lerpValue = FingerFlexReactor.GetLerpValue(this._fingers[sourceFinger]);
						Vector2 inputRange = blendShapeTarget.inputRange;
						Vector2 outputRange = blendShapeTarget.outputRange;
						float num = MathUtils.Linear(lerpValue, inputRange.x, inputRange.y, outputRange.x, outputRange.y);
						blendShapeTarget.currentValue = num;
						targetRenderer.SetBlendShapeWeight(blendShapeTarget.blendShapeIndex, num);
					}
				}
			}
		}
	}

	// Token: 0x0600298B RID: 10635 RVA: 0x000E0170 File Offset: 0x000DE370
	private static float GetLerpValue(VRMap map)
	{
		VRMapThumb vrmapThumb = map as VRMapThumb;
		float result;
		if (vrmapThumb == null)
		{
			VRMapIndex vrmapIndex = map as VRMapIndex;
			if (vrmapIndex == null)
			{
				VRMapMiddle vrmapMiddle = map as VRMapMiddle;
				if (vrmapMiddle == null)
				{
					result = 0f;
				}
				else
				{
					result = vrmapMiddle.calcT;
				}
			}
			else
			{
				result = vrmapIndex.calcT;
			}
		}
		else
		{
			result = ((vrmapThumb.calcT > 0.1f) ? 1f : 0f);
		}
		return result;
	}

	// Token: 0x04003613 RID: 13843
	[SerializeField]
	private VRRig _rig;

	// Token: 0x04003614 RID: 13844
	[SerializeField]
	private VRMap[] _fingers = new VRMap[0];

	// Token: 0x04003615 RID: 13845
	[SerializeField]
	private FingerFlexReactor.BlendShapeTarget[] _blendShapeTargets = new FingerFlexReactor.BlendShapeTarget[0];

	// Token: 0x02000684 RID: 1668
	[Serializable]
	public class BlendShapeTarget
	{
		// Token: 0x04003616 RID: 13846
		public FingerFlexReactor.FingerMap sourceFinger;

		// Token: 0x04003617 RID: 13847
		public SkinnedMeshRenderer targetRenderer;

		// Token: 0x04003618 RID: 13848
		public int blendShapeIndex;

		// Token: 0x04003619 RID: 13849
		public Vector2 inputRange = new Vector2(0f, 1f);

		// Token: 0x0400361A RID: 13850
		public Vector2 outputRange = new Vector2(0f, 1f);

		// Token: 0x0400361B RID: 13851
		[NonSerialized]
		public float currentValue;
	}

	// Token: 0x02000685 RID: 1669
	public enum FingerMap
	{
		// Token: 0x0400361D RID: 13853
		None = -1,
		// Token: 0x0400361E RID: 13854
		LeftThumb,
		// Token: 0x0400361F RID: 13855
		LeftIndex,
		// Token: 0x04003620 RID: 13856
		LeftMiddle,
		// Token: 0x04003621 RID: 13857
		RightThumb,
		// Token: 0x04003622 RID: 13858
		RightIndex,
		// Token: 0x04003623 RID: 13859
		RightMiddle
	}
}
