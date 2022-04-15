Shader "Unlit/VertexColor"
{
    Properties
    {
        //_MainTex("Texture",2D) = "white" {}
        _Color("_Color", Color) = (1,1,1,1)
        _Ambient("_Ambient", Float) = 0.7
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

                struct appdata
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float4 color : COLOR;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float4 color : COLOR2;

                    fixed3 diff : COLOR0;
                };

                half4 _Color;
                half _Ambient;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.color = v.color;

                    half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                    half nl = lerp(_Ambient, 1.0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                    o.diff = nl * _LightColor0.rgb;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = i.color;

                    fixed3 lighting = i.diff;

                    col.rgb *= lighting;

                    return col;
                }
                ENDCG
            }
            UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
        }
}
