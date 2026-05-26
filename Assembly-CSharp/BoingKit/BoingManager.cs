using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200135D RID: 4957
	public static class BoingManager
	{
		// Token: 0x17000BD9 RID: 3033
		// (get) Token: 0x06007CDA RID: 31962 RVA: 0x00290034 File Offset: 0x0028E234
		public static IEnumerable<BoingBehavior> Behaviors
		{
			get
			{
				return BoingManager.s_behaviorMap.Values;
			}
		}

		// Token: 0x17000BDA RID: 3034
		// (get) Token: 0x06007CDB RID: 31963 RVA: 0x00290040 File Offset: 0x0028E240
		public static IEnumerable<BoingReactor> Reactors
		{
			get
			{
				return BoingManager.s_reactorMap.Values;
			}
		}

		// Token: 0x17000BDB RID: 3035
		// (get) Token: 0x06007CDC RID: 31964 RVA: 0x0029004C File Offset: 0x0028E24C
		public static IEnumerable<BoingEffector> Effectors
		{
			get
			{
				return BoingManager.s_effectorMap.Values;
			}
		}

		// Token: 0x17000BDC RID: 3036
		// (get) Token: 0x06007CDD RID: 31965 RVA: 0x00290058 File Offset: 0x0028E258
		public static IEnumerable<BoingReactorField> ReactorFields
		{
			get
			{
				return BoingManager.s_fieldMap.Values;
			}
		}

		// Token: 0x17000BDD RID: 3037
		// (get) Token: 0x06007CDE RID: 31966 RVA: 0x00290064 File Offset: 0x0028E264
		public static IEnumerable<BoingReactorFieldCPUSampler> ReactorFieldCPUSamlers
		{
			get
			{
				return BoingManager.s_cpuSamplerMap.Values;
			}
		}

		// Token: 0x17000BDE RID: 3038
		// (get) Token: 0x06007CDF RID: 31967 RVA: 0x00290070 File Offset: 0x0028E270
		public static IEnumerable<BoingReactorFieldGPUSampler> ReactorFieldGPUSampler
		{
			get
			{
				return BoingManager.s_gpuSamplerMap.Values;
			}
		}

		// Token: 0x17000BDF RID: 3039
		// (get) Token: 0x06007CE0 RID: 31968 RVA: 0x0029007C File Offset: 0x0028E27C
		public static float DeltaTime
		{
			get
			{
				return BoingManager.s_deltaTime;
			}
		}

		// Token: 0x17000BE0 RID: 3040
		// (get) Token: 0x06007CE1 RID: 31969 RVA: 0x00290083 File Offset: 0x0028E283
		public static float FixedDeltaTime
		{
			get
			{
				return Time.fixedDeltaTime;
			}
		}

		// Token: 0x17000BE1 RID: 3041
		// (get) Token: 0x06007CE2 RID: 31970 RVA: 0x0029008A File Offset: 0x0028E28A
		internal static int NumBehaviors
		{
			get
			{
				return BoingManager.s_behaviorMap.Count;
			}
		}

		// Token: 0x17000BE2 RID: 3042
		// (get) Token: 0x06007CE3 RID: 31971 RVA: 0x00290096 File Offset: 0x0028E296
		internal static int NumEffectors
		{
			get
			{
				return BoingManager.s_effectorMap.Count;
			}
		}

		// Token: 0x17000BE3 RID: 3043
		// (get) Token: 0x06007CE4 RID: 31972 RVA: 0x002900A2 File Offset: 0x0028E2A2
		internal static int NumReactors
		{
			get
			{
				return BoingManager.s_reactorMap.Count;
			}
		}

		// Token: 0x17000BE4 RID: 3044
		// (get) Token: 0x06007CE5 RID: 31973 RVA: 0x002900AE File Offset: 0x0028E2AE
		internal static int NumFields
		{
			get
			{
				return BoingManager.s_fieldMap.Count;
			}
		}

		// Token: 0x17000BE5 RID: 3045
		// (get) Token: 0x06007CE6 RID: 31974 RVA: 0x002900BA File Offset: 0x0028E2BA
		internal static int NumCPUFieldSamplers
		{
			get
			{
				return BoingManager.s_cpuSamplerMap.Count;
			}
		}

		// Token: 0x17000BE6 RID: 3046
		// (get) Token: 0x06007CE7 RID: 31975 RVA: 0x002900C6 File Offset: 0x0028E2C6
		internal static int NumGPUFieldSamplers
		{
			get
			{
				return BoingManager.s_gpuSamplerMap.Count;
			}
		}

		// Token: 0x06007CE8 RID: 31976 RVA: 0x002900D4 File Offset: 0x0028E2D4
		private static void ValidateManager()
		{
			if (BoingManager.s_managerGo != null)
			{
				return;
			}
			BoingManager.s_managerGo = new GameObject("Boing Kit manager (don't delete)");
			BoingManager.s_managerGo.AddComponent<BoingManagerPreUpdatePump>();
			BoingManager.s_managerGo.AddComponent<BoingManagerPostUpdatePump>();
			Object.DontDestroyOnLoad(BoingManager.s_managerGo);
			BoingManager.s_managerGo.AddComponent<SphereCollider>().enabled = false;
		}

		// Token: 0x17000BE7 RID: 3047
		// (get) Token: 0x06007CE9 RID: 31977 RVA: 0x0029012E File Offset: 0x0028E32E
		internal static SphereCollider SharedSphereCollider
		{
			get
			{
				if (BoingManager.s_managerGo == null)
				{
					return null;
				}
				return BoingManager.s_managerGo.GetComponent<SphereCollider>();
			}
		}

		// Token: 0x06007CEA RID: 31978 RVA: 0x00290149 File Offset: 0x0028E349
		internal static void Register(BoingBehavior behavior)
		{
			BoingManager.PreRegisterBehavior();
			BoingManager.s_behaviorMap.Add(behavior.GetInstanceID(), behavior);
			if (BoingManager.OnBehaviorRegister != null)
			{
				BoingManager.OnBehaviorRegister(behavior);
			}
		}

		// Token: 0x06007CEB RID: 31979 RVA: 0x00290173 File Offset: 0x0028E373
		internal static void Unregister(BoingBehavior behavior)
		{
			if (BoingManager.OnBehaviorUnregister != null)
			{
				BoingManager.OnBehaviorUnregister(behavior);
			}
			BoingManager.s_behaviorMap.Remove(behavior.GetInstanceID());
			BoingManager.PostUnregisterBehavior();
		}

		// Token: 0x06007CEC RID: 31980 RVA: 0x0029019D File Offset: 0x0028E39D
		internal static void Register(BoingEffector effector)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_effectorMap.Add(effector.GetInstanceID(), effector);
			if (BoingManager.OnEffectorRegister != null)
			{
				BoingManager.OnEffectorRegister(effector);
			}
		}

		// Token: 0x06007CED RID: 31981 RVA: 0x002901C7 File Offset: 0x0028E3C7
		internal static void Unregister(BoingEffector effector)
		{
			if (BoingManager.OnEffectorUnregister != null)
			{
				BoingManager.OnEffectorUnregister(effector);
			}
			BoingManager.s_effectorMap.Remove(effector.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06007CEE RID: 31982 RVA: 0x002901F1 File Offset: 0x0028E3F1
		internal static void Register(BoingReactor reactor)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_reactorMap.Add(reactor.GetInstanceID(), reactor);
			if (BoingManager.OnReactorRegister != null)
			{
				BoingManager.OnReactorRegister(reactor);
			}
		}

		// Token: 0x06007CEF RID: 31983 RVA: 0x0029021B File Offset: 0x0028E41B
		internal static void Unregister(BoingReactor reactor)
		{
			if (BoingManager.OnReactorUnregister != null)
			{
				BoingManager.OnReactorUnregister(reactor);
			}
			BoingManager.s_reactorMap.Remove(reactor.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06007CF0 RID: 31984 RVA: 0x00290245 File Offset: 0x0028E445
		internal static void Register(BoingReactorField field)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_fieldMap.Add(field.GetInstanceID(), field);
			if (BoingManager.OnReactorFieldRegister != null)
			{
				BoingManager.OnReactorFieldRegister(field);
			}
		}

		// Token: 0x06007CF1 RID: 31985 RVA: 0x0029026F File Offset: 0x0028E46F
		internal static void Unregister(BoingReactorField field)
		{
			if (BoingManager.OnReactorFieldUnregister != null)
			{
				BoingManager.OnReactorFieldUnregister(field);
			}
			BoingManager.s_fieldMap.Remove(field.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06007CF2 RID: 31986 RVA: 0x00290299 File Offset: 0x0028E499
		internal static void Register(BoingReactorFieldCPUSampler sampler)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_cpuSamplerMap.Add(sampler.GetInstanceID(), sampler);
			if (BoingManager.OnReactorFieldCPUSamplerRegister != null)
			{
				BoingManager.OnReactorFieldCPUSamplerUnregister(sampler);
			}
		}

		// Token: 0x06007CF3 RID: 31987 RVA: 0x002902C3 File Offset: 0x0028E4C3
		internal static void Unregister(BoingReactorFieldCPUSampler sampler)
		{
			if (BoingManager.OnReactorFieldCPUSamplerUnregister != null)
			{
				BoingManager.OnReactorFieldCPUSamplerUnregister(sampler);
			}
			BoingManager.s_cpuSamplerMap.Remove(sampler.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06007CF4 RID: 31988 RVA: 0x002902ED File Offset: 0x0028E4ED
		internal static void Register(BoingReactorFieldGPUSampler sampler)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_gpuSamplerMap.Add(sampler.GetInstanceID(), sampler);
			if (BoingManager.OnReactorFieldGPUSamplerRegister != null)
			{
				BoingManager.OnReactorFieldGPUSamplerRegister(sampler);
			}
		}

		// Token: 0x06007CF5 RID: 31989 RVA: 0x00290317 File Offset: 0x0028E517
		internal static void Unregister(BoingReactorFieldGPUSampler sampler)
		{
			if (BoingManager.OnFieldGPUSamplerUnregister != null)
			{
				BoingManager.OnFieldGPUSamplerUnregister(sampler);
			}
			BoingManager.s_gpuSamplerMap.Remove(sampler.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06007CF6 RID: 31990 RVA: 0x00290341 File Offset: 0x0028E541
		internal static void Register(BoingBones bones)
		{
			BoingManager.PreRegisterBones();
			BoingManager.s_bonesMap.Add(bones.GetInstanceID(), bones);
			if (BoingManager.OnBonesRegister != null)
			{
				BoingManager.OnBonesRegister(bones);
			}
		}

		// Token: 0x06007CF7 RID: 31991 RVA: 0x0029036B File Offset: 0x0028E56B
		internal static void Unregister(BoingBones bones)
		{
			if (BoingManager.OnBonesUnregister != null)
			{
				BoingManager.OnBonesUnregister(bones);
			}
			BoingManager.s_bonesMap.Remove(bones.GetInstanceID());
			BoingManager.PostUnregisterBones();
		}

		// Token: 0x06007CF8 RID: 31992 RVA: 0x00290395 File Offset: 0x0028E595
		private static void PreRegisterBehavior()
		{
			BoingManager.ValidateManager();
		}

		// Token: 0x06007CF9 RID: 31993 RVA: 0x0029039C File Offset: 0x0028E59C
		private static void PostUnregisterBehavior()
		{
			if (BoingManager.s_behaviorMap.Count > 0)
			{
				return;
			}
			BoingWorkAsynchronous.PostUnregisterBehaviorCleanUp();
		}

		// Token: 0x06007CFA RID: 31994 RVA: 0x002903B4 File Offset: 0x0028E5B4
		private static void PreRegisterEffectorReactor()
		{
			BoingManager.ValidateManager();
			if (BoingManager.s_effectorParamsBuffer == null)
			{
				BoingManager.s_effectorParamsList = new List<BoingEffector.Params>(BoingManager.kEffectorParamsIncrement);
				BoingManager.s_effectorParamsBuffer = new ComputeBuffer(BoingManager.s_effectorParamsList.Capacity, BoingEffector.Params.Stride);
			}
			if (BoingManager.s_effectorMap.Count >= BoingManager.s_effectorParamsList.Capacity)
			{
				BoingManager.s_effectorParamsList.Capacity += BoingManager.kEffectorParamsIncrement;
				BoingManager.s_effectorParamsBuffer.Dispose();
				BoingManager.s_effectorParamsBuffer = new ComputeBuffer(BoingManager.s_effectorParamsList.Capacity, BoingEffector.Params.Stride);
			}
		}

		// Token: 0x06007CFB RID: 31995 RVA: 0x00290444 File Offset: 0x0028E644
		private static void PostUnregisterEffectorReactor()
		{
			if (BoingManager.s_effectorMap.Count > 0 || BoingManager.s_reactorMap.Count > 0 || BoingManager.s_fieldMap.Count > 0 || BoingManager.s_cpuSamplerMap.Count > 0 || BoingManager.s_gpuSamplerMap.Count > 0)
			{
				return;
			}
			BoingManager.s_effectorParamsList = null;
			BoingManager.s_effectorParamsBuffer.Dispose();
			BoingManager.s_effectorParamsBuffer = null;
			BoingWorkAsynchronous.PostUnregisterEffectorReactorCleanUp();
		}

		// Token: 0x06007CFC RID: 31996 RVA: 0x00290395 File Offset: 0x0028E595
		private static void PreRegisterBones()
		{
			BoingManager.ValidateManager();
		}

		// Token: 0x06007CFD RID: 31997 RVA: 0x000028C5 File Offset: 0x00000AC5
		private static void PostUnregisterBones()
		{
		}

		// Token: 0x06007CFE RID: 31998 RVA: 0x002904AE File Offset: 0x0028E6AE
		internal static void Execute(BoingManager.UpdateMode updateMode)
		{
			if (updateMode == BoingManager.UpdateMode.EarlyUpdate)
			{
				BoingManager.s_deltaTime = Time.deltaTime;
			}
			BoingManager.RefreshEffectorParams();
			BoingManager.ExecuteBones(updateMode);
			BoingManager.ExecuteBehaviors(updateMode);
			BoingManager.ExecuteReactors(updateMode);
		}

		// Token: 0x06007CFF RID: 31999 RVA: 0x002904D8 File Offset: 0x0028E6D8
		internal static void ExecuteBehaviors(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_behaviorMap.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in BoingManager.s_behaviorMap)
			{
				BoingBehavior value = keyValuePair.Value;
				if (!value.InitRebooted)
				{
					value.Reboot();
					value.InitRebooted = true;
				}
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.ExecuteBehaviors(BoingManager.s_behaviorMap, updateMode);
				return;
			}
			BoingWorkSynchronous.ExecuteBehaviors(BoingManager.s_behaviorMap, updateMode);
		}

		// Token: 0x06007D00 RID: 32000 RVA: 0x0029056C File Offset: 0x0028E76C
		internal static void PullBehaviorResults(BoingManager.UpdateMode updateMode)
		{
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in BoingManager.s_behaviorMap)
			{
				if (keyValuePair.Value.UpdateMode == updateMode)
				{
					keyValuePair.Value.PullResults();
				}
			}
		}

		// Token: 0x06007D01 RID: 32001 RVA: 0x002905D4 File Offset: 0x0028E7D4
		internal static void RestoreBehaviors()
		{
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in BoingManager.s_behaviorMap)
			{
				keyValuePair.Value.Restore();
			}
		}

		// Token: 0x06007D02 RID: 32002 RVA: 0x0029062C File Offset: 0x0028E82C
		internal static void RefreshEffectorParams()
		{
			if (BoingManager.s_effectorParamsList == null)
			{
				return;
			}
			BoingManager.s_effectorParamsIndexMap.Clear();
			BoingManager.s_effectorParamsList.Clear();
			foreach (KeyValuePair<int, BoingEffector> keyValuePair in BoingManager.s_effectorMap)
			{
				BoingEffector value = keyValuePair.Value;
				BoingManager.s_effectorParamsIndexMap.Add(value.GetInstanceID(), BoingManager.s_effectorParamsList.Count);
				BoingManager.s_effectorParamsList.Add(new BoingEffector.Params(value));
			}
			if (BoingManager.s_aEffectorParams == null || BoingManager.s_aEffectorParams.Length != BoingManager.s_effectorParamsList.Count)
			{
				BoingManager.s_aEffectorParams = BoingManager.s_effectorParamsList.ToArray();
				return;
			}
			BoingManager.s_effectorParamsList.CopyTo(BoingManager.s_aEffectorParams);
		}

		// Token: 0x06007D03 RID: 32003 RVA: 0x00290700 File Offset: 0x0028E900
		internal static void ExecuteReactors(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_effectorMap.Count == 0 && BoingManager.s_reactorMap.Count == 0 && BoingManager.s_fieldMap.Count == 0 && BoingManager.s_cpuSamplerMap.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in BoingManager.s_reactorMap)
			{
				BoingReactor value = keyValuePair.Value;
				if (!value.InitRebooted)
				{
					value.Reboot();
					value.InitRebooted = true;
				}
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.ExecuteReactors(BoingManager.s_effectorMap, BoingManager.s_reactorMap, BoingManager.s_fieldMap, BoingManager.s_cpuSamplerMap, updateMode);
				return;
			}
			BoingWorkSynchronous.ExecuteReactors(BoingManager.s_aEffectorParams, BoingManager.s_reactorMap, BoingManager.s_fieldMap, BoingManager.s_cpuSamplerMap, updateMode);
		}

		// Token: 0x06007D04 RID: 32004 RVA: 0x002907D8 File Offset: 0x0028E9D8
		internal static void PullReactorResults(BoingManager.UpdateMode updateMode)
		{
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in BoingManager.s_reactorMap)
			{
				if (keyValuePair.Value.UpdateMode == updateMode)
				{
					keyValuePair.Value.PullResults();
				}
			}
			foreach (KeyValuePair<int, BoingReactorFieldCPUSampler> keyValuePair2 in BoingManager.s_cpuSamplerMap)
			{
				if (keyValuePair2.Value.UpdateMode == updateMode)
				{
					keyValuePair2.Value.SampleFromField();
				}
			}
		}

		// Token: 0x06007D05 RID: 32005 RVA: 0x00290894 File Offset: 0x0028EA94
		internal static void RestoreReactors()
		{
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in BoingManager.s_reactorMap)
			{
				keyValuePair.Value.Restore();
			}
			foreach (KeyValuePair<int, BoingReactorFieldCPUSampler> keyValuePair2 in BoingManager.s_cpuSamplerMap)
			{
				keyValuePair2.Value.Restore();
			}
		}

		// Token: 0x06007D06 RID: 32006 RVA: 0x00290934 File Offset: 0x0028EB34
		internal static void DispatchReactorFieldCompute()
		{
			if (BoingManager.s_effectorParamsBuffer == null)
			{
				return;
			}
			BoingManager.s_effectorParamsBuffer.SetData(BoingManager.s_aEffectorParams);
			float deltaTime = Time.deltaTime;
			foreach (KeyValuePair<int, BoingReactorField> keyValuePair in BoingManager.s_fieldMap)
			{
				BoingReactorField value = keyValuePair.Value;
				if (value.HardwareMode == BoingReactorField.HardwareModeEnum.GPU)
				{
					value.ExecuteGpu(deltaTime, BoingManager.s_effectorParamsBuffer, BoingManager.s_effectorParamsIndexMap);
				}
			}
		}

		// Token: 0x06007D07 RID: 32007 RVA: 0x002909C0 File Offset: 0x0028EBC0
		internal static void ExecuteBones(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_bonesMap.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<int, BoingBones> keyValuePair in BoingManager.s_bonesMap)
			{
				BoingBones value = keyValuePair.Value;
				if (!value.InitRebooted)
				{
					value.Reboot();
					value.InitRebooted = true;
				}
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.ExecuteBones(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
				return;
			}
			BoingWorkSynchronous.ExecuteBones(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
		}

		// Token: 0x06007D08 RID: 32008 RVA: 0x00290A60 File Offset: 0x0028EC60
		internal static void PullBonesResults(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_bonesMap.Count == 0)
			{
				return;
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.PullBonesResults(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
				return;
			}
			BoingWorkSynchronous.PullBonesResults(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
		}

		// Token: 0x06007D09 RID: 32009 RVA: 0x00290A98 File Offset: 0x0028EC98
		internal static void RestoreBones()
		{
			foreach (KeyValuePair<int, BoingBones> keyValuePair in BoingManager.s_bonesMap)
			{
				keyValuePair.Value.Restore();
			}
		}

		// Token: 0x04008E55 RID: 36437
		public static BoingManager.BehaviorRegisterDelegate OnBehaviorRegister;

		// Token: 0x04008E56 RID: 36438
		public static BoingManager.BehaviorUnregisterDelegate OnBehaviorUnregister;

		// Token: 0x04008E57 RID: 36439
		public static BoingManager.EffectorRegisterDelegate OnEffectorRegister;

		// Token: 0x04008E58 RID: 36440
		public static BoingManager.EffectorUnregisterDelegate OnEffectorUnregister;

		// Token: 0x04008E59 RID: 36441
		public static BoingManager.ReactorRegisterDelegate OnReactorRegister;

		// Token: 0x04008E5A RID: 36442
		public static BoingManager.ReactorUnregisterDelegate OnReactorUnregister;

		// Token: 0x04008E5B RID: 36443
		public static BoingManager.ReactorFieldRegisterDelegate OnReactorFieldRegister;

		// Token: 0x04008E5C RID: 36444
		public static BoingManager.ReactorFieldUnregisterDelegate OnReactorFieldUnregister;

		// Token: 0x04008E5D RID: 36445
		public static BoingManager.ReactorFieldCPUSamplerRegisterDelegate OnReactorFieldCPUSamplerRegister;

		// Token: 0x04008E5E RID: 36446
		public static BoingManager.ReactorFieldCPUSamplerUnregisterDelegate OnReactorFieldCPUSamplerUnregister;

		// Token: 0x04008E5F RID: 36447
		public static BoingManager.ReactorFieldGPUSamplerRegisterDelegate OnReactorFieldGPUSamplerRegister;

		// Token: 0x04008E60 RID: 36448
		public static BoingManager.ReactorFieldGPUSamplerUnregisterDelegate OnFieldGPUSamplerUnregister;

		// Token: 0x04008E61 RID: 36449
		public static BoingManager.BonesRegisterDelegate OnBonesRegister;

		// Token: 0x04008E62 RID: 36450
		public static BoingManager.BonesUnregisterDelegate OnBonesUnregister;

		// Token: 0x04008E63 RID: 36451
		private static float s_deltaTime = 0f;

		// Token: 0x04008E64 RID: 36452
		private static Dictionary<int, BoingBehavior> s_behaviorMap = new Dictionary<int, BoingBehavior>();

		// Token: 0x04008E65 RID: 36453
		private static Dictionary<int, BoingEffector> s_effectorMap = new Dictionary<int, BoingEffector>();

		// Token: 0x04008E66 RID: 36454
		private static Dictionary<int, BoingReactor> s_reactorMap = new Dictionary<int, BoingReactor>();

		// Token: 0x04008E67 RID: 36455
		private static Dictionary<int, BoingReactorField> s_fieldMap = new Dictionary<int, BoingReactorField>();

		// Token: 0x04008E68 RID: 36456
		private static Dictionary<int, BoingReactorFieldCPUSampler> s_cpuSamplerMap = new Dictionary<int, BoingReactorFieldCPUSampler>();

		// Token: 0x04008E69 RID: 36457
		private static Dictionary<int, BoingReactorFieldGPUSampler> s_gpuSamplerMap = new Dictionary<int, BoingReactorFieldGPUSampler>();

		// Token: 0x04008E6A RID: 36458
		private static Dictionary<int, BoingBones> s_bonesMap = new Dictionary<int, BoingBones>();

		// Token: 0x04008E6B RID: 36459
		private static readonly int kEffectorParamsIncrement = 16;

		// Token: 0x04008E6C RID: 36460
		private static List<BoingEffector.Params> s_effectorParamsList = new List<BoingEffector.Params>(BoingManager.kEffectorParamsIncrement);

		// Token: 0x04008E6D RID: 36461
		private static BoingEffector.Params[] s_aEffectorParams;

		// Token: 0x04008E6E RID: 36462
		private static ComputeBuffer s_effectorParamsBuffer;

		// Token: 0x04008E6F RID: 36463
		private static Dictionary<int, int> s_effectorParamsIndexMap = new Dictionary<int, int>();

		// Token: 0x04008E70 RID: 36464
		internal static readonly bool UseAsynchronousJobs = true;

		// Token: 0x04008E71 RID: 36465
		internal static GameObject s_managerGo;

		// Token: 0x0200135E RID: 4958
		public enum UpdateMode
		{
			// Token: 0x04008E73 RID: 36467
			FixedUpdate,
			// Token: 0x04008E74 RID: 36468
			EarlyUpdate,
			// Token: 0x04008E75 RID: 36469
			LateUpdate
		}

		// Token: 0x0200135F RID: 4959
		public enum TranslationLockSpace
		{
			// Token: 0x04008E77 RID: 36471
			Global,
			// Token: 0x04008E78 RID: 36472
			Local
		}

		// Token: 0x02001360 RID: 4960
		// (Invoke) Token: 0x06007D0C RID: 32012
		public delegate void BehaviorRegisterDelegate(BoingBehavior behavior);

		// Token: 0x02001361 RID: 4961
		// (Invoke) Token: 0x06007D10 RID: 32016
		public delegate void BehaviorUnregisterDelegate(BoingBehavior behavior);

		// Token: 0x02001362 RID: 4962
		// (Invoke) Token: 0x06007D14 RID: 32020
		public delegate void EffectorRegisterDelegate(BoingEffector effector);

		// Token: 0x02001363 RID: 4963
		// (Invoke) Token: 0x06007D18 RID: 32024
		public delegate void EffectorUnregisterDelegate(BoingEffector effector);

		// Token: 0x02001364 RID: 4964
		// (Invoke) Token: 0x06007D1C RID: 32028
		public delegate void ReactorRegisterDelegate(BoingReactor reactor);

		// Token: 0x02001365 RID: 4965
		// (Invoke) Token: 0x06007D20 RID: 32032
		public delegate void ReactorUnregisterDelegate(BoingReactor reactor);

		// Token: 0x02001366 RID: 4966
		// (Invoke) Token: 0x06007D24 RID: 32036
		public delegate void ReactorFieldRegisterDelegate(BoingReactorField field);

		// Token: 0x02001367 RID: 4967
		// (Invoke) Token: 0x06007D28 RID: 32040
		public delegate void ReactorFieldUnregisterDelegate(BoingReactorField field);

		// Token: 0x02001368 RID: 4968
		// (Invoke) Token: 0x06007D2C RID: 32044
		public delegate void ReactorFieldCPUSamplerRegisterDelegate(BoingReactorFieldCPUSampler sampler);

		// Token: 0x02001369 RID: 4969
		// (Invoke) Token: 0x06007D30 RID: 32048
		public delegate void ReactorFieldCPUSamplerUnregisterDelegate(BoingReactorFieldCPUSampler sampler);

		// Token: 0x0200136A RID: 4970
		// (Invoke) Token: 0x06007D34 RID: 32052
		public delegate void ReactorFieldGPUSamplerRegisterDelegate(BoingReactorFieldGPUSampler sampler);

		// Token: 0x0200136B RID: 4971
		// (Invoke) Token: 0x06007D38 RID: 32056
		public delegate void ReactorFieldGPUSamplerUnregisterDelegate(BoingReactorFieldGPUSampler sampler);

		// Token: 0x0200136C RID: 4972
		// (Invoke) Token: 0x06007D3C RID: 32060
		public delegate void BonesRegisterDelegate(BoingBones bones);

		// Token: 0x0200136D RID: 4973
		// (Invoke) Token: 0x06007D40 RID: 32064
		public delegate void BonesUnregisterDelegate(BoingBones bones);
	}
}
