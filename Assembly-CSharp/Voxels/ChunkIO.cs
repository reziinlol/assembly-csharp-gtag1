using System;
using System.IO;
using K4os.Compression.LZ4;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Voxels
{
	// Token: 0x020012C9 RID: 4809
	public static class ChunkIO
	{
		// Token: 0x06007841 RID: 30785 RVA: 0x0027790E File Offset: 0x00275B0E
		public static string PathFor(int3 id)
		{
			return Path.Combine(ChunkIO.Root, string.Format("{0}_{1}_{2}.vox", id.x, id.y, id.z));
		}

		// Token: 0x06007842 RID: 30786 RVA: 0x00277948 File Offset: 0x00275B48
		public static void SaveChunk(ChunkDTO dto)
		{
			string text = ChunkIO.PathFor(dto.Id);
			Debug.Log(string.Format("Saving chunk {0} to {1}", dto.Id, text));
			ChunkIO.Save(text, dto);
		}

		// Token: 0x06007843 RID: 30787 RVA: 0x00277984 File Offset: 0x00275B84
		public static bool TryLoadChunk(int3 id, out ChunkDTO dto)
		{
			string arg = ChunkIO.PathFor(id);
			if (!File.Exists(ChunkIO.PathFor(id)))
			{
				dto = default(ChunkDTO);
				return false;
			}
			dto = ChunkIO.Load(ChunkIO.PathFor(id), Allocator.Persistent);
			if (dto.IsValid)
			{
				Debug.Log(string.Format("Loaded chunk {0} from {1}", id, arg));
			}
			else
			{
				Debug.Log(string.Format("Chunk {0} at {1} magic or version mismatch.", id, arg));
			}
			return dto.IsValid;
		}

		// Token: 0x06007844 RID: 30788 RVA: 0x002779FC File Offset: 0x00275BFC
		public static void Save(string path, in ChunkDTO chunk)
		{
			if (!Directory.Exists(ChunkIO.Root))
			{
				Directory.CreateDirectory(ChunkIO.Root);
			}
			using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, false))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
				{
					ChunkIO.WriteChunk(binaryWriter, chunk);
				}
			}
		}

		// Token: 0x06007845 RID: 30789 RVA: 0x00277A70 File Offset: 0x00275C70
		public static byte[] SerializeChunk(in ChunkDTO chunk)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(4096))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					ChunkIO.WriteChunk(binaryWriter, chunk);
					binaryWriter.Flush();
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		// Token: 0x06007846 RID: 30790 RVA: 0x00277AD8 File Offset: 0x00275CD8
		private static void WriteChunk(BinaryWriter bw, in ChunkDTO chunk)
		{
			bw.Write(1448040524U);
			bw.Write(5);
			bw.Write(chunk.WorldId);
			bw.Write(chunk.Id.x);
			bw.Write(chunk.Id.y);
			bw.Write(chunk.Id.z);
			bw.Write(chunk.Size.x);
			bw.Write(chunk.Size.y);
			bw.Write(chunk.Size.z);
			bw.Write(chunk.Dimensions.x);
			bw.Write(chunk.Dimensions.y);
			bw.Write(chunk.Dimensions.z);
			ChunkIO.WriteNativeArray(bw, chunk.Density);
			ChunkIO.WriteNativeArray(bw, chunk.Material);
		}

		// Token: 0x06007847 RID: 30791 RVA: 0x00277BB4 File Offset: 0x00275DB4
		public static ChunkDTO Load(string path, Allocator alloc = Allocator.Persistent)
		{
			ChunkDTO result;
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, false))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					result = ChunkIO.ReadChunk(binaryReader, alloc);
				}
			}
			return result;
		}

		// Token: 0x06007848 RID: 30792 RVA: 0x00277C14 File Offset: 0x00275E14
		public static bool TryDeserializeChunk(in byte[] data, out ChunkDTO dto)
		{
			dto = ChunkIO.DeserializeChunk(data, Allocator.Persistent);
			return dto.IsValid;
		}

		// Token: 0x06007849 RID: 30793 RVA: 0x00277C2C File Offset: 0x00275E2C
		public static ChunkDTO DeserializeChunk(in byte[] data, Allocator alloc = Allocator.Persistent)
		{
			ChunkDTO result;
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					result = ChunkIO.ReadChunk(binaryReader, Allocator.Persistent);
				}
			}
			return result;
		}

		// Token: 0x0600784A RID: 30794 RVA: 0x00277C84 File Offset: 0x00275E84
		private static ChunkDTO ReadChunk(BinaryReader br, Allocator alloc = Allocator.Persistent)
		{
			int num = (int)br.ReadUInt32();
			int num2 = br.ReadInt32();
			if (num != 1448040524 || num2 != 5)
			{
				return default(ChunkDTO);
			}
			int worldId = br.ReadInt32();
			int3 id = new int3(br.ReadInt32(), br.ReadInt32(), br.ReadInt32());
			int3 size = new int3(br.ReadInt32(), br.ReadInt32(), br.ReadInt32());
			int3 dimensions = new int3(br.ReadInt32(), br.ReadInt32(), br.ReadInt32());
			NativeArray<byte> density = ChunkIO.ReadNativeArray(br, alloc);
			NativeArray<byte> material = ChunkIO.ReadNativeArray(br, alloc);
			return new ChunkDTO
			{
				WorldId = worldId,
				Id = id,
				Size = size,
				Dimensions = dimensions,
				Density = density,
				Material = material
			};
		}

		// Token: 0x0600784B RID: 30795 RVA: 0x00277D58 File Offset: 0x00275F58
		private static void WriteNativeArray(BinaryWriter bw, NativeArray<byte> src)
		{
			byte[] array = LZ4Pickler.Pickle(src.ToArray(), LZ4Level.L00_FAST);
			bw.Write(array.Length);
			bw.Write(array);
		}

		// Token: 0x0600784C RID: 30796 RVA: 0x00277D84 File Offset: 0x00275F84
		private static NativeArray<byte> ReadNativeArray(BinaryReader br, Allocator alloc = Allocator.Persistent)
		{
			int count = br.ReadInt32();
			byte[] array = LZ4Pickler.Unpickle(br.ReadBytes(count));
			int length = array.Length;
			NativeArray<byte> nativeArray = new NativeArray<byte>(length, alloc, NativeArrayOptions.ClearMemory);
			NativeArray<byte>.Copy(array, nativeArray, length);
			return nativeArray;
		}

		// Token: 0x0600784D RID: 30797 RVA: 0x00277DBA File Offset: 0x00275FBA
		public static void DeleteWorld()
		{
			if (Directory.Exists(ChunkIO.Root))
			{
				Directory.Delete(ChunkIO.Root, true);
			}
			Debug.Log("All chunks deleted.");
		}

		// Token: 0x04008B4A RID: 35658
		public const uint MAGIC = 1448040524U;

		// Token: 0x04008B4B RID: 35659
		public const int VERSION = 5;

		// Token: 0x04008B4C RID: 35660
		private static readonly string Root = Path.Combine(Application.persistentDataPath, "WorldSaves");
	}
}
