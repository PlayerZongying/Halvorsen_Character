Shader "Unlit/Halvorsen"
{
    Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        // _Color ("Color (RGBA)", Color) = (1, 1, 1, 1) // add _Color property

        [HDR] _Color1("Color1", Color) = (1, 1, 1, 1)
        _Transparency1("Transparency1", Range(0, 1)) = 0
        [HDR]_Color2("Color2", Color) = (1, 1, 1, 1)
        _Transparency2("Transparency2", Range(0, 1)) = 0
        _HalvorsenParam("HalvorsenParam", Float) = 1.4
        _MaxSpeed("MaxSpeed", Float) = 75.0
        _Transparency("Transparency", Range(0, 1)) = 0.1
        // _PreLocalPos("PreLocalPos", Vector) = (0,0,0,0)
    }

    SubShader 
    {
        Tags {"RenderType"="Transparent"}
        ZWrite Off
        // Blend SrcAlpha DstAlpha
//        Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency
//        Blend SrcAlpha DstAlpha  // Traditional transparency use this
//        BlendOp Add// use this
        Blend SrcAlpha OneMinusSrcAlpha
//        BlendOp Add

        Cull Off 
        LOD 100

        Pass 
        {
            CGPROGRAM

            #pragma vertex vert alpha
            #pragma fragment frag alpha
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            // float4 _Color;
            float _HalvorsenParam;
            float4  _Color1;
            float4  _Color2;
            float  _Transparency1;
            float  _Transparency2;
            float _MaxSpeed;
            float _Transparency;
            float4x4 _Transform;
            float4 _PreLocalPos;

            struct appdata_t 
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f 
            {
                float4 vertex  : SV_POSITION;
                half2 texcoord : TEXCOORD0;
                float4 localPos : TEXCOORD1;
                // float4 prelocalPos : TEXCOORD2;
                // float speed : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            UNITY_INSTANCING_BUFFER_START(Props)
            // UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.localPos = mul(_Transform, worldPos);
                // float4 velocity = o.localPos - _PreLocalPos;
                // o.speed = length(velocity);
                // _PreLocalPos = o.localPos;
                v.texcoord.x = 1 - v.texcoord.x;
                o.texcoord   = TRANSFORM_TEX(v.texcoord, _MainTex);

                o.vertex = UnityObjectToClipPos(v.vertex);

                // float4x4 modelToWorld = unity_ObjectToWorld;
                // float4x4 worldToModel = unity_WorldToObject;

                // compute camera position in object space
                // float3 camPosObjSpace = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1)).xyz;

                // // compute direction to camera in object space
                // float3 toCamObjSpace = normalize(camPosObjSpace - v.vertex.xyz);

                // // compute rotation to face camera
                // float3 upVector = float3(0, 1, 0);
                // float3 rightVector = normalize(cross(upVector, toCamObjSpace));
                // float3 newUpVector = normalize(cross(toCamObjSpace, rightVector));
                // float3x3 rotation = float3x3(rightVector, newUpVector, toCamObjSpace);

                // // float3x3 rotation = float3x3(0,0,-1, 0,1,0, -1,0,0);

                // // // // apply rotation to vertex
                // // // o.vertex = mul(unity_ObjectToWorld, float4(mul(rotation, v.vertex.xyz), 1));
                // o.vertex = float4(mul(rotation, v.vertex.xyz), 1);
                // o.vertex = UnityObjectToClipPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                // float t = saturate(i.speed / _MaxSpeed);
                // fixed4 col = lerp(_Color1, _Color2, t); // multiply by _Color
                // col.a = (1 - i.texcoord.x) * (sin(i.texcoord.y * UNITY_PI) + 1) / 2 * _Transparency;
                // return col;

                float x = i.localPos.x;
                float y = i.localPos.y;
                float z = i.localPos.z;

                float dx = -1.0 * _HalvorsenParam * x - 4.0 * y - 4.0 * z - y * y;
                float dy = -1.0 * _HalvorsenParam * y - 4.0 * z - 4.0 * x - z * z;
                float dz = -1.0 * _HalvorsenParam * z - 4.0 * x - 4.0 * y - x * x;

                float speed = length(float3(dx, dy, dz));
                float t = saturate(speed / _MaxSpeed);

                float4 color1 = _Color1;
                color1.a = _Transparency1;

                float4 color2 = _Color2;
                color2.a = _Transparency2;

                float4 color;
                color = lerp(color1, color2, t);
                // float4 color1 = _Color1;
                // color1.a = _Transparency1;
                // color = lerp(_Color1, _Color2, t);

                color.a *= (1 - i.texcoord.x) * (sin(i.texcoord.y * UNITY_PI) + 1) / 2 * _Transparency;
                return color;
            }

            ENDCG
        }
    }
}
