using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Fusion;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020008F5 RID: 2293
[NetworkBehaviourWeaved(4)]
public class MagicCauldron : NetworkComponent
{
	// Token: 0x06003BF1 RID: 15345 RVA: 0x00147B90 File Offset: 0x00145D90
	private new void Awake()
	{
		this.currentIngredients.Clear();
		this.witchesComponent.Clear();
		this.currentStateElapsedTime = 0f;
		this.currentRecipeIndex = -1;
		this.ingredientIndex = -1;
		if (this.flyingWitchesContainer != null)
		{
			for (int i = 0; i < this.flyingWitchesContainer.transform.childCount; i++)
			{
				NoncontrollableBroomstick componentInChildren = this.flyingWitchesContainer.transform.GetChild(i).gameObject.GetComponentInChildren<NoncontrollableBroomstick>();
				this.witchesComponent.Add(componentInChildren);
				if (componentInChildren)
				{
					componentInChildren.gameObject.SetActive(false);
				}
			}
		}
		if (this.reusableFXContext == null)
		{
			this.reusableFXContext = new MagicCauldron.IngrediantFXContext();
		}
		if (this.reusableIngrediantArgs == null)
		{
			this.reusableIngrediantArgs = new MagicCauldron.IngredientArgs();
		}
		this.reusableFXContext.fxCallBack = new MagicCauldron.IngrediantFXContext.Callback(this.OnIngredientAdd);
	}

	// Token: 0x06003BF2 RID: 15346 RVA: 0x00147C6E File Offset: 0x00145E6E
	private new void Start()
	{
		this.ChangeState(MagicCauldron.CauldronState.notReady);
	}

	// Token: 0x06003BF3 RID: 15347 RVA: 0x00147C77 File Offset: 0x00145E77
	private void LateUpdate()
	{
		this.UpdateState();
	}

	// Token: 0x06003BF4 RID: 15348 RVA: 0x00147C7F File Offset: 0x00145E7F
	private IEnumerator LevitationSpellCoroutine()
	{
		GTPlayer.Instance.SetHalloweenLevitation(this.levitationStrength, this.levitationDuration, this.levitationBlendOutDuration, this.levitationBonusStrength, this.levitationBonusOffAtYSpeed, this.levitationBonusFullAtYSpeed);
		yield return new WaitForSeconds(this.levitationSpellDuration);
		GTPlayer.Instance.SetHalloweenLevitation(0f, this.levitationDuration, this.levitationBlendOutDuration, 0f, this.levitationBonusOffAtYSpeed, this.levitationBonusFullAtYSpeed);
		yield break;
	}

	// Token: 0x06003BF5 RID: 15349 RVA: 0x00147C90 File Offset: 0x00145E90
	private void ChangeState(MagicCauldron.CauldronState state)
	{
		this.currentState = state;
		if (base.IsMine)
		{
			this.currentStateElapsedTime = 0f;
		}
		bool flag = state == MagicCauldron.CauldronState.summoned;
		foreach (NoncontrollableBroomstick noncontrollableBroomstick in this.witchesComponent)
		{
			if (noncontrollableBroomstick.gameObject.activeSelf != flag)
			{
				noncontrollableBroomstick.gameObject.SetActive(flag);
			}
		}
		if (this.currentState == MagicCauldron.CauldronState.summoned && Vector3.Distance(GTPlayer.Instance.transform.position, base.transform.position) < this.levitationRadius)
		{
			base.StartCoroutine(this.LevitationSpellCoroutine());
		}
		switch (this.currentState)
		{
		case MagicCauldron.CauldronState.notReady:
			this.currentIngredients.Clear();
			this.UpdateCauldronColor(this.CauldronNotReadyColor);
			return;
		case MagicCauldron.CauldronState.ready:
			this.UpdateCauldronColor(this.CauldronActiveColor);
			return;
		case MagicCauldron.CauldronState.recipeCollecting:
			if (this.ingredientIndex >= 0 && this.ingredientIndex < this.allIngredients.Length)
			{
				this.UpdateCauldronColor(this.allIngredients[this.ingredientIndex].color);
				return;
			}
			break;
		case MagicCauldron.CauldronState.recipeActivated:
			if (this.audioSource && this.recipes[this.currentRecipeIndex].successAudio)
			{
				this.audioSource.GTPlayOneShot(this.recipes[this.currentRecipeIndex].successAudio, 1f);
			}
			if (this.successParticle)
			{
				this.successParticle.Play();
				return;
			}
			break;
		case MagicCauldron.CauldronState.summoned:
			break;
		case MagicCauldron.CauldronState.failed:
			this.currentIngredients.Clear();
			this.UpdateCauldronColor(this.CauldronFailedColor);
			this.audioSource.GTPlayOneShot(this.recipeFailedAudio, 1f);
			return;
		case MagicCauldron.CauldronState.cooldown:
			this.currentIngredients.Clear();
			this.UpdateCauldronColor(this.CauldronFailedColor);
			break;
		default:
			return;
		}
	}

	// Token: 0x06003BF6 RID: 15350 RVA: 0x00147E88 File Offset: 0x00146088
	private void UpdateState()
	{
		if (base.IsMine)
		{
			this.currentStateElapsedTime += Time.deltaTime;
			switch (this.currentState)
			{
			case MagicCauldron.CauldronState.notReady:
			case MagicCauldron.CauldronState.ready:
				break;
			case MagicCauldron.CauldronState.recipeCollecting:
				if (this.currentStateElapsedTime >= this.maxTimeToAddAllIngredients && !this.CheckIngredients())
				{
					this.ChangeState(MagicCauldron.CauldronState.failed);
					return;
				}
				break;
			case MagicCauldron.CauldronState.recipeActivated:
				if (this.currentStateElapsedTime >= this.waitTimeToSummonWitches)
				{
					this.ChangeState(MagicCauldron.CauldronState.summoned);
					return;
				}
				break;
			case MagicCauldron.CauldronState.summoned:
				if (this.currentStateElapsedTime >= this.summonWitchesDuration)
				{
					this.ChangeState(MagicCauldron.CauldronState.cooldown);
					return;
				}
				break;
			case MagicCauldron.CauldronState.failed:
				if (this.currentStateElapsedTime >= this.recipeFailedDuration)
				{
					this.ChangeState(MagicCauldron.CauldronState.ready);
					return;
				}
				break;
			case MagicCauldron.CauldronState.cooldown:
				if (this.currentStateElapsedTime >= this.cooldownDuration)
				{
					this.ChangeState(MagicCauldron.CauldronState.ready);
				}
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06003BF7 RID: 15351 RVA: 0x00147F51 File Offset: 0x00146151
	public void OnEventStart()
	{
		this.ChangeState(MagicCauldron.CauldronState.ready);
	}

	// Token: 0x06003BF8 RID: 15352 RVA: 0x00147C6E File Offset: 0x00145E6E
	public void OnEventEnd()
	{
		this.ChangeState(MagicCauldron.CauldronState.notReady);
	}

	// Token: 0x06003BF9 RID: 15353 RVA: 0x00147F5A File Offset: 0x0014615A
	[PunRPC]
	public void OnIngredientAdd(int _ingredientIndex, PhotonMessageInfo info)
	{
		this.OnIngredientAddShared(_ingredientIndex, info);
	}

	// Token: 0x06003BFA RID: 15354 RVA: 0x00147F6C File Offset: 0x0014616C
	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public unsafe void RPC_OnIngredientAdd(int _ingredientIndex, RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 1) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void MagicCauldron::RPC_OnIngredientAdd(System.Int32,Fusion.RpcInfo)", base.Object, 1);
				}
				else
				{
					int num = 8;
					num += 4;
					if (!SimulationMessage.CanAllocateUserPayload(num))
					{
						NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void MagicCauldron::RPC_OnIngredientAdd(System.Int32,Fusion.RpcInfo)", num);
					}
					else
					{
						if (base.Runner.HasAnyActiveConnections())
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1);
							int num2 = 8;
							*(int*)(ptr2 + num2) = _ingredientIndex;
							num2 += 4;
							ptr->Offset = num2 * 8;
							base.Runner.SendRpc(ptr);
						}
						if ((localAuthorityMask & 7) != 0)
						{
							info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_12;
						}
					}
				}
			}
			return;
		}
		this.InvokeRpc = false;
		IL_12:
		this.OnIngredientAddShared(_ingredientIndex, info);
	}

	// Token: 0x06003BFB RID: 15355 RVA: 0x001480D0 File Offset: 0x001462D0
	private void OnIngredientAddShared(int _ingredientIndex, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "OnIngredientAdd");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		this.reusableFXContext.playerSettings = rigContainer.Rig.fxSettings;
		this.reusableIngrediantArgs.key = _ingredientIndex;
		FXSystem.PlayFX<MagicCauldron.IngredientArgs>(FXType.HWIngredients, this.reusableFXContext, this.reusableIngrediantArgs, info);
	}

	// Token: 0x06003BFC RID: 15356 RVA: 0x00148134 File Offset: 0x00146334
	private void OnIngredientAdd(int _ingredientIndex)
	{
		if (this.audioSource)
		{
			this.audioSource.GTPlayOneShot(this.ingredientAddedAudio, 1f);
		}
		if (!RoomSystem.AmITheHost)
		{
			return;
		}
		if (_ingredientIndex < 0 || _ingredientIndex >= this.allIngredients.Length || (this.currentState != MagicCauldron.CauldronState.ready && this.currentState != MagicCauldron.CauldronState.recipeCollecting))
		{
			return;
		}
		MagicIngredientType magicIngredientType = this.allIngredients[_ingredientIndex];
		Debug.Log(string.Format("Received ingredient RPC {0} = {1}", _ingredientIndex, magicIngredientType));
		MagicIngredientType magicIngredientType2 = null;
		if (this.recipes[0].recipeIngredients.Count > this.currentIngredients.Count)
		{
			magicIngredientType2 = this.recipes[0].recipeIngredients[this.currentIngredients.Count];
		}
		if (!(magicIngredientType == magicIngredientType2))
		{
			Debug.Log(string.Format("Failure: Expected ingredient {0}, got {1} from recipe[{2}]", magicIngredientType2, magicIngredientType, this.currentIngredients.Count));
			this.ChangeState(MagicCauldron.CauldronState.failed);
			return;
		}
		this.ingredientIndex = _ingredientIndex;
		this.currentIngredients.Add(magicIngredientType);
		if (this.CheckIngredients())
		{
			this.ChangeState(MagicCauldron.CauldronState.recipeActivated);
			return;
		}
		if (this.currentState == MagicCauldron.CauldronState.ready)
		{
			this.ChangeState(MagicCauldron.CauldronState.recipeCollecting);
			return;
		}
		this.UpdateCauldronColor(magicIngredientType.color);
	}

	// Token: 0x06003BFD RID: 15357 RVA: 0x00148268 File Offset: 0x00146468
	private bool CheckIngredients()
	{
		foreach (MagicCauldron.Recipe recipe in this.recipes)
		{
			if (this.currentIngredients.SequenceEqual(recipe.recipeIngredients))
			{
				this.currentRecipeIndex = this.recipes.IndexOf(recipe);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003BFE RID: 15358 RVA: 0x001482E0 File Offset: 0x001464E0
	private void UpdateCauldronColor(Color color)
	{
		if (this.bubblesParticle)
		{
			if (this.bubblesParticle.isPlaying)
			{
				if (this.currentState == MagicCauldron.CauldronState.failed || this.currentState == MagicCauldron.CauldronState.notReady)
				{
					this.bubblesParticle.Stop();
				}
			}
			else
			{
				this.bubblesParticle.Play();
			}
		}
		this.currentColor = this.cauldronColor;
		if (this.currentColor == color)
		{
			return;
		}
		if (this.rendr)
		{
			this._liquid.AnimateColorFromTo(this.cauldronColor, color, 1f);
			this.cauldronColor = color;
		}
		if (this.bubblesParticle)
		{
			this.bubblesParticle.main.startColor = color;
		}
	}

	// Token: 0x06003BFF RID: 15359 RVA: 0x0014839C File Offset: 0x0014659C
	private void OnTriggerEnter(Collider other)
	{
		ThrowableSetDressing componentInParent = other.GetComponentInParent<ThrowableSetDressing>();
		if (componentInParent == null || componentInParent.IngredientTypeSO == null || componentInParent.InHand())
		{
			return;
		}
		if (componentInParent.IsLocalOwnedWorldShareable)
		{
			if (componentInParent.IngredientTypeSO != null && (this.currentState == MagicCauldron.CauldronState.ready || this.currentState == MagicCauldron.CauldronState.recipeCollecting))
			{
				int num = this.allIngredients.IndexOfRef(componentInParent.IngredientTypeSO);
				Debug.Log(string.Format("Sending ingredient RPC {0} = {1}", componentInParent.IngredientTypeSO, num));
				base.SendRPC("OnIngredientAdd", RpcTarget.Others, new object[]
				{
					num
				});
				this.OnIngredientAdd(num);
			}
			componentInParent.StartRespawnTimer(0f);
		}
		if (componentInParent.IngredientTypeSO != null && this.splashParticle)
		{
			this.splashParticle.Play();
		}
	}

	// Token: 0x06003C00 RID: 15360 RVA: 0x00148478 File Offset: 0x00146678
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		this.currentIngredients.Clear();
	}

	// Token: 0x17000560 RID: 1376
	// (get) Token: 0x06003C01 RID: 15361 RVA: 0x00148491 File Offset: 0x00146691
	// (set) Token: 0x06003C02 RID: 15362 RVA: 0x001484BB File Offset: 0x001466BB
	[Networked]
	[NetworkedWeaved(0, 4)]
	private unsafe MagicCauldron.MagicCauldronData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MagicCauldron.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(MagicCauldron.MagicCauldronData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MagicCauldron.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(MagicCauldron.MagicCauldronData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06003C03 RID: 15363 RVA: 0x001484E6 File Offset: 0x001466E6
	public override void WriteDataFusion()
	{
		this.Data = new MagicCauldron.MagicCauldronData(this.currentStateElapsedTime, this.currentRecipeIndex, this.currentState, this.ingredientIndex);
	}

	// Token: 0x06003C04 RID: 15364 RVA: 0x0014850C File Offset: 0x0014670C
	public override void ReadDataFusion()
	{
		this.ReadDataShared(this.Data.CurrentStateElapsedTime, this.Data.CurrentRecipeIndex, this.Data.CurrentState, this.Data.IngredientIndex);
	}

	// Token: 0x06003C05 RID: 15365 RVA: 0x00148558 File Offset: 0x00146758
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.currentStateElapsedTime);
		stream.SendNext(this.currentRecipeIndex);
		stream.SendNext(this.currentState);
		stream.SendNext(this.ingredientIndex);
	}

	// Token: 0x06003C06 RID: 15366 RVA: 0x001485B8 File Offset: 0x001467B8
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		float stateElapsedTime = (float)stream.ReceiveNext();
		int recipeIndex = (int)stream.ReceiveNext();
		MagicCauldron.CauldronState state = (MagicCauldron.CauldronState)stream.ReceiveNext();
		int num = (int)stream.ReceiveNext();
		this.ReadDataShared(stateElapsedTime, recipeIndex, state, num);
	}

	// Token: 0x06003C07 RID: 15367 RVA: 0x00148610 File Offset: 0x00146810
	private void ReadDataShared(float stateElapsedTime, int recipeIndex, MagicCauldron.CauldronState state, int ingredientIndex)
	{
		MagicCauldron.CauldronState cauldronState = this.currentState;
		this.currentStateElapsedTime = stateElapsedTime;
		this.currentRecipeIndex = recipeIndex;
		this.currentState = state;
		this.ingredientIndex = ingredientIndex;
		if (cauldronState != this.currentState)
		{
			this.ChangeState(this.currentState);
			return;
		}
		if (this.currentState == MagicCauldron.CauldronState.recipeCollecting && ingredientIndex != ingredientIndex && ingredientIndex >= 0 && ingredientIndex < this.allIngredients.Length)
		{
			this.UpdateCauldronColor(this.allIngredients[ingredientIndex].color);
		}
	}

	// Token: 0x06003C09 RID: 15369 RVA: 0x0014870D File Offset: 0x0014690D
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06003C0A RID: 15370 RVA: 0x00148725 File Offset: 0x00146925
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x06003C0B RID: 15371 RVA: 0x0014873C File Offset: 0x0014693C
	[NetworkRpcWeavedInvoker(1, 1, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_OnIngredientAdd@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		int num2 = *(int*)(ptr + num);
		num += 4;
		int num3 = num2;
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((MagicCauldron)behaviour).RPC_OnIngredientAdd(num3, info);
	}

	// Token: 0x04004C7D RID: 19581
	public List<MagicCauldron.Recipe> recipes = new List<MagicCauldron.Recipe>();

	// Token: 0x04004C7E RID: 19582
	public float maxTimeToAddAllIngredients = 30f;

	// Token: 0x04004C7F RID: 19583
	public float summonWitchesDuration = 20f;

	// Token: 0x04004C80 RID: 19584
	public float recipeFailedDuration = 5f;

	// Token: 0x04004C81 RID: 19585
	public float cooldownDuration = 30f;

	// Token: 0x04004C82 RID: 19586
	public MagicIngredientType[] allIngredients;

	// Token: 0x04004C83 RID: 19587
	public GameObject flyingWitchesContainer;

	// Token: 0x04004C84 RID: 19588
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04004C85 RID: 19589
	public AudioClip ingredientAddedAudio;

	// Token: 0x04004C86 RID: 19590
	public AudioClip recipeFailedAudio;

	// Token: 0x04004C87 RID: 19591
	public ParticleSystem bubblesParticle;

	// Token: 0x04004C88 RID: 19592
	public ParticleSystem successParticle;

	// Token: 0x04004C89 RID: 19593
	public ParticleSystem splashParticle;

	// Token: 0x04004C8A RID: 19594
	public Color CauldronActiveColor;

	// Token: 0x04004C8B RID: 19595
	public Color CauldronFailedColor;

	// Token: 0x04004C8C RID: 19596
	[Tooltip("only if we are using the time of day event")]
	public Color CauldronNotReadyColor;

	// Token: 0x04004C8D RID: 19597
	private readonly List<NoncontrollableBroomstick> witchesComponent = new List<NoncontrollableBroomstick>();

	// Token: 0x04004C8E RID: 19598
	private readonly List<MagicIngredientType> currentIngredients = new List<MagicIngredientType>();

	// Token: 0x04004C8F RID: 19599
	private float currentStateElapsedTime;

	// Token: 0x04004C90 RID: 19600
	private MagicCauldron.CauldronState currentState;

	// Token: 0x04004C91 RID: 19601
	[SerializeField]
	private Renderer rendr;

	// Token: 0x04004C92 RID: 19602
	private Color cauldronColor;

	// Token: 0x04004C93 RID: 19603
	private Color currentColor;

	// Token: 0x04004C94 RID: 19604
	private int currentRecipeIndex;

	// Token: 0x04004C95 RID: 19605
	private int ingredientIndex;

	// Token: 0x04004C96 RID: 19606
	private float waitTimeToSummonWitches = 2f;

	// Token: 0x04004C97 RID: 19607
	[Space]
	[SerializeField]
	private MagicCauldronLiquid _liquid;

	// Token: 0x04004C98 RID: 19608
	private MagicCauldron.IngrediantFXContext reusableFXContext = new MagicCauldron.IngrediantFXContext();

	// Token: 0x04004C99 RID: 19609
	private MagicCauldron.IngredientArgs reusableIngrediantArgs = new MagicCauldron.IngredientArgs();

	// Token: 0x04004C9A RID: 19610
	public bool testLevitationAlwaysOn;

	// Token: 0x04004C9B RID: 19611
	public float levitationRadius;

	// Token: 0x04004C9C RID: 19612
	public float levitationSpellDuration;

	// Token: 0x04004C9D RID: 19613
	public float levitationStrength;

	// Token: 0x04004C9E RID: 19614
	public float levitationDuration;

	// Token: 0x04004C9F RID: 19615
	public float levitationBlendOutDuration;

	// Token: 0x04004CA0 RID: 19616
	public float levitationBonusStrength;

	// Token: 0x04004CA1 RID: 19617
	public float levitationBonusOffAtYSpeed;

	// Token: 0x04004CA2 RID: 19618
	public float levitationBonusFullAtYSpeed;

	// Token: 0x04004CA3 RID: 19619
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 4)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private MagicCauldron.MagicCauldronData _Data;

	// Token: 0x020008F6 RID: 2294
	private enum CauldronState
	{
		// Token: 0x04004CA5 RID: 19621
		notReady,
		// Token: 0x04004CA6 RID: 19622
		ready,
		// Token: 0x04004CA7 RID: 19623
		recipeCollecting,
		// Token: 0x04004CA8 RID: 19624
		recipeActivated,
		// Token: 0x04004CA9 RID: 19625
		summoned,
		// Token: 0x04004CAA RID: 19626
		failed,
		// Token: 0x04004CAB RID: 19627
		cooldown
	}

	// Token: 0x020008F7 RID: 2295
	[Serializable]
	public struct Recipe
	{
		// Token: 0x04004CAC RID: 19628
		public List<MagicIngredientType> recipeIngredients;

		// Token: 0x04004CAD RID: 19629
		public AudioClip successAudio;
	}

	// Token: 0x020008F8 RID: 2296
	private class IngredientArgs : FXSArgs
	{
		// Token: 0x04004CAE RID: 19630
		public int key;
	}

	// Token: 0x020008F9 RID: 2297
	private class IngrediantFXContext : IFXContextParems<MagicCauldron.IngredientArgs>
	{
		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x06003C0D RID: 15373 RVA: 0x0014879C File Offset: 0x0014699C
		FXSystemSettings IFXContextParems<MagicCauldron.IngredientArgs>.settings
		{
			get
			{
				return this.playerSettings;
			}
		}

		// Token: 0x06003C0E RID: 15374 RVA: 0x001487A4 File Offset: 0x001469A4
		void IFXContextParems<MagicCauldron.IngredientArgs>.OnPlayFX(MagicCauldron.IngredientArgs args)
		{
			this.fxCallBack(args.key);
		}

		// Token: 0x04004CAF RID: 19631
		public FXSystemSettings playerSettings;

		// Token: 0x04004CB0 RID: 19632
		public MagicCauldron.IngrediantFXContext.Callback fxCallBack;

		// Token: 0x020008FA RID: 2298
		// (Invoke) Token: 0x06003C11 RID: 15377
		public delegate void Callback(int key);
	}

	// Token: 0x020008FB RID: 2299
	[NetworkStructWeaved(4)]
	[StructLayout(LayoutKind.Explicit, Size = 16)]
	private struct MagicCauldronData : INetworkStruct
	{
		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x06003C14 RID: 15380 RVA: 0x001487B7 File Offset: 0x001469B7
		// (set) Token: 0x06003C15 RID: 15381 RVA: 0x001487BF File Offset: 0x001469BF
		public float CurrentStateElapsedTime { readonly get; set; }

		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x06003C16 RID: 15382 RVA: 0x001487C8 File Offset: 0x001469C8
		// (set) Token: 0x06003C17 RID: 15383 RVA: 0x001487D0 File Offset: 0x001469D0
		public int CurrentRecipeIndex { readonly get; set; }

		// Token: 0x17000564 RID: 1380
		// (get) Token: 0x06003C18 RID: 15384 RVA: 0x001487D9 File Offset: 0x001469D9
		// (set) Token: 0x06003C19 RID: 15385 RVA: 0x001487E1 File Offset: 0x001469E1
		public MagicCauldron.CauldronState CurrentState { readonly get; set; }

		// Token: 0x17000565 RID: 1381
		// (get) Token: 0x06003C1A RID: 15386 RVA: 0x001487EA File Offset: 0x001469EA
		// (set) Token: 0x06003C1B RID: 15387 RVA: 0x001487F2 File Offset: 0x001469F2
		public int IngredientIndex { readonly get; set; }

		// Token: 0x06003C1C RID: 15388 RVA: 0x001487FB File Offset: 0x001469FB
		public MagicCauldronData(float stateElapsedTime, int recipeIndex, MagicCauldron.CauldronState state, int ingredientIndex)
		{
			this.CurrentStateElapsedTime = stateElapsedTime;
			this.CurrentRecipeIndex = recipeIndex;
			this.CurrentState = state;
			this.IngredientIndex = ingredientIndex;
		}
	}
}
