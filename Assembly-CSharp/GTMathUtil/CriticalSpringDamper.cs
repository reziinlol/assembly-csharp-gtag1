using System;

namespace GTMathUtil
{
	// Token: 0x02000EB4 RID: 3764
	internal class CriticalSpringDamper
	{
		// Token: 0x06005C8A RID: 23690 RVA: 0x001D3766 File Offset: 0x001D1966
		private static float halflife_to_damping(float halflife, float eps = 1E-05f)
		{
			return 2.7725887f / (halflife + eps);
		}

		// Token: 0x06005C8B RID: 23691 RVA: 0x001D348D File Offset: 0x001D168D
		private static float fast_negexp(float x)
		{
			return 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);
		}

		// Token: 0x06005C8C RID: 23692 RVA: 0x001D5EB8 File Offset: 0x001D40B8
		public float Update(float dt)
		{
			float num = CriticalSpringDamper.halflife_to_damping(this.halfLife, 1E-05f) / 2f;
			float num2 = this.x - this.xGoal;
			float num3 = this.curVel + num2 * num;
			float num4 = CriticalSpringDamper.fast_negexp(num * dt);
			this.x = num4 * (num2 + num3 * dt) + this.xGoal;
			this.curVel = num4 * (this.curVel - num3 * num * dt);
			return this.x;
		}

		// Token: 0x04006AEC RID: 27372
		public float x;

		// Token: 0x04006AED RID: 27373
		public float xGoal;

		// Token: 0x04006AEE RID: 27374
		public float halfLife = 0.1f;

		// Token: 0x04006AEF RID: 27375
		private float curVel;
	}
}
