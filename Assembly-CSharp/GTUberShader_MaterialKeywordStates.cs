using System;
using UnityEngine;

// Token: 0x02000E04 RID: 3588
public struct GTUberShader_MaterialKeywordStates
{
	// Token: 0x0600578F RID: 22415 RVA: 0x001C51E8 File Offset: 0x001C33E8
	public GTUberShader_MaterialKeywordStates(Material mat)
	{
		this.material = mat;
		this._ALPHA_BLUE_LIVE_ON = mat.IsKeywordEnabled("_ALPHA_BLUE_LIVE_ON");
		this._ALPHA_DETAIL_MAP = mat.IsKeywordEnabled("_ALPHA_DETAIL_MAP");
		this._ALPHATEST_ON = mat.IsKeywordEnabled("_ALPHATEST_ON");
		this._COLOR_GRADE_ACHROMATOMALY = mat.IsKeywordEnabled("_COLOR_GRADE_ACHROMATOMALY");
		this._COLOR_GRADE_ACHROMATOPSIA = mat.IsKeywordEnabled("_COLOR_GRADE_ACHROMATOPSIA");
		this._COLOR_GRADE_DEUTERANOMALY = mat.IsKeywordEnabled("_COLOR_GRADE_DEUTERANOMALY");
		this._COLOR_GRADE_DEUTERANOPIA = mat.IsKeywordEnabled("_COLOR_GRADE_DEUTERANOPIA");
		this._COLOR_GRADE_PROTANOMALY = mat.IsKeywordEnabled("_COLOR_GRADE_PROTANOMALY");
		this._COLOR_GRADE_PROTANOPIA = mat.IsKeywordEnabled("_COLOR_GRADE_PROTANOPIA");
		this._COLOR_GRADE_TRITANOMALY = mat.IsKeywordEnabled("_COLOR_GRADE_TRITANOMALY");
		this._COLOR_GRADE_TRITANOPIA = mat.IsKeywordEnabled("_COLOR_GRADE_TRITANOPIA");
		this._CRYSTAL_EFFECT = mat.IsKeywordEnabled("_CRYSTAL_EFFECT");
		this._DAY_CYCLE_BRIGHTNESS__OPTION_1 = mat.IsKeywordEnabled("_DAY_CYCLE_BRIGHTNESS__OPTION_1");
		this._DAY_CYCLE_BRIGHTNESS__OPTION_2 = mat.IsKeywordEnabled("_DAY_CYCLE_BRIGHTNESS__OPTION_2");
		this._DEBUG_PAWN_DATA = mat.IsKeywordEnabled("_DEBUG_PAWN_DATA");
		this._EMISSION = mat.IsKeywordEnabled("_EMISSION");
		this._EMISSION_USE_UV_WAVE_WARP = mat.IsKeywordEnabled("_EMISSION_USE_UV_WAVE_WARP");
		this._EYECOMP = mat.IsKeywordEnabled("_EYECOMP");
		this._FX_LAVA_LAMP = mat.IsKeywordEnabled("_FX_LAVA_LAMP");
		this._GLOBAL_ZONE_LIQUID_TYPE__LAVA = mat.IsKeywordEnabled("_GLOBAL_ZONE_LIQUID_TYPE__LAVA");
		this._GLOBAL_ZONE_LIQUID_TYPE__WATER = mat.IsKeywordEnabled("_GLOBAL_ZONE_LIQUID_TYPE__WATER");
		this._GRADIENT_MAP_ON = mat.IsKeywordEnabled("_GRADIENT_MAP_ON");
		this._GRID_EFFECT = mat.IsKeywordEnabled("_GRID_EFFECT");
		this._GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY = mat.IsKeywordEnabled("_GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY");
		this._GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z = mat.IsKeywordEnabled("_GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z");
		this._GT_EDITOR_TIME = mat.IsKeywordEnabled("_GT_EDITOR_TIME");
		this._GT_RIM_LIGHT = mat.IsKeywordEnabled("_GT_RIM_LIGHT");
		this._GT_RIM_LIGHT_FLAT = mat.IsKeywordEnabled("_GT_RIM_LIGHT_FLAT");
		this._GT_RIM_LIGHT_USE_ALPHA = mat.IsKeywordEnabled("_GT_RIM_LIGHT_USE_ALPHA");
		this._HALF_LAMBERT_TERM = mat.IsKeywordEnabled("_HALF_LAMBERT_TERM");
		this._HEIGHT_BASED_WATER_EFFECT = mat.IsKeywordEnabled("_HEIGHT_BASED_WATER_EFFECT");
		this._INNER_GLOW = mat.IsKeywordEnabled("_INNER_GLOW");
		this._LIQUID_CONTAINER = mat.IsKeywordEnabled("_LIQUID_CONTAINER");
		this._LIQUID_VOLUME = mat.IsKeywordEnabled("_LIQUID_VOLUME");
		this._MAINTEX_ROTATE = mat.IsKeywordEnabled("_MAINTEX_ROTATE");
		this._MASK_MAP_ON = mat.IsKeywordEnabled("_MASK_MAP_ON");
		this._MOUTHCOMP = mat.IsKeywordEnabled("_MOUTHCOMP");
		this._PARALLAX = mat.IsKeywordEnabled("_PARALLAX");
		this._PARALLAX_AA = mat.IsKeywordEnabled("_PARALLAX_AA");
		this._PARALLAX_PLANAR = mat.IsKeywordEnabled("_PARALLAX_PLANAR");
		this._REFLECTIONS = mat.IsKeywordEnabled("_REFLECTIONS");
		this._REFLECTIONS_ALBEDO_TINT = mat.IsKeywordEnabled("_REFLECTIONS_ALBEDO_TINT");
		this._REFLECTIONS_BOX_PROJECT = mat.IsKeywordEnabled("_REFLECTIONS_BOX_PROJECT");
		this._REFLECTIONS_MATCAP = mat.IsKeywordEnabled("_REFLECTIONS_MATCAP");
		this._REFLECTIONS_MATCAP_PERSP_AWARE = mat.IsKeywordEnabled("_REFLECTIONS_MATCAP_PERSP_AWARE");
		this._REFLECTIONS_USE_NORMAL_TEX = mat.IsKeywordEnabled("_REFLECTIONS_USE_NORMAL_TEX");
		this._SPECULAR_HIGHLIGHT = mat.IsKeywordEnabled("_SPECULAR_HIGHLIGHT");
		this._STEALTH_EFFECT = mat.IsKeywordEnabled("_STEALTH_EFFECT");
		this._TEXEL_SNAP_UVS = mat.IsKeywordEnabled("_TEXEL_SNAP_UVS");
		this._UNITY_EDIT_MODE = mat.IsKeywordEnabled("_UNITY_EDIT_MODE");
		this._USE_DAY_NIGHT_LIGHTMAP = mat.IsKeywordEnabled("_USE_DAY_NIGHT_LIGHTMAP");
		this._USE_DEFORM_MAP = mat.IsKeywordEnabled("_USE_DEFORM_MAP");
		this._USE_TEX_ARRAY_ATLAS = mat.IsKeywordEnabled("_USE_TEX_ARRAY_ATLAS");
		this._USE_TEXTURE = mat.IsKeywordEnabled("_USE_TEXTURE");
		this._USE_VERTEX_COLOR = mat.IsKeywordEnabled("_USE_VERTEX_COLOR");
		this._USE_WEATHER_MAP = mat.IsKeywordEnabled("_USE_WEATHER_MAP");
		this._UV_SHIFT = mat.IsKeywordEnabled("_UV_SHIFT");
		this._UV_SOURCE__UV0 = mat.IsKeywordEnabled("_UV_SOURCE__UV0");
		this._UV_SOURCE__WORLD_PLANAR_Y = mat.IsKeywordEnabled("_UV_SOURCE__WORLD_PLANAR_Y");
		this._UV_WAVE_WARP = mat.IsKeywordEnabled("_UV_WAVE_WARP");
		this._VERTEX_ANIM_FLAP = mat.IsKeywordEnabled("_VERTEX_ANIM_FLAP");
		this._VERTEX_ANIM_WAVE = mat.IsKeywordEnabled("_VERTEX_ANIM_WAVE");
		this._VERTEX_ANIM_WAVE_DEBUG = mat.IsKeywordEnabled("_VERTEX_ANIM_WAVE_DEBUG");
		this._VERTEX_ROTATE = mat.IsKeywordEnabled("_VERTEX_ROTATE");
		this._WATER_CAUSTICS = mat.IsKeywordEnabled("_WATER_CAUSTICS");
		this._WATER_EFFECT = mat.IsKeywordEnabled("_WATER_EFFECT");
		this._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX = mat.IsKeywordEnabled("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
		this._ZONE_LIQUID_SHAPE__CYLINDER = mat.IsKeywordEnabled("_ZONE_LIQUID_SHAPE__CYLINDER");
		this.DIRLIGHTMAP_COMBINED = mat.IsKeywordEnabled("DIRLIGHTMAP_COMBINED");
		this.INSTANCING_ON = mat.IsKeywordEnabled("INSTANCING_ON");
		this.LIGHTMAP_ON = mat.IsKeywordEnabled("LIGHTMAP_ON");
		this.STEREO_CUBEMAP_RENDER_ON = mat.IsKeywordEnabled("STEREO_CUBEMAP_RENDER_ON");
		this.STEREO_INSTANCING_ON = mat.IsKeywordEnabled("STEREO_INSTANCING_ON");
		this.STEREO_MULTIVIEW_ON = mat.IsKeywordEnabled("STEREO_MULTIVIEW_ON");
		this.UNITY_SINGLE_PASS_STEREO = mat.IsKeywordEnabled("UNITY_SINGLE_PASS_STEREO");
		this.USE_TEXTURE__AS_MASK = mat.IsKeywordEnabled("USE_TEXTURE__AS_MASK");
	}

	// Token: 0x06005790 RID: 22416 RVA: 0x001C5708 File Offset: 0x001C3908
	public void Refresh()
	{
		Material material = this.material;
		this._ALPHA_BLUE_LIVE_ON = material.IsKeywordEnabled("_ALPHA_BLUE_LIVE_ON");
		this._ALPHA_DETAIL_MAP = material.IsKeywordEnabled("_ALPHA_DETAIL_MAP");
		this._ALPHATEST_ON = material.IsKeywordEnabled("_ALPHATEST_ON");
		this._COLOR_GRADE_ACHROMATOMALY = material.IsKeywordEnabled("_COLOR_GRADE_ACHROMATOMALY");
		this._COLOR_GRADE_ACHROMATOPSIA = material.IsKeywordEnabled("_COLOR_GRADE_ACHROMATOPSIA");
		this._COLOR_GRADE_DEUTERANOMALY = material.IsKeywordEnabled("_COLOR_GRADE_DEUTERANOMALY");
		this._COLOR_GRADE_DEUTERANOPIA = material.IsKeywordEnabled("_COLOR_GRADE_DEUTERANOPIA");
		this._COLOR_GRADE_PROTANOMALY = material.IsKeywordEnabled("_COLOR_GRADE_PROTANOMALY");
		this._COLOR_GRADE_PROTANOPIA = material.IsKeywordEnabled("_COLOR_GRADE_PROTANOPIA");
		this._COLOR_GRADE_TRITANOMALY = material.IsKeywordEnabled("_COLOR_GRADE_TRITANOMALY");
		this._COLOR_GRADE_TRITANOPIA = material.IsKeywordEnabled("_COLOR_GRADE_TRITANOPIA");
		this._CRYSTAL_EFFECT = material.IsKeywordEnabled("_CRYSTAL_EFFECT");
		this._DAY_CYCLE_BRIGHTNESS__OPTION_1 = material.IsKeywordEnabled("_DAY_CYCLE_BRIGHTNESS__OPTION_1");
		this._DAY_CYCLE_BRIGHTNESS__OPTION_2 = material.IsKeywordEnabled("_DAY_CYCLE_BRIGHTNESS__OPTION_2");
		this._DEBUG_PAWN_DATA = material.IsKeywordEnabled("_DEBUG_PAWN_DATA");
		this._EMISSION = material.IsKeywordEnabled("_EMISSION");
		this._EMISSION_USE_UV_WAVE_WARP = material.IsKeywordEnabled("_EMISSION_USE_UV_WAVE_WARP");
		this._EYECOMP = material.IsKeywordEnabled("_EYECOMP");
		this._FX_LAVA_LAMP = material.IsKeywordEnabled("_FX_LAVA_LAMP");
		this._GLOBAL_ZONE_LIQUID_TYPE__LAVA = material.IsKeywordEnabled("_GLOBAL_ZONE_LIQUID_TYPE__LAVA");
		this._GLOBAL_ZONE_LIQUID_TYPE__WATER = material.IsKeywordEnabled("_GLOBAL_ZONE_LIQUID_TYPE__WATER");
		this._GRADIENT_MAP_ON = material.IsKeywordEnabled("_GRADIENT_MAP_ON");
		this._GRID_EFFECT = material.IsKeywordEnabled("_GRID_EFFECT");
		this._GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY = material.IsKeywordEnabled("_GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY");
		this._GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z = material.IsKeywordEnabled("_GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z");
		this._GT_EDITOR_TIME = material.IsKeywordEnabled("_GT_EDITOR_TIME");
		this._GT_RIM_LIGHT = material.IsKeywordEnabled("_GT_RIM_LIGHT");
		this._GT_RIM_LIGHT_FLAT = material.IsKeywordEnabled("_GT_RIM_LIGHT_FLAT");
		this._GT_RIM_LIGHT_USE_ALPHA = material.IsKeywordEnabled("_GT_RIM_LIGHT_USE_ALPHA");
		this._HALF_LAMBERT_TERM = material.IsKeywordEnabled("_HALF_LAMBERT_TERM");
		this._HEIGHT_BASED_WATER_EFFECT = material.IsKeywordEnabled("_HEIGHT_BASED_WATER_EFFECT");
		this._INNER_GLOW = material.IsKeywordEnabled("_INNER_GLOW");
		this._LIQUID_CONTAINER = material.IsKeywordEnabled("_LIQUID_CONTAINER");
		this._LIQUID_VOLUME = material.IsKeywordEnabled("_LIQUID_VOLUME");
		this._MAINTEX_ROTATE = material.IsKeywordEnabled("_MAINTEX_ROTATE");
		this._MASK_MAP_ON = material.IsKeywordEnabled("_MASK_MAP_ON");
		this._MOUTHCOMP = material.IsKeywordEnabled("_MOUTHCOMP");
		this._PARALLAX = material.IsKeywordEnabled("_PARALLAX");
		this._PARALLAX_AA = material.IsKeywordEnabled("_PARALLAX_AA");
		this._PARALLAX_PLANAR = material.IsKeywordEnabled("_PARALLAX_PLANAR");
		this._REFLECTIONS = material.IsKeywordEnabled("_REFLECTIONS");
		this._REFLECTIONS_ALBEDO_TINT = material.IsKeywordEnabled("_REFLECTIONS_ALBEDO_TINT");
		this._REFLECTIONS_BOX_PROJECT = material.IsKeywordEnabled("_REFLECTIONS_BOX_PROJECT");
		this._REFLECTIONS_MATCAP = material.IsKeywordEnabled("_REFLECTIONS_MATCAP");
		this._REFLECTIONS_MATCAP_PERSP_AWARE = material.IsKeywordEnabled("_REFLECTIONS_MATCAP_PERSP_AWARE");
		this._REFLECTIONS_USE_NORMAL_TEX = material.IsKeywordEnabled("_REFLECTIONS_USE_NORMAL_TEX");
		this._SPECULAR_HIGHLIGHT = material.IsKeywordEnabled("_SPECULAR_HIGHLIGHT");
		this._STEALTH_EFFECT = material.IsKeywordEnabled("_STEALTH_EFFECT");
		this._TEXEL_SNAP_UVS = material.IsKeywordEnabled("_TEXEL_SNAP_UVS");
		this._UNITY_EDIT_MODE = material.IsKeywordEnabled("_UNITY_EDIT_MODE");
		this._USE_DAY_NIGHT_LIGHTMAP = material.IsKeywordEnabled("_USE_DAY_NIGHT_LIGHTMAP");
		this._USE_DEFORM_MAP = material.IsKeywordEnabled("_USE_DEFORM_MAP");
		this._USE_TEX_ARRAY_ATLAS = material.IsKeywordEnabled("_USE_TEX_ARRAY_ATLAS");
		this._USE_TEXTURE = material.IsKeywordEnabled("_USE_TEXTURE");
		this._USE_VERTEX_COLOR = material.IsKeywordEnabled("_USE_VERTEX_COLOR");
		this._USE_WEATHER_MAP = material.IsKeywordEnabled("_USE_WEATHER_MAP");
		this._UV_SHIFT = material.IsKeywordEnabled("_UV_SHIFT");
		this._UV_SOURCE__UV0 = material.IsKeywordEnabled("_UV_SOURCE__UV0");
		this._UV_SOURCE__WORLD_PLANAR_Y = material.IsKeywordEnabled("_UV_SOURCE__WORLD_PLANAR_Y");
		this._UV_WAVE_WARP = material.IsKeywordEnabled("_UV_WAVE_WARP");
		this._VERTEX_ANIM_FLAP = material.IsKeywordEnabled("_VERTEX_ANIM_FLAP");
		this._VERTEX_ANIM_WAVE = material.IsKeywordEnabled("_VERTEX_ANIM_WAVE");
		this._VERTEX_ANIM_WAVE_DEBUG = material.IsKeywordEnabled("_VERTEX_ANIM_WAVE_DEBUG");
		this._VERTEX_ROTATE = material.IsKeywordEnabled("_VERTEX_ROTATE");
		this._WATER_CAUSTICS = material.IsKeywordEnabled("_WATER_CAUSTICS");
		this._WATER_EFFECT = material.IsKeywordEnabled("_WATER_EFFECT");
		this._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX = material.IsKeywordEnabled("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
		this._ZONE_LIQUID_SHAPE__CYLINDER = material.IsKeywordEnabled("_ZONE_LIQUID_SHAPE__CYLINDER");
		this.DIRLIGHTMAP_COMBINED = material.IsKeywordEnabled("DIRLIGHTMAP_COMBINED");
		this.INSTANCING_ON = material.IsKeywordEnabled("INSTANCING_ON");
		this.LIGHTMAP_ON = material.IsKeywordEnabled("LIGHTMAP_ON");
		this.STEREO_CUBEMAP_RENDER_ON = material.IsKeywordEnabled("STEREO_CUBEMAP_RENDER_ON");
		this.STEREO_INSTANCING_ON = material.IsKeywordEnabled("STEREO_INSTANCING_ON");
		this.STEREO_MULTIVIEW_ON = material.IsKeywordEnabled("STEREO_MULTIVIEW_ON");
		this.UNITY_SINGLE_PASS_STEREO = material.IsKeywordEnabled("UNITY_SINGLE_PASS_STEREO");
		this.USE_TEXTURE__AS_MASK = material.IsKeywordEnabled("USE_TEXTURE__AS_MASK");
	}

	// Token: 0x040067E2 RID: 26594
	public Material material;

	// Token: 0x040067E3 RID: 26595
	public bool _ALPHA_BLUE_LIVE_ON;

	// Token: 0x040067E4 RID: 26596
	public bool _ALPHA_DETAIL_MAP;

	// Token: 0x040067E5 RID: 26597
	public bool _ALPHATEST_ON;

	// Token: 0x040067E6 RID: 26598
	public bool _COLOR_GRADE_ACHROMATOMALY;

	// Token: 0x040067E7 RID: 26599
	public bool _COLOR_GRADE_ACHROMATOPSIA;

	// Token: 0x040067E8 RID: 26600
	public bool _COLOR_GRADE_DEUTERANOMALY;

	// Token: 0x040067E9 RID: 26601
	public bool _COLOR_GRADE_DEUTERANOPIA;

	// Token: 0x040067EA RID: 26602
	public bool _COLOR_GRADE_PROTANOMALY;

	// Token: 0x040067EB RID: 26603
	public bool _COLOR_GRADE_PROTANOPIA;

	// Token: 0x040067EC RID: 26604
	public bool _COLOR_GRADE_TRITANOMALY;

	// Token: 0x040067ED RID: 26605
	public bool _COLOR_GRADE_TRITANOPIA;

	// Token: 0x040067EE RID: 26606
	public bool _CRYSTAL_EFFECT;

	// Token: 0x040067EF RID: 26607
	public bool _DAY_CYCLE_BRIGHTNESS__OPTION_1;

	// Token: 0x040067F0 RID: 26608
	public bool _DAY_CYCLE_BRIGHTNESS__OPTION_2;

	// Token: 0x040067F1 RID: 26609
	public bool _DEBUG_PAWN_DATA;

	// Token: 0x040067F2 RID: 26610
	public bool _EMISSION;

	// Token: 0x040067F3 RID: 26611
	public bool _EMISSION_USE_UV_WAVE_WARP;

	// Token: 0x040067F4 RID: 26612
	public bool _EYECOMP;

	// Token: 0x040067F5 RID: 26613
	public bool _FX_LAVA_LAMP;

	// Token: 0x040067F6 RID: 26614
	public bool _GLOBAL_ZONE_LIQUID_TYPE__LAVA;

	// Token: 0x040067F7 RID: 26615
	public bool _GLOBAL_ZONE_LIQUID_TYPE__WATER;

	// Token: 0x040067F8 RID: 26616
	public bool _GRADIENT_MAP_ON;

	// Token: 0x040067F9 RID: 26617
	public bool _GRID_EFFECT;

	// Token: 0x040067FA RID: 26618
	public bool _GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY;

	// Token: 0x040067FB RID: 26619
	public bool _GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z;

	// Token: 0x040067FC RID: 26620
	public bool _GT_EDITOR_TIME;

	// Token: 0x040067FD RID: 26621
	public bool _GT_RIM_LIGHT;

	// Token: 0x040067FE RID: 26622
	public bool _GT_RIM_LIGHT_FLAT;

	// Token: 0x040067FF RID: 26623
	public bool _GT_RIM_LIGHT_USE_ALPHA;

	// Token: 0x04006800 RID: 26624
	public bool _HALF_LAMBERT_TERM;

	// Token: 0x04006801 RID: 26625
	public bool _HEIGHT_BASED_WATER_EFFECT;

	// Token: 0x04006802 RID: 26626
	public bool _INNER_GLOW;

	// Token: 0x04006803 RID: 26627
	public bool _LIQUID_CONTAINER;

	// Token: 0x04006804 RID: 26628
	public bool _LIQUID_VOLUME;

	// Token: 0x04006805 RID: 26629
	public bool _MAINTEX_ROTATE;

	// Token: 0x04006806 RID: 26630
	public bool _MASK_MAP_ON;

	// Token: 0x04006807 RID: 26631
	public bool _MOUTHCOMP;

	// Token: 0x04006808 RID: 26632
	public bool _PARALLAX;

	// Token: 0x04006809 RID: 26633
	public bool _PARALLAX_AA;

	// Token: 0x0400680A RID: 26634
	public bool _PARALLAX_PLANAR;

	// Token: 0x0400680B RID: 26635
	public bool _REFLECTIONS;

	// Token: 0x0400680C RID: 26636
	public bool _REFLECTIONS_ALBEDO_TINT;

	// Token: 0x0400680D RID: 26637
	public bool _REFLECTIONS_BOX_PROJECT;

	// Token: 0x0400680E RID: 26638
	public bool _REFLECTIONS_MATCAP;

	// Token: 0x0400680F RID: 26639
	public bool _REFLECTIONS_MATCAP_PERSP_AWARE;

	// Token: 0x04006810 RID: 26640
	public bool _REFLECTIONS_USE_NORMAL_TEX;

	// Token: 0x04006811 RID: 26641
	public bool _SPECULAR_HIGHLIGHT;

	// Token: 0x04006812 RID: 26642
	public bool _STEALTH_EFFECT;

	// Token: 0x04006813 RID: 26643
	public bool _TEXEL_SNAP_UVS;

	// Token: 0x04006814 RID: 26644
	public bool _UNITY_EDIT_MODE;

	// Token: 0x04006815 RID: 26645
	public bool _USE_DAY_NIGHT_LIGHTMAP;

	// Token: 0x04006816 RID: 26646
	public bool _USE_DEFORM_MAP;

	// Token: 0x04006817 RID: 26647
	public bool _USE_TEX_ARRAY_ATLAS;

	// Token: 0x04006818 RID: 26648
	public bool _USE_TEXTURE;

	// Token: 0x04006819 RID: 26649
	public bool _USE_VERTEX_COLOR;

	// Token: 0x0400681A RID: 26650
	public bool _USE_WEATHER_MAP;

	// Token: 0x0400681B RID: 26651
	public bool _UV_SHIFT;

	// Token: 0x0400681C RID: 26652
	public bool _UV_SOURCE__UV0;

	// Token: 0x0400681D RID: 26653
	public bool _UV_SOURCE__WORLD_PLANAR_Y;

	// Token: 0x0400681E RID: 26654
	public bool _UV_WAVE_WARP;

	// Token: 0x0400681F RID: 26655
	public bool _VERTEX_ANIM_FLAP;

	// Token: 0x04006820 RID: 26656
	public bool _VERTEX_ANIM_WAVE;

	// Token: 0x04006821 RID: 26657
	public bool _VERTEX_ANIM_WAVE_DEBUG;

	// Token: 0x04006822 RID: 26658
	public bool _VERTEX_ROTATE;

	// Token: 0x04006823 RID: 26659
	public bool _WATER_CAUSTICS;

	// Token: 0x04006824 RID: 26660
	public bool _WATER_EFFECT;

	// Token: 0x04006825 RID: 26661
	public bool _ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX;

	// Token: 0x04006826 RID: 26662
	public bool _ZONE_LIQUID_SHAPE__CYLINDER;

	// Token: 0x04006827 RID: 26663
	public bool DIRLIGHTMAP_COMBINED;

	// Token: 0x04006828 RID: 26664
	public bool INSTANCING_ON;

	// Token: 0x04006829 RID: 26665
	public bool LIGHTMAP_ON;

	// Token: 0x0400682A RID: 26666
	public bool STEREO_CUBEMAP_RENDER_ON;

	// Token: 0x0400682B RID: 26667
	public bool STEREO_INSTANCING_ON;

	// Token: 0x0400682C RID: 26668
	public bool STEREO_MULTIVIEW_ON;

	// Token: 0x0400682D RID: 26669
	public bool UNITY_SINGLE_PASS_STEREO;

	// Token: 0x0400682E RID: 26670
	public bool USE_TEXTURE__AS_MASK;
}
