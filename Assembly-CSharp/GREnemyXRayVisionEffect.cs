using System;
using UnityEngine;

// Token: 0x02000799 RID: 1945
public class GREnemyXRayVisionEffect : MonoBehaviour
{
	// Token: 0x060031CF RID: 12751 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void Awake()
	{
	}

	// Token: 0x060031D0 RID: 12752 RVA: 0x00111B92 File Offset: 0x0010FD92
	private void Start()
	{
		base.InvokeRepeating("UpdateEffect", 0f, 0.5f);
	}

	// Token: 0x060031D1 RID: 12753 RVA: 0x00111BA9 File Offset: 0x0010FDA9
	private bool ShouldShowEffect()
	{
		return GRPlayer.GetLocal().HasXRayVision();
	}

	// Token: 0x060031D2 RID: 12754 RVA: 0x00111BB5 File Offset: 0x0010FDB5
	private void UpdateEffect()
	{
		this.enemyXRayEffect.SetActive(this.ShouldShowEffect());
	}

	// Token: 0x040040A7 RID: 16551
	public GameObject enemyXRayEffect;
}
