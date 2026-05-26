using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x02000C5C RID: 3164
public class LuauScriptRunner
{
	// Token: 0x06004E29 RID: 20009 RVA: 0x0019F2F4 File Offset: 0x0019D4F4
	public unsafe static bool ErrorCheck(lua_State* L, int status)
	{
		if (status != 0)
		{
			sbyte* value = Luau.lua_tostring(L, -1);
			LuauHud.Instance.LuauLog(new string(value));
			sbyte* value2 = (sbyte*)Luau.lua_debugtrace(L);
			LuauHud.Instance.LuauLog(new string(value2));
			LuauHud.Instance.LuauLog("Error code: " + status.ToString());
			Luau.lua_close(L);
			return true;
		}
		return false;
	}

	// Token: 0x06004E2A RID: 20010 RVA: 0x0019F358 File Offset: 0x0019D558
	public bool Tick(float deltaTime)
	{
		if (!this.ShouldTick)
		{
			return false;
		}
		this.preTickCallback(this.L);
		LuauVm.ProcessEvents();
		if (!this.ShouldTick)
		{
			return false;
		}
		Luau.lua_settop(this.L, 0);
		Luau.lua_getfield(this.L, -10002, "tick");
		if (Luau.lua_type(this.L, -1) == 7)
		{
			Luau.lua_pushnumber(this.L, (double)deltaTime);
			int status = Luau.lua_pcall(this.L, 1, 0, 0);
			this.ShouldTick = !LuauScriptRunner.ErrorCheck(this.L, status);
			if (this.ShouldTick)
			{
				this.postTickCallback(this.L);
				Luau.lua_settop(this.L, 0);
				int data = Luau.lua_gc(this.L, 3, 0);
				Luau.lua_gc(this.L, 6, data);
			}
			return this.ShouldTick;
		}
		Luau.lua_pop(this.L, 1);
		return false;
	}

	// Token: 0x06004E2B RID: 20011 RVA: 0x0019F448 File Offset: 0x0019D648
	public unsafe LuauScriptRunner(string script, string name, [CanBeNull] lua_CFunction bindings = null, [CanBeNull] lua_CFunction preTick = null, [CanBeNull] lua_CFunction postTick = null)
	{
		this.Script = script;
		this.ScriptName = name;
		this.L = Luau.luaL_newstate();
		LuauScriptRunner.ScriptRunners.Add(this);
		Luau.luaL_openlibs(this.L);
		Bindings.Vec3Builder(this.L);
		Bindings.QuatBuilder(this.L);
		if (bindings != null)
		{
			bindings(this.L);
		}
		this.postTickCallback = postTick;
		this.preTickCallback = preTick;
		UIntPtr size = (UIntPtr)((IntPtr)0);
		Luau.lua_register(this.L, new lua_CFunction(Luau.lua_print), "print");
		byte[] bytes = Encoding.UTF8.GetBytes(script);
		sbyte* data = Luau.luau_compile(script, (UIntPtr)((IntPtr)bytes.Length), null, &size);
		Luau.luau_load(this.L, name, data, size, 0);
		int status = Luau.lua_resume(this.L, null, 0);
		this.ShouldTick = !LuauScriptRunner.ErrorCheck(this.L, status);
	}

	// Token: 0x06004E2C RID: 20012 RVA: 0x0019F52F File Offset: 0x0019D72F
	public LuauScriptRunner FromFile(string filePath, [CanBeNull] lua_CFunction bindings = null, [CanBeNull] lua_CFunction tick = null)
	{
		return new LuauScriptRunner(File.ReadAllText(Path.Join(Application.persistentDataPath, "Scripts", filePath)), filePath, bindings, tick, null);
	}

	// Token: 0x06004E2D RID: 20013 RVA: 0x0019F560 File Offset: 0x0019D760
	~LuauScriptRunner()
	{
		LuauVm.ClassBuilders.Clear();
		Bindings.LuauPlayerList.Clear();
		Bindings.LuauGameObjectList.Clear();
		Bindings.LuauGameObjectListReverse.Clear();
		Bindings.LuauGameObjectStates.Clear();
		Bindings.LuauVRRigList.Clear();
		Bindings.LuauAIAgentList.Clear();
		Bindings.Components.ComponentList.Clear();
		ReflectionMetaNames.ReflectedNames.Clear();
		if (BurstClassInfo.ClassList.InfoFields.Data.IsCreated)
		{
			BurstClassInfo.ClassList.InfoFields.Data.Clear();
		}
	}

	// Token: 0x04006020 RID: 24608
	public static List<LuauScriptRunner> ScriptRunners = new List<LuauScriptRunner>();

	// Token: 0x04006021 RID: 24609
	public bool ShouldTick;

	// Token: 0x04006022 RID: 24610
	private lua_CFunction postTickCallback;

	// Token: 0x04006023 RID: 24611
	private lua_CFunction preTickCallback;

	// Token: 0x04006024 RID: 24612
	public string ScriptName;

	// Token: 0x04006025 RID: 24613
	public string Script;

	// Token: 0x04006026 RID: 24614
	public unsafe lua_State* L;
}
