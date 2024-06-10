#version 330 core
#define EPSILON 0.001
#define BIG 1000000.0
const int DIFFUSE = 1;
const int REFLECTION = 2;
const int REFRACTION = 3;

const int DIFFUSE_REFLECTION = 1;
const int MIRROR_REFLECTION = 2;
const int CASE_REFRACTION = 3;

uniform vec3 pos_light;

struct SSphere
{
    vec3 Center;
    float Radius;
    int MaterialIdx;
};

struct STriangle
{
    vec3 v1;
    vec3 v2;
    vec3 v3;
    int MaterialIdx;
};

struct SCamera {
    vec3 Position;
    vec3 View;
    vec3 Up;
    vec3 Side;
    vec2 Scale;
};

struct SRay {
    vec3 Origin;//pozition start 
    vec3 Direction;//napravlenie
};

struct SIntersection
{
    float Time;
    vec3 Point;
    vec3 Normal;
    vec3 Color;
    // ambient, diffuse and specular coeffs
    vec4 LightCoeffs;//osvechennost
    // 0 - non-reflection, 1 - mirror
    float ReflectionCoef;//coefficient otrachenie
    float RefractionCoef; //coefficient prelomlenie
    int MaterialType;
};

struct SLight
{
    vec3 Position;
};

struct SMaterial
{
    //diffuse color
    vec3 Color;

    // ambient, diffuse and specular coeffs
    vec4 LightCoeffs;
    // 0 - non-reflection, 1 - mirror
    float ReflectionCoef;
    float RefractionCoef;
    int MaterialType;
};

struct STracingRay
{
    SRay ray;
    float contribution;//
    int depth;//glubina
};


out vec4 outputColor;
in vec3 glPosition;////
SCamera uCamera;
STriangle triangles[10];
SSphere spheres[2];
SLight light;
SMaterial materials[7];

SRay GenerateRay( SCamera uCamera )
{
    vec2 coords = glPosition.xy * uCamera.Scale;
    vec3 direction = uCamera.View + uCamera.Side * coords.x + uCamera.Up * coords.y;//napravlenie lucha
    return SRay( uCamera.Position, normalize(direction) );
}



SCamera initializeDefaultCamera()
{
    SCamera camera;
    camera.Position = vec3(0.0, 0.0, -8.0);
    camera.View = vec3(0.0, 0.0, 1.0);
    camera.Up = vec3(0.0, 1.0, 0.0);
    camera.Side = vec3(1.0, 0.0, 0.0);
    camera.Scale = vec2(1.0);//otnoshenie x/y
    return camera;
}

void initializeDefaultScene(out STriangle triangles[10], out SSphere spheres[2])
{
    /* left wall */
    triangles[0].v1 = vec3(-5.0,-5.0,-5.0);
    triangles[0].v2 = vec3(-5.0, 5.0, 5.0);
    triangles[0].v3 = vec3(-5.0, 5.0,-5.0);
    triangles[0].MaterialIdx = 3;

    triangles[1].v1 = vec3(-5.0,-5.0,-5.0);
    triangles[1].v2 = vec3(-5.0,-5.0, 5.0);
    triangles[1].v3 = vec3(-5.0, 5.0, 5.0);
    triangles[1].MaterialIdx = 3;

    /* right wall */
    triangles[2].v1 = vec3(5.0, -5.0, -5.0);
    triangles[2].v2 = vec3(5.0, 5.0, -5.0);
    triangles[2].v3 = vec3(5.0, 5.0, 5.0);
    triangles[2].MaterialIdx = 4;

    triangles[3].v1 = vec3(5.0, 5.0, 5.0);
    triangles[3].v2 = vec3(5.0, -5.0, 5.0);
    triangles[3].v3 = vec3(5.0,-5.0, -5.0);
    triangles[3].MaterialIdx = 4;             

    /* back wall */
    triangles[4].v1 = vec3(-5.0,-5.0, 5.0);
    triangles[4].v2 = vec3( 5.0,-5.0, 5.0);
    triangles[4].v3 = vec3(-5.0, 5.0, 5.0);
    triangles[4].MaterialIdx = 5;

    triangles[5].v1 = vec3( 5.0, 5.0, 5.0);
    triangles[5].v2 = vec3(-5.0, 5.0, 5.0);
    triangles[5].v3 = vec3( 5.0,-5.0, 5.0);
    triangles[5].MaterialIdx = 5;

    /* top */
    triangles[6].v1 = vec3(-5.0, 5.0, -5.0);
    triangles[6].v2 = vec3(5.0, 5.0, -5.0);
    triangles[6].v3 = vec3(-5.0, 5.0, 5.0);
    triangles[6].MaterialIdx = 6;

    triangles[7].v1 = vec3(5.0, 5.0, 5.0);
    triangles[7].v2 = vec3(-5.0, 5.0, 5.0);
    triangles[7].v3 = vec3(5.0, 5.0, -5.0);
    triangles[7].MaterialIdx = 6;

    /* floor */ 
    //Left niz triangles
    triangles[8].v1 = vec3(-5.0, -5.0, -5.0);
    triangles[8].v2 = vec3(5.0, -5.0, -5.0);
    triangles[8].v3 = vec3(-5.0, -5.0, 5.0);
    triangles[8].MaterialIdx = 0;

    //Right niz triangles
    triangles[9].v1 = vec3(5.0, -5.0, 5.0);
    triangles[9].v2 = vec3(-5.0, -5.0, 5.0);
    triangles[9].v3 = vec3(5.0, -5.0, -5.0);
    triangles[9].MaterialIdx = 0;

    /* spheres */
    spheres[0].Center = vec3(-1.0,-1.5,-1.0);//
    spheres[0].Radius = 2.0;
    spheres[0].MaterialIdx = 1;

    spheres[1].Center = vec3(2.0,1.0,2.0);
    spheres[1].Radius = 1.0;
    spheres[1].MaterialIdx = 0;
}


////
bool IntersectSphere ( SSphere sphere, SRay ray, float start, float final, out float time )
{
    ray.Origin -= sphere.Center;// odna sistema koordinat (sphera)
    float A = dot ( ray.Direction, ray.Direction );
    float B = dot ( ray.Direction, ray.Origin );
    float C = dot ( ray.Origin, ray.Origin ) - sphere.Radius * sphere.Radius;
    float D = B * B - A * C;
    if ( D > 0.0 )
    {
        D = sqrt ( D );
        float t1 = ( -B - D ) / A;
        float t2 = ( -B + D ) / A;
        if(t1 < 0 && t2 < 0)
            return false;

        if(min(t1, t2) < 0)
        {
            time = max(t1,t2);
            return true;
        }
        time = min(t1, t2);
        return true;
    }
    return false;
}
////

bool IntersectTriangle (SRay ray, vec3 v1, vec3 v2, vec3 v3, out float time )
{
    time = -1;
    vec3 A = v2 - v1;
    vec3 B = v3 - v1;

    vec3 N = cross(A, B);

    float NdotRayDirection = dot(N, ray.Direction);
    if (abs(NdotRayDirection) < 0.001)
        return false;
    
    float d = dot(N, v1);
    float t = -(dot(N, ray.Origin) - d) / NdotRayDirection;

    if (t < 0)
        return false;

    vec3 P = ray.Origin + t * ray.Direction;
    vec3 C;

    vec3 edge1 = v2 - v1;
    vec3 VP1 = P - v1;
    C = cross(edge1, VP1);

    if (dot(N, C) < 0)
        return false;

    vec3 edge2 = v3 - v2;
    vec3 VP2 = P - v2;
    C = cross(edge2, VP2);
    if (dot(N, C) < 0)
        return false;

    vec3 edge3 = v1 - v3;
    vec3 VP3 = P - v3;
    C = cross(edge3, VP3);
    if (dot(N, C) < 0)
        return false;

    time = t;
    return true;
}

void initializeDefaultLightMaterials(out SMaterial materials[7])
{
    light.Position = pos_light;
    vec4 lightCoefs = vec4(0.4, 0.5, 0.0, 512.0);
    
    // Spheres material (mirror)
    materials[1].Color = vec3(0.1, 0.1, 0.1);
    materials[1].LightCoeffs = vec4(lightCoefs);
    materials[1].ReflectionCoef = 0.8;
    materials[1].RefractionCoef = 1;
    materials[1].MaterialType = REFLECTION;

    ///Spheres material (transparent);
    materials[2].LightCoeffs = vec4(lightCoefs);
    materials[2].ReflectionCoef = 0.1;
    materials[2].RefractionCoef = 0.5;
    materials[2].MaterialType = REFRACTION;

    // Up and right triangles
    materials[0].Color = vec3(0.5, 0.6, 0.8);
    materials[0].LightCoeffs = vec4(lightCoefs);
    materials[0].ReflectionCoef = 0;
    materials[0].RefractionCoef = 1;
    materials[0].MaterialType = DIFFUSE;

    materials[3].Color = vec3(2.0, 1.0, 1.0);
    materials[3].LightCoeffs = vec4(lightCoefs);
    materials[3].ReflectionCoef = 0.5;
    materials[3].RefractionCoef = 1.0;
    materials[3].MaterialType = DIFFUSE;

    materials[4].Color = vec3(0.4, 1.0, 0.4);
    materials[4].LightCoeffs = vec4(lightCoefs);
    materials[4].ReflectionCoef = 0.4;
    materials[4].RefractionCoef = 1;
    materials[4].MaterialType = DIFFUSE;

    materials[5].Color = vec3(0.9, 1.0, 1.0);
    materials[5].LightCoeffs = vec4(lightCoefs);
    materials[5].ReflectionCoef = 0;
    materials[5].RefractionCoef = 1;
    materials[5].MaterialType = DIFFUSE;

    materials[6].Color = vec3(1.0, 1.0, 0.0);
    materials[6].LightCoeffs = vec4(lightCoefs);
    materials[6].ReflectionCoef = 0;
    materials[6].RefractionCoef = 1;
    materials[6].MaterialType = DIFFUSE;

}

////
vec3 Phong(SIntersection intersect, SLight currLight, float shadowFactor)
{
    vec3 light = normalize ( currLight.Position - intersect.Point );
    float diffuse = max(dot(light, intersect.Normal), 0.0);//diffuse consecration. 
    //This is the scalar product of the normalized light vector and the normal at the intersection point.

    vec3 view = normalize(uCamera.Position - intersect.Point);
    //The normalized vector from the intersection point to the camera is calculated.

    vec3 reflected= reflect( -view, intersect.Normal );
    //The reflection vector is calculated relative to the normal at the intersection point.

    float specular = pow(max(dot(reflected, light), 0.0), intersect.LightCoeffs.w);
    //The specular component of lighting is calculated. This is the scalar 
    //product of the normalized reflection vector and the normalized light vector, raised to the power specified in intersect.LightCoeffs.w (gloss coefficient).

    vec3 ambient = intersect.LightCoeffs.x * intersect.Color;
    vec3 diffuseComponent = intersect.LightCoeffs.y * diffuse * intersect.Color * shadowFactor;
    vec3 specularComponent = intersect.LightCoeffs.z * specular * intersect.Color * vec3(1.0,1.0,1.0);

    return ambient + diffuseComponent + specularComponent;
}
////

bool Raytrace_func(SRay ray, SSphere spheres[2], STriangle triangles[10], SMaterial materials[7], float start, float final, inout SIntersection intersect)
{
    bool result = false;
    float test = start;
    intersect.Time = final;

    SSphere sphere = spheres[0];
    if( IntersectSphere (sphere, ray, start, final, test ) && test < intersect.Time )
    {
        intersect.Time = test;
        intersect.Point = ray.Origin + ray.Direction * test;
        intersect.Normal = normalize ( intersect.Point - spheres[0].Center );

        intersect.Color = materials[2].Color;
        intersect.LightCoeffs = materials[2].LightCoeffs;
        intersect.ReflectionCoef = materials[2].ReflectionCoef;
        intersect.RefractionCoef = materials[2].RefractionCoef;
        intersect.MaterialType = materials[2].MaterialType;
        result = true;
    }

    sphere = spheres[1];
    if( IntersectSphere (sphere, ray, start, final, test ) && test < intersect.Time )
    {
        intersect.Time = test;
        intersect.Point = ray.Origin + ray.Direction * test;
        intersect.Normal = normalize ( intersect.Point - spheres[1].Center );
        intersect.Color = materials[1].Color;
        intersect.LightCoeffs = materials[1].LightCoeffs;
        intersect.ReflectionCoef = materials[1].ReflectionCoef;
        intersect.RefractionCoef = materials[1].RefractionCoef;
        intersect.MaterialType = materials[1].MaterialType;
        result = true;
    }

    //calculate intersect with triangles
    for(int i = 0; i < 10; i++)
    {   
        STriangle triangle = triangles[i];
        if(IntersectTriangle(ray, triangle.v1, triangle.v2, triangle.v3, test) && test < intersect.Time)
        {
            int nymMaterial = triangles[i].MaterialIdx;
            intersect.Time = test;
            intersect.Point = ray.Origin + ray.Direction * test;
            intersect.Normal = normalize(cross(triangle.v1 - triangle.v2, triangle.v3 - triangle.v2));
            intersect.Color = materials[nymMaterial].Color;
            intersect.LightCoeffs = materials[nymMaterial].LightCoeffs;
            intersect.ReflectionCoef = materials[nymMaterial].ReflectionCoef;
            intersect.RefractionCoef = materials[nymMaterial].RefractionCoef;
            intersect.MaterialType = materials[nymMaterial].MaterialType;
            result = true;
        }
    }
    return result;
}

float Shadow(SLight currLight, SIntersection intersect)
{
    float shadowing = 1.0;

    //normalize vector ot istochnika do point intersect
    vec3 direction = normalize(currLight.Position - intersect.Point);

    //distance ot istochnika do point intersect
    float distanceLight = distance(currLight.Position, intersect.Point);

    SRay shadowRay;
    shadowRay.Origin = intersect.Point + direction * EPSILON;
    shadowRay.Direction = direction;

    SIntersection shadowIntersect;
    shadowIntersect.Time = BIG;

    bool raytr_res = Raytrace_func(shadowRay, spheres, triangles, materials, 0.0f, distanceLight, shadowIntersect);
    if(raytr_res)
    {
        shadowing = 0.0;
    }
    return shadowing;
}

// STACK
const int MAX_STACK_SIZE = 100;
const int MAX_TRACE_DEPTH = 8;
STracingRay stack[MAX_STACK_SIZE];
int stack_size = 0;

bool pushRay(STracingRay newRay)
{
    if(stack_size < MAX_STACK_SIZE - 1 && newRay.depth < MAX_TRACE_DEPTH)
    {
        stack[stack_size] = newRay;
        stack_size++;
        return true;
    }
    return false;
}

bool isEmpty()
{
    return (stack_size < 0);
}

STracingRay popRay()
{
    stack_size--;
    return stack[stack_size];
}

void main()
{
    float start = 0;
    float final = BIG;

    uCamera = initializeDefaultCamera();
    SRay ray = GenerateRay( uCamera );
    vec3 resultColor = vec3(0.0,0.0,0.0);
    SIntersection intersect;
    intersect.Time = BIG;

    initializeDefaultScene(triangles, spheres);
    initializeDefaultLightMaterials(materials);

    STracingRay trRay = STracingRay(ray, 1, 0);
    pushRay(trRay);
    while(!isEmpty())
    {
        STracingRay trRay = popRay();
        ray = trRay.ray;
        SIntersection intersect;
        intersect.Time = BIG;
        start = 0;
        final = BIG;
        
        if (Raytrace_func(ray, spheres, triangles, materials, start, final, intersect))
        {
            switch(intersect.MaterialType)
            {
            case DIFFUSE_REFLECTION:
            {
                float shadowing = Shadow(light, intersect);
                resultColor += trRay.contribution * Phong ( intersect, light, shadowing );
                break;
            }
            case MIRROR_REFLECTION:
            {
                if(intersect.ReflectionCoef < 1)
                {
                    float contribution = trRay.contribution * (1 - intersect.ReflectionCoef);
                    float shadowing = Shadow(light, intersect);
                    resultColor += contribution * Phong(intersect, light, shadowing);
                }
                vec3 reflectDirection = reflect(ray.Direction, intersect.Normal);
                // creare reflection ray
                float contribution = trRay.contribution * intersect.ReflectionCoef;
                STracingRay reflectRay = STracingRay(
                SRay(intersect.Point + reflectDirection * EPSILON, reflectDirection),
                contribution, trRay.depth + 1);
                pushRay(reflectRay);
                break;
            }
            case CASE_REFRACTION:
            {      
                float ior;
                vec3 refractDirection;


                 if (dot(ray.Direction, intersect.Normal) < 0.0)
                 {
                 refractDirection = refract(ray.Direction, intersect.Normal,1/1.3);
                 ior = 0.1;
                 }
                 else
                 {
                  refractDirection = -refract(ray.Direction, intersect.Normal, 1.3);
                  ior = 0.7;
                 }

                 //vec3 refractionDirection = normalize(refract(ray.Direction, intersect.Normal * sign, ior));
                 STracingRay refractRay = STracingRay(SRay(intersect.Point + EPSILON * refractDirection, refractDirection), ior, trRay.depth + 1);
                 pushRay(refractRay);
                 break;
                 }
                }
              }
            }

    outputColor = vec4(resultColor, 1.0);
}
