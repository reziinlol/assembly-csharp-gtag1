using System;
using UnityEngine;

// Token: 0x02000D86 RID: 3462
public class SplineDecorator : MonoBehaviour
{
	// Token: 0x060054F2 RID: 21746 RVA: 0x001BC1E0 File Offset: 0x001BA3E0
	private void Awake()
	{
		if (this.frequency <= 0 || this.items == null || this.items.Length == 0)
		{
			return;
		}
		float num = (float)(this.frequency * this.items.Length);
		if (this.spline.Loop || num == 1f)
		{
			num = 1f / num;
		}
		else
		{
			num = 1f / (num - 1f);
		}
		int num2 = 0;
		for (int i = 0; i < this.frequency; i++)
		{
			int j = 0;
			while (j < this.items.Length)
			{
				Transform transform = Object.Instantiate<Transform>(this.items[j]);
				Vector3 point = this.spline.GetPoint((float)num2 * num);
				transform.transform.localPosition = point;
				if (this.lookForward)
				{
					transform.transform.LookAt(point + this.spline.GetDirection((float)num2 * num));
				}
				transform.transform.parent = base.transform;
				j++;
				num2++;
			}
		}
	}

	// Token: 0x04006570 RID: 25968
	public BezierSpline spline;

	// Token: 0x04006571 RID: 25969
	public int frequency;

	// Token: 0x04006572 RID: 25970
	public bool lookForward;

	// Token: 0x04006573 RID: 25971
	public Transform[] items;
}
