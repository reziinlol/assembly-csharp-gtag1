using System;
using System.Collections;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor
{
	// Token: 0x02000F94 RID: 3988
	public class GRSquishVolume : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x06006368 RID: 25448 RVA: 0x000DCF37 File Offset: 0x000DB137
		private void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this);
		}

		// Token: 0x06006369 RID: 25449 RVA: 0x000DCF3F File Offset: 0x000DB13F
		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this);
		}

		// Token: 0x0600636A RID: 25450 RVA: 0x001FF9B4 File Offset: 0x001FDBB4
		private void Start()
		{
			this.SetCollider(false);
			this.SetTentacleColliders(true);
			this.moonBoss = base.GetComponentInParent<GREnemyBossMoon>();
			if (this.moonBoss != null && !this.moonBoss.squishVolumes.Contains(this))
			{
				this.moonBoss.squishVolumes.Add(this);
			}
		}

		// Token: 0x0600636B RID: 25451 RVA: 0x001FFA10 File Offset: 0x001FDC10
		public void SliceUpdate()
		{
			this.SetCollider(!this.overrideDisabled && base.transform.position.y < this.squishHeight && Vector3.Angle(-base.transform.forward, Quaternion.Euler(this.rotationOffset) * Vector3.down) < this.facingDownDegrees);
		}

		// Token: 0x0600636C RID: 25452 RVA: 0x001FFA78 File Offset: 0x001FDC78
		public void SetCollider(bool colliderEnabled)
		{
			this._collider.enabled = colliderEnabled;
		}

		// Token: 0x0600636D RID: 25453 RVA: 0x001FFA88 File Offset: 0x001FDC88
		private void OnTriggerEnter(Collider other)
		{
			GRPlayer y;
			if (!other.gameObject.TryGetComponentInParent(out y))
			{
				return;
			}
			if (GRPlayer.GetLocal() != y)
			{
				return;
			}
			if (this._reenableCoroutine != null)
			{
				return;
			}
			this.SetTentacleColliders(false);
			GTPlayer.Instance.DoLaunch(this.GetLaunchVector());
			this.moonBoss.HitPlayer(GRPlayer.GetLocal(), false);
			this._reenableCoroutine = base.StartCoroutine(this.ReenableCoroutine());
			this.moonBoss.SetSquishVolumeState(false);
		}

		// Token: 0x0600636E RID: 25454 RVA: 0x001FFB04 File Offset: 0x001FDD04
		private void SetTentacleColliders(bool enabled)
		{
			Collider[] collidersToDisable = this._collidersToDisable;
			for (int i = 0; i < collidersToDisable.Length; i++)
			{
				collidersToDisable[i].enabled = enabled;
			}
		}

		// Token: 0x0600636F RID: 25455 RVA: 0x001FFB2F File Offset: 0x001FDD2F
		private IEnumerator ReenableCoroutine()
		{
			yield return new WaitForSeconds(this._reenableDelay);
			this.SetTentacleColliders(true);
			this._reenableCoroutine = null;
			this.moonBoss.SetSquishVolumeState(true);
			yield break;
		}

		// Token: 0x06006370 RID: 25456 RVA: 0x001FFB40 File Offset: 0x001FDD40
		private Vector3 GetLaunchVector()
		{
			Vector3 position = GRPlayer.GetLocal().transform.position;
			Vector3 lhs = position - base.transform.position;
			Vector3 b = base.transform.position + base.transform.right * Vector3.Dot(lhs, base.transform.right);
			Vector3 normalized = (position - b).normalized;
			Vector3 normalized2 = new Vector3(normalized.x, 0f, normalized.y).normalized;
			float maxRadiansDelta = Random.Range(this._launchDeflectionDegrees / 2f, this._launchDeflectionDegrees) * 0.017453292f;
			Vector3 vector = Vector3.RotateTowards(normalized2, Vector3.up, maxRadiansDelta, 0f);
			return this._launchStrength * vector.normalized;
		}

		// Token: 0x04007215 RID: 29205
		[SerializeField]
		private Collider _collider;

		// Token: 0x04007216 RID: 29206
		[SerializeField]
		private Collider[] _collidersToDisable;

		// Token: 0x04007217 RID: 29207
		[SerializeField]
		private float _reenableDelay = 1f;

		// Token: 0x04007218 RID: 29208
		[SerializeField]
		private float _launchStrength = 8f;

		// Token: 0x04007219 RID: 29209
		[SerializeField]
		private float _launchDeflectionDegrees = 10f;

		// Token: 0x0400721A RID: 29210
		private Coroutine _reenableCoroutine;

		// Token: 0x0400721B RID: 29211
		private GREnemyBossMoon moonBoss;

		// Token: 0x0400721C RID: 29212
		public float squishHeight;

		// Token: 0x0400721D RID: 29213
		public Vector3 rotationOffset;

		// Token: 0x0400721E RID: 29214
		public float facingDownDegrees = 20f;

		// Token: 0x0400721F RID: 29215
		public bool overrideDisabled;
	}
}
