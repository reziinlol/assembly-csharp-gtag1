using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000B66 RID: 2918
public class MessageBox : MonoBehaviour
{
	// Token: 0x170006F1 RID: 1777
	// (get) Token: 0x0600497F RID: 18815 RVA: 0x00189A0F File Offset: 0x00187C0F
	// (set) Token: 0x06004980 RID: 18816 RVA: 0x00189A17 File Offset: 0x00187C17
	public MessageBoxResult Result { get; private set; }

	// Token: 0x170006F2 RID: 1778
	// (get) Token: 0x06004981 RID: 18817 RVA: 0x00189A20 File Offset: 0x00187C20
	// (set) Token: 0x06004982 RID: 18818 RVA: 0x00189A2D File Offset: 0x00187C2D
	public string Header
	{
		get
		{
			return this._headerText.text;
		}
		set
		{
			this._headerText.text = value;
			this._headerText.gameObject.SetActive(!string.IsNullOrEmpty(value));
		}
	}

	// Token: 0x170006F3 RID: 1779
	// (get) Token: 0x06004983 RID: 18819 RVA: 0x00189A54 File Offset: 0x00187C54
	// (set) Token: 0x06004984 RID: 18820 RVA: 0x00189A61 File Offset: 0x00187C61
	public string Body
	{
		get
		{
			return this._bodyText.text;
		}
		set
		{
			this._bodyText.text = value;
		}
	}

	// Token: 0x170006F4 RID: 1780
	// (get) Token: 0x06004985 RID: 18821 RVA: 0x00189A6F File Offset: 0x00187C6F
	// (set) Token: 0x06004986 RID: 18822 RVA: 0x00189A7C File Offset: 0x00187C7C
	public string LeftButton
	{
		get
		{
			return this._leftButtonText.text;
		}
		set
		{
			this._leftButtonText.text = value;
			this._leftButton.SetActive(!string.IsNullOrEmpty(value));
			if (string.IsNullOrEmpty(value))
			{
				RectTransform component = this._rightButton.GetComponent<RectTransform>();
				component.anchorMin = new Vector2(0.5f, 0.5f);
				component.anchorMax = new Vector2(0.5f, 0.5f);
				component.pivot = new Vector2(0.5f, 0.5f);
				component.anchoredPosition = Vector3.zero;
				return;
			}
			RectTransform component2 = this._rightButton.GetComponent<RectTransform>();
			component2.anchorMin = new Vector2(1f, 0.5f);
			component2.anchorMax = new Vector2(1f, 0.5f);
			component2.pivot = new Vector2(1f, 0.5f);
			component2.anchoredPosition = Vector3.zero;
		}
	}

	// Token: 0x170006F5 RID: 1781
	// (get) Token: 0x06004987 RID: 18823 RVA: 0x00189B64 File Offset: 0x00187D64
	// (set) Token: 0x06004988 RID: 18824 RVA: 0x00189B74 File Offset: 0x00187D74
	public string RightButton
	{
		get
		{
			return this._rightButtonText.text;
		}
		set
		{
			this._rightButtonText.text = value;
			this._rightButton.SetActive(!string.IsNullOrEmpty(value));
			if (string.IsNullOrEmpty(value))
			{
				RectTransform component = this._leftButton.GetComponent<RectTransform>();
				component.anchorMin = new Vector2(0.5f, 0.5f);
				component.anchorMax = new Vector2(0.5f, 0.5f);
				component.pivot = new Vector2(0.5f, 0.5f);
				component.anchoredPosition3D = Vector3.zero;
				return;
			}
			RectTransform component2 = this._leftButton.GetComponent<RectTransform>();
			component2.anchorMin = new Vector2(0f, 0.5f);
			component2.anchorMax = new Vector2(0f, 0.5f);
			component2.pivot = new Vector2(0f, 0.5f);
			component2.anchoredPosition3D = Vector3.zero;
		}
	}

	// Token: 0x170006F6 RID: 1782
	// (get) Token: 0x06004989 RID: 18825 RVA: 0x00189C52 File Offset: 0x00187E52
	public UnityEvent LeftButtonCallback
	{
		get
		{
			return this._leftButtonCallback;
		}
	}

	// Token: 0x170006F7 RID: 1783
	// (get) Token: 0x0600498A RID: 18826 RVA: 0x00189C5A File Offset: 0x00187E5A
	public UnityEvent RightButtonCallback
	{
		get
		{
			return this._rightButtonCallback;
		}
	}

	// Token: 0x0600498B RID: 18827 RVA: 0x00189C62 File Offset: 0x00187E62
	private void Start()
	{
		this.Result = MessageBoxResult.None;
	}

	// Token: 0x0600498C RID: 18828 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Update()
	{
	}

	// Token: 0x0600498D RID: 18829 RVA: 0x00189C6C File Offset: 0x00187E6C
	public void ShowQuitButtonAsPrimary()
	{
		this._leftButton.SetActive(false);
		RectTransform component = this._rightButton.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0.5f, 0.5f);
		component.anchorMax = new Vector2(0.5f, 0.5f);
		component.pivot = new Vector2(0.5f, 0.5f);
		component.anchoredPosition = Vector3.zero;
	}

	// Token: 0x0600498E RID: 18830 RVA: 0x00189CDE File Offset: 0x00187EDE
	public void OnClickLeftButton()
	{
		this.Result = MessageBoxResult.Left;
		this._leftButtonCallback.Invoke();
	}

	// Token: 0x0600498F RID: 18831 RVA: 0x00189CF2 File Offset: 0x00187EF2
	public void OnClickRightButton()
	{
		this.Result = MessageBoxResult.Right;
		this._rightButtonCallback.Invoke();
	}

	// Token: 0x06004990 RID: 18832 RVA: 0x00189D06 File Offset: 0x00187F06
	public GameObject GetCanvas()
	{
		return base.GetComponentInChildren<Canvas>(true).gameObject;
	}

	// Token: 0x06004991 RID: 18833 RVA: 0x00189D14 File Offset: 0x00187F14
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x04005C49 RID: 23625
	[SerializeField]
	private TMP_Text _headerText;

	// Token: 0x04005C4A RID: 23626
	[SerializeField]
	private TMP_Text _bodyText;

	// Token: 0x04005C4B RID: 23627
	[SerializeField]
	private TMP_Text _leftButtonText;

	// Token: 0x04005C4C RID: 23628
	[SerializeField]
	private TMP_Text _rightButtonText;

	// Token: 0x04005C4D RID: 23629
	[SerializeField]
	private GameObject _leftButton;

	// Token: 0x04005C4E RID: 23630
	[SerializeField]
	private GameObject _rightButton;

	// Token: 0x04005C50 RID: 23632
	[SerializeField]
	private UnityEvent _leftButtonCallback = new UnityEvent();

	// Token: 0x04005C51 RID: 23633
	[SerializeField]
	private UnityEvent _rightButtonCallback = new UnityEvent();
}
