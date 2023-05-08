Shader "Unlit/Fog"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
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
            // make fog work
            #pragma multi_compile_fog

            #define vec2 float2
            #define vec3 float3
            #define vec4 float4
            #define mix lerp
            #define fract frac
            #define iTime _Time.y
            #define iResolution _ScreenParams



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

            float rand(in vec2 sd) {

                return fract(sin(dot(sd.xy, vec2(9.128, 3.256) * 293699.963)));
            }


            float n2D(in vec2 sd) {

                vec2 iComp = floor(sd);
                //integer and fractional components
                vec2 fComp = fract(sd);


                float a = rand(iComp + vec2(0.0, 0.0));    //
                float b = rand(iComp + vec2(1.0, 0.0));    // interpolation points
                float c = rand(iComp + vec2(0.0, 1.0));    // (4 corners)
                float d = rand(iComp + vec2(1.0, 1.0));    //

                vec2 fac = smoothstep(0.0, 1.0, fComp);    //interpolation factor

                //Quad corners interpolation
                return
                    mix(a, b, fac.x) +

                    (c - a) * fac.y * (1.0 - fac.x) +

                    (d - b) * fac.x * fac.y;
            }

            //fractal Brownian Motion and Motion Pattern

            #define OCTAVES 6

            float fBM(in vec2 sd) {

                //init values
                float val = 0.0;
                float freq = 1.0;
                float amp = 0.5;

                float lacunarity = 2.0;
                float gain = 0.5;

                //Octaves iterations
                for (int i = 0; i < OCTAVES; i++) {

                    val += amp * n2D(sd * freq);

                    freq *= lacunarity;
                    amp *= gain;
                }

                return val;
            }


            float mp(in vec2 p) {

                float qx = fBM(p + vec2(0.0, 0.0));
                float qy = fBM(p + vec2(6.8, 2.4));

                vec2 q = vec2(qx, qy);

                float tm = 0.008 * iTime * 1.3;    //time factor

                float rx = fBM(p + 1.1 * q + vec2(9.5, 9.3) * tm);
                float ry = fBM(p + 1.5 * q + vec2(7.2, 1.5) * -(tm + 0.002));

                vec2 r = vec2(rx, ry);

                return fBM(p + (2.0 * r));
            }


            //========================================================================

            //main()


            void mainImage(out vec4 fragColor, in vec2 fragCoord)
            {
                //Normalized pixel coordinates (from 0 to 1)
                vec2 uv = fragCoord;// / iResolution.xy;

                vec3 col = vec3(0.0,0.0,0.0);
                //col += fBM(uv*3.0);

                float wFac = mp(uv * 3.0); //warping factor

                col = mix(vec3(0.101961, 0.29608, 0.26567), vec3(0.66667, 0.45667, 0.89839), clamp(pow(wFac, 2.5), 0.0, 1.0));
                col = mix(col, vec3(0.44467, 0.00567, 0.19809), clamp(pow(wFac, 0.4), 0.0, 1.0));
                col = mix(col, vec3(0.52467, 0.42567, 0.01809), clamp(wFac * wFac, 0.0, 1.0));
                col = mix(col, vec3(0.84467, 0.32567, 0.13809), clamp(smoothstep(0.0, 1.0, wFac), 0.0, 1.0));


                // Output to screen
                fragColor = vec4(col, 1.0);
            }


            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                vec4 col = vec4(0,0,0,0);
                mainImage(col, i.uv);
                return col;
            }
            ENDCG
        }
    }

}
