using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001371 RID: 4977
	public class BoingReactorField : BoingBase
	{
		// Token: 0x17000BE8 RID: 3048
		// (get) Token: 0x06007D52 RID: 32082 RVA: 0x00290C83 File Offset: 0x0028EE83
		public static BoingReactorField.ShaderPropertyIdSet ShaderPropertyId
		{
			get
			{
				if (BoingReactorField.s_shaderPropertyId == null)
				{
					BoingReactorField.s_shaderPropertyId = new BoingReactorField.ShaderPropertyIdSet();
				}
				return BoingReactorField.s_shaderPropertyId;
			}
		}

		// Token: 0x06007D53 RID: 32083 RVA: 0x00290C9C File Offset: 0x0028EE9C
		public bool UpdateShaderConstants(MaterialPropertyBlock props, float positionSampleMultiplier = 1f, float rotationSampleMultiplier = 1f)
		{
			if (this.HardwareMode != BoingReactorField.HardwareModeEnum.GPU)
			{
				return false;
			}
			if (this.m_fieldParamsBuffer == null || this.m_cellsBuffer == null)
			{
				return false;
			}
			props.SetFloat(BoingReactorField.ShaderPropertyId.PositionSampleMultiplier, positionSampleMultiplier);
			props.SetFloat(BoingReactorField.ShaderPropertyId.RotationSampleMultiplier, rotationSampleMultiplier);
			props.SetBuffer(BoingReactorField.ShaderPropertyId.RenderFieldParams, this.m_fieldParamsBuffer);
			props.SetBuffer(BoingReactorField.ShaderPropertyId.RenderCells, this.m_cellsBuffer);
			return true;
		}

		// Token: 0x06007D54 RID: 32084 RVA: 0x00290D18 File Offset: 0x0028EF18
		public bool UpdateShaderConstants(Material material, float positionSampleMultiplier = 1f, float rotationSampleMultiplier = 1f)
		{
			if (this.HardwareMode != BoingReactorField.HardwareModeEnum.GPU)
			{
				return false;
			}
			if (this.m_fieldParamsBuffer == null || this.m_cellsBuffer == null)
			{
				return false;
			}
			material.SetFloat(BoingReactorField.ShaderPropertyId.PositionSampleMultiplier, positionSampleMultiplier);
			material.SetFloat(BoingReactorField.ShaderPropertyId.RotationSampleMultiplier, rotationSampleMultiplier);
			material.SetBuffer(BoingReactorField.ShaderPropertyId.RenderFieldParams, this.m_fieldParamsBuffer);
			material.SetBuffer(BoingReactorField.ShaderPropertyId.RenderCells, this.m_cellsBuffer);
			return true;
		}

		// Token: 0x17000BE9 RID: 3049
		// (get) Token: 0x06007D55 RID: 32085 RVA: 0x00290D91 File Offset: 0x0028EF91
		public int GpuResourceSetId
		{
			get
			{
				return this.m_gpuResourceSetId;
			}
		}

		// Token: 0x06007D56 RID: 32086 RVA: 0x00290D9C File Offset: 0x0028EF9C
		public BoingReactorField()
		{
			this.Params.Init();
			this.m_bounds = Aabb.Empty;
			this.m_init = false;
		}

		// Token: 0x06007D57 RID: 32087 RVA: 0x00290E7C File Offset: 0x0028F07C
		public void Reboot()
		{
			this.m_gridCenter = base.transform.position;
			Vector3 vector = this.QuantizeNorm(this.m_gridCenter);
			this.m_qPrevGridCenterNorm = vector;
			BoingReactorField.CellMoveModeEnum cellMoveMode = this.CellMoveMode;
			if (cellMoveMode == BoingReactorField.CellMoveModeEnum.Follow)
			{
				this.m_gridCenter = base.transform.position;
				this.m_iCellBaseX = 0;
				this.m_iCellBaseY = 0;
				this.m_iCellBaseZ = 0;
				this.m_iCellBaseZ = 0;
				this.m_iCellBaseZ = 0;
				return;
			}
			if (cellMoveMode != BoingReactorField.CellMoveModeEnum.WrapAround)
			{
				return;
			}
			this.m_gridCenter = vector * this.CellSize;
			this.m_iCellBaseX = MathUtil.Modulo((int)this.m_qPrevGridCenterNorm.x, this.CellsX);
			this.m_iCellBaseY = MathUtil.Modulo((int)this.m_qPrevGridCenterNorm.y, this.CellsY);
			this.m_iCellBaseZ = MathUtil.Modulo((int)this.m_qPrevGridCenterNorm.z, this.CellsZ);
		}

		// Token: 0x06007D58 RID: 32088 RVA: 0x00290F5B File Offset: 0x0028F15B
		public void OnEnable()
		{
			this.Reboot();
			BoingManager.Register(this);
		}

		// Token: 0x06007D59 RID: 32089 RVA: 0x00290F69 File Offset: 0x0028F169
		public void Start()
		{
			this.Reboot();
			this.m_cellMoveMode = this.CellMoveMode;
		}

		// Token: 0x06007D5A RID: 32090 RVA: 0x00290F7D File Offset: 0x0028F17D
		public void OnDisable()
		{
			BoingManager.Unregister(this);
			this.DisposeCpuResources();
			this.DisposeGpuResources();
		}

		// Token: 0x06007D5B RID: 32091 RVA: 0x00290F91 File Offset: 0x0028F191
		public void DisposeCpuResources()
		{
			this.m_aCpuCell = null;
		}

		// Token: 0x06007D5C RID: 32092 RVA: 0x00290F9C File Offset: 0x0028F19C
		public void DisposeGpuResources()
		{
			if (this.m_effectorIndexBuffer != null)
			{
				this.m_effectorIndexBuffer.Dispose();
				this.m_effectorIndexBuffer = null;
			}
			if (this.m_reactorParamsBuffer != null)
			{
				this.m_reactorParamsBuffer.Dispose();
				this.m_reactorParamsBuffer = null;
			}
			if (this.m_fieldParamsBuffer != null)
			{
				this.m_fieldParamsBuffer.Dispose();
				this.m_fieldParamsBuffer = null;
			}
			if (this.m_cellsBuffer != null)
			{
				this.m_cellsBuffer.Dispose();
				this.m_cellsBuffer = null;
			}
			if (this.m_cellsBuffer != null)
			{
				this.m_cellsBuffer.Dispose();
				this.m_cellsBuffer = null;
			}
		}

		// Token: 0x06007D5D RID: 32093 RVA: 0x0029102C File Offset: 0x0028F22C
		public bool SampleCpuGrid(Vector3 p, out Vector3 positionOffset, out Vector4 rotationOffset)
		{
			bool flag = false;
			switch (this.FalloffDimensions)
			{
			case BoingReactorField.FalloffDimensionsEnum.XYZ:
				flag = this.m_bounds.Contains(p);
				break;
			case BoingReactorField.FalloffDimensionsEnum.XY:
				flag = (this.m_bounds.ContainsX(p) && this.m_bounds.ContainsY(p));
				break;
			case BoingReactorField.FalloffDimensionsEnum.XZ:
				flag = (this.m_bounds.ContainsX(p) && this.m_bounds.ContainsZ(p));
				break;
			case BoingReactorField.FalloffDimensionsEnum.YZ:
				flag = (this.m_bounds.ContainsY(p) && this.m_bounds.ContainsZ(p));
				break;
			}
			if (!flag)
			{
				positionOffset = Vector3.zero;
				rotationOffset = QuaternionUtil.ToVector4(Quaternion.identity);
				return false;
			}
			float num = 0.5f * this.CellSize;
			Vector3 a = p - (this.m_gridCenter + this.GetCellCenterOffset(0, 0, 0));
			Vector3 vector = this.QuantizeNorm(a + new Vector3(-num, -num, -num));
			Vector3 b = vector * this.CellSize;
			int num2 = Mathf.Clamp((int)vector.x, 0, this.CellsX - 1);
			int num3 = Mathf.Clamp((int)vector.y, 0, this.CellsY - 1);
			int num4 = Mathf.Clamp((int)vector.z, 0, this.CellsZ - 1);
			int x = Mathf.Min(num2 + 1, this.CellsX - 1);
			int y = Mathf.Min(num3 + 1, this.CellsY - 1);
			int z = Mathf.Min(num4 + 1, this.CellsZ - 1);
			int num5;
			int num6;
			int num7;
			this.ResolveCellIndex(num2, num3, num4, 1, out num5, out num6, out num7);
			int num8;
			int num9;
			int num10;
			this.ResolveCellIndex(x, y, z, 1, out num8, out num9, out num10);
			bool lerpX = num5 != num8;
			bool lerpY = num6 != num9;
			bool lerpZ = num7 != num10;
			Vector3 vector2 = (a - b) / this.CellSize;
			Vector3 vector3 = p - base.transform.position;
			switch (this.FalloffDimensions)
			{
			case BoingReactorField.FalloffDimensionsEnum.XY:
				vector3.z = 0f;
				break;
			case BoingReactorField.FalloffDimensionsEnum.XZ:
				vector3.y = 0f;
				break;
			case BoingReactorField.FalloffDimensionsEnum.YZ:
				vector3.x = 0f;
				break;
			}
			int num11 = Mathf.Max(this.CellsX, Mathf.Max(this.CellsY, this.CellsZ));
			float num12 = 1f;
			BoingReactorField.FalloffModeEnum falloffMode = this.FalloffMode;
			if (falloffMode != BoingReactorField.FalloffModeEnum.Circle)
			{
				if (falloffMode == BoingReactorField.FalloffModeEnum.Square)
				{
					Vector3 a2 = num * new Vector3((float)this.CellsX, (float)this.CellsY, (float)this.CellsZ);
					Vector3 vector4 = this.FalloffRatio * a2 - num * Vector3.one;
					vector4.x = Mathf.Max(0f, vector4.x);
					vector4.y = Mathf.Max(0f, vector4.y);
					vector4.z = Mathf.Max(0f, vector4.z);
					Vector3 vector5 = (1f - this.FalloffRatio) * a2 - num * Vector3.one;
					vector5.x = Mathf.Max(MathUtil.Epsilon, vector5.x);
					vector5.y = Mathf.Max(MathUtil.Epsilon, vector5.y);
					vector5.z = Mathf.Max(MathUtil.Epsilon, vector5.z);
					Vector3 vector6 = new Vector3(1f - Mathf.Clamp01((Mathf.Abs(vector3.x) - vector4.x) / vector5.x), 1f - Mathf.Clamp01((Mathf.Abs(vector3.y) - vector4.y) / vector5.y), 1f - Mathf.Clamp01((Mathf.Abs(vector3.z) - vector4.z) / vector5.z));
					switch (this.FalloffDimensions)
					{
					case BoingReactorField.FalloffDimensionsEnum.XY:
						vector6.x = 1f;
						break;
					case BoingReactorField.FalloffDimensionsEnum.XZ:
						vector6.y = 1f;
						break;
					case BoingReactorField.FalloffDimensionsEnum.YZ:
						vector6.z = 1f;
						break;
					}
					num12 = Mathf.Min(vector6.x, Mathf.Min(vector6.y, vector6.z));
				}
			}
			else
			{
				float num13 = num * (float)num11;
				Vector3 vector7 = new Vector3((float)num11 / (float)this.CellsX, (float)num11 / (float)this.CellsY, (float)num11 / (float)this.CellsZ);
				vector3.x *= vector7.x;
				vector3.y *= vector7.y;
				vector3.z *= vector7.z;
				float magnitude = vector3.magnitude;
				float num14 = Mathf.Max(0f, this.FalloffRatio * num13 - num);
				float num15 = Mathf.Max(MathUtil.Epsilon, (1f - this.FalloffRatio) * num13 - num);
				num12 = 1f - Mathf.Clamp01((magnitude - num14) / num15);
			}
			BoingReactorField.s_aCellOffset[0] = this.m_aCpuCell[num7, num6, num5].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(num2, num3, num4);
			BoingReactorField.s_aCellOffset[1] = this.m_aCpuCell[num7, num6, num8].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(x, num3, num4);
			BoingReactorField.s_aCellOffset[2] = this.m_aCpuCell[num7, num9, num5].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(num2, y, num4);
			BoingReactorField.s_aCellOffset[3] = this.m_aCpuCell[num7, num9, num8].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(x, y, num4);
			BoingReactorField.s_aCellOffset[4] = this.m_aCpuCell[num10, num6, num5].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(num2, num3, z);
			BoingReactorField.s_aCellOffset[5] = this.m_aCpuCell[num10, num6, num8].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(x, num3, z);
			BoingReactorField.s_aCellOffset[6] = this.m_aCpuCell[num10, num9, num5].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(num2, y, z);
			BoingReactorField.s_aCellOffset[7] = this.m_aCpuCell[num10, num9, num8].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(x, y, z);
			positionOffset = VectorUtil.TriLerp(ref BoingReactorField.s_aCellOffset[0], ref BoingReactorField.s_aCellOffset[1], ref BoingReactorField.s_aCellOffset[2], ref BoingReactorField.s_aCellOffset[3], ref BoingReactorField.s_aCellOffset[4], ref BoingReactorField.s_aCellOffset[5], ref BoingReactorField.s_aCellOffset[6], ref BoingReactorField.s_aCellOffset[7], lerpX, lerpY, lerpZ, vector2.x, vector2.y, vector2.z);
			rotationOffset = VectorUtil.TriLerp(ref this.m_aCpuCell[num7, num6, num5].RotationSpring.ValueVec, ref this.m_aCpuCell[num7, num6, num8].RotationSpring.ValueVec, ref this.m_aCpuCell[num7, num9, num5].RotationSpring.ValueVec, ref this.m_aCpuCell[num7, num9, num8].RotationSpring.ValueVec, ref this.m_aCpuCell[num10, num6, num5].RotationSpring.ValueVec, ref this.m_aCpuCell[num10, num6, num8].RotationSpring.ValueVec, ref this.m_aCpuCell[num10, num9, num5].RotationSpring.ValueVec, ref this.m_aCpuCell[num10, num9, num8].RotationSpring.ValueVec, lerpX, lerpY, lerpZ, vector2.x, vector2.y, vector2.z);
			positionOffset *= num12;
			rotationOffset = QuaternionUtil.ToVector4(QuaternionUtil.Pow(QuaternionUtil.FromVector4(rotationOffset, true), num12));
			return true;
		}

		// Token: 0x06007D5E RID: 32094 RVA: 0x002918FC File Offset: 0x0028FAFC
		private void UpdateFieldParamsGpu()
		{
			this.m_fieldParams.CellsX = this.CellsX;
			this.m_fieldParams.CellsY = this.CellsY;
			this.m_fieldParams.CellsZ = this.CellsZ;
			this.m_fieldParams.NumEffectors = 0;
			if (this.Effectors != null)
			{
				foreach (BoingEffector boingEffector in this.Effectors)
				{
					if (!(boingEffector == null))
					{
						BoingEffector component = boingEffector.GetComponent<BoingEffector>();
						if (!(component == null) && component.isActiveAndEnabled)
						{
							this.m_fieldParams.NumEffectors = this.m_fieldParams.NumEffectors + 1;
						}
					}
				}
			}
			this.m_fieldParams.iCellBaseX = this.m_iCellBaseX;
			this.m_fieldParams.iCellBaseY = this.m_iCellBaseY;
			this.m_fieldParams.iCellBaseZ = this.m_iCellBaseZ;
			this.m_fieldParams.FalloffMode = (int)this.FalloffMode;
			this.m_fieldParams.FalloffDimensions = (int)this.FalloffDimensions;
			this.m_fieldParams.PropagationDepth = this.PropagationDepth;
			this.m_fieldParams.GridCenter = this.m_gridCenter;
			this.m_fieldParams.UpWs = (this.Params.Bits.IsBitSet(6) ? this.Params.RotationReactionUp : (base.transform.rotation * VectorUtil.NormalizeSafe(this.Params.RotationReactionUp, Vector3.up)));
			this.m_fieldParams.FieldPosition = base.transform.position;
			this.m_fieldParams.FalloffRatio = this.FalloffRatio;
			this.m_fieldParams.CellSize = this.CellSize;
			this.m_fieldParams.DeltaTime = Time.deltaTime;
			if (this.m_fieldParamsBuffer != null)
			{
				this.m_fieldParamsBuffer.SetData(new BoingReactorField.FieldParams[]
				{
					this.m_fieldParams
				});
			}
		}

		// Token: 0x06007D5F RID: 32095 RVA: 0x00291AE0 File Offset: 0x0028FCE0
		private void UpdateFlags()
		{
			this.Params.Bits.SetBit(0, this.TwoDDistanceCheck);
			this.Params.Bits.SetBit(1, this.TwoDPositionInfluence);
			this.Params.Bits.SetBit(2, this.TwoDRotationInfluence);
			this.Params.Bits.SetBit(3, this.EnablePositionEffect);
			this.Params.Bits.SetBit(4, this.EnableRotationEffect);
			this.Params.Bits.SetBit(6, this.GlobalReactionUpVector);
			this.Params.Bits.SetBit(7, this.EnablePropagation);
			this.Params.Bits.SetBit(8, this.AnchorPropagationAtBorder);
		}

		// Token: 0x06007D60 RID: 32096 RVA: 0x00291BA8 File Offset: 0x0028FDA8
		public void UpdateBounds()
		{
			this.m_bounds = new Aabb(this.m_gridCenter + this.GetCellCenterOffset(0, 0, 0), this.m_gridCenter + this.GetCellCenterOffset(this.CellsX - 1, this.CellsY - 1, this.CellsZ - 1));
			this.m_bounds.Expand(this.CellSize);
		}

		// Token: 0x06007D61 RID: 32097 RVA: 0x00291C10 File Offset: 0x0028FE10
		public void PrepareExecute()
		{
			this.Init();
			if (this.SharedParams != null)
			{
				BoingWork.Params.Copy(ref this.SharedParams.Params, ref this.Params);
			}
			this.UpdateFlags();
			this.UpdateBounds();
			BoingReactorField.HardwareModeEnum hardwareMode;
			if (this.m_hardwareMode != this.HardwareMode)
			{
				hardwareMode = this.m_hardwareMode;
				if (hardwareMode != BoingReactorField.HardwareModeEnum.CPU)
				{
					if (hardwareMode == BoingReactorField.HardwareModeEnum.GPU)
					{
						this.DisposeGpuResources();
					}
				}
				else
				{
					this.DisposeCpuResources();
				}
				this.m_hardwareMode = this.HardwareMode;
			}
			hardwareMode = this.m_hardwareMode;
			if (hardwareMode != BoingReactorField.HardwareModeEnum.CPU)
			{
				if (hardwareMode == BoingReactorField.HardwareModeEnum.GPU)
				{
					this.ValidateGpuResources();
				}
			}
			else
			{
				this.ValidateCpuResources();
			}
			this.HandleCellMove();
			hardwareMode = this.m_hardwareMode;
			if (hardwareMode == BoingReactorField.HardwareModeEnum.CPU)
			{
				this.FinishPrepareExecuteCpu();
				return;
			}
			if (hardwareMode != BoingReactorField.HardwareModeEnum.GPU)
			{
				return;
			}
			this.FinishPrepareExecuteGpu();
		}

		// Token: 0x06007D62 RID: 32098 RVA: 0x00291CCC File Offset: 0x0028FECC
		private void ValidateCpuResources()
		{
			this.CellsX = Mathf.Max(1, this.CellsX);
			this.CellsY = Mathf.Max(1, this.CellsY);
			this.CellsZ = Mathf.Max(1, this.CellsZ);
			if (this.m_aCpuCell == null || this.m_cellsX != this.CellsX || this.m_cellsY != this.CellsY || this.m_cellsZ != this.CellsZ)
			{
				this.m_aCpuCell = new BoingWork.Params.InstanceData[this.CellsZ, this.CellsY, this.CellsX];
				for (int i = 0; i < this.CellsZ; i++)
				{
					for (int j = 0; j < this.CellsY; j++)
					{
						for (int k = 0; k < this.CellsX; k++)
						{
							int x;
							int y;
							int z;
							this.ResolveCellIndex(k, j, i, -1, out x, out y, out z);
							this.m_aCpuCell[i, j, k].Reset(this.m_gridCenter + this.GetCellCenterOffset(x, y, z), false);
						}
					}
				}
				this.m_cellsX = this.CellsX;
				this.m_cellsY = this.CellsY;
				this.m_cellsZ = this.CellsZ;
			}
		}

		// Token: 0x06007D63 RID: 32099 RVA: 0x00291DFC File Offset: 0x0028FFFC
		private void ValidateGpuResources()
		{
			bool flag = false;
			bool flag2 = this.m_shader == null || BoingReactorField.s_computeKernelId == null;
			if (flag2)
			{
				this.m_shader = Resources.Load<ComputeShader>("Boing Kit/BoingReactorFieldCompute");
				flag = true;
				if (BoingReactorField.s_computeKernelId == null)
				{
					BoingReactorField.s_computeKernelId = new BoingReactorField.ComputeKernelId();
					BoingReactorField.s_computeKernelId.InitKernel = this.m_shader.FindKernel("Init");
					BoingReactorField.s_computeKernelId.MoveKernel = this.m_shader.FindKernel("Move");
					BoingReactorField.s_computeKernelId.WrapXKernel = this.m_shader.FindKernel("WrapX");
					BoingReactorField.s_computeKernelId.WrapYKernel = this.m_shader.FindKernel("WrapY");
					BoingReactorField.s_computeKernelId.WrapZKernel = this.m_shader.FindKernel("WrapZ");
					BoingReactorField.s_computeKernelId.ExecuteKernel = this.m_shader.FindKernel("Execute");
				}
			}
			bool flag3 = this.m_effectorIndexBuffer == null || (this.Effectors != null && this.m_numEffectors != this.Effectors.Length);
			if (flag3 && this.Effectors != null)
			{
				if (this.m_effectorIndexBuffer != null)
				{
					this.m_effectorIndexBuffer.Dispose();
				}
				this.m_effectorIndexBuffer = new ComputeBuffer(this.Effectors.Length, 4);
				flag = true;
				this.m_numEffectors = this.Effectors.Length;
			}
			if (flag2 || flag3)
			{
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.ExecuteKernel, BoingReactorField.ShaderPropertyId.EffectorIndices, this.m_effectorIndexBuffer);
			}
			bool flag4 = this.m_reactorParamsBuffer == null;
			if (flag4)
			{
				this.m_reactorParamsBuffer = new ComputeBuffer(1, BoingWork.Params.Stride);
				flag = true;
			}
			if (flag2 || flag4)
			{
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.ExecuteKernel, BoingReactorField.ShaderPropertyId.ReactorParams, this.m_reactorParamsBuffer);
			}
			bool flag5 = this.m_fieldParamsBuffer == null;
			if (flag5)
			{
				this.m_fieldParamsBuffer = new ComputeBuffer(1, BoingReactorField.FieldParams.Stride);
				flag = true;
			}
			if (flag2 || flag5)
			{
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.InitKernel, BoingReactorField.ShaderPropertyId.ComputeFieldParams, this.m_fieldParamsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.MoveKernel, BoingReactorField.ShaderPropertyId.ComputeFieldParams, this.m_fieldParamsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.WrapXKernel, BoingReactorField.ShaderPropertyId.ComputeFieldParams, this.m_fieldParamsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.WrapYKernel, BoingReactorField.ShaderPropertyId.ComputeFieldParams, this.m_fieldParamsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.WrapZKernel, BoingReactorField.ShaderPropertyId.ComputeFieldParams, this.m_fieldParamsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.ExecuteKernel, BoingReactorField.ShaderPropertyId.ComputeFieldParams, this.m_fieldParamsBuffer);
			}
			this.m_cellBufferNeedsReset = (this.m_cellsBuffer == null || this.m_cellsX != this.CellsX || this.m_cellsY != this.CellsY || this.m_cellsZ != this.CellsZ);
			if (this.m_cellBufferNeedsReset)
			{
				if (this.m_cellsBuffer != null)
				{
					this.m_cellsBuffer.Dispose();
				}
				int num = this.CellsX * this.CellsY * this.CellsZ;
				this.m_cellsBuffer = new ComputeBuffer(num, BoingWork.Params.InstanceData.Stride);
				BoingWork.Params.InstanceData[] array = new BoingWork.Params.InstanceData[num];
				for (int i = 0; i < num; i++)
				{
					array[i].PositionSpring.Reset();
					array[i].RotationSpring.Reset();
				}
				this.m_cellsBuffer.SetData(array);
				flag = true;
				this.m_cellsX = this.CellsX;
				this.m_cellsY = this.CellsY;
				this.m_cellsZ = this.CellsZ;
			}
			if (flag2 || this.m_cellBufferNeedsReset)
			{
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.InitKernel, BoingReactorField.ShaderPropertyId.ComputeCells, this.m_cellsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.MoveKernel, BoingReactorField.ShaderPropertyId.ComputeCells, this.m_cellsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.WrapXKernel, BoingReactorField.ShaderPropertyId.ComputeCells, this.m_cellsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.WrapYKernel, BoingReactorField.ShaderPropertyId.ComputeCells, this.m_cellsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.WrapZKernel, BoingReactorField.ShaderPropertyId.ComputeCells, this.m_cellsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.ExecuteKernel, BoingReactorField.ShaderPropertyId.ComputeCells, this.m_cellsBuffer);
			}
			if (flag)
			{
				this.m_gpuResourceSetId++;
				if (this.m_gpuResourceSetId < 0)
				{
					this.m_gpuResourceSetId = -1;
				}
			}
		}

		// Token: 0x06007D64 RID: 32100 RVA: 0x002922DC File Offset: 0x002904DC
		private void FinishPrepareExecuteCpu()
		{
			Quaternion rotation = base.transform.rotation;
			for (int i = 0; i < this.CellsZ; i++)
			{
				for (int j = 0; j < this.CellsY; j++)
				{
					for (int k = 0; k < this.CellsX; k++)
					{
						int x;
						int y;
						int z;
						this.ResolveCellIndex(k, j, i, -1, out x, out y, out z);
						this.m_aCpuCell[i, j, k].PrepareExecute(ref this.Params, this.m_gridCenter, rotation, this.GetCellCenterOffset(x, y, z));
					}
				}
			}
		}

		// Token: 0x06007D65 RID: 32101 RVA: 0x00292364 File Offset: 0x00290564
		private void FinishPrepareExecuteGpu()
		{
			if (this.m_cellBufferNeedsReset)
			{
				this.UpdateFieldParamsGpu();
				this.m_shader.Dispatch(BoingReactorField.s_computeKernelId.InitKernel, this.CellsX, this.CellsY, this.CellsZ);
			}
		}

		// Token: 0x06007D66 RID: 32102 RVA: 0x0029239B File Offset: 0x0029059B
		public void Init()
		{
			if (this.m_init)
			{
				return;
			}
			this.m_hardwareMode = this.HardwareMode;
			this.m_init = true;
		}

		// Token: 0x06007D67 RID: 32103 RVA: 0x002923B9 File Offset: 0x002905B9
		public void Sanitize()
		{
			if (this.PropagationDepth < 0)
			{
				Debug.LogWarning("Propagation iterations must be a positive number.");
			}
			else if (this.PropagationDepth > 3)
			{
				Debug.LogWarning("For performance reasons, propagation is limited to 3 iterations.");
			}
			this.PropagationDepth = Mathf.Clamp(this.PropagationDepth, 1, 3);
		}

		// Token: 0x06007D68 RID: 32104 RVA: 0x002923F8 File Offset: 0x002905F8
		public void HandleCellMove()
		{
			if (this.m_cellMoveMode != this.CellMoveMode)
			{
				this.Reboot();
				this.m_cellMoveMode = this.CellMoveMode;
			}
			BoingReactorField.CellMoveModeEnum cellMoveMode = this.CellMoveMode;
			BoingReactorField.HardwareModeEnum hardwareMode;
			if (cellMoveMode == BoingReactorField.CellMoveModeEnum.Follow)
			{
				Vector3 vector = base.transform.position - this.m_gridCenter;
				hardwareMode = this.HardwareMode;
				if (hardwareMode != BoingReactorField.HardwareModeEnum.CPU)
				{
					if (hardwareMode == BoingReactorField.HardwareModeEnum.GPU)
					{
						this.UpdateFieldParamsGpu();
						this.m_shader.SetVector(BoingReactorField.ShaderPropertyId.MoveParams, vector);
						this.m_shader.Dispatch(BoingReactorField.s_computeKernelId.MoveKernel, this.CellsX, this.CellsY, this.CellsZ);
					}
				}
				else
				{
					for (int i = 0; i < this.CellsZ; i++)
					{
						for (int j = 0; j < this.CellsY; j++)
						{
							for (int k = 0; k < this.CellsX; k++)
							{
								ref BoingWork.Params.InstanceData ptr = ref this.m_aCpuCell[i, j, k];
								ptr.PositionSpring.Value = ptr.PositionSpring.Value + vector;
							}
						}
					}
				}
				this.m_gridCenter = base.transform.position;
				this.m_qPrevGridCenterNorm = this.QuantizeNorm(this.m_gridCenter);
				return;
			}
			if (cellMoveMode != BoingReactorField.CellMoveModeEnum.WrapAround)
			{
				return;
			}
			this.m_gridCenter = base.transform.position;
			Vector3 vector2 = this.QuantizeNorm(this.m_gridCenter);
			this.m_gridCenter = vector2 * this.CellSize;
			int num = (int)(vector2.x - this.m_qPrevGridCenterNorm.x);
			int num2 = (int)(vector2.y - this.m_qPrevGridCenterNorm.y);
			int num3 = (int)(vector2.z - this.m_qPrevGridCenterNorm.z);
			this.m_qPrevGridCenterNorm = vector2;
			if (num == 0 && num2 == 0 && num3 == 0)
			{
				return;
			}
			hardwareMode = this.m_hardwareMode;
			if (hardwareMode != BoingReactorField.HardwareModeEnum.CPU)
			{
				if (hardwareMode == BoingReactorField.HardwareModeEnum.GPU)
				{
					this.WrapGpu(num, num2, num3);
				}
			}
			else
			{
				this.WrapCpu(num, num2, num3);
			}
			this.m_iCellBaseX = MathUtil.Modulo(this.m_iCellBaseX + num, this.CellsX);
			this.m_iCellBaseY = MathUtil.Modulo(this.m_iCellBaseY + num2, this.CellsY);
			this.m_iCellBaseZ = MathUtil.Modulo(this.m_iCellBaseZ + num3, this.CellsZ);
		}

		// Token: 0x06007D69 RID: 32105 RVA: 0x00292636 File Offset: 0x00290836
		private void InitPropagationCpu(ref BoingWork.Params.InstanceData data)
		{
			data.PositionPropagationWorkData = Vector3.zero;
			data.RotationPropagationWorkData = Vector3.zero;
		}

		// Token: 0x06007D6A RID: 32106 RVA: 0x00292654 File Offset: 0x00290854
		private void PropagateSpringCpu(ref BoingWork.Params.InstanceData data, float dt)
		{
			data.PositionSpring.Velocity = data.PositionSpring.Velocity + BoingReactorField.kPropagationFactor * this.PositionPropagation * data.PositionPropagationWorkData * dt;
			data.RotationSpring.VelocityVec = data.RotationSpring.VelocityVec + BoingReactorField.kPropagationFactor * this.RotationPropagation * data.RotationPropagationWorkData * dt;
		}

		// Token: 0x06007D6B RID: 32107 RVA: 0x002926D4 File Offset: 0x002908D4
		private void ExtendPropagationBorder(ref BoingWork.Params.InstanceData data, float weight, int adjDeltaX, int adjDeltaY, int adjDeltaZ)
		{
			data.PositionPropagationWorkData += weight * (data.PositionOrigin + new Vector3((float)adjDeltaX, (float)adjDeltaY, (float)adjDeltaZ) * this.CellSize);
			data.RotationPropagationWorkData += weight * data.RotationOrigin;
		}

		// Token: 0x06007D6C RID: 32108 RVA: 0x00292744 File Offset: 0x00290944
		private void AccumulatePropagationWeightedNeighbor(ref BoingWork.Params.InstanceData data, ref BoingWork.Params.InstanceData neighbor, float weight)
		{
			data.PositionPropagationWorkData += weight * (neighbor.PositionSpring.Value - neighbor.PositionOrigin);
			data.RotationPropagationWorkData += weight * (neighbor.RotationSpring.ValueVec - neighbor.RotationOrigin);
		}

		// Token: 0x06007D6D RID: 32109 RVA: 0x002927B8 File Offset: 0x002909B8
		private void GatherPropagation(ref BoingWork.Params.InstanceData data, float weightSum)
		{
			data.PositionPropagationWorkData = data.PositionPropagationWorkData / weightSum - (data.PositionSpring.Value - data.PositionOrigin);
			data.RotationPropagationWorkData = data.RotationPropagationWorkData / weightSum - (data.RotationSpring.ValueVec - data.RotationOrigin);
		}

		// Token: 0x06007D6E RID: 32110 RVA: 0x00292636 File Offset: 0x00290836
		private void AnchorPropagationBorder(ref BoingWork.Params.InstanceData data)
		{
			data.PositionPropagationWorkData = Vector3.zero;
			data.RotationPropagationWorkData = Vector3.zero;
		}

		// Token: 0x06007D6F RID: 32111 RVA: 0x00292820 File Offset: 0x00290A20
		private void PropagateCpu(float dt)
		{
			int[] array = new int[this.PropagationDepth * 2 + 1];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = i - this.PropagationDepth;
			}
			for (int j = 0; j < this.CellsZ; j++)
			{
				for (int k = 0; k < this.CellsY; k++)
				{
					for (int l = 0; l < this.CellsX; l++)
					{
						this.InitPropagationCpu(ref this.m_aCpuCell[j, k, l]);
					}
				}
			}
			for (int m = 0; m < this.CellsZ; m++)
			{
				for (int n = 0; n < this.CellsY; n++)
				{
					for (int num = 0; num < this.CellsX; num++)
					{
						int num2;
						int num3;
						int num4;
						this.ResolveCellIndex(num, n, m, -1, out num2, out num3, out num4);
						float num5 = 0f;
						foreach (int num7 in array)
						{
							foreach (int num9 in array)
							{
								foreach (int num11 in array)
								{
									if (num11 != 0 || num9 != 0 || num7 != 0)
									{
										int num12 = num11 * num11 + num9 * num9 + num7 * num7;
										float num13 = BoingReactorField.s_aSqrtInv[num12];
										num5 += num13;
										if ((this.CellsX <= 2 || ((num2 != 0 || num11 >= 0) && (num2 != this.CellsX - 1 || num11 <= 0))) && (this.CellsY <= 2 || ((num3 != 0 || num9 >= 0) && (num3 != this.CellsY - 1 || num9 <= 0))) && (this.CellsZ <= 2 || ((num4 != 0 || num7 >= 0) && (num4 != this.CellsZ - 1 || num7 <= 0))))
										{
											int num14 = MathUtil.Modulo(num + num11, this.CellsX);
											int num15 = MathUtil.Modulo(n + num9, this.CellsY);
											int num16 = MathUtil.Modulo(m + num7, this.CellsZ);
											this.AccumulatePropagationWeightedNeighbor(ref this.m_aCpuCell[m, n, num], ref this.m_aCpuCell[num16, num15, num14], num13);
										}
									}
								}
							}
						}
						if (num5 > 0f)
						{
							this.GatherPropagation(ref this.m_aCpuCell[m, n, num], num5);
						}
					}
				}
			}
			if (this.AnchorPropagationAtBorder)
			{
				for (int num17 = 0; num17 < this.CellsZ; num17++)
				{
					for (int num18 = 0; num18 < this.CellsY; num18++)
					{
						for (int num19 = 0; num19 < this.CellsX; num19++)
						{
							int num20;
							int num21;
							int num22;
							this.ResolveCellIndex(num19, num18, num17, -1, out num20, out num21, out num22);
							if (((num20 == 0 || num20 == this.CellsX - 1) && this.CellsX > 2) || ((num21 == 0 || num21 == this.CellsY - 1) && this.CellsY > 2) || ((num22 == 0 || num22 == this.CellsZ - 1) && this.CellsZ > 2))
							{
								this.AnchorPropagationBorder(ref this.m_aCpuCell[num17, num18, num19]);
							}
						}
					}
				}
			}
			for (int num23 = 0; num23 < this.CellsZ; num23++)
			{
				for (int num24 = 0; num24 < this.CellsY; num24++)
				{
					for (int num25 = 0; num25 < this.CellsX; num25++)
					{
						this.PropagateSpringCpu(ref this.m_aCpuCell[num23, num24, num25], dt);
					}
				}
			}
		}

		// Token: 0x06007D70 RID: 32112 RVA: 0x00292BD0 File Offset: 0x00290DD0
		private void WrapCpu(int deltaX, int deltaY, int deltaZ)
		{
			if (deltaX != 0)
			{
				int num = (deltaX > 0) ? -1 : 1;
				for (int i = 0; i < this.CellsZ; i++)
				{
					for (int j = 0; j < this.CellsY; j++)
					{
						int num2 = (deltaX > 0) ? (deltaX - 1) : (this.CellsX + deltaX);
						while (num2 >= 0 && num2 < this.CellsX)
						{
							int num3;
							int num4;
							int num5;
							this.ResolveCellIndex(num2, j, i, 1, out num3, out num4, out num5);
							int x;
							int y;
							int z;
							this.ResolveCellIndex(num3 - deltaX, num4 - deltaY, num5 - deltaZ, -1, out x, out y, out z);
							this.m_aCpuCell[num5, num4, num3].Reset(this.m_gridCenter + this.GetCellCenterOffset(x, y, z), true);
							num2 += num;
						}
					}
				}
			}
			if (deltaY != 0)
			{
				int num6 = (deltaY > 0) ? -1 : 1;
				for (int k = 0; k < this.CellsZ; k++)
				{
					int num7 = (deltaY > 0) ? (deltaY - 1) : (this.CellsY + deltaY);
					while (num7 >= 0 && num7 < this.CellsY)
					{
						for (int l = 0; l < this.CellsX; l++)
						{
							int num8;
							int num9;
							int num10;
							this.ResolveCellIndex(l, num7, k, 1, out num8, out num9, out num10);
							int x2;
							int y2;
							int z2;
							this.ResolveCellIndex(num8 - deltaX, num9 - deltaY, num10 - deltaZ, -1, out x2, out y2, out z2);
							this.m_aCpuCell[num10, num9, num8].Reset(this.m_gridCenter + this.GetCellCenterOffset(x2, y2, z2), true);
						}
						num7 += num6;
					}
				}
			}
			if (deltaZ != 0)
			{
				int num11 = (deltaZ > 0) ? -1 : 1;
				int num12 = (deltaZ > 0) ? (deltaZ - 1) : (this.CellsZ + deltaZ);
				while (num12 >= 0 && num12 < this.CellsZ)
				{
					for (int m = 0; m < this.CellsY; m++)
					{
						for (int n = 0; n < this.CellsX; n++)
						{
							int num13;
							int num14;
							int num15;
							this.ResolveCellIndex(n, m, num12, 1, out num13, out num14, out num15);
							int x3;
							int y3;
							int z3;
							this.ResolveCellIndex(num13 - deltaX, num14 - deltaY, num15 - deltaZ, -1, out x3, out y3, out z3);
							this.m_aCpuCell[num15, num14, num13].Reset(this.m_gridCenter + this.GetCellCenterOffset(x3, y3, z3), true);
						}
					}
					num12 += num11;
				}
			}
		}

		// Token: 0x06007D71 RID: 32113 RVA: 0x00292E24 File Offset: 0x00291024
		private void WrapGpu(int deltaX, int deltaY, int deltaZ)
		{
			this.UpdateFieldParamsGpu();
			this.m_shader.SetInts(BoingReactorField.ShaderPropertyId.WrapParams, new int[]
			{
				deltaX,
				deltaY,
				deltaZ
			});
			if (deltaX != 0)
			{
				this.m_shader.Dispatch(BoingReactorField.s_computeKernelId.WrapXKernel, 1, this.CellsY, this.CellsZ);
			}
			if (deltaY != 0)
			{
				this.m_shader.Dispatch(BoingReactorField.s_computeKernelId.WrapYKernel, this.CellsX, 1, this.CellsZ);
			}
			if (deltaZ != 0)
			{
				this.m_shader.Dispatch(BoingReactorField.s_computeKernelId.WrapZKernel, this.CellsX, this.CellsY, 1);
			}
		}

		// Token: 0x06007D72 RID: 32114 RVA: 0x00292ED0 File Offset: 0x002910D0
		public void ExecuteCpu(float dt)
		{
			this.PrepareExecute();
			if (this.Effectors == null || this.Effectors.Length == 0)
			{
				return;
			}
			if (this.EnablePropagation)
			{
				this.PropagateCpu(dt);
			}
			foreach (BoingEffector boingEffector in this.Effectors)
			{
				if (!(boingEffector == null))
				{
					BoingEffector.Params @params = default(BoingEffector.Params);
					@params.Fill(boingEffector);
					if (this.m_bounds.Intersects(ref @params))
					{
						for (int j = 0; j < this.CellsZ; j++)
						{
							for (int k = 0; k < this.CellsY; k++)
							{
								for (int l = 0; l < this.CellsX; l++)
								{
									this.m_aCpuCell[j, k, l].AccumulateTarget(ref this.Params, ref @params, dt);
								}
							}
						}
					}
				}
			}
			for (int m = 0; m < this.CellsZ; m++)
			{
				for (int n = 0; n < this.CellsY; n++)
				{
					for (int num = 0; num < this.CellsX; num++)
					{
						this.m_aCpuCell[m, n, num].EndAccumulateTargets(ref this.Params);
						this.m_aCpuCell[m, n, num].Execute(ref this.Params, dt);
					}
				}
			}
		}

		// Token: 0x06007D73 RID: 32115 RVA: 0x00293024 File Offset: 0x00291224
		public void ExecuteGpu(float dt, ComputeBuffer effectorParamsBuffer, Dictionary<int, int> effectorParamsIndexMap)
		{
			this.PrepareExecute();
			this.UpdateFieldParamsGpu();
			this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.ExecuteKernel, BoingReactorField.ShaderPropertyId.Effectors, effectorParamsBuffer);
			if (this.m_fieldParams.NumEffectors > 0)
			{
				int[] array = new int[this.m_fieldParams.NumEffectors];
				int num = 0;
				foreach (BoingEffector boingEffector in this.Effectors)
				{
					if (!(boingEffector == null))
					{
						BoingEffector component = boingEffector.GetComponent<BoingEffector>();
						int num2;
						if (!(component == null) && component.isActiveAndEnabled && effectorParamsIndexMap.TryGetValue(component.GetInstanceID(), out num2))
						{
							array[num++] = num2;
						}
					}
				}
				this.m_effectorIndexBuffer.SetData(array);
			}
			this.s_aReactorParams[0] = this.Params;
			this.m_reactorParamsBuffer.SetData(this.s_aReactorParams);
			this.m_shader.SetVector(BoingReactorField.ShaderPropertyId.PropagationParams, new Vector4(this.PositionPropagation, this.RotationPropagation, BoingReactorField.kPropagationFactor, 0f));
			this.m_shader.Dispatch(BoingReactorField.s_computeKernelId.ExecuteKernel, this.CellsX, this.CellsY, this.CellsZ);
		}

		// Token: 0x06007D74 RID: 32116 RVA: 0x0029315E File Offset: 0x0029135E
		public void OnDrawGizmosSelected()
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			this.DrawGizmos(true);
		}

		// Token: 0x06007D75 RID: 32117 RVA: 0x00293170 File Offset: 0x00291370
		private void DrawGizmos(bool drawEffectors)
		{
			Vector3 vector = this.GetGridCenter();
			BoingReactorField.CellMoveModeEnum cellMoveMode = this.CellMoveMode;
			if (cellMoveMode != BoingReactorField.CellMoveModeEnum.Follow)
			{
				if (cellMoveMode == BoingReactorField.CellMoveModeEnum.WrapAround)
				{
					vector = new Vector3(Mathf.Round(base.transform.position.x / this.CellSize), Mathf.Round(base.transform.position.y / this.CellSize), Mathf.Round(base.transform.position.z / this.CellSize)) * this.CellSize;
				}
			}
			else
			{
				vector = base.transform.position;
			}
			BoingWork.Params.InstanceData[,,] array = null;
			BoingReactorField.HardwareModeEnum hardwareMode = this.HardwareMode;
			if (hardwareMode != BoingReactorField.HardwareModeEnum.CPU)
			{
				if (hardwareMode == BoingReactorField.HardwareModeEnum.GPU)
				{
					if (this.m_cellsBuffer != null)
					{
						array = new BoingWork.Params.InstanceData[this.CellsZ, this.CellsY, this.CellsX];
						this.m_cellsBuffer.GetData(array);
					}
				}
			}
			else
			{
				array = this.m_aCpuCell;
			}
			int num = 1;
			if (this.CellsX * this.CellsY * this.CellsZ > 1024)
			{
				num = 2;
			}
			if (this.CellsX * this.CellsY * this.CellsZ > 4096)
			{
				num = 3;
			}
			if (this.CellsX * this.CellsY * this.CellsZ > 8192)
			{
				num = 4;
			}
			for (int i = 0; i < this.CellsZ; i++)
			{
				for (int j = 0; j < this.CellsY; j++)
				{
					for (int k = 0; k < this.CellsX; k++)
					{
						int x;
						int y;
						int z;
						this.ResolveCellIndex(k, j, i, -1, out x, out y, out z);
						Vector3 center = vector + this.GetCellCenterOffset(x, y, z);
						if (array != null && k % num == 0 && j % num == 0 && i % num == 0)
						{
							BoingWork.Params.InstanceData instanceData = array[i, j, k];
							Gizmos.color = new Color(1f, 1f, 1f, 1f);
							Gizmos.matrix = Matrix4x4.TRS(instanceData.PositionSpring.Value, instanceData.RotationSpring.ValueQuat, Vector3.one);
							Gizmos.DrawCube(Vector3.zero, Mathf.Min(0.1f, 0.5f * this.CellSize) * Vector3.one);
							Gizmos.matrix = Matrix4x4.identity;
						}
						Gizmos.color = new Color(1f, 0.5f, 0.2f, 1f);
						Gizmos.DrawWireCube(center, this.CellSize * Vector3.one);
					}
				}
			}
			BoingReactorField.FalloffModeEnum falloffMode = this.FalloffMode;
			if (falloffMode != BoingReactorField.FalloffModeEnum.Circle)
			{
				if (falloffMode == BoingReactorField.FalloffModeEnum.Square)
				{
					Vector3 size = this.CellSize * this.FalloffRatio * new Vector3((float)this.CellsX, (float)this.CellsY, (float)this.CellsZ);
					Gizmos.color = new Color(1f, 1f, 0.2f, 0.5f);
					Gizmos.DrawWireCube(vector, size);
				}
			}
			else
			{
				float num2 = (float)Mathf.Max(this.CellsX, Mathf.Max(this.CellsY, this.CellsZ));
				Gizmos.color = new Color(1f, 1f, 0.2f, 0.5f);
				Gizmos.matrix = Matrix4x4.Translate(vector) * Matrix4x4.Scale(new Vector3((float)this.CellsX, (float)this.CellsY, (float)this.CellsZ) / num2);
				Gizmos.DrawWireSphere(Vector3.zero, 0.5f * this.CellSize * num2 * this.FalloffRatio);
				Gizmos.matrix = Matrix4x4.identity;
			}
			if (drawEffectors && this.Effectors != null)
			{
				foreach (BoingEffector boingEffector in this.Effectors)
				{
					if (!(boingEffector == null))
					{
						boingEffector.OnDrawGizmosSelected();
					}
				}
			}
		}

		// Token: 0x06007D76 RID: 32118 RVA: 0x00293548 File Offset: 0x00291748
		private Vector3 GetGridCenter()
		{
			BoingReactorField.CellMoveModeEnum cellMoveMode = this.CellMoveMode;
			if (cellMoveMode == BoingReactorField.CellMoveModeEnum.Follow)
			{
				return base.transform.position;
			}
			if (cellMoveMode != BoingReactorField.CellMoveModeEnum.WrapAround)
			{
				return base.transform.position;
			}
			return this.QuantizeNorm(base.transform.position) * this.CellSize;
		}

		// Token: 0x06007D77 RID: 32119 RVA: 0x00293599 File Offset: 0x00291799
		private Vector3 QuantizeNorm(Vector3 p)
		{
			return new Vector3(Mathf.Round(p.x / this.CellSize), Mathf.Round(p.y / this.CellSize), Mathf.Round(p.z / this.CellSize));
		}

		// Token: 0x06007D78 RID: 32120 RVA: 0x002935D8 File Offset: 0x002917D8
		private Vector3 GetCellCenterOffset(int x, int y, int z)
		{
			return this.CellSize * (-0.5f * (new Vector3((float)this.CellsX, (float)this.CellsY, (float)this.CellsZ) - Vector3.one) + new Vector3((float)x, (float)y, (float)z));
		}

		// Token: 0x06007D79 RID: 32121 RVA: 0x00293630 File Offset: 0x00291830
		private void ResolveCellIndex(int x, int y, int z, int baseMult, out int resX, out int resY, out int resZ)
		{
			resX = MathUtil.Modulo(x + baseMult * this.m_iCellBaseX, this.CellsX);
			resY = MathUtil.Modulo(y + baseMult * this.m_iCellBaseY, this.CellsY);
			resZ = MathUtil.Modulo(z + baseMult * this.m_iCellBaseZ, this.CellsZ);
		}

		// Token: 0x04008E7A RID: 36474
		private static BoingReactorField.ShaderPropertyIdSet s_shaderPropertyId;

		// Token: 0x04008E7B RID: 36475
		private BoingReactorField.FieldParams m_fieldParams;

		// Token: 0x04008E7C RID: 36476
		public BoingReactorField.HardwareModeEnum HardwareMode = BoingReactorField.HardwareModeEnum.GPU;

		// Token: 0x04008E7D RID: 36477
		private BoingReactorField.HardwareModeEnum m_hardwareMode;

		// Token: 0x04008E7E RID: 36478
		public BoingReactorField.CellMoveModeEnum CellMoveMode = BoingReactorField.CellMoveModeEnum.WrapAround;

		// Token: 0x04008E7F RID: 36479
		private BoingReactorField.CellMoveModeEnum m_cellMoveMode;

		// Token: 0x04008E80 RID: 36480
		[Range(0.1f, 10f)]
		public float CellSize = 1f;

		// Token: 0x04008E81 RID: 36481
		public int CellsX = 8;

		// Token: 0x04008E82 RID: 36482
		public int CellsY = 1;

		// Token: 0x04008E83 RID: 36483
		public int CellsZ = 8;

		// Token: 0x04008E84 RID: 36484
		private int m_cellsX = -1;

		// Token: 0x04008E85 RID: 36485
		private int m_cellsY = -1;

		// Token: 0x04008E86 RID: 36486
		private int m_cellsZ = -1;

		// Token: 0x04008E87 RID: 36487
		private int m_iCellBaseX;

		// Token: 0x04008E88 RID: 36488
		private int m_iCellBaseY;

		// Token: 0x04008E89 RID: 36489
		private int m_iCellBaseZ;

		// Token: 0x04008E8A RID: 36490
		public BoingReactorField.FalloffModeEnum FalloffMode = BoingReactorField.FalloffModeEnum.Square;

		// Token: 0x04008E8B RID: 36491
		[Range(0f, 1f)]
		public float FalloffRatio = 0.7f;

		// Token: 0x04008E8C RID: 36492
		public BoingReactorField.FalloffDimensionsEnum FalloffDimensions = BoingReactorField.FalloffDimensionsEnum.XZ;

		// Token: 0x04008E8D RID: 36493
		public BoingEffector[] Effectors = new BoingEffector[1];

		// Token: 0x04008E8E RID: 36494
		private int m_numEffectors = -1;

		// Token: 0x04008E8F RID: 36495
		private Aabb m_bounds;

		// Token: 0x04008E90 RID: 36496
		public bool TwoDDistanceCheck;

		// Token: 0x04008E91 RID: 36497
		public bool TwoDPositionInfluence;

		// Token: 0x04008E92 RID: 36498
		public bool TwoDRotationInfluence;

		// Token: 0x04008E93 RID: 36499
		public bool EnablePositionEffect = true;

		// Token: 0x04008E94 RID: 36500
		public bool EnableRotationEffect = true;

		// Token: 0x04008E95 RID: 36501
		public bool GlobalReactionUpVector;

		// Token: 0x04008E96 RID: 36502
		public BoingWork.Params Params;

		// Token: 0x04008E97 RID: 36503
		public SharedBoingParams SharedParams;

		// Token: 0x04008E98 RID: 36504
		public bool EnablePropagation;

		// Token: 0x04008E99 RID: 36505
		[Range(0f, 1f)]
		public float PositionPropagation = 1f;

		// Token: 0x04008E9A RID: 36506
		[Range(0f, 1f)]
		public float RotationPropagation = 1f;

		// Token: 0x04008E9B RID: 36507
		[Range(1f, 3f)]
		public int PropagationDepth = 1;

		// Token: 0x04008E9C RID: 36508
		public bool AnchorPropagationAtBorder;

		// Token: 0x04008E9D RID: 36509
		private static readonly float kPropagationFactor = 600f;

		// Token: 0x04008E9E RID: 36510
		private BoingWork.Params.InstanceData[,,] m_aCpuCell;

		// Token: 0x04008E9F RID: 36511
		private ComputeShader m_shader;

		// Token: 0x04008EA0 RID: 36512
		private ComputeBuffer m_effectorIndexBuffer;

		// Token: 0x04008EA1 RID: 36513
		private ComputeBuffer m_reactorParamsBuffer;

		// Token: 0x04008EA2 RID: 36514
		private ComputeBuffer m_fieldParamsBuffer;

		// Token: 0x04008EA3 RID: 36515
		private ComputeBuffer m_cellsBuffer;

		// Token: 0x04008EA4 RID: 36516
		private int m_gpuResourceSetId = -1;

		// Token: 0x04008EA5 RID: 36517
		private static BoingReactorField.ComputeKernelId s_computeKernelId;

		// Token: 0x04008EA6 RID: 36518
		private bool m_init;

		// Token: 0x04008EA7 RID: 36519
		private Vector3 m_gridCenter;

		// Token: 0x04008EA8 RID: 36520
		private Vector3 m_qPrevGridCenterNorm;

		// Token: 0x04008EA9 RID: 36521
		private static Vector3[] s_aCellOffset = new Vector3[8];

		// Token: 0x04008EAA RID: 36522
		private bool m_cellBufferNeedsReset;

		// Token: 0x04008EAB RID: 36523
		private static float[] s_aSqrtInv = new float[]
		{
			0f,
			1f,
			0.70711f,
			0.57735f,
			0.5f,
			0.44721f,
			0.40825f,
			0.37796f,
			0.35355f,
			0.33333f,
			0.31623f,
			0.30151f,
			0.28868f,
			0.27735f,
			0.26726f,
			0.2582f,
			0.25f,
			0.24254f,
			0.2357f,
			0.22942f,
			0.22361f,
			0.21822f,
			0.2132f,
			0.20851f,
			0.20412f,
			0.2f,
			0.19612f,
			0.19245f
		};

		// Token: 0x04008EAC RID: 36524
		private BoingWork.Params[] s_aReactorParams = new BoingWork.Params[1];

		// Token: 0x02001372 RID: 4978
		public enum HardwareModeEnum
		{
			// Token: 0x04008EAE RID: 36526
			CPU,
			// Token: 0x04008EAF RID: 36527
			GPU
		}

		// Token: 0x02001373 RID: 4979
		public enum CellMoveModeEnum
		{
			// Token: 0x04008EB1 RID: 36529
			Follow,
			// Token: 0x04008EB2 RID: 36530
			WrapAround
		}

		// Token: 0x02001374 RID: 4980
		public enum FalloffModeEnum
		{
			// Token: 0x04008EB4 RID: 36532
			None,
			// Token: 0x04008EB5 RID: 36533
			Circle,
			// Token: 0x04008EB6 RID: 36534
			Square
		}

		// Token: 0x02001375 RID: 4981
		public enum FalloffDimensionsEnum
		{
			// Token: 0x04008EB8 RID: 36536
			XYZ,
			// Token: 0x04008EB9 RID: 36537
			XY,
			// Token: 0x04008EBA RID: 36538
			XZ,
			// Token: 0x04008EBB RID: 36539
			YZ
		}

		// Token: 0x02001376 RID: 4982
		public class ShaderPropertyIdSet
		{
			// Token: 0x06007D7B RID: 32123 RVA: 0x002936B8 File Offset: 0x002918B8
			public ShaderPropertyIdSet()
			{
				this.MoveParams = Shader.PropertyToID("moveParams");
				this.WrapParams = Shader.PropertyToID("wrapParams");
				this.Effectors = Shader.PropertyToID("aEffector");
				this.EffectorIndices = Shader.PropertyToID("aEffectorIndex");
				this.ReactorParams = Shader.PropertyToID("reactorParams");
				this.ComputeFieldParams = Shader.PropertyToID("fieldParams");
				this.ComputeCells = Shader.PropertyToID("aCell");
				this.RenderFieldParams = Shader.PropertyToID("aBoingFieldParams");
				this.RenderCells = Shader.PropertyToID("aBoingFieldCell");
				this.PositionSampleMultiplier = Shader.PropertyToID("positionSampleMultiplier");
				this.RotationSampleMultiplier = Shader.PropertyToID("rotationSampleMultiplier");
				this.PropagationParams = Shader.PropertyToID("propagationParams");
			}

			// Token: 0x04008EBC RID: 36540
			public int MoveParams;

			// Token: 0x04008EBD RID: 36541
			public int WrapParams;

			// Token: 0x04008EBE RID: 36542
			public int Effectors;

			// Token: 0x04008EBF RID: 36543
			public int EffectorIndices;

			// Token: 0x04008EC0 RID: 36544
			public int ReactorParams;

			// Token: 0x04008EC1 RID: 36545
			public int ComputeFieldParams;

			// Token: 0x04008EC2 RID: 36546
			public int ComputeCells;

			// Token: 0x04008EC3 RID: 36547
			public int RenderFieldParams;

			// Token: 0x04008EC4 RID: 36548
			public int RenderCells;

			// Token: 0x04008EC5 RID: 36549
			public int PositionSampleMultiplier;

			// Token: 0x04008EC6 RID: 36550
			public int RotationSampleMultiplier;

			// Token: 0x04008EC7 RID: 36551
			public int PropagationParams;
		}

		// Token: 0x02001377 RID: 4983
		private struct FieldParams
		{
			// Token: 0x06007D7C RID: 32124 RVA: 0x0029378C File Offset: 0x0029198C
			private void SuppressWarnings()
			{
				this.m_padding0 = 0;
				this.m_padding1 = 0;
				this.m_padding2 = 0f;
				this.m_padding4 = 0f;
				this.m_padding5 = 0f;
				this.m_padding0 = this.m_padding1;
				this.m_padding1 = (int)this.m_padding2;
				this.m_padding2 = this.m_padding3;
				this.m_padding3 = this.m_padding4;
				this.m_padding4 = this.m_padding5;
			}

			// Token: 0x04008EC8 RID: 36552
			public static readonly int Stride = 112;

			// Token: 0x04008EC9 RID: 36553
			public int CellsX;

			// Token: 0x04008ECA RID: 36554
			public int CellsY;

			// Token: 0x04008ECB RID: 36555
			public int CellsZ;

			// Token: 0x04008ECC RID: 36556
			public int NumEffectors;

			// Token: 0x04008ECD RID: 36557
			public int iCellBaseX;

			// Token: 0x04008ECE RID: 36558
			public int iCellBaseY;

			// Token: 0x04008ECF RID: 36559
			public int iCellBaseZ;

			// Token: 0x04008ED0 RID: 36560
			public int m_padding0;

			// Token: 0x04008ED1 RID: 36561
			public int FalloffMode;

			// Token: 0x04008ED2 RID: 36562
			public int FalloffDimensions;

			// Token: 0x04008ED3 RID: 36563
			public int PropagationDepth;

			// Token: 0x04008ED4 RID: 36564
			public int m_padding1;

			// Token: 0x04008ED5 RID: 36565
			public Vector3 GridCenter;

			// Token: 0x04008ED6 RID: 36566
			private float m_padding3;

			// Token: 0x04008ED7 RID: 36567
			public Vector3 UpWs;

			// Token: 0x04008ED8 RID: 36568
			private float m_padding2;

			// Token: 0x04008ED9 RID: 36569
			public Vector3 FieldPosition;

			// Token: 0x04008EDA RID: 36570
			public float m_padding4;

			// Token: 0x04008EDB RID: 36571
			public float FalloffRatio;

			// Token: 0x04008EDC RID: 36572
			public float CellSize;

			// Token: 0x04008EDD RID: 36573
			public float DeltaTime;

			// Token: 0x04008EDE RID: 36574
			private float m_padding5;
		}

		// Token: 0x02001378 RID: 4984
		private class ComputeKernelId
		{
			// Token: 0x04008EDF RID: 36575
			public int InitKernel;

			// Token: 0x04008EE0 RID: 36576
			public int MoveKernel;

			// Token: 0x04008EE1 RID: 36577
			public int WrapXKernel;

			// Token: 0x04008EE2 RID: 36578
			public int WrapYKernel;

			// Token: 0x04008EE3 RID: 36579
			public int WrapZKernel;

			// Token: 0x04008EE4 RID: 36580
			public int ExecuteKernel;
		}
	}
}
