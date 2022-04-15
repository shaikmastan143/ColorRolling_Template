Shader "Unlit/Geometry"
{
    Properties
    {
        _Color("_Color", Color) = (1,1,1,1)
        _YOffset("_YOffset", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc" // for _LightColor0

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed3 diff : COLOR0;
                fixed3 ambient : COLOR1;
            };

            half4 _Color;
            float _YOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex + float4(0, _YOffset, 0, 0));

                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0.rgb;
                o.ambient = ShadeSH9(half4(worldNormal, 1));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color;

                fixed3 lighting = i.diff * 0.5 + i.ambient + fixed3(0.5,0.5,0.5);
                col.rgb *= lighting;

                return col;
            }
            ENDCG
        }
    }
}
