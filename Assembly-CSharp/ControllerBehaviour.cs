using System;
using UnityEngine;

// Token: 0x02000BC1 RID: 3009
[Obsolete("Use ControllerInputPoller instead", false)]
public class ControllerBehaviour : MonoBehaviour, IBuildValidation
{
	// Token: 0x1700070D RID: 1805
	// (get) Token: 0x06004B5B RID: 19291 RVA: 0x0019306E File Offset: 0x0019126E
	// (set) Token: 0x06004B5C RID: 19292 RVA: 0x00193075 File Offset: 0x00191275
	public static ControllerBehaviour Instance { get; private set; }

	// Token: 0x1700070E RID: 1806
	// (get) Token: 0x06004B5D RID: 19293 RVA: 0x0019307D File Offset: 0x0019127D
	private ControllerInputPoller Poller
	{
		get
		{
			if (this.poller != null)
			{
				return this.poller;
			}
			if (ControllerInputPoller.instance != null)
			{
				this.poller = ControllerInputPoller.instance;
				return this.poller;
			}
			return null;
		}
	}

	// Token: 0x1700070F RID: 1807
	// (get) Token: 0x06004B5E RID: 19294 RVA: 0x001930B8 File Offset: 0x001912B8
	public bool ButtonDown
	{
		get
		{
			return !(this.Poller == null) && (this.Poller.leftControllerPrimaryButton || this.Poller.leftControllerSecondaryButton || this.Poller.rightControllerPrimaryButton || this.Poller.rightControllerSecondaryButton);
		}
	}

	// Token: 0x17000710 RID: 1808
	// (get) Token: 0x06004B5F RID: 19295 RVA: 0x00193109 File Offset: 0x00191309
	public bool LeftButtonDown
	{
		get
		{
			return !(this.Poller == null) && (this.Poller.leftControllerPrimaryButton || this.Poller.leftControllerSecondaryButton || this.Poller.leftControllerTriggerButton);
		}
	}

	// Token: 0x17000711 RID: 1809
	// (get) Token: 0x06004B60 RID: 19296 RVA: 0x00193142 File Offset: 0x00191342
	public bool RightButtonDown
	{
		get
		{
			return !(this.Poller == null) && (this.Poller.rightControllerPrimaryButton || this.Poller.rightControllerSecondaryButton || this.Poller.rightControllerTriggerButton);
		}
	}

	// Token: 0x17000712 RID: 1810
	// (get) Token: 0x06004B61 RID: 19297 RVA: 0x0019317C File Offset: 0x0019137C
	public bool IsLeftStick
	{
		get
		{
			return !(this.Poller == null) && Mathf.Min(this.Poller.leftControllerPrimary2DAxis.x, this.Poller.rightControllerPrimary2DAxis.x) < -this.uxSettings.StickSensitvity;
		}
	}

	// Token: 0x17000713 RID: 1811
	// (get) Token: 0x06004B62 RID: 19298 RVA: 0x001931CC File Offset: 0x001913CC
	public bool IsRightStick
	{
		get
		{
			return !(this.Poller == null) && Mathf.Max(this.Poller.leftControllerPrimary2DAxis.x, this.Poller.rightControllerPrimary2DAxis.x) > this.uxSettings.StickSensitvity;
		}
	}

	// Token: 0x17000714 RID: 1812
	// (get) Token: 0x06004B63 RID: 19299 RVA: 0x0019321C File Offset: 0x0019141C
	public bool IsUpStick
	{
		get
		{
			return !(this.Poller == null) && Mathf.Max(this.Poller.leftControllerPrimary2DAxis.y, this.Poller.rightControllerPrimary2DAxis.y) > this.uxSettings.StickSensitvity;
		}
	}

	// Token: 0x17000715 RID: 1813
	// (get) Token: 0x06004B64 RID: 19300 RVA: 0x0019326C File Offset: 0x0019146C
	public bool IsDownStick
	{
		get
		{
			return !(this.Poller == null) && Mathf.Min(this.Poller.leftControllerPrimary2DAxis.y, this.Poller.rightControllerPrimary2DAxis.y) < -this.uxSettings.StickSensitvity;
		}
	}

	// Token: 0x17000716 RID: 1814
	// (get) Token: 0x06004B65 RID: 19301 RVA: 0x001932BC File Offset: 0x001914BC
	public float StickXValue
	{
		get
		{
			if (!(this.Poller == null))
			{
				return Mathf.Max(Mathf.Abs(this.Poller.leftControllerPrimary2DAxis.x), Mathf.Abs(this.Poller.rightControllerPrimary2DAxis.x));
			}
			return 0f;
		}
	}

	// Token: 0x17000717 RID: 1815
	// (get) Token: 0x06004B66 RID: 19302 RVA: 0x0019330C File Offset: 0x0019150C
	public float StickYValue
	{
		get
		{
			if (!(this.Poller == null))
			{
				return Mathf.Max(Mathf.Abs(this.Poller.leftControllerPrimary2DAxis.y), Mathf.Abs(this.Poller.rightControllerPrimary2DAxis.y));
			}
			return 0f;
		}
	}

	// Token: 0x17000718 RID: 1816
	// (get) Token: 0x06004B67 RID: 19303 RVA: 0x0019335C File Offset: 0x0019155C
	public bool TriggerDown
	{
		get
		{
			return !(this.Poller == null) && (this.Poller.leftControllerTriggerButton || this.Poller.rightControllerTriggerButton);
		}
	}

	// Token: 0x14000085 RID: 133
	// (add) Token: 0x06004B68 RID: 19304 RVA: 0x00193388 File Offset: 0x00191588
	// (remove) Token: 0x06004B69 RID: 19305 RVA: 0x001933C0 File Offset: 0x001915C0
	public event ControllerBehaviour.OnActionEvent OnAction;

	// Token: 0x06004B6A RID: 19306 RVA: 0x001933F5 File Offset: 0x001915F5
	private void Awake()
	{
		if (ControllerBehaviour.Instance != null)
		{
			Debug.LogError("[CONTROLLER_BEHAVIOUR] Trying to create new singleton but one already exists", base.gameObject);
			Object.DestroyImmediate(this);
			return;
		}
		ControllerBehaviour.Instance = this;
	}

	// Token: 0x06004B6B RID: 19307 RVA: 0x00193424 File Offset: 0x00191624
	private void Update()
	{
		bool flag = (this.IsLeftStick && this.wasLeftStick) || (this.IsRightStick && this.wasRightStick) || (this.IsUpStick && this.wasUpStick) || (this.IsDownStick && this.wasDownStick);
		if (Time.time - this.actionTime < this.actionDelay / this.repeatAction)
		{
			return;
		}
		if (this.wasHeld && flag)
		{
			this.repeatAction += this.actionRepeatDelayReduction;
		}
		else
		{
			this.repeatAction = 1f;
		}
		if (this.IsLeftStick || this.IsRightStick || this.IsUpStick || this.IsDownStick || this.ButtonDown)
		{
			this.actionTime = Time.time;
		}
		if (this.OnAction != null)
		{
			this.OnAction();
		}
		this.wasHeld = flag;
		this.wasDownStick = this.IsDownStick;
		this.wasUpStick = this.IsUpStick;
		this.wasLeftStick = this.IsLeftStick;
		this.wasRightStick = this.IsRightStick;
	}

	// Token: 0x06004B6C RID: 19308 RVA: 0x00193539 File Offset: 0x00191739
	public bool BuildValidationCheck()
	{
		if (this.uxSettings == null)
		{
			Debug.LogError("ControllerBehaviour must set UXSettings");
			return false;
		}
		return true;
	}

	// Token: 0x06004B6D RID: 19309 RVA: 0x00193556 File Offset: 0x00191756
	public static ControllerBehaviour CreateNewControllerBehaviour(GameObject gameObject, UXSettings settings)
	{
		ControllerBehaviour controllerBehaviour = gameObject.AddComponent<ControllerBehaviour>();
		controllerBehaviour.uxSettings = settings;
		return controllerBehaviour;
	}

	// Token: 0x04005E72 RID: 24178
	private float actionTime;

	// Token: 0x04005E73 RID: 24179
	private float repeatAction = 1f;

	// Token: 0x04005E74 RID: 24180
	[SerializeField]
	private UXSettings uxSettings;

	// Token: 0x04005E75 RID: 24181
	[SerializeField]
	private float actionDelay = 0.5f;

	// Token: 0x04005E76 RID: 24182
	[SerializeField]
	private float actionRepeatDelayReduction = 0.5f;

	// Token: 0x04005E77 RID: 24183
	[Tooltip("Should the triggers modify the x axis like the sticks do?")]
	[SerializeField]
	private bool useTriggersAsSticks;

	// Token: 0x04005E78 RID: 24184
	private ControllerInputPoller poller;

	// Token: 0x04005E79 RID: 24185
	private bool wasLeftStick;

	// Token: 0x04005E7A RID: 24186
	private bool wasRightStick;

	// Token: 0x04005E7B RID: 24187
	private bool wasUpStick;

	// Token: 0x04005E7C RID: 24188
	private bool wasDownStick;

	// Token: 0x04005E7D RID: 24189
	private bool wasHeld;

	// Token: 0x02000BC2 RID: 3010
	// (Invoke) Token: 0x06004B70 RID: 19312
	public delegate void OnActionEvent();
}
