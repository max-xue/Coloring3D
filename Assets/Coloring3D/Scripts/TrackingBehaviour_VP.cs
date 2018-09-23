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
        //�ȴ���Ⱦ�߳̽���  
        yield return new WaitForEndOfFrame();

        //��������
        //this.transform.parent.localPosition = Vector3.zero;
        //this.transform.parent.localRotation = Quaternion.Euler(0, 0, 0);
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.Euler(0, 180, 0);

        //�޳�UI��
        //camera.cullingMask &= ~(1 << 5);

        ////����һ��RenderTexture����  
        //RenderTexture render = new RenderTexture((int)rect.width, (int)rect.height, 0);
        ////��ʱ������������targetTextureΪrender, ���ֶ���Ⱦ������  
        //camera.targetTexture = render;
        //camera.Render();

        ////�������rt, �������ж�ȡ���ء�  
        //RenderTexture.active = render;
        if (_texture)
            DestroyImmediate(_texture);

        _texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        //��ȡ��Ļ������Ϣ���洢Ϊ��������  
        _texture.ReadPixels(rect, 0, 0);
        _texture.Apply();

        ////������ز�������ʹ��camera��������Ļ����ʾ
        //camera.targetTexture = null;
        //RenderTexture.active = null; 
        //Destroy(render);

        //��ȡVPֵ
        Matrix4x4 P = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, false);
        Matrix4x4 V = Camera.main.worldToCameraMatrix;
        Matrix4x4 VP = P * V;

        //һ��Ĵ�С��(0.5,0.5)
        //ʶ��ͼ��С��(512,512)
        //targetBehaviour.GetSize()��ImageTarget��Unity�������е�λ(Unity scene units)��С(1,1)
        //VuforiaĬ�ϵĵ�λ����
        Vector2 halfSize = _targetBehaviour.GetSize() * 0.5f;
        //��ȡ�������꣬ʶ��ͼ��4���ս���������
        //ͼƬ(ģ��)����ԭ�������ĵ㣬���Ƕ�ά������yΪ0,ͨ��һ��Ĵ�С�����õ�4���ǵľֲ�����
        Vector3 targetAnglePoint1 = _controller.transform.TransformPoint(new Vector3(-halfSize.x, 0, halfSize.y));
        Vector3 targetAnglePoint2 = _controller.transform.TransformPoint(new Vector3(-halfSize.x, 0, -halfSize.y));
        Vector3 targetAnglePoint3 = _controller.transform.TransformPoint(new Vector3(halfSize.x, 0, halfSize.y));
        Vector3 targetAnglePoint4 = _controller.transform.TransformPoint(new Vector3(halfSize.x, 0, -halfSize.y));

        //����Shader����
        GetComponent<Renderer>().material.SetVector("_Uvpoint1", new Vector4(targetAnglePoint1.x, targetAnglePoint1.y, targetAnglePoint1.z, 1f));
        GetComponent<Renderer>().material.SetVector("_Uvpoint2", new Vector4(targetAnglePoint2.x, targetAnglePoint2.y, targetAnglePoint2.z, 1f));
        GetComponent<Renderer>().material.SetVector("_Uvpoint3", new Vector4(targetAnglePoint3.x, targetAnglePoint3.y, targetAnglePoint3.z, 1f));
        GetComponent<Renderer>().material.SetVector("_Uvpoint4", new Vector4(targetAnglePoint4.x, targetAnglePoint4.y, targetAnglePoint4.z, 1f));
        GetComponent<Renderer>().material.SetMatrix("_VP", VP);
        GetComponent<Renderer>().material.mainTexture = _texture;

        //�ָ�UI��
        //camera.cullingMask |= (1 << 5);
        //����λ��
        _controller.OnTrackingFound();
    }

    void OnDestroy()
    {
        if (_texture)
            DestroyImmediate(_texture);
    }
}

