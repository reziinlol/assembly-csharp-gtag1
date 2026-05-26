using System;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace GorillaLocomotion
{
	// Token: 0x020010E7 RID: 4327
	public sealed class Playspace : MonoBehaviour
	{
		// Token: 0x06006CFE RID: 27902 RVA: 0x002380D3 File Offset: 0x002362D3
		private void Awake()
		{
			this._sqrSphereRadius = this._sphereRadius * this._sphereRadius;
			this._sqrSnapToThreshold = this._snapToThreshold * this._snapToThreshold;
		}

		// Token: 0x06006CFF RID: 27903 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void Start()
		{
		}

		// Token: 0x06006D00 RID: 27904 RVA: 0x002380FC File Offset: 0x002362FC
		private void Update()
		{
			Vector3 b = this._localGorillaHead.transform.position - base.transform.position;
			float sqrMagnitude = b.sqrMagnitude;
			if (GTPlayer.Instance.enableHoverMode || GTPlayer.Instance.isClimbing || b.sqrMagnitude > this._sqrSnapToThreshold)
			{
				base.transform.position = this._localGorillaHead.transform.position;
				return;
			}
			Vector3 normalized = b.normalized;
			b = this.GetChaseSpeed() * Time.deltaTime * normalized;
			base.transform.position = ((b.sqrMagnitude > sqrMagnitude) ? this._localGorillaHead.transform.position : (base.transform.position + b));
			if ((this._localGorillaHead.transform.position - base.transform.position).sqrMagnitude > this._sqrSphereRadius)
			{
				this._localGorillaHead.transform.position = base.transform.position + this._sphereRadius * normalized;
			}
		}

		// Token: 0x06006D01 RID: 27905 RVA: 0x00238227 File Offset: 0x00236427
		private float GetChaseSpeed()
		{
			return this._defaultChaseSpeed;
		}

		// Token: 0x06006D02 RID: 27906 RVA: 0x0023822F File Offset: 0x0023642F
		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireSphere(base.transform.position, this._sphereRadius);
		}

		// Token: 0x04007DA0 RID: 32160
		[SerializeField]
		private GameObject _localGorillaHead;

		// Token: 0x04007DA1 RID: 32161
		[SerializeField]
		private float _sphereRadius;

		// Token: 0x04007DA2 RID: 32162
		private float _sqrSphereRadius;

		// Token: 0x04007DA3 RID: 32163
		[SerializeField]
		private float _defaultChaseSpeed;

		// Token: 0x04007DA4 RID: 32164
		[SerializeField]
		private float _snapToThreshold;

		// Token: 0x04007DA5 RID: 32165
		private float _sqrSnapToThreshold;

		// Token: 0x04007DA6 RID: 32166
		[SerializeField]
		private GTPlayer m_gtPlayer;

		// Token: 0x04007DA7 RID: 32167
		[SerializeField]
		private XROrigin m_xrOrigin;

		// Token: 0x04007DA8 RID: 32168
		private Transform m_xrBody;
	}
}
