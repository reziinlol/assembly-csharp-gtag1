using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Subscription
{
	// Token: 0x02000F63 RID: 3939
	public class FeatureTogglesScreen : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x1700093F RID: 2367
		// (get) Token: 0x06006221 RID: 25121 RVA: 0x001FADB0 File Offset: 0x001F8FB0
		private int NumPages
		{
			get
			{
				if (this._features.Length % 3 != 0)
				{
					return this._features.Length / 3 + 1;
				}
				return this._features.Length / 3;
			}
		}

		// Token: 0x17000940 RID: 2368
		// (get) Token: 0x06006222 RID: 25122 RVA: 0x001FADD5 File Offset: 0x001F8FD5
		private int LastPageIndex
		{
			get
			{
				return Math.Max(0, this.NumPages - 1);
			}
		}

		// Token: 0x06006223 RID: 25123 RVA: 0x001FADE8 File Offset: 0x001F8FE8
		private void Awake()
		{
			this._nextButton.button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(this.OnNextButtonPressed));
			this._backButton.button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(this.OnBackButtonPressed));
			this._exitButton.button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(this.OnExitButtonPressed));
			this.MarkDirty();
		}

		// Token: 0x06006224 RID: 25124 RVA: 0x001FAE5E File Offset: 0x001F905E
		private void OnNextButtonPressed(SITouchscreenButton.SITouchscreenButtonType type, int data, int actorNr)
		{
			this._currentPage++;
			if (this._currentPage > this.LastPageIndex)
			{
				this._currentPage = this.LastPageIndex;
			}
			this.MarkDirty();
		}

		// Token: 0x06006225 RID: 25125 RVA: 0x001FAE8E File Offset: 0x001F908E
		private void OnBackButtonPressed(SITouchscreenButton.SITouchscreenButtonType type, int data, int actorNr)
		{
			this._currentPage--;
			if (this._currentPage < 0)
			{
				this._currentPage = 0;
			}
			this.MarkDirty();
		}

		// Token: 0x06006226 RID: 25126 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void OnExitButtonPressed(SITouchscreenButton.SITouchscreenButtonType type, int data, int actorNr)
		{
		}

		// Token: 0x06006227 RID: 25127 RVA: 0x001FAEB4 File Offset: 0x001F90B4
		public void SliceUpdate()
		{
			if (!this._dirty)
			{
				return;
			}
			this._backButton.gameObject.SetActive(this._currentPage != 0);
			this._nextButton.gameObject.SetActive(this._currentPage != this.LastPageIndex);
			this.UpdateFeatureToggleUI();
			this._dirty = false;
		}

		// Token: 0x06006228 RID: 25128 RVA: 0x001FAF11 File Offset: 0x001F9111
		private void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
			this.MarkDirty();
		}

		// Token: 0x06006229 RID: 25129 RVA: 0x00011DE0 File Offset: 0x0000FFE0
		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x0600622A RID: 25130 RVA: 0x001FAF20 File Offset: 0x001F9120
		private void UpdateFeatureToggleUI()
		{
			for (int i = 0; i < this._featureToggleUi.Length; i++)
			{
				FeatureToggleUI featureToggleUI = this._featureToggleUi[i];
				int num = this._currentPage * 3 + i;
				bool flag = num < this._features.Length;
				featureToggleUI.gameObject.SetActive(flag);
				if (flag)
				{
					FeatureTogglesScreen.Feature feature = this._features[num];
					featureToggleUI.AttachToFeature(feature);
				}
			}
		}

		// Token: 0x0600622B RID: 25131 RVA: 0x001FAF81 File Offset: 0x001F9181
		public void MarkDirty()
		{
			this._dirty = true;
		}

		// Token: 0x040070F4 RID: 28916
		private const int TogglesPerPage = 3;

		// Token: 0x040070F5 RID: 28917
		[SerializeField]
		private FeatureTogglesScreen.Feature[] _features;

		// Token: 0x040070F6 RID: 28918
		[SerializeField]
		private SITouchscreenButtonContainer _nextButton;

		// Token: 0x040070F7 RID: 28919
		[SerializeField]
		private SITouchscreenButtonContainer _backButton;

		// Token: 0x040070F8 RID: 28920
		[SerializeField]
		private SITouchscreenButtonContainer _exitButton;

		// Token: 0x040070F9 RID: 28921
		[SerializeField]
		private FeatureToggleUI[] _featureToggleUi;

		// Token: 0x040070FA RID: 28922
		private int _currentPage;

		// Token: 0x040070FB RID: 28923
		private bool _dirty = true;

		// Token: 0x02000F64 RID: 3940
		[Serializable]
		public class Feature
		{
			// Token: 0x040070FC RID: 28924
			public string DisplayName = string.Empty;

			// Token: 0x040070FD RID: 28925
			public SubscriptionManager.SubscriptionFeatures Value;

			// Token: 0x040070FE RID: 28926
			public UnityEvent OnPressed;

			// Token: 0x040070FF RID: 28927
			public UnityEvent<bool> OnToggle;

			// Token: 0x04007100 RID: 28928
			public string UnavailableMessage = "NOT AVAILABLE ON THIS DEVICE";
		}
	}
}
