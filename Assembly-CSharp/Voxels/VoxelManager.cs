using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Fusion;
using GorillaExtensions;
using K4os.Compression.LZ4;
using Photon.Pun;
using PlayFab.Internal;
using Unity.Mathematics;
using UnityEngine;

namespace Voxels
{
	// Token: 0x020012FB RID: 4859
	[NetworkBehaviourWeaved(0)]
	public class VoxelManager : NetworkComponent
	{
		// Token: 0x17000BAB RID: 2987
		// (get) Token: 0x06007944 RID: 31044 RVA: 0x0027E502 File Offset: 0x0027C702
		public static bool HasAuthority
		{
			get
			{
				return !VoxelManager.InRoom || VoxelManager._instance.IsLocallyOwned;
			}
		}

		// Token: 0x17000BAC RID: 2988
		// (get) Token: 0x06007945 RID: 31045 RVA: 0x0027E517 File Offset: 0x0027C717
		public static bool InRoom
		{
			get
			{
				return NetworkSystem.Instance.InRoom;
			}
		}

		// Token: 0x06007946 RID: 31046 RVA: 0x0027E523 File Offset: 0x0027C723
		protected override void Start()
		{
			base.Start();
			VoxelManager._instance = this;
		}

		// Token: 0x06007947 RID: 31047 RVA: 0x0027E534 File Offset: 0x0027C734
		internal override void OnEnable()
		{
			NetworkBehaviourUtils.InternalOnEnable(this);
			base.OnEnable();
			RoomSystem.JoinedRoomEvent += new Action(this.OnNetworkJoinedRoom);
			RoomSystem.LeftRoomEvent += new Action(this.OnNetworkLeftRoom);
			NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeft;
		}

		// Token: 0x06007948 RID: 31048 RVA: 0x0027E5A4 File Offset: 0x0027C7A4
		internal override void OnDisable()
		{
			NetworkBehaviourUtils.InternalOnDisable(this);
			base.OnDisable();
			RoomSystem.JoinedRoomEvent -= new Action(this.OnNetworkJoinedRoom);
			RoomSystem.LeftRoomEvent -= new Action(this.OnNetworkLeftRoom);
			NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerLeft;
		}

		// Token: 0x06007949 RID: 31049 RVA: 0x0027E614 File Offset: 0x0027C814
		private void Update()
		{
			if (!VoxelManager._shouldProcessQueues)
			{
				return;
			}
			VoxelManager._shouldProcessQueues = false;
			if (!PhotonNetwork.IsMasterClient)
			{
				VoxelManager._initQueues.Clear();
				return;
			}
			if (VoxelManager._worlds.Count == 0)
			{
				return;
			}
			VoxelManager.UpdateTransferLog();
			foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
			{
				VoxelManager.StateInitQueue stateInitQueue;
				if (!netPlayer.IsLocal && VoxelManager._initQueues.TryGetValue(netPlayer.ActorNumber, out stateInitQueue) && !stateInitQueue.IsEmpty)
				{
					if (VoxelManager._sendRate < 10000)
					{
						VoxelManager.SendNextChunk(stateInitQueue);
					}
					if (!stateInitQueue.IsEmpty)
					{
						VoxelManager._shouldProcessQueues = true;
					}
				}
			}
		}

		// Token: 0x0600794A RID: 31050 RVA: 0x0027E6D8 File Offset: 0x0027C8D8
		private static void UpdateTransferLog()
		{
			float num = Time.realtimeSinceStartup - 1f;
			while (VoxelManager._sendHistory.Count > 0 && VoxelManager._sendHistory.Peek().Item1 <= num)
			{
				int item = VoxelManager._sendHistory.Dequeue().Item2;
				VoxelManager._sendRate -= item;
			}
		}

		// Token: 0x0600794B RID: 31051 RVA: 0x0027E72E File Offset: 0x0027C92E
		private static void EnqueueTransferLog(int bytes)
		{
			VoxelManager._sendHistory.Enqueue(new ValueTuple<float, int>(Time.realtimeSinceStartup, bytes));
			VoxelManager._sendRate += bytes;
		}

		// Token: 0x0600794C RID: 31052 RVA: 0x0027E754 File Offset: 0x0027C954
		public static void Register(VoxelWorld world)
		{
			VoxelManager._worlds[world.Id] = world;
			Debug.Log(string.Format("Registered voxel world {0}[{1}]", world.name, world.Id), world);
			if (!VoxelManager.HasAuthority)
			{
				VoxelManager.SendWorldStateRequest(world.Id);
			}
		}

		// Token: 0x0600794D RID: 31053 RVA: 0x0027E7A5 File Offset: 0x0027C9A5
		public static void Unregister(VoxelWorld world)
		{
			VoxelManager._worlds.Remove(world.Id);
			Debug.Log(string.Format("Unregistered voxel world {0}[{1}]", world.name, world.Id), world);
		}

		// Token: 0x0600794E RID: 31054 RVA: 0x0027E7DC File Offset: 0x0027C9DC
		public static void ReplicateState(VoxelWorld world)
		{
			if (!VoxelManager.InRoom || !VoxelManager.HasAuthority)
			{
				return;
			}
			foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
			{
				if (!netPlayer.IsLocal)
				{
					VoxelManager.SendWorldStateToPlayer(world, netPlayer);
				}
			}
		}

		// Token: 0x0600794F RID: 31055 RVA: 0x0027E848 File Offset: 0x0027CA48
		private void OnNetworkJoinedRoom()
		{
			if (!VoxelManager.HasAuthority)
			{
				VoxelManager.RequestVoxelWorldStates();
			}
		}

		// Token: 0x06007950 RID: 31056 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void OnNetworkLeftRoom()
		{
		}

		// Token: 0x06007951 RID: 31057 RVA: 0x0027E856 File Offset: 0x0027CA56
		private void OnPlayerLeft(NetPlayer player)
		{
			if (!VoxelManager.HasAuthority || !VoxelManager.InRoom)
			{
				return;
			}
			VoxelManager.ClearQueueForPlayer(player);
		}

		// Token: 0x06007952 RID: 31058 RVA: 0x0027E86D File Offset: 0x0027CA6D
		protected override void OnOwnerSwitched(NetPlayer newOwningPlayer)
		{
			this._owner = newOwningPlayer;
			if (!VoxelManager.HasAuthority)
			{
				VoxelManager.RequestVoxelWorldStates();
			}
		}

		// Token: 0x06007953 RID: 31059 RVA: 0x000028C5 File Offset: 0x00000AC5
		public override void WriteDataFusion()
		{
		}

		// Token: 0x06007954 RID: 31060 RVA: 0x000028C5 File Offset: 0x00000AC5
		public override void ReadDataFusion()
		{
		}

		// Token: 0x06007955 RID: 31061 RVA: 0x000028C5 File Offset: 0x00000AC5
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
		}

		// Token: 0x06007956 RID: 31062 RVA: 0x000028C5 File Offset: 0x00000AC5
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
		}

		// Token: 0x06007957 RID: 31063 RVA: 0x0027E884 File Offset: 0x0027CA84
		private static void RequestVoxelWorldStates()
		{
			foreach (int worldId in VoxelManager._worlds.Keys)
			{
				VoxelManager.SendWorldStateRequest(worldId);
			}
		}

		// Token: 0x06007958 RID: 31064 RVA: 0x0027E8D8 File Offset: 0x0027CAD8
		public static void RequestWorldState(VoxelWorld world)
		{
			VoxelManager.SendWorldStateRequest(world.Id);
		}

		// Token: 0x06007959 RID: 31065 RVA: 0x0027E8E8 File Offset: 0x0027CAE8
		private static void OnWorldStateRequestReceived(int worldId, PhotonMessageInfoWrapped info)
		{
			VoxelWorld world;
			if (VoxelManager._worlds.TryGetValue(worldId, out world))
			{
				VoxelManager.SendWorldStateToPlayer(world, info.Sender);
			}
		}

		// Token: 0x0600795A RID: 31066 RVA: 0x0027E910 File Offset: 0x0027CB10
		private static void SendWorldStateToPlayer(VoxelWorld world, NetPlayer player)
		{
			VoxelManager.StateInitQueue stateInitQueue;
			if (VoxelManager._initQueues.TryGetValue(player.ActorNumber, out stateInitQueue))
			{
				for (int i = 0; i < stateInitQueue.chunks.Count; i++)
				{
					if (stateInitQueue.chunks[i].worldId == world.Id)
					{
						stateInitQueue.chunks.RemoveAt(i--);
					}
				}
			}
			foreach (Chunk chunk in world.Chunks)
			{
				VoxelManager.QueueChunkForPlayer(chunk, player);
			}
		}

		// Token: 0x0600795B RID: 31067 RVA: 0x0027E9B0 File Offset: 0x0027CBB0
		private static bool WorldIsQueuedForPlayer(VoxelWorld world, NetPlayer player)
		{
			VoxelManager.StateInitQueue stateInitQueue;
			if (!VoxelManager._initQueues.TryGetValue(player.ActorNumber, out stateInitQueue))
			{
				return false;
			}
			using (List<VoxelManager.ChunkInitState>.Enumerator enumerator = stateInitQueue.chunks.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.worldId == world.Id)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600795C RID: 31068 RVA: 0x0027EA28 File Offset: 0x0027CC28
		private static void ClearQueueForPlayer(NetPlayer player)
		{
			VoxelManager.StateInitQueue stateInitQueue;
			if (VoxelManager._initQueues.TryGetValue(player.ActorNumber, out stateInitQueue))
			{
				stateInitQueue.chunks.Clear();
				stateInitQueue.operations.Clear();
			}
		}

		// Token: 0x0600795D RID: 31069 RVA: 0x0027EA60 File Offset: 0x0027CC60
		private static VoxelManager.StateInitQueue GetOrCreateQueueForPlayer(NetPlayer player)
		{
			VoxelManager.StateInitQueue stateInitQueue;
			if (!VoxelManager._initQueues.TryGetValue(player.ActorNumber, out stateInitQueue))
			{
				stateInitQueue = new VoxelManager.StateInitQueue(player);
				VoxelManager._initQueues[player.ActorNumber] = stateInitQueue;
			}
			return stateInitQueue;
		}

		// Token: 0x0600795E RID: 31070 RVA: 0x0027EA9C File Offset: 0x0027CC9C
		private static void QueueChunkForPlayer(Chunk chunk, NetPlayer player)
		{
			VoxelManager.StateInitQueue orCreateQueueForPlayer = VoxelManager.GetOrCreateQueueForPlayer(player);
			byte[] array = null;
			int totalSerializedBytes = 0;
			if (chunk.IsDataChanged)
			{
				ChunkDTO chunkDTO = new ChunkDTO(chunk);
				array = ChunkIO.SerializeChunk(chunkDTO);
				totalSerializedBytes = array.Length;
			}
			VoxelManager.ChunkInitState item = new VoxelManager.ChunkInitState
			{
				numSerializedBytes = 0,
				totalSerializedBytes = totalSerializedBytes,
				worldId = chunk.World.Id,
				chunkId = chunk.Id,
				hash = (chunk.World.Id ^ chunk.Id.GetHashCode()),
				serializedChunkState = array
			};
			orCreateQueueForPlayer.chunks.Add(item);
			VoxelManager._shouldProcessQueues = true;
		}

		// Token: 0x0600795F RID: 31071 RVA: 0x0027EB3C File Offset: 0x0027CD3C
		private static void QueueOperationForPlayer(VoxelWorld world, NetPlayer player, UnityEngine.BoundsInt bounds, byte[] data)
		{
			VoxelManager.StateInitQueue stateInitQueue;
			if (!VoxelManager._initQueues.TryGetValue(player.ActorNumber, out stateInitQueue))
			{
				stateInitQueue = new VoxelManager.StateInitQueue();
				VoxelManager._initQueues[player.ActorNumber] = stateInitQueue;
			}
			VoxelManager.VoxelOperationResult item = new VoxelManager.VoxelOperationResult
			{
				worldId = world.Id,
				bounds = bounds,
				data = data
			};
			stateInitQueue.operations.Add(item);
			VoxelManager._shouldProcessQueues = true;
		}

		// Token: 0x06007960 RID: 31072 RVA: 0x0027EBB0 File Offset: 0x0027CDB0
		private static void SendNextChunk(VoxelManager.StateInitQueue queue)
		{
			NetPlayer player = queue.player;
			if (queue.currentChunk != null)
			{
				VoxelManager.ChunkInitState currentChunk = queue.currentChunk;
				VoxelManager.SendNextPacketForChunk(currentChunk, player);
				if (currentChunk.numSerializedBytes == currentChunk.totalSerializedBytes)
				{
					queue.currentChunk = null;
					return;
				}
			}
			else
			{
				if (queue.chunks.Count > 0)
				{
					VoxelManager.ChunkInitState chunkInitState = queue.chunks[0];
					VoxelManager.SendStartChunk(player, chunkInitState.worldId, chunkInitState.chunkId, chunkInitState.hash, chunkInitState.totalSerializedBytes);
					if (chunkInitState.totalSerializedBytes > 0)
					{
						queue.currentChunk = chunkInitState;
					}
					queue.chunks.RemoveAt(0);
					return;
				}
				if (queue.operations.Count > 0)
				{
					VoxelManager.VoxelOperationResult voxelOperationResult = queue.operations[0];
					VoxelManager.SendSetDensity(player, voxelOperationResult.worldId, voxelOperationResult.bounds, voxelOperationResult.data);
					queue.operations.RemoveAt(0);
				}
			}
		}

		// Token: 0x06007961 RID: 31073 RVA: 0x0027EC88 File Offset: 0x0027CE88
		private static void SendNextPacketForChunk(VoxelManager.ChunkInitState chunkState, NetPlayer player)
		{
			int numSerializedBytes = chunkState.numSerializedBytes;
			int num = Mathf.Min(chunkState.totalSerializedBytes - numSerializedBytes, 1000);
			if (num <= 0)
			{
				return;
			}
			Array.Copy(chunkState.serializedChunkState, numSerializedBytes, VoxelManager._packetData, 0, num);
			chunkState.numSerializedBytes += num;
			VoxelManager.SendContinueChunk(player, chunkState.hash, num, VoxelManager._packetData);
			VoxelManager.EnqueueTransferLog(num);
			int numSerializedBytes2 = chunkState.numSerializedBytes;
			int totalSerializedBytes = chunkState.totalSerializedBytes;
		}

		// Token: 0x06007962 RID: 31074 RVA: 0x0027ECFC File Offset: 0x0027CEFC
		private static void OnStartChunkReceived(int worldId, int3 chunkId, int hash, int size)
		{
			int chunkIndex = VoxelManager._localInitQueue.GetChunkIndex(hash);
			if (chunkIndex >= 0)
			{
				VoxelManager._localInitQueue.chunks.RemoveAt(chunkIndex);
			}
			VoxelWorld voxelWorld;
			if (!VoxelManager._worlds.TryGetValue(worldId, out voxelWorld))
			{
				Debug.LogError(string.Format("Failed to find world {0}", worldId));
				return;
			}
			Chunk chunk;
			if (!voxelWorld.TryGetChunk(chunkId, out chunk))
			{
				Debug.LogError(string.Format("Tried to receive a non-loaded chunk {0}", chunkId));
				return;
			}
			if (size > 0)
			{
				VoxelManager.ChunkInitState item = new VoxelManager.ChunkInitState
				{
					worldId = worldId,
					hash = hash,
					serializedChunkState = new byte[size],
					numSerializedBytes = 0,
					totalSerializedBytes = size
				};
				VoxelManager._localInitQueue.chunks.Add(item);
				return;
			}
			voxelWorld.ResetChunk(chunkId);
		}

		// Token: 0x06007963 RID: 31075 RVA: 0x0027EDBC File Offset: 0x0027CFBC
		private static void OnChunkPacketReceived(int hash, int size, byte[] data)
		{
			if (size > data.Length)
			{
				Debug.LogError("Size value is is larger than data");
				return;
			}
			int chunkIndex = VoxelManager._localInitQueue.GetChunkIndex(hash);
			if (chunkIndex < 0)
			{
				Debug.LogError(string.Format("Couldn't fetch chunk state with hash {0}", hash));
				return;
			}
			VoxelManager.ChunkInitState chunkInitState = VoxelManager._localInitQueue.chunks[chunkIndex];
			if (chunkInitState.numSerializedBytes + size > chunkInitState.totalSerializedBytes)
			{
				Debug.LogError(string.Format("Received data larger than {0} bytes for chunk {1}", chunkInitState.totalSerializedBytes, hash));
				return;
			}
			Array.Copy(data, 0, chunkInitState.serializedChunkState, chunkInitState.numSerializedBytes, size);
			chunkInitState.numSerializedBytes += size;
			if (chunkInitState.numSerializedBytes == chunkInitState.totalSerializedBytes)
			{
				ChunkDTO chunkDTO;
				if (ChunkIO.TryDeserializeChunk(chunkInitState.serializedChunkState, out chunkDTO))
				{
					VoxelWorld voxelWorld;
					if (VoxelManager._worlds.TryGetValue(chunkDTO.WorldId, out voxelWorld))
					{
						voxelWorld.UpdateChunkFrom(chunkDTO);
					}
					else
					{
						Debug.LogError(string.Format("Deserialized chunk for nonexistent world {0}", chunkDTO.WorldId));
					}
				}
				else
				{
					Debug.LogError(string.Format("Unable to deserialize chunk with hash {0}", chunkInitState.hash));
				}
				VoxelManager._localInitQueue.chunks.RemoveAt(chunkIndex);
			}
		}

		// Token: 0x06007964 RID: 31076 RVA: 0x0027EEE4 File Offset: 0x0027D0E4
		private static void SendDensity(VoxelWorld world, UnityEngine.BoundsInt bounds)
		{
			byte[] source;
			VoxelManager.GetDensityForBounds(world, bounds, out source);
			byte[] data = LZ4Pickler.Pickle(source, LZ4Level.L00_FAST);
			foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
			{
				if (!netPlayer.IsLocal)
				{
					if (VoxelManager.WorldIsQueuedForPlayer(world, netPlayer))
					{
						VoxelManager.QueueOperationForPlayer(world, netPlayer, bounds, data);
					}
					else
					{
						VoxelManager.SendSetDensity(netPlayer, world.Id, bounds, data);
					}
				}
			}
		}

		// Token: 0x06007965 RID: 31077 RVA: 0x0027EF6C File Offset: 0x0027D16C
		public static void PerformOperation(VoxelWorld world, Vector3 position, VoxelAction action)
		{
			position = world.GetLocalPosition(position);
			action.radius /= world.Scale;
			if (!VoxelManager.InRoom)
			{
				world.PerformLocalOperation(position, action);
				return;
			}
			if (VoxelManager.HasAuthority)
			{
				VoxelManager.OperateAuthority(world, position, action);
				return;
			}
			world.PerformLocalOperation(position, action);
			VoxelManager.SendOperationRequest(world.Id, position, action);
		}

		// Token: 0x06007966 RID: 31078 RVA: 0x0027EFC8 File Offset: 0x0027D1C8
		private static void OperateAuthority(VoxelWorld world, Vector3 localPosition, VoxelAction action)
		{
			world.PerformLocalOperation(localPosition, action);
			UnityEngine.BoundsInt bounds = world.GetBounds(localPosition, action.radius);
			VoxelManager.SendDensity(world, bounds);
		}

		// Token: 0x06007967 RID: 31079 RVA: 0x0027EFF8 File Offset: 0x0027D1F8
		private static void OnOperationRequestReceived(int worldId, Vector3 localPosition, VoxelAction action, PhotonMessageInfoWrapped info)
		{
			VoxelWorld world;
			if (!VoxelManager._worlds.TryGetValue(worldId, out world))
			{
				Debug.LogError(string.Format("Couldn't find voxel world {0}", worldId));
				return;
			}
			UnityEngine.BoundsInt bounds = world.GetBounds(localPosition, action.radius);
			if (bounds.GetVoxelCount() > 1000)
			{
				GTDev.LogError<string>(string.Format("Received voxel operation request was too large [{0} = {1} voxels]", bounds, bounds.GetVoxelCount()), null);
				return;
			}
			VoxelManager.OperateAuthority(world, localPosition, action);
		}

		// Token: 0x06007968 RID: 31080 RVA: 0x0027F074 File Offset: 0x0027D274
		public static void Mine(VoxelWorld world, UnityEngine.BoundsInt bounds, Vector3 hitPoint, Vector3 hitNormal, Vector3 origin, VoxelAction action)
		{
			if (!VoxelManager.InRoom)
			{
				world.PerformLocalMiningOperation(bounds, hitPoint, hitNormal, origin, action);
				return;
			}
			if (VoxelManager.HasAuthority)
			{
				VoxelManager.MineAuthority(world, bounds, hitPoint, hitNormal, origin, action, null);
				return;
			}
			world.PerformLocalMiningOperation(bounds, hitPoint, hitNormal, origin, action);
			VoxelManager.SendMineOperationRequest(world.Id, bounds, hitPoint, hitNormal, origin, action);
		}

		// Token: 0x06007969 RID: 31081 RVA: 0x0027F0D0 File Offset: 0x0027D2D0
		private static void MineAuthority(VoxelWorld world, UnityEngine.BoundsInt bounds, Vector3 hitPoint, Vector3 hitNormal, Vector3 origin, VoxelAction action, NetPlayer sender = null)
		{
			if (sender == null)
			{
				sender = NetworkSystem.Instance.LocalPlayer;
			}
			ValueTuple<int, int> valueTuple = world.PerformLocalMiningOperation(bounds, hitPoint, hitNormal, origin, action);
			int item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			if (item > 0 || item2 > 0)
			{
				foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
				{
					if (!netPlayer.IsLocal && netPlayer != sender)
					{
						VoxelManager.SendPlayDigFX(netPlayer, hitPoint, hitNormal, item, item2);
					}
				}
			}
			VoxelManager.SendDensity(world, bounds);
		}

		// Token: 0x0600796A RID: 31082 RVA: 0x0027F16C File Offset: 0x0027D36C
		private static void OnPlayDigFXReceived(Vector3 hitPoint, Vector3 hitNormal, int dirtMined, int stoneMined)
		{
			SingletonMonoBehaviour<VoxelActions>.instance.PlayDigFX(hitPoint, hitNormal, dirtMined, stoneMined);
		}

		// Token: 0x0600796B RID: 31083 RVA: 0x0027F17C File Offset: 0x0027D37C
		private static void OnMineRequestReceived(int worldId, UnityEngine.BoundsInt bounds, Vector3 hitPoint, Vector3 hitNormal, Vector3 origin, VoxelAction action, PhotonMessageInfoWrapped info)
		{
			VoxelWorld world;
			if (!VoxelManager._worlds.TryGetValue(worldId, out world))
			{
				Debug.LogError(string.Format("Couldn't find voxel world {0}", worldId));
				return;
			}
			VoxelManager.MineAuthority(world, bounds, hitPoint, hitNormal, origin, action, info.Sender);
		}

		// Token: 0x0600796C RID: 31084 RVA: 0x0027F1C4 File Offset: 0x0027D3C4
		private static void GetVoxelsForBounds(VoxelWorld world, UnityEngine.BoundsInt bounds, out Voxel[] voxels)
		{
			int voxelCount = bounds.GetVoxelCount();
			voxels = new Voxel[voxelCount];
			int num = 0;
			for (int i = bounds.min.x; i <= bounds.max.x; i++)
			{
				for (int j = bounds.min.y; j <= bounds.max.y; j++)
				{
					for (int k = bounds.min.z; k <= bounds.max.z; k++)
					{
						voxels[num++] = world.GetVoxelData(new int3(i, j, k));
					}
				}
			}
		}

		// Token: 0x0600796D RID: 31085 RVA: 0x0027F284 File Offset: 0x0027D484
		private static void GetDensityForBounds(VoxelWorld world, UnityEngine.BoundsInt bounds, out byte[] voxels)
		{
			int voxelCount = bounds.GetVoxelCount();
			voxels = VoxelManager._arrayBag.GetStaticArray(voxelCount);
			int num = 0;
			for (int i = bounds.min.x; i <= bounds.max.x; i++)
			{
				for (int j = bounds.min.y; j <= bounds.max.y; j++)
				{
					for (int k = bounds.min.z; k <= bounds.max.z; k++)
					{
						voxels[num++] = world.GetVoxelDensity(new int3(i, j, k));
					}
				}
			}
		}

		// Token: 0x0600796E RID: 31086 RVA: 0x0027F344 File Offset: 0x0027D544
		private static void OnSetDensityReceived(int worldId, UnityEngine.BoundsInt bounds, byte[] data)
		{
			VoxelWorld voxelWorld;
			if (!VoxelManager._worlds.TryGetValue(worldId, out voxelWorld))
			{
				throw new InvalidOperationException(string.Format("Couldn't find voxel world {0}", worldId));
			}
			byte[] array = LZ4Pickler.Unpickle(data);
			if (bounds.GetVoxelCount() != array.Length)
			{
				Debug.LogError(string.Format("Voxel count mismatch: {0} vs {1}", bounds.GetVoxelCount(), array.Length));
				return;
			}
			voxelWorld.SetVoxelDensity(bounds, array, false);
		}

		// Token: 0x0600796F RID: 31087 RVA: 0x0027F3B4 File Offset: 0x0027D5B4
		internal static bool IsValidAuthorityRPC(PhotonMessageInfoWrapped info, VoxelManager.RPC eventType)
		{
			return ((VoxelManager.HasAuthority && VoxelManager.InRoom) || info.Sender.IsLocal) && !VoxelManager.IsSpamming(info, eventType);
		}

		// Token: 0x06007970 RID: 31088 RVA: 0x0027F3DD File Offset: 0x0027D5DD
		internal static bool IsValidClientRPC(PhotonMessageInfoWrapped info, VoxelManager.RPC eventType)
		{
			return (info.Sender.IsMasterClient || info.Sender.IsLocal) && !VoxelManager.IsSpamming(info, eventType);
		}

		// Token: 0x06007971 RID: 31089 RVA: 0x0027F405 File Offset: 0x0027D605
		internal static bool IsSpamming(PhotonMessageInfoWrapped info, VoxelManager.RPC eventType)
		{
			return !VoxelManager.GetSpamChecksForUser(info.senderID)[(int)eventType].CheckCallTime(Time.unscaledTime);
		}

		// Token: 0x06007972 RID: 31090 RVA: 0x0027F424 File Offset: 0x0027D624
		internal static CallLimiter[] GetSpamChecksForUser(int userID)
		{
			CallLimiter[] array;
			if (!VoxelManager._spamChecks.TryGetValue(userID, out array))
			{
				array = new CallLimiter[]
				{
					new CallLimiter(10, 30f, 0.5f),
					new CallLimiter(10, 1f, 0.5f),
					new CallLimiter(10, 1f, 0.5f),
					new CallLimiter(10, 1f, 0.5f),
					new CallLimiter(100, 1f, 0.5f),
					new CallLimiter(20, 1f, 0.5f),
					new CallLimiter(50, 1f, 0.5f),
					new CallLimiter(50, 1f, 0.5f)
				};
				VoxelManager._spamChecks[userID] = array;
			}
			return array;
		}

		// Token: 0x06007973 RID: 31091 RVA: 0x0027F4F8 File Offset: 0x0027D6F8
		internal static void RegisterNetEventCallbacks()
		{
			RoomSystem.netEventCallbacks[100] = new Action<object[], PhotonMessageInfoWrapped>(VoxelManager.DeserializeWorldStateRequest);
			RoomSystem.netEventCallbacks[101] = new Action<object[], PhotonMessageInfoWrapped>(VoxelManager.DeserializeOperationRequest);
			RoomSystem.netEventCallbacks[102] = new Action<object[], PhotonMessageInfoWrapped>(VoxelManager.DeserializeMineOperationRequest);
			RoomSystem.netEventCallbacks[103] = new Action<object[], PhotonMessageInfoWrapped>(VoxelManager.DeserializeStartChunk);
			RoomSystem.netEventCallbacks[104] = new Action<object[], PhotonMessageInfoWrapped>(VoxelManager.DeserializeContinueChunk);
			RoomSystem.netEventCallbacks[105] = new Action<object[], PhotonMessageInfoWrapped>(VoxelManager.DeserializeSetDensity);
			RoomSystem.netEventCallbacks[106] = new Action<object[], PhotonMessageInfoWrapped>(VoxelManager.DeserializePlayDigFX);
		}

		// Token: 0x06007974 RID: 31092 RVA: 0x0027F5AD File Offset: 0x0027D7AD
		private static void SendWorldStateRequest(int worldId)
		{
			RoomSystem.SendEvent(100, new object[]
			{
				worldId
			}, NetworkSystemRaiseEvent.neoMaster, true);
		}

		// Token: 0x06007975 RID: 31093 RVA: 0x0027F5CC File Offset: 0x0027D7CC
		internal static void DeserializeWorldStateRequest(object[] eventData, PhotonMessageInfoWrapped info)
		{
			MonkeAgent.IncrementRPCCall(info, "DeserializeWorldStateRequest");
			if (!VoxelManager.IsValidAuthorityRPC(info, VoxelManager.RPC.WorldRequest))
			{
				return;
			}
			int worldId;
			if (!eventData.TryDeserializeTo(out worldId))
			{
				return;
			}
			VoxelManager.OnWorldStateRequestReceived(worldId, info);
		}

		// Token: 0x06007976 RID: 31094 RVA: 0x0027F600 File Offset: 0x0027D800
		private static void SendOperationRequest(int worldId, Vector3 localPosition, VoxelAction action)
		{
			object[] evData = new object[]
			{
				worldId,
				localPosition,
				action
			};
			RoomSystem.SendEvent(101, evData, NetworkSystemRaiseEvent.neoMaster, true);
		}

		// Token: 0x06007977 RID: 31095 RVA: 0x0027F640 File Offset: 0x0027D840
		private static void DeserializeOperationRequest(object[] eventData, PhotonMessageInfoWrapped info)
		{
			MonkeAgent.IncrementRPCCall(info, "DeserializeOperationRequest");
			if (!VoxelManager.IsValidAuthorityRPC(info, VoxelManager.RPC.OperationRequest))
			{
				return;
			}
			int worldId;
			Vector3 localPosition;
			VoxelAction voxelAction;
			if (!eventData.TryDeserializeTo(out worldId, out localPosition, out voxelAction))
			{
				return;
			}
			float num = 10000f;
			if (!localPosition.IsValid(num) || !voxelAction.IsValid() || voxelAction.radius > 5f)
			{
				return;
			}
			VoxelManager.OnOperationRequestReceived(worldId, localPosition, voxelAction, info);
		}

		// Token: 0x06007978 RID: 31096 RVA: 0x0027F6A4 File Offset: 0x0027D8A4
		private static void SendMineOperationRequest(int worldId, UnityEngine.BoundsInt bounds, Vector3 hitPoint, Vector3 hitNormal, Vector3 origin, VoxelAction action)
		{
			object[] evData = new object[]
			{
				worldId,
				bounds,
				hitPoint,
				hitNormal,
				origin,
				action
			};
			RoomSystem.SendEvent(102, evData, NetworkSystemRaiseEvent.neoMaster, true);
		}

		// Token: 0x06007979 RID: 31097 RVA: 0x0027F700 File Offset: 0x0027D900
		private static void DeserializeMineOperationRequest(object[] eventData, PhotonMessageInfoWrapped info)
		{
			MonkeAgent.IncrementRPCCall(info, "DeserializeMineOperationRequest");
			if (!VoxelManager.IsValidAuthorityRPC(info, VoxelManager.RPC.MineRequest))
			{
				return;
			}
			int worldId;
			UnityEngine.BoundsInt bounds;
			Vector3 hitPoint;
			Vector3 hitNormal;
			Vector3 origin;
			VoxelAction voxelAction;
			if (!eventData.TryDeserializeTo(out worldId, out bounds, out hitPoint, out hitNormal, out origin, out voxelAction))
			{
				return;
			}
			float num = 10000f;
			if (hitPoint.IsValid(num) && Mathf.Approximately(hitNormal.sqrMagnitude, 1f))
			{
				float num2 = 10000f;
				if (origin.IsValid(num2) && voxelAction.IsValid() && voxelAction.radius <= 5f)
				{
					VoxelManager.OnMineRequestReceived(worldId, bounds, hitPoint, hitNormal, origin, voxelAction, info);
					return;
				}
			}
		}

		// Token: 0x0600797A RID: 31098 RVA: 0x0027F794 File Offset: 0x0027D994
		private static void SendStartChunk(NetPlayer player, int worldId, int3 chunkId, int hash, int totalSerializedBytes)
		{
			object[] evData = new object[]
			{
				worldId,
				chunkId,
				hash,
				totalSerializedBytes
			};
			RoomSystem.SendEvent(103, evData, player, true);
		}

		// Token: 0x0600797B RID: 31099 RVA: 0x0027F7D8 File Offset: 0x0027D9D8
		private static void DeserializeStartChunk(object[] eventData, PhotonMessageInfoWrapped info)
		{
			int worldId;
			int3 chunkId;
			int hash;
			int num;
			if (!eventData.TryDeserializeTo(out worldId, out chunkId, out hash, out num))
			{
				return;
			}
			if (num > 0)
			{
				MonkeAgent.IncrementRPCCall(info, "DeserializeStartChunk");
			}
			if (!VoxelManager.IsValidClientRPC(info, (num > 0) ? VoxelManager.RPC.StartChunk : VoxelManager.RPC.StartEmptyChunk))
			{
				return;
			}
			VoxelManager.OnStartChunkReceived(worldId, chunkId, hash, num);
		}

		// Token: 0x0600797C RID: 31100 RVA: 0x0027F820 File Offset: 0x0027DA20
		private static void SendContinueChunk(NetPlayer player, int hash, int size, byte[] data)
		{
			if (data.Length > 1000)
			{
				Debug.LogError(string.Format("Attempted to send ContinueChunk() with too many bytes ({0})", data.Length));
				return;
			}
			object[] evData = new object[]
			{
				hash,
				size,
				data
			};
			RoomSystem.SendEvent(104, evData, player, true);
		}

		// Token: 0x0600797D RID: 31101 RVA: 0x0027F878 File Offset: 0x0027DA78
		private static void DeserializeContinueChunk(object[] eventData, PhotonMessageInfoWrapped info)
		{
			MonkeAgent.IncrementRPCCall(info, "DeserializeContinueChunk");
			if (!VoxelManager.IsValidClientRPC(info, VoxelManager.RPC.ContinueChunk))
			{
				return;
			}
			int hash;
			int num;
			byte[] array;
			if (!eventData.TryDeserializeTo(out hash, out num, out array))
			{
				return;
			}
			if (num < 1 || num > 1000 || num > array.Length)
			{
				return;
			}
			VoxelManager.OnChunkPacketReceived(hash, num, array);
		}

		// Token: 0x0600797E RID: 31102 RVA: 0x0027F8C4 File Offset: 0x0027DAC4
		private static void SendSetDensity(NetPlayer player, int worldId, UnityEngine.BoundsInt bounds, byte[] data)
		{
			if (data.Length > 1000)
			{
				Debug.LogError(string.Format("Attempted to send SetDensity() with too many bytes ({0})", data.Length));
				return;
			}
			object[] evData = new object[]
			{
				worldId,
				bounds,
				data
			};
			RoomSystem.SendEvent(105, evData, player, true);
			VoxelManager.EnqueueTransferLog(data.Length);
		}

		// Token: 0x0600797F RID: 31103 RVA: 0x0027F924 File Offset: 0x0027DB24
		private static void DeserializeSetDensity(object[] eventData, PhotonMessageInfoWrapped info)
		{
			MonkeAgent.IncrementRPCCall(info, "DeserializeSetDensity");
			if (!VoxelManager.IsValidClientRPC(info, VoxelManager.RPC.SetDensity))
			{
				return;
			}
			int worldId;
			UnityEngine.BoundsInt bounds;
			byte[] array;
			if (!eventData.TryDeserializeTo(out worldId, out bounds, out array))
			{
				return;
			}
			if (array.Length > 1000)
			{
				return;
			}
			VoxelManager.OnSetDensityReceived(worldId, bounds, array);
		}

		// Token: 0x06007980 RID: 31104 RVA: 0x0027F968 File Offset: 0x0027DB68
		private static void SendPlayDigFX(NetPlayer player, Vector3 hitPoint, Vector3 hitNormal, int dirt, int stone)
		{
			object[] evData = new object[]
			{
				hitPoint,
				hitNormal,
				dirt,
				stone
			};
			float num = 10000f;
			if (!hitPoint.IsValid(num) || !Mathf.Approximately(hitNormal.sqrMagnitude, 1f))
			{
				return;
			}
			RoomSystem.SendEvent(106, evData, player, true);
		}

		// Token: 0x06007981 RID: 31105 RVA: 0x0027F9D4 File Offset: 0x0027DBD4
		private static void DeserializePlayDigFX(object[] eventData, PhotonMessageInfoWrapped info)
		{
			MonkeAgent.IncrementRPCCall(info, "DeserializePlayDigFX");
			if (!VoxelManager.IsValidClientRPC(info, VoxelManager.RPC.PLayFX))
			{
				return;
			}
			Vector3 hitPoint;
			Vector3 hitNormal;
			int dirtMined;
			int stoneMined;
			if (!eventData.TryDeserializeTo(out hitPoint, out hitNormal, out dirtMined, out stoneMined))
			{
				return;
			}
			float num = 10000f;
			if (!hitPoint.IsValid(num) || !Mathf.Approximately(hitNormal.sqrMagnitude, 1f))
			{
				return;
			}
			VoxelManager.OnPlayDigFXReceived(hitPoint, hitNormal, dirtMined, stoneMined);
		}

		// Token: 0x06007984 RID: 31108 RVA: 0x00002B07 File Offset: 0x00000D07
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
		}

		// Token: 0x06007985 RID: 31109 RVA: 0x00002B13 File Offset: 0x00000D13
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
		}

		// Token: 0x04008C2A RID: 35882
		private const int MAX_DATA_SIZE = 1000;

		// Token: 0x04008C2B RID: 35883
		private static VoxelManager _instance;

		// Token: 0x04008C2C RID: 35884
		private static Dictionary<int, VoxelWorld> _worlds = new Dictionary<int, VoxelWorld>();

		// Token: 0x04008C2D RID: 35885
		private static StaticArrayBag<byte> _arrayBag = new StaticArrayBag<byte>();

		// Token: 0x04008C2E RID: 35886
		private NetPlayer _owner;

		// Token: 0x04008C2F RID: 35887
		private static Dictionary<int, VoxelManager.StateInitQueue> _initQueues = new Dictionary<int, VoxelManager.StateInitQueue>();

		// Token: 0x04008C30 RID: 35888
		private static VoxelManager.StateInitQueue _localInitQueue = new VoxelManager.StateInitQueue();

		// Token: 0x04008C31 RID: 35889
		private static byte[] _packetData = new byte[1000];

		// Token: 0x04008C32 RID: 35890
		private static bool _shouldProcessQueues;

		// Token: 0x04008C33 RID: 35891
		[TupleElementNames(new string[]
		{
			"time",
			"bytes"
		})]
		private static Queue<ValueTuple<float, int>> _sendHistory = new Queue<ValueTuple<float, int>>();

		// Token: 0x04008C34 RID: 35892
		private static int _sendRate;

		// Token: 0x04008C35 RID: 35893
		private const int MAX_DATA_RATE = 10000;

		// Token: 0x04008C36 RID: 35894
		private static Dictionary<int, CallLimiter[]> _spamChecks = new Dictionary<int, CallLimiter[]>();

		// Token: 0x020012FC RID: 4860
		public class StateInitQueue
		{
			// Token: 0x17000BAD RID: 2989
			// (get) Token: 0x06007986 RID: 31110 RVA: 0x0027FA90 File Offset: 0x0027DC90
			public bool IsEmpty
			{
				get
				{
					return this.currentChunk == null && this.chunks.Count == 0 && this.operations.Count == 0;
				}
			}

			// Token: 0x06007987 RID: 31111 RVA: 0x0027FAB7 File Offset: 0x0027DCB7
			public StateInitQueue()
			{
			}

			// Token: 0x06007988 RID: 31112 RVA: 0x0027FAD5 File Offset: 0x0027DCD5
			public StateInitQueue(NetPlayer player)
			{
				this.player = player;
			}

			// Token: 0x06007989 RID: 31113 RVA: 0x0027FAFC File Offset: 0x0027DCFC
			public int GetChunkIndex(int hash)
			{
				for (int i = 0; i < this.chunks.Count; i++)
				{
					if (this.chunks[i].hash == hash)
					{
						return i;
					}
				}
				return -1;
			}

			// Token: 0x0600798A RID: 31114 RVA: 0x0027FB38 File Offset: 0x0027DD38
			public VoxelManager.ChunkInitState GetChunkState(int hash)
			{
				foreach (VoxelManager.ChunkInitState chunkInitState in this.chunks)
				{
					if (chunkInitState.hash == hash)
					{
						return chunkInitState;
					}
				}
				return null;
			}

			// Token: 0x04008C37 RID: 35895
			public NetPlayer player;

			// Token: 0x04008C38 RID: 35896
			public List<VoxelManager.ChunkInitState> chunks = new List<VoxelManager.ChunkInitState>();

			// Token: 0x04008C39 RID: 35897
			public List<VoxelManager.VoxelOperationResult> operations = new List<VoxelManager.VoxelOperationResult>();

			// Token: 0x04008C3A RID: 35898
			public VoxelManager.ChunkInitState currentChunk;
		}

		// Token: 0x020012FD RID: 4861
		public struct VoxelOperationResult
		{
			// Token: 0x04008C3B RID: 35899
			public int worldId;

			// Token: 0x04008C3C RID: 35900
			public UnityEngine.BoundsInt bounds;

			// Token: 0x04008C3D RID: 35901
			public byte[] data;
		}

		// Token: 0x020012FE RID: 4862
		public class ChunkInitState
		{
			// Token: 0x04008C3E RID: 35902
			public int worldId;

			// Token: 0x04008C3F RID: 35903
			public int3 chunkId;

			// Token: 0x04008C40 RID: 35904
			public int hash;

			// Token: 0x04008C41 RID: 35905
			public byte[] serializedChunkState;

			// Token: 0x04008C42 RID: 35906
			public int numSerializedBytes;

			// Token: 0x04008C43 RID: 35907
			public int totalSerializedBytes;
		}

		// Token: 0x020012FF RID: 4863
		public enum RPC
		{
			// Token: 0x04008C45 RID: 35909
			WorldRequest,
			// Token: 0x04008C46 RID: 35910
			OperationRequest,
			// Token: 0x04008C47 RID: 35911
			MineRequest,
			// Token: 0x04008C48 RID: 35912
			StartChunk,
			// Token: 0x04008C49 RID: 35913
			StartEmptyChunk,
			// Token: 0x04008C4A RID: 35914
			ContinueChunk,
			// Token: 0x04008C4B RID: 35915
			SetDensity,
			// Token: 0x04008C4C RID: 35916
			PLayFX,
			// Token: 0x04008C4D RID: 35917
			Count
		}
	}
}
