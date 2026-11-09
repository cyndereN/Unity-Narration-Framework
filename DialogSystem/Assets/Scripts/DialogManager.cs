using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  //TMP_Text需要的命名空间
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance; //单例
    public TextAsset dialogDataFile; //对话内容文件
    string[] dialogRows; //对话内容的每一行
    public TMP_Text dialogText; //显示对话的内容
    public Image dialogImage; //显示对话的角色头像
    public Button nextBtn; //显示下一条对话信息
    public List<Sprite> images = new List<Sprite>(); //保存角色的所有头像
    Dictionary<string, Sprite> imageDic = new Dictionary<string, Sprite>(); //保存角色名字和头像的对应关系
    int curDialogIndex = 1; //当前对话的ID
    [HideInInspector]
    public string curNPCName; //当前对话的NPC名字
    [HideInInspector]
    public string excelNPCName = "同龄人"; //excel中NPC的名字
    public GameObject talkUI; //对话框UI
    public GameObject optionButtonPrefab; //选项按钮的预制体
    public Transform buttonGroup; //选项按钮的父物体
    public float textSpeed = 0.01f; //文字显示的速度
    bool cancelTyping = false; //有没有取消打字的方式显示
    bool textFinish = true; //文本有没有显示完成
    Role role = new Role(); //主角的属性类
    public TMP_Text growValueText; //成长值属性的UI
    public AudioClip growClip; //成长值增加的音效

    void Awake()
    {
        instance = this;
        ReadText();
        Init();
    }
    //初始化名字和头像的对应关系
    void Init()
    {
        imageDic["同龄人"] = images[0];
        imageDic["SiKi客服"] = images[1];
        imageDic["小Y老师"] = images[2];
        imageDic["Mono老师"] = images[3];
        imageDic["稀粥老师"] = images[4];
        imageDic["Trigger老师"] = images[5];
        imageDic["SiKi老师"] = images[6];
        imageDic["小白"] = images[7];

        role.name = "小白";
    }

    //读取文件中的文本
    void ReadText()
    {
        dialogRows = dialogDataFile.text.Split('\n');
    }
    //用文件中的每一行显示对话框中的内容
    public void ShowDialogRow()
    {
        //遍历每一行中的内容
        for(int i=0;i<dialogRows.Length;i++)
        {
            //读取每一行用逗号分割的内容
            string[] cells = dialogRows[i].Split(',');
            if (cells[0] == "#" && int.Parse(cells[1]) == curDialogIndex && curNPCName == excelNPCName) //顺序对话
            {
                nextBtn.gameObject.SetActive(true);
                UpdateText(cells[2], cells[3]);
                curDialogIndex = int.Parse(cells[5]); //更新当前的对话索引
                excelNPCName = cells[4];//更新excelNPC的名字
                if (cells[6] != "")
                {
                    string[] effect = cells[6].Split('@');
                    ShowEffectAttri(effect[0], int.Parse(effect[1]));
                }
                break;
            }
            else if (cells[0] == "&" && int.Parse(cells[1]) == curDialogIndex && curNPCName == excelNPCName) //分支对话
            {
                nextBtn.gameObject.SetActive(false);
                GenerateOptionButton(i);
                break;
            }
            else if (cells[0] == "END" && int.Parse(cells[1]) == curDialogIndex)  //结束对话
            {
                print("剧情结束");
            }
        }
        
    }
    //更新对话框的文本和头像
    void UpdateText(string name,string text)
    {
        //如果上一个文本显示完成，没有取消打字显示
        if(textFinish==true && cancelTyping == false)
        {
            dialogImage.sprite = imageDic[name]; //头像赋值
            StartCoroutine(SetTextUI(text));//使用协程来显示文字，文字会像打字的方式一样出现
        }
        else if(textFinish == false && cancelTyping == false)
        {
            cancelTyping = true;
        }

    }

    IEnumerator SetTextUI(string text)
    {
        textFinish = false;
        dialogText.text = ""; //清空文本框中的内容
        int letter = 0;
        while(!cancelTyping && letter < text.Length)
        {
            dialogText.text += text[letter];
            letter++;
            yield return new WaitForSeconds(textSpeed);
        }
        dialogText.text = text; //如果取消打字输入，直接给对话框赋值
        cancelTyping = false;
        textFinish = true;
    }
    //显示下一个对话内容按钮被点击
    public void OnNextBtnClick()
    {
        if(curNPCName == excelNPCName)
        {
            ShowDialogRow();
        }
       else
        {
            talkUI.SetActive(false);
        }
    }

    //产生分支的选项按钮
    void GenerateOptionButton(int index)
    {
        //读取index所在行的内容
        string[] cells = dialogRows[index].Split(',');
        if (cells[0] == "&")
        {
            GameObject btn = Instantiate(optionButtonPrefab, buttonGroup);
            btn.GetComponentInChildren<TMP_Text>().text = cells[3];
            btn.GetComponent<Button>().onClick.AddListener(
                delegate
                {
                    OnOptionClick(int.Parse(cells[5]));
                    if (cells[6] != "")
                    {
                        string[] effect = cells[6].Split('@');
                        ShowEffectAttri(effect[0], int.Parse(effect[1]));
                    }
                });
            GenerateOptionButton(index + 1);
        }
    }

    //选项按钮的点击事件
    void OnOptionClick(int index)
    {
        curDialogIndex = index; //更新当前的对话索引
        ShowDialogRow(); //显示最新的对话内容
        //销毁选项按钮
        for (int i=0;i<buttonGroup.childCount;i++)
        {
            Destroy(buttonGroup.GetChild(i).gameObject);
        }
    }

    //显示效果属性值
    void ShowEffectAttri(string effectName,int param)
    {
        if(effectName == "成长值")
        {
            role.growValue += param;
            growValueText.text = "成长值：" + role.growValue.ToString();
            AudioManager.instance.PlayAudio(growClip);
        }
    }
}
