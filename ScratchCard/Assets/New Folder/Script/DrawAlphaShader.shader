Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Coordinates("Coordinate",Vector) = (0,0,0,0)
        _Color("Draw Color",Color) = (1,0,0,0)
        _Size("Size",Range(1,500))=0
        _Ratio("Ratio",float)= 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Coordinates,_Color;
            float _Size;
            float _Ratio;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);                
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float2 ratio = float2(_Ratio,1);
                float2 uv = i.uv * ratio; 
                float2 cord = _Coordinates.xy * ratio; 
                float draw= ceil(saturate((_Size/500)-distance(uv,cord)));
                float4 drawcol=_Color * (draw);
                float4 final=saturate(col+drawcol);
                final.a=1;
                return final;
            }
            ENDHLSL
        }
    }
}
