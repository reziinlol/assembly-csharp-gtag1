using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x020010DB RID: 4315
	public class TagEffectTester : MonoBehaviour, IHandEffectsTrigger
	{
		// Token: 0x17000A38 RID: 2616
		// (get) Token: 0x06006C00 RID: 27648 RVA: 0x0022F3EF File Offset: 0x0022D5EF
		public bool Static
		{
			get
			{
				return this.isStatic;
			}
		}

		// Token: 0x17000A39 RID: 2617
		// (get) Token: 0x06006C01 RID: 27649 RVA: 0x0022F3F7 File Offset: 0x0022D5F7
		public IHandEffectsTrigger.Mode EffectMode { get; }

		// Token: 0x17000A3A RID: 2618
		// (get) Token: 0x06006C02 RID: 27650 RVA: 0x0022F3FF File Offset: 0x0022D5FF
		public Transform Transform { get; }

		// Token: 0x17000A3B RID: 2619
		// (get) Token: 0x06006C03 RID: 27651 RVA: 0x00035D0D File Offset: 0x00033F0D
		public VRRig Rig
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000A3C RID: 2620
		// (get) Token: 0x06006C04 RID: 27652 RVA: 0x0022F407 File Offset: 0x0022D607
		public bool FingersDown { get; }

		// Token: 0x17000A3D RID: 2621
		// (get) Token: 0x06006C05 RID: 27653 RVA: 0x0022F40F File Offset: 0x0022D60F
		public bool FingersUp { get; }

		// Token: 0x17000A3E RID: 2622
		// (get) Token: 0x06006C06 RID: 27654 RVA: 0x0022F417 File Offset: 0x0022D617
		public Vector3 Velocity { get; }

		// Token: 0x17000A3F RID: 2623
		// (get) Token: 0x06006C07 RID: 27655 RVA: 0x0022F41F File Offset: 0x0022D61F
		// (set) Token: 0x06006C08 RID: 27656 RVA: 0x0022F427 File Offset: 0x0022D627
		public Action<IHandEffectsTrigger.Mode> OnTrigger { get; set; }

		// Token: 0x17000A40 RID: 2624
		// (get) Token: 0x06006C09 RID: 27657 RVA: 0x0022F430 File Offset: 0x0022D630
		public bool RightHand { get; }

		// Token: 0x17000A41 RID: 2625
		// (get) Token: 0x06006C0A RID: 27658 RVA: 0x0022F438 File Offset: 0x0022D638
		public float Magnitude { get; }

		// Token: 0x17000A42 RID: 2626
		// (get) Token: 0x06006C0B RID: 27659 RVA: 0x0022F440 File Offset: 0x0022D640
		public TagEffectPack CosmeticEffectPack { get; }

		// Token: 0x06006C0C RID: 27660 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnTriggerEntered(IHandEffectsTrigger other)
		{
		}

		// Token: 0x06006C0D RID: 27661 RVA: 0x00002076 File Offset: 0x00000276
		public bool InTriggerZone(IHandEffectsTrigger t)
		{
			return false;
		}

		// Token: 0x04007C2D RID: 31789
		[SerializeField]
		private bool isStatic = true;
	}
}
