Shader "Unlit/DrawAorBWithmask3"
{
    Properties
    {
        _MainTexture ("_MainTexture", 2D) = "white" {}
        _SplatMap ("_SplatMap", 2D) = "white" {} 
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTexture;
            float4 _MainTexture_ST; 
            sampler2D _SplatMap;
            float4 _SplatMap_ST; 

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTexture);    
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 main = tex2D(_MainTexture,i.uv);
                float4 splat = tex2D(_SplatMap,i.uv);

                main.a=(1-splat.r);
                return main;
            }
            ENDHLSL
        }
    }
}
