using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextSortLayer : MonoBehaviour
{
    //将文本父物体的渲染顺序赋值给当前文本
    void Start()
    {
        var parentRender = transform.parent.GetComponent<Renderer>();
        var render = GetComponent<Renderer>();
        render.sortingLayerName = parentRender.sortingLayerName;
        render.sortingOrder = parentRender.sortingOrder;
    }


    void Update()
    {
        
    }
}
