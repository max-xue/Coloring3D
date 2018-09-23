using UnityEngine;
using Vuforia;

public class TrackingBehaviour : BaseTrackingBehaviour
{
    Camera cam;
    RenderTexture renderTexture;
    ImageTargetBehaviour targetBehaviour;

    void Start()
    {
        targetBehaviour = GameObject.Find("ImageTarget").GetComponent<ImageTargetBehaviour>(); //GetComponentInParent<ImageTargetBehaviour>();
        gameObject.layer = 31;
    }

    public override void OnTrackingFind(ColoringController controller)
    {
        base.OnTrackingFind(controller);
    }

    void Renderprepare()
    {
        if (!cam)
        {
            GameObject go = new GameObject("__cam");
            cam = go.AddComponent<Camera>();
            go.transform.parent = transform.parent;
            cam.hideFlags = HideFlags.HideAndDontSave;
        }
        cam.CopyFrom(Camera.main);
        cam.depth = 0;
        cam.cullingMask = 31;

        if (!renderTexture)
        {
            renderTexture = new RenderTexture(Screen.width, Screen.height, -50);
        }
        cam.targetTexture = renderTexture;
        cam.Render();
        GetComponent<Renderer>().material.SetTexture("_MainTex", renderTexture);

        _controller.OnTrackingFound();
    }

    void OnWillRenderObject()
    {
        if (!targetBehaviour || targetBehaviour.ImageTarget == null)
            return;
        //一半的大小是(0.5,0.3)
        //识别图大小是(512,326)
        //targetBehaviour.GetSize()是ImageTarget在Unity场景的中单位(Unity scene units)大小(1,0.6)
        //Vuforia默认的单位是米
        Vector2 halfSize = targetBehaviour.GetSize() * 0.5f;
        //获取世界坐标，识别图的4个拐角世界坐标
        //图片(模型)坐标原点在中心点，因是二维的所以y为0,通过一半的大小参数得到4个角的局部坐标
        Vector3 targetAnglePoint1 = _controller.transform.TransformPoint(new Vector3(-halfSize.x, 0, halfSize.y));
        Vector3 targetAnglePoint2 = _controller.transform.TransformPoint(new Vector3(-halfSize.x, 0, -halfSize.y));
        Vector3 targetAnglePoint3 = _controller.transform.TransformPoint(new Vector3(halfSize.x, 0, halfSize.y));
        Vector3 targetAnglePoint4 = _controller.transform.TransformPoint(new Vector3(halfSize.x, 0, -halfSize.y));
        Renderprepare();
        //设置Shader参数
        GetComponent<Renderer>().material.SetVector("_Uvpoint1", new Vector4(targetAnglePoint1.x, targetAnglePoint1.y, targetAnglePoint1.z, 1f));
        GetComponent<Renderer>().material.SetVector("_Uvpoint2", new Vector4(targetAnglePoint2.x, targetAnglePoint2.y, targetAnglePoint2.z, 1f));
        GetComponent<Renderer>().material.SetVector("_Uvpoint3", new Vector4(targetAnglePoint3.x, targetAnglePoint3.y, targetAnglePoint3.z, 1f));
        GetComponent<Renderer>().material.SetVector("_Uvpoint4", new Vector4(targetAnglePoint4.x, targetAnglePoint4.y, targetAnglePoint4.z, 1f));
    }

    void OnDestroy()
    {
        if (renderTexture)
            DestroyImmediate(renderTexture);
        if (cam)
            DestroyImmediate(cam.gameObject);
    }
}

