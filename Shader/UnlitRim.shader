Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _RimColor("Rim Color", color) = (1.0, 0, 0, 1.0)
        _RimPower("Rim Power", Range(0.0001, 3.0)) = 1.0
        _RimIntensity("Rim Intensity", Range(0, 100.0)) = 1.0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"
                #include "Lighting.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normal : NORMAL;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 worldNormal : TEXCOORD1;
                    float3 worldViewDir : TEXCOORD2;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                fixed4 _RimColor;
                half _RimPower;
                float _RimIntensity;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    o.worldNormal = UnityObjectToWorldNormal(v.normal);
                    o.worldViewDir = WorldSpaceViewDir(v.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed3 worldNormal = normalize(i.worldNormal);
                    fixed3 worldViewDir = normalize(i.worldViewDir);

                    fixed4 diffuse = tex2D(_MainTex, i.uv);
                    // pow起衰减作用
                    fixed4 rimLight = _RimColor * pow(saturate(1.0 - dot(worldNormal, worldViewDir)), 1.0 / _RimPower) * _RimIntensity;
                    //fixed4 rimLight = _RimColor * saturate(1.0 - dot(worldNormal, worldViewDir)) * _RimIntensity;

                    fixed3 color = diffuse.rgb + rimLight;
                    return fixed4(color, 1.0);
                }
                ENDCG
            }
        }

}
