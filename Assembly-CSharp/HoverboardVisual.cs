using System;
using GorillaExtensions;
using GorillaLocomotion;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020008EE RID: 2286
public class HoverboardVisual : MonoBehaviour, ICallBack
{
	// Token: 0x17000553 RID: 1363
	// (get) Token: 0x06003BC7 RID: 15303 RVA: 0x001474F6 File Offset: 0x001456F6
	// (set) Token: 0x06003BC8 RID: 15304 RVA: 0x001474FE File Offset: 0x001456FE
	public Color boardColor { get; private set; }

	// Token: 0x06003BC9 RID: 15305 RVA: 0x00147508 File Offset: 0x00145708
	private void Awake()
	{
		Material[] sharedMaterials = this.boardMesh.sharedMaterials;
		this.colorMaterial = new Material(sharedMaterials[1]);
		sharedMaterials[1] = this.colorMaterial;
		this.boardMesh.sharedMaterials = sharedMaterials;
	}

	// Token: 0x17000554 RID: 1364
	// (get) Token: 0x06003BCA RID: 15306 RVA: 0x00147544 File Offset: 0x00145744
	// (set) Token: 0x06003BCB RID: 15307 RVA: 0x0014754C File Offset: 0x0014574C
	public bool IsHeld { get; private set; }

	// Token: 0x17000555 RID: 1365
	// (get) Token: 0x06003BCC RID: 15308 RVA: 0x00147555 File Offset: 0x00145755
	// (set) Token: 0x06003BCD RID: 15309 RVA: 0x0014755D File Offset: 0x0014575D
	public bool IsLeftHanded { get; private set; }

	// Token: 0x17000556 RID: 1366
	// (get) Token: 0x06003BCE RID: 15310 RVA: 0x00147566 File Offset: 0x00145766
	// (set) Token: 0x06003BCF RID: 15311 RVA: 0x0014756E File Offset: 0x0014576E
	public Vector3 NominalLocalPosition { get; private set; }

	// Token: 0x17000557 RID: 1367
	// (get) Token: 0x06003BD0 RID: 15312 RVA: 0x00147577 File Offset: 0x00145777
	// (set) Token: 0x06003BD1 RID: 15313 RVA: 0x0014757F File Offset: 0x0014577F
	public Quaternion NominalLocalRotation { get; private set; }

	// Token: 0x17000558 RID: 1368
	// (get) Token: 0x06003BD2 RID: 15314 RVA: 0x00147588 File Offset: 0x00145788
	private Transform NominalParentTransform
	{
		get
		{
			if (!this.IsHeld)
			{
				return base.transform.parent;
			}
			return (this.IsLeftHanded ? this.parentRig.leftHand : this.parentRig.rightHand).rigTarget.transform;
		}
	}

	// Token: 0x06003BD3 RID: 15315 RVA: 0x001475C8 File Offset: 0x001457C8
	public void SetIsHeld(bool isHeldLeftHanded, Vector3 localPosition, Quaternion localRotation, Color boardColor)
	{
		if (!this.isCallbackActive)
		{
			this.parentRig.AddLateUpdateCallback(this);
			this.isCallbackActive = true;
		}
		this.IsHeld = true;
		base.gameObject.SetActive(true);
		this.IsLeftHanded = isHeldLeftHanded;
		this.NominalLocalPosition = localPosition;
		this.NominalLocalRotation = localRotation;
		Transform nominalParentTransform = this.NominalParentTransform;
		this.interpolatedLocalPosition = nominalParentTransform.InverseTransformPoint(base.transform.position);
		this.interpolatedLocalRotation = nominalParentTransform.InverseTransformRotation(base.transform.rotation);
		this.positionLerpSpeed = (this.interpolatedLocalPosition - this.NominalLocalPosition).magnitude / this.lerpIntoHandDuration;
		float num;
		Vector3 vector;
		(Quaternion.Inverse(this.interpolatedLocalRotation) * this.NominalLocalRotation).ToAngleAxis(out num, out vector);
		this.rotationLerpSpeed = num / this.lerpIntoHandDuration;
		if (this.parentRig.isLocal)
		{
			GTPlayer.Instance.SetHoverActive(true);
		}
		this.colorMaterial.color = boardColor;
		this.boardColor = boardColor;
	}

	// Token: 0x06003BD4 RID: 15316 RVA: 0x001476D1 File Offset: 0x001458D1
	public void SetNotHeld(bool isLeftHanded)
	{
		this.IsLeftHanded = isLeftHanded;
		this.SetNotHeld();
	}

	// Token: 0x06003BD5 RID: 15317 RVA: 0x001476E0 File Offset: 0x001458E0
	public void SetNotHeld()
	{
		bool isHeld = this.IsHeld;
		base.gameObject.SetActive(false);
		this.IsHeld = false;
		this.interpolatedLocalPosition = base.transform.localPosition;
		this.interpolatedLocalRotation = base.transform.localRotation;
		this.positionLerpSpeed = (this.interpolatedLocalPosition - this.NominalLocalPosition).magnitude / this.lerpIntoHandDuration;
		float num;
		Vector3 vector;
		(Quaternion.Inverse(this.interpolatedLocalRotation) * this.NominalLocalRotation).ToAngleAxis(out num, out vector);
		this.rotationLerpSpeed = num / this.lerpIntoHandDuration;
		if (!isHeld)
		{
			base.transform.position = base.transform.parent.TransformPoint(this.NominalLocalPosition);
			base.transform.rotation = base.transform.parent.TransformRotation(this.NominalLocalRotation);
		}
		if (this.parentRig.isLocal)
		{
			GTPlayer.Instance.SetHoverActive(false);
		}
		this.hoverboardAudio.Stop();
	}

	// Token: 0x06003BD6 RID: 15318 RVA: 0x001477E8 File Offset: 0x001459E8
	void ICallBack.CallBack()
	{
		Transform nominalParentTransform = this.NominalParentTransform;
		if ((this.interpolatedLocalPosition - this.NominalLocalPosition).IsShorterThan(0.01f))
		{
			base.transform.position = nominalParentTransform.TransformPoint(this.NominalLocalPosition);
			base.transform.rotation = nominalParentTransform.TransformRotation(this.NominalLocalRotation);
			if (!this.IsHeld)
			{
				this.parentRig.RemoveLateUpdateCallback(this);
				this.isCallbackActive = false;
			}
		}
		else
		{
			this.interpolatedLocalPosition = Vector3.MoveTowards(this.interpolatedLocalPosition, this.NominalLocalPosition, this.positionLerpSpeed * Time.deltaTime);
			this.interpolatedLocalRotation = Quaternion.RotateTowards(this.interpolatedLocalRotation, this.NominalLocalRotation, this.rotationLerpSpeed * Time.deltaTime);
			base.transform.position = nominalParentTransform.TransformPoint(this.interpolatedLocalPosition);
			base.transform.rotation = nominalParentTransform.TransformRotation(this.interpolatedLocalRotation);
		}
		if (this.IsHeld)
		{
			if (this.parentRig.isLocal)
			{
				GTPlayer.Instance.SetHoverboardPosRot(base.transform.position, base.transform.rotation);
				return;
			}
			this.hoverboardAudio.UpdateAudioLoop(this.parentRig.LatestVelocity().magnitude, 0f, 0f, 0f);
		}
	}

	// Token: 0x06003BD7 RID: 15319 RVA: 0x0014793E File Offset: 0x00145B3E
	public void PlayGrindHaptic()
	{
		if (this.IsHeld)
		{
			GorillaTagger.Instance.StartVibration(this.IsLeftHanded, this.grindHapticStrength, this.grindHapticDuration);
		}
	}

	// Token: 0x06003BD8 RID: 15320 RVA: 0x00147964 File Offset: 0x00145B64
	public void PlayCarveHaptic(float carveForce)
	{
		if (this.IsHeld)
		{
			GorillaTagger.Instance.StartVibration(this.IsLeftHanded, carveForce * this.carveHapticStrength, this.carveHapticDuration);
		}
	}

	// Token: 0x06003BD9 RID: 15321 RVA: 0x0014798C File Offset: 0x00145B8C
	public void ProxyGrabHandle(bool isLeftHand)
	{
		EquipmentInteractor.instance.UpdateHandEquipment(this.handlePosition, isLeftHand);
	}

	// Token: 0x06003BDA RID: 15322 RVA: 0x001479A1 File Offset: 0x00145BA1
	public void DropFreeBoard()
	{
		FreeHoverboardManager.instance.SendDropBoardRPC(base.transform.position, base.transform.rotation, this.velocityEstimator.linearVelocity, this.velocityEstimator.angularVelocity, this.boardColor);
	}

	// Token: 0x06003BDB RID: 15323 RVA: 0x001479DF File Offset: 0x00145BDF
	public void SetRaceDisplay(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			this.racePositionReadout.gameObject.SetActive(false);
			return;
		}
		this.racePositionReadout.gameObject.SetActive(true);
		this.racePositionReadout.text = text;
	}

	// Token: 0x06003BDC RID: 15324 RVA: 0x00147A18 File Offset: 0x00145C18
	public void SetRaceLapsDisplay(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			this.raceLapsReadout.gameObject.SetActive(false);
			return;
		}
		this.raceLapsReadout.gameObject.SetActive(true);
		this.raceLapsReadout.text = text;
	}

	// Token: 0x04004C5E RID: 19550
	[SerializeField]
	private VRRig parentRig;

	// Token: 0x04004C5F RID: 19551
	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04004C60 RID: 19552
	[SerializeField]
	[FormerlySerializedAs("audio")]
	private HoverboardAudio hoverboardAudio;

	// Token: 0x04004C61 RID: 19553
	[SerializeField]
	private HoverboardHandle handlePosition;

	// Token: 0x04004C62 RID: 19554
	[SerializeField]
	private float grindHapticStrength;

	// Token: 0x04004C63 RID: 19555
	[SerializeField]
	private float grindHapticDuration;

	// Token: 0x04004C64 RID: 19556
	[SerializeField]
	private float carveHapticStrength;

	// Token: 0x04004C65 RID: 19557
	[SerializeField]
	private float carveHapticDuration;

	// Token: 0x04004C66 RID: 19558
	[SerializeField]
	private MeshRenderer boardMesh;

	// Token: 0x04004C67 RID: 19559
	[SerializeField]
	private InteractionPoint handleInteractionPoint;

	// Token: 0x04004C68 RID: 19560
	[SerializeField]
	private TextMeshPro racePositionReadout;

	// Token: 0x04004C69 RID: 19561
	[SerializeField]
	private TextMeshPro raceLapsReadout;

	// Token: 0x04004C6A RID: 19562
	private Material colorMaterial;

	// Token: 0x04004C70 RID: 19568
	private Vector3 interpolatedLocalPosition;

	// Token: 0x04004C71 RID: 19569
	private Quaternion interpolatedLocalRotation;

	// Token: 0x04004C72 RID: 19570
	[SerializeField]
	private float lerpIntoHandDuration;

	// Token: 0x04004C73 RID: 19571
	private float positionLerpSpeed;

	// Token: 0x04004C74 RID: 19572
	private float rotationLerpSpeed;

	// Token: 0x04004C75 RID: 19573
	private bool isCallbackActive;
}
