using UnityEngine;
using System.Collections;

public static class CreateCamera {

    public static Camera Execute(GameObject parentLight, RenderTexture rt)
    {
        GameObject cameraObject = new GameObject();
        cameraObject.name = "Light Camera";
        cameraObject.transform.SetParent(parentLight.transform);
        cameraObject.transform.localPosition = Vector3.zero;
        cameraObject.transform.localRotation = Quaternion.identity;

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.targetTexture = rt;
        camera.clearFlags = CameraClearFlags.Color;
        camera.backgroundColor = Color.black;
        camera.orthographic = true;

        camera.enabled = false;


        Shader.SetGlobalTexture("_ShadowDepthMap", rt);

        return camera;
    }

}
