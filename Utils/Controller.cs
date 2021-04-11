using ShadowMap;
using UnityEngine;

class Controller : MonoBehaviour
{
    public bool createTreeToggle;
    public Material ShadowMaterial;
    public Texture tex;
    public GameObject sceneAABB;
    public Light sceneLight;
    public FrustumType frustumType;

    // For Render
    public RenderTexture depthShadowMap;
    public Shader depthCaptureShader;

    void Start()
    {
        if (sceneLight != null)
        {
            var lightController = sceneLight.GetComponent<BasicShadowMap>();
            lightController.Init();

            Execute();
        }
    }

    void Update()
    {
        
    }

    private void Execute()
    {

    }
}

