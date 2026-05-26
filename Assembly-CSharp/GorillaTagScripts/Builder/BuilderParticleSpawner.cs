using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FA1 RID: 4001
	public class BuilderParticleSpawner : MonoBehaviour
	{
		// Token: 0x060063CA RID: 25546 RVA: 0x00201D2C File Offset: 0x001FFF2C
		private void Start()
		{
			this.spawnTrigger.onTriggerFirstEntered += this.OnEnter;
			this.spawnTrigger.onTriggerLastExited += this.OnExit;
		}

		// Token: 0x060063CB RID: 25547 RVA: 0x00201D5C File Offset: 0x001FFF5C
		private void OnDestroy()
		{
			if (this.spawnTrigger != null)
			{
				this.spawnTrigger.onTriggerFirstEntered -= this.OnEnter;
				this.spawnTrigger.onTriggerLastExited -= this.OnExit;
			}
		}

		// Token: 0x060063CC RID: 25548 RVA: 0x00201D9C File Offset: 0x001FFF9C
		public void TrySpawning()
		{
			if (Time.time > this.lastSpawnTime + this.cooldown)
			{
				this.lastSpawnTime = Time.time;
				ObjectPools.instance.Instantiate(this.prefab, this.spawnLocation.position, this.spawnLocation.rotation, this.myPiece.GetScale(), true);
			}
		}

		// Token: 0x060063CD RID: 25549 RVA: 0x00201DFB File Offset: 0x001FFFFB
		private void OnEnter()
		{
			if (this.spawnOnEnter)
			{
				this.TrySpawning();
			}
		}

		// Token: 0x060063CE RID: 25550 RVA: 0x00201E0B File Offset: 0x0020000B
		private void OnExit()
		{
			if (this.spawnOnExit)
			{
				this.TrySpawning();
			}
		}

		// Token: 0x04007272 RID: 29298
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x04007273 RID: 29299
		public GameObject prefab;

		// Token: 0x04007274 RID: 29300
		public float cooldown = 0.1f;

		// Token: 0x04007275 RID: 29301
		private float lastSpawnTime;

		// Token: 0x04007276 RID: 29302
		[SerializeField]
		private BuilderSmallMonkeTrigger spawnTrigger;

		// Token: 0x04007277 RID: 29303
		[SerializeField]
		private bool spawnOnEnter = true;

		// Token: 0x04007278 RID: 29304
		[SerializeField]
		private bool spawnOnExit;

		// Token: 0x04007279 RID: 29305
		[SerializeField]
		private Transform spawnLocation;
	}
}
