using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10; //主角移动的速度
    Rigidbody2D rig; //主角的刚体
    Animator anim; //主角的动画控制器
    Vector2 lookDirection = new Vector2(0, -1); //主角默认朝下
    public AudioClip moveClip; //主角移动的音效
    public AudioSource audioSource; //音频源组件

    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        //返回水平的虚拟轴值
        float moveX = Input.GetAxisRaw("Horizontal"); //A:-1  D:1  0
        //返回垂直的虚拟轴值
        float moveY = Input.GetAxisRaw("Vertical"); //W:1  S:-1  0
        Vector2 moveVector2 = new Vector2(moveX, moveY);
        //主角的移动
        Vector2 pos = rig.position; //保存当前刚体的位置
        pos += moveVector2 * speed * Time.deltaTime;
        rig.MovePosition(pos);
        //主角的动画
        if(moveVector2.x != 0 || moveVector2.y != 0)
        {
            lookDirection = moveVector2;
            if(audioSource.isPlaying == false)
                audioSource.Play();
        }
        anim.SetFloat("LookX", lookDirection.x);
        anim.SetFloat("LookY", lookDirection.y);
        anim.SetFloat("Speed", moveVector2.magnitude); //用向量的长度当成速度
    }
}
