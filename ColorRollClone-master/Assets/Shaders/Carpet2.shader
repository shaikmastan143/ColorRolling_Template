Shader "Unlit/Carpet2"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)

        _StartAngle("Start Angle (Radians)",float) = 25.0
        _AnglePerUnit("Angle per Unit (Radians)",float) = 6.28
        _Pitch("Pitch",float) = 0.01
        _UnrolledAngle("Unrolled Angle",float) = 0



        _BottomColor("_BottomColor", Color) = (1,1,1,1)
        _TopColor("_TopColor", Color) = (1,1,1,1)

        _BendRotationRandom("Bend Rotation Random", Range(0, 1)) = 0.2

        _BladeWidth("Blade Width", Float) = 0.05
        _BladeWidthRandom("Blade Width Random", Float) = 0.02
        _BladeHeight("Blade Height", Float) = 0.5
        _BladeHeightRandom("Blade Height Random", Float) = 0.3

        _WindDistortionMap("Wind Distortion Map", 2D) = "white" {}
        _WindFrequency("Wind Frequency", Vector) = (0.05, 0.05, 0, 0)
        _WindStrength("Wind Strength", Float) = 1
    }



        CGINCLUDE
#include "UnityCG.cginc"
#include "Autolight.cginc"
            //#include "CustomTessellation.cginc"
#include "UnityLightingCommon.cginc" // for _LightColor0

            float _Factor;
        float4 _BottomColor;
        float4 _TopColor;
        float _BendRotationRandom;


        float _BladeHeight;
        float _BladeHeightRandom;
        float _BladeWidth;
        float _BladeWidthRandom;

        sampler2D _WindDistortionMap;
        float4 _WindDistortionMap_ST;

        float2 _WindFrequency;
        float _WindStrength;

        struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 tangent : TANGENT;
            float2 uv : TEXCOORD0;
        };

        struct vertexOutput {

            float4 vertex: SV_POSITION;
            float3 normal: NORMAL;
            float4 tangent : TANGENT;
            float2 uv: TEXCOORD0;


        };

        struct GeometryOutput {
            float4 vertex: SV_POSITION;
            float2 uv: TEXCOORD0;

            fixed3 diff : COLOR0;
            fixed3 ambient : COLOR1;
            //fixed3 normal: COLOR3
        };
        // Simple noise function, sourced from http://answers.unity.com/answers/624136/view.html
        // Extended discussion on this function can be found at the following link:
        // https://forum.unity.com/threads/am-i-over-complicating-this-random-function.454887/#post-2949326
        // Returns a number in the 0...1 range.
        float rand(float3 co)
        {
            return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
        }
        // Construct a rotation matrix that rotates around the provided axis, sourced from:
        // https://gist.github.com/keijiro/ee439d5e7388f3aafc5296005c8c3f33
        float3x3 AngleAxis3x3(float angle, float3 axis)
        {
            float c, s;
            sincos(angle, s, c);

            float t = 1 - c;
            float x = axis.x;
            float y = axis.y;
            float z = axis.z;

            return float3x3(
                t * x * x + c, t * x * y - s * z, t * x * z + s * y,
                t * x * y + s * z, t * y * y + c, t * y * z - s * x,
                t * x * z - s * y, t * y * z + s * x, t * z * z + c
                );
        }


        GeometryOutput geometryOutput;
        GeometryOutput GeometryVertex(float3 pos, float2 uv,float3 normal/*, float3 diff, float3 ambient*/) {
            geometryOutput.vertex = UnityObjectToClipPos(pos);
            geometryOutput.uv = uv;

            half3 worldNormal = UnityObjectToWorldNormal(normal);
            half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
            geometryOutput.diff = nl * _LightColor0.rgb;
            geometryOutput.ambient = ShadeSH9(half4(worldNormal, 1));

            return geometryOutput;
        }

        //vertexOutput vert(vertexInput i)
        //{
        //    vertexOutput o;

        //    o.pos = i.vertex;
        //    o.normal = i.normal;
        //    o.tangent = i.tangent;
        //    //o.uv = i.uv;

        //    return o;
        //}

        [maxvertexcount(12)]
        void geo(triangle vertexOutput input[3], inout TriangleStream<GeometryOutput> triStream) {//float4 input[3]: SV_POSITION
            float3 pos = input[0].vertex;

            float3 vNormal = input[0].normal;
            float4 vTangent = input[0].tangent;
            float3 vBinormal = cross(vNormal, vTangent) * vTangent.w;

            float3x3 tangentToLocal = float3x3(
                vTangent.x, vBinormal.x, vNormal.x,
                vTangent.y, vBinormal.y, vNormal.y,
                vTangent.z, vBinormal.z, vNormal.z
                );

            float3x3 facingRotationMatrix = AngleAxis3x3(rand(pos) * UNITY_TWO_PI, float3(0, 0, 1));
            float3x3 bendRotationMatrix = AngleAxis3x3(rand(pos.zzx) * _BendRotationRandom * UNITY_PI * 0.5, float3(-1, 0, 0));

            float2 uv = pos.xz * _WindDistortionMap_ST.xy + _WindDistortionMap_ST.zw + _WindFrequency * _Time.y;
            float2 windSample = (tex2Dlod(_WindDistortionMap, float4(uv.x, uv.y, 0, 0)).xy * 2 - 1) * _WindStrength;
            float3 wind = normalize(float3(windSample.x, windSample.y, 0));
            float3x3 windRotation = AngleAxis3x3(UNITY_PI * windSample, wind);

            float3x3 transformationMatrix = mul(mul(mul(tangentToLocal, windRotation), facingRotationMatrix), bendRotationMatrix);


            float height = (rand(pos.zyx) * 2 - 1) * _BladeHeightRandom + _BladeHeight;
            float width = (rand(pos.xzy) * 2 - 1) * _BladeWidthRandom + _BladeWidth;



            float3 p1 = float3(width, 0, 0);
            float3 p2 = float3(-width, 0, 0);
            float3 p3 = float3(0, 0, height);
            float3 normal = cross(p2 - p1, p3 - p1);
    
            triStream.Append(GeometryVertex(pos + mul(transformationMatrix, p1), float2(0, 0), normal));
            triStream.Append(GeometryVertex(pos + mul(transformationMatrix, p2), float2(1, 0), normal));
            triStream.Append(GeometryVertex(pos + mul(transformationMatrix, p3), float2(0.5, 1), normal));


        }

        ENDCG
        SubShader
        {
            Tags { "RenderType" = "Opaque" }

            Cull Off

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma geometry geo


                //struct appdata
                //{
                //    float4 vertex : POSITION;
                //    float2 uv : TEXCOORD0;
                //    float3 normal : NORMAL;
                //};

                struct v2f
                {
                    float2 uv : TEXCOORD0;
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

                vertexOutput vert(vertexInput v)
                {
                    vertexOutput o;

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
                    }

                    //o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                    //half3 worldNormal = UnityObjectToWorldNormal(v.normal);

                    //half nl = max(0,dot(worldNormal, _WorldSpaceLightPos0.xyz));
                    //o.diff = nl * _LightColor0.rgb;
                    //o.ambient = ShadeSH9(half4(worldNormal, 1));

                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;


                    return o;
                }

                fixed4 frag(GeometryOutput i) : SV_Target
                {
                    fixed4 col = _Color;

                    fixed3 lighting = i.diff * 0.5 + i.ambient + fixed3(0.5,0.5,0.5);
                    col.rgb *= lighting;

                    return col* lerp(_BottomColor, _TopColor, i.uv.y);;
                }
                ENDCG
            }
            UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

        }
}
