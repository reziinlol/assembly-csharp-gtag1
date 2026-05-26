using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000CC RID: 204
public class SteeringWheelCosmetic : MonoBehaviour
{
	// Token: 0x060004F1 RID: 1265 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Start()
	{
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x0001B712 File Offset: 0x00019912
	public void TryHornHit()
	{
		if (Time.time > this.lastHornTime + this.cooldown)
		{
			this.lastHornTime = Time.time;
			UnityEvent unityEvent = this.onHornHit;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x0001B744 File Offset: 0x00019944
	private void Update()
	{
		float z = base.transform.localEulerAngles.z;
		if (Mathf.Abs(Mathf.DeltaAngle(this.lastZAngle, z)) >= this.dramaticTurnThreshold)
		{
			UnityEvent unityEvent = this.onDramaticTurn;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
		}
		this.lastZAngle = z;
	}

	// Token: 0x04000599 RID: 1433
	[SerializeField]
	private float cooldown = 1.5f;

	// Token: 0x0400059A RID: 1434
	[SerializeField]
	private float dramaticTurnThreshold = 35f;

	// Token: 0x0400059B RID: 1435
	[SerializeField]
	private UnityEvent onHornHit;

	// Token: 0x0400059C RID: 1436
	[SerializeField]
	private UnityEvent onDramaticTurn;

	// Token: 0x0400059D RID: 1437
	private float lastHornTime = -999f;

	// Token: 0x0400059E RID: 1438
	private float lastZAngle;
}
