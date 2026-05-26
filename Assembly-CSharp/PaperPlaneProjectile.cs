using System;
using GorillaTag.Reactions;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000588 RID: 1416
public class PaperPlaneProjectile : MonoBehaviour
{
	// Token: 0x1400004F RID: 79
	// (add) Token: 0x060023DA RID: 9178 RVA: 0x000C0994 File Offset: 0x000BEB94
	// (remove) Token: 0x060023DB RID: 9179 RVA: 0x000C09CC File Offset: 0x000BEBCC
	public event PaperPlaneProjectile.PaperPlaneHit OnHit;

	// Token: 0x170003C5 RID: 965
	// (get) Token: 0x060023DC RID: 9180 RVA: 0x000C0A01 File Offset: 0x000BEC01
	public new Transform transform
	{
		get
		{
			return this._tCached;
		}
	}

	// Token: 0x170003C6 RID: 966
	// (get) Token: 0x060023DD RID: 9181 RVA: 0x000C0A09 File Offset: 0x000BEC09
	public VRRig MyRig
	{
		get
		{
			return this.myRig;
		}
	}

	// Token: 0x060023DE RID: 9182 RVA: 0x000C0A11 File Offset: 0x000BEC11
	private void Awake()
	{
		this._tCached = base.transform;
		this.spawnWorldEffects = base.GetComponent<SpawnWorldEffects>();
	}

	// Token: 0x060023DF RID: 9183 RVA: 0x000C0A2B File Offset: 0x000BEC2B
	private void Start()
	{
		this.ResetProjectile();
	}

	// Token: 0x060023E0 RID: 9184 RVA: 0x000C0A33 File Offset: 0x000BEC33
	public void ResetProjectile()
	{
		this._timeElapsed = 0f;
		this.flyingObject.SetActive(true);
		this.crashingObject.SetActive(false);
	}

	// Token: 0x060023E1 RID: 9185 RVA: 0x000C0A58 File Offset: 0x000BEC58
	internal void SetTransferrableState(TransferrableObject.SyncOptions syncType, int state)
	{
		if (!this.useTransferrableObjectState)
		{
			return;
		}
		if (syncType != TransferrableObject.SyncOptions.Bool)
		{
			if (syncType != TransferrableObject.SyncOptions.Int)
			{
				return;
			}
			UnityEvent<int> onItemStateIntChanged = this.OnItemStateIntChanged;
			if (onItemStateIntChanged == null)
			{
				return;
			}
			onItemStateIntChanged.Invoke(state);
			return;
		}
		else
		{
			bool flag = (state & 1) != 0;
			bool flag2 = (state & 2) != 0;
			bool flag3 = (state & 4) != 0;
			bool flag4 = (state & 8) != 0;
			if (flag)
			{
				UnityEvent onItemStateBoolATrue = this.OnItemStateBoolATrue;
				if (onItemStateBoolATrue != null)
				{
					onItemStateBoolATrue.Invoke();
				}
			}
			else
			{
				UnityEvent onItemStateBoolAFalse = this.OnItemStateBoolAFalse;
				if (onItemStateBoolAFalse != null)
				{
					onItemStateBoolAFalse.Invoke();
				}
			}
			if (flag2)
			{
				UnityEvent onItemStateBoolBTrue = this.OnItemStateBoolBTrue;
				if (onItemStateBoolBTrue != null)
				{
					onItemStateBoolBTrue.Invoke();
				}
			}
			else
			{
				UnityEvent onItemStateBoolBFalse = this.OnItemStateBoolBFalse;
				if (onItemStateBoolBFalse != null)
				{
					onItemStateBoolBFalse.Invoke();
				}
			}
			if (flag3)
			{
				UnityEvent onItemStateBoolCTrue = this.OnItemStateBoolCTrue;
				if (onItemStateBoolCTrue != null)
				{
					onItemStateBoolCTrue.Invoke();
				}
			}
			else
			{
				UnityEvent onItemStateBoolCFalse = this.OnItemStateBoolCFalse;
				if (onItemStateBoolCFalse != null)
				{
					onItemStateBoolCFalse.Invoke();
				}
			}
			if (flag4)
			{
				UnityEvent onItemStateBoolDTrue = this.OnItemStateBoolDTrue;
				if (onItemStateBoolDTrue == null)
				{
					return;
				}
				onItemStateBoolDTrue.Invoke();
				return;
			}
			else
			{
				UnityEvent onItemStateBoolDFalse = this.OnItemStateBoolDFalse;
				if (onItemStateBoolDFalse == null)
				{
					return;
				}
				onItemStateBoolDFalse.Invoke();
				return;
			}
		}
	}

	// Token: 0x060023E2 RID: 9186 RVA: 0x000C0B40 File Offset: 0x000BED40
	public void Launch(Vector3 startPos, Quaternion startRot, Vector3 vel)
	{
		base.gameObject.SetActive(true);
		this.ResetProjectile();
		this.transform.position = startPos;
		if (this.enableRotation)
		{
			this.transform.rotation = startRot;
		}
		else
		{
			this.transform.LookAt(this.transform.position + vel.normalized);
		}
		this._direction = vel.normalized;
		this._speed = Mathf.Clamp(this.speedCurve.Evaluate(vel.magnitude), this.minSpeed, this.maxSpeed);
		this._stopped = false;
		this.scaleFactor = 0.7f * (this.transform.lossyScale.x - 1f + 1.4285715f);
	}

	// Token: 0x060023E3 RID: 9187 RVA: 0x000C0C08 File Offset: 0x000BEE08
	private void Update()
	{
		if (this._stopped)
		{
			if (!this.crashingObject.gameObject.activeSelf)
			{
				if (ObjectPools.instance)
				{
					ObjectPools.instance.Destroy(base.gameObject);
					return;
				}
				base.gameObject.SetActive(false);
			}
			return;
		}
		this._timeElapsed += Time.deltaTime;
		this.nextPos = this.transform.position + this._direction * this._speed * Time.deltaTime * this.scaleFactor;
		if (this._timeElapsed < this.maxFlightTime && (this._timeElapsed < this.minFlightTime || Physics.RaycastNonAlloc(this.transform.position, this.nextPos - this.transform.position, this.results, Vector3.Distance(this.transform.position, this.nextPos), this.layerMask.value) == 0))
		{
			this.transform.position = this.nextPos;
			this.transform.Rotate(Mathf.Sin(this._timeElapsed) * 10f * Time.deltaTime, 0f, 0f);
			return;
		}
		if (this._timeElapsed < this.maxFlightTime)
		{
			SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
			if (this.results[0].collider.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
			{
				slingshotProjectileHitNotifier.InvokeHit(this, this.results[0].collider);
			}
			if (this.spawnWorldEffects != null)
			{
				this.spawnWorldEffects.RequestSpawn(this.nextPos);
			}
		}
		this._stopped = true;
		this._timeElapsed = 0f;
		PaperPlaneProjectile.PaperPlaneHit onHit = this.OnHit;
		if (onHit != null)
		{
			onHit(this.nextPos);
		}
		this.OnHit = null;
		this.flyingObject.SetActive(false);
		this.crashingObject.SetActive(true);
	}

	// Token: 0x060023E4 RID: 9188 RVA: 0x000C0DFA File Offset: 0x000BEFFA
	internal void SetVRRig(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x060023E5 RID: 9189 RVA: 0x000C0E03 File Offset: 0x000BF003
	private void OnDisable()
	{
		if (this.useTransferrableObjectState)
		{
			UnityEvent onResetProjectileState = this.OnResetProjectileState;
			if (onResetProjectileState == null)
			{
				return;
			}
			onResetProjectileState.Invoke();
		}
	}

	// Token: 0x04002F03 RID: 12035
	private const float speedScaleRatio = 0.7f;

	// Token: 0x04002F05 RID: 12037
	[Space]
	[NonSerialized]
	private float _timeElapsed;

	// Token: 0x04002F06 RID: 12038
	[NonSerialized]
	private float _speed;

	// Token: 0x04002F07 RID: 12039
	[NonSerialized]
	private Vector3 _direction;

	// Token: 0x04002F08 RID: 12040
	[NonSerialized]
	private bool _stopped;

	// Token: 0x04002F09 RID: 12041
	private Transform _tCached;

	// Token: 0x04002F0A RID: 12042
	private SpawnWorldEffects spawnWorldEffects;

	// Token: 0x04002F0B RID: 12043
	private Vector3 nextPos;

	// Token: 0x04002F0C RID: 12044
	private RaycastHit[] results = new RaycastHit[1];

	// Token: 0x04002F0D RID: 12045
	[Tooltip("Maximum lifetime in seconds for the projectile")]
	[SerializeField]
	private float maxFlightTime = 7.5f;

	// Token: 0x04002F0E RID: 12046
	[Tooltip("Collisions are ignored for minFlightTime seconds after launch")]
	[SerializeField]
	private float minFlightTime = 0.5f;

	// Token: 0x04002F0F RID: 12047
	[Tooltip("Hand speed to projectile launch Speed")]
	[SerializeField]
	private AnimationCurve speedCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f),
		new Keyframe(6.324555f, 20f, 6.324555f, 6.324555f)
	});

	// Token: 0x04002F10 RID: 12048
	[Tooltip("maximum speed of launched projectile (clamped after applying speed curve)")]
	[SerializeField]
	private float maxSpeed = 10f;

	// Token: 0x04002F11 RID: 12049
	[Tooltip("minimum speed of launched projectile (clamped after applying speed curve)")]
	[SerializeField]
	private float minSpeed = 1f;

	// Token: 0x04002F12 RID: 12050
	[SerializeField]
	private bool enableRotation;

	// Token: 0x04002F13 RID: 12051
	[Tooltip("Objects enabled when launched and disabled on Hit")]
	[SerializeField]
	private GameObject flyingObject;

	// Token: 0x04002F14 RID: 12052
	[Tooltip("Objects disabled when launched and enabled on Hit")]
	[SerializeField]
	private GameObject crashingObject;

	// Token: 0x04002F15 RID: 12053
	[Tooltip("Layers the projectile collides with")]
	[SerializeField]
	private LayerMask layerMask;

	// Token: 0x04002F16 RID: 12054
	[SerializeField]
	private bool useTransferrableObjectState;

	// Token: 0x04002F17 RID: 12055
	[SerializeField]
	protected UnityEvent OnResetProjectileState;

	// Token: 0x04002F18 RID: 12056
	[SerializeField]
	protected string boolADebugName;

	// Token: 0x04002F19 RID: 12057
	[SerializeField]
	protected UnityEvent OnItemStateBoolATrue;

	// Token: 0x04002F1A RID: 12058
	[SerializeField]
	protected UnityEvent OnItemStateBoolAFalse;

	// Token: 0x04002F1B RID: 12059
	[SerializeField]
	protected string boolBDebugName;

	// Token: 0x04002F1C RID: 12060
	[SerializeField]
	protected UnityEvent OnItemStateBoolBTrue;

	// Token: 0x04002F1D RID: 12061
	[SerializeField]
	protected UnityEvent OnItemStateBoolBFalse;

	// Token: 0x04002F1E RID: 12062
	[SerializeField]
	protected string boolCDebugName;

	// Token: 0x04002F1F RID: 12063
	[SerializeField]
	protected UnityEvent OnItemStateBoolCTrue;

	// Token: 0x04002F20 RID: 12064
	[SerializeField]
	protected UnityEvent OnItemStateBoolCFalse;

	// Token: 0x04002F21 RID: 12065
	[SerializeField]
	protected string boolDDebugName;

	// Token: 0x04002F22 RID: 12066
	[SerializeField]
	protected UnityEvent OnItemStateBoolDTrue;

	// Token: 0x04002F23 RID: 12067
	[SerializeField]
	protected UnityEvent OnItemStateBoolDFalse;

	// Token: 0x04002F24 RID: 12068
	[SerializeField]
	protected UnityEvent<int> OnItemStateIntChanged;

	// Token: 0x04002F25 RID: 12069
	private VRRig myRig;

	// Token: 0x04002F26 RID: 12070
	private float scaleFactor;

	// Token: 0x02000589 RID: 1417
	// (Invoke) Token: 0x060023E8 RID: 9192
	public delegate void PaperPlaneHit(Vector3 endPoint);
}
