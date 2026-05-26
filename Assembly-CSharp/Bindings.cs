using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000BEB RID: 3051
[BurstCompile]
public static class Bindings
{
	// Token: 0x06004C33 RID: 19507 RVA: 0x00197C18 File Offset: 0x00195E18
	public unsafe static void GameObjectBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.LuauGameObject>("GameObject").AddField("position", "Position").AddField("rotation", "Rotation").AddField("scale", "Scale").AddStaticFunction("findGameObject", new lua_CFunction(Bindings.GameObjectFunctions.FindGameObject)).AddFunction("setCollision", new lua_CFunction(Bindings.GameObjectFunctions.SetCollision)).AddFunction("setVisibility", new lua_CFunction(Bindings.GameObjectFunctions.SetVisibility)).AddFunction("setActive", new lua_CFunction(Bindings.GameObjectFunctions.SetActive)).AddFunction("setText", new lua_CFunction(Bindings.GameObjectFunctions.SetText)).AddFunction("onTouched", new lua_CFunction(Bindings.GameObjectFunctions.OnTouched)).AddFunction("setVelocity", new lua_CFunction(Bindings.GameObjectFunctions.SetVelocity)).AddFunction("getVelocity", new lua_CFunction(Bindings.GameObjectFunctions.GetVelocity)).AddFunction("setColor", new lua_CFunction(Bindings.GameObjectFunctions.SetColor)).AddFunction("findChild", new lua_CFunction(Bindings.GameObjectFunctions.FindChildGameObject)).AddFunction("clone", new lua_CFunction(Bindings.GameObjectFunctions.CloneGameObject)).AddFunction("destroy", new lua_CFunction(Bindings.GameObjectFunctions.DestroyGameObject)).AddFunction("findComponent", new lua_CFunction(Bindings.GameObjectFunctions.FindComponent)).AddFunction("equals", new lua_CFunction(Bindings.GameObjectFunctions.Equals)).Build(L, true));
	}

	// Token: 0x06004C34 RID: 19508 RVA: 0x00197DA4 File Offset: 0x00195FA4
	public unsafe static void GorillaLocomotionSettingsBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.GorillaLocomotionSettings>("PSettings").AddField("velocityLimit", null).AddField("slideVelocityLimit", null).AddField("maxJumpSpeed", null).AddField("jumpMultiplier", null).Build(L, false));
		Bindings.LocomotionSettings = Luau.lua_class_push<Bindings.GorillaLocomotionSettings>(L);
		Bindings.LocomotionSettings->velocityLimit = GTPlayer.Instance.velocityLimit;
		Bindings.LocomotionSettings->slideVelocityLimit = GTPlayer.Instance.slideVelocityLimit;
		Bindings.LocomotionSettings->maxJumpSpeed = 6.5f;
		Bindings.LocomotionSettings->jumpMultiplier = 1.1f;
		Luau.lua_setglobal(L, "PlayerSettings");
	}

	// Token: 0x06004C35 RID: 19509 RVA: 0x00197E58 File Offset: 0x00196058
	public unsafe static void PlayerInputBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.PlayerInput>("PInput").AddField("leftXAxis", null).AddField("rightXAxis", null).AddField("leftYAxis", null).AddField("rightYAxis", null).AddField("leftTrigger", null).AddField("rightTrigger", null).AddField("leftGrip", null).AddField("rightGrip", null).AddField("leftPrimaryButton", null).AddField("rightPrimaryButton", null).AddField("leftSecondaryButton", null).AddField("rightSecondaryButton", null).Build(L, false));
		Bindings.LocalPlayerInput = Luau.lua_class_push<Bindings.PlayerInput>(L);
		Bindings.UpdateInputs();
		Luau.lua_setglobal(L, "PlayerInput");
	}

	// Token: 0x06004C36 RID: 19510 RVA: 0x00197F20 File Offset: 0x00196120
	public unsafe static void UpdateInputs()
	{
		if (Bindings.LocalPlayerInput != null)
		{
			Bindings.LocalPlayerInput->leftPrimaryButton = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
			Bindings.LocalPlayerInput->rightPrimaryButton = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
			Bindings.LocalPlayerInput->leftSecondaryButton = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
			Bindings.LocalPlayerInput->rightSecondaryButton = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
			Bindings.LocalPlayerInput->leftGrip = ControllerInputPoller.GripFloat(XRNode.LeftHand);
			Bindings.LocalPlayerInput->rightGrip = ControllerInputPoller.GripFloat(XRNode.RightHand);
			Bindings.LocalPlayerInput->leftTrigger = ControllerInputPoller.TriggerFloat(XRNode.LeftHand);
			Bindings.LocalPlayerInput->rightTrigger = ControllerInputPoller.TriggerFloat(XRNode.RightHand);
			Vector2 vector = ControllerInputPoller.Primary2DAxis(XRNode.LeftHand);
			Vector2 vector2 = ControllerInputPoller.Primary2DAxis(XRNode.RightHand);
			Bindings.LocalPlayerInput->leftXAxis = vector.x;
			Bindings.LocalPlayerInput->leftYAxis = vector.y;
			Bindings.LocalPlayerInput->rightXAxis = vector2.x;
			Bindings.LocalPlayerInput->rightYAxis = vector2.y;
		}
	}

	// Token: 0x06004C37 RID: 19511 RVA: 0x00198008 File Offset: 0x00196208
	public unsafe static void Vec3Builder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Vector3>("Vec3").AddField("x", null).AddField("y", null).AddField("z", null).AddStaticFunction("new", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.New))).AddFunction("__add", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Add))).AddFunction("__sub", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Sub))).AddFunction("__mul", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Mul))).AddFunction("__div", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Div))).AddFunction("__unm", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Unm))).AddFunction("__eq", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Eq))).AddFunction("__tostring", new lua_CFunction(Bindings.Vec3Functions.ToString)).AddFunction("toString", new lua_CFunction(Bindings.Vec3Functions.ToString)).AddFunction("dot", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Dot))).AddFunction("cross", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Cross))).AddFunction("projectOnTo", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Project))).AddFunction("length", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Length))).AddFunction("normalize", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Normalize))).AddFunction("getSafeNormal", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.SafeNormal))).AddStaticFunction("rotate", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Rotate))).AddFunction("rotate", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Rotate))).AddStaticFunction("distance", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Distance))).AddFunction("distance", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Distance))).AddStaticFunction("lerp", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Lerp))).AddFunction("lerp", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Lerp))).AddProperty("zeroVector", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.ZeroVector))).AddProperty("oneVector", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.OneVector))).AddStaticFunction("nearlyEqual", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.NearlyEqual))).Build(L, true));
	}

	// Token: 0x06004C38 RID: 19512 RVA: 0x001982D0 File Offset: 0x001964D0
	public unsafe static void QuatBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Quaternion>("Quat").AddField("x", null).AddField("y", null).AddField("z", null).AddField("w", null).AddStaticFunction("new", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.New))).AddFunction("__mul", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.Mul))).AddFunction("__eq", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.Eq))).AddFunction("__tostring", new lua_CFunction(Bindings.QuatFunctions.ToString)).AddFunction("toString", new lua_CFunction(Bindings.QuatFunctions.ToString)).AddStaticFunction("fromEuler", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.FromEuler))).AddStaticFunction("fromDirection", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.FromDirection))).AddFunction("getUpVector", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.GetUpVector))).AddFunction("euler", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.Euler))).Build(L, true));
	}

	// Token: 0x06004C39 RID: 19513 RVA: 0x00198410 File Offset: 0x00196610
	public unsafe static void PlayerBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.LuauPlayer>("Player").AddField("playerID", "PlayerID").AddField("playerName", "PlayerName").AddField("playerMaterial", "PlayerMaterial").AddField("isMasterClient", "IsMasterClient").AddField("bodyPosition", "BodyPosition").AddField("velocity", "Velocity").AddField("isPCVR", "IsPCVR").AddField("leftHandPosition", "LeftHandPosition").AddField("rightHandPosition", "RightHandPosition").AddField("headRotation", "HeadRotation").AddField("leftHandRotation", "LeftHandRotation").AddField("rightHandRotation", "RightHandRotation").AddField("isInVStump", "IsInVStump").AddField("isEntityAuthority", "IsEntityAuthority").AddStaticFunction("getPlayerByID", new lua_CFunction(Bindings.PlayerFunctions.GetPlayerByID)).Build(L, true));
	}

	// Token: 0x06004C3A RID: 19514 RVA: 0x00198524 File Offset: 0x00196724
	public unsafe static void AIAgentBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.LuauAIAgent>("AIAgent").AddField("entityID", "EntityID").AddField("agentPosition", "EntityPosition").AddField("agentRotation", "EntityRotation").AddFunction("__tostring", new lua_CFunction(Bindings.AIAgentFunctions.ToString)).AddFunction("toString", new lua_CFunction(Bindings.AIAgentFunctions.ToString)).AddFunction("setDestination", new lua_CFunction(Bindings.AIAgentFunctions.SetDestination)).AddFunction("destroyAgent", new lua_CFunction(Bindings.AIAgentFunctions.DestroyEntity)).AddFunction("playAgentAnimation", new lua_CFunction(Bindings.AIAgentFunctions.PlayAgentAnimation)).AddFunction("getTargetPlayer", new lua_CFunction(Bindings.AIAgentFunctions.GetTarget)).AddFunction("setTargetPlayer", new lua_CFunction(Bindings.AIAgentFunctions.SetTarget)).AddStaticFunction("findPrePlacedAIAgentByID", new lua_CFunction(Bindings.AIAgentFunctions.FindPrePlacedAIAgentByID)).AddStaticFunction("getAIAgentByEntityID", new lua_CFunction(Bindings.AIAgentFunctions.GetAIAgentByEntityID)).AddStaticFunction("spawnAIAgent", new lua_CFunction(Bindings.AIAgentFunctions.SpawnAIAgent)).Build(L, true));
	}

	// Token: 0x06004C3B RID: 19515 RVA: 0x00198658 File Offset: 0x00196858
	public unsafe static void GrabbableEntityBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.LuauGrabbableEntity>("GrabbableEntity").AddField("entityID", "EntityID").AddField("entityPosition", "EntityPosition").AddField("entityRotation", "EntityRotation").AddFunction("__tostring", new lua_CFunction(Bindings.GrabbableEntityFunctions.ToString)).AddFunction("toString", new lua_CFunction(Bindings.GrabbableEntityFunctions.ToString)).AddFunction("destroyGrabbable", new lua_CFunction(Bindings.GrabbableEntityFunctions.DestroyEntity)).AddStaticFunction("findPrePlacedGrabbableEntityByID", new lua_CFunction(Bindings.GrabbableEntityFunctions.FindPrePlacedGrabbableEntityByID)).AddStaticFunction("getGrabbableEntityByEntityID", new lua_CFunction(Bindings.GrabbableEntityFunctions.GetGrabbableEntityByEntityID)).AddStaticFunction("getHoldingActorNumberByEntityID", new lua_CFunction(Bindings.GrabbableEntityFunctions.GetHoldingActorNumberByEntityID)).AddStaticFunction("getHoldingActorNumberByLuauID", new lua_CFunction(Bindings.GrabbableEntityFunctions.GetHoldingActorNumberByLuauID)).AddStaticFunction("spawnGrabbableEntity", new lua_CFunction(Bindings.GrabbableEntityFunctions.SpawnGrabbableEntity)).Build(L, true));
	}

	// Token: 0x06004C3C RID: 19516 RVA: 0x00198760 File Offset: 0x00196960
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int LuaStartVibration(lua_State* L)
	{
		bool forLeftController = Luau.lua_toboolean(L, 1) == 1;
		float amplitude = (float)Luau.luaL_checknumber(L, 2);
		float duration = (float)Luau.luaL_checknumber(L, 3);
		GorillaTagger.Instance.StartVibration(forLeftController, amplitude, duration);
		return 0;
	}

	// Token: 0x06004C3D RID: 19517 RVA: 0x00198798 File Offset: 0x00196998
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int LuaPlaySound(lua_State* L)
	{
		int num = (int)Luau.luaL_checknumber(L, 1);
		Vector3 position = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
		float volume = (float)Luau.luaL_checknumber(L, 3);
		if (num < 0 || num >= VRRig.LocalRig.clipToPlay.Length)
		{
			return 0;
		}
		AudioSource.PlayClipAtPoint(VRRig.LocalRig.clipToPlay[num], position, volume);
		return 0;
	}

	// Token: 0x06004C3E RID: 19518 RVA: 0x001987F8 File Offset: 0x001969F8
	public unsafe static void RoomStateBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.LuauRoomState>("RState").AddField("isQuest", "IsQuest").AddField("fps", "FPS").AddField("isPrivate", "IsPrivate").AddField("code", "RoomCode").Build(L, false));
		Bindings.RoomState = Luau.lua_class_push<Bindings.LuauRoomState>(L);
		Bindings.UpdateRoomState();
		Bindings.RoomState->IsQuest = false;
		Bindings.RoomState->IsPrivate = !PhotonNetwork.CurrentRoom.IsVisible;
		Bindings.RoomState->RoomCode = PhotonNetwork.CurrentRoom.Name;
		Luau.lua_setglobal(L, "Room");
	}

	// Token: 0x06004C3F RID: 19519 RVA: 0x001988B3 File Offset: 0x00196AB3
	public unsafe static void UpdateRoomState()
	{
		Bindings.RoomState->FPS = 1f / Time.smoothDeltaTime;
	}

	// Token: 0x04005F5C RID: 24412
	public static Dictionary<GameObject, IntPtr> LuauGameObjectList = new Dictionary<GameObject, IntPtr>();

	// Token: 0x04005F5D RID: 24413
	public static List<KeyValuePair<GameObject, IntPtr>> LuauGameObjectDepthList = new List<KeyValuePair<GameObject, IntPtr>>();

	// Token: 0x04005F5E RID: 24414
	public static Dictionary<IntPtr, GameObject> LuauGameObjectListReverse = new Dictionary<IntPtr, GameObject>();

	// Token: 0x04005F5F RID: 24415
	public static Dictionary<GameObject, Bindings.LuauGameObjectInitialState> LuauGameObjectStates = new Dictionary<GameObject, Bindings.LuauGameObjectInitialState>();

	// Token: 0x04005F60 RID: 24416
	public static Dictionary<GameObject, int> LuauTriggerCallbacks = new Dictionary<GameObject, int>();

	// Token: 0x04005F61 RID: 24417
	public static Dictionary<int, IntPtr> LuauPlayerList = new Dictionary<int, IntPtr>();

	// Token: 0x04005F62 RID: 24418
	public static Dictionary<int, VRRig> LuauVRRigList = new Dictionary<int, VRRig>();

	// Token: 0x04005F63 RID: 24419
	public unsafe static Bindings.GorillaLocomotionSettings* LocomotionSettings;

	// Token: 0x04005F64 RID: 24420
	public unsafe static Bindings.PlayerInput* LocalPlayerInput;

	// Token: 0x04005F65 RID: 24421
	public unsafe static Bindings.LuauRoomState* RoomState;

	// Token: 0x04005F66 RID: 24422
	public static Dictionary<int, IntPtr> LuauAIAgentList = new Dictionary<int, IntPtr>();

	// Token: 0x04005F67 RID: 24423
	public static Dictionary<int, IntPtr> LuauGrabbablesList = new Dictionary<int, IntPtr>();

	// Token: 0x02000BEC RID: 3052
	public static class LuaEmit
	{
		// Token: 0x06004C41 RID: 19521 RVA: 0x00198934 File Offset: 0x00196B34
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Emit(lua_State* L)
		{
			if (Bindings.LuaEmit.callTime < Time.time - 1f)
			{
				Bindings.LuaEmit.callTime = Time.time - 1f;
			}
			Bindings.LuaEmit.callTime += 1f / Bindings.LuaEmit.callCount;
			if (Bindings.LuaEmit.callTime > Time.time)
			{
				LuauHud.Instance.LuauLog("Emit rate limit reached, event not sent");
				return 0;
			}
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions
			{
				Receivers = ReceiverGroup.Others
			};
			if (Luau.lua_type(L, 2) != 6)
			{
				Luau.luaL_errorL(L, "Argument 2 must be a table", Array.Empty<string>());
				return 0;
			}
			Luau.lua_pushnil(L);
			int num = 0;
			List<object> list = new List<object>();
			list.Add(Marshal.PtrToStringAnsi((IntPtr)((void*)Luau.luaL_checkstring(L, 1))));
			while (Luau.lua_next(L, 2) != 0 && num++ < 10)
			{
				Luau.lua_Types lua_Types = (Luau.lua_Types)Luau.lua_type(L, -1);
				if (lua_Types <= Luau.lua_Types.LUA_TNUMBER)
				{
					if (lua_Types == Luau.lua_Types.LUA_TBOOLEAN)
					{
						list.Add(Luau.lua_toboolean(L, -1) == 1);
						Luau.lua_pop(L, 1);
						continue;
					}
					if (lua_Types == Luau.lua_Types.LUA_TNUMBER)
					{
						list.Add(Luau.luaL_checknumber(L, -1));
						Luau.lua_pop(L, 1);
						continue;
					}
				}
				else if (lua_Types == Luau.lua_Types.LUA_TTABLE || lua_Types == Luau.lua_Types.LUA_TUSERDATA)
				{
					Luau.luaL_getmetafield(L, -1, "metahash");
					BurstClassInfo.ClassInfo classInfo;
					if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), out classInfo))
					{
						FixedString64Bytes fixedString64Bytes = "\"Internal Class Info Error No Metatable Found\"";
						Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString64Bytes>(ref fixedString64Bytes) + 2));
						return 0;
					}
					Luau.lua_pop(L, 1);
					FixedString32Bytes fixedString32Bytes = "Vec3";
					if (classInfo.Name == fixedString32Bytes)
					{
						list.Add(*Luau.lua_class_get<Vector3>(L, -1));
						Luau.lua_pop(L, 1);
						continue;
					}
					fixedString32Bytes = "Quat";
					if (classInfo.Name == fixedString32Bytes)
					{
						list.Add(*Luau.lua_class_get<Quaternion>(L, -1));
						Luau.lua_pop(L, 1);
						continue;
					}
					fixedString32Bytes = "Player";
					if (classInfo.Name == fixedString32Bytes)
					{
						int playerID = Luau.lua_class_get<Bindings.LuauPlayer>(L, -1)->PlayerID;
						NetPlayer netPlayer = null;
						foreach (NetPlayer netPlayer2 in RoomSystem.PlayersInRoom)
						{
							if (netPlayer2.ActorNumber == playerID)
							{
								netPlayer = netPlayer2;
							}
						}
						if (netPlayer == null)
						{
							list.Add(null);
						}
						else
						{
							list.Add(netPlayer.GetPlayerRef());
						}
						Luau.lua_pop(L, 1);
						continue;
					}
					FixedString32Bytes fixedString32Bytes2 = "\"Unknown Type in table\"";
					Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
					continue;
				}
				FixedString32Bytes fixedString32Bytes3 = "\"Unknown Type in table\"";
				Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes3) + 2));
				return 0;
			}
			if (PhotonNetwork.InRoom)
			{
				PhotonNetwork.RaiseEvent(180, list.ToArray(), raiseEventOptions, SendOptions.SendReliable);
			}
			return 0;
		}

		// Token: 0x04005F68 RID: 24424
		private static float callTime = 0f;

		// Token: 0x04005F69 RID: 24425
		private static float callCount = 20f;
	}

	// Token: 0x02000BED RID: 3053
	[BurstCompile]
	public struct LuauGameObject
	{
		// Token: 0x04005F6A RID: 24426
		public Vector3 Position;

		// Token: 0x04005F6B RID: 24427
		public Quaternion Rotation;

		// Token: 0x04005F6C RID: 24428
		public Vector3 Scale;
	}

	// Token: 0x02000BEE RID: 3054
	[BurstCompile]
	public struct LuauGameObjectInitialState
	{
		// Token: 0x04005F6D RID: 24429
		public Vector3 Position;

		// Token: 0x04005F6E RID: 24430
		public Quaternion Rotation;

		// Token: 0x04005F6F RID: 24431
		public Vector3 Scale;

		// Token: 0x04005F70 RID: 24432
		public bool Visible;

		// Token: 0x04005F71 RID: 24433
		public bool Collidable;

		// Token: 0x04005F72 RID: 24434
		public bool Created;
	}

	// Token: 0x02000BEF RID: 3055
	[BurstCompile]
	public static class GameObjectFunctions
	{
		// Token: 0x06004C43 RID: 19523 RVA: 0x00198C40 File Offset: 0x00196E40
		public static int GetDepth(GameObject gameObject)
		{
			int num = 0;
			Transform transform = gameObject.transform;
			while (transform.parent != null)
			{
				num++;
				transform = transform.parent;
			}
			return num;
		}

		// Token: 0x06004C44 RID: 19524 RVA: 0x00198C72 File Offset: 0x00196E72
		public static void UpdateDepthList()
		{
			Bindings.LuauGameObjectDepthList.Clear();
			Bindings.LuauGameObjectDepthList = (from kv in Bindings.LuauGameObjectList
			orderby Bindings.GameObjectFunctions.GetDepth(kv.Key) descending
			select kv).ToList<KeyValuePair<GameObject, IntPtr>>();
		}

		// Token: 0x06004C45 RID: 19525 RVA: 0x00198CB4 File Offset: 0x00196EB4
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int New(lua_State* L)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			Bindings.LuauGameObject* ptr = Luau.lua_class_push<Bindings.LuauGameObject>(L);
			ptr->Position = gameObject.transform.position;
			ptr->Rotation = gameObject.transform.rotation;
			ptr->Scale = gameObject.transform.localScale;
			Bindings.LuauGameObjectList.TryAdd(gameObject, (IntPtr)((void*)ptr));
			Bindings.LuauGameObjectListReverse.TryAdd((IntPtr)((void*)ptr), gameObject);
			return 1;
		}

		// Token: 0x06004C46 RID: 19526 RVA: 0x00198D28 File Offset: 0x00196F28
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FindGameObject(lua_State* L)
		{
			GameObject gameObject = GameObject.Find(new string((sbyte*)Luau.luaL_checkstring(L, 1)));
			if (!(gameObject != null))
			{
				return 0;
			}
			if (!CustomMapLoader.IsCustomScene(gameObject.scene.name))
			{
				return 0;
			}
			IntPtr ptr;
			if (Bindings.LuauGameObjectList.TryGetValue(gameObject, out ptr))
			{
				Luau.lua_class_push(L, "GameObject", ptr);
			}
			else
			{
				Bindings.LuauGameObject* ptr2 = Luau.lua_class_push<Bindings.LuauGameObject>(L);
				ptr2->Position = gameObject.transform.position;
				ptr2->Rotation = gameObject.transform.rotation;
				ptr2->Scale = gameObject.transform.localScale;
				Bindings.LuauGameObjectInitialState value = default(Bindings.LuauGameObjectInitialState);
				value.Position = gameObject.transform.localPosition;
				value.Rotation = gameObject.transform.localRotation;
				value.Scale = gameObject.transform.localScale;
				value.Visible = true;
				value.Collidable = true;
				value.Created = false;
				MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
				Collider component2 = gameObject.GetComponent<Collider>();
				if (component2.IsNotNull())
				{
					value.Collidable = component2.enabled;
				}
				if (component.IsNotNull())
				{
					value.Visible = component.enabled;
				}
				Bindings.LuauGameObjectList.TryAdd(gameObject, (IntPtr)((void*)ptr2));
				Bindings.LuauGameObjectListReverse.TryAdd((IntPtr)((void*)ptr2), gameObject);
				Bindings.LuauGameObjectStates.TryAdd(gameObject, value);
				Bindings.GameObjectFunctions.UpdateDepthList();
			}
			return 1;
		}

		// Token: 0x06004C47 RID: 19527 RVA: 0x00198E98 File Offset: 0x00197098
		public static Transform FindChild(Transform parent, string name)
		{
			foreach (object obj in parent)
			{
				Transform transform = (Transform)obj;
				if (transform.name == name)
				{
					return transform;
				}
				Transform transform2 = Bindings.GameObjectFunctions.FindChild(transform, name);
				if (transform2 != null)
				{
					return transform2;
				}
			}
			return null;
		}

		// Token: 0x06004C48 RID: 19528 RVA: 0x00198F14 File Offset: 0x00197114
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FindChildGameObject(lua_State* L)
		{
			Bindings.LuauGameObject* value = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)value), out gameObject))
			{
				string name = new string((sbyte*)Luau.luaL_checkstring(L, 2));
				Transform transform = Bindings.GameObjectFunctions.FindChild(gameObject.transform, name);
				GameObject gameObject2 = (transform != null) ? transform.gameObject : null;
				if (gameObject2.IsNotNull())
				{
					IntPtr ptr;
					if (Bindings.LuauGameObjectList.TryGetValue(gameObject2, out ptr))
					{
						Luau.lua_class_push(L, "GameObject", ptr);
					}
					else
					{
						Bindings.LuauGameObject* ptr2 = Luau.lua_class_push<Bindings.LuauGameObject>(L);
						ptr2->Position = gameObject2.transform.position;
						ptr2->Rotation = gameObject2.transform.rotation;
						ptr2->Scale = gameObject2.transform.localScale;
						Bindings.LuauGameObjectInitialState value2 = default(Bindings.LuauGameObjectInitialState);
						value2.Position = gameObject2.transform.localPosition;
						value2.Rotation = gameObject2.transform.localRotation;
						value2.Scale = gameObject2.transform.localScale;
						value2.Visible = true;
						value2.Collidable = true;
						value2.Created = false;
						MeshRenderer component = gameObject2.GetComponent<MeshRenderer>();
						Collider component2 = gameObject2.GetComponent<Collider>();
						if (component2.IsNotNull())
						{
							value2.Collidable = component2.enabled;
						}
						if (component.IsNotNull())
						{
							value2.Visible = component.enabled;
						}
						Bindings.LuauGameObjectList.TryAdd(gameObject2, (IntPtr)((void*)ptr2));
						Bindings.LuauGameObjectListReverse.TryAdd((IntPtr)((void*)ptr2), gameObject2);
						Bindings.LuauGameObjectStates.TryAdd(gameObject2, value2);
						Bindings.GameObjectFunctions.UpdateDepthList();
					}
					return 1;
				}
			}
			return 0;
		}

		// Token: 0x06004C49 RID: 19529 RVA: 0x001990B0 File Offset: 0x001972B0
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FindComponent(lua_State* L)
		{
			Bindings.LuauGameObject* value = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)value), out gameObject))
			{
				if (gameObject == null)
				{
					return 0;
				}
				string a = new string((sbyte*)Luau.luaL_checkstring(L, 2));
				if (a == "ParticleSystem")
				{
					ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
					if (component == null)
					{
						return 0;
					}
					Bindings.Components.LuauParticleSystemBindings.LuauParticleSystem* value2 = Luau.lua_class_push<Bindings.Components.LuauParticleSystemBindings.LuauParticleSystem>(L);
					Bindings.Components.ComponentList.TryAdd((IntPtr)((void*)value2), component);
					return 1;
				}
				else if (a == "AudioSource")
				{
					AudioSource component2 = gameObject.GetComponent<AudioSource>();
					if (component2 == null)
					{
						return 0;
					}
					Bindings.Components.LuauAudioSourceBindings.LuauAudioSource* value3 = Luau.lua_class_push<Bindings.Components.LuauAudioSourceBindings.LuauAudioSource>(L);
					Bindings.Components.ComponentList.TryAdd((IntPtr)((void*)value3), component2);
					return 1;
				}
				else if (a == "Light")
				{
					Light component3 = gameObject.GetComponent<Light>();
					if (component3 == null)
					{
						return 0;
					}
					Bindings.Components.LuauLightBindings.LuauLight* value4 = Luau.lua_class_push<Bindings.Components.LuauLightBindings.LuauLight>(L);
					Bindings.Components.ComponentList.TryAdd((IntPtr)((void*)value4), component3);
					return 1;
				}
				else if (a == "Animator")
				{
					Animator component4 = gameObject.GetComponent<Animator>();
					if (component4 == null)
					{
						return 0;
					}
					Bindings.Components.LuauAnimatorBindings.LuauAnimator* value5 = Luau.lua_class_push<Bindings.Components.LuauAnimatorBindings.LuauAnimator>(L);
					Bindings.Components.ComponentList.TryAdd((IntPtr)((void*)value5), component4);
					return 1;
				}
			}
			return 0;
		}

		// Token: 0x06004C4A RID: 19530 RVA: 0x001991F8 File Offset: 0x001973F8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int CloneGameObject(lua_State* L)
		{
			Bindings.LuauGameObject* value = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)value), out gameObject))
			{
				GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, gameObject.transform.parent, false);
				Bindings.LuauGameObject* ptr = Luau.lua_class_push<Bindings.LuauGameObject>(L);
				ptr->Position = gameObject2.transform.position;
				ptr->Rotation = gameObject2.transform.rotation;
				ptr->Scale = gameObject2.transform.localScale;
				Bindings.LuauGameObjectInitialState value2 = default(Bindings.LuauGameObjectInitialState);
				value2.Position = gameObject2.transform.localPosition;
				value2.Rotation = gameObject2.transform.localRotation;
				value2.Scale = gameObject2.transform.localScale;
				value2.Visible = true;
				value2.Collidable = true;
				value2.Created = true;
				MeshRenderer component = gameObject2.GetComponent<MeshRenderer>();
				Collider component2 = gameObject2.GetComponent<Collider>();
				if (component2.IsNotNull())
				{
					value2.Collidable = component2.enabled;
				}
				if (component.IsNotNull())
				{
					value2.Visible = component.enabled;
				}
				Bindings.LuauGameObjectList.TryAdd(gameObject2, (IntPtr)((void*)ptr));
				Bindings.LuauGameObjectListReverse.TryAdd((IntPtr)((void*)ptr), gameObject2);
				Bindings.LuauGameObjectStates.TryAdd(gameObject2, value2);
				Bindings.GameObjectFunctions.UpdateDepthList();
				return 1;
			}
			return 0;
		}

		// Token: 0x06004C4B RID: 19531 RVA: 0x0019934C File Offset: 0x0019754C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int DestroyGameObject(lua_State* L)
		{
			Bindings.LuauGameObject* value = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			Bindings.LuauGameObjectInitialState luauGameObjectInitialState;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)value), out gameObject) && Bindings.LuauGameObjectStates.TryGetValue(gameObject, out luauGameObjectInitialState))
			{
				if (!luauGameObjectInitialState.Created)
				{
					Luau.luaL_errorL(L, "Cannot destroy a non-instantiated GameObject.", Array.Empty<string>());
					return 0;
				}
				Queue<GameObject> queue = new Queue<GameObject>();
				queue.Enqueue(gameObject);
				while (queue.Count != 0)
				{
					GameObject gameObject2 = queue.Dequeue();
					IntPtr key;
					if (Bindings.LuauGameObjectList.TryGetValue(gameObject2, out key))
					{
						Bindings.LuauGameObjectList.Remove(gameObject2);
						Bindings.LuauGameObjectListReverse.Remove(key);
						Bindings.LuauGameObjectStates.Remove(gameObject2);
						foreach (object obj in gameObject2.transform)
						{
							Transform transform = (Transform)obj;
							queue.Enqueue(transform.gameObject);
						}
					}
				}
				Bindings.GameObjectFunctions.UpdateDepthList();
				gameObject.Destroy();
			}
			return 0;
		}

		// Token: 0x06004C4C RID: 19532 RVA: 0x00199478 File Offset: 0x00197678
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetCollision(lua_State* L)
		{
			Bindings.LuauGameObject* value = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)value), out gameObject))
			{
				Collider component = gameObject.GetComponent<Collider>();
				if (component.IsNotNull())
				{
					component.enabled = (Luau.lua_toboolean(L, 2) == 1);
				}
			}
			return 0;
		}

		// Token: 0x06004C4D RID: 19533 RVA: 0x001994CC File Offset: 0x001976CC
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetVisibility(lua_State* L)
		{
			Bindings.LuauGameObject* value = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)value), out gameObject))
			{
				MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
				if (component.IsNotNull())
				{
					component.enabled = (Luau.lua_toboolean(L, 2) == 1);
				}
			}
			return 0;
		}

		// Token: 0x06004C4E RID: 19534 RVA: 0x00199520 File Offset: 0x00197720
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetActive(lua_State* L)
		{
			Bindings.LuauGameObject* value = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)value), out gameObject))
			{
				gameObject.SetActive(Luau.lua_toboolean(L, 2) == 1);
			}
			return 0;
		}

		// Token: 0x06004C4F RID: 19535 RVA: 0x00199564 File Offset: 0x00197764
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetText(lua_State* L)
		{
			Bindings.LuauGameObject* value = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)value), out gameObject))
			{
				string text = new string(Luau.lua_tostring(L, 2));
				TextMeshPro component = gameObject.GetComponent<TextMeshPro>();
				if (component.IsNotNull())
				{
					component.text = text;
				}
				else
				{
					TextMesh component2 = gameObject.GetComponent<TextMesh>();
					if (component2.IsNotNull())
					{
						component2.text = text;
					}
				}
			}
			return 0;
		}

		// Token: 0x06004C50 RID: 19536 RVA: 0x001995D8 File Offset: 0x001977D8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int OnTouched(lua_State* L)
		{
			Bindings.LuauGameObject* value = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject key;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)value), out key))
			{
				int rid;
				if (Bindings.LuauTriggerCallbacks.TryGetValue(key, out rid))
				{
					Luau.lua_unref(L, rid);
					Bindings.LuauTriggerCallbacks.Remove(key);
				}
				if (Luau.lua_type(L, 2) == 7)
				{
					int value2 = Luau.lua_ref(L, 2);
					Bindings.LuauTriggerCallbacks.TryAdd(key, value2);
				}
				else
				{
					FixedString32Bytes fixedString32Bytes = "Callback must be a function";
					Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
				}
			}
			return 0;
		}

		// Token: 0x06004C51 RID: 19537 RVA: 0x0019966C File Offset: 0x0019786C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetVelocity(lua_State* L)
		{
			Bindings.LuauGameObject* value = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)value), out gameObject))
			{
				Vector3 linearVelocity = *Luau.lua_class_get<Vector3>(L, 2);
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				if (component.IsNotNull())
				{
					component.linearVelocity = linearVelocity;
				}
			}
			return 0;
		}

		// Token: 0x06004C52 RID: 19538 RVA: 0x001996C4 File Offset: 0x001978C4
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetVelocity(lua_State* L)
		{
			Bindings.LuauGameObject* value = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)value), out gameObject))
			{
				if (gameObject.IsNull())
				{
					return 0;
				}
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				Vector3* ptr = Luau.lua_class_push<Vector3>(L, "Vec3");
				if (component.IsNotNull())
				{
					*ptr = component.linearVelocity;
				}
				else
				{
					*ptr = Vector3.zero;
				}
			}
			return 1;
		}

		// Token: 0x06004C53 RID: 19539 RVA: 0x0019973C File Offset: 0x0019793C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetColor(lua_State* L)
		{
			Bindings.LuauGameObject* value = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 2);
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)value), out gameObject))
			{
				Color color = new Color(Mathf.Clamp01(vector.x / 255f), Mathf.Clamp01(vector.y / 255f), Mathf.Clamp01(vector.z / 255f), 1f);
				TextMeshPro component = gameObject.GetComponent<TextMeshPro>();
				if (component != null)
				{
					component.color = color;
					return 0;
				}
				TextMesh component2 = gameObject.GetComponent<TextMesh>();
				if (component2 != null)
				{
					component2.color = color;
					return 0;
				}
				Renderer component3 = gameObject.GetComponent<Renderer>();
				if (component3 != null)
				{
					component3.material.color = color;
				}
			}
			return 0;
		}

		// Token: 0x06004C54 RID: 19540 RVA: 0x00199818 File Offset: 0x00197A18
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Equals(lua_State* L)
		{
			Bindings.LuauGameObject* value = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject x;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)value), out x))
			{
				Bindings.LuauGameObject* value2 = Luau.lua_class_get<Bindings.LuauGameObject>(L, 2, "GameObject");
				GameObject y;
				if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)value2), out y) && x == y)
				{
					Luau.lua_pushboolean(L, 1);
					return 1;
				}
			}
			Luau.lua_pushboolean(L, 0);
			return 1;
		}
	}

	// Token: 0x02000BF1 RID: 3057
	[BurstCompile]
	public struct LuauPlayer
	{
		// Token: 0x04005F75 RID: 24437
		public int PlayerID;

		// Token: 0x04005F76 RID: 24438
		public FixedString32Bytes PlayerName;

		// Token: 0x04005F77 RID: 24439
		public int PlayerMaterial;

		// Token: 0x04005F78 RID: 24440
		[MarshalAs(UnmanagedType.U1)]
		public bool IsMasterClient;

		// Token: 0x04005F79 RID: 24441
		public Vector3 BodyPosition;

		// Token: 0x04005F7A RID: 24442
		public Vector3 Velocity;

		// Token: 0x04005F7B RID: 24443
		[MarshalAs(UnmanagedType.U1)]
		public bool IsPCVR;

		// Token: 0x04005F7C RID: 24444
		public Vector3 LeftHandPosition;

		// Token: 0x04005F7D RID: 24445
		public Vector3 RightHandPosition;

		// Token: 0x04005F7E RID: 24446
		[MarshalAs(UnmanagedType.U1)]
		public bool IsEntityAuthority;

		// Token: 0x04005F7F RID: 24447
		public Quaternion HeadRotation;

		// Token: 0x04005F80 RID: 24448
		public Quaternion LeftHandRotation;

		// Token: 0x04005F81 RID: 24449
		public Quaternion RightHandRotation;

		// Token: 0x04005F82 RID: 24450
		[MarshalAs(UnmanagedType.U1)]
		public bool IsInVStump;
	}

	// Token: 0x02000BF2 RID: 3058
	[BurstCompile]
	public static class PlayerFunctions
	{
		// Token: 0x06004C58 RID: 19544 RVA: 0x001998A8 File Offset: 0x00197AA8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetPlayerByID(lua_State* L)
		{
			int num = (int)Luau.luaL_checknumber(L, 1);
			foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
			{
				if (netPlayer.ActorNumber == num)
				{
					IntPtr ptr;
					if (Bindings.LuauPlayerList.TryGetValue(netPlayer.ActorNumber, out ptr))
					{
						Luau.lua_class_push(L, "Player", ptr);
					}
					else
					{
						Bindings.LuauPlayer* ptr2 = Luau.lua_class_push<Bindings.LuauPlayer>(L);
						ptr2->PlayerID = netPlayer.ActorNumber;
						ptr2->PlayerMaterial = 0;
						ptr2->IsMasterClient = netPlayer.IsMasterClient;
						Bindings.LuauPlayerList[netPlayer.ActorNumber] = (IntPtr)((void*)ptr2);
						GorillaGameManager instance = GorillaGameManager.instance;
						VRRig vrrig = (instance != null) ? instance.FindPlayerVRRig(netPlayer) : null;
						if (vrrig != null)
						{
							ptr2->PlayerName = vrrig.playerNameVisible;
							Bindings.LuauVRRigList[netPlayer.ActorNumber] = vrrig;
							Bindings.PlayerFunctions.UpdatePlayer(L, vrrig, ptr2);
							Bindings.LuauPlayerList[netPlayer.ActorNumber] = (IntPtr)((void*)ptr2);
						}
					}
				}
			}
			return 0;
		}

		// Token: 0x06004C59 RID: 19545 RVA: 0x001999E0 File Offset: 0x00197BE0
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static void UpdatePlayer(lua_State* L, VRRig p, Bindings.LuauPlayer* data)
		{
			data->BodyPosition = p.transform.position;
			data->Velocity = p.LatestVelocity();
			data->LeftHandPosition = p.leftHandTransform.position;
			data->RightHandPosition = p.rightHandTransform.position;
			data->HeadRotation = p.head.rigTarget.rotation;
			data->LeftHandRotation = p.leftHandTransform.rotation;
			data->RightHandRotation = p.rightHandTransform.rotation;
			if (p.isLocal)
			{
				data->IsInVStump = CustomMapManager.IsLocalPlayerInVirtualStump();
			}
			else if (p.creator != null)
			{
				data->IsInVStump = CustomMapManager.IsRemotePlayerInVirtualStump(p.creator.UserId);
			}
			else
			{
				data->IsInVStump = false;
			}
			data->IsEntityAuthority = (CustomMapsGameManager.instance.IsNotNull() && CustomMapsGameManager.instance.gameEntityManager.IsNotNull() && CustomMapsGameManager.instance.gameEntityManager.IsZoneAuthority());
		}
	}

	// Token: 0x02000BF3 RID: 3059
	[BurstCompile]
	public struct LuauAIAgent
	{
		// Token: 0x04005F83 RID: 24451
		public int EntityID;

		// Token: 0x04005F84 RID: 24452
		public Vector3 EntityPosition;

		// Token: 0x04005F85 RID: 24453
		public Quaternion EntityRotation;
	}

	// Token: 0x02000BF4 RID: 3060
	[BurstCompile]
	public struct LuauGrabbableEntity
	{
		// Token: 0x04005F86 RID: 24454
		public int EntityID;

		// Token: 0x04005F87 RID: 24455
		public Vector3 EntityPosition;

		// Token: 0x04005F88 RID: 24456
		public Quaternion EntityRotation;
	}

	// Token: 0x02000BF5 RID: 3061
	[BurstCompile]
	public static class GrabbableEntityFunctions
	{
		// Token: 0x06004C5A RID: 19546 RVA: 0x00199AD8 File Offset: 0x00197CD8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ToString(lua_State* L)
		{
			string s = "NULL";
			Bindings.LuauGrabbableEntity* ptr = Luau.lua_class_get<Bindings.LuauGrabbableEntity>(L, 1);
			if (ptr != null)
			{
				s = string.Concat(new string[]
				{
					"ID: ",
					ptr->EntityID.ToString(),
					" | Pos: ",
					ptr->EntityPosition.ToString(),
					" | Rot: ",
					ptr->EntityRotation.ToString()
				});
			}
			Luau.lua_pushstring(L, s);
			return 1;
		}

		// Token: 0x06004C5B RID: 19547 RVA: 0x00199B5C File Offset: 0x00197D5C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetGrabbableEntityByEntityID(lua_State* L)
		{
			int num = (int)Luau.luaL_checknumber(L, 1);
			Debug.Log(string.Format("[LuauBindings::GetGrabbableEntityByEntityID] ID: {0}", num));
			GameEntityManager gameEntityManager = CustomMapsGameManager.instance.gameEntityManager;
			if (gameEntityManager.IsNotNull())
			{
				GameEntityId entityIdFromNetId = gameEntityManager.GetEntityIdFromNetId(num);
				GameEntity gameEntity = gameEntityManager.GetGameEntity(entityIdFromNetId);
				if (gameEntity.IsNotNull())
				{
					if (gameEntity.gameObject.IsNull())
					{
						return 0;
					}
					Debug.Log("[LuauBindings::GetGrabbableEntityByEntityID] Found agent: " + gameEntity.gameObject.name);
					IntPtr intPtr;
					if (Bindings.LuauGrabbablesList.TryGetValue(num, out intPtr))
					{
						Bindings.GrabbableEntityFunctions.UpdateEntity(gameEntity, (Bindings.LuauGrabbableEntity*)((void*)intPtr));
						Luau.lua_class_push(L, "GrabbableEntity", intPtr);
					}
					else
					{
						Bindings.LuauGrabbableEntity* ptr = Luau.lua_class_push<Bindings.LuauGrabbableEntity>(L);
						Bindings.GrabbableEntityFunctions.UpdateEntity(gameEntity, ptr);
						Bindings.LuauGrabbablesList[num] = (IntPtr)((void*)ptr);
					}
					return 1;
				}
			}
			return 0;
		}

		// Token: 0x06004C5C RID: 19548 RVA: 0x00199C34 File Offset: 0x00197E34
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetHoldingActorNumberByLuauID(lua_State* L)
		{
			short num = (short)Luau.luaL_checknumber(L, 1);
			Debug.Log(string.Format("[LuauBindings::GetHoldingActorNumberByLuauID] ID: {0}", num));
			GameEntityManager gameEntityManager = CustomMapsGameManager.instance.gameEntityManager;
			if (gameEntityManager.IsNull())
			{
				return 0;
			}
			List<GameEntity> gameEntities = gameEntityManager.GetGameEntities();
			for (int i = 0; i < gameEntities.Count; i++)
			{
				if (!gameEntities[i].gameObject.IsNull())
				{
					CustomMapsGrabbablesController component = gameEntities[i].gameObject.GetComponent<CustomMapsGrabbablesController>();
					if (!component.IsNull())
					{
						Debug.Log("[LuauBindings::GetHoldingActorNumberByLuauID] checking GrabbableController on " + string.Format("{0}, id: {1}", component.gameObject.name, component.luaAgentID));
						if (component.luaAgentID == num)
						{
							Luau.lua_pushnumber(L, (double)component.GetGrabbingActor());
							return 1;
						}
					}
				}
			}
			return 0;
		}

		// Token: 0x06004C5D RID: 19549 RVA: 0x00199D0C File Offset: 0x00197F0C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetHoldingActorNumberByEntityID(lua_State* L)
		{
			int num = (int)Luau.luaL_checknumber(L, 1);
			Debug.Log(string.Format("[LuauBindings::GetHoldingActorNumberByEntityID] ID: {0}", num));
			GameEntityManager gameEntityManager = CustomMapsGameManager.instance.gameEntityManager;
			if (gameEntityManager.IsNull())
			{
				return 0;
			}
			GameEntityId entityIdFromNetId = gameEntityManager.GetEntityIdFromNetId(num);
			GameEntity gameEntity = gameEntityManager.GetGameEntity(entityIdFromNetId);
			if (gameEntity.IsNotNull() || gameEntity.gameObject.IsNull())
			{
				return 0;
			}
			Debug.Log("[LuauBindings::GetHoldingActorNumberByEntityID] Found agent: " + gameEntity.gameObject.name);
			CustomMapsGrabbablesController component = gameEntity.gameObject.GetComponent<CustomMapsGrabbablesController>();
			if (component.IsNull())
			{
				return 0;
			}
			Luau.lua_pushnumber(L, (double)component.GetGrabbingActor());
			return 1;
		}

		// Token: 0x06004C5E RID: 19550 RVA: 0x00199DB4 File Offset: 0x00197FB4
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FindPrePlacedGrabbableEntityByID(lua_State* L)
		{
			short num = (short)Luau.luaL_checknumber(L, 1);
			Debug.Log(string.Format("[LuauBindings::FindPrePlacedGrabbableEntityByID] ID: {0}", num));
			GameEntityManager gameEntityManager = CustomMapsGameManager.instance.gameEntityManager;
			if (gameEntityManager.IsNotNull())
			{
				List<GameEntity> gameEntities = gameEntityManager.GetGameEntities();
				for (int i = 0; i < gameEntities.Count; i++)
				{
					if (!gameEntities[i].gameObject.IsNull())
					{
						CustomMapsGrabbablesController component = gameEntities[i].gameObject.GetComponent<CustomMapsGrabbablesController>();
						if (!component.IsNull())
						{
							Debug.Log("[LuauBindings::FindPrePlacedGrabbableEntityByID] checking GrabbableController on " + string.Format("{0}, id: {1}", component.gameObject.name, component.luaAgentID));
							if (component.luaAgentID == num)
							{
								IntPtr intPtr;
								if (Bindings.LuauGrabbablesList.TryGetValue(gameEntities[i].GetNetId(), out intPtr))
								{
									Bindings.GrabbableEntityFunctions.UpdateEntity(gameEntities[i], (Bindings.LuauGrabbableEntity*)((void*)intPtr));
									Luau.lua_class_push(L, "GrabbableEntity", intPtr);
								}
								else
								{
									Bindings.LuauGrabbableEntity* ptr = Luau.lua_class_push<Bindings.LuauGrabbableEntity>(L);
									Bindings.GrabbableEntityFunctions.UpdateEntity(gameEntities[i], ptr);
									Bindings.LuauGrabbablesList[gameEntities[i].GetNetId()] = (IntPtr)((void*)ptr);
								}
								return 1;
							}
						}
					}
				}
			}
			return 0;
		}

		// Token: 0x06004C5F RID: 19551 RVA: 0x00199EFC File Offset: 0x001980FC
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SpawnGrabbableEntity(lua_State* L)
		{
			Debug.Log("[LuauBindings::SpawnGrabbableEntity]");
			CustomMapsGameManager instance = CustomMapsGameManager.instance;
			GameEntityManager gameEntityManager = instance.IsNotNull() ? instance.gameEntityManager : null;
			if (gameEntityManager.IsNull())
			{
				LuauHud.Instance.LuauLog("SpawnGrabbableEntity failed. EntityManager is null.");
				return 0;
			}
			if (!gameEntityManager.IsZoneAuthority())
			{
				LuauHud.Instance.LuauLog("SpawnGrabbableEntity failed. Local Player doesn't have Entity Authority.");
				return 0;
			}
			if (Bindings.LuauAIAgentList.Count + Bindings.LuauGrabbablesList.Count == Constants.aiAgentLimit)
			{
				LuauHud.Instance.LuauLog(string.Format("SpawnGrabbableEntity failed, EntityLimit of {0}", Constants.aiAgentLimit) + " has already been reached.");
				return 0;
			}
			int enemyTypeId = (int)Luau.luaL_checknumber(L, 1);
			Vector3 position = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			Quaternion rotation = *Luau.lua_class_get<Quaternion>(L, 3, "Quat");
			GameEntityId id = instance.SpawnGrabbableAtLocation(enemyTypeId, position, rotation);
			Debug.Log("[LuauBindings::SpawnGrabbableEntity] spawnedGrabbable");
			if (!id.IsValid())
			{
				LuauHud.Instance.LuauLog("SpawnGrabbableEntity failed to create entity.");
				return 0;
			}
			Debug.Log("[LuauBindings::SpawnGrabbableEntity] spawnedGrabbable ID valid");
			GameEntity gameEntity = gameEntityManager.GetGameEntity(id);
			IntPtr intPtr;
			if (Bindings.LuauGrabbablesList.TryGetValue(gameEntity.GetNetId(), out intPtr))
			{
				Debug.Log("[LuauBindings::SpawnGrabbableEntity] fround grabbable");
				Bindings.GrabbableEntityFunctions.UpdateEntity(gameEntity, (Bindings.LuauGrabbableEntity*)((void*)intPtr));
				Luau.lua_class_push(L, "GrabbableEntity", intPtr);
				return 1;
			}
			Debug.Log("[LuauBindings::SpawnGrabbableEntity] grabbable not found");
			Luau.lua_getglobal(L, "GrabbableEntities");
			Bindings.LuauGrabbableEntity* ptr = Luau.lua_class_push<Bindings.LuauGrabbableEntity>(L);
			Bindings.GrabbableEntityFunctions.UpdateEntity(gameEntity, ptr);
			Bindings.LuauGrabbablesList[gameEntity.GetNetId()] = (IntPtr)((void*)ptr);
			Debug.Log("[LuauBindings::SpawnGrabbableEntity] created new grabbable");
			Luau.lua_rawseti(L, -2, Bindings.LuauGrabbablesList.Count);
			Luau.lua_pop(L, 1);
			Debug.Log("[LuauBindings::SpawnGrabbableEntity] pushing new grabbable");
			Luau.lua_class_push(L, "GrabbableEntity", (IntPtr)((void*)ptr));
			return 1;
		}

		// Token: 0x06004C60 RID: 19552 RVA: 0x0019A0E6 File Offset: 0x001982E6
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static void UpdateEntity(GameEntity entity, Bindings.LuauGrabbableEntity* luaAgent)
		{
			luaAgent->EntityID = entity.GetNetId();
			luaAgent->EntityPosition = entity.transform.position;
			luaAgent->EntityRotation = entity.transform.rotation;
		}

		// Token: 0x06004C61 RID: 19553 RVA: 0x0019A118 File Offset: 0x00198318
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int DestroyEntity(lua_State* L)
		{
			Bindings.LuauGrabbableEntity* ptr = Luau.lua_class_get<Bindings.LuauGrabbableEntity>(L, 1);
			if (ptr != null)
			{
				GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
				if (entityManager.IsNotNull())
				{
					GameEntityId entityIdFromNetId = entityManager.GetEntityIdFromNetId(ptr->EntityID);
					entityManager.RequestDestroyItem(entityIdFromNetId);
				}
			}
			return 0;
		}
	}

	// Token: 0x02000BF6 RID: 3062
	[BurstCompile]
	public static class AIAgentFunctions
	{
		// Token: 0x06004C62 RID: 19554 RVA: 0x0019A158 File Offset: 0x00198358
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ToString(lua_State* L)
		{
			string s = "NULL";
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			if (ptr != null)
			{
				s = string.Concat(new string[]
				{
					"ID: ",
					ptr->EntityID.ToString(),
					" | Pos: ",
					ptr->EntityPosition.ToString(),
					" | Rot: ",
					ptr->EntityRotation.ToString()
				});
			}
			Luau.lua_pushstring(L, s);
			return 1;
		}

		// Token: 0x06004C63 RID: 19555 RVA: 0x0019A1DC File Offset: 0x001983DC
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetAIAgentByEntityID(lua_State* L)
		{
			int num = (int)Luau.luaL_checknumber(L, 1);
			Debug.Log(string.Format("[LuauBindings::GetAIAgentByEntityID] ID: {0}", num));
			GameEntityManager gameEntityManager = CustomMapsGameManager.instance.gameEntityManager;
			if (gameEntityManager.IsNotNull())
			{
				GameEntityId entityIdFromNetId = gameEntityManager.GetEntityIdFromNetId(num);
				GameEntity gameEntity = gameEntityManager.GetGameEntity(entityIdFromNetId);
				if (gameEntity.IsNotNull())
				{
					if (gameEntity.gameObject.IsNull())
					{
						return 0;
					}
					if (gameEntity.gameObject.GetComponent<GameAgent>().IsNotNull())
					{
						Debug.Log("[LuauBindings::GetAIAgentByEntityID] Found agent: " + gameEntity.gameObject.name);
						IntPtr intPtr;
						if (Bindings.LuauAIAgentList.TryGetValue(num, out intPtr))
						{
							Bindings.AIAgentFunctions.UpdateEntity(gameEntity, (Bindings.LuauAIAgent*)((void*)intPtr));
							Luau.lua_class_push(L, "AIAgent", intPtr);
						}
						else
						{
							Bindings.LuauAIAgent* ptr = Luau.lua_class_push<Bindings.LuauAIAgent>(L);
							Bindings.AIAgentFunctions.UpdateEntity(gameEntity, ptr);
							Bindings.LuauAIAgentList[num] = (IntPtr)((void*)ptr);
						}
					}
					return 1;
				}
			}
			return 0;
		}

		// Token: 0x06004C64 RID: 19556 RVA: 0x0019A2C8 File Offset: 0x001984C8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FindPrePlacedAIAgentByID(lua_State* L)
		{
			short num = (short)Luau.luaL_checknumber(L, 1);
			GameAgentManager gameAgentManager = CustomMapsGameManager.instance.gameAgentManager;
			if (gameAgentManager.IsNotNull())
			{
				List<GameAgent> agents = gameAgentManager.GetAgents();
				for (int i = 0; i < agents.Count; i++)
				{
					if (!agents[i].gameObject.IsNull())
					{
						CustomMapsAIBehaviourController component = agents[i].gameObject.GetComponent<CustomMapsAIBehaviourController>();
						if (!component.IsNull() && component.luaAgentID == num)
						{
							IntPtr intPtr;
							if (Bindings.LuauAIAgentList.TryGetValue(agents[i].entity.GetNetId(), out intPtr))
							{
								Bindings.AIAgentFunctions.UpdateEntity(agents[i].entity, (Bindings.LuauAIAgent*)((void*)intPtr));
								Luau.lua_class_push(L, "AIAgent", intPtr);
							}
							else
							{
								Bindings.LuauAIAgent* ptr = Luau.lua_class_push<Bindings.LuauAIAgent>(L);
								Bindings.AIAgentFunctions.UpdateEntity(agents[i].entity, ptr);
								Bindings.LuauAIAgentList[agents[i].entity.GetNetId()] = (IntPtr)((void*)ptr);
							}
							return 1;
						}
					}
				}
			}
			return 0;
		}

		// Token: 0x06004C65 RID: 19557 RVA: 0x0019A3E0 File Offset: 0x001985E0
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SpawnAIAgent(lua_State* L)
		{
			CustomMapsGameManager instance = CustomMapsGameManager.instance;
			GameEntityManager gameEntityManager = instance.IsNotNull() ? instance.gameEntityManager : null;
			if (gameEntityManager.IsNull())
			{
				LuauHud.Instance.LuauLog("SpawnAIAgent failed. EntityManager is null.");
				return 0;
			}
			if (!gameEntityManager.IsZoneAuthority())
			{
				LuauHud.Instance.LuauLog("SpawnAIAgent failed. Local Player doesn't have Entity Authority.");
				return 0;
			}
			if (Bindings.LuauAIAgentList.Count + Bindings.LuauGrabbablesList.Count == Constants.aiAgentLimit)
			{
				LuauHud.Instance.LuauLog(string.Format("SpawnAIAgent failed, AIAgentLimit of {0}", Constants.aiAgentLimit) + " has already been reached.");
				return 0;
			}
			int enemyTypeId = (int)Luau.luaL_checknumber(L, 1);
			Vector3 position = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			Quaternion rotation = *Luau.lua_class_get<Quaternion>(L, 3, "Quat");
			GameEntityId id = instance.SpawnEnemyAtLocation(enemyTypeId, position, rotation);
			if (id.IsValid())
			{
				GameEntity gameEntity = gameEntityManager.GetGameEntity(id);
				if ((gameEntity.IsNotNull() ? gameEntity.gameObject.GetComponent<GameAgent>() : null).IsNotNull())
				{
					IntPtr intPtr;
					if (Bindings.LuauAIAgentList.TryGetValue(gameEntity.GetNetId(), out intPtr))
					{
						Bindings.AIAgentFunctions.UpdateEntity(gameEntity, (Bindings.LuauAIAgent*)((void*)intPtr));
						Luau.lua_class_push(L, "AIAgent", intPtr);
						return 1;
					}
					Luau.lua_getglobal(L, "AIAgents");
					Bindings.LuauAIAgent* ptr = Luau.lua_class_push<Bindings.LuauAIAgent>(L);
					Bindings.AIAgentFunctions.UpdateEntity(gameEntity, ptr);
					Bindings.LuauAIAgentList[gameEntity.GetNetId()] = (IntPtr)((void*)ptr);
					Luau.lua_rawseti(L, -2, Bindings.LuauAIAgentList.Count);
					Luau.lua_pop(L, 1);
					Luau.lua_class_push(L, "AIAgent", (IntPtr)((void*)ptr));
					return 1;
				}
			}
			LuauHud.Instance.LuauLog("SpawnAIAgent failed to create entity.");
			return 0;
		}

		// Token: 0x06004C66 RID: 19558 RVA: 0x0019A5A8 File Offset: 0x001987A8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetDestination(lua_State* L)
		{
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			Vector3* ptr2 = Luau.lua_class_get<Vector3>(L, 2);
			GameEntityManager gameEntityManager = CustomMapsGameManager.instance.gameEntityManager;
			if (gameEntityManager.IsNotNull())
			{
				CustomMapsAIBehaviourController component = gameEntityManager.GetGameEntity(gameEntityManager.GetEntityIdFromNetId(ptr->EntityID)).gameObject.GetComponent<CustomMapsAIBehaviourController>();
				if (component.IsNotNull())
				{
					component.RequestDestination(*ptr2);
				}
			}
			return 0;
		}

		// Token: 0x06004C67 RID: 19559 RVA: 0x0019A60C File Offset: 0x0019880C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int PlayAgentAnimation(lua_State* L)
		{
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			string stateName = Marshal.PtrToStringAnsi((IntPtr)((void*)Luau.luaL_checkstring(L, 2)));
			if (ptr != null)
			{
				GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
				if (entityManager.IsNotNull())
				{
					CustomMapsAIBehaviourController behaviorControllerForEntity = CustomMapsGameManager.GetBehaviorControllerForEntity(entityManager.GetEntityIdFromNetId(ptr->EntityID));
					if (behaviorControllerForEntity.IsNotNull())
					{
						behaviorControllerForEntity.PlayAnimation(stateName, 0f);
					}
				}
			}
			return 0;
		}

		// Token: 0x06004C68 RID: 19560 RVA: 0x0019A670 File Offset: 0x00198870
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetTarget(lua_State* L)
		{
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			if (ptr == null)
			{
				return 0;
			}
			GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
			if (entityManager.IsNull() || !entityManager.IsAuthority())
			{
				return 0;
			}
			int num = (int)Luau.luaL_checknumber(L, 2);
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(num, out rigContainer))
			{
				num = -1;
			}
			CustomMapsAIBehaviourController behaviorControllerForEntity = CustomMapsGameManager.GetBehaviorControllerForEntity(entityManager.GetEntityIdFromNetId(ptr->EntityID));
			if (behaviorControllerForEntity.IsNull())
			{
				return 0;
			}
			if (num == -1)
			{
				behaviorControllerForEntity.ClearTarget();
			}
			else
			{
				GRPlayer component = rigContainer.Rig.GetComponent<GRPlayer>();
				behaviorControllerForEntity.SetTarget(component);
			}
			return 0;
		}

		// Token: 0x06004C69 RID: 19561 RVA: 0x0019A700 File Offset: 0x00198900
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetTarget(lua_State* L)
		{
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			if (ptr != null)
			{
				GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
				if (entityManager.IsNotNull() && entityManager.IsAuthority())
				{
					CustomMapsAIBehaviourController behaviorControllerForEntity = CustomMapsGameManager.GetBehaviorControllerForEntity(entityManager.GetEntityIdFromNetId(ptr->EntityID));
					if (behaviorControllerForEntity.IsNotNull() && behaviorControllerForEntity.TargetPlayer.IsNotNull() && behaviorControllerForEntity.TargetPlayer.MyRig.IsNotNull() && !behaviorControllerForEntity.TargetPlayer.MyRig.OwningNetPlayer.IsNull)
					{
						Luau.lua_pushnumber(L, (double)behaviorControllerForEntity.TargetPlayer.MyRig.OwningNetPlayer.ActorNumber);
						return 1;
					}
				}
			}
			Luau.lua_pushnumber(L, -1.0);
			return 1;
		}

		// Token: 0x06004C6A RID: 19562 RVA: 0x0019A7B1 File Offset: 0x001989B1
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static void UpdateEntity(GameEntity entity, Bindings.LuauAIAgent* luaAgent)
		{
			luaAgent->EntityID = entity.GetNetId();
			luaAgent->EntityPosition = entity.transform.position;
			luaAgent->EntityRotation = entity.transform.rotation;
		}

		// Token: 0x06004C6B RID: 19563 RVA: 0x0019A7E4 File Offset: 0x001989E4
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int DestroyEntity(lua_State* L)
		{
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			if (ptr != null)
			{
				GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
				if (entityManager.IsNotNull())
				{
					GameEntityId entityIdFromNetId = entityManager.GetEntityIdFromNetId(ptr->EntityID);
					entityManager.RequestDestroyItem(entityIdFromNetId);
				}
			}
			return 0;
		}
	}

	// Token: 0x02000BF7 RID: 3063
	[BurstCompile]
	public static class Vec3Functions
	{
		// Token: 0x06004C6C RID: 19564 RVA: 0x0019A821 File Offset: 0x00198A21
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int New(lua_State* L)
		{
			return Bindings.Vec3Functions.New_00004C6C$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004C6D RID: 19565 RVA: 0x0019A829 File Offset: 0x00198A29
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Add(lua_State* L)
		{
			return Bindings.Vec3Functions.Add_00004C6D$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004C6E RID: 19566 RVA: 0x0019A831 File Offset: 0x00198A31
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Sub(lua_State* L)
		{
			return Bindings.Vec3Functions.Sub_00004C6E$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004C6F RID: 19567 RVA: 0x0019A839 File Offset: 0x00198A39
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Mul(lua_State* L)
		{
			return Bindings.Vec3Functions.Mul_00004C6F$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004C70 RID: 19568 RVA: 0x0019A841 File Offset: 0x00198A41
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Div(lua_State* L)
		{
			return Bindings.Vec3Functions.Div_00004C70$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004C71 RID: 19569 RVA: 0x0019A849 File Offset: 0x00198A49
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Unm(lua_State* L)
		{
			return Bindings.Vec3Functions.Unm_00004C71$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004C72 RID: 19570 RVA: 0x0019A851 File Offset: 0x00198A51
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Eq(lua_State* L)
		{
			return Bindings.Vec3Functions.Eq_00004C72$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004C73 RID: 19571 RVA: 0x0019A85C File Offset: 0x00198A5C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ToString(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Luau.lua_pushstring(L, vector.ToString());
			return 1;
		}

		// Token: 0x06004C74 RID: 19572 RVA: 0x0019A895 File Offset: 0x00198A95
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Dot(lua_State* L)
		{
			return Bindings.Vec3Functions.Dot_00004C74$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004C75 RID: 19573 RVA: 0x0019A89D File Offset: 0x00198A9D
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Cross(lua_State* L)
		{
			return Bindings.Vec3Functions.Cross_00004C75$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004C76 RID: 19574 RVA: 0x0019A8A5 File Offset: 0x00198AA5
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Project(lua_State* L)
		{
			return Bindings.Vec3Functions.Project_00004C76$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004C77 RID: 19575 RVA: 0x0019A8AD File Offset: 0x00198AAD
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Length(lua_State* L)
		{
			return Bindings.Vec3Functions.Length_00004C77$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004C78 RID: 19576 RVA: 0x0019A8B5 File Offset: 0x00198AB5
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Normalize(lua_State* L)
		{
			return Bindings.Vec3Functions.Normalize_00004C78$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004C79 RID: 19577 RVA: 0x0019A8BD File Offset: 0x00198ABD
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SafeNormal(lua_State* L)
		{
			return Bindings.Vec3Functions.SafeNormal_00004C79$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004C7A RID: 19578 RVA: 0x0019A8C5 File Offset: 0x00198AC5
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Distance(lua_State* L)
		{
			return Bindings.Vec3Functions.Distance_00004C7A$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004C7B RID: 19579 RVA: 0x0019A8CD File Offset: 0x00198ACD
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Lerp(lua_State* L)
		{
			return Bindings.Vec3Functions.Lerp_00004C7B$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004C7C RID: 19580 RVA: 0x0019A8D5 File Offset: 0x00198AD5
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Rotate(lua_State* L)
		{
			return Bindings.Vec3Functions.Rotate_00004C7C$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004C7D RID: 19581 RVA: 0x0019A8DD File Offset: 0x00198ADD
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ZeroVector(lua_State* L)
		{
			return Bindings.Vec3Functions.ZeroVector_00004C7D$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004C7E RID: 19582 RVA: 0x0019A8E5 File Offset: 0x00198AE5
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int OneVector(lua_State* L)
		{
			return Bindings.Vec3Functions.OneVector_00004C7E$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004C7F RID: 19583 RVA: 0x0019A8ED File Offset: 0x00198AED
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int NearlyEqual(lua_State* L)
		{
			return Bindings.Vec3Functions.NearlyEqual_00004C7F$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004C80 RID: 19584 RVA: 0x0019A8F8 File Offset: 0x00198AF8
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int New$BurstManaged(lua_State* L)
		{
			Vector3* ptr = Luau.lua_class_push<Vector3>(L, "Vec3");
			ptr->x = (float)Luau.luaL_optnumber(L, 1, 0.0);
			ptr->y = (float)Luau.luaL_optnumber(L, 2, 0.0);
			ptr->z = (float)Luau.luaL_optnumber(L, 3, 0.0);
			return 1;
		}

		// Token: 0x06004C81 RID: 19585 RVA: 0x0019A95C File Offset: 0x00198B5C
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Add$BurstManaged(lua_State* L)
		{
			Vector3 a = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 b = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = a + b;
			return 1;
		}

		// Token: 0x06004C82 RID: 19586 RVA: 0x0019A9B4 File Offset: 0x00198BB4
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Sub$BurstManaged(lua_State* L)
		{
			Vector3 a = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 b = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = a - b;
			return 1;
		}

		// Token: 0x06004C83 RID: 19587 RVA: 0x0019AA0C File Offset: 0x00198C0C
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Mul$BurstManaged(lua_State* L)
		{
			Vector3 a = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			float d = (float)Luau.luaL_checknumber(L, 2);
			*Luau.lua_class_push<Vector3>(L, "Vec3") = a * d;
			return 1;
		}

		// Token: 0x06004C84 RID: 19588 RVA: 0x0019AA58 File Offset: 0x00198C58
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Div$BurstManaged(lua_State* L)
		{
			Vector3 a = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			float d = (float)Luau.luaL_checknumber(L, 2);
			*Luau.lua_class_push<Vector3>(L, "Vec3") = a / d;
			return 1;
		}

		// Token: 0x06004C85 RID: 19589 RVA: 0x0019AAA4 File Offset: 0x00198CA4
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Unm$BurstManaged(lua_State* L)
		{
			Vector3 a = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = -a;
			return 1;
		}

		// Token: 0x06004C86 RID: 19590 RVA: 0x0019AAE4 File Offset: 0x00198CE4
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Eq$BurstManaged(lua_State* L)
		{
			Vector3 lhs = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 rhs = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			int num = (lhs == rhs) ? 1 : 0;
			Luau.lua_pushnumber(L, (double)num);
			return 1;
		}

		// Token: 0x06004C87 RID: 19591 RVA: 0x0019AB34 File Offset: 0x00198D34
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Dot$BurstManaged(lua_State* L)
		{
			Vector3 lhs = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 rhs = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			double n = (double)Vector3.Dot(lhs, rhs);
			Luau.lua_pushnumber(L, n);
			return 1;
		}

		// Token: 0x06004C88 RID: 19592 RVA: 0x0019AB80 File Offset: 0x00198D80
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Cross$BurstManaged(lua_State* L)
		{
			Vector3 lhs = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 rhs = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Vector3.Cross(lhs, rhs);
			return 1;
		}

		// Token: 0x06004C89 RID: 19593 RVA: 0x0019ABD8 File Offset: 0x00198DD8
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Project$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 onNormal = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Vector3.Project(vector, onNormal);
			return 1;
		}

		// Token: 0x06004C8A RID: 19594 RVA: 0x0019AC30 File Offset: 0x00198E30
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Length$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Luau.lua_pushnumber(L, (double)Vector3.Magnitude(vector));
			return 1;
		}

		// Token: 0x06004C8B RID: 19595 RVA: 0x0019AC62 File Offset: 0x00198E62
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Normalize$BurstManaged(lua_State* L)
		{
			Luau.lua_class_get<Vector3>(L, 1, "Vec3")->Normalize();
			return 0;
		}

		// Token: 0x06004C8C RID: 19596 RVA: 0x0019AC7C File Offset: 0x00198E7C
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int SafeNormal$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = vector.normalized;
			return 1;
		}

		// Token: 0x06004C8D RID: 19597 RVA: 0x0019ACC0 File Offset: 0x00198EC0
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Distance$BurstManaged(lua_State* L)
		{
			Vector3 a = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 b = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			Luau.lua_pushnumber(L, (double)Vector3.Distance(a, b));
			return 1;
		}

		// Token: 0x06004C8E RID: 19598 RVA: 0x0019AD0C File Offset: 0x00198F0C
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Lerp$BurstManaged(lua_State* L)
		{
			Vector3 a = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 b = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			double num = Luau.luaL_checknumber(L, 3);
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Vector3.Lerp(a, b, (float)num);
			return 1;
		}

		// Token: 0x06004C8F RID: 19599 RVA: 0x0019AD70 File Offset: 0x00198F70
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Rotate$BurstManaged(lua_State* L)
		{
			Vector3 point = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Quaternion rotation = *Luau.lua_class_get<Quaternion>(L, 2, "Quat");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = rotation * point;
			return 1;
		}

		// Token: 0x06004C90 RID: 19600 RVA: 0x0019ADC8 File Offset: 0x00198FC8
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int ZeroVector$BurstManaged(lua_State* L)
		{
			Vector3* ptr = Luau.lua_class_push<Vector3>(L, "Vec3");
			ptr->x = 0f;
			ptr->y = 0f;
			ptr->z = 0f;
			return 1;
		}

		// Token: 0x06004C91 RID: 19601 RVA: 0x0019ADFB File Offset: 0x00198FFB
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int OneVector$BurstManaged(lua_State* L)
		{
			Vector3* ptr = Luau.lua_class_push<Vector3>(L, "Vec3");
			ptr->x = 1f;
			ptr->y = 1f;
			ptr->z = 1f;
			return 1;
		}

		// Token: 0x06004C92 RID: 19602 RVA: 0x0019AE30 File Offset: 0x00199030
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int NearlyEqual$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			float num = (float)Luau.luaL_optnumber(L, 3, 0.0001);
			bool flag = Math.Abs(vector.x - vector2.x) <= num;
			if (flag && Math.Abs(vector.y - vector2.y) > num)
			{
				flag = false;
			}
			if (flag && Math.Abs(vector.z - vector2.z) > num)
			{
				flag = false;
			}
			Luau.lua_pushboolean(L, flag ? 1 : 0);
			return 1;
		}

		// Token: 0x02000BF8 RID: 3064
		// (Invoke) Token: 0x06004C94 RID: 19604
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int New_00004C6C$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000BF9 RID: 3065
		internal static class New_00004C6C$BurstDirectCall
		{
			// Token: 0x06004C97 RID: 19607 RVA: 0x0019AED8 File Offset: 0x001990D8
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.New_00004C6C$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.New_00004C6C$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.New_00004C6C$PostfixBurstDelegate>(new Bindings.Vec3Functions.New_00004C6C$PostfixBurstDelegate(Bindings.Vec3Functions.New)).Value;
				}
				A_0 = Bindings.Vec3Functions.New_00004C6C$BurstDirectCall.Pointer;
			}

			// Token: 0x06004C98 RID: 19608 RVA: 0x0019AF18 File Offset: 0x00199118
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.New_00004C6C$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004C99 RID: 19609 RVA: 0x0019AF30 File Offset: 0x00199130
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.New_00004C6C$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.New$BurstManaged(L);
			}

			// Token: 0x04005F89 RID: 24457
			private static IntPtr Pointer;
		}

		// Token: 0x02000BFA RID: 3066
		// (Invoke) Token: 0x06004C9B RID: 19611
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Add_00004C6D$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000BFB RID: 3067
		internal static class Add_00004C6D$BurstDirectCall
		{
			// Token: 0x06004C9E RID: 19614 RVA: 0x0019AF64 File Offset: 0x00199164
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Add_00004C6D$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Add_00004C6D$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Add_00004C6D$PostfixBurstDelegate>(new Bindings.Vec3Functions.Add_00004C6D$PostfixBurstDelegate(Bindings.Vec3Functions.Add)).Value;
				}
				A_0 = Bindings.Vec3Functions.Add_00004C6D$BurstDirectCall.Pointer;
			}

			// Token: 0x06004C9F RID: 19615 RVA: 0x0019AFA4 File Offset: 0x001991A4
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Add_00004C6D$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004CA0 RID: 19616 RVA: 0x0019AFBC File Offset: 0x001991BC
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Add_00004C6D$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Add$BurstManaged(L);
			}

			// Token: 0x04005F8A RID: 24458
			private static IntPtr Pointer;
		}

		// Token: 0x02000BFC RID: 3068
		// (Invoke) Token: 0x06004CA2 RID: 19618
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Sub_00004C6E$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000BFD RID: 3069
		internal static class Sub_00004C6E$BurstDirectCall
		{
			// Token: 0x06004CA5 RID: 19621 RVA: 0x0019AFF0 File Offset: 0x001991F0
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Sub_00004C6E$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Sub_00004C6E$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Sub_00004C6E$PostfixBurstDelegate>(new Bindings.Vec3Functions.Sub_00004C6E$PostfixBurstDelegate(Bindings.Vec3Functions.Sub)).Value;
				}
				A_0 = Bindings.Vec3Functions.Sub_00004C6E$BurstDirectCall.Pointer;
			}

			// Token: 0x06004CA6 RID: 19622 RVA: 0x0019B030 File Offset: 0x00199230
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Sub_00004C6E$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004CA7 RID: 19623 RVA: 0x0019B048 File Offset: 0x00199248
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Sub_00004C6E$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Sub$BurstManaged(L);
			}

			// Token: 0x04005F8B RID: 24459
			private static IntPtr Pointer;
		}

		// Token: 0x02000BFE RID: 3070
		// (Invoke) Token: 0x06004CA9 RID: 19625
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Mul_00004C6F$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000BFF RID: 3071
		internal static class Mul_00004C6F$BurstDirectCall
		{
			// Token: 0x06004CAC RID: 19628 RVA: 0x0019B07C File Offset: 0x0019927C
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Mul_00004C6F$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Mul_00004C6F$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Mul_00004C6F$PostfixBurstDelegate>(new Bindings.Vec3Functions.Mul_00004C6F$PostfixBurstDelegate(Bindings.Vec3Functions.Mul)).Value;
				}
				A_0 = Bindings.Vec3Functions.Mul_00004C6F$BurstDirectCall.Pointer;
			}

			// Token: 0x06004CAD RID: 19629 RVA: 0x0019B0BC File Offset: 0x001992BC
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Mul_00004C6F$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004CAE RID: 19630 RVA: 0x0019B0D4 File Offset: 0x001992D4
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Mul_00004C6F$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Mul$BurstManaged(L);
			}

			// Token: 0x04005F8C RID: 24460
			private static IntPtr Pointer;
		}

		// Token: 0x02000C00 RID: 3072
		// (Invoke) Token: 0x06004CB0 RID: 19632
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Div_00004C70$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C01 RID: 3073
		internal static class Div_00004C70$BurstDirectCall
		{
			// Token: 0x06004CB3 RID: 19635 RVA: 0x0019B108 File Offset: 0x00199308
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Div_00004C70$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Div_00004C70$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Div_00004C70$PostfixBurstDelegate>(new Bindings.Vec3Functions.Div_00004C70$PostfixBurstDelegate(Bindings.Vec3Functions.Div)).Value;
				}
				A_0 = Bindings.Vec3Functions.Div_00004C70$BurstDirectCall.Pointer;
			}

			// Token: 0x06004CB4 RID: 19636 RVA: 0x0019B148 File Offset: 0x00199348
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Div_00004C70$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004CB5 RID: 19637 RVA: 0x0019B160 File Offset: 0x00199360
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Div_00004C70$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Div$BurstManaged(L);
			}

			// Token: 0x04005F8D RID: 24461
			private static IntPtr Pointer;
		}

		// Token: 0x02000C02 RID: 3074
		// (Invoke) Token: 0x06004CB7 RID: 19639
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Unm_00004C71$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C03 RID: 3075
		internal static class Unm_00004C71$BurstDirectCall
		{
			// Token: 0x06004CBA RID: 19642 RVA: 0x0019B194 File Offset: 0x00199394
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Unm_00004C71$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Unm_00004C71$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Unm_00004C71$PostfixBurstDelegate>(new Bindings.Vec3Functions.Unm_00004C71$PostfixBurstDelegate(Bindings.Vec3Functions.Unm)).Value;
				}
				A_0 = Bindings.Vec3Functions.Unm_00004C71$BurstDirectCall.Pointer;
			}

			// Token: 0x06004CBB RID: 19643 RVA: 0x0019B1D4 File Offset: 0x001993D4
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Unm_00004C71$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004CBC RID: 19644 RVA: 0x0019B1EC File Offset: 0x001993EC
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Unm_00004C71$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Unm$BurstManaged(L);
			}

			// Token: 0x04005F8E RID: 24462
			private static IntPtr Pointer;
		}

		// Token: 0x02000C04 RID: 3076
		// (Invoke) Token: 0x06004CBE RID: 19646
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Eq_00004C72$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C05 RID: 3077
		internal static class Eq_00004C72$BurstDirectCall
		{
			// Token: 0x06004CC1 RID: 19649 RVA: 0x0019B220 File Offset: 0x00199420
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Eq_00004C72$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Eq_00004C72$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Eq_00004C72$PostfixBurstDelegate>(new Bindings.Vec3Functions.Eq_00004C72$PostfixBurstDelegate(Bindings.Vec3Functions.Eq)).Value;
				}
				A_0 = Bindings.Vec3Functions.Eq_00004C72$BurstDirectCall.Pointer;
			}

			// Token: 0x06004CC2 RID: 19650 RVA: 0x0019B260 File Offset: 0x00199460
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Eq_00004C72$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004CC3 RID: 19651 RVA: 0x0019B278 File Offset: 0x00199478
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Eq_00004C72$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Eq$BurstManaged(L);
			}

			// Token: 0x04005F8F RID: 24463
			private static IntPtr Pointer;
		}

		// Token: 0x02000C06 RID: 3078
		// (Invoke) Token: 0x06004CC5 RID: 19653
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Dot_00004C74$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C07 RID: 3079
		internal static class Dot_00004C74$BurstDirectCall
		{
			// Token: 0x06004CC8 RID: 19656 RVA: 0x0019B2AC File Offset: 0x001994AC
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Dot_00004C74$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Dot_00004C74$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Dot_00004C74$PostfixBurstDelegate>(new Bindings.Vec3Functions.Dot_00004C74$PostfixBurstDelegate(Bindings.Vec3Functions.Dot)).Value;
				}
				A_0 = Bindings.Vec3Functions.Dot_00004C74$BurstDirectCall.Pointer;
			}

			// Token: 0x06004CC9 RID: 19657 RVA: 0x0019B2EC File Offset: 0x001994EC
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Dot_00004C74$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004CCA RID: 19658 RVA: 0x0019B304 File Offset: 0x00199504
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Dot_00004C74$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Dot$BurstManaged(L);
			}

			// Token: 0x04005F90 RID: 24464
			private static IntPtr Pointer;
		}

		// Token: 0x02000C08 RID: 3080
		// (Invoke) Token: 0x06004CCC RID: 19660
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Cross_00004C75$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C09 RID: 3081
		internal static class Cross_00004C75$BurstDirectCall
		{
			// Token: 0x06004CCF RID: 19663 RVA: 0x0019B338 File Offset: 0x00199538
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Cross_00004C75$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Cross_00004C75$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Cross_00004C75$PostfixBurstDelegate>(new Bindings.Vec3Functions.Cross_00004C75$PostfixBurstDelegate(Bindings.Vec3Functions.Cross)).Value;
				}
				A_0 = Bindings.Vec3Functions.Cross_00004C75$BurstDirectCall.Pointer;
			}

			// Token: 0x06004CD0 RID: 19664 RVA: 0x0019B378 File Offset: 0x00199578
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Cross_00004C75$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004CD1 RID: 19665 RVA: 0x0019B390 File Offset: 0x00199590
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Cross_00004C75$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Cross$BurstManaged(L);
			}

			// Token: 0x04005F91 RID: 24465
			private static IntPtr Pointer;
		}

		// Token: 0x02000C0A RID: 3082
		// (Invoke) Token: 0x06004CD3 RID: 19667
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Project_00004C76$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C0B RID: 3083
		internal static class Project_00004C76$BurstDirectCall
		{
			// Token: 0x06004CD6 RID: 19670 RVA: 0x0019B3C4 File Offset: 0x001995C4
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Project_00004C76$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Project_00004C76$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Project_00004C76$PostfixBurstDelegate>(new Bindings.Vec3Functions.Project_00004C76$PostfixBurstDelegate(Bindings.Vec3Functions.Project)).Value;
				}
				A_0 = Bindings.Vec3Functions.Project_00004C76$BurstDirectCall.Pointer;
			}

			// Token: 0x06004CD7 RID: 19671 RVA: 0x0019B404 File Offset: 0x00199604
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Project_00004C76$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004CD8 RID: 19672 RVA: 0x0019B41C File Offset: 0x0019961C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Project_00004C76$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Project$BurstManaged(L);
			}

			// Token: 0x04005F92 RID: 24466
			private static IntPtr Pointer;
		}

		// Token: 0x02000C0C RID: 3084
		// (Invoke) Token: 0x06004CDA RID: 19674
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Length_00004C77$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C0D RID: 3085
		internal static class Length_00004C77$BurstDirectCall
		{
			// Token: 0x06004CDD RID: 19677 RVA: 0x0019B450 File Offset: 0x00199650
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Length_00004C77$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Length_00004C77$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Length_00004C77$PostfixBurstDelegate>(new Bindings.Vec3Functions.Length_00004C77$PostfixBurstDelegate(Bindings.Vec3Functions.Length)).Value;
				}
				A_0 = Bindings.Vec3Functions.Length_00004C77$BurstDirectCall.Pointer;
			}

			// Token: 0x06004CDE RID: 19678 RVA: 0x0019B490 File Offset: 0x00199690
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Length_00004C77$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004CDF RID: 19679 RVA: 0x0019B4A8 File Offset: 0x001996A8
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Length_00004C77$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Length$BurstManaged(L);
			}

			// Token: 0x04005F93 RID: 24467
			private static IntPtr Pointer;
		}

		// Token: 0x02000C0E RID: 3086
		// (Invoke) Token: 0x06004CE1 RID: 19681
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Normalize_00004C78$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C0F RID: 3087
		internal static class Normalize_00004C78$BurstDirectCall
		{
			// Token: 0x06004CE4 RID: 19684 RVA: 0x0019B4DC File Offset: 0x001996DC
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Normalize_00004C78$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Normalize_00004C78$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Normalize_00004C78$PostfixBurstDelegate>(new Bindings.Vec3Functions.Normalize_00004C78$PostfixBurstDelegate(Bindings.Vec3Functions.Normalize)).Value;
				}
				A_0 = Bindings.Vec3Functions.Normalize_00004C78$BurstDirectCall.Pointer;
			}

			// Token: 0x06004CE5 RID: 19685 RVA: 0x0019B51C File Offset: 0x0019971C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Normalize_00004C78$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004CE6 RID: 19686 RVA: 0x0019B534 File Offset: 0x00199734
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Normalize_00004C78$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Normalize$BurstManaged(L);
			}

			// Token: 0x04005F94 RID: 24468
			private static IntPtr Pointer;
		}

		// Token: 0x02000C10 RID: 3088
		// (Invoke) Token: 0x06004CE8 RID: 19688
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int SafeNormal_00004C79$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C11 RID: 3089
		internal static class SafeNormal_00004C79$BurstDirectCall
		{
			// Token: 0x06004CEB RID: 19691 RVA: 0x0019B568 File Offset: 0x00199768
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.SafeNormal_00004C79$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.SafeNormal_00004C79$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.SafeNormal_00004C79$PostfixBurstDelegate>(new Bindings.Vec3Functions.SafeNormal_00004C79$PostfixBurstDelegate(Bindings.Vec3Functions.SafeNormal)).Value;
				}
				A_0 = Bindings.Vec3Functions.SafeNormal_00004C79$BurstDirectCall.Pointer;
			}

			// Token: 0x06004CEC RID: 19692 RVA: 0x0019B5A8 File Offset: 0x001997A8
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.SafeNormal_00004C79$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004CED RID: 19693 RVA: 0x0019B5C0 File Offset: 0x001997C0
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.SafeNormal_00004C79$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.SafeNormal$BurstManaged(L);
			}

			// Token: 0x04005F95 RID: 24469
			private static IntPtr Pointer;
		}

		// Token: 0x02000C12 RID: 3090
		// (Invoke) Token: 0x06004CEF RID: 19695
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Distance_00004C7A$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C13 RID: 3091
		internal static class Distance_00004C7A$BurstDirectCall
		{
			// Token: 0x06004CF2 RID: 19698 RVA: 0x0019B5F4 File Offset: 0x001997F4
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Distance_00004C7A$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Distance_00004C7A$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Distance_00004C7A$PostfixBurstDelegate>(new Bindings.Vec3Functions.Distance_00004C7A$PostfixBurstDelegate(Bindings.Vec3Functions.Distance)).Value;
				}
				A_0 = Bindings.Vec3Functions.Distance_00004C7A$BurstDirectCall.Pointer;
			}

			// Token: 0x06004CF3 RID: 19699 RVA: 0x0019B634 File Offset: 0x00199834
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Distance_00004C7A$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004CF4 RID: 19700 RVA: 0x0019B64C File Offset: 0x0019984C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Distance_00004C7A$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Distance$BurstManaged(L);
			}

			// Token: 0x04005F96 RID: 24470
			private static IntPtr Pointer;
		}

		// Token: 0x02000C14 RID: 3092
		// (Invoke) Token: 0x06004CF6 RID: 19702
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Lerp_00004C7B$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C15 RID: 3093
		internal static class Lerp_00004C7B$BurstDirectCall
		{
			// Token: 0x06004CF9 RID: 19705 RVA: 0x0019B680 File Offset: 0x00199880
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Lerp_00004C7B$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Lerp_00004C7B$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Lerp_00004C7B$PostfixBurstDelegate>(new Bindings.Vec3Functions.Lerp_00004C7B$PostfixBurstDelegate(Bindings.Vec3Functions.Lerp)).Value;
				}
				A_0 = Bindings.Vec3Functions.Lerp_00004C7B$BurstDirectCall.Pointer;
			}

			// Token: 0x06004CFA RID: 19706 RVA: 0x0019B6C0 File Offset: 0x001998C0
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Lerp_00004C7B$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004CFB RID: 19707 RVA: 0x0019B6D8 File Offset: 0x001998D8
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Lerp_00004C7B$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Lerp$BurstManaged(L);
			}

			// Token: 0x04005F97 RID: 24471
			private static IntPtr Pointer;
		}

		// Token: 0x02000C16 RID: 3094
		// (Invoke) Token: 0x06004CFD RID: 19709
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Rotate_00004C7C$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C17 RID: 3095
		internal static class Rotate_00004C7C$BurstDirectCall
		{
			// Token: 0x06004D00 RID: 19712 RVA: 0x0019B70C File Offset: 0x0019990C
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Rotate_00004C7C$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Rotate_00004C7C$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Rotate_00004C7C$PostfixBurstDelegate>(new Bindings.Vec3Functions.Rotate_00004C7C$PostfixBurstDelegate(Bindings.Vec3Functions.Rotate)).Value;
				}
				A_0 = Bindings.Vec3Functions.Rotate_00004C7C$BurstDirectCall.Pointer;
			}

			// Token: 0x06004D01 RID: 19713 RVA: 0x0019B74C File Offset: 0x0019994C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Rotate_00004C7C$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004D02 RID: 19714 RVA: 0x0019B764 File Offset: 0x00199964
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Rotate_00004C7C$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Rotate$BurstManaged(L);
			}

			// Token: 0x04005F98 RID: 24472
			private static IntPtr Pointer;
		}

		// Token: 0x02000C18 RID: 3096
		// (Invoke) Token: 0x06004D04 RID: 19716
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int ZeroVector_00004C7D$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C19 RID: 3097
		internal static class ZeroVector_00004C7D$BurstDirectCall
		{
			// Token: 0x06004D07 RID: 19719 RVA: 0x0019B798 File Offset: 0x00199998
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.ZeroVector_00004C7D$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.ZeroVector_00004C7D$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.ZeroVector_00004C7D$PostfixBurstDelegate>(new Bindings.Vec3Functions.ZeroVector_00004C7D$PostfixBurstDelegate(Bindings.Vec3Functions.ZeroVector)).Value;
				}
				A_0 = Bindings.Vec3Functions.ZeroVector_00004C7D$BurstDirectCall.Pointer;
			}

			// Token: 0x06004D08 RID: 19720 RVA: 0x0019B7D8 File Offset: 0x001999D8
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.ZeroVector_00004C7D$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004D09 RID: 19721 RVA: 0x0019B7F0 File Offset: 0x001999F0
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.ZeroVector_00004C7D$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.ZeroVector$BurstManaged(L);
			}

			// Token: 0x04005F99 RID: 24473
			private static IntPtr Pointer;
		}

		// Token: 0x02000C1A RID: 3098
		// (Invoke) Token: 0x06004D0B RID: 19723
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int OneVector_00004C7E$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C1B RID: 3099
		internal static class OneVector_00004C7E$BurstDirectCall
		{
			// Token: 0x06004D0E RID: 19726 RVA: 0x0019B824 File Offset: 0x00199A24
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.OneVector_00004C7E$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.OneVector_00004C7E$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.OneVector_00004C7E$PostfixBurstDelegate>(new Bindings.Vec3Functions.OneVector_00004C7E$PostfixBurstDelegate(Bindings.Vec3Functions.OneVector)).Value;
				}
				A_0 = Bindings.Vec3Functions.OneVector_00004C7E$BurstDirectCall.Pointer;
			}

			// Token: 0x06004D0F RID: 19727 RVA: 0x0019B864 File Offset: 0x00199A64
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.OneVector_00004C7E$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004D10 RID: 19728 RVA: 0x0019B87C File Offset: 0x00199A7C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.OneVector_00004C7E$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.OneVector$BurstManaged(L);
			}

			// Token: 0x04005F9A RID: 24474
			private static IntPtr Pointer;
		}

		// Token: 0x02000C1C RID: 3100
		// (Invoke) Token: 0x06004D12 RID: 19730
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int NearlyEqual_00004C7F$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C1D RID: 3101
		internal static class NearlyEqual_00004C7F$BurstDirectCall
		{
			// Token: 0x06004D15 RID: 19733 RVA: 0x0019B8B0 File Offset: 0x00199AB0
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.NearlyEqual_00004C7F$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.NearlyEqual_00004C7F$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.NearlyEqual_00004C7F$PostfixBurstDelegate>(new Bindings.Vec3Functions.NearlyEqual_00004C7F$PostfixBurstDelegate(Bindings.Vec3Functions.NearlyEqual)).Value;
				}
				A_0 = Bindings.Vec3Functions.NearlyEqual_00004C7F$BurstDirectCall.Pointer;
			}

			// Token: 0x06004D16 RID: 19734 RVA: 0x0019B8F0 File Offset: 0x00199AF0
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.NearlyEqual_00004C7F$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004D17 RID: 19735 RVA: 0x0019B908 File Offset: 0x00199B08
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.NearlyEqual_00004C7F$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.NearlyEqual$BurstManaged(L);
			}

			// Token: 0x04005F9B RID: 24475
			private static IntPtr Pointer;
		}
	}

	// Token: 0x02000C1E RID: 3102
	[BurstCompile]
	public static class QuatFunctions
	{
		// Token: 0x06004D18 RID: 19736 RVA: 0x0019B939 File Offset: 0x00199B39
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int New(lua_State* L)
		{
			return Bindings.QuatFunctions.New_00004C80$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004D19 RID: 19737 RVA: 0x0019B941 File Offset: 0x00199B41
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Mul(lua_State* L)
		{
			return Bindings.QuatFunctions.Mul_00004C81$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004D1A RID: 19738 RVA: 0x0019B949 File Offset: 0x00199B49
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Eq(lua_State* L)
		{
			return Bindings.QuatFunctions.Eq_00004C82$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004D1B RID: 19739 RVA: 0x0019B954 File Offset: 0x00199B54
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ToString(lua_State* L)
		{
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			Luau.lua_pushstring(L, quaternion.ToString());
			return 1;
		}

		// Token: 0x06004D1C RID: 19740 RVA: 0x0019B98D File Offset: 0x00199B8D
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FromEuler(lua_State* L)
		{
			return Bindings.QuatFunctions.FromEuler_00004C84$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004D1D RID: 19741 RVA: 0x0019B995 File Offset: 0x00199B95
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FromDirection(lua_State* L)
		{
			return Bindings.QuatFunctions.FromDirection_00004C85$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004D1E RID: 19742 RVA: 0x0019B99D File Offset: 0x00199B9D
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetUpVector(lua_State* L)
		{
			return Bindings.QuatFunctions.GetUpVector_00004C86$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004D1F RID: 19743 RVA: 0x0019B9A5 File Offset: 0x00199BA5
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Euler(lua_State* L)
		{
			return Bindings.QuatFunctions.Euler_00004C87$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004D20 RID: 19744 RVA: 0x0019B9B0 File Offset: 0x00199BB0
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int New$BurstManaged(lua_State* L)
		{
			Quaternion* ptr = Luau.lua_class_push<Quaternion>(L, "Quat");
			ptr->x = (float)Luau.luaL_optnumber(L, 1, 0.0);
			ptr->y = (float)Luau.luaL_optnumber(L, 2, 0.0);
			ptr->z = (float)Luau.luaL_optnumber(L, 3, 0.0);
			ptr->w = (float)Luau.luaL_optnumber(L, 4, 0.0);
			return 1;
		}

		// Token: 0x06004D21 RID: 19745 RVA: 0x0019BA2C File Offset: 0x00199C2C
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Mul$BurstManaged(lua_State* L)
		{
			Quaternion lhs = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			Quaternion rhs = *Luau.lua_class_get<Quaternion>(L, 2, "Quat");
			*Luau.lua_class_push<Quaternion>(L, "Quat") = lhs * rhs;
			return 1;
		}

		// Token: 0x06004D22 RID: 19746 RVA: 0x0019BA84 File Offset: 0x00199C84
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Eq$BurstManaged(lua_State* L)
		{
			Quaternion lhs = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			Quaternion rhs = *Luau.lua_class_get<Quaternion>(L, 2, "Quat");
			int num = (lhs == rhs) ? 1 : 0;
			Luau.lua_pushnumber(L, (double)num);
			return 1;
		}

		// Token: 0x06004D23 RID: 19747 RVA: 0x0019BAD4 File Offset: 0x00199CD4
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int FromEuler$BurstManaged(lua_State* L)
		{
			float x = (float)Luau.luaL_optnumber(L, 1, 0.0);
			float y = (float)Luau.luaL_optnumber(L, 2, 0.0);
			float z = (float)Luau.luaL_optnumber(L, 3, 0.0);
			Luau.lua_class_push<Quaternion>(L, "Quat")->eulerAngles = new Vector3(x, y, z);
			return 1;
		}

		// Token: 0x06004D24 RID: 19748 RVA: 0x0019BB38 File Offset: 0x00199D38
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int FromDirection$BurstManaged(lua_State* L)
		{
			Vector3 lookRotation = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Luau.lua_class_push<Quaternion>(L, "Quat")->SetLookRotation(lookRotation);
			return 1;
		}

		// Token: 0x06004D25 RID: 19749 RVA: 0x0019BB74 File Offset: 0x00199D74
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int GetUpVector$BurstManaged(lua_State* L)
		{
			Quaternion rotation = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = rotation * Vector3.up;
			return 1;
		}

		// Token: 0x06004D26 RID: 19750 RVA: 0x0019BBBC File Offset: 0x00199DBC
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Euler$BurstManaged(lua_State* L)
		{
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = quaternion.eulerAngles;
			return 1;
		}

		// Token: 0x02000C1F RID: 3103
		// (Invoke) Token: 0x06004D28 RID: 19752
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int New_00004C80$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C20 RID: 3104
		internal static class New_00004C80$BurstDirectCall
		{
			// Token: 0x06004D2B RID: 19755 RVA: 0x0019BC00 File Offset: 0x00199E00
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.New_00004C80$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.New_00004C80$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.New_00004C80$PostfixBurstDelegate>(new Bindings.QuatFunctions.New_00004C80$PostfixBurstDelegate(Bindings.QuatFunctions.New)).Value;
				}
				A_0 = Bindings.QuatFunctions.New_00004C80$BurstDirectCall.Pointer;
			}

			// Token: 0x06004D2C RID: 19756 RVA: 0x0019BC40 File Offset: 0x00199E40
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.New_00004C80$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004D2D RID: 19757 RVA: 0x0019BC58 File Offset: 0x00199E58
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.New_00004C80$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.New$BurstManaged(L);
			}

			// Token: 0x04005F9C RID: 24476
			private static IntPtr Pointer;
		}

		// Token: 0x02000C21 RID: 3105
		// (Invoke) Token: 0x06004D2F RID: 19759
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Mul_00004C81$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C22 RID: 3106
		internal static class Mul_00004C81$BurstDirectCall
		{
			// Token: 0x06004D32 RID: 19762 RVA: 0x0019BC8C File Offset: 0x00199E8C
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.Mul_00004C81$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.Mul_00004C81$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.Mul_00004C81$PostfixBurstDelegate>(new Bindings.QuatFunctions.Mul_00004C81$PostfixBurstDelegate(Bindings.QuatFunctions.Mul)).Value;
				}
				A_0 = Bindings.QuatFunctions.Mul_00004C81$BurstDirectCall.Pointer;
			}

			// Token: 0x06004D33 RID: 19763 RVA: 0x0019BCCC File Offset: 0x00199ECC
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.Mul_00004C81$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004D34 RID: 19764 RVA: 0x0019BCE4 File Offset: 0x00199EE4
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.Mul_00004C81$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.Mul$BurstManaged(L);
			}

			// Token: 0x04005F9D RID: 24477
			private static IntPtr Pointer;
		}

		// Token: 0x02000C23 RID: 3107
		// (Invoke) Token: 0x06004D36 RID: 19766
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Eq_00004C82$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C24 RID: 3108
		internal static class Eq_00004C82$BurstDirectCall
		{
			// Token: 0x06004D39 RID: 19769 RVA: 0x0019BD18 File Offset: 0x00199F18
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.Eq_00004C82$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.Eq_00004C82$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.Eq_00004C82$PostfixBurstDelegate>(new Bindings.QuatFunctions.Eq_00004C82$PostfixBurstDelegate(Bindings.QuatFunctions.Eq)).Value;
				}
				A_0 = Bindings.QuatFunctions.Eq_00004C82$BurstDirectCall.Pointer;
			}

			// Token: 0x06004D3A RID: 19770 RVA: 0x0019BD58 File Offset: 0x00199F58
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.Eq_00004C82$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004D3B RID: 19771 RVA: 0x0019BD70 File Offset: 0x00199F70
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.Eq_00004C82$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.Eq$BurstManaged(L);
			}

			// Token: 0x04005F9E RID: 24478
			private static IntPtr Pointer;
		}

		// Token: 0x02000C25 RID: 3109
		// (Invoke) Token: 0x06004D3D RID: 19773
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int FromEuler_00004C84$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C26 RID: 3110
		internal static class FromEuler_00004C84$BurstDirectCall
		{
			// Token: 0x06004D40 RID: 19776 RVA: 0x0019BDA4 File Offset: 0x00199FA4
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.FromEuler_00004C84$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.FromEuler_00004C84$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.FromEuler_00004C84$PostfixBurstDelegate>(new Bindings.QuatFunctions.FromEuler_00004C84$PostfixBurstDelegate(Bindings.QuatFunctions.FromEuler)).Value;
				}
				A_0 = Bindings.QuatFunctions.FromEuler_00004C84$BurstDirectCall.Pointer;
			}

			// Token: 0x06004D41 RID: 19777 RVA: 0x0019BDE4 File Offset: 0x00199FE4
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.FromEuler_00004C84$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004D42 RID: 19778 RVA: 0x0019BDFC File Offset: 0x00199FFC
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.FromEuler_00004C84$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.FromEuler$BurstManaged(L);
			}

			// Token: 0x04005F9F RID: 24479
			private static IntPtr Pointer;
		}

		// Token: 0x02000C27 RID: 3111
		// (Invoke) Token: 0x06004D44 RID: 19780
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int FromDirection_00004C85$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C28 RID: 3112
		internal static class FromDirection_00004C85$BurstDirectCall
		{
			// Token: 0x06004D47 RID: 19783 RVA: 0x0019BE30 File Offset: 0x0019A030
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.FromDirection_00004C85$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.FromDirection_00004C85$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.FromDirection_00004C85$PostfixBurstDelegate>(new Bindings.QuatFunctions.FromDirection_00004C85$PostfixBurstDelegate(Bindings.QuatFunctions.FromDirection)).Value;
				}
				A_0 = Bindings.QuatFunctions.FromDirection_00004C85$BurstDirectCall.Pointer;
			}

			// Token: 0x06004D48 RID: 19784 RVA: 0x0019BE70 File Offset: 0x0019A070
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.FromDirection_00004C85$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004D49 RID: 19785 RVA: 0x0019BE88 File Offset: 0x0019A088
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.FromDirection_00004C85$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.FromDirection$BurstManaged(L);
			}

			// Token: 0x04005FA0 RID: 24480
			private static IntPtr Pointer;
		}

		// Token: 0x02000C29 RID: 3113
		// (Invoke) Token: 0x06004D4B RID: 19787
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int GetUpVector_00004C86$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C2A RID: 3114
		internal static class GetUpVector_00004C86$BurstDirectCall
		{
			// Token: 0x06004D4E RID: 19790 RVA: 0x0019BEBC File Offset: 0x0019A0BC
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.GetUpVector_00004C86$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.GetUpVector_00004C86$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.GetUpVector_00004C86$PostfixBurstDelegate>(new Bindings.QuatFunctions.GetUpVector_00004C86$PostfixBurstDelegate(Bindings.QuatFunctions.GetUpVector)).Value;
				}
				A_0 = Bindings.QuatFunctions.GetUpVector_00004C86$BurstDirectCall.Pointer;
			}

			// Token: 0x06004D4F RID: 19791 RVA: 0x0019BEFC File Offset: 0x0019A0FC
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.GetUpVector_00004C86$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004D50 RID: 19792 RVA: 0x0019BF14 File Offset: 0x0019A114
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.GetUpVector_00004C86$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.GetUpVector$BurstManaged(L);
			}

			// Token: 0x04005FA1 RID: 24481
			private static IntPtr Pointer;
		}

		// Token: 0x02000C2B RID: 3115
		// (Invoke) Token: 0x06004D52 RID: 19794
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Euler_00004C87$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C2C RID: 3116
		internal static class Euler_00004C87$BurstDirectCall
		{
			// Token: 0x06004D55 RID: 19797 RVA: 0x0019BF48 File Offset: 0x0019A148
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.Euler_00004C87$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.Euler_00004C87$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.Euler_00004C87$PostfixBurstDelegate>(new Bindings.QuatFunctions.Euler_00004C87$PostfixBurstDelegate(Bindings.QuatFunctions.Euler)).Value;
				}
				A_0 = Bindings.QuatFunctions.Euler_00004C87$BurstDirectCall.Pointer;
			}

			// Token: 0x06004D56 RID: 19798 RVA: 0x0019BF88 File Offset: 0x0019A188
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.Euler_00004C87$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004D57 RID: 19799 RVA: 0x0019BFA0 File Offset: 0x0019A1A0
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.Euler_00004C87$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.Euler$BurstManaged(L);
			}

			// Token: 0x04005FA2 RID: 24482
			private static IntPtr Pointer;
		}
	}

	// Token: 0x02000C2D RID: 3117
	public struct GorillaLocomotionSettings
	{
		// Token: 0x04005FA3 RID: 24483
		public float velocityLimit;

		// Token: 0x04005FA4 RID: 24484
		public float slideVelocityLimit;

		// Token: 0x04005FA5 RID: 24485
		public float maxJumpSpeed;

		// Token: 0x04005FA6 RID: 24486
		public float jumpMultiplier;
	}

	// Token: 0x02000C2E RID: 3118
	[BurstCompile]
	public struct PlayerInput
	{
		// Token: 0x04005FA7 RID: 24487
		public float leftXAxis;

		// Token: 0x04005FA8 RID: 24488
		[MarshalAs(UnmanagedType.U1)]
		public bool leftPrimaryButton;

		// Token: 0x04005FA9 RID: 24489
		public float rightXAxis;

		// Token: 0x04005FAA RID: 24490
		[MarshalAs(UnmanagedType.U1)]
		public bool rightPrimaryButton;

		// Token: 0x04005FAB RID: 24491
		public float leftYAxis;

		// Token: 0x04005FAC RID: 24492
		[MarshalAs(UnmanagedType.U1)]
		public bool leftSecondaryButton;

		// Token: 0x04005FAD RID: 24493
		public float rightYAxis;

		// Token: 0x04005FAE RID: 24494
		[MarshalAs(UnmanagedType.U1)]
		public bool rightSecondaryButton;

		// Token: 0x04005FAF RID: 24495
		public float leftTrigger;

		// Token: 0x04005FB0 RID: 24496
		public float rightTrigger;

		// Token: 0x04005FB1 RID: 24497
		public float leftGrip;

		// Token: 0x04005FB2 RID: 24498
		public float rightGrip;
	}

	// Token: 0x02000C2F RID: 3119
	public static class JSON
	{
		// Token: 0x06004D58 RID: 19800 RVA: 0x0019BFD4 File Offset: 0x0019A1D4
		public unsafe static Dictionary<object, object> ConsumeTable(lua_State* L, int tableIndex)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			Luau.lua_pushnil(L);
			if (tableIndex < 0)
			{
				tableIndex--;
			}
			while (Luau.lua_next(L, tableIndex) != 0)
			{
				Luau.lua_Types lua_Types = (Luau.lua_Types)Luau.lua_type(L, -1);
				Luau.lua_Types lua_Types2 = (Luau.lua_Types)Luau.lua_type(L, -2);
				object key;
				if (lua_Types2 == Luau.lua_Types.LUA_TSTRING)
				{
					key = new string(Luau.lua_tostring(L, -2));
				}
				else
				{
					if (lua_Types2 != Luau.lua_Types.LUA_TNUMBER)
					{
						FixedString64Bytes fixedString64Bytes = "Invalid key in table, key must be a string or a number";
						Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString64Bytes>(ref fixedString64Bytes) + 2));
						return null;
					}
					key = Luau.lua_tonumber(L, -2);
				}
				switch (lua_Types)
				{
				case Luau.lua_Types.LUA_TBOOLEAN:
					dictionary.Add(key, Luau.lua_toboolean(L, -1) == 1);
					Luau.lua_pop(L, 1);
					continue;
				case Luau.lua_Types.LUA_TNUMBER:
					dictionary.Add(key, Luau.luaL_checknumber(L, -1));
					Luau.lua_pop(L, 1);
					continue;
				case Luau.lua_Types.LUA_TSTRING:
					dictionary.Add(key, new string(Luau.lua_tostring(L, -1)));
					Luau.lua_pop(L, 1);
					continue;
				case Luau.lua_Types.LUA_TTABLE:
				case Luau.lua_Types.LUA_TUSERDATA:
					if (Luau.luaL_getmetafield(L, -1, "metahash") == 1)
					{
						BurstClassInfo.ClassInfo classInfo;
						if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), out classInfo))
						{
							FixedString64Bytes fixedString64Bytes2 = "\"Internal Class Info Error No Metatable Found\"";
							Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString64Bytes>(ref fixedString64Bytes2) + 2));
							return null;
						}
						Luau.lua_pop(L, 1);
						FixedString32Bytes fixedString32Bytes = "Vec3";
						if (classInfo.Name == fixedString32Bytes)
						{
							dictionary.Add(key, *Luau.lua_class_get<Vector3>(L, -1));
							Luau.lua_pop(L, 1);
							continue;
						}
						fixedString32Bytes = "Quat";
						if (classInfo.Name == fixedString32Bytes)
						{
							dictionary.Add(key, *Luau.lua_class_get<Quaternion>(L, -1));
							Luau.lua_pop(L, 1);
							continue;
						}
						FixedString32Bytes fixedString32Bytes2 = "Invalid type in table";
						Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
						return null;
					}
					else
					{
						object obj = Bindings.JSON.ConsumeTable(L, -1);
						Luau.lua_pop(L, 1);
						if (obj != null)
						{
							dictionary.Add(key, obj);
							continue;
						}
						return null;
					}
					break;
				}
				FixedString32Bytes fixedString32Bytes3 = "Unknown type in table";
				Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes3) + 2));
				return null;
			}
			return dictionary;
		}

		// Token: 0x06004D59 RID: 19801 RVA: 0x0019C21C File Offset: 0x0019A41C
		private static int ParseStrictInt(string input)
		{
			if (string.IsNullOrEmpty(input) || input != input.Trim())
			{
				return -1;
			}
			int result;
			if (!int.TryParse(input, out result))
			{
				return -1;
			}
			return result;
		}

		// Token: 0x06004D5A RID: 19802 RVA: 0x0019C250 File Offset: 0x0019A450
		private static bool CompareKeys(JObject obj, HashSet<string> set)
		{
			HashSet<string> equals = new HashSet<string>(from p in obj.Properties()
			select p.Name);
			return set.SetEquals(equals);
		}

		// Token: 0x06004D5B RID: 19803 RVA: 0x0019C294 File Offset: 0x0019A494
		public unsafe static bool PushTable(lua_State* L, JObject table)
		{
			Luau.lua_createtable(L, 0, 0);
			foreach (KeyValuePair<string, JToken> keyValuePair in table)
			{
				if (keyValuePair.Key != null && keyValuePair.Value != null)
				{
					int num = Bindings.JSON.ParseStrictInt(keyValuePair.Key);
					if (num == -1)
					{
						Luau.lua_pushstring(L, keyValuePair.Key);
					}
					if (keyValuePair.Value is JObject)
					{
						if (Bindings.JSON.CompareKeys((JObject)keyValuePair.Value, new HashSet<string>
						{
							"x",
							"y",
							"z"
						}))
						{
							JObject jobject = keyValuePair.Value as JObject;
							float x = jobject["x"].ToObject<float>();
							float y = jobject["y"].ToObject<float>();
							float z = jobject["z"].ToObject<float>();
							Vector3 vector = new Vector3(x, y, z);
							*Luau.lua_class_push<Vector3>(L) = vector;
						}
						else if (Bindings.JSON.CompareKeys((JObject)keyValuePair.Value, new HashSet<string>
						{
							"x",
							"y",
							"z",
							"w"
						}))
						{
							JObject jobject2 = keyValuePair.Value as JObject;
							float x2 = jobject2["x"].ToObject<float>();
							float y2 = jobject2["y"].ToObject<float>();
							float z2 = jobject2["z"].ToObject<float>();
							float w = jobject2["w"].ToObject<float>();
							Quaternion quaternion = new Quaternion(x2, y2, z2, w);
							*Luau.lua_class_push<Quaternion>(L) = quaternion;
						}
						else
						{
							Bindings.JSON.PushTable(L, (JObject)keyValuePair.Value);
						}
					}
					else if (keyValuePair.Value is JValue)
					{
						JTokenType type = keyValuePair.Value.Type;
						if (type == JTokenType.Integer)
						{
							Luau.lua_pushnumber(L, (double)keyValuePair.Value.ToObject<int>());
						}
						else if (type == JTokenType.Boolean)
						{
							Luau.lua_pushboolean(L, keyValuePair.Value.ToObject<bool>() ? 1 : 0);
						}
						else if (type == JTokenType.Float)
						{
							Luau.lua_pushnumber(L, keyValuePair.Value.ToObject<double>());
						}
						else
						{
							if (type != JTokenType.String)
							{
								continue;
							}
							Luau.lua_pushstring(L, keyValuePair.Value.ToString());
						}
					}
					if (num == -1)
					{
						Luau.lua_rawset(L, -3);
					}
					else
					{
						Luau.lua_rawseti(L, -2, num);
					}
				}
			}
			return true;
		}

		// Token: 0x06004D5C RID: 19804 RVA: 0x0019C544 File Offset: 0x0019A744
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int DataSave(lua_State* L)
		{
			int result;
			try
			{
				string text = JsonConvert.SerializeObject(Bindings.JSON.ConsumeTable(L, 1), Formatting.Indented);
				if (text.Length > 10000)
				{
					Luau.luaL_errorL(L, "Save exceeds 10000 bytes", Array.Empty<string>());
					result = 0;
				}
				else
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(Path.Join(Bindings.JSON.ModIODirectory, "saves", CustomMapLoader.LoadedMapModId.ToString()));
					if (!directoryInfo.Exists)
					{
						directoryInfo.Create();
					}
					File.WriteAllText(Path.Join(directoryInfo.FullName, "luau.json"), text);
					result = 0;
				}
			}
			catch
			{
				Luau.luaL_errorL(L, "Argument 2 must be a table", Array.Empty<string>());
				result = 0;
			}
			return result;
		}

		// Token: 0x06004D5D RID: 19805 RVA: 0x0019C610 File Offset: 0x0019A810
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int DataLoad(lua_State* L)
		{
			int result;
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(Path.Join(Bindings.JSON.ModIODirectory, "saves", CustomMapLoader.LoadedMapModId.ToString()));
				if (!directoryInfo.Exists)
				{
					Luau.lua_createtable(L, 0, 0);
					result = 1;
				}
				else
				{
					FileInfo[] files = directoryInfo.GetFiles("luau.json");
					if (files.Length == 0)
					{
						Luau.lua_createtable(L, 0, 0);
						result = 1;
					}
					else
					{
						JObject table = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(files[0].FullName));
						if (Bindings.JSON.PushTable(L, table))
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
					}
				}
			}
			catch
			{
				Luau.luaL_errorL(L, "Error while loading data", Array.Empty<string>());
				result = 0;
			}
			return result;
		}

		// Token: 0x04005FB3 RID: 24499
		private static string ModIODirectory = Path.Join(Path.Join(Application.persistentDataPath, "mod.io", "06657"), "data");
	}

	// Token: 0x02000C31 RID: 3121
	[BurstCompile]
	public struct LuauRoomState
	{
		// Token: 0x04005FB6 RID: 24502
		[MarshalAs(UnmanagedType.U1)]
		public bool IsQuest;

		// Token: 0x04005FB7 RID: 24503
		public float FPS;

		// Token: 0x04005FB8 RID: 24504
		[MarshalAs(UnmanagedType.U1)]
		public bool IsPrivate;

		// Token: 0x04005FB9 RID: 24505
		public FixedString32Bytes RoomCode;
	}

	// Token: 0x02000C32 RID: 3122
	public static class PlayerUtils
	{
		// Token: 0x06004D62 RID: 19810 RVA: 0x0019C728 File Offset: 0x0019A928
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int TeleportPlayer(lua_State* L)
		{
			Vector3 a = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			bool keepVelocity = Luau.lua_toboolean(L, 2) == 1;
			if (GTPlayer.hasInstance)
			{
				GTPlayer instance = GTPlayer.Instance;
				Vector3 position = instance.transform.position;
				Vector3 b = instance.mainCamera.transform.position - position;
				Vector3 position2 = a - b;
				instance.TeleportTo(position2, instance.transform.rotation, keepVelocity, false);
			}
			return 0;
		}

		// Token: 0x06004D63 RID: 19811 RVA: 0x0019C7A8 File Offset: 0x0019A9A8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetVelocity(lua_State* L)
		{
			Vector3 velocity = *Luau.lua_class_get<Vector3>(L, 1);
			if (GTPlayer.hasInstance)
			{
				GTPlayer.Instance.SetVelocity(velocity);
			}
			return 0;
		}
	}

	// Token: 0x02000C33 RID: 3123
	public static class RayCastUtils
	{
		// Token: 0x06004D64 RID: 19812 RVA: 0x0019C7D8 File Offset: 0x0019A9D8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int RayCast(lua_State* L)
		{
			Vector3 origin = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 direction = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			if (!Physics.Raycast(origin, direction, out Bindings.RayCastUtils.rayHit))
			{
				return 0;
			}
			Luau.lua_createtable(L, 0, 0);
			Luau.lua_pushstring(L, "distance");
			Luau.lua_pushnumber(L, (double)Bindings.RayCastUtils.rayHit.distance);
			Luau.lua_rawset(L, -3);
			Luau.lua_pushstring(L, "point");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Bindings.RayCastUtils.rayHit.point;
			Luau.lua_rawset(L, -3);
			Luau.lua_pushstring(L, "normal");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Bindings.RayCastUtils.rayHit.normal;
			Luau.lua_rawset(L, -3);
			Luau.lua_pushstring(L, "object");
			IntPtr ptr;
			if (Bindings.LuauGameObjectList.TryGetValue(Bindings.RayCastUtils.rayHit.transform.gameObject, out ptr))
			{
				Luau.lua_class_push(L, "GameObject", ptr);
			}
			else
			{
				Luau.lua_pushnil(L);
			}
			Luau.lua_rawset(L, -3);
			Luau.lua_pushstring(L, "player");
			Collider collider = Bindings.RayCastUtils.rayHit.collider;
			VRRig vrrig = (collider != null) ? collider.GetComponentInParent<VRRig>() : null;
			if (vrrig != null)
			{
				NetPlayer creator = vrrig.creator;
				if (creator != null)
				{
					IntPtr ptr2;
					if (Bindings.LuauPlayerList.TryGetValue(creator.ActorNumber, out ptr2))
					{
						Luau.lua_class_push(L, "Player", ptr2);
					}
					else
					{
						Luau.lua_pushnil(L);
					}
				}
				else
				{
					Luau.lua_pushnil(L);
				}
			}
			else
			{
				Luau.lua_pushnil(L);
			}
			Luau.lua_rawset(L, -3);
			return 1;
		}

		// Token: 0x04005FBA RID: 24506
		public static RaycastHit rayHit;
	}

	// Token: 0x02000C34 RID: 3124
	public static class Components
	{
		// Token: 0x06004D65 RID: 19813 RVA: 0x0019C97B File Offset: 0x0019AB7B
		public unsafe static void Build(lua_State* L)
		{
			Bindings.Components.LuauParticleSystemBindings.Builder(L);
			Bindings.Components.LuauAudioSourceBindings.Builder(L);
			Bindings.Components.LuauLightBindings.Builder(L);
			Bindings.Components.LuauAnimatorBindings.Builder(L);
		}

		// Token: 0x04005FBB RID: 24507
		public static Dictionary<IntPtr, object> ComponentList = new Dictionary<IntPtr, object>();

		// Token: 0x02000C35 RID: 3125
		public static class LuauParticleSystemBindings
		{
			// Token: 0x06004D67 RID: 19815 RVA: 0x0019C9A4 File Offset: 0x0019ABA4
			public unsafe static void Builder(lua_State* L)
			{
				LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.Components.LuauParticleSystemBindings.LuauParticleSystem>("ParticleSystem").AddFunction("play", new lua_CFunction(Bindings.Components.LuauParticleSystemBindings.play)).AddFunction("stop", new lua_CFunction(Bindings.Components.LuauParticleSystemBindings.stop)).AddFunction("clear", new lua_CFunction(Bindings.Components.LuauParticleSystemBindings.clear)).Build(L, false));
			}

			// Token: 0x06004D68 RID: 19816 RVA: 0x0019CA10 File Offset: 0x0019AC10
			public unsafe static ParticleSystem GetParticleSystem(lua_State* L)
			{
				Bindings.Components.LuauParticleSystemBindings.LuauParticleSystem* value = Luau.lua_class_get<Bindings.Components.LuauParticleSystemBindings.LuauParticleSystem>(L, 1);
				object obj;
				if (Bindings.Components.ComponentList.TryGetValue((IntPtr)((void*)value), out obj))
				{
					ParticleSystem particleSystem = obj as ParticleSystem;
					if (particleSystem != null)
					{
						return particleSystem;
					}
				}
				return null;
			}

			// Token: 0x06004D69 RID: 19817 RVA: 0x0019CA48 File Offset: 0x0019AC48
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int play(lua_State* L)
			{
				ParticleSystem particleSystem = Bindings.Components.LuauParticleSystemBindings.GetParticleSystem(L);
				if (particleSystem != null)
				{
					particleSystem.Play();
				}
				return 0;
			}

			// Token: 0x06004D6A RID: 19818 RVA: 0x0019CA6C File Offset: 0x0019AC6C
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int stop(lua_State* L)
			{
				ParticleSystem particleSystem = Bindings.Components.LuauParticleSystemBindings.GetParticleSystem(L);
				if (particleSystem != null)
				{
					particleSystem.Stop();
				}
				return 0;
			}

			// Token: 0x06004D6B RID: 19819 RVA: 0x0019CA90 File Offset: 0x0019AC90
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int clear(lua_State* L)
			{
				ParticleSystem particleSystem = Bindings.Components.LuauParticleSystemBindings.GetParticleSystem(L);
				if (particleSystem != null)
				{
					particleSystem.Clear();
				}
				return 0;
			}

			// Token: 0x02000C36 RID: 3126
			public struct LuauParticleSystem
			{
				// Token: 0x04005FBC RID: 24508
				public int x;
			}
		}

		// Token: 0x02000C37 RID: 3127
		public static class LuauAudioSourceBindings
		{
			// Token: 0x06004D6C RID: 19820 RVA: 0x0019CAB4 File Offset: 0x0019ACB4
			public unsafe static void Builder(lua_State* L)
			{
				LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.Components.LuauAudioSourceBindings.LuauAudioSource>("AudioSource").AddFunction("play", new lua_CFunction(Bindings.Components.LuauAudioSourceBindings.play)).AddFunction("setVolume", new lua_CFunction(Bindings.Components.LuauAudioSourceBindings.setVolume)).AddFunction("setLoop", new lua_CFunction(Bindings.Components.LuauAudioSourceBindings.setLoop)).AddFunction("setPitch", new lua_CFunction(Bindings.Components.LuauAudioSourceBindings.setPitch)).AddFunction("setMinDistance", new lua_CFunction(Bindings.Components.LuauAudioSourceBindings.setMinDistance)).AddFunction("setMaxDistance", new lua_CFunction(Bindings.Components.LuauAudioSourceBindings.setMaxDistance)).Build(L, false));
			}

			// Token: 0x06004D6D RID: 19821 RVA: 0x0019CB64 File Offset: 0x0019AD64
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static AudioSource GetAudioSource(lua_State* L)
			{
				Bindings.Components.LuauAudioSourceBindings.LuauAudioSource* value = Luau.lua_class_get<Bindings.Components.LuauAudioSourceBindings.LuauAudioSource>(L, 1);
				object obj;
				if (Bindings.Components.ComponentList.TryGetValue((IntPtr)((void*)value), out obj))
				{
					AudioSource audioSource = obj as AudioSource;
					if (audioSource != null)
					{
						return audioSource;
					}
				}
				return null;
			}

			// Token: 0x06004D6E RID: 19822 RVA: 0x0019CB9C File Offset: 0x0019AD9C
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int play(lua_State* L)
			{
				AudioSource audioSource = Bindings.Components.LuauAudioSourceBindings.GetAudioSource(L);
				if (audioSource != null)
				{
					audioSource.Play();
				}
				return 0;
			}

			// Token: 0x06004D6F RID: 19823 RVA: 0x0019CBC0 File Offset: 0x0019ADC0
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setVolume(lua_State* L)
			{
				AudioSource audioSource = Bindings.Components.LuauAudioSourceBindings.GetAudioSource(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (audioSource != null)
				{
					audioSource.volume = (float)num;
				}
				return 0;
			}

			// Token: 0x06004D70 RID: 19824 RVA: 0x0019CBF0 File Offset: 0x0019ADF0
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setLoop(lua_State* L)
			{
				AudioSource audioSource = Bindings.Components.LuauAudioSourceBindings.GetAudioSource(L);
				bool loop = Luau.lua_toboolean(L, 2) == 1;
				if (audioSource != null)
				{
					audioSource.loop = loop;
				}
				return 0;
			}

			// Token: 0x06004D71 RID: 19825 RVA: 0x0019CC20 File Offset: 0x0019AE20
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setPitch(lua_State* L)
			{
				AudioSource audioSource = Bindings.Components.LuauAudioSourceBindings.GetAudioSource(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (audioSource != null)
				{
					audioSource.pitch = (float)num;
				}
				return 0;
			}

			// Token: 0x06004D72 RID: 19826 RVA: 0x0019CC50 File Offset: 0x0019AE50
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setMinDistance(lua_State* L)
			{
				AudioSource audioSource = Bindings.Components.LuauAudioSourceBindings.GetAudioSource(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (audioSource != null)
				{
					audioSource.minDistance = (float)num;
				}
				return 0;
			}

			// Token: 0x06004D73 RID: 19827 RVA: 0x0019CC80 File Offset: 0x0019AE80
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setMaxDistance(lua_State* L)
			{
				AudioSource audioSource = Bindings.Components.LuauAudioSourceBindings.GetAudioSource(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (audioSource != null)
				{
					audioSource.maxDistance = (float)num;
				}
				return 0;
			}

			// Token: 0x02000C38 RID: 3128
			public struct LuauAudioSource
			{
				// Token: 0x04005FBD RID: 24509
				public int x;
			}
		}

		// Token: 0x02000C39 RID: 3129
		public static class LuauLightBindings
		{
			// Token: 0x06004D74 RID: 19828 RVA: 0x0019CCB0 File Offset: 0x0019AEB0
			public unsafe static void Builder(lua_State* L)
			{
				LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.Components.LuauLightBindings.LuauLight>("Light").AddFunction("setColor", new lua_CFunction(Bindings.Components.LuauLightBindings.setColor)).AddFunction("setIntensity", new lua_CFunction(Bindings.Components.LuauLightBindings.setIntensity)).AddFunction("setRange", new lua_CFunction(Bindings.Components.LuauLightBindings.setRange)).Build(L, false));
			}

			// Token: 0x06004D75 RID: 19829 RVA: 0x0019CD1C File Offset: 0x0019AF1C
			public unsafe static Light GetLight(lua_State* L)
			{
				Bindings.Components.LuauLightBindings.LuauLight* value = Luau.lua_class_get<Bindings.Components.LuauLightBindings.LuauLight>(L, 1);
				object obj;
				if (Bindings.Components.ComponentList.TryGetValue((IntPtr)((void*)value), out obj))
				{
					Light light = obj as Light;
					if (light != null)
					{
						return light;
					}
				}
				return null;
			}

			// Token: 0x06004D76 RID: 19830 RVA: 0x0019CD54 File Offset: 0x0019AF54
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setColor(lua_State* L)
			{
				Light light = Bindings.Components.LuauLightBindings.GetLight(L);
				Vector3 vector = *Luau.lua_class_get<Vector3>(L, 2);
				if (light != null)
				{
					light.color = new Color(vector.x, vector.y, vector.z);
				}
				return 0;
			}

			// Token: 0x06004D77 RID: 19831 RVA: 0x0019CD9C File Offset: 0x0019AF9C
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setIntensity(lua_State* L)
			{
				Light light = Bindings.Components.LuauLightBindings.GetLight(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (light != null)
				{
					light.intensity = (float)num;
				}
				return 0;
			}

			// Token: 0x06004D78 RID: 19832 RVA: 0x0019CDCC File Offset: 0x0019AFCC
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setRange(lua_State* L)
			{
				Light light = Bindings.Components.LuauLightBindings.GetLight(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (light != null)
				{
					light.range = (float)num;
				}
				return 0;
			}

			// Token: 0x02000C3A RID: 3130
			public struct LuauLight
			{
				// Token: 0x04005FBE RID: 24510
				public int x;
			}
		}

		// Token: 0x02000C3B RID: 3131
		public static class LuauAnimatorBindings
		{
			// Token: 0x06004D79 RID: 19833 RVA: 0x0019CDFC File Offset: 0x0019AFFC
			public unsafe static void Builder(lua_State* L)
			{
				LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.Components.LuauAnimatorBindings.LuauAnimator>("Animator").AddFunction("setSpeed", new lua_CFunction(Bindings.Components.LuauAnimatorBindings.setSpeed)).AddFunction("startPlayback", new lua_CFunction(Bindings.Components.LuauAnimatorBindings.startPlayback)).AddFunction("stopPlayback", new lua_CFunction(Bindings.Components.LuauAnimatorBindings.stopPlayback)).AddFunction("reset", new lua_CFunction(Bindings.Components.LuauAnimatorBindings.reset)).Build(L, false));
			}

			// Token: 0x06004D7A RID: 19834 RVA: 0x0019CE80 File Offset: 0x0019B080
			public unsafe static Animator GetAnimator(lua_State* L)
			{
				Bindings.Components.LuauAnimatorBindings.LuauAnimator* value = Luau.lua_class_get<Bindings.Components.LuauAnimatorBindings.LuauAnimator>(L, 1);
				object obj;
				if (Bindings.Components.ComponentList.TryGetValue((IntPtr)((void*)value), out obj))
				{
					Animator animator = obj as Animator;
					if (animator != null)
					{
						return animator;
					}
				}
				return null;
			}

			// Token: 0x06004D7B RID: 19835 RVA: 0x0019CEB8 File Offset: 0x0019B0B8
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setSpeed(lua_State* L)
			{
				Animator animator = Bindings.Components.LuauAnimatorBindings.GetAnimator(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (animator != null)
				{
					animator.speed = (float)num;
				}
				return 0;
			}

			// Token: 0x06004D7C RID: 19836 RVA: 0x0019CEE8 File Offset: 0x0019B0E8
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int startPlayback(lua_State* L)
			{
				Animator animator = Bindings.Components.LuauAnimatorBindings.GetAnimator(L);
				if (animator != null)
				{
					animator.StartPlayback();
				}
				return 0;
			}

			// Token: 0x06004D7D RID: 19837 RVA: 0x0019CF0C File Offset: 0x0019B10C
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int stopPlayback(lua_State* L)
			{
				Animator animator = Bindings.Components.LuauAnimatorBindings.GetAnimator(L);
				if (animator != null)
				{
					animator.StopPlayback();
				}
				return 0;
			}

			// Token: 0x06004D7E RID: 19838 RVA: 0x0019CF30 File Offset: 0x0019B130
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int reset(lua_State* L)
			{
				Animator animator = Bindings.Components.LuauAnimatorBindings.GetAnimator(L);
				if (animator != null)
				{
					animator.ResetToEntryState();
				}
				return 0;
			}

			// Token: 0x02000C3C RID: 3132
			public struct LuauAnimator
			{
				// Token: 0x04005FBF RID: 24511
				public int x;
			}
		}
	}
}
