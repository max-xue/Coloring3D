using System.Collections;
using UnityEngine;
using Vuforia;

public class TrackingBehaviour_VP : BaseTrackingBehaviour
{
    ImageTargetBehaviour _targetBehaviour;
    Texture2D _texture;
   

    void Start()
    {
        _targetBehaviour = GameObject.Find("ImageTarget").GetComponent<ImageTargetBehaviour>(); //GetComponentInParent<ImageTargetBehaviour>();
        gameObject.layer = 31;
    }

    public override void OnTrackingFind(ColoringController controller)
    {
        base.OnTrackingFind(controller);
        
        StartCoroutine(GenTexture(Camera.main, new Rect(0, 0, Screen.width, Screen.height)));
    }

    private IEnumerator GenTexture(Camera camera, Rect rect)
    {
        //等待渲染线程结束  
        yield return new WaitForEndOfFrame();

        //重置坐标
        //this.transform.parent.localPosition = Vector3.zero;
        //this.transform.parent.localRotation = Quaternion.Euler(0, 0, 0);
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.Euler(0, 180, 0);

        //剔除UI层
        //camera.cullingMask &= ~(1 << 5);

        ////创建一个RenderTexture对象  
        //RenderTexture render = new RenderTexture((int)rect.width, (int)rect.height, 0);
        ////临时设置相关相机的targetTexture为render, 并手动渲染相关相机  
        //camera.targetTexture = render;
        //camera.Render();

        ////激活这个rt, 并从中中读取像素。  
        //RenderTexture.active = render;
        if (_texture)
            DestroyImmediate(_texture);

        _texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        //读取屏幕像素信息并存储为纹理数据  
        _texture.ReadPixels(rect, 0, 0);
        _texture.Apply();

        ////重置相关参数，以使用camera继续在屏幕上显示
        //camera.targetTexture = null;
        //RenderTexture.active = null; 
        //Destroy(render);

        //获取VP值
        Matrix4x4 P = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, false);
        Matrix4x4 V = Camera.main.worldToCameraMatrix;
        Matrix4x4 VP = P * V;

        //一半的大小是(0.5,0.5)
        //识别图大小是(512,512)
        //targetBehaviour.GetSize()是ImageTarget在Unity场景的中单位(Unity scene units)大小(1,1)
        //Vuforia默认的单位是米
        Vector2 halfSize = _targetBehaviour.GetSize() * 0.5f;
        //获取世界坐标，识别图的4个拐角世界坐标
        //图片(模型)坐标原点在中心点，因是二维的所以y为0,通过一半的大小参数得到4个角的局部坐标
        Vector3 targetAnglePoint1 = _controller.transform.TransformPoint(new Vector3(-halfSize.x, 0, halfSize.y));
        Vector3 targetAnglePoint2 = _controller.transform.TransformPoint(new Vector3(-halfSize.x, 0, -halfSize.y));
        Vector3 targetAnglePoint3 = _controller.transform.TransformPoint(new Vector3(halfSize.x, 0, halfSize.y));
        Vector3 targetAnglePoint4 = _controller.transform.TransformPoint(new Vector3(halfSize.x, 0, -halfSize.y));

        //设置Shader参数
        GetComponent<Renderer>().material.SetVector("_Uvpoint1", new Vector4(targetAnglePoint1.x, targetAnglePoint1.y, targetAnglePoint1.z, 1f));
        GetComponent<Renderer>().material.SetVector("_Uvpoint2", new Vector4(targetAnglePoint2.x, targetAnglePoint2.y, targetAnglePoint2.z, 1f));
        GetComponent<Renderer>().material.SetVector("_Uvpoint3", new Vector4(targetAnglePoint3.x, targetAnglePoint3.y, targetAnglePoint3.z, 1f));
        GetComponent<Renderer>().material.SetVector("_Uvpoint4", new Vector4(targetAnglePoint4.x, targetAnglePoint4.y, targetAnglePoint4.z, 1f));
        GetComponent<Renderer>().material.SetMatrix("_VP", VP);
        GetComponent<Renderer>().material.mainTexture = _texture;

        //恢复UI层
        //camera.cullingMask |= (1 << 5);
        //重置位置
        _controller.OnTrackingFound();
    }

    void OnDestroy()
    {
        if (_texture)
            DestroyImmediate(_texture);
    }
}

