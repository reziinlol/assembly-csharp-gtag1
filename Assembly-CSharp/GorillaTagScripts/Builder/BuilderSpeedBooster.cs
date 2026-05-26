using System;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FBE RID: 4030
	public class BuilderSpeedBooster : MonoBehaviour
	{
		// Token: 0x060064C7 RID: 25799 RVA: 0x00207C18 File Offset: 0x00205E18
		private void Awake()
		{
			this.volume = base.GetComponent<Collider>();
			this.windRenderer.enabled = false;
			this.boosting = false;
		}

		// Token: 0x060064C8 RID: 25800 RVA: 0x00207C3C File Offset: 0x00205E3C
		private void LateUpdate()
		{
			if (this.audioSource && this.audioSource != null && !this.audioSource.isPlaying && this.audioSource.enabled)
			{
				this.audioSource.enabled = false;
			}
		}

		// Token: 0x060064C9 RID: 25801 RVA: 0x00207C8C File Offset: 0x00205E8C
		private bool TriggerFilter(Collider other, out Rigidbody rb, out Transform xf)
		{
			rb = null;
			xf = null;
			if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
			{
				rb = GorillaTagger.Instance.GetComponent<Rigidbody>();
				xf = GorillaTagger.Instance.headCollider.GetComponent<Transform>();
			}
			return rb != null && xf != null;
		}

		// Token: 0x060064CA RID: 25802 RVA: 0x00207CEC File Offset: 0x00205EEC
		private void CheckTableZone()
		{
			if (this.hasCheckedZone)
			{
				return;
			}
			BuilderTable builderTable;
			if (BuilderTable.TryGetBuilderTableForZone(GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone, out builderTable))
			{
				this.ignoreMonkeScale = !builderTable.isTableMutable;
			}
			this.hasCheckedZone = true;
		}

		// Token: 0x060064CB RID: 25803 RVA: 0x00207D38 File Offset: 0x00205F38
		public void OnTriggerEnter(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			this.CheckTableZone();
			if (!this.ignoreMonkeScale && (double)GorillaTagger.Instance.offlineVRRig.scaleFactor > 0.99)
			{
				return;
			}
			this.positiveForce = (Vector3.Dot(base.transform.up, rigidbody.linearVelocity) > 0f);
			if (this.positiveForce)
			{
				this.windRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
			}
			else
			{
				this.windRenderer.transform.localRotation = Quaternion.Euler(0f, 180f, -90f);
			}
			this.windRenderer.enabled = true;
			this.enterPos = transform.position;
			if (!this.boosting)
			{
				this.boosting = true;
				this.enterTime = Time.timeAsDouble;
			}
		}

		// Token: 0x060064CC RID: 25804 RVA: 0x00207E28 File Offset: 0x00206028
		public void OnTriggerExit(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			this.windRenderer.enabled = false;
			this.CheckTableZone();
			if (!this.ignoreMonkeScale && (double)GorillaTagger.Instance.offlineVRRig.scaleFactor > 0.99)
			{
				return;
			}
			if (this.boosting && this.audioSource)
			{
				this.audioSource.enabled = true;
				this.audioSource.Stop();
				this.audioSource.GTPlayOneShot(this.exitClip, 1f);
			}
			this.boosting = false;
		}

		// Token: 0x060064CD RID: 25805 RVA: 0x00207EC8 File Offset: 0x002060C8
		public void OnTriggerStay(Collider other)
		{
			if (!this.boosting)
			{
				return;
			}
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			if (!this.ignoreMonkeScale && (double)GorillaTagger.Instance.offlineVRRig.scaleFactor > 0.99)
			{
				this.OnTriggerExit(other);
				return;
			}
			if (Time.timeAsDouble > this.enterTime + (double)this.maxBoostDuration)
			{
				this.OnTriggerExit(other);
				return;
			}
			if (this.disableGrip)
			{
				GTPlayer.Instance.SetMaximumSlipThisFrame();
			}
			SizeManager sizeManager = null;
			if (this.scaleWithSize)
			{
				sizeManager = rigidbody.GetComponent<SizeManager>();
			}
			Vector3 vector = rigidbody.linearVelocity;
			if (this.scaleWithSize && sizeManager)
			{
				vector /= sizeManager.currentScale;
			}
			Vector3 b = Vector3.Dot(transform.position - base.transform.position, base.transform.up) * base.transform.up;
			Vector3 a = base.transform.position + b - transform.position;
			float num = a.magnitude + 0.0001f;
			Vector3 vector2 = a / num;
			float num2 = Vector3.Dot(vector, vector2);
			float d = this.accel;
			if (this.maxDepth > -1f)
			{
				float num3 = Vector3.Dot(transform.position - this.enterPos, vector2);
				float num4 = this.maxDepth - num3;
				float b2 = 0f;
				if (num4 > 0.0001f)
				{
					b2 = num2 * num2 / num4;
				}
				d = Mathf.Max(this.accel, b2);
			}
			float deltaTime = Time.deltaTime;
			Vector3 vector3 = base.transform.up * d * deltaTime;
			if (!this.positiveForce)
			{
				vector3 *= -1f;
			}
			vector += vector3;
			if ((double)Vector3.Dot(vector3, Vector3.down) <= 0.1)
			{
				vector += Vector3.up * this.addedWorldUpVelocity * deltaTime;
			}
			Vector3 a2 = Mathf.Min(Vector3.Dot(vector, base.transform.up), this.maxSpeed) * base.transform.up;
			Vector3 a3 = Vector3.Dot(vector, base.transform.right) * base.transform.right;
			Vector3 a4 = Vector3.Dot(vector, base.transform.forward) * base.transform.forward;
			float d2 = 1f;
			float d3 = 1f;
			if (this.dampenLateralVelocity)
			{
				d2 = 1f - this.dampenXVelPerc * 0.01f * deltaTime;
				d3 = 1f - this.dampenZVelPerc * 0.01f * deltaTime;
			}
			vector = a2 + d2 * a3 + d3 * a4;
			if (this.applyPullToCenterAcceleration && this.pullToCenterAccel > 0f && this.pullToCenterMaxSpeed > 0f)
			{
				vector -= num2 * vector2;
				if (num > this.pullTOCenterMinDistance)
				{
					num2 += this.pullToCenterAccel * deltaTime;
					float b3 = Mathf.Min(this.pullToCenterMaxSpeed, num / deltaTime);
					num2 = Mathf.Min(num2, b3);
				}
				else
				{
					num2 = 0f;
				}
				vector += num2 * vector2;
				if (vector.magnitude > 0.0001f)
				{
					Vector3 vector4 = Vector3.Cross(base.transform.up, vector2);
					float magnitude = vector4.magnitude;
					if (magnitude > 0.0001f)
					{
						vector4 /= magnitude;
						num2 = Vector3.Dot(vector, vector4);
						vector -= num2 * vector4;
						num2 -= this.pullToCenterAccel * deltaTime;
						num2 = Mathf.Max(0f, num2);
						vector += num2 * vector4;
					}
				}
			}
			if (this.scaleWithSize && sizeManager)
			{
				vector *= sizeManager.currentScale;
			}
			rigidbody.linearVelocity = vector;
		}

		// Token: 0x060064CE RID: 25806 RVA: 0x002082DC File Offset: 0x002064DC
		public void OnDrawGizmosSelected()
		{
			base.GetComponents<Collider>();
			Gizmos.color = Color.magenta;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(this.pullTOCenterMinDistance / base.transform.lossyScale.x, 1f, this.pullTOCenterMinDistance / base.transform.lossyScale.z));
		}

		// Token: 0x040073B7 RID: 29623
		[SerializeField]
		public bool scaleWithSize = true;

		// Token: 0x040073B8 RID: 29624
		[SerializeField]
		private float accel;

		// Token: 0x040073B9 RID: 29625
		[SerializeField]
		private float maxDepth = -1f;

		// Token: 0x040073BA RID: 29626
		[SerializeField]
		private float maxSpeed;

		// Token: 0x040073BB RID: 29627
		[SerializeField]
		private bool disableGrip;

		// Token: 0x040073BC RID: 29628
		[SerializeField]
		private bool dampenLateralVelocity = true;

		// Token: 0x040073BD RID: 29629
		[SerializeField]
		private float dampenXVelPerc;

		// Token: 0x040073BE RID: 29630
		[SerializeField]
		private float dampenZVelPerc;

		// Token: 0x040073BF RID: 29631
		[SerializeField]
		private bool applyPullToCenterAcceleration = true;

		// Token: 0x040073C0 RID: 29632
		[SerializeField]
		private float pullToCenterAccel;

		// Token: 0x040073C1 RID: 29633
		[SerializeField]
		private float pullToCenterMaxSpeed;

		// Token: 0x040073C2 RID: 29634
		[SerializeField]
		private float pullTOCenterMinDistance = 0.1f;

		// Token: 0x040073C3 RID: 29635
		[SerializeField]
		private float addedWorldUpVelocity = 10f;

		// Token: 0x040073C4 RID: 29636
		[SerializeField]
		private float maxBoostDuration = 2f;

		// Token: 0x040073C5 RID: 29637
		private bool boosting;

		// Token: 0x040073C6 RID: 29638
		private double enterTime;

		// Token: 0x040073C7 RID: 29639
		private Collider volume;

		// Token: 0x040073C8 RID: 29640
		public AudioClip exitClip;

		// Token: 0x040073C9 RID: 29641
		public AudioSource audioSource;

		// Token: 0x040073CA RID: 29642
		public MeshRenderer windRenderer;

		// Token: 0x040073CB RID: 29643
		private Vector3 enterPos;

		// Token: 0x040073CC RID: 29644
		private bool positiveForce = true;

		// Token: 0x040073CD RID: 29645
		private bool ignoreMonkeScale;

		// Token: 0x040073CE RID: 29646
		private bool hasCheckedZone;
	}
}
