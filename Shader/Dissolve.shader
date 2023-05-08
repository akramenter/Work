Shader "Custom/Dissolve"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _DissolveTex("DissolveTex",2D) = "" {}
        _Clip("Clip",Range(0,1)) = 0
        _RampColor("RampColor",Color) = (0,0,0,0)
        [Toggle]_DissolveEffect("DissolveEffect",float) = 0
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
                #pragma multi_compile _ _DISSOLVEEFFECT_ON



                #include "UnityCG.cginc"

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
                sampler2D _DissolveTex;
                float4 _DissolveTex_ST;
                float _Clip;
                float4 _RampColor;
                float _DissolveEffect;


                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    //clip()函数，Clip(x)  x<=0 裁掉像素。
                    // sample the texture
                    fixed4 col = tex2D(_MainTex, i.uv);
                #if _DISSOLVEEFFECT_ON
                    i.uv = TRANSFORM_TEX(i.uv,_DissolveTex);
                    //i.uv = i.uv * _DissolveTex_ST.xy + _DissolveTex_ST.zw; 上面那句代码相当于这一行代码的简写
                    fixed4 dissolveCol = tex2D(_DissolveTex,i.uv);
                    clip(dissolveCol.r - _Clip);

                    fixed maxEdge = _Clip + 0.1; //（_Clip,_Clip + 0.1）范围加边缘色
                    if (dissolveCol.r < maxEdge)
                    {
                        col += _RampColor * smoothstep(_Clip,maxEdge,dissolveCol.r);
                    }
                #endif

                    return col;
                }
                ENDCG
            }
        }


}
