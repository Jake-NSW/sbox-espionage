//=========================================================================================================================
// Optional
//=========================================================================================================================
HEADER
{
	Description = "Template Shader for S&box";
}

MODES
{
    Default();
    VrForward();
}


//=========================================================================================================================
// Optional
//=========================================================================================================================
FEATURES
{

}

//=========================================================================================================================
COMMON
{
	#include "postprocess/shared.hlsl"
}

//=========================================================================================================================

struct VertexInput
{
	float3 vPositionOs : POSITION < Semantic( PosXyz ); >;
    float2 vTexCoord : TEXCOORD0 < Semantic( LowPrecisionUv ); >;
};
//=========================================================================================================================

struct PixelInput
{
    float2 vTexCoord : TEXCOORD0;

	// VS only
	#if ( PROGRAM == VFX_PROGRAM_VS )
		float4 vPositionPs		: SV_Position;
	#endif

	// PS only
	#if ( ( PROGRAM == VFX_PROGRAM_PS ) )
		float4 vPositionSs		: SV_ScreenPosition;
	#endif
};

//=========================================================================================================================

VS
{
	    PixelInput MainVs( VertexInput i )
    {
        PixelInput o;
        
        o.vPositionPs = float4(i.vPositionOs.xy, 0.0f, 1.0f);
        o.vTexCoord = i.vTexCoord;
        return o;
    }
}

//=========================================================================================================================



PS
{
    #include "postprocess/common.hlsl"
    #include "postprocess/functions.hlsl"
    #include "common/proceedural.hlsl"

	RenderState( DepthWriteEnable, false );
    RenderState( DepthEnable, false );

    CreateTexture2D( g_tColorBuffer ) < Attribute( "ColorBuffer" );  	SrgbRead( true ); Filter( MIN_MAG_LINEAR_MIP_POINT ); AddressU( CLAMP ); AddressV( BORDER ); >;
    CreateTexture2D( g_tDepthBuffer ) < Attribute( "DepthBuffer" ); 	SrgbRead( false ); Filter( MIN_MAG_MIP_POINT ); AddressU( CLAMP ); AddressV( BORDER ); >;

	float3 FetchSceneColor( float2 vScreenUv )
    {
       return Tex2D( g_tColorBuffer, vScreenUv.xy ).rgb;
    }

	float intensity < Default(0.0); Range(0.0,50.0); >;
	float intensityX < Default(0.0); Range(0.0,1.0);>;
	float intensityY < Default(0.0); Range(0.0,1.0);>;
	float centerX <Default(0.5); Range(0.0,1.0);>;
	float centerY < Default(0.5); Range(0.0,1.0);>;
	float scale < Default(1.0); Range(0.0,1.0);>;
	float flBlurSize< Default(0.0); Range(0.0,2.0); >;
	float flBloomThreshold< Default(0.0); Range(0.0, 1.0); >;
	float flBloomSoftThreshold< Default(0.0); Range(0.0, 1.0); >;
	float flBloomIntensity< Default(0.0); Range(0.0, 3.0); >;


	float2 Distort(float2 uv)
	{
		float amount = 1.6f * max(abs(intensity), 1.0f);
		float theta = radians(min(160.0f, amount));
		float sigma = 2.0f * tan(theta * 0.5f);
		//center scale
		float4 p0 = float4(centerX, centerY, max(intensityX, 0.0001), max(intensityY, 0.0001));
		//amount
		float4 p1 = float4(intensity >= 0.0 ? theta : 1.0 / theta, sigma, 1.0 / scale, intensity);


        uv = (uv - 0.5) * p1.z + 0.5;
        float2 ruv = p0.zw * (uv - 0.5 - p0.xy);
        float ru = length(float2(ruv));
        if (p1.w > 0.0)
        {
            float wu = ru * p1.x;
            ru = tan(wu) * (1.0 / (ru * p1.y));
            uv = uv + ruv * (ru - 1.0);
        }
        else
        {
            ru = (1.0 / ru) * p1.x * atan(ru * p1.y);
            uv = uv + ruv * (ru - 1.0);
        }
    	return uv;
	}

	float3 PreFilter (float3 c) {
			half brightness = max(c.r, max(c.g, c.b));
			half knee = flBloomThreshold * flBloomSoftThreshold;
			half soft = brightness - flBloomThreshold + knee;
			soft = clamp(soft, 0, 2 * knee);
			soft = soft * soft / (4 * knee + 0.00001);
			half contribution = max(soft, brightness - flBloomThreshold);
			contribution /= max(brightness, 0.00001);
			return c * contribution;
	}

	float3 GaussianBlurEx( float3 vColor, float2 vTexCoords )
    {
        float flRemappedBlurSize = flBlurSize;

        float fl2PI = 6.28318530718f;
        float flDirections = 16.0f;
        float flQuality = 4.0f;
        float flTaps = 1.0f;

        [unroll]
        for( float d=0.0; d<fl2PI; d+=fl2PI/flDirections)
        {
            [unroll]
            for(float j=1.0/flQuality; j<=1.0; j+=1.0/flQuality)
            {
                flTaps += 1;
                vColor += PreFilter(FetchSceneColor( vTexCoords + float2( cos(d), sin(d) ) * lerp(0.0f, 0.02, flRemappedBlurSize) * j ));    
            }
        }
        return vColor / flTaps;
    }

	

	//Bloom function
	float4 Bloom(float2 uv, float3 c)
	{
		float3 sceneColor = PreFilter(c);
		//Blur the image
		float3 color = GaussianBlurEx(sceneColor , uv );
		
		//Return the color
		return float4( color, 1.0f );
	}

	//Scanlines
	float4 Scanlines(float uv, float resolution, float opacity)
    {
        float intensity = sin(uv * resolution * PI * 2.0);
        intensity = ((0.5 * intensity) + 0.5) * 0.9 + 0.1;
        float po = pow(intensity, opacity);
        return float4(po,po,po, 1.0);
    }

	float4 ScanBreak(float uv){
		float t = g_flTime * 0.5;
		float b = sin(uv * 4.0 + t);

		//Clamp between 0 and 1
		b = saturate(b);

		//Add 0.1
		b += 0.95;

		//Clamp between 0 and 1
		b = saturate(b);
		

		return float4(b , b , b, 1.0);
		
	}


	static float PI = 3.1415926535;

	//
	// Main
	//
	float4 MainPs( PixelInput i ) : SV_Target0
    {
        float2 vScreenUv = i.vPositionSs.xy / g_vRenderTargetSize;

		
		float2 distortedUV = Distort(vScreenUv);
        float3 vFinalColor = FetchSceneColor( distortedUV).rgb;
		float3 bloom = Bloom(distortedUV, vFinalColor).rgb;

		float t = g_flTime;
		float bloomIntensityMod =  Simplex2D(float2(0.0,t * 2.0));
		//Remap to 0.0 - 1.0
		bloomIntensityMod = (bloomIntensityMod + 1.0) * 0.5;
		bloomIntensityMod *= 0.1;
		
		vFinalColor += bloom * (flBloomIntensity + bloomIntensityMod);
		vFinalColor = Scanlines(distortedUV.y, 256.0, 0.05).rgb * vFinalColor;
		vFinalColor = Scanlines(distortedUV.x, 256.0, 0.05).rgb * vFinalColor;
		vFinalColor = ScanBreak(distortedUV.y).rgb * vFinalColor;




        return float4( vFinalColor, 1.0f );
    }
}
