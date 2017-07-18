Shader "GOE/Scene/LightmapReflectReceiveShadowTransparent"
{
Properties
{
	_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_IllumFactor ("Illumin Factor", Range(1,2)) = 1
	_Alpha("Alpha", Range(0,1)) = 1
}

SubShader 
{
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" }
	UsePass "GOE/Scene/LightmapTransparent/TRANSPARENTPASS"
}
}



