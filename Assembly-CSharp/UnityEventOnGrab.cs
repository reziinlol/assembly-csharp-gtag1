using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200059D RID: 1437
[RequireComponent(typeof(TransferrableObject))]
public class UnityEventOnGrab : MonoBehaviour
{
	// Token: 0x06002468 RID: 9320 RVA: 0x000C3B8C File Offset: 0x000C1D8C
	private void Awake()
	{
		TransferrableObject componentInParent = base.GetComponentInParent<TransferrableObject>();
		Behaviour[] behavioursEnabledOnlyWhileHeld = componentInParent.behavioursEnabledOnlyWhileHeld;
		Behaviour[] array = new Behaviour[behavioursEnabledOnlyWhileHeld.Length + 1];
		for (int i = 0; i < behavioursEnabledOnlyWhileHeld.Length; i++)
		{
			array[i] = behavioursEnabledOnlyWhileHeld[i];
		}
		array[behavioursEnabledOnlyWhileHeld.Length] = this;
		componentInParent.behavioursEnabledOnlyWhileHeld = array;
	}

	// Token: 0x06002469 RID: 9321 RVA: 0x000C3BD3 File Offset: 0x000C1DD3
	private void OnEnable()
	{
		UnityEvent unityEvent = this.onGrab;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x0600246A RID: 9322 RVA: 0x000C3BE5 File Offset: 0x000C1DE5
	private void OnDisable()
	{
		UnityEvent unityEvent = this.onRelease;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x04002FD2 RID: 12242
	[SerializeField]
	private UnityEvent onGrab;

	// Token: 0x04002FD3 RID: 12243
	[SerializeField]
	private UnityEvent onRelease;
}
