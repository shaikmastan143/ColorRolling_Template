Shader "Unlit/Carpet"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)

        _StartAngle("Start Angle (Radians)",float) = 25.0
        _AnglePerUnit("Angle per Unit (Radians)",float) = 6.28
        _Pitch("Pitch",float) = 0.01
        _UnrolledAngle("Unrolled Angle",float) = 0

    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        LOD 100
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc" // for _LightColor0

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                fixed3 diff : COLOR0;
                fixed3 ambient : COLOR1;
                //fixed3 normal: COLOR3
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            float _StartAngle;
            float _AnglePerUnit;
            float _Pitch;
            float _UnrolledAngle;

            float angleToArcLength(float angle) {
                float radical = sqrt(angle * angle + 1.0f);
                return _Pitch * 0.5f * (angle * radical + log(angle + radical));
            }

            v2f vert(appdata v)
            {
                v2f o;

                float angle = v.vertex.z * _AnglePerUnit;
                float fromOrigin = _StartAngle - angle;
                float lengthToHere = angleToArcLength(fromOrigin);
                float lengthToStart = angleToArcLength(_StartAngle);

                v.uv.y = lengthToStart - lengthToHere;
                if (angle < _UnrolledAngle) {
                    //float lengthToSplit = arcLengthToAngle(_StartAngle - _UnrolledAngle);
                    //v.vertex.z = lengthToSplit - lengthToHere;
                    //v.vertex.y = 0.0f;
                }
                else {
                    float radiusAtSplit = _Pitch * (_StartAngle - _UnrolledAngle);
                    float radius = _Pitch * fromOrigin;
                    float shifted = angle - _UnrolledAngle;
                    float rolledLength = _UnrolledAngle / _AnglePerUnit;
                    float sinShifted = sin(shifted);
                    float cosShifted = cos(shifted);
                    v.vertex.z = sinShifted * (radius - v.vertex.y) + rolledLength;
                    v.vertex.y = radiusAtSplit - cosShifted * (radius - v.vertex.y);
                    //v.normal = float3(0, cos(shifted), -sin(shifted));

                    if (v.normal.z != 0) {
                        sinShifted = -sinShifted;
                        fixed4x4 rotationMatrix = fixed4x4(fixed4(1, 0, 0, 0), fixed4(0, cosShifted, -sinShifted, 0), fixed4(0, sinShifted, cosShifted, 0), fixed4(0, 0, 0, 1));
                        fixed4 newNormal = mul(rotationMatrix, fixed4(v.normal, 0.0));
                        v.normal = newNormal.xyz;
                    }
                    else {
                        fixed up = sign(v.normal.y);
                        v.normal = float3(v.normal.x, cos(shifted) * up, -sin(shifted) * up);
                    }


                    //fixed4x4 rotationMatrix = fixed4x4(fixed4(1, 0, 0, 0), fixed4(0, cosShifted, sinShifted, 0), fixed4(0, sinShifted, cosShifted, 0), fixed4(0, 0, 0, 1));
                    //fixed4 newNormal = mul(rotationMatrix, fixed4(v.normal.xyz, 0.0));
                    //v.normal = newNormal.xyz;


                    //update the normal
                    //fixed4x4 rotationMatrix = fixed4x4(fixed4(1,0,0,0), fixed4(0,cosShifted,-sinShifted,0), fixed4(0, sinShifted,cosShifted,0), fixed4(0,0,0,1));

                    ////fixed up = sign(v.normal.y);
                    //fixed4 newNormal = mul(rotationMatrix,fixed4(v.normal,0.0));
                    //v.normal = float3(newNormal.x,newNormal.y,newNormal.z);// float3(v.normal.x, cosShifted * up, -sinShifted * up);
                }

                //fixed theta = length(fixed3(0, v.vertex.yz))*_Time.y*3.14 / 4.0;
                //if (_Clockwise == 0)
                //    theta *= -1;
                //
                //fixed cosTheta = cos(theta);
                //fixed sinTheta = sin(theta);
                //fixed scale = 0.5;

                //fixed4x4 transfomation = fixed4x4(fixed4(1,0,0,0), fixed4(0,cosTheta*scale,-sinTheta,0), fixed4(0, sinTheta,cosTheta * scale,0), fixed4(0,0,0,1));
                //v.vertex = mul(transfomation, v.vertex);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                half3 worldNormal = UnityObjectToWorldNormal(v.normal);

                half nl = max(0,dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0.rgb;
                o.ambient = ShadeSH9(half4(worldNormal, 1));

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = _Color;

                fixed3 lighting = i.diff*0.5 +i.ambient+fixed3(0.5,0.5,0.5);
                col.rgb *= lighting;

                return col;
            }
            ENDCG
        }
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

    }
}
