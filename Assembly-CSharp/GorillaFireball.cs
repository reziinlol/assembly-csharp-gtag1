using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000A23 RID: 2595
public class GorillaFireball : GorillaThrowable, IPunInstantiateMagicCallback
{
	// Token: 0x06004263 RID: 16995 RVA: 0x0016293B File Offset: 0x00160B3B
	public override void Start()
	{
		base.Start();
		this.canExplode = false;
		this.explosionStartTime = 0f;
	}

	// Token: 0x06004264 RID: 16996 RVA: 0x00162958 File Offset: 0x00160B58
	private void Update()
	{
		if (this.explosionStartTime != 0f)
		{
			float num = (Time.time - this.explosionStartTime) / this.totalExplosionTime * (this.maxExplosionScale - 0.25f) + 0.25f;
			base.gameObject.transform.localScale = new Vector3(num, num, num);
			if (base.photonView.IsMine && Time.time > this.explosionStartTime + this.totalExplosionTime)
			{
				PhotonNetwork.Destroy(PhotonView.Get(this));
			}
		}
	}

	// Token: 0x06004265 RID: 16997 RVA: 0x001629E0 File Offset: 0x00160BE0
	public override void LateUpdate()
	{
		base.LateUpdate();
		if (this.rigidbody.useGravity)
		{
			this.rigidbody.AddForce(Physics.gravity * -this.gravityStrength * this.rigidbody.mass);
		}
	}

	// Token: 0x06004266 RID: 16998 RVA: 0x00162A2C File Offset: 0x00160C2C
	public override void ThrowThisThingo()
	{
		base.ThrowThisThingo();
		this.canExplode = true;
	}

	// Token: 0x06004267 RID: 16999 RVA: 0x00162A3B File Offset: 0x00160C3B
	private new void OnCollisionEnter(Collision collision)
	{
		if (base.photonView.IsMine && this.canExplode)
		{
			base.photonView.RPC("Explode", RpcTarget.All, null);
		}
	}

	// Token: 0x06004268 RID: 17000 RVA: 0x00162A64 File Offset: 0x00160C64
	public void LocalExplode()
	{
		this.rigidbody.isKinematic = true;
		this.canExplode = false;
		this.explosionStartTime = Time.time;
	}

	// Token: 0x06004269 RID: 17001 RVA: 0x00162A84 File Offset: 0x00160C84
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (base.photonView.IsMine)
		{
			if ((bool)base.photonView.InstantiationData[0])
			{
				base.transform.parent = GorillaPlaySpace.Instance.myVRRig.leftHandTransform;
				return;
			}
			base.transform.parent = GorillaPlaySpace.Instance.myVRRig.rightHandTransform;
		}
	}

	// Token: 0x0600426A RID: 17002 RVA: 0x00162AE7 File Offset: 0x00160CE7
	public void Explode()
	{
		this.LocalExplode();
	}

	// Token: 0x04005435 RID: 21557
	public float maxExplosionScale;

	// Token: 0x04005436 RID: 21558
	public float totalExplosionTime;

	// Token: 0x04005437 RID: 21559
	public float gravityStrength;

	// Token: 0x04005438 RID: 21560
	private bool canExplode;

	// Token: 0x04005439 RID: 21561
	private float explosionStartTime;
}
