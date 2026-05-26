using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag;
using Liv.Lck.GorillaTag;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000405 RID: 1029
public class LCKSocialCameraFollower : MonoBehaviour, ITickSystemTick
{
	// Token: 0x17000267 RID: 615
	// (get) Token: 0x0600185F RID: 6239 RVA: 0x0008A669 File Offset: 0x00088869
	public Transform ScaleTransform
	{
		get
		{
			return this._scaleTransform;
		}
	}

	// Token: 0x17000268 RID: 616
	// (get) Token: 0x06001860 RID: 6240 RVA: 0x0008A671 File Offset: 0x00088871
	public GameObject CameraVisualsRoot
	{
		get
		{
			return this._cameraVisualsRoot;
		}
	}

	// Token: 0x17000269 RID: 617
	// (get) Token: 0x06001861 RID: 6241 RVA: 0x0008A679 File Offset: 0x00088879
	public List<GameObject> VisualObjects
	{
		get
		{
			return this._visualObjects;
		}
	}

	// Token: 0x06001862 RID: 6242 RVA: 0x0008A684 File Offset: 0x00088884
	private void Awake()
	{
		this.m_gtCameraVisuals = this._cameraVisualsRoot.GetComponent<IGtCameraVisuals>();
		if (this.m_rigContainer.Rig.isOfflineVRRig)
		{
			base.gameObject.SetActive(false);
			return;
		}
		ListProcessor<Action<RigContainer>> disableEvent = this.m_rigContainer.RigEvents.disableEvent;
		Action<RigContainer> action = new Action<RigContainer>(this.PreRigDisable);
		disableEvent.Add(action);
		ListProcessor<Action<RigContainer>> enableEvent = this.m_rigContainer.RigEvents.enableEvent;
		action = new Action<RigContainer>(this.PostRigEnable);
		enableEvent.Add(action);
	}

	// Token: 0x06001863 RID: 6243 RVA: 0x0008A709 File Offset: 0x00088909
	private void Start()
	{
		if (!this.isParentedToRig)
		{
			base.transform.parent = null;
		}
	}

	// Token: 0x06001864 RID: 6244 RVA: 0x0008A720 File Offset: 0x00088920
	public void SetParentToRig()
	{
		this.isParentedToRig = true;
		base.transform.parent = this.m_rigContainer.transform;
		base.transform.localPosition = new Vector3(0f, -0.2f, 0.132f);
		base.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x06001865 RID: 6245 RVA: 0x0008A779 File Offset: 0x00088979
	public void SetParentNull()
	{
		this.isParentedToRig = false;
		base.transform.parent = null;
	}

	// Token: 0x06001866 RID: 6246 RVA: 0x0008A78E File Offset: 0x0008898E
	private void PostRigEnable(RigContainer _)
	{
		base.gameObject.SetActive(true);
		this.m_gtCameraVisuals.SetNetworkedVisualsActive(false);
		this.m_gtCameraVisuals.SetRecordingState(false);
	}

	// Token: 0x06001867 RID: 6247 RVA: 0x000440BC File Offset: 0x000422BC
	private void PreRigDisable(RigContainer _)
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001868 RID: 6248 RVA: 0x0008A7B4 File Offset: 0x000889B4
	public void SetNetworkController(LckSocialCamera networkController)
	{
		if (this.m_networkController.IsNotNull() && this.m_networkController != networkController)
		{
			this.m_networkController.TurnOff();
		}
		this.m_networkController = networkController;
		this.m_transformToFollow = this.m_networkController.transform;
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06001869 RID: 6249 RVA: 0x0008A805 File Offset: 0x00088A05
	public void RemoveNetworkController(LckSocialCamera networkController)
	{
		if (this.m_networkController != networkController)
		{
			return;
		}
		this.m_transformToFollow = null;
		this.m_networkController = null;
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x1700026A RID: 618
	// (get) Token: 0x0600186A RID: 6250 RVA: 0x0008A82A File Offset: 0x00088A2A
	// (set) Token: 0x0600186B RID: 6251 RVA: 0x0008A832 File Offset: 0x00088A32
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x0600186C RID: 6252 RVA: 0x0008A83C File Offset: 0x00088A3C
	void ITickSystemTick.Tick()
	{
		if (this.isParentedToRig || this.m_transformToFollow == null)
		{
			return;
		}
		base.transform.position = this.m_transformToFollow.position;
		base.transform.root.rotation = this.m_transformToFollow.rotation;
	}

	// Token: 0x04002388 RID: 9096
	[SerializeField]
	private Transform _scaleTransform;

	// Token: 0x04002389 RID: 9097
	[FormerlySerializedAs("_coconutCamera")]
	[SerializeField]
	private GameObject _cameraVisualsRoot;

	// Token: 0x0400238A RID: 9098
	[SerializeField]
	private List<GameObject> _visualObjects;

	// Token: 0x0400238B RID: 9099
	[SerializeField]
	private RigContainer m_rigContainer;

	// Token: 0x0400238C RID: 9100
	private Transform m_transformToFollow;

	// Token: 0x0400238D RID: 9101
	private LckSocialCamera m_networkController;

	// Token: 0x0400238E RID: 9102
	private IGtCameraVisuals m_gtCameraVisuals;

	// Token: 0x0400238F RID: 9103
	private bool isParentedToRig;
}
