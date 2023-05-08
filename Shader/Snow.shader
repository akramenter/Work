Shader "Custom/Snow"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0


        _Bump("Bump", 2D) = "bump" {}

        //积雪
        _Snow("Snow Level", Range(0,1)) = 0
        _SnowColor("Snow Color", Color) = (1.0,1.0,1.0,1.0)
        _SnowDirection("Snow Direction", Vector) = (0,1,0)
        _SnowDepth("Snow Depth", Range(0, 0.3)) = 0.1
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard fullforwardshadows

             // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

            sampler2D _MainTex;
            sampler2D _Bump;

             float _Snow;
            float4 _SnowColor;
            float4 _SnowDirection;
            float _SnowDepth;

            struct Input
            {
                float2 uv_MainTex;
                float2 uv_Bump;

                float3 worldNormal;
                INTERNAL_DATA
            };
            inline float4 LightingCustomDiffuse(SurfaceOutput s, fixed3 lightDir, fixed atten)
            {
                float difLight = max(0, dot(s.Normal, lightDir));
                float hLambert = difLight * 0.5 + 0.5;
                float4 col;
                col.rgb = s.Albedo * _LightColor0.rgb * (difLight * atten * 2);
                col.a = s.Alpha;
                return col;
            }
            void vert(inout appdata_full v)
            {
                float4 sn = mul(transpose(unity_ObjectToWorld), _SnowDirection);
                if (dot(v.normal, sn.xyz) >= lerp(1, -1, (_Snow * 2) / 3))
                {
                    v.vertex.xyz += (sn.xyz + v.normal) * _SnowDepth * _Snow;
                }
            }
            half _Glossiness;
            half _Metallic;
            fixed4 _Color;

            // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
                // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                // Albedo comes from a texture tinted by color
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Normal = UnpackNormal(tex2D(_Bump, IN.uv_Bump));

                if (dot(WorldNormalVector(IN, o.Normal), _SnowDirection.xyz) > lerp(1, -1, _Snow))
                {
                    o.Albedo = _SnowColor.rgb;
                }
                else
                {
                    o.Albedo = c.rgb;
                }
                // Metallic and smoothness come from slider variables
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                o.Alpha = c.a;
            }
            ENDCG
        }
            FallBack "Diffuse"
}
