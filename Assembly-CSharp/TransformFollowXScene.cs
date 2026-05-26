using System;
using UnityEngine;

// Token: 0x020009EF RID: 2543
public class TransformFollowXScene : MonoBehaviour
{
	// Token: 0x06004111 RID: 16657 RVA: 0x0015BC9B File Offset: 0x00159E9B
	private void Awake()
	{
		this.prevPos = base.transform.position;
	}

	// Token: 0x06004112 RID: 16658 RVA: 0x0015BCAE File Offset: 0x00159EAE
	private void Start()
	{
		this.refToFollow.TryResolve<Transform>(out this.transformToFollow);
	}

	// Token: 0x06004113 RID: 16659 RVA: 0x0015BCC4 File Offset: 0x00159EC4
	private void LateUpdate()
	{
		this.prevPos = base.transform.position;
		base.transform.rotation = this.transformToFollow.rotation;
		base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
	}

	// Token: 0x040051BF RID: 20927
	public XSceneRef refToFollow;

	// Token: 0x040051C0 RID: 20928
	private Transform transformToFollow;

	// Token: 0x040051C1 RID: 20929
	public Vector3 offset;

	// Token: 0x040051C2 RID: 20930
	public Vector3 prevPos;
}
