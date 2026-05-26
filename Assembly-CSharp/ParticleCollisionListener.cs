using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000AE9 RID: 2793
public class ParticleCollisionListener : MonoBehaviour
{
	// Token: 0x06004739 RID: 18233 RVA: 0x0018017C File Offset: 0x0017E37C
	private void Awake()
	{
		this._events = new List<ParticleCollisionEvent>();
	}

	// Token: 0x0600473A RID: 18234 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnCollisionEvent(ParticleCollisionEvent ev)
	{
	}

	// Token: 0x0600473B RID: 18235 RVA: 0x0018018C File Offset: 0x0017E38C
	public void OnParticleCollision(GameObject other)
	{
		int collisionEvents = this.target.GetCollisionEvents(other, this._events);
		for (int i = 0; i < collisionEvents; i++)
		{
			this.OnCollisionEvent(this._events[i]);
		}
	}

	// Token: 0x040059B1 RID: 22961
	public ParticleSystem target;

	// Token: 0x040059B2 RID: 22962
	[SerializeReference]
	private List<ParticleCollisionEvent> _events = new List<ParticleCollisionEvent>();
}
