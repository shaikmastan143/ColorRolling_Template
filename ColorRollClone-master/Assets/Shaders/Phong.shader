Shader "Unlit/Phong"
{
    Properties
    {
        _MainTex("Texture",2D) = "white" {}
        _Color("_Color", Color) = (1,1,1,1)
        _Order("_Order", Float) = 1
        _Opaque("_Opaque", Float) = 0.5
        _UVScale("_UVScale", Float) = 1
    }
    SubShader
    {
        LOD 100
        ZTest Less 

        Pass
        {
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc" // for _LightColor0

            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #include"AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                
                float4 color : COLOR2;

                fixed3 diff : COLOR0;
                fixed3 ambient : COLOR1;

                SHADOW_COORDS(1) // put shadows data into TEXCOORD1
            };

            half4 _Color;
            half _Order;
            sampler2D _MainTex;
            half _Opaque;
            half _UVScale;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex) - float4(0,_Order * 0.001,0,0);
                o.uv = v.uv;
                o.color = v.color;

                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0.rgb;
                o.ambient = ShadeSH9(half4(worldNormal, 1));

                TRANSFER_SHADOW(o);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = lerp(tex2D(_MainTex,i.uv* _UVScale),_Color,_Opaque);
                fixed shadow = SHADOW_ATTENUATION(i);

                fixed3 lighting = i.diff * 0.2 * shadow + i.ambient + fixed3(0.7,0.7,0.7);
                col.rgb *= lighting;

                return col*i.color;
            }
            ENDCG
        }
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
