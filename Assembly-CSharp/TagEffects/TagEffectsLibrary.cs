using System;
using System.Collections;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x020010D2 RID: 4306
	public class TagEffectsLibrary : MonoBehaviour
	{
		// Token: 0x17000A30 RID: 2608
		// (get) Token: 0x06006BDA RID: 27610 RVA: 0x0022EC5F File Offset: 0x0022CE5F
		public static float FistBumpSpeedThreshold
		{
			get
			{
				return TagEffectsLibrary._instance.fistBumpSpeedThreshold;
			}
		}

		// Token: 0x17000A31 RID: 2609
		// (get) Token: 0x06006BDB RID: 27611 RVA: 0x0022EC6B File Offset: 0x0022CE6B
		public static float HighFiveSpeedThreshold
		{
			get
			{
				return TagEffectsLibrary._instance.highFiveSpeedThreshold;
			}
		}

		// Token: 0x17000A32 RID: 2610
		// (get) Token: 0x06006BDC RID: 27612 RVA: 0x0022EC77 File Offset: 0x0022CE77
		public static bool DebugMode
		{
			get
			{
				return TagEffectsLibrary._instance.debugMode;
			}
		}

		// Token: 0x06006BDD RID: 27613 RVA: 0x0022EC83 File Offset: 0x0022CE83
		private void Awake()
		{
			if (TagEffectsLibrary._instance != null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			TagEffectsLibrary._instance = this;
			this.tagEffectsPool = new Dictionary<string, Queue<GameObjectOnDisableDispatcher>>();
			this.tagEffectsComboLookUp = new Dictionary<TagEffectsCombo, TagEffectPack[]>();
		}

		// Token: 0x06006BDE RID: 27614 RVA: 0x0022ECBC File Offset: 0x0022CEBC
		public static void PlayEffect(Transform target, bool isLeftHand, float rigScale, TagEffectsLibrary.EffectType effectType, TagEffectPack playerCosmeticTagEffectPack, TagEffectPack otherPlayerCosmeticTagEffectPack, Quaternion rotation)
		{
			if (TagEffectsLibrary._instance == null)
			{
				return;
			}
			ModeTagEffect modeTagEffect = null;
			TagEffectPack tagEffectPack = null;
			GameModeType item = (GameMode.ActiveGameMode != null) ? GameMode.ActiveGameMode.GameType() : GameModeType.Casual;
			for (int i = 0; i < TagEffectsLibrary._instance.defaultTagEffects.Length; i++)
			{
				if (TagEffectsLibrary._instance.defaultTagEffects[i] != null && TagEffectsLibrary._instance.defaultTagEffects[i].Modes.Contains(item))
				{
					modeTagEffect = TagEffectsLibrary._instance.defaultTagEffects[i];
					tagEffectPack = modeTagEffect.tagEffect;
					break;
				}
			}
			if (tagEffectPack == null)
			{
				return;
			}
			GameObject firstPerson = tagEffectPack.firstPerson;
			GameObject thirdPerson = tagEffectPack.thirdPerson;
			GameObject fistBump = tagEffectPack.fistBump;
			GameObject highFive = tagEffectPack.highFive;
			bool firstPersonParentEffect = tagEffectPack.firstPersonParentEffect;
			bool thirdPersonParentEffect = tagEffectPack.thirdPersonParentEffect;
			bool flag = tagEffectPack.fistBumpParentEffect;
			bool highFiveParentEffect = tagEffectPack.highFiveParentEffect;
			if (playerCosmeticTagEffectPack != null)
			{
				TagEffectPack tagEffectPack2 = TagEffectsLibrary.comboLookup(playerCosmeticTagEffectPack, otherPlayerCosmeticTagEffectPack);
				if (!modeTagEffect.blockFistBumpOverride && playerCosmeticTagEffectPack.fistBump != null)
				{
					fistBump = tagEffectPack2.fistBump;
					flag = tagEffectPack2.firstPersonParentEffect;
				}
				if (!modeTagEffect.blockHiveFiveOverride && playerCosmeticTagEffectPack.highFive != null)
				{
					highFive = tagEffectPack2.highFive;
					highFiveParentEffect = tagEffectPack2.highFiveParentEffect;
				}
			}
			if (otherPlayerCosmeticTagEffectPack != null)
			{
				if (!modeTagEffect.blockTagOverride && otherPlayerCosmeticTagEffectPack.firstPerson != null)
				{
					firstPerson = otherPlayerCosmeticTagEffectPack.firstPerson;
					firstPersonParentEffect = otherPlayerCosmeticTagEffectPack.firstPersonParentEffect;
				}
				if (!modeTagEffect.blockTagOverride && otherPlayerCosmeticTagEffectPack.thirdPerson != null)
				{
					thirdPerson = otherPlayerCosmeticTagEffectPack.thirdPerson;
					thirdPersonParentEffect = otherPlayerCosmeticTagEffectPack.thirdPersonParentEffect;
				}
			}
			switch (effectType)
			{
			case TagEffectsLibrary.EffectType.FIRST_PERSON:
				TagEffectsLibrary.placeEffects(firstPerson, target, firstPersonParentEffect ? 1f : rigScale, false, firstPersonParentEffect, rotation);
				return;
			case TagEffectsLibrary.EffectType.THIRD_PERSON:
				TagEffectsLibrary.placeEffects(thirdPerson, target, thirdPersonParentEffect ? 1f : rigScale, false, thirdPersonParentEffect, rotation);
				return;
			case TagEffectsLibrary.EffectType.HIGH_FIVE:
				TagEffectsLibrary.placeEffects(highFive, target, highFiveParentEffect ? 1f : rigScale, isLeftHand, highFiveParentEffect, rotation);
				return;
			case TagEffectsLibrary.EffectType.FIST_BUMP:
				TagEffectsLibrary.placeEffects(fistBump, target, flag ? 1f : rigScale, isLeftHand, flag, rotation);
				return;
			default:
				return;
			}
		}

		// Token: 0x06006BDF RID: 27615 RVA: 0x0022EEDC File Offset: 0x0022D0DC
		private static TagEffectPack comboLookup(TagEffectPack playerCosmeticTagEffectPack, TagEffectPack otherPlayerCosmeticTagEffectPack)
		{
			if (otherPlayerCosmeticTagEffectPack == null)
			{
				return playerCosmeticTagEffectPack;
			}
			TagEffectsCombo tagEffectsCombo = new TagEffectsCombo();
			tagEffectsCombo.inputA = playerCosmeticTagEffectPack;
			tagEffectsCombo.inputB = otherPlayerCosmeticTagEffectPack;
			TagEffectPack[] array;
			if (!TagEffectsLibrary._instance.tagEffectsComboLookUp.TryGetValue(tagEffectsCombo, out array))
			{
				return playerCosmeticTagEffectPack;
			}
			int num = 0;
			if (GorillaComputer.instance != null)
			{
				num = GorillaComputer.instance.GetServerTime().Second;
			}
			return array[num % array.Length];
		}

		// Token: 0x06006BE0 RID: 27616 RVA: 0x0022EF4C File Offset: 0x0022D14C
		public static void placeEffects(GameObject prefab, Transform target, float scale, bool flipZAxis, bool parentEffect, Quaternion rotation)
		{
			if (prefab == null)
			{
				return;
			}
			Queue<GameObjectOnDisableDispatcher> queue;
			if (!TagEffectsLibrary._instance.tagEffectsPool.TryGetValue(prefab.name, out queue))
			{
				queue = new Queue<GameObjectOnDisableDispatcher>();
				TagEffectsLibrary._instance.tagEffectsPool.Add(prefab.name, queue);
			}
			if (queue.Count == 0 || (queue.Peek().gameObject.activeInHierarchy && queue.Count < 12))
			{
				GameObject gameObject = Object.Instantiate<GameObject>(prefab, target.transform.position, rotation, parentEffect ? target : TagEffectsLibrary._instance.transform);
				gameObject.name = prefab.name;
				gameObject.transform.localScale = (flipZAxis ? new Vector3(scale, scale, -scale) : (Vector3.one * scale));
				GameObjectOnDisableDispatcher gameObjectOnDisableDispatcher;
				if (!gameObject.TryGetComponent<GameObjectOnDisableDispatcher>(out gameObjectOnDisableDispatcher))
				{
					gameObjectOnDisableDispatcher = gameObject.AddComponent<GameObjectOnDisableDispatcher>();
				}
				gameObjectOnDisableDispatcher.OnDisabled += TagEffectsLibrary.NewGameObjectOnDisableDispatcher_OnDisabled;
				gameObject.SetActive(true);
				queue.Enqueue(gameObjectOnDisableDispatcher);
				return;
			}
			GameObjectOnDisableDispatcher recycledGameObject = queue.Dequeue();
			TagEffectsLibrary._instance.StartCoroutine(TagEffectsLibrary._instance.RecycleGameObject(recycledGameObject, target, scale, flipZAxis, parentEffect));
		}

		// Token: 0x06006BE1 RID: 27617 RVA: 0x0022F06B File Offset: 0x0022D26B
		private static void NewGameObjectOnDisableDispatcher_OnDisabled(GameObjectOnDisableDispatcher goodd)
		{
			TagEffectsLibrary._instance.StartCoroutine(TagEffectsLibrary._instance.ReclaimDisabled(goodd.transform));
		}

		// Token: 0x06006BE2 RID: 27618 RVA: 0x0022F088 File Offset: 0x0022D288
		private IEnumerator RecycleGameObject(GameObjectOnDisableDispatcher recycledGameObject, Transform target, float scale, bool flipZAxis, bool parentEffect)
		{
			if (recycledGameObject.gameObject.activeInHierarchy)
			{
				recycledGameObject.gameObject.SetActive(false);
				recycledGameObject.OnDisabled -= TagEffectsLibrary.NewGameObjectOnDisableDispatcher_OnDisabled;
				yield return null;
			}
			recycledGameObject.transform.position = target.transform.position;
			recycledGameObject.transform.rotation = target.transform.rotation;
			recycledGameObject.transform.localScale = (flipZAxis ? new Vector3(scale, scale, -scale) : (Vector3.one * scale));
			recycledGameObject.transform.parent = (parentEffect ? target : TagEffectsLibrary._instance.transform);
			Queue<GameObjectOnDisableDispatcher> queue;
			if (TagEffectsLibrary._instance.tagEffectsPool.TryGetValue(recycledGameObject.gameObject.name, out queue))
			{
				recycledGameObject.gameObject.SetActive(true);
				queue.Enqueue(recycledGameObject);
			}
			yield break;
		}

		// Token: 0x06006BE3 RID: 27619 RVA: 0x0022F0B5 File Offset: 0x0022D2B5
		private IEnumerator ReclaimDisabled(Transform transform)
		{
			yield return null;
			transform.parent = TagEffectsLibrary._instance.transform;
			yield break;
		}

		// Token: 0x04007C0A RID: 31754
		private const int OBJECT_QUEUE_LIMIT = 12;

		// Token: 0x04007C0B RID: 31755
		[OnEnterPlay_SetNull]
		private static TagEffectsLibrary _instance;

		// Token: 0x04007C0C RID: 31756
		[SerializeField]
		private float fistBumpSpeedThreshold = 1f;

		// Token: 0x04007C0D RID: 31757
		[SerializeField]
		private float highFiveSpeedThreshold = 1f;

		// Token: 0x04007C0E RID: 31758
		[SerializeField]
		private ModeTagEffect[] defaultTagEffects;

		// Token: 0x04007C0F RID: 31759
		[SerializeField]
		private TagEffectsComboResult[] tagEffectsCombos;

		// Token: 0x04007C10 RID: 31760
		[SerializeField]
		private bool debugMode;

		// Token: 0x04007C11 RID: 31761
		private Dictionary<string, Queue<GameObjectOnDisableDispatcher>> tagEffectsPool;

		// Token: 0x04007C12 RID: 31762
		private Dictionary<TagEffectsCombo, TagEffectPack[]> tagEffectsComboLookUp;

		// Token: 0x020010D3 RID: 4307
		public enum EffectType
		{
			// Token: 0x04007C14 RID: 31764
			FIRST_PERSON,
			// Token: 0x04007C15 RID: 31765
			THIRD_PERSON,
			// Token: 0x04007C16 RID: 31766
			HIGH_FIVE,
			// Token: 0x04007C17 RID: 31767
			FIST_BUMP
		}
	}
}
