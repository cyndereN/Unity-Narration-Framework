using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talk : MonoBehaviour
{
    public GameObject talkUI; //对话框UI

    //进入对话区域
    void OnTriggerEnter2D(Collider2D col)
    {
        if(DialogManager.instance.excelNPCName == gameObject.name)
        {
            talkUI.SetActive(true);
            DialogManager.instance.curNPCName = gameObject.name; //获取当前对话的NPC名字
            DialogManager.instance.ShowDialogRow(); //显示对话框中的内容
        }
    }

    //离开对话区域
    void OnTriggerExit2D(Collider2D other)
    {
        talkUI.SetActive(false);
    }
}
