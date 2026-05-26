using System;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001152 RID: 4434
	[ExecuteAlways]
	public class TextureTransitioner : MonoBehaviour, IResettableItem
	{
		// Token: 0x0600705B RID: 28763 RVA: 0x0024A337 File Offset: 0x00248537
		protected void Awake()
		{
			if (Application.isPlaying || this.editorPreview)
			{
				TextureTransitionerManager.EnsureInstanceIsAvailable();
			}
			this.RefreshShaderParams();
			this.iDynamicFloat = (IDynamicFloat)this.dynamicFloatComponent;
			this.ResetToDefaultState();
		}

		// Token: 0x0600705C RID: 28764 RVA: 0x0024A36C File Offset: 0x0024856C
		protected void OnEnable()
		{
			TextureTransitionerManager.Register(this);
			if (Application.isPlaying && !this.remapInfo.IsValid())
			{
				Debug.LogError("Bad min/max values for remapRanges: " + this.GetComponentPath(int.MaxValue), this);
				base.enabled = false;
			}
			if (Application.isPlaying && this.textures.Length == 0)
			{
				Debug.LogError("Textures array is empty: " + this.GetComponentPath(int.MaxValue), this);
				base.enabled = false;
			}
			if (Application.isPlaying && this.iDynamicFloat == null)
			{
				if (this.dynamicFloatComponent == null)
				{
					Debug.LogError("dynamicFloatComponent cannot be null: " + this.GetComponentPath(int.MaxValue), this);
				}
				this.iDynamicFloat = (IDynamicFloat)this.dynamicFloatComponent;
				if (this.iDynamicFloat == null)
				{
					Debug.LogError("Component assigned to dynamicFloatComponent does not implement IDynamicFloat: " + this.GetComponentPath(int.MaxValue), this);
					base.enabled = false;
				}
			}
		}

		// Token: 0x0600705D RID: 28765 RVA: 0x0024A45A File Offset: 0x0024865A
		protected void OnDisable()
		{
			TextureTransitionerManager.Unregister(this);
		}

		// Token: 0x0600705E RID: 28766 RVA: 0x0024A462 File Offset: 0x00248662
		private void RefreshShaderParams()
		{
			this.texTransitionShaderParam = Shader.PropertyToID(this.texTransitionShaderParamName);
			this.tex1ShaderParam = Shader.PropertyToID(this.tex1ShaderParamName);
			this.tex2ShaderParam = Shader.PropertyToID(this.tex2ShaderParamName);
		}

		// Token: 0x0600705F RID: 28767 RVA: 0x0024A497 File Offset: 0x00248697
		public void ResetToDefaultState()
		{
			this.normalizedValue = 0f;
			this.transitionPercent = 0;
			this.tex1Index = 0;
			this.tex2Index = 0;
		}

		// Token: 0x04008034 RID: 32820
		public bool editorPreview;

		// Token: 0x04008035 RID: 32821
		[Tooltip("The component that will drive the texture transitions.")]
		public MonoBehaviour dynamicFloatComponent;

		// Token: 0x04008036 RID: 32822
		[Tooltip("Set these values so that after remap 0 is the first texture in the textures list and 1 is the last.")]
		public GorillaMath.RemapFloatInfo remapInfo;

		// Token: 0x04008037 RID: 32823
		public TextureTransitioner.DirectionRetentionMode directionRetentionMode;

		// Token: 0x04008038 RID: 32824
		public string texTransitionShaderParamName = "_TexTransition";

		// Token: 0x04008039 RID: 32825
		public string tex1ShaderParamName = "_MainTex";

		// Token: 0x0400803A RID: 32826
		public string tex2ShaderParamName = "_Tex2";

		// Token: 0x0400803B RID: 32827
		public Texture[] textures;

		// Token: 0x0400803C RID: 32828
		public Renderer[] renderers;

		// Token: 0x0400803D RID: 32829
		[NonSerialized]
		public IDynamicFloat iDynamicFloat;

		// Token: 0x0400803E RID: 32830
		[NonSerialized]
		public int texTransitionShaderParam;

		// Token: 0x0400803F RID: 32831
		[NonSerialized]
		public int tex1ShaderParam;

		// Token: 0x04008040 RID: 32832
		[NonSerialized]
		public int tex2ShaderParam;

		// Token: 0x04008041 RID: 32833
		[DebugReadout]
		[NonSerialized]
		public float normalizedValue;

		// Token: 0x04008042 RID: 32834
		[DebugReadout]
		[NonSerialized]
		public int transitionPercent;

		// Token: 0x04008043 RID: 32835
		[DebugReadout]
		[NonSerialized]
		public int tex1Index;

		// Token: 0x04008044 RID: 32836
		[DebugReadout]
		[NonSerialized]
		public int tex2Index;

		// Token: 0x02001153 RID: 4435
		public enum DirectionRetentionMode
		{
			// Token: 0x04008046 RID: 32838
			None,
			// Token: 0x04008047 RID: 32839
			IncreaseOnly,
			// Token: 0x04008048 RID: 32840
			DecreaseOnly
		}
	}
}
