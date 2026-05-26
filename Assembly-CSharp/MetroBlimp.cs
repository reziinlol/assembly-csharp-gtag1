using System;
using UnityEngine;

// Token: 0x020001BB RID: 443
public class MetroBlimp : MonoBehaviour
{
	// Token: 0x06000BDD RID: 3037 RVA: 0x00040BF7 File Offset: 0x0003EDF7
	private void Awake()
	{
		this._startLocalHeight = base.transform.localPosition.y;
	}

	// Token: 0x06000BDE RID: 3038 RVA: 0x00040C10 File Offset: 0x0003EE10
	public void Tick()
	{
		bool flag = Mathf.Sin(Time.time * 2f) * 0.5f + 0.5f < 0.0001f;
		int num = Mathf.CeilToInt(this._numHandsOnBlimp / 2f);
		if (this._numHandsOnBlimp == 0f)
		{
			this._topStayTime = 0f;
			if (flag)
			{
				this.blimpRenderer.material.DisableKeyword("_INNER_GLOW");
			}
		}
		else
		{
			this._topStayTime += Time.deltaTime;
			if (flag)
			{
				this.blimpRenderer.material.EnableKeyword("_INNER_GLOW");
			}
		}
		Vector3 localPosition = base.transform.localPosition;
		Vector3 vector = localPosition;
		float y = vector.y;
		float num2 = this._startLocalHeight + this.descendOffset;
		float deltaTime = Time.deltaTime;
		if (num > 0)
		{
			if (y > num2)
			{
				vector += Vector3.down * (this.descendSpeed * (float)num * deltaTime);
			}
		}
		else if (y < this._startLocalHeight)
		{
			vector += Vector3.up * (this.ascendSpeed * deltaTime);
		}
		base.transform.localPosition = Vector3.Slerp(localPosition, vector, 0.5f);
	}

	// Token: 0x06000BDF RID: 3039 RVA: 0x00040D3F File Offset: 0x0003EF3F
	private static bool IsPlayerHand(Collider c)
	{
		return c.gameObject.IsOnLayer(UnityLayer.GorillaHand);
	}

	// Token: 0x06000BE0 RID: 3040 RVA: 0x00040D4E File Offset: 0x0003EF4E
	private void OnTriggerEnter(Collider other)
	{
		if (MetroBlimp.IsPlayerHand(other))
		{
			this._numHandsOnBlimp += 1f;
		}
	}

	// Token: 0x06000BE1 RID: 3041 RVA: 0x00040D6A File Offset: 0x0003EF6A
	private void OnTriggerExit(Collider other)
	{
		if (MetroBlimp.IsPlayerHand(other))
		{
			this._numHandsOnBlimp -= 1f;
		}
	}

	// Token: 0x04000E6B RID: 3691
	public MetroSpotlight spotLightLeft;

	// Token: 0x04000E6C RID: 3692
	public MetroSpotlight spotLightRight;

	// Token: 0x04000E6D RID: 3693
	[Space]
	public BoxCollider topCollider;

	// Token: 0x04000E6E RID: 3694
	public Material blimpMaterial;

	// Token: 0x04000E6F RID: 3695
	public Renderer blimpRenderer;

	// Token: 0x04000E70 RID: 3696
	[Space]
	public float ascendSpeed = 1f;

	// Token: 0x04000E71 RID: 3697
	public float descendSpeed = 0.5f;

	// Token: 0x04000E72 RID: 3698
	public float descendOffset = -24.1f;

	// Token: 0x04000E73 RID: 3699
	public float descendReactionTime = 3f;

	// Token: 0x04000E74 RID: 3700
	[Space]
	[NonSerialized]
	private float _startLocalHeight;

	// Token: 0x04000E75 RID: 3701
	[NonSerialized]
	private float _topStayTime;

	// Token: 0x04000E76 RID: 3702
	[NonSerialized]
	private float _numHandsOnBlimp;

	// Token: 0x04000E77 RID: 3703
	[NonSerialized]
	private bool _lowering;

	// Token: 0x04000E78 RID: 3704
	private const string _INNER_GLOW = "_INNER_GLOW";
}
