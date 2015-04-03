
precision lowp float;

uniform vec3 uResolution;

vec3 iResolution = uResolution;
float iGlobalTime = 0.0;


void sampleCamera(vec2 px, vec2 u, out vec3 rayOrigin, out vec3 rayDir)
{
	vec2 filmUv = (px + u)/iResolution.xy;

	float tx = (2.0*filmUv.x - 1.0)*(iResolution.x/iResolution.y);
	float ty = (1.0 - 2.0*filmUv.y);
	float tz = 0.0;

	rayOrigin = vec3(0.0, 0.0, 5.0);
	rayDir = normalize(vec3(tx, ty, tz) - rayOrigin);
}

float Q(float a, float b, float c)
{
	float d = b*b-4.0*a*c;
	if (d < 0.0) return -1.0;
	d=sqrt(d);	
	float oo2a = 0.5/a;
	return min((-b-d)*oo2a,(-b+d)*oo2a);
}


vec3 L = normalize(vec3(-2,-1,1));

float Lit(vec3 N, vec3 V, vec3 H)
{
	float d = max(dot(N,L),0.0) * 0.5;
	float s = pow(max(dot(N,H),0.0),1000.0)*8.0;
	
	return d+s;
}

vec3 trace(vec3 P, vec3 V, vec3 H, float time)
{
	time += iGlobalTime;
	float t2 = sin(time*.1)*40.0;
	time += sin(time)*0.9;	
	vec3 c=vec3(0.2,0.2,0.1);

#define N_SPHERE	1
	
	vec4 S[N_SPHERE];
	vec3 C[N_SPHERE];
	vec2 gb=vec2(0.1,0);
	
	for (int i=0; i<N_SPHERE; i++)
	{
		float I = float(i)*(1.0/float(N_SPHERE));
		float t = I*2.0*3.1415927 + time;
		vec3 A = vec3(sin(t+t2),sin(t*3.0)*0.4,cos(t+t2));
		float R = (1.0-I)*0.33 + 0.1;
		S[i]=vec4(A,R);
		
		C[i]=vec3(0,gb);
		gb=vec2(0.1,0.1)-gb;

	}
		
	float nearest = 1e10;	
	vec3 E=V;
	
	for (int i=0; i<N_SPHERE; i++)
	{
		vec3 A=S[i].xyz;
		float R=S[i].w;
		float T=Q(dot(V,V),2.0*(dot(P,V)-(dot(A,V))),dot(A,A)+dot(P,P)-R*R-(2.0*(dot(A,P))));		
		if (T > 0.0)
		{			
			if (T < nearest)
			{
				vec3 X=P+T*V;
				vec3 N=normalize(X-A);
				
				vec3 Ref = reflect(V,N);
				
				float nearestR = 1e10;
				vec3 Rcol=vec3(0.0,0.0,0.0);
				int blocked =0;
								
				float b = blocked > 0 ? 0.0 : Lit(N,V,H); //d+s;
				nearest = T;
								
				c = vec3(b,b,b)+C[i]+Rcol;
			}
		}		
	}
	
	return c;
}

//void mainImage( out vec4 fragColor, in vec2 fragCoord )
void main(void)
{

	vec2 fragCoord = gl_FragCoord.xy;

	vec3 P, V;
	sampleCamera(fragCoord,vec2(0.5,0.5), P, V);

	vec3 H=normalize(L-V);				

	float t = iGlobalTime;
	vec3 c = trace(P,V,H, t);

	//vec3 c = vec3(0.5, 1.0, 0.0);	
	gl_FragColor = vec4(c,1.0);
}
