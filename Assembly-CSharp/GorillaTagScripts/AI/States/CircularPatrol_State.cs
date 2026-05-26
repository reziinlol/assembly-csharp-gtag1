using System;
using UnityEngine;

namespace GorillaTagScripts.AI.States
{
	// Token: 0x02000FEF RID: 4079
	public class CircularPatrol_State : IState
	{
		// Token: 0x060065FD RID: 26109 RVA: 0x0020E08C File Offset: 0x0020C28C
		public CircularPatrol_State(AIEntity entity)
		{
			this.entity = entity;
		}

		// Token: 0x060065FE RID: 26110 RVA: 0x0020E09C File Offset: 0x0020C29C
		public void Tick()
		{
			Vector3 position = this.entity.circleCenter.position;
			float x = position.x + Mathf.Cos(this.angle) * this.entity.angularSpeed;
			float y = position.y;
			float z = position.z + Mathf.Sin(this.angle) * this.entity.angularSpeed;
			this.entity.transform.position = new Vector3(x, y, z);
			this.angle += this.entity.angularSpeed * Time.deltaTime;
		}

		// Token: 0x060065FF RID: 26111 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnEnter()
		{
		}

		// Token: 0x06006600 RID: 26112 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnExit()
		{
		}

		// Token: 0x04007562 RID: 30050
		private AIEntity entity;

		// Token: 0x04007563 RID: 30051
		private float angle;
	}
}
