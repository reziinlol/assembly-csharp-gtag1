using System;
using TagEffects;
using UnityEngine;

// Token: 0x020002DC RID: 732
public class HandTapReactor : MonoBehaviour
{
	// Token: 0x060012A5 RID: 4773 RVA: 0x00063295 File Offset: 0x00061495
	private void LeftDown(HandEffectContext ctx)
	{
		this.handTapEvents.InvokeAll(HandTapReactor.TapType.LeftDown, false);
	}

	// Token: 0x060012A6 RID: 4774 RVA: 0x000632A4 File Offset: 0x000614A4
	private void LeftUp(HandEffectContext ctx)
	{
		this.handTapEvents.InvokeAll(HandTapReactor.TapType.LeftUp, false);
	}

	// Token: 0x060012A7 RID: 4775 RVA: 0x000632B4 File Offset: 0x000614B4
	private void LeftGesture(IHandEffectsTrigger.Mode mode)
	{
		FlagEvents<HandTapReactor.TapType> flagEvents = this.handTapEvents;
		HandTapReactor.TapType test;
		switch (mode)
		{
		case IHandEffectsTrigger.Mode.HighFive:
			test = HandTapReactor.TapType.LeftHighFive;
			break;
		case IHandEffectsTrigger.Mode.FistBump:
			test = HandTapReactor.TapType.LeftFistBump;
			break;
		case IHandEffectsTrigger.Mode.Tag3P:
			test = HandTapReactor.TapType.LeftTagThirdPerson;
			break;
		case IHandEffectsTrigger.Mode.Tag1P:
			test = HandTapReactor.TapType.LeftTagFirstPerson;
			break;
		default:
			<PrivateImplementationDetails>.ThrowSwitchExpressionException(mode);
			break;
		}
		flagEvents.InvokeAll(test, false);
	}

	// Token: 0x060012A8 RID: 4776 RVA: 0x00063305 File Offset: 0x00061505
	private void RightDown(HandEffectContext ctx)
	{
		this.handTapEvents.InvokeAll(HandTapReactor.TapType.RightDown, false);
	}

	// Token: 0x060012A9 RID: 4777 RVA: 0x00063315 File Offset: 0x00061515
	private void RightUp(HandEffectContext ctx)
	{
		this.handTapEvents.InvokeAll(HandTapReactor.TapType.RightUp, false);
	}

	// Token: 0x060012AA RID: 4778 RVA: 0x00063328 File Offset: 0x00061528
	private void RightGesture(IHandEffectsTrigger.Mode mode)
	{
		FlagEvents<HandTapReactor.TapType> flagEvents = this.handTapEvents;
		HandTapReactor.TapType test;
		switch (mode)
		{
		case IHandEffectsTrigger.Mode.HighFive:
			test = HandTapReactor.TapType.RightHighFive;
			break;
		case IHandEffectsTrigger.Mode.FistBump:
			test = HandTapReactor.TapType.RightFistBump;
			break;
		case IHandEffectsTrigger.Mode.Tag3P:
			test = HandTapReactor.TapType.RightTagThirdPerson;
			break;
		case IHandEffectsTrigger.Mode.Tag1P:
			test = HandTapReactor.TapType.RightTagFirstPerson;
			break;
		default:
			<PrivateImplementationDetails>.ThrowSwitchExpressionException(mode);
			break;
		}
		flagEvents.InvokeAll(test, false);
	}

	// Token: 0x060012AB RID: 4779 RVA: 0x00063388 File Offset: 0x00061588
	private void OnEnable()
	{
		if (this.myRig == null)
		{
			this.myRig = base.GetComponentInParent<VRRig>();
			IHandEffectsTrigger[] componentsInChildren = this.myRig.GetComponentsInChildren<IHandEffectsTrigger>();
			if (componentsInChildren[0].RightHand)
			{
				this.rightHandTrigger = componentsInChildren[0];
				this.leftHandTrigger = componentsInChildren[1];
			}
			else
			{
				this.rightHandTrigger = componentsInChildren[1];
				this.leftHandTrigger = componentsInChildren[0];
			}
		}
		if (this.myRig != null)
		{
			this.myRig.LeftHandEffect.handTapDown += this.LeftDown;
			this.myRig.LeftHandEffect.handTapUp += this.LeftUp;
			IHandEffectsTrigger handEffectsTrigger = this.leftHandTrigger;
			handEffectsTrigger.OnTrigger = (Action<IHandEffectsTrigger.Mode>)Delegate.Combine(handEffectsTrigger.OnTrigger, new Action<IHandEffectsTrigger.Mode>(this.LeftGesture));
			this.myRig.RightHandEffect.handTapDown += this.RightDown;
			this.myRig.RightHandEffect.handTapUp += this.RightUp;
			IHandEffectsTrigger handEffectsTrigger2 = this.rightHandTrigger;
			handEffectsTrigger2.OnTrigger = (Action<IHandEffectsTrigger.Mode>)Delegate.Combine(handEffectsTrigger2.OnTrigger, new Action<IHandEffectsTrigger.Mode>(this.RightGesture));
		}
	}

	// Token: 0x060012AC RID: 4780 RVA: 0x000634BC File Offset: 0x000616BC
	private void OnDisable()
	{
		if (this.myRig != null)
		{
			this.myRig.LeftHandEffect.handTapDown -= this.LeftDown;
			this.myRig.LeftHandEffect.handTapUp -= this.LeftUp;
			IHandEffectsTrigger handEffectsTrigger = this.leftHandTrigger;
			handEffectsTrigger.OnTrigger = (Action<IHandEffectsTrigger.Mode>)Delegate.Remove(handEffectsTrigger.OnTrigger, new Action<IHandEffectsTrigger.Mode>(this.LeftGesture));
			this.myRig.RightHandEffect.handTapDown -= this.RightDown;
			this.myRig.RightHandEffect.handTapUp -= this.RightUp;
			IHandEffectsTrigger handEffectsTrigger2 = this.rightHandTrigger;
			handEffectsTrigger2.OnTrigger = (Action<IHandEffectsTrigger.Mode>)Delegate.Remove(handEffectsTrigger2.OnTrigger, new Action<IHandEffectsTrigger.Mode>(this.RightGesture));
		}
	}

	// Token: 0x040016BA RID: 5818
	[SerializeField]
	private FlagEvents<HandTapReactor.TapType> handTapEvents;

	// Token: 0x040016BB RID: 5819
	private VRRig myRig;

	// Token: 0x040016BC RID: 5820
	private IHandEffectsTrigger leftHandTrigger;

	// Token: 0x040016BD RID: 5821
	private IHandEffectsTrigger rightHandTrigger;

	// Token: 0x020002DD RID: 733
	[Flags]
	private enum TapType
	{
		// Token: 0x040016BF RID: 5823
		None = 0,
		// Token: 0x040016C0 RID: 5824
		LeftDown = 1,
		// Token: 0x040016C1 RID: 5825
		LeftUp = 2,
		// Token: 0x040016C2 RID: 5826
		LeftHighFive = 4,
		// Token: 0x040016C3 RID: 5827
		LeftFistBump = 8,
		// Token: 0x040016C4 RID: 5828
		LeftTagFirstPerson = 16,
		// Token: 0x040016C5 RID: 5829
		LeftTagThirdPerson = 32,
		// Token: 0x040016C6 RID: 5830
		AllLeft = 63,
		// Token: 0x040016C7 RID: 5831
		RightDown = 64,
		// Token: 0x040016C8 RID: 5832
		RightUp = 128,
		// Token: 0x040016C9 RID: 5833
		RightHighFive = 256,
		// Token: 0x040016CA RID: 5834
		RightFistBump = 512,
		// Token: 0x040016CB RID: 5835
		RightTagFirstPerson = 1024,
		// Token: 0x040016CC RID: 5836
		RightTagThirdPerson = 2048,
		// Token: 0x040016CD RID: 5837
		AllRight = 4032,
		// Token: 0x040016CE RID: 5838
		All = -1
	}
}
