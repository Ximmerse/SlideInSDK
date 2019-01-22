// Ximmerse shader source. Copyright (c) 2018 Ximmerse.

//  Normal colored texture shader. 
// - Intensity
// - Opaque 

Shader "Ximmerse/AR" 
{
Properties 
{
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _Intensity ("Color intensity", Float) = 1
}

SubShader 
{
    Tags 
    { 
       "RenderType"="Opaque" 
       "RenderToEye" = "NONE"
    }
    LOD 100

    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog
            #pragma multi_compile __ _VIEW_CUT_ENABLED
            
            #include "UnityCG.cginc"


            struct appdata_t {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_OUTPUT_STEREO
                float normalizeX : TEXCOORD1;
            };

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Intensity;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normalizeX = o.vertex.x;
                o.color = v.color;
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord) * i.color * _Color;
                UNITY_OPAQUE_ALPHA(col.a);
                return col * _Intensity;
            }
        ENDCG
    }
}

}
