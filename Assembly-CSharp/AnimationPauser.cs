using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000B73 RID: 2931
public class AnimationPauser : StateMachineBehaviour
{
	// Token: 0x060049CC RID: 18892 RVA: 0x0018B568 File Offset: 0x00189768
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		AnimationPauser.<OnStateEnter>d__4 <OnStateEnter>d__;
		<OnStateEnter>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnStateEnter>d__.<>4__this = this;
		<OnStateEnter>d__.animator = animator;
		<OnStateEnter>d__.stateInfo = stateInfo;
		<OnStateEnter>d__.layerIndex = layerIndex;
		<OnStateEnter>d__.<>1__state = -1;
		<OnStateEnter>d__.<>t__builder.Start<AnimationPauser.<OnStateEnter>d__4>(ref <OnStateEnter>d__);
	}

	// Token: 0x04005C8F RID: 23695
	[SerializeField]
	private int _maxTimeBetweenAnims = 5;

	// Token: 0x04005C90 RID: 23696
	[SerializeField]
	private int _minTimeBetweenAnims = 1;

	// Token: 0x04005C91 RID: 23697
	private int _animPauseDuration;

	// Token: 0x04005C92 RID: 23698
	private static readonly string Restart_Anim_Name = "RestartAnim";
}
