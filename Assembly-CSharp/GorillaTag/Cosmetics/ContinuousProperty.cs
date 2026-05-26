using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200124E RID: 4686
	[Serializable]
	public class ContinuousProperty
	{
		// Token: 0x0600756F RID: 30063 RVA: 0x002677F4 File Offset: 0x002659F4
		private static ContinuousProperty.Cast GetTargetCast(Object o)
		{
			ContinuousProperty.Cast result;
			if (!(o is ParticleSystem))
			{
				if (!(o is SkinnedMeshRenderer))
				{
					if (!(o is Animator))
					{
						if (!(o is AudioSource))
						{
							if (!(o is VoiceShiftCosmetic))
							{
								if (!(o is Rigidbody))
								{
									if (!(o is Transform))
									{
										if (!(o is Renderer))
										{
											if (!(o is Behaviour))
											{
												if (!(o is GameObject))
												{
													result = ContinuousProperty.Cast.Null;
												}
												else
												{
													result = ContinuousProperty.Cast.GameObject;
												}
											}
											else
											{
												result = ContinuousProperty.Cast.Behaviour;
											}
										}
										else
										{
											result = ContinuousProperty.Cast.Renderer;
										}
									}
									else
									{
										result = ContinuousProperty.Cast.Transform;
									}
								}
								else
								{
									result = ContinuousProperty.Cast.Rigidbody;
								}
							}
							else
							{
								result = ContinuousProperty.Cast.VoicePitchShiftCosmetic;
							}
						}
						else
						{
							result = ContinuousProperty.Cast.AudioSource;
						}
					}
					else
					{
						result = ContinuousProperty.Cast.Animator;
					}
				}
				else
				{
					result = ContinuousProperty.Cast.SkinnedMeshRenderer;
				}
			}
			else
			{
				result = ContinuousProperty.Cast.ParticleSystem;
			}
			return result;
		}

		// Token: 0x06007570 RID: 30064 RVA: 0x002678A8 File Offset: 0x00265AA8
		public static bool CastMatches(ContinuousProperty.Cast cast, ContinuousProperty.Cast test)
		{
			if (cast <= ContinuousProperty.Cast.Any)
			{
				if (cast == ContinuousProperty.Cast.Null)
				{
					return false;
				}
				if (cast == ContinuousProperty.Cast.Any)
				{
					return true;
				}
			}
			else
			{
				if (cast == ContinuousProperty.Cast.Renderer)
				{
					return test == ContinuousProperty.Cast.Renderer || test == ContinuousProperty.Cast.SkinnedMeshRenderer;
				}
				if (cast == ContinuousProperty.Cast.Behaviour)
				{
					return test != ContinuousProperty.Cast.Transform && test != ContinuousProperty.Cast.GameObject && test != ContinuousProperty.Cast.Rigidbody;
				}
			}
			return test == cast;
		}

		// Token: 0x06007571 RID: 30065 RVA: 0x00267921 File Offset: 0x00265B21
		public static bool HasAllFlags(ContinuousProperty.DataFlags flags, ContinuousProperty.DataFlags test)
		{
			return (flags & test) == test;
		}

		// Token: 0x06007572 RID: 30066 RVA: 0x00267929 File Offset: 0x00265B29
		public static bool HasAnyFlag(ContinuousProperty.DataFlags flags, ContinuousProperty.DataFlags test)
		{
			return (flags & test) > ContinuousProperty.DataFlags.None;
		}

		// Token: 0x06007573 RID: 30067 RVA: 0x00267934 File Offset: 0x00265B34
		private static void GetAllValidObjectsNonAlloc(Transform t, List<Object> objects)
		{
			objects.Clear();
			objects.Add(t.gameObject);
			foreach (Component @object in t.GetComponents<Component>())
			{
				if (ContinuousProperty.IsValidObject(@object.GetType()))
				{
					objects.Add(@object);
				}
			}
		}

		// Token: 0x06007574 RID: 30068 RVA: 0x00267980 File Offset: 0x00265B80
		private static bool IsValidObject(System.Type t)
		{
			return t != typeof(Renderer) && t != typeof(ParticleSystemRenderer);
		}

		// Token: 0x06007575 RID: 30069 RVA: 0x002679A8 File Offset: 0x00265BA8
		public ContinuousProperty()
		{
		}

		// Token: 0x06007576 RID: 30070 RVA: 0x00267A0C File Offset: 0x00265C0C
		public ContinuousProperty(ContinuousPropertyModeSO mode, Transform initialTarget, Vector2 range = default(Vector2))
		{
			this.mode = mode;
			this.target = initialTarget;
			this.range = range;
			this.ShiftTarget(0);
		}

		// Token: 0x17000B33 RID: 2867
		// (get) Token: 0x06007577 RID: 30071 RVA: 0x00267A8C File Offset: 0x00265C8C
		private string ModeTooltip
		{
			get
			{
				if (!this.mode)
				{
					return "";
				}
				return string.Format("{0}: {1}", this.mode.type, this.mode.GetDescriptionForCast(ContinuousProperty.GetTargetCast(this.target)));
			}
		}

		// Token: 0x17000B34 RID: 2868
		// (get) Token: 0x06007578 RID: 30072 RVA: 0x00267ADC File Offset: 0x00265CDC
		private bool ModeInfoVisible
		{
			get
			{
				return this.mode == null;
			}
		}

		// Token: 0x17000B35 RID: 2869
		// (get) Token: 0x06007579 RID: 30073 RVA: 0x00267AEA File Offset: 0x00265CEA
		private bool ModeErrorVisible
		{
			get
			{
				return !this.IsValid();
			}
		}

		// Token: 0x17000B36 RID: 2870
		// (get) Token: 0x0600757A RID: 30074 RVA: 0x00267AF5 File Offset: 0x00265CF5
		private string ModeErrorMessage
		{
			get
			{
				if (!(this.mode != null))
				{
					return "How did we get here?";
				}
				return "I couldn't find any valid target to apply my '" + this.mode.name + "' to in the whole prefab.\n\n" + this.mode.ListValidCasts();
			}
		}

		// Token: 0x17000B37 RID: 2871
		// (get) Token: 0x0600757B RID: 30075 RVA: 0x00267B30 File Offset: 0x00265D30
		public ContinuousPropertyModeSO Mode
		{
			get
			{
				return this.mode;
			}
		}

		// Token: 0x17000B38 RID: 2872
		// (get) Token: 0x0600757C RID: 30076 RVA: 0x00267B38 File Offset: 0x00265D38
		public ContinuousProperty.Type MyType
		{
			get
			{
				if (!(this.mode != null))
				{
					return ContinuousProperty.Type.Color;
				}
				return this.mode.type;
			}
		}

		// Token: 0x17000B39 RID: 2873
		// (get) Token: 0x0600757D RID: 30077 RVA: 0x00267B55 File Offset: 0x00265D55
		private bool HasTarget
		{
			get
			{
				return this.MyType != ContinuousProperty.Type.UnityEvent;
			}
		}

		// Token: 0x17000B3A RID: 2874
		// (get) Token: 0x0600757E RID: 30078 RVA: 0x00267B64 File Offset: 0x00265D64
		private bool TargetInfoVisible
		{
			get
			{
				return this.HasTarget && this.target == null;
			}
		}

		// Token: 0x17000B3B RID: 2875
		// (get) Token: 0x0600757F RID: 30079 RVA: 0x00267B7C File Offset: 0x00265D7C
		private string TargetTooltip
		{
			get
			{
				if (!(this.mode != null))
				{
					return "";
				}
				return this.mode.ListValidCasts();
			}
		}

		// Token: 0x17000B3C RID: 2876
		// (get) Token: 0x06007580 RID: 30080 RVA: 0x00267B9D File Offset: 0x00265D9D
		private bool ShiftButtonsVisible
		{
			get
			{
				return this.mode != null;
			}
		}

		// Token: 0x17000B3D RID: 2877
		// (get) Token: 0x06007581 RID: 30081 RVA: 0x00267BAB File Offset: 0x00265DAB
		public Object Target
		{
			get
			{
				return this.target;
			}
		}

		// Token: 0x06007582 RID: 30082 RVA: 0x00267BB3 File Offset: 0x00265DB3
		private void PreviousTarget()
		{
			this.ShiftTarget(-1);
		}

		// Token: 0x06007583 RID: 30083 RVA: 0x00267BBD File Offset: 0x00265DBD
		private void NextTarget()
		{
			this.ShiftTarget(1);
		}

		// Token: 0x06007584 RID: 30084 RVA: 0x00267BC8 File Offset: 0x00265DC8
		public bool ShiftTarget(int shiftAmount)
		{
			if (this.mode == null)
			{
				return false;
			}
			int num = -1;
			Transform transform;
			if (!(this.target != null))
			{
				transform = null;
			}
			else
			{
				GameObject gameObject = this.target as GameObject;
				transform = (((gameObject != null) ? gameObject.transform : null) ?? ((Component)this.target).transform);
			}
			Transform transform2 = transform;
			Transform transform3 = transform2;
			if (transform3 == null)
			{
				return false;
			}
			Stack<Transform> stack = new Stack<Transform>();
			stack.Push(transform3);
			List<Object> list = new List<Object>();
			List<Object> list2 = new List<Object>();
			Transform transform4;
			while (stack.TryPop(out transform4))
			{
				if (num < 0 && transform4 == transform2)
				{
					num = list.Count;
				}
				ContinuousProperty.GetAllValidObjectsNonAlloc(transform4, list2);
				foreach (Object @object in list2)
				{
					if (this.mode.IsCastValid(ContinuousProperty.GetTargetCast(@object)))
					{
						if (@object == this.target)
						{
							num = list.Count;
						}
						list.Add(@object);
					}
				}
				for (int i = transform4.childCount - 1; i >= 0; i--)
				{
					stack.Push(transform4.GetChild(i));
				}
			}
			if (list.Count == 0)
			{
				return false;
			}
			this.target = list[(num < 0) ? 0 : ((num + shiftAmount + list.Count) % list.Count)];
			return true;
		}

		// Token: 0x06007585 RID: 30085 RVA: 0x00267D48 File Offset: 0x00265F48
		private void OnModeOrTargetChanged()
		{
			if (!this.IsValid())
			{
				this.ShiftTarget(0);
			}
		}

		// Token: 0x17000B3E RID: 2878
		// (get) Token: 0x06007586 RID: 30086 RVA: 0x00267D5A File Offset: 0x00265F5A
		// (set) Token: 0x06007587 RID: 30087 RVA: 0x00267D62 File Offset: 0x00265F62
		public bool IsShaderProperty_Cached { get; private set; }

		// Token: 0x17000B3F RID: 2879
		// (get) Token: 0x06007588 RID: 30088 RVA: 0x00267D6B File Offset: 0x00265F6B
		// (set) Token: 0x06007589 RID: 30089 RVA: 0x00267D73 File Offset: 0x00265F73
		public bool UsesThreshold_Cached { get; private set; }

		// Token: 0x0600758A RID: 30090 RVA: 0x00267D7C File Offset: 0x00265F7C
		public bool IsValid()
		{
			return this.mode == null || this.target == null || this.mode.IsCastValid(ContinuousProperty.GetTargetCast(this.target));
		}

		// Token: 0x0600758B RID: 30091 RVA: 0x00267DB2 File Offset: 0x00265FB2
		public int GetTargetInstanceID()
		{
			return this.target.GetInstanceID();
		}

		// Token: 0x0600758C RID: 30092 RVA: 0x00267DBF File Offset: 0x00265FBF
		private bool HasAllFlags(ContinuousProperty.DataFlags test)
		{
			return this.mode != null && ContinuousProperty.HasAllFlags(this.mode.GetFlagsForClosestCast(ContinuousProperty.GetTargetCast(this.target)), test);
		}

		// Token: 0x0600758D RID: 30093 RVA: 0x00267DED File Offset: 0x00265FED
		private bool HasAnyFlag(ContinuousProperty.DataFlags test)
		{
			return this.mode != null && ContinuousProperty.HasAnyFlag(this.mode.GetFlagsForClosestCast(ContinuousProperty.GetTargetCast(this.target)), test);
		}

		// Token: 0x17000B40 RID: 2880
		// (get) Token: 0x0600758E RID: 30094 RVA: 0x00267E1B File Offset: 0x0026601B
		private bool HasGradient
		{
			get
			{
				return this.HasAllFlags(ContinuousProperty.DataFlags.HasColor);
			}
		}

		// Token: 0x17000B41 RID: 2881
		// (get) Token: 0x0600758F RID: 30095 RVA: 0x00267E24 File Offset: 0x00266024
		private bool HasCurve
		{
			get
			{
				return this.HasAllFlags(ContinuousProperty.DataFlags.HasCurve);
			}
		}

		// Token: 0x06007590 RID: 30096 RVA: 0x00267E30 File Offset: 0x00266030
		private string DynamicIntLabel()
		{
			if (!this.HasAllFlags(ContinuousProperty.DataFlags.IsShaderProperty))
			{
				ContinuousProperty.Type myType = this.MyType;
				if (myType != ContinuousProperty.Type.Color && myType != ContinuousProperty.Type.BlendShape)
				{
					return "Int Value";
				}
			}
			return "Material Index";
		}

		// Token: 0x17000B42 RID: 2882
		// (get) Token: 0x06007591 RID: 30097 RVA: 0x00267E60 File Offset: 0x00266060
		private bool HasInt
		{
			get
			{
				return this.HasAllFlags(ContinuousProperty.DataFlags.HasInteger);
			}
		}

		// Token: 0x17000B43 RID: 2883
		// (get) Token: 0x06007592 RID: 30098 RVA: 0x00267E69 File Offset: 0x00266069
		public int IntValue
		{
			get
			{
				return this.intValue;
			}
		}

		// Token: 0x06007593 RID: 30099 RVA: 0x00267E71 File Offset: 0x00266071
		private string DynamicStringLabel()
		{
			if (this.HasAllFlags(ContinuousProperty.DataFlags.IsShaderProperty))
			{
				return "Property Name";
			}
			if (this.HasAllFlags(ContinuousProperty.DataFlags.IsAnimatorParameter))
			{
				return "Parameter Name";
			}
			return "String Value";
		}

		// Token: 0x17000B44 RID: 2884
		// (get) Token: 0x06007594 RID: 30100 RVA: 0x00267E98 File Offset: 0x00266098
		private bool HasString
		{
			get
			{
				return this.HasAnyFlag(ContinuousProperty.DataFlags.IsShaderProperty | ContinuousProperty.DataFlags.IsAnimatorParameter);
			}
		}

		// Token: 0x17000B45 RID: 2885
		// (get) Token: 0x06007595 RID: 30101 RVA: 0x00267EA2 File Offset: 0x002660A2
		public string StringValue
		{
			get
			{
				return this.stringValue;
			}
		}

		// Token: 0x17000B46 RID: 2886
		// (get) Token: 0x06007596 RID: 30102 RVA: 0x00267EAA File Offset: 0x002660AA
		private bool HasBezier
		{
			get
			{
				return this.MyType == ContinuousProperty.Type.BezierInterpolation;
			}
		}

		// Token: 0x17000B47 RID: 2887
		// (get) Token: 0x06007597 RID: 30103 RVA: 0x00267EB5 File Offset: 0x002660B5
		private bool MissingBezier
		{
			get
			{
				return this.bezierCurve == null;
			}
		}

		// Token: 0x17000B48 RID: 2888
		// (get) Token: 0x06007598 RID: 30104 RVA: 0x00267EC3 File Offset: 0x002660C3
		private bool AxisError
		{
			get
			{
				return !Enum.IsDefined(typeof(ContinuousProperty.RotationAxis), this.localAxis);
			}
		}

		// Token: 0x17000B49 RID: 2889
		// (get) Token: 0x06007599 RID: 30105 RVA: 0x00267EE2 File Offset: 0x002660E2
		private bool HasAxisMode
		{
			get
			{
				return this.HasAllFlags(ContinuousProperty.DataFlags.HasAxis);
			}
		}

		// Token: 0x17000B4A RID: 2890
		// (get) Token: 0x0600759A RID: 30106 RVA: 0x00267EEB File Offset: 0x002660EB
		private bool InterpolationError
		{
			get
			{
				return !Enum.IsDefined(typeof(ContinuousProperty.InterpolationMode), this.interpolationMode);
			}
		}

		// Token: 0x17000B4B RID: 2891
		// (get) Token: 0x0600759B RID: 30107 RVA: 0x00267F0A File Offset: 0x0026610A
		private bool HasInterpolationMode
		{
			get
			{
				return this.HasAllFlags(ContinuousProperty.DataFlags.HasInterpolation);
			}
		}

		// Token: 0x17000B4C RID: 2892
		// (get) Token: 0x0600759C RID: 30108 RVA: 0x00267F14 File Offset: 0x00266114
		private bool HasStopAction
		{
			get
			{
				return this.MyType == ContinuousProperty.Type.PlayStop && this.target is ParticleSystem;
			}
		}

		// Token: 0x17000B4D RID: 2893
		// (get) Token: 0x0600759D RID: 30109 RVA: 0x00267F30 File Offset: 0x00266130
		private bool HasXforms
		{
			get
			{
				return this.MyType == ContinuousProperty.Type.TransformInterpolation;
			}
		}

		// Token: 0x17000B4E RID: 2894
		// (get) Token: 0x0600759E RID: 30110 RVA: 0x00267F3B File Offset: 0x0026613B
		private bool MissingXforms
		{
			get
			{
				return this.transformA == null || this.transformB == null;
			}
		}

		// Token: 0x17000B4F RID: 2895
		// (get) Token: 0x0600759F RID: 30111 RVA: 0x00267F59 File Offset: 0x00266159
		private bool HasOffsets
		{
			get
			{
				return this.MyType == ContinuousProperty.Type.OffsetInterpolation;
			}
		}

		// Token: 0x17000B50 RID: 2896
		// (get) Token: 0x060075A0 RID: 30112 RVA: 0x00267F65 File Offset: 0x00266165
		private string ThresholdErrorMessage
		{
			get
			{
				return "The threshold will always be " + ((this.thresholdOption == ContinuousProperty.ThresholdOption.Normal ^ this.range.x >= this.range.y) ? "true." : "false.");
			}
		}

		// Token: 0x17000B51 RID: 2897
		// (get) Token: 0x060075A1 RID: 30113 RVA: 0x00267FA4 File Offset: 0x002661A4
		private string ThresholdTooltip
		{
			get
			{
				if (!this.ThresholdError)
				{
					return "The threshold will be true" + ((this.thresholdOption == ContinuousProperty.ThresholdOption.Normal) ? ((this.range.x > 0f && this.range.y < 1f) ? string.Format(" between {0} and {1}", this.range.x, this.range.y) : ((this.range.x > 0f) ? (" above " + this.range.x.ToString()) : (" below " + this.range.y.ToString()))) : (((this.range.x > 0f) ? (" below " + this.range.x.ToString()) : "") + ((this.range.x > 0f && this.range.y < 1f) ? " and" : "") + ((this.range.y < 1f) ? (" above " + this.range.y.ToString()) : ""))) + ", and false otherwise.";
				}
				return this.ThresholdErrorMessage;
			}
		}

		// Token: 0x17000B52 RID: 2898
		// (get) Token: 0x060075A2 RID: 30114 RVA: 0x00268116 File Offset: 0x00266316
		private bool HasThreshold
		{
			get
			{
				return this.HasAllFlags(ContinuousProperty.DataFlags.HasThreshold);
			}
		}

		// Token: 0x17000B53 RID: 2899
		// (get) Token: 0x060075A3 RID: 30115 RVA: 0x00268124 File Offset: 0x00266324
		private bool ThresholdError
		{
			get
			{
				return (this.range.x <= 0f && this.range.y >= 1f) || this.range.x >= this.range.y;
			}
		}

		// Token: 0x17000B54 RID: 2900
		// (get) Token: 0x060075A4 RID: 30116 RVA: 0x00268172 File Offset: 0x00266372
		private bool HasEventMode
		{
			get
			{
				return this.MyType == ContinuousProperty.Type.UnityEvent && !this.HasAnyFlag(ContinuousProperty.DataFlags.HasThreshold);
			}
		}

		// Token: 0x17000B55 RID: 2901
		// (get) Token: 0x060075A5 RID: 30117 RVA: 0x0026818E File Offset: 0x0026638E
		private bool HasUnityEvent
		{
			get
			{
				return this.MyType == ContinuousProperty.Type.UnityEvent;
			}
		}

		// Token: 0x17000B56 RID: 2902
		// (get) Token: 0x060075A6 RID: 30118 RVA: 0x0026819A File Offset: 0x0026639A
		public bool RunOnlyLocally
		{
			get
			{
				return this.runOnlyLocally;
			}
		}

		// Token: 0x060075A7 RID: 30119 RVA: 0x002681A2 File Offset: 0x002663A2
		public void SetRigIsLocal(bool v)
		{
			this.rigLocal = v;
		}

		// Token: 0x060075A8 RID: 30120 RVA: 0x002681AC File Offset: 0x002663AC
		public void Init()
		{
			if (this.mode == null)
			{
				this.internalSwitchValue = 0;
				return;
			}
			ContinuousProperty.Type type = this.mode.type;
			ContinuousProperty.Cast cast = this.mode.GetClosestCast(ContinuousProperty.GetTargetCast(this.target));
			ContinuousProperty.DataFlags dataFlags = this.mode.GetFlagsForCast(cast);
			if (cast == ContinuousProperty.Cast.Null || (type == ContinuousProperty.Type.BezierInterpolation && this.MissingBezier) || (type == ContinuousProperty.Type.TransformInterpolation && this.MissingXforms) || (type == ContinuousProperty.Type.UnityEvent && this.unityEvent == null))
			{
				this.internalSwitchValue = 0;
				this.IsShaderProperty_Cached = false;
				this.UsesThreshold_Cached = false;
				return;
			}
			if (type == ContinuousProperty.Type.Color && ContinuousProperty.CastMatches(ContinuousProperty.Cast.Renderer, cast))
			{
				type = ContinuousProperty.Type.ShaderColor;
				cast = ContinuousProperty.Cast.Renderer;
				dataFlags |= ContinuousProperty.DataFlags.IsShaderProperty;
				this.stringValue = "_BaseColor";
			}
			else if (type == ContinuousProperty.Type.PlayStop && cast == ContinuousProperty.Cast.Animator)
			{
				type = ContinuousProperty.Type.EnableDisable;
				cast = ContinuousProperty.Cast.Behaviour;
			}
			this.internalSwitchValue = (int)(type | (ContinuousProperty.Type)cast | (ContinuousProperty.Type)(this.HasAxisMode ? this.localAxis : ((ContinuousProperty.RotationAxis)0)) | (ContinuousProperty.Type)(this.HasInterpolationMode ? this.interpolationMode : ((ContinuousProperty.InterpolationMode)0)) | (ContinuousProperty.Type)(this.HasEventMode ? this.eventMode : ((ContinuousProperty.EventMode)0)));
			this.IsShaderProperty_Cached = ContinuousProperty.HasAllFlags(dataFlags, ContinuousProperty.DataFlags.IsShaderProperty);
			this.UsesThreshold_Cached = ContinuousProperty.HasAllFlags(dataFlags, ContinuousProperty.DataFlags.HasThreshold);
			if (cast == ContinuousProperty.Cast.ParticleSystem)
			{
				this.particleMain = ((ParticleSystem)this.target).main;
				this.particleEmission = ((ParticleSystem)this.target).emission;
				this.speedCurveCache = this.particleMain.startSpeed;
				this.rateCurveCache = this.particleEmission.rateOverTime;
			}
			if (this.IsShaderProperty_Cached)
			{
				this.stringHash = Shader.PropertyToID(this.stringValue);
			}
			else if (ContinuousProperty.HasAllFlags(dataFlags, ContinuousProperty.DataFlags.IsAnimatorParameter))
			{
				this.stringHash = Animator.StringToHash(this.stringValue);
			}
			if (!ContinuousProperty.HasAnyFlag(dataFlags, ContinuousProperty.DataFlags.HasCurve))
			{
				this.curve = AnimationCurves.Linear;
			}
		}

		// Token: 0x060075A9 RID: 30121 RVA: 0x0026837B File Offset: 0x0026657B
		public void InitThreshold()
		{
			if (!this.UsesThreshold_Cached)
			{
				return;
			}
			this.CheckThreshold(0f);
			if (this.IsShaderProperty_Cached)
			{
				return;
			}
			this.previousBoolValue = !this.previousBoolValue;
			this.Apply(0f, 0f, null);
		}

		// Token: 0x060075AA RID: 30122 RVA: 0x002683BC File Offset: 0x002665BC
		public void Apply(float f, float deltaTime, MaterialPropertyBlock mpb)
		{
			if (this.runOnlyLocally && !this.rigLocal)
			{
				return;
			}
			int num = this.internalSwitchValue | (int)this.CheckThreshold(f);
			if (num <= 1057808)
			{
				if (num <= 6157)
				{
					if (num <= 3083)
					{
						if (num <= 2049)
						{
							if (num == 0)
							{
								return;
							}
							if (num != 2049)
							{
								return;
							}
							((Transform)this.target).localScale = this.curve.Evaluate(f) * Vector3.one;
							return;
						}
						else
						{
							if (num == 3072)
							{
								this.particleMain.startColor = this.color.Evaluate(f);
								return;
							}
							if (num == 3073)
							{
								this.particleMain.startSize = this.curve.Evaluate(f);
								return;
							}
							if (num != 3083)
							{
								return;
							}
							this.particleMain.startSpeed = this.ScaleCurve(this.speedCurveCache, this.curve.Evaluate(f));
							return;
						}
					}
					else if (num <= 4098)
					{
						if (num == 3084)
						{
							this.particleEmission.rateOverTime = this.ScaleCurve(this.rateCurveCache, this.curve.Evaluate(f));
							return;
						}
						if (num != 4098)
						{
							return;
						}
						((SkinnedMeshRenderer)this.target).SetBlendShapeWeight(this.intValue, this.curve.Evaluate(f) * 100f);
						return;
					}
					else
					{
						if (num == 5123)
						{
							((Animator)this.target).SetFloat(this.stringHash, this.curve.Evaluate(f));
							return;
						}
						if (num == 5131)
						{
							((Animator)this.target).speed = this.curve.Evaluate(f);
							return;
						}
						if (num != 6157)
						{
							return;
						}
						((AudioSource)this.target).volume = Mathf.Clamp01(this.curve.Evaluate(f));
						return;
					}
				}
				else if (num <= 1051663)
				{
					if (num <= 7173)
					{
						if (num == 6158)
						{
							((AudioSource)this.target).pitch = Mathf.Clamp(this.curve.Evaluate(f), -3f, 3f);
							return;
						}
						switch (num)
						{
						case 7171:
							mpb.SetFloat(this.stringHash, this.curve.Evaluate(f));
							return;
						case 7172:
							mpb.SetVector(this.stringHash, new Vector2(this.curve.Evaluate(f), 0f));
							return;
						case 7173:
							mpb.SetColor(this.stringHash, this.color.Evaluate(f));
							return;
						default:
							return;
						}
					}
					else
					{
						if (num == 11278)
						{
							((VoiceShiftCosmetic)this.target).Pitch = this.curve.Evaluate(f);
							return;
						}
						if (num == 1049617)
						{
							this.unityEvent.Invoke(this.curve.Evaluate(f));
							return;
						}
						if (num != 1051663)
						{
							return;
						}
						((ParticleSystem)this.target).Play();
						return;
					}
				}
				else if (num <= 1054735)
				{
					if (num != 1053706)
					{
						if (num == 1053714)
						{
							((Animator)this.target).SetTrigger(this.stringHash);
							return;
						}
						if (num != 1054735)
						{
							return;
						}
						((AudioSource)this.target).Play();
						return;
					}
				}
				else
				{
					if (num == 1055760)
					{
						goto IL_7AB;
					}
					if (num == 1056784)
					{
						goto IL_7C2;
					}
					if (num != 1057808)
					{
						return;
					}
					goto IL_7D9;
				}
			}
			else if (num <= 3150858)
			{
				if (num <= 2103311)
				{
					if (num <= 2100239)
					{
						if (num == 2098193)
						{
							return;
						}
						if (num != 2100239)
						{
							return;
						}
						((ParticleSystem)this.target).Stop(true, this.stopType);
						return;
					}
					else if (num != 2102282)
					{
						if (num == 2102290)
						{
							return;
						}
						if (num != 2103311)
						{
							return;
						}
						((AudioSource)this.target).Stop();
						return;
					}
				}
				else if (num <= 2106384)
				{
					if (num == 2104336)
					{
						goto IL_7AB;
					}
					if (num == 2105360)
					{
						goto IL_7C2;
					}
					if (num != 2106384)
					{
						return;
					}
					goto IL_7D9;
				}
				else
				{
					if (num != 3146769 && num != 3148815)
					{
						return;
					}
					return;
				}
			}
			else if (num <= 3154960)
			{
				if (num <= 3151887)
				{
					if (num != 3150866)
					{
						return;
					}
					return;
				}
				else
				{
					if (num != 3152912 && num != 3153936)
					{
						return;
					}
					return;
				}
			}
			else if (num <= 8389649)
			{
				if (num == 4195345)
				{
					this.unityEvent.Invoke(this.curve.Evaluate(f));
					return;
				}
				switch (num)
				{
				case 4196358:
					((Transform)this.target).position = this.bezierCurve.GetPoint(this.curve.Evaluate(f));
					return;
				case 4196359:
					((Transform)this.target).localRotation = Quaternion.Euler(this.curve.Evaluate(f) * 360f, 0f, 0f);
					return;
				case 4196360:
					((Transform)this.target).position = Vector3.Lerp(this.transformA.position, this.transformB.position, this.curve.Evaluate(f));
					return;
				case 4196361:
					((Transform)this.target).localPosition = Vector3.Lerp(this.offsetA.pos, this.offsetB.pos, this.curve.Evaluate(f));
					return;
				default:
				{
					if (num != 8389649)
					{
						return;
					}
					float num2 = this.curve.Evaluate(f);
					float num3 = 1f / num2;
					this.frequencyTimer += deltaTime;
					if (this.frequencyTimer >= num3)
					{
						this.frequencyTimer = Mathf.Repeat(this.frequencyTimer - num3, num3);
						this.unityEvent.Invoke(num2);
						return;
					}
					return;
				}
				}
			}
			else
			{
				switch (num)
				{
				case 8390662:
					((Transform)this.target).rotation = Quaternion.LookRotation(this.bezierCurve.GetDirection(this.curve.Evaluate(f)));
					return;
				case 8390663:
					((Transform)this.target).localRotation = Quaternion.Euler(0f, this.curve.Evaluate(f) * 360f, 0f);
					return;
				case 8390664:
					((Transform)this.target).rotation = Quaternion.Slerp(this.transformA.rotation, this.transformB.rotation, this.curve.Evaluate(f));
					return;
				case 8390665:
					((Transform)this.target).localRotation = Quaternion.Slerp(this.offsetA.rot, this.offsetB.rot, this.curve.Evaluate(f));
					return;
				default:
					if (num != 12583953)
					{
						switch (num)
						{
						case 12584966:
						{
							float t = this.curve.Evaluate(f);
							((Transform)this.target).SetPositionAndRotation(this.bezierCurve.GetPoint(t), Quaternion.LookRotation(this.bezierCurve.GetDirection(t)));
							return;
						}
						case 12584967:
							((Transform)this.target).localRotation = Quaternion.Euler(0f, 0f, this.curve.Evaluate(f) * 360f);
							return;
						case 12584968:
						{
							Vector3 a;
							Quaternion a2;
							this.transformA.GetPositionAndRotation(out a, out a2);
							Vector3 b;
							Quaternion b2;
							this.transformB.GetPositionAndRotation(out b, out b2);
							float t2 = this.curve.Evaluate(f);
							((Transform)this.target).SetPositionAndRotation(Vector3.Lerp(a, b, t2), Quaternion.Slerp(a2, b2, t2));
							return;
						}
						case 12584969:
						{
							float t3 = this.curve.Evaluate(f);
							((Transform)this.target).SetLocalPositionAndRotation(Vector3.Lerp(this.offsetA.pos, this.offsetB.pos, t3), Quaternion.Slerp(this.offsetA.rot, this.offsetB.rot, t3));
							return;
						}
						default:
							return;
						}
					}
					else
					{
						float num4 = this.curve.Evaluate(f);
						float num5 = 1f - Mathf.Exp(-num4 * deltaTime);
						if (Random.value < num5)
						{
							this.unityEvent.Invoke(num4);
							return;
						}
						return;
					}
					break;
				}
			}
			((Animator)this.target).SetBool(this.stringHash, this.previousBoolValue);
			return;
			IL_7AB:
			((Renderer)this.target).enabled = this.previousBoolValue;
			return;
			IL_7C2:
			((Behaviour)this.target).enabled = this.previousBoolValue;
			return;
			IL_7D9:
			((GameObject)this.target).SetActive(this.previousBoolValue);
		}

		// Token: 0x060075AB RID: 30123 RVA: 0x00268C90 File Offset: 0x00266E90
		private ParticleSystem.MinMaxCurve ScaleCurve(in ParticleSystem.MinMaxCurve inCurve, float scale)
		{
			ParticleSystem.MinMaxCurve result = inCurve;
			switch (result.mode)
			{
			case ParticleSystemCurveMode.Constant:
				result.constant *= scale;
				break;
			case ParticleSystemCurveMode.Curve:
			case ParticleSystemCurveMode.TwoCurves:
				result.curveMultiplier *= scale;
				break;
			case ParticleSystemCurveMode.TwoConstants:
				result.constantMin *= scale;
				result.constantMax *= scale;
				break;
			}
			return result;
		}

		// Token: 0x060075AC RID: 30124 RVA: 0x00268D08 File Offset: 0x00266F08
		private bool CheckContinuousEvent(float f, float deltaTime)
		{
			ContinuousProperty.EventMode eventMode = this.eventMode;
			if (eventMode == ContinuousProperty.EventMode.Passthrough)
			{
				return true;
			}
			if (eventMode != ContinuousProperty.EventMode.Frequency)
			{
				if (eventMode != ContinuousProperty.EventMode.AveragePerSecond)
				{
					return false;
				}
				float num = 1f - Mathf.Exp(-f * deltaTime);
				return Random.value < num;
			}
			else
			{
				this.frequencyTimer += deltaTime;
				if (this.frequencyTimer < f)
				{
					return false;
				}
				this.frequencyTimer = Mathf.Repeat(this.frequencyTimer - f, f);
				return true;
			}
		}

		// Token: 0x060075AD RID: 30125 RVA: 0x00268D84 File Offset: 0x00266F84
		private ContinuousProperty.ThresholdResult CheckThreshold(float f)
		{
			if (!this.UsesThreshold_Cached)
			{
				return ContinuousProperty.ThresholdResult.Null;
			}
			bool flag = f >= this.range.x && f <= this.range.y;
			if (!this.previousBoolValue && ((this.thresholdOption == ContinuousProperty.ThresholdOption.Normal && flag) || (this.thresholdOption == ContinuousProperty.ThresholdOption.Invert && !flag)))
			{
				this.previousBoolValue = true;
				return ContinuousProperty.ThresholdResult.RisingEdge;
			}
			if (this.previousBoolValue && ((this.thresholdOption == ContinuousProperty.ThresholdOption.Normal && !flag) || (this.thresholdOption == ContinuousProperty.ThresholdOption.Invert && flag)))
			{
				this.previousBoolValue = false;
				return ContinuousProperty.ThresholdResult.FallingEdge;
			}
			return ContinuousProperty.ThresholdResult.Unchanged;
		}

		// Token: 0x04008710 RID: 34576
		[SerializeField]
		private ContinuousPropertyModeSO mode;

		// Token: 0x04008711 RID: 34577
		[FormerlySerializedAs("component")]
		[SerializeField]
		protected Object target;

		// Token: 0x04008714 RID: 34580
		[SerializeField]
		private Gradient color;

		// Token: 0x04008715 RID: 34581
		[SerializeField]
		private AnimationCurve curve = AnimationCurves.Linear;

		// Token: 0x04008716 RID: 34582
		[FormerlySerializedAs("materialIndex")]
		[SerializeField]
		private int intValue;

		// Token: 0x04008717 RID: 34583
		[SerializeField]
		private string stringValue;

		// Token: 0x04008718 RID: 34584
		[SerializeField]
		private BezierCurve bezierCurve;

		// Token: 0x04008719 RID: 34585
		private const string ENUM_ERROR = "Internal values were changed at some point. Please select a new value.";

		// Token: 0x0400871A RID: 34586
		[SerializeField]
		private ContinuousProperty.RotationAxis localAxis = ContinuousProperty.RotationAxis.X;

		// Token: 0x0400871B RID: 34587
		[SerializeField]
		private ContinuousProperty.InterpolationMode interpolationMode = ContinuousProperty.InterpolationMode.PositionAndRotation;

		// Token: 0x0400871C RID: 34588
		[SerializeField]
		private ParticleSystemStopBehavior stopType = ParticleSystemStopBehavior.StopEmitting;

		// Token: 0x0400871D RID: 34589
		[SerializeField]
		private Transform transformA;

		// Token: 0x0400871E RID: 34590
		[SerializeField]
		private Transform transformB;

		// Token: 0x0400871F RID: 34591
		[SerializeField]
		private XformOffset offsetA;

		// Token: 0x04008720 RID: 34592
		[SerializeField]
		private XformOffset offsetB;

		// Token: 0x04008721 RID: 34593
		[SerializeField]
		private Vector2 range = new Vector2(0.5f, 1f);

		// Token: 0x04008722 RID: 34594
		[SerializeField]
		private ContinuousProperty.ThresholdOption thresholdOption = ContinuousProperty.ThresholdOption.Normal;

		// Token: 0x04008723 RID: 34595
		[SerializeField]
		private ContinuousProperty.EventMode eventMode = ContinuousProperty.EventMode.Passthrough;

		// Token: 0x04008724 RID: 34596
		[SerializeField]
		private UnityEvent<float> unityEvent;

		// Token: 0x04008725 RID: 34597
		[Tooltip("Check this box if only the owner/local player is supposed to run this property.")]
		[SerializeField]
		private bool runOnlyLocally;

		// Token: 0x04008726 RID: 34598
		private bool rigLocal;

		// Token: 0x04008727 RID: 34599
		private int internalSwitchValue;

		// Token: 0x04008728 RID: 34600
		private ParticleSystem.MainModule particleMain;

		// Token: 0x04008729 RID: 34601
		private ParticleSystem.EmissionModule particleEmission;

		// Token: 0x0400872A RID: 34602
		private ParticleSystem.MinMaxCurve speedCurveCache;

		// Token: 0x0400872B RID: 34603
		private ParticleSystem.MinMaxCurve rateCurveCache;

		// Token: 0x0400872C RID: 34604
		private float frequencyTimer;

		// Token: 0x0400872D RID: 34605
		private bool previousBoolValue;

		// Token: 0x0400872E RID: 34606
		private int stringHash;

		// Token: 0x0200124F RID: 4687
		public enum Type
		{
			// Token: 0x04008730 RID: 34608
			Color,
			// Token: 0x04008731 RID: 34609
			Scale,
			// Token: 0x04008732 RID: 34610
			BlendShape,
			// Token: 0x04008733 RID: 34611
			Float,
			// Token: 0x04008734 RID: 34612
			ShaderVector2_X,
			// Token: 0x04008735 RID: 34613
			ShaderColor,
			// Token: 0x04008736 RID: 34614
			BezierInterpolation,
			// Token: 0x04008737 RID: 34615
			AxisAngle,
			// Token: 0x04008738 RID: 34616
			TransformInterpolation,
			// Token: 0x04008739 RID: 34617
			OffsetInterpolation,
			// Token: 0x0400873A RID: 34618
			Boolean,
			// Token: 0x0400873B RID: 34619
			Speed,
			// Token: 0x0400873C RID: 34620
			Rate,
			// Token: 0x0400873D RID: 34621
			Volume,
			// Token: 0x0400873E RID: 34622
			Pitch,
			// Token: 0x0400873F RID: 34623
			PlayStop,
			// Token: 0x04008740 RID: 34624
			EnableDisable,
			// Token: 0x04008741 RID: 34625
			UnityEvent,
			// Token: 0x04008742 RID: 34626
			Trigger
		}

		// Token: 0x02001250 RID: 4688
		public enum Cast
		{
			// Token: 0x04008744 RID: 34628
			Null,
			// Token: 0x04008745 RID: 34629
			Any = 1024,
			// Token: 0x04008746 RID: 34630
			Transform = 2048,
			// Token: 0x04008747 RID: 34631
			ParticleSystem = 3072,
			// Token: 0x04008748 RID: 34632
			SkinnedMeshRenderer = 4096,
			// Token: 0x04008749 RID: 34633
			Animator = 5120,
			// Token: 0x0400874A RID: 34634
			AudioSource = 6144,
			// Token: 0x0400874B RID: 34635
			Renderer = 7168,
			// Token: 0x0400874C RID: 34636
			Behaviour = 8192,
			// Token: 0x0400874D RID: 34637
			GameObject = 9216,
			// Token: 0x0400874E RID: 34638
			Rigidbody = 10240,
			// Token: 0x0400874F RID: 34639
			VoicePitchShiftCosmetic = 11264
		}

		// Token: 0x02001251 RID: 4689
		[Flags]
		public enum DataFlags
		{
			// Token: 0x04008751 RID: 34641
			None = 0,
			// Token: 0x04008752 RID: 34642
			[Tooltip("Expose the AnimationCurve for single values")]
			HasCurve = 1,
			// Token: 0x04008753 RID: 34643
			[Tooltip("Expose the Gradient for colors")]
			HasColor = 2,
			// Token: 0x04008754 RID: 34644
			[Tooltip("Select which axis it should rotate on")]
			HasAxis = 4,
			// Token: 0x04008755 RID: 34645
			[Tooltip("Expose the integer, usually for material index")]
			HasInteger = 8,
			// Token: 0x04008756 RID: 34646
			[Tooltip("Select whether to use position, rotation, or both when interpolating")]
			HasInterpolation = 16,
			// Token: 0x04008757 RID: 34647
			[Tooltip("Expose the string and hash it into a shader property ID")]
			IsShaderProperty = 32,
			// Token: 0x04008758 RID: 34648
			[Tooltip("Expose the string and hash it into an animator parameter ID")]
			IsAnimatorParameter = 64,
			// Token: 0x04008759 RID: 34649
			[Tooltip("Expose the threshold range as a dual slider")]
			HasThreshold = 128
		}

		// Token: 0x02001252 RID: 4690
		private enum ThresholdResult
		{
			// Token: 0x0400875B RID: 34651
			Null,
			// Token: 0x0400875C RID: 34652
			RisingEdge = 1048576,
			// Token: 0x0400875D RID: 34653
			FallingEdge = 2097152,
			// Token: 0x0400875E RID: 34654
			Unchanged = 3145728
		}

		// Token: 0x02001253 RID: 4691
		private enum ThresholdOption
		{
			// Token: 0x04008760 RID: 34656
			Invert,
			// Token: 0x04008761 RID: 34657
			Normal
		}

		// Token: 0x02001254 RID: 4692
		private enum RotationAxis
		{
			// Token: 0x04008763 RID: 34659
			X = 4194304,
			// Token: 0x04008764 RID: 34660
			Y = 8388608,
			// Token: 0x04008765 RID: 34661
			Z = 12582912
		}

		// Token: 0x02001255 RID: 4693
		public enum InterpolationMode
		{
			// Token: 0x04008767 RID: 34663
			Position = 4194304,
			// Token: 0x04008768 RID: 34664
			Rotation = 8388608,
			// Token: 0x04008769 RID: 34665
			PositionAndRotation = 12582912
		}

		// Token: 0x02001256 RID: 4694
		public enum EventMode
		{
			// Token: 0x0400876B RID: 34667
			Passthrough = 4194304,
			// Token: 0x0400876C RID: 34668
			Frequency = 8388608,
			// Token: 0x0400876D RID: 34669
			AveragePerSecond = 12582912
		}
	}
}
