using System;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x02001190 RID: 4496
	public class SplinePlanetZone : PlanetZone
	{
		// Token: 0x060071E0 RID: 29152 RVA: 0x002512F8 File Offset: 0x0024F4F8
		protected override Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
		{
			Vector3 vector;
			float closestEvaluationOnSpline = this.spline.GetClosestEvaluationOnSpline(worldPosition, out vector);
			Vector3 b = this.spline.Evaluate(closestEvaluationOnSpline);
			return worldPosition - b;
		}

		// Token: 0x04008191 RID: 33169
		[SerializeField]
		private CatmullRomSpline spline;
	}
}
