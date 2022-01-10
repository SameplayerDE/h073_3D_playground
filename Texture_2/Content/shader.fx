#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;

float Delta;
float Total;

Texture2D Texture : register(t0);
sampler TextureSampler : register(s0)
{
	Texture = (Texture);
	MinFilter = Point; // Minification Filter
    MagFilter = Point;// Magnification Filter
    MipFilter = Linear; // Mip-mapping
	AddressU = Wrap; // Address Mode for U Coordinates
	AddressV = Wrap; // Address Mode for V Coordinates
};

Texture2D Effect : register(t1);
sampler EffectSampler : register(s1)
{
	Texture = (Effect);
	MinFilter = Point; // Minification Filter
    MagFilter = Point;// Magnification Filter
    MipFilter = Linear; // Mip-mapping
	AddressU = Wrap; // Address Mode for U Coordinates
	AddressV = Wrap; // Address Mode for V Coordinates
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TextureUVs : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TextureUVs : TEXCOORD0;
	float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	
	float4 position = input.Position;
	float2 textureUVs = input.TextureUVs;
	float4 color = float4(1, 1, 1, 1);

	output.Position = mul(position, WorldViewProjection);
	output.TextureUVs = textureUVs;
	output.Color = color;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 color = input.Color;
    float4 effectColor = input.Color;
    float2 textureUVs = input.TextureUVs;
    
    color *= tex2D(TextureSampler, textureUVs);
    
    float _RotationSpeed = 100 * Delta;
    textureUVs.xy -= 0.5;
    float sinX = sin ( _RotationSpeed * Total );
    float cosX = cos ( _RotationSpeed * Total );
    float sinY = sin ( _RotationSpeed * Total );
    float2x2 rotationMatrix = float2x2( cosX, -sinX, sinY, cosX);
    textureUVs.xy = mul ( textureUVs.xy, rotationMatrix );
    textureUVs.xy += 0.5;

    effectColor *= tex2D(EffectSampler, textureUVs) * float4(0, 1, 1, 1);
    
  
    if (color.r == 1 && color.g == 0 && color.b == 1)
    {
        color = float4(0, 1, 1, 1) * abs(sin(Total));
    }

    if (color.r == 0 && color.g == 0 && color.b == 1)
    {
        color = effectColor;
    }
    
	return color;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
		CullMode = NONE;
	}
};