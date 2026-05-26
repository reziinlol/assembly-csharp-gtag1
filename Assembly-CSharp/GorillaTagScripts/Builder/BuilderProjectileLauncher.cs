using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FB3 RID: 4019
	public class BuilderProjectileLauncher : MonoBehaviour, IBuilderPieceFunctional, IBuilderPieceComponent
	{
		// Token: 0x06006468 RID: 25704 RVA: 0x00205C18 File Offset: 0x00203E18
		private void LaunchProjectile(int timeStamp)
		{
			if (Time.time > this.lastFireTime + this.fireCooldown)
			{
				this.lastFireTime = Time.time;
				int hash = PoolUtils.GameObjHashCode(this.projectilePrefab);
				try
				{
					GameObject gameObject = ObjectPools.instance.Instantiate(hash, true);
					this.projectileScale = this.myPiece.GetScale();
					gameObject.transform.localScale = Vector3.one * this.projectileScale;
					BuilderProjectile component = gameObject.GetComponent<BuilderProjectile>();
					int num = HashCode.Combine<int, int>(this.myPiece.pieceId, timeStamp);
					if (this.allProjectiles.ContainsKey(num))
					{
						this.allProjectiles.Remove(num);
					}
					this.allProjectiles.Add(num, component);
					SlingshotProjectile.AOEKnockbackConfig value = new SlingshotProjectile.AOEKnockbackConfig
					{
						aeoOuterRadius = this.knockbackConfig.aeoOuterRadius * this.projectileScale,
						aeoInnerRadius = this.knockbackConfig.aeoInnerRadius * this.projectileScale,
						applyAOEKnockback = this.knockbackConfig.applyAOEKnockback,
						impactVelocityThreshold = this.knockbackConfig.impactVelocityThreshold * this.projectileScale,
						knockbackVelocity = this.knockbackConfig.knockbackVelocity * this.projectileScale,
						playerProximityEffect = this.knockbackConfig.playerProximityEffect
					};
					component.aoeKnockbackConfig = new SlingshotProjectile.AOEKnockbackConfig?(value);
					component.gravityMultiplier = this.gravityMultiplier;
					component.Launch(this.launchPosition.position, this.launchVelocity * this.projectileScale * this.launchPosition.up, this, num, this.projectileScale, timeStamp);
					if (this.launchSound != null && this.launchSound.clip != null)
					{
						this.launchSound.Play();
					}
				}
				catch (Exception value2)
				{
					Console.WriteLine(value2);
					throw;
				}
			}
		}

		// Token: 0x06006469 RID: 25705 RVA: 0x00205E00 File Offset: 0x00204000
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if ((BuilderProjectileLauncher.FunctionalState)newState == this.currentState)
			{
				return;
			}
			this.currentState = (BuilderProjectileLauncher.FunctionalState)newState;
			if (newState == 1)
			{
				this.LaunchProjectile(timeStamp);
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x0600646A RID: 25706 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
		}

		// Token: 0x0600646B RID: 25707 RVA: 0x00204ACF File Offset: 0x00202CCF
		public bool IsStateValid(byte state)
		{
			return state <= 1;
		}

		// Token: 0x0600646C RID: 25708 RVA: 0x00205E54 File Offset: 0x00204054
		public void FunctionalPieceUpdate()
		{
			for (int i = this.launchedProjectiles.Count - 1; i >= 0; i--)
			{
				this.launchedProjectiles[i].UpdateProjectile();
			}
			if (PhotonNetwork.IsMasterClient && this.lastFireTime + this.fireCooldown < Time.time)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x0600646D RID: 25709 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		// Token: 0x0600646E RID: 25710 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPieceDestroy()
		{
		}

		// Token: 0x0600646F RID: 25711 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06006470 RID: 25712 RVA: 0x00205ED5 File Offset: 0x002040D5
		public void OnPieceActivate()
		{
			this.myPiece.GetTable().RegisterFunctionalPiece(this);
		}

		// Token: 0x06006471 RID: 25713 RVA: 0x00205EE8 File Offset: 0x002040E8
		public void OnPieceDeactivate()
		{
			this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			for (int i = this.launchedProjectiles.Count - 1; i >= 0; i--)
			{
				this.launchedProjectiles[i].Deactivate();
			}
		}

		// Token: 0x06006472 RID: 25714 RVA: 0x00205F2F File Offset: 0x0020412F
		public void RegisterProjectile(BuilderProjectile projectile)
		{
			this.launchedProjectiles.Add(projectile);
		}

		// Token: 0x06006473 RID: 25715 RVA: 0x00205F3D File Offset: 0x0020413D
		public void UnRegisterProjectile(BuilderProjectile projectile)
		{
			this.launchedProjectiles.Remove(projectile);
			this.allProjectiles.Remove(projectile.projectileId);
		}

		// Token: 0x04007332 RID: 29490
		private List<BuilderProjectile> launchedProjectiles = new List<BuilderProjectile>();

		// Token: 0x04007333 RID: 29491
		[SerializeField]
		protected BuilderPiece myPiece;

		// Token: 0x04007334 RID: 29492
		[SerializeField]
		protected float fireCooldown = 2f;

		// Token: 0x04007335 RID: 29493
		[Tooltip("launch in Y direction")]
		[SerializeField]
		private Transform launchPosition;

		// Token: 0x04007336 RID: 29494
		[SerializeField]
		private float launchVelocity;

		// Token: 0x04007337 RID: 29495
		[SerializeField]
		private AudioSource launchSound;

		// Token: 0x04007338 RID: 29496
		[SerializeField]
		protected GameObject projectilePrefab;

		// Token: 0x04007339 RID: 29497
		protected float projectileScale = 0.06f;

		// Token: 0x0400733A RID: 29498
		[SerializeField]
		protected float gravityMultiplier = 1f;

		// Token: 0x0400733B RID: 29499
		public SlingshotProjectile.AOEKnockbackConfig knockbackConfig;

		// Token: 0x0400733C RID: 29500
		private float lastFireTime;

		// Token: 0x0400733D RID: 29501
		private BuilderProjectileLauncher.FunctionalState currentState;

		// Token: 0x0400733E RID: 29502
		private Dictionary<int, BuilderProjectile> allProjectiles = new Dictionary<int, BuilderProjectile>();

		// Token: 0x02000FB4 RID: 4020
		private enum FunctionalState
		{
			// Token: 0x04007340 RID: 29504
			Idle,
			// Token: 0x04007341 RID: 29505
			Fire
		}
	}
}
