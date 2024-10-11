using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PaintWithMouseUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private Shader drawShader;

    private RenderTexture splatMap;
    private Material drawMaterial;

    public Material currentMaterial;
    private RenderTexture finalImage;
    public RawImage currentImage;

    [SerializeField]
    [Range(1, 500)]
    private float size;

    RectTransform rect;

    bool drawing = false;
    Camera pressEventCamera;

    public ComputeShader cs;
    ComputeBuffer computeBuffer;

    private float sumofPixel;
    float ratio = 1;
    const int resolutionX = 1024;
    const int resolutionY = 1024;

    public Action OnClear { get; set; }

    bool isClear = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        pressEventCamera = eventData.pressEventCamera;
        drawing = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        drawing = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out rect); 
        ratio = rect.sizeDelta.x / rect.sizeDelta.y;
        computeBuffer=new ComputeBuffer(1, sizeof(int));
        drawMaterial = new Material(drawShader);
        drawMaterial.SetVector("_Color", Color.red);

        var mainTexture = currentMaterial.GetTexture("_MainTexture");

        splatMap = new RenderTexture(resolutionX, resolutionY, 0, RenderTextureFormat.ARGBFloat);
        finalImage = new RenderTexture(resolutionX, resolutionY, 0, RenderTextureFormat.ARGBFloat);
        currentMaterial.SetTexture("_SplatMap", splatMap); 

        currentImage.texture = finalImage;
        Draw(new Vector2(-1, -1));
    }

    // Update is called once per frame
    void Update()
    {
        if (drawing)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, pressEventCamera, out var local);
            local.x /= rect.rect.width;
            local.x += rect.pivot.x;
            local.y /= rect.rect.height;
            local.y += rect.pivot.y;
            Draw(local);
        }
    }

    void Draw(Vector2 coordinate)
    {
        drawMaterial.SetVector("_Coordinates", new Vector4(coordinate.x, coordinate.y, 0, 0));
        drawMaterial.SetFloat("_Size", size);
        drawMaterial.SetFloat("_Ratio", ratio);
        RenderTexture temp = RenderTexture.GetTemporary(splatMap.width, splatMap.height, 0, RenderTextureFormat.ARGBFloat);
        Graphics.Blit(splatMap, temp);
        Graphics.Blit(temp, splatMap, drawMaterial);
        Graphics.Blit(temp, finalImage, currentMaterial);
        RenderTexture.ReleaseTemporary(temp);
        CalculatePixel();
    }

    void CalculatePixel()
    {
        // the code that you want to measure comes here
        
        int initHandle = cs.FindKernel("CSInit");
        int mainHandle = cs.FindKernel("CSMain");

        cs.SetBuffer(initHandle, "compute_buffer", computeBuffer); 
        cs.SetBuffer(mainHandle, "compute_buffer", computeBuffer); 
        cs.SetTexture(mainHandle,"image",splatMap);
        cs.Dispatch(initHandle, 64, 1, 1);
        cs.Dispatch(mainHandle, resolutionX / 8, resolutionY / 8, 1);

        int[] result = new int[1];
        computeBuffer.GetData(result);
        sumofPixel = (float)result[0] / resolutionX / resolutionY;
        if(!isClear && sumofPixel > 0.6f)
        {
            ClearTexture();
        }
    }

    void ClearTexture()
    {        
        isClear = true;
        Graphics.Blit(Texture2D.whiteTexture, splatMap);
        OnClear();
        //Draw(new Vector2(-1, -1));
    }

    public void ResetAndDraw()
    {
        isClear = false;
        Graphics.Blit(Texture2D.blackTexture, splatMap);
        Draw(new Vector2(-1, -1));
    }

    void OnDestroy()
    {
        SafeReleaseBuffer(ref computeBuffer);
    }

    void SafeReleaseBuffer(ref ComputeBuffer bf)
    {
        if (bf == null) return;
        bf.Release();
        bf = null;
    }

}
