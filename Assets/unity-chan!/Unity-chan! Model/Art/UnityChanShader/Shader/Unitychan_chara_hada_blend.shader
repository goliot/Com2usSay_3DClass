Shader "UnityChan/Skin - Transparent"
{
    Properties
    {
        _Color ("Main Color", Color) = (1, 1, 1, 1)
        _ShadowColor ("Shadow Color", Color) = (0.8, 0.8, 1, 1)

        _MainTex ("Diffuse", 2D) = "white" {}
        _FalloffSampler ("Falloff Control", 2D) = "white" {}
        _RimLightSampler ("RimLight Control", 2D) = "white" {}
    }

    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha, One One
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent+1"
            "IgnoreProjector"="True"
            "RenderType"="Overlay"
            "LightMode"="UniversalForward"
        }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "CharaSkin.cg"
        ENDHLSL

        Pass
        {
            Cull Back
            ZTest LEqual
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }
    }

}