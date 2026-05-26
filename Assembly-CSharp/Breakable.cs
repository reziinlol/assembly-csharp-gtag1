using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000604 RID: 1540
public class Breakable : MonoBehaviour
{
	// Token: 0x0600266C RID: 9836 RVA: 0x000CB62C File Offset: 0x000C982C
	private void Awake()
	{
		this._breakSignal.OnSignal += this.BreakRPC;
		if (this._rigidbody.IsNotNull())
		{
			this.m_useGravity = this._rigidbody.useGravity;
		}
	}

	// Token: 0x0600266D RID: 9837 RVA: 0x000CB664 File Offset: 0x000C9864
	private void BreakRPC(int owner, PhotonSignalInfo info)
	{
		VRRig vrrig = base.GetComponent<OwnerRig>();
		if (vrrig == null)
		{
			return;
		}
		if (vrrig.OwningNetPlayer.ActorNumber != owner)
		{
			return;
		}
		if (!this.m_spamChecker.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.OnBreak(true, false);
	}

	// Token: 0x0600266E RID: 9838 RVA: 0x000CB6B4 File Offset: 0x000C98B4
	private void Setup()
	{
		if (this._collider == null)
		{
			SphereCollider collider;
			this.GetOrAddComponent(out collider);
			this._collider = collider;
		}
		this._collider.enabled = true;
		if (this._rigidbody == null)
		{
			this.GetOrAddComponent(out this._rigidbody);
		}
		this._rigidbody.isKinematic = false;
		this._rigidbody.useGravity = false;
		this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		this.UpdatePhysMasks();
		if (this.rendererRoot == null)
		{
			this._renderers = base.GetComponentsInChildren<Renderer>();
			return;
		}
		this._renderers = this.rendererRoot.GetComponentsInChildren<Renderer>();
	}

	// Token: 0x0600266F RID: 9839 RVA: 0x000CB75B File Offset: 0x000C995B
	private void OnCollisionEnter(Collision col)
	{
		this.OnBreak(true, true);
	}

	// Token: 0x06002670 RID: 9840 RVA: 0x000CB75B File Offset: 0x000C995B
	private void OnCollisionStay(Collision col)
	{
		this.OnBreak(true, true);
	}

	// Token: 0x06002671 RID: 9841 RVA: 0x000CB75B File Offset: 0x000C995B
	private void OnTriggerEnter(Collider col)
	{
		this.OnBreak(true, true);
	}

	// Token: 0x06002672 RID: 9842 RVA: 0x000CB75B File Offset: 0x000C995B
	private void OnTriggerStay(Collider col)
	{
		this.OnBreak(true, true);
	}

	// Token: 0x06002673 RID: 9843 RVA: 0x000CB765 File Offset: 0x000C9965
	private void OnEnable()
	{
		this._breakSignal.Enable();
		this._broken = false;
		this.OnSpawn(true);
	}

	// Token: 0x06002674 RID: 9844 RVA: 0x000CB780 File Offset: 0x000C9980
	private void OnDisable()
	{
		this._breakSignal.Disable();
		this._broken = false;
		this.OnReset(false);
		this.ShowRenderers(false);
	}

	// Token: 0x06002675 RID: 9845 RVA: 0x000CB75B File Offset: 0x000C995B
	public void Break()
	{
		this.OnBreak(true, true);
	}

	// Token: 0x06002676 RID: 9846 RVA: 0x000CB7A2 File Offset: 0x000C99A2
	public void Reset()
	{
		this.OnReset(true);
	}

	// Token: 0x06002677 RID: 9847 RVA: 0x000CB7AC File Offset: 0x000C99AC
	protected virtual void ShowRenderers(bool visible)
	{
		if (this._renderers.IsNullOrEmpty<Renderer>())
		{
			return;
		}
		for (int i = 0; i < this._renderers.Length; i++)
		{
			Renderer renderer = this._renderers[i];
			if (renderer)
			{
				renderer.forceRenderingOff = !visible;
			}
		}
	}

	// Token: 0x06002678 RID: 9848 RVA: 0x000CB7F8 File Offset: 0x000C99F8
	protected virtual void OnReset(bool callback = true)
	{
		if (this._breakEffect && this._breakEffect.isPlaying)
		{
			this._breakEffect.Stop();
		}
		this.ShowRenderers(true);
		this._broken = false;
		if (callback)
		{
			UnityEvent<Breakable> unityEvent = this.onReset;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}
	}

	// Token: 0x06002679 RID: 9849 RVA: 0x000CB84C File Offset: 0x000C9A4C
	protected virtual void OnSpawn(bool callback = true)
	{
		this.startTime = Time.time;
		this.endTime = this.startTime + this.canBreakDelay;
		this.ShowRenderers(true);
		if (this._rigidbody.IsNotNull())
		{
			this._rigidbody.detectCollisions = true;
			this._rigidbody.useGravity = this.m_useGravity;
		}
		if (callback)
		{
			UnityEvent<Breakable> unityEvent = this.onSpawn;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}
	}

	// Token: 0x0600267A RID: 9850 RVA: 0x000CB8BC File Offset: 0x000C9ABC
	protected virtual void OnBreak(bool callback = true, bool signal = true)
	{
		if (this._broken)
		{
			return;
		}
		if (Time.time < this.endTime)
		{
			return;
		}
		if (this._breakEffect)
		{
			if (this._breakEffect.isPlaying)
			{
				this._breakEffect.Stop();
			}
			this._breakEffect.Play();
		}
		if (signal && PhotonNetwork.InRoom)
		{
			VRRig vrrig = base.GetComponent<OwnerRig>();
			if (vrrig != null)
			{
				this._breakSignal.Raise(vrrig.OwningNetPlayer.ActorNumber);
			}
		}
		this.ShowRenderers(false);
		if (this._rigidbody.IsNotNull())
		{
			this._rigidbody.detectCollisions = false;
			this._rigidbody.useGravity = false;
		}
		this._broken = true;
		if (callback)
		{
			UnityEvent<Breakable> unityEvent = this.onBreak;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}
	}

	// Token: 0x0600267B RID: 9851 RVA: 0x000CB98C File Offset: 0x000C9B8C
	private void UpdatePhysMasks()
	{
		int physicsMask = (int)this._physicsMask;
		if (this._collider)
		{
			this._collider.includeLayers = physicsMask;
			this._collider.excludeLayers = ~physicsMask;
		}
		if (this._rigidbody)
		{
			this._rigidbody.includeLayers = physicsMask;
			this._rigidbody.excludeLayers = ~physicsMask;
		}
	}

	// Token: 0x040031C8 RID: 12744
	[SerializeField]
	private Collider _collider;

	// Token: 0x040031C9 RID: 12745
	[SerializeField]
	private Rigidbody _rigidbody;

	// Token: 0x040031CA RID: 12746
	[SerializeField]
	private GameObject rendererRoot;

	// Token: 0x040031CB RID: 12747
	[SerializeField]
	private Renderer[] _renderers = new Renderer[0];

	// Token: 0x040031CC RID: 12748
	[Space]
	[SerializeField]
	private ParticleSystem _breakEffect;

	// Token: 0x040031CD RID: 12749
	[SerializeField]
	private UnityLayerMask _physicsMask = UnityLayerMask.GorillaHand;

	// Token: 0x040031CE RID: 12750
	public UnityEvent<Breakable> onSpawn;

	// Token: 0x040031CF RID: 12751
	public UnityEvent<Breakable> onBreak;

	// Token: 0x040031D0 RID: 12752
	public UnityEvent<Breakable> onReset;

	// Token: 0x040031D1 RID: 12753
	public float canBreakDelay = 1f;

	// Token: 0x040031D2 RID: 12754
	[SerializeField]
	private PhotonSignal<int> _breakSignal = "_breakSignal";

	// Token: 0x040031D3 RID: 12755
	[SerializeField]
	private CallLimiter m_spamChecker = new CallLimiter(2, 1f, 0.5f);

	// Token: 0x040031D4 RID: 12756
	[Space]
	[NonSerialized]
	private bool _broken;

	// Token: 0x040031D5 RID: 12757
	private bool m_useGravity = true;

	// Token: 0x040031D6 RID: 12758
	private float startTime;

	// Token: 0x040031D7 RID: 12759
	private float endTime;
}
