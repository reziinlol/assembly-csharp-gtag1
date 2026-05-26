using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x020010CF RID: 4303
	public interface IHandEffectsTrigger
	{
		// Token: 0x17000A26 RID: 2598
		// (get) Token: 0x06006BCC RID: 27596
		IHandEffectsTrigger.Mode EffectMode { get; }

		// Token: 0x17000A27 RID: 2599
		// (get) Token: 0x06006BCD RID: 27597
		Transform Transform { get; }

		// Token: 0x17000A28 RID: 2600
		// (get) Token: 0x06006BCE RID: 27598
		VRRig Rig { get; }

		// Token: 0x17000A29 RID: 2601
		// (get) Token: 0x06006BCF RID: 27599
		bool FingersDown { get; }

		// Token: 0x17000A2A RID: 2602
		// (get) Token: 0x06006BD0 RID: 27600
		bool FingersUp { get; }

		// Token: 0x17000A2B RID: 2603
		// (get) Token: 0x06006BD1 RID: 27601
		Vector3 Velocity { get; }

		// Token: 0x17000A2C RID: 2604
		// (get) Token: 0x06006BD2 RID: 27602
		// (set) Token: 0x06006BD3 RID: 27603
		Action<IHandEffectsTrigger.Mode> OnTrigger { get; set; }

		// Token: 0x17000A2D RID: 2605
		// (get) Token: 0x06006BD4 RID: 27604
		bool RightHand { get; }

		// Token: 0x17000A2E RID: 2606
		// (get) Token: 0x06006BD5 RID: 27605
		TagEffectPack CosmeticEffectPack { get; }

		// Token: 0x17000A2F RID: 2607
		// (get) Token: 0x06006BD6 RID: 27606
		bool Static { get; }

		// Token: 0x06006BD7 RID: 27607
		void OnTriggerEntered(IHandEffectsTrigger other);

		// Token: 0x06006BD8 RID: 27608
		bool InTriggerZone(IHandEffectsTrigger t);

		// Token: 0x020010D0 RID: 4304
		public enum Mode
		{
			// Token: 0x04007BFC RID: 31740
			HighFive,
			// Token: 0x04007BFD RID: 31741
			FistBump,
			// Token: 0x04007BFE RID: 31742
			Tag3P,
			// Token: 0x04007BFF RID: 31743
			Tag1P,
			// Token: 0x04007C00 RID: 31744
			HighFive_And_FistBump
		}
	}
}
