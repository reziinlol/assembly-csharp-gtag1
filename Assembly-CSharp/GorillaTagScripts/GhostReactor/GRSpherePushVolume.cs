using System;
using System.Collections;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor
{
	// Token: 0x02000F90 RID: 3984
	[RequireComponent(typeof(SphereCollider))]
	public class GRSpherePushVolume : MonoBehaviour
	{
		// Token: 0x06006353 RID: 25427 RVA: 0x001FF626 File Offset: 0x001FD826
		private void Awake()
		{
			this._collider = base.GetComponent<SphereCollider>();
			this._collider.enabled = false;
		}

		// Token: 0x06006354 RID: 25428 RVA: 0x001FF640 File Offset: 0x001FD840
		public void Trigger()
		{
			this._collider.enabled = true;
			base.StartCoroutine(this.DisableCoroutine());
		}

		// Token: 0x06006355 RID: 25429 RVA: 0x001FF65C File Offset: 0x001FD85C
		private void OnTriggerStay(Collider other)
		{
			if (this._localFlung)
			{
				return;
			}
			if (this._coroutine != null)
			{
				return;
			}
			GRPlayer y;
			if (!other.gameObject.TryGetComponentInParent(out y))
			{
				return;
			}
			if (GRPlayer.GetLocal() != y)
			{
				return;
			}
			this._coroutine = base.StartCoroutine(this.ActionCoroutine(other));
			this._collider.enabled = false;
		}

		// Token: 0x06006356 RID: 25430 RVA: 0x001FF6B8 File Offset: 0x001FD8B8
		private IEnumerator ActionCoroutine(Collider other)
		{
			yield return new WaitForSeconds(this._pushDelay);
			Vector3 velocity = this.CalculatePushVector(other);
			GTPlayer.Instance.DoLaunch(velocity);
			this._localFlung = true;
			yield return new WaitForSeconds(this._pushCooldown);
			this._localFlung = false;
			this._coroutine = null;
			yield break;
		}

		// Token: 0x06006357 RID: 25431 RVA: 0x001FF6CE File Offset: 0x001FD8CE
		private IEnumerator DisableCoroutine()
		{
			yield return new WaitForSeconds(this._disableAfter);
			this._collider.enabled = false;
			yield break;
		}

		// Token: 0x06006358 RID: 25432 RVA: 0x001FF6E0 File Offset: 0x001FD8E0
		private Vector3 CalculatePushVector(Collider other)
		{
			GRSpherePushVolume.PushKind pushKind = this._pushKind;
			Vector3 result;
			if (pushKind != GRSpherePushVolume.PushKind.Radial)
			{
				if (pushKind != GRSpherePushVolume.PushKind.UpAndOut)
				{
					throw new NotImplementedException();
				}
				result = this.CalculateUpAndOutPushVector(other);
			}
			else
			{
				result = this.CalculateRadialPushVector(other);
			}
			return result;
		}

		// Token: 0x06006359 RID: 25433 RVA: 0x001FF718 File Offset: 0x001FD918
		private Vector3 CalculateRadialPushVector(Collider other)
		{
			Vector3 vector = other.gameObject.transform.position - base.transform.position;
			float time = vector.magnitude / this._collider.radius;
			return this._pushScaling.Evaluate(time) * this._pushForce * vector.normalized;
		}

		// Token: 0x0600635A RID: 25434 RVA: 0x001FF77C File Offset: 0x001FD97C
		private Vector3 CalculateUpAndOutPushVector(Collider other)
		{
			Vector3 vector = new Vector3(other.gameObject.transform.position.x - base.transform.position.x, 0f, other.gameObject.transform.position.z - base.transform.position.z);
			float time = vector.magnitude / this._collider.radius;
			vector.Normalize();
			Vector3.RotateTowards(vector, Vector3.up, 0.7853982f, 0f);
			vector *= this._pushForce * this._pushScaling.Evaluate(time);
			return vector;
		}

		// Token: 0x04007202 RID: 29186
		[SerializeField]
		private GRSpherePushVolume.PushKind _pushKind;

		// Token: 0x04007203 RID: 29187
		[SerializeField]
		private float _pushDelay;

		// Token: 0x04007204 RID: 29188
		[SerializeField]
		private float _pushCooldown = 1f;

		// Token: 0x04007205 RID: 29189
		[SerializeField]
		private AnimationCurve _pushScaling = AnimationCurve.Constant(0f, 1f, 1f);

		// Token: 0x04007206 RID: 29190
		[SerializeField]
		private float _pushForce = 1f;

		// Token: 0x04007207 RID: 29191
		[SerializeField]
		private float _disableAfter = 3f;

		// Token: 0x04007208 RID: 29192
		private SphereCollider _collider;

		// Token: 0x04007209 RID: 29193
		private bool _localFlung;

		// Token: 0x0400720A RID: 29194
		private Coroutine _coroutine;

		// Token: 0x02000F91 RID: 3985
		public enum PushKind
		{
			// Token: 0x0400720C RID: 29196
			Radial,
			// Token: 0x0400720D RID: 29197
			UpAndOut
		}
	}
}
