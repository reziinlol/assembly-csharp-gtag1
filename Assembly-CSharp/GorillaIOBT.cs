using System;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000876 RID: 2166
public class GorillaIOBT : MonoBehaviour
{
	// Token: 0x170004F8 RID: 1272
	// (get) Token: 0x0600385F RID: 14431 RVA: 0x00134922 File Offset: 0x00132B22
	// (set) Token: 0x06003860 RID: 14432 RVA: 0x0013492A File Offset: 0x00132B2A
	public OVRInput.Controller leftActiveController { get; private set; }

	// Token: 0x170004F9 RID: 1273
	// (get) Token: 0x06003861 RID: 14433 RVA: 0x00134933 File Offset: 0x00132B33
	// (set) Token: 0x06003862 RID: 14434 RVA: 0x0013493B File Offset: 0x00132B3B
	public OVRInput.Controller rightActiveController { get; private set; }

	// Token: 0x170004FA RID: 1274
	// (get) Token: 0x06003863 RID: 14435 RVA: 0x00134944 File Offset: 0x00132B44
	public bool IsHandTracking
	{
		get
		{
			return this.leftActiveController == OVRInput.Controller.LHand || this.rightActiveController == OVRInput.Controller.RHand;
		}
	}

	// Token: 0x170004FB RID: 1275
	// (get) Token: 0x06003864 RID: 14436 RVA: 0x0013495C File Offset: 0x00132B5C
	// (set) Token: 0x06003865 RID: 14437 RVA: 0x00134964 File Offset: 0x00132B64
	public HandTrackingFingerCurl leftHandCurl { get; private set; }

	// Token: 0x170004FC RID: 1276
	// (get) Token: 0x06003866 RID: 14438 RVA: 0x0013496D File Offset: 0x00132B6D
	// (set) Token: 0x06003867 RID: 14439 RVA: 0x00134975 File Offset: 0x00132B75
	public HandTrackingFingerCurl rightHandCurl { get; private set; }

	// Token: 0x170004FD RID: 1277
	// (get) Token: 0x06003868 RID: 14440 RVA: 0x0013497E File Offset: 0x00132B7E
	// (set) Token: 0x06003869 RID: 14441 RVA: 0x00134986 File Offset: 0x00132B86
	public Transform trackingSpace { get; private set; }

	// Token: 0x170004FE RID: 1278
	// (get) Token: 0x0600386A RID: 14442 RVA: 0x0013498F File Offset: 0x00132B8F
	// (set) Token: 0x0600386B RID: 14443 RVA: 0x00134997 File Offset: 0x00132B97
	public Transform centerEyeAnchor { get; private set; }

	// Token: 0x170004FF RID: 1279
	// (get) Token: 0x0600386C RID: 14444 RVA: 0x001349A0 File Offset: 0x00132BA0
	// (set) Token: 0x0600386D RID: 14445 RVA: 0x001349A8 File Offset: 0x00132BA8
	public Transform leftHandAnchor { get; private set; }

	// Token: 0x17000500 RID: 1280
	// (get) Token: 0x0600386E RID: 14446 RVA: 0x001349B1 File Offset: 0x00132BB1
	// (set) Token: 0x0600386F RID: 14447 RVA: 0x001349B9 File Offset: 0x00132BB9
	public Transform rightHandAnchor { get; private set; }

	// Token: 0x17000501 RID: 1281
	// (get) Token: 0x06003870 RID: 14448 RVA: 0x001349C2 File Offset: 0x00132BC2
	// (set) Token: 0x06003871 RID: 14449 RVA: 0x001349CA File Offset: 0x00132BCA
	public Transform leftControllerAnchor { get; private set; }

	// Token: 0x17000502 RID: 1282
	// (get) Token: 0x06003872 RID: 14450 RVA: 0x001349D3 File Offset: 0x00132BD3
	// (set) Token: 0x06003873 RID: 14451 RVA: 0x001349DB File Offset: 0x00132BDB
	public Transform rightControllerAnchor { get; private set; }

	// Token: 0x14000063 RID: 99
	// (add) Token: 0x06003874 RID: 14452 RVA: 0x001349E4 File Offset: 0x00132BE4
	// (remove) Token: 0x06003875 RID: 14453 RVA: 0x00134A1C File Offset: 0x00132C1C
	public event Action<GorillaIOBT> UpdatedAnchors;

	// Token: 0x14000064 RID: 100
	// (add) Token: 0x06003876 RID: 14454 RVA: 0x00134A54 File Offset: 0x00132C54
	// (remove) Token: 0x06003877 RID: 14455 RVA: 0x00134A8C File Offset: 0x00132C8C
	public event Action<Transform> TrackingSpaceChanged;

	// Token: 0x06003878 RID: 14456 RVA: 0x00134AC1 File Offset: 0x00132CC1
	protected virtual void Awake()
	{
		this._skipUpdate = true;
		this.EnsureGameObjectIntegrity();
		this.upperBodySkeleton = base.GetComponent<OVRSkeleton>();
	}

	// Token: 0x06003879 RID: 14457 RVA: 0x00134ADC File Offset: 0x00132CDC
	protected virtual void Start()
	{
		this.UpdateAnchors();
		Application.onBeforeRender += this.OnBeforeRenderCallback;
	}

	// Token: 0x0600387A RID: 14458 RVA: 0x00134AF6 File Offset: 0x00132CF6
	protected virtual void Update()
	{
		this._skipUpdate = false;
		this.UpdateAnchors();
	}

	// Token: 0x0600387B RID: 14459 RVA: 0x00134B05 File Offset: 0x00132D05
	protected virtual void OnDestroy()
	{
		Application.onBeforeRender -= this.OnBeforeRenderCallback;
	}

	// Token: 0x0600387C RID: 14460 RVA: 0x00134B1C File Offset: 0x00132D1C
	protected virtual void UpdateAnchors()
	{
		if (!OVRManager.OVRManagerinitialized)
		{
			return;
		}
		this.EnsureGameObjectIntegrity();
		if (!Application.isPlaying)
		{
			return;
		}
		if (this._skipUpdate)
		{
			this.centerEyeAnchor.FromOVRPose(OVRPose.identity, true);
			return;
		}
		bool monoscopic = OVRManager.instance.monoscopic;
		OVRNodeStateProperties.IsHmdPresent();
		OVRManager.tracker.GetPose(0);
		Quaternion.Euler(-OVRManager.instance.headPoseRelativeOffsetRotation.x, -OVRManager.instance.headPoseRelativeOffsetRotation.y, OVRManager.instance.headPoseRelativeOffsetRotation.z);
		OVRInput.Controller leftActiveController = this.leftActiveController;
		OVRInput.Controller rightActiveController = this.rightActiveController;
		this.leftActiveController = OVRInput.GetActiveControllerForHand(OVRInput.Handedness.LeftHanded);
		this.rightActiveController = OVRInput.GetActiveControllerForHand(OVRInput.Handedness.RightHanded);
		if (this.leftActiveController == OVRInput.Controller.None)
		{
			if (OVRInput.GetControllerPositionValid(OVRInput.Controller.LHand))
			{
				this.leftActiveController = OVRInput.Controller.LHand;
			}
			else if (OVRInput.GetControllerPositionValid(OVRInput.Controller.LTouch))
			{
				this.leftActiveController = OVRInput.Controller.LTouch;
			}
		}
		if (this.rightActiveController == OVRInput.Controller.None)
		{
			if (OVRInput.GetControllerPositionValid(OVRInput.Controller.RHand))
			{
				this.rightActiveController = OVRInput.Controller.RHand;
			}
			else if (OVRInput.GetControllerPositionValid(OVRInput.Controller.RTouch))
			{
				this.rightActiveController = OVRInput.Controller.RTouch;
			}
		}
		if (leftActiveController == OVRInput.Controller.None && this.leftActiveController != OVRInput.Controller.None)
		{
			this.trackingChangedAudioSource.PlayOneShot(this.trackingGainedClip);
		}
		else if (leftActiveController != OVRInput.Controller.None && this.leftActiveController == OVRInput.Controller.None)
		{
			this.trackingChangedAudioSource.PlayOneShot(this.trackingLostClip);
		}
		if (rightActiveController == OVRInput.Controller.None && this.rightActiveController != OVRInput.Controller.None)
		{
			this.trackingChangedAudioSource.PlayOneShot(this.trackingGainedClip);
		}
		else if (rightActiveController != OVRInput.Controller.None && this.rightActiveController == OVRInput.Controller.None)
		{
			this.trackingChangedAudioSource.PlayOneShot(this.trackingLostClip);
		}
		if (this.leftActiveController == OVRInput.Controller.LHand)
		{
			this.leftHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(this.leftActiveController);
			this.leftHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(this.leftActiveController);
			this.leftHandAnchor.localRotation = this.leftHandAnchor.localRotation * Quaternion.Euler(0f, 90f, -90f);
		}
		if (this.rightActiveController == OVRInput.Controller.RHand)
		{
			this.rightHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(this.rightActiveController);
			this.rightHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(this.rightActiveController);
			this.rightHandAnchor.localRotation = this.rightHandAnchor.localRotation * Quaternion.Euler(0f, -90f, 90f);
		}
		OVRPose ovrpose = OVRPose.identity;
		OVRPose ovrpose2 = OVRPose.identity;
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
		{
			ovrpose = OVRManager.GetOpenVRControllerOffset(XRNode.LeftHand);
			ovrpose2 = OVRManager.GetOpenVRControllerOffset(XRNode.RightHand);
			OVRManager.SetOpenVRLocalPose(this.trackingSpace.InverseTransformPoint(this.leftControllerAnchor.position), this.trackingSpace.InverseTransformPoint(this.rightControllerAnchor.position), Quaternion.Inverse(this.trackingSpace.rotation) * this.leftControllerAnchor.rotation, Quaternion.Inverse(this.trackingSpace.rotation) * this.rightControllerAnchor.rotation);
		}
		this.rightControllerAnchor.localPosition = ovrpose2.position;
		this.rightControllerAnchor.localRotation = ovrpose2.orientation;
		this.leftControllerAnchor.localPosition = ovrpose.position;
		this.leftControllerAnchor.localRotation = ovrpose.orientation;
		GTPlayer.Instance.SetHandOffsets(true, new Vector3(0.03f, -0.16f, 0f), Quaternion.Euler(89f, 6f, 11f));
		GTPlayer.Instance.SetHandOffsets(false, new Vector3(-0.01f, -0.16f, 0f), Quaternion.Euler(89f, 6f, 11f));
		this.RaiseUpdatedAnchorsEvent();
		this.CheckForTrackingSpaceChangesAndRaiseEvent();
	}

	// Token: 0x0600387D RID: 14461 RVA: 0x00134EAC File Offset: 0x001330AC
	protected virtual void OnBeforeRenderCallback()
	{
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus && OVRManager.instance.LateControllerUpdate)
		{
			this.UpdateAnchors();
		}
	}

	// Token: 0x0600387E RID: 14462 RVA: 0x00134EC8 File Offset: 0x001330C8
	protected virtual void CheckForTrackingSpaceChangesAndRaiseEvent()
	{
		if (this.trackingSpace == null)
		{
			return;
		}
		Matrix4x4 localToWorldMatrix = this.trackingSpace.localToWorldMatrix;
		bool flag = this.TrackingSpaceChanged != null && !this._previousTrackingSpaceTransform.Equals(localToWorldMatrix);
		this._previousTrackingSpaceTransform = localToWorldMatrix;
		if (flag)
		{
			this.TrackingSpaceChanged(this.trackingSpace);
		}
	}

	// Token: 0x0600387F RID: 14463 RVA: 0x00134F24 File Offset: 0x00133124
	protected virtual void RaiseUpdatedAnchorsEvent()
	{
		if (this.UpdatedAnchors != null)
		{
			this.UpdatedAnchors(this);
		}
	}

	// Token: 0x06003880 RID: 14464 RVA: 0x00134F3C File Offset: 0x0013313C
	public virtual void EnsureGameObjectIntegrity()
	{
		if (OVRManager.instance != null)
		{
			bool monoscopic = OVRManager.instance.monoscopic;
		}
		if (this.trackingSpace == null)
		{
			this.trackingSpace = this.ConfigureAnchor(null, this.trackingSpaceName);
			this._previousTrackingSpaceTransform = this.trackingSpace.localToWorldMatrix;
		}
		if (this.centerEyeAnchor == null)
		{
			this.centerEyeAnchor = this.ConfigureAnchor(this.trackingSpace, this.centerEyeAnchorName);
		}
		if (this.leftHandAnchor == null)
		{
			this.leftHandAnchor = this.ConfigureAnchor(this.trackingSpace, this.leftHandAnchorName);
		}
		if (this.rightHandAnchor == null)
		{
			this.rightHandAnchor = this.ConfigureAnchor(this.trackingSpace, this.rightHandAnchorName);
		}
		if (this.leftControllerAnchor == null)
		{
			this.leftControllerAnchor = this.ConfigureAnchor(this.leftHandAnchor, this.leftControllerAnchorName);
		}
		if (this.rightControllerAnchor == null)
		{
			this.rightControllerAnchor = this.ConfigureAnchor(this.rightHandAnchor, this.rightControllerAnchorName);
		}
		if (this.leftHandCurl == null)
		{
			Transform leftHandAnchor = this.leftHandAnchor;
			this.leftHandCurl = ((leftHandAnchor != null) ? leftHandAnchor.GetComponent<HandTrackingFingerCurl>() : null);
		}
		if (this.rightHandCurl == null)
		{
			Transform rightHandAnchor = this.rightHandAnchor;
			this.rightHandCurl = ((rightHandAnchor != null) ? rightHandAnchor.GetComponent<HandTrackingFingerCurl>() : null);
		}
	}

	// Token: 0x06003881 RID: 14465 RVA: 0x001350A0 File Offset: 0x001332A0
	protected Transform ConfigureAnchor(Transform root, string name)
	{
		Transform transform = (root != null) ? root.Find(name) : null;
		if (transform == null)
		{
			transform = base.transform.Find(name);
		}
		if (transform == null)
		{
			transform = new GameObject(name).transform;
		}
		transform.name = name;
		transform.parent = ((root != null) ? root : base.transform);
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		return transform;
	}

	// Token: 0x06003882 RID: 14466 RVA: 0x00135130 File Offset: 0x00133330
	public virtual Matrix4x4 ComputeTrackReferenceMatrix()
	{
		if (this.centerEyeAnchor == null)
		{
			Debug.LogError("centerEyeAnchor is required");
			return Matrix4x4.identity;
		}
		OVRPose identity = OVRPose.identity;
		Vector3 position;
		if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.Head, NodeStatePropertyType.Position, OVRPlugin.Node.Head, OVRPlugin.Step.Render, out position))
		{
			identity.position = position;
		}
		Quaternion orientation;
		if (OVRNodeStateProperties.GetNodeStatePropertyQuaternion(XRNode.Head, NodeStatePropertyType.Orientation, OVRPlugin.Node.Head, OVRPlugin.Step.Render, out orientation))
		{
			identity.orientation = orientation;
		}
		OVRPose ovrpose = identity.Inverse();
		Matrix4x4 rhs = Matrix4x4.TRS(ovrpose.position, ovrpose.orientation, Vector3.one);
		return this.centerEyeAnchor.localToWorldMatrix * rhs;
	}

	// Token: 0x06003883 RID: 14467 RVA: 0x001351C0 File Offset: 0x001333C0
	protected void CheckForAnchorsInParent()
	{
		Transform parent = base.transform.parent;
		while (parent)
		{
			this.<CheckForAnchorsInParent>g__Check|71_0<OVRSpatialAnchor>(parent);
			this.<CheckForAnchorsInParent>g__Check|71_0<OVRSceneAnchor>(parent);
			parent = parent.parent;
		}
	}

	// Token: 0x06003885 RID: 14469 RVA: 0x00135250 File Offset: 0x00133450
	[CompilerGenerated]
	private void <CheckForAnchorsInParent>g__Check|71_0<T>(Transform node) where T : MonoBehaviour
	{
		T component = node.GetComponent<T>();
		if (component && component.enabled)
		{
			component.enabled = false;
			Debug.LogError(string.Concat(new string[]
			{
				"The ",
				typeof(T).Name,
				" '",
				component.name,
				"' is a parent of the GorillaIOBT '",
				base.name,
				"', which is not allowed. An ",
				typeof(T).Name,
				" may not be the parent of an GorillaIOBT because the GorillaIOBT defines the tracking space for the anchor, and its transform is relative to the GorillaIOBT."
			}));
		}
	}

	// Token: 0x04004876 RID: 18550
	private OVRSkeleton upperBodySkeleton;

	// Token: 0x0400487F RID: 18559
	public AudioSource trackingChangedAudioSource;

	// Token: 0x04004880 RID: 18560
	public AudioClip trackingGainedClip;

	// Token: 0x04004881 RID: 18561
	public AudioClip trackingLostClip;

	// Token: 0x04004882 RID: 18562
	protected bool _skipUpdate;

	// Token: 0x04004883 RID: 18563
	protected readonly string trackingSpaceName = "TurnParent";

	// Token: 0x04004884 RID: 18564
	protected readonly string centerEyeAnchorName = "Main Camera";

	// Token: 0x04004885 RID: 18565
	protected readonly string leftHandAnchorName = "LeftHand Controller";

	// Token: 0x04004886 RID: 18566
	protected readonly string rightHandAnchorName = "RightHand Controller";

	// Token: 0x04004887 RID: 18567
	protected readonly string leftControllerAnchorName = "LeftControllerAnchor";

	// Token: 0x04004888 RID: 18568
	protected readonly string rightControllerAnchorName = "RightControllerAnchor";

	// Token: 0x04004889 RID: 18569
	protected Matrix4x4 _previousTrackingSpaceTransform;
}
