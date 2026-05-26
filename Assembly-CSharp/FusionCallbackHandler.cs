using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200042D RID: 1069
public class FusionCallbackHandler : SimulationBehaviour, INetworkRunnerCallbacks, IPublicFacingInterface
{
	// Token: 0x0600195C RID: 6492 RVA: 0x0008E468 File Offset: 0x0008C668
	public void Setup(NetworkSystemFusion parentController)
	{
		this.parent = parentController;
		this.parent.runner.AddCallbacks(new INetworkRunnerCallbacks[]
		{
			this
		});
	}

	// Token: 0x0600195D RID: 6493 RVA: 0x0008E48B File Offset: 0x0008C68B
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		this.RemoveCallbacks();
	}

	// Token: 0x0600195E RID: 6494 RVA: 0x0008E49C File Offset: 0x0008C69C
	private void RemoveCallbacks()
	{
		FusionCallbackHandler.<RemoveCallbacks>d__3 <RemoveCallbacks>d__;
		<RemoveCallbacks>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RemoveCallbacks>d__.<>4__this = this;
		<RemoveCallbacks>d__.<>1__state = -1;
		<RemoveCallbacks>d__.<>t__builder.Start<FusionCallbackHandler.<RemoveCallbacks>d__3>(ref <RemoveCallbacks>d__);
	}

	// Token: 0x0600195F RID: 6495 RVA: 0x0008E4D3 File Offset: 0x0008C6D3
	public void OnConnectedToServer(NetworkRunner runner)
	{
		this.parent.OnJoinedSession();
	}

	// Token: 0x06001960 RID: 6496 RVA: 0x0008E4E0 File Offset: 0x0008C6E0
	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
		this.parent.OnJoinFailed(reason);
	}

	// Token: 0x06001961 RID: 6497 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	// Token: 0x06001962 RID: 6498 RVA: 0x0008E4F0 File Offset: 0x0008C6F0
	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
		this.parent.CustomAuthenticationResponse(data);
		Debug.Log("Received custom auth response:");
		foreach (KeyValuePair<string, object> keyValuePair in data)
		{
			Debug.Log(keyValuePair.Key + ":" + (keyValuePair.Value as string));
		}
	}

	// Token: 0x06001963 RID: 6499 RVA: 0x0008E570 File Offset: 0x0008C770
	public void OnDisconnectedFromServer(NetworkRunner runner)
	{
		this.parent.OnDisconnectedFromSession();
	}

	// Token: 0x06001964 RID: 6500 RVA: 0x0008E57D File Offset: 0x0008C77D
	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
		this.parent.MigrateHost(runner, hostMigrationToken);
	}

	// Token: 0x06001965 RID: 6501 RVA: 0x0008E58C File Offset: 0x0008C78C
	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
		NetworkedInput input2 = NetInput.GetInput();
		input.Set<NetworkedInput>(input2);
	}

	// Token: 0x06001966 RID: 6502 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	// Token: 0x06001967 RID: 6503 RVA: 0x0008E5A8 File Offset: 0x0008C7A8
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		this.parent.OnFusionPlayerJoined(player);
	}

	// Token: 0x06001968 RID: 6504 RVA: 0x0008E5B6 File Offset: 0x0008C7B6
	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		this.parent.OnFusionPlayerLeft(player);
	}

	// Token: 0x06001969 RID: 6505 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
	{
	}

	// Token: 0x0600196A RID: 6506 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnSceneLoadDone(NetworkRunner runner)
	{
	}

	// Token: 0x0600196B RID: 6507 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnSceneLoadStart(NetworkRunner runner)
	{
	}

	// Token: 0x0600196C RID: 6508 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	// Token: 0x0600196D RID: 6509 RVA: 0x0008E5C4 File Offset: 0x0008C7C4
	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
		this.parent.OnRunnerShutDown();
	}

	// Token: 0x0600196E RID: 6510 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	// Token: 0x0600196F RID: 6511 RVA: 0x0008E5D4 File Offset: 0x0008C7D4
	[Rpc(Channel = RpcChannel.Reliable)]
	public unsafe static void RPC_OnEventRaisedReliable(NetworkRunner runner, byte eventCode, byte[] byteData, bool hasOps, byte[] netOptsData, RpcInfo info = default(RpcInfo))
	{
		if (NetworkBehaviourUtils.InvokeRpc)
		{
			NetworkBehaviourUtils.InvokeRpc = false;
		}
		else
		{
			if (runner == null)
			{
				throw new ArgumentNullException("runner");
			}
			if (runner.Stage != SimulationStages.Resimulate)
			{
				int num = 8;
				num += 4;
				num += (byteData.Length * 1 + 4 + 3 & -4);
				num += 4;
				num += (netOptsData.Length * 1 + 4 + 3 & -4);
				if (SimulationMessage.CanAllocateUserPayload(num))
				{
					if (runner.HasAnyActiveConnections())
					{
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
						*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void FusionCallbackHandler::RPC_OnEventRaisedReliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)"));
						int num2 = 8;
						ptr2[num2] = eventCode;
						num2 += (1 + 3 & -4);
						*(int*)(ptr2 + num2) = byteData.Length;
						num2 += 4;
						num2 = (Native.CopyFromArray<byte>((void*)(ptr2 + num2), byteData) + 3 & -4) + num2;
						ReadWriteUtilsForWeaver.WriteBoolean((int*)(ptr2 + num2), hasOps);
						num2 += 4;
						*(int*)(ptr2 + num2) = netOptsData.Length;
						num2 += 4;
						num2 = (Native.CopyFromArray<byte>((void*)(ptr2 + num2), netOptsData) + 3 & -4) + num2;
						ptr->Offset = num2 * 8;
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
					info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
					goto IL_10;
				}
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void FusionCallbackHandler::RPC_OnEventRaisedReliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)", num);
			}
			return;
		}
		IL_10:
		object data = byteData.ByteDeserialize();
		NetEventOptions opts = null;
		if (hasOps)
		{
			opts = (NetEventOptions)netOptsData.ByteDeserialize();
		}
		if (!FusionCallbackHandler.CanRecieveEvent(runner, opts, info))
		{
			return;
		}
		NetworkSystem.Instance.RaiseEvent(eventCode, data, info.Source.PlayerId);
	}

	// Token: 0x06001970 RID: 6512 RVA: 0x0008E7F8 File Offset: 0x0008C9F8
	[Rpc(Channel = RpcChannel.Unreliable)]
	public unsafe static void RPC_OnEventRaisedUnreliable(NetworkRunner runner, byte eventCode, byte[] byteData, bool hasOps, byte[] netOptsData, RpcInfo info = default(RpcInfo))
	{
		if (NetworkBehaviourUtils.InvokeRpc)
		{
			NetworkBehaviourUtils.InvokeRpc = false;
		}
		else
		{
			if (runner == null)
			{
				throw new ArgumentNullException("runner");
			}
			if (runner.Stage != SimulationStages.Resimulate)
			{
				int num = 8;
				num += 4;
				num += (byteData.Length * 1 + 4 + 3 & -4);
				num += 4;
				num += (netOptsData.Length * 1 + 4 + 3 & -4);
				if (SimulationMessage.CanAllocateUserPayload(num))
				{
					if (runner.HasAnyActiveConnections())
					{
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
						*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void FusionCallbackHandler::RPC_OnEventRaisedUnreliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)"));
						int num2 = 8;
						ptr2[num2] = eventCode;
						num2 += (1 + 3 & -4);
						*(int*)(ptr2 + num2) = byteData.Length;
						num2 += 4;
						num2 = (Native.CopyFromArray<byte>((void*)(ptr2 + num2), byteData) + 3 & -4) + num2;
						ReadWriteUtilsForWeaver.WriteBoolean((int*)(ptr2 + num2), hasOps);
						num2 += 4;
						*(int*)(ptr2 + num2) = netOptsData.Length;
						num2 += 4;
						num2 = (Native.CopyFromArray<byte>((void*)(ptr2 + num2), netOptsData) + 3 & -4) + num2;
						ptr->Offset = num2 * 8;
						ptr->SetUnreliable();
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
					info = RpcInfo.FromLocal(runner, RpcChannel.Unreliable, RpcHostMode.SourceIsServer);
					goto IL_10;
				}
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void FusionCallbackHandler::RPC_OnEventRaisedUnreliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)", num);
			}
			return;
		}
		IL_10:
		object data = byteData.ByteDeserialize();
		NetEventOptions opts = null;
		if (hasOps)
		{
			opts = (NetEventOptions)netOptsData.ByteDeserialize();
		}
		if (!FusionCallbackHandler.CanRecieveEvent(runner, opts, info))
		{
			return;
		}
		NetworkSystem.Instance.RaiseEvent(eventCode, data, info.Source.PlayerId);
	}

	// Token: 0x06001971 RID: 6513 RVA: 0x0008EA24 File Offset: 0x0008CC24
	private static bool CanRecieveEvent(NetworkRunner runner, NetEventOptions opts, RpcInfo info)
	{
		if (opts != null)
		{
			if (opts.Reciever != NetEventOptions.RecieverTarget.all)
			{
				if (opts.Reciever == NetEventOptions.RecieverTarget.master && !NetworkSystem.Instance.IsMasterClient)
				{
					return false;
				}
				if (info.Source == runner.LocalPlayer)
				{
					return false;
				}
			}
			if (opts.TargetActors != null && !opts.TargetActors.Contains(runner.LocalPlayer.PlayerId))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001972 RID: 6514 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x06001973 RID: 6515 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x06001974 RID: 6516 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
	{
	}

	// Token: 0x06001975 RID: 6517 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
	{
	}

	// Token: 0x06001976 RID: 6518 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
	{
	}

	// Token: 0x06001978 RID: 6520 RVA: 0x0008EA98 File Offset: 0x0008CC98
	[NetworkRpcStaticWeavedInvoker("System.Void FusionCallbackHandler::RPC_OnEventRaisedReliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_OnEventRaisedReliable@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		byte b = ptr[num];
		num += (1 + 3 & -4);
		byte eventCode = b;
		byte[] array = new byte[*(int*)(ptr + num)];
		num += 4;
		num = (Native.CopyToArray<byte>(array, (void*)(ptr + num)) + 3 & -4) + num;
		bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(ptr + num));
		num += 4;
		bool hasOps = flag;
		byte[] array2 = new byte[*(int*)(ptr + num)];
		num += 4;
		num = (Native.CopyToArray<byte>(array2, (void*)(ptr + num)) + 3 & -4) + num;
		RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
		NetworkBehaviourUtils.InvokeRpc = true;
		FusionCallbackHandler.RPC_OnEventRaisedReliable(runner, eventCode, array, hasOps, array2, info);
	}

	// Token: 0x06001979 RID: 6521 RVA: 0x0008EBA8 File Offset: 0x0008CDA8
	[NetworkRpcStaticWeavedInvoker("System.Void FusionCallbackHandler::RPC_OnEventRaisedUnreliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_OnEventRaisedUnreliable@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		byte b = ptr[num];
		num += (1 + 3 & -4);
		byte eventCode = b;
		byte[] array = new byte[*(int*)(ptr + num)];
		num += 4;
		num = (Native.CopyToArray<byte>(array, (void*)(ptr + num)) + 3 & -4) + num;
		bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(ptr + num));
		num += 4;
		bool hasOps = flag;
		byte[] array2 = new byte[*(int*)(ptr + num)];
		num += 4;
		num = (Native.CopyToArray<byte>(array2, (void*)(ptr + num)) + 3 & -4) + num;
		RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
		NetworkBehaviourUtils.InvokeRpc = true;
		FusionCallbackHandler.RPC_OnEventRaisedUnreliable(runner, eventCode, array, hasOps, array2, info);
	}

	// Token: 0x04002452 RID: 9298
	private NetworkSystemFusion parent;
}
