using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002E4 RID: 740
public class OnSqueezeTrigger : MonoBehaviour
{
	// Token: 0x060012D3 RID: 4819 RVA: 0x000640B8 File Offset: 0x000622B8
	private void Start()
	{
		this.myRig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x060012D4 RID: 4820 RVA: 0x000640C8 File Offset: 0x000622C8
	private void Update()
	{
		bool flag;
		if (this.myHoldable.InLeftHand())
		{
			flag = ((this.indexFinger ? this.myRig.leftIndex.calcT : this.myRig.leftMiddle.calcT) > 0.5f);
		}
		else
		{
			flag = (this.myHoldable.InRightHand() && (this.indexFinger ? this.myRig.rightIndex.calcT : this.myRig.rightMiddle.calcT) > 0.5f);
		}
		if (flag != this.triggerWasDown)
		{
			if (flag)
			{
				this.onPress.Invoke();
				this.updateWhilePressed.Invoke();
			}
			else
			{
				this.onRelease.Invoke();
			}
		}
		else if (flag)
		{
			this.updateWhilePressed.Invoke();
		}
		this.triggerWasDown = flag;
	}

	// Token: 0x04001705 RID: 5893
	[SerializeField]
	private TransferrableObject myHoldable;

	// Token: 0x04001706 RID: 5894
	[SerializeField]
	private UnityEvent onPress;

	// Token: 0x04001707 RID: 5895
	[SerializeField]
	private UnityEvent onRelease;

	// Token: 0x04001708 RID: 5896
	[SerializeField]
	private UnityEvent updateWhilePressed;

	// Token: 0x04001709 RID: 5897
	private VRRig myRig;

	// Token: 0x0400170A RID: 5898
	private bool indexFinger = true;

	// Token: 0x0400170B RID: 5899
	private bool triggerWasDown;
}
