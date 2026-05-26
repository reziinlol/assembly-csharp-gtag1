using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x020013DE RID: 5086
	[WeaverGenerated]
	internal struct ReaderWriter@BatteryChargerState__FusionCrankData : IElementReaderWriter<BatteryChargerState.FusionCrankData>
	{
		// Token: 0x06007EC7 RID: 32455 RVA: 0x00298E71 File Offset: 0x00297071
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public unsafe BatteryChargerState.FusionCrankData Read(byte* data, int index)
		{
			return *(BatteryChargerState.FusionCrankData*)(data + index * 12);
		}

		// Token: 0x06007EC8 RID: 32456 RVA: 0x00298E81 File Offset: 0x00297081
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public unsafe ref BatteryChargerState.FusionCrankData ReadRef(byte* data, int index)
		{
			return ref *(BatteryChargerState.FusionCrankData*)(data + index * 12);
		}

		// Token: 0x06007EC9 RID: 32457 RVA: 0x00298E8C File Offset: 0x0029708C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, BatteryChargerState.FusionCrankData val)
		{
			*(BatteryChargerState.FusionCrankData*)(data + index * 12) = val;
		}

		// Token: 0x06007ECA RID: 32458 RVA: 0x00135CD9 File Offset: 0x00133ED9
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 3;
		}

		// Token: 0x06007ECB RID: 32459 RVA: 0x00298EA0 File Offset: 0x002970A0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public int GetElementHashCode(BatteryChargerState.FusionCrankData val)
		{
			return val.GetHashCode();
		}

		// Token: 0x06007ECC RID: 32460 RVA: 0x00298EBC File Offset: 0x002970BC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<BatteryChargerState.FusionCrankData> GetInstance()
		{
			if (ReaderWriter@BatteryChargerState__FusionCrankData.Instance == null)
			{
				ReaderWriter@BatteryChargerState__FusionCrankData.Instance = default(ReaderWriter@BatteryChargerState__FusionCrankData);
			}
			return ReaderWriter@BatteryChargerState__FusionCrankData.Instance;
		}

		// Token: 0x04009316 RID: 37654
		[WeaverGenerated]
		public static IElementReaderWriter<BatteryChargerState.FusionCrankData> Instance;
	}
}
