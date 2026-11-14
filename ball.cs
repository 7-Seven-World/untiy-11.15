using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball : MonoBehaviour
{
    //声明路径点
    [Header("路径点")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    //声明可调参数
    [Header("动态参数")]
    [SerializeField, Tooltip("单程移动时间（秒）"), Range(0.1f, 20f)]
    private float moveDuration = 2f;

    [SerializeField, Tooltip("两端等待时间（秒）"), Range(0f, 10f)]
    private float waitTime = 0.5f;

    //声明运行时的状态变量
    //方向
    int _direction = 1;
    //计时器
    float _timer ;

    //定义状态枚举
    private enum State
    {
        Moving,
        Waiting
    }
    private State _state;

    //Awake做参数兜底
    private void Awake()
    {
        moveDuration = Mathf.Max(0.1f, moveDuration);
        waitTime = Mathf.Max(0f, waitTime);
    }

    //start初始化位置和状态
    private void Start()
    {
        if (!HasPath()) return;

        transform.position = pointA.position;
        _state = State.Moving;
        _timer = 0f;
        _direction = 1;
    }

    //update分发状态
    void Update()
    {
        if (!HasPath()) return;

        switch(_state)
        {
            case State.Moving:
                TickMoving();
                break;
            case State.Waiting:
                TickWaiting();
                break;
        }
    }

    //TickMoving处理移动状态
    private void TickMoving()
    {
       _timer += Time.deltaTime;
        float t = _timer / moveDuration;
        t = Mathf.Clamp01(t);
        float easedT = EaseInOut(t);
        Vector3 from = _direction ==1 ? pointA.position : pointB.position;
        Vector3 to = _direction == 1 ? pointB.position : pointA.position;

        transform.position = Vector3.Lerp(from, to, easedT);

        if(t >= 1f)
        {
            transform.position = to;
            _state = State.Waiting;
            _timer = 0f;
        }

    }

    //TickWaiting处理等待状态
    private void TickWaiting()
    {
        _timer += Time.deltaTime;
        if (_timer >= waitTime)
        {
            _state = State.Moving;
            _timer = 0f;
            _direction *= -1;
        }
    }

    //工具函数HasPath
    private bool HasPath() => pointA != null && pointB != null;

    //缓动函数EaseInOut
    private static float EaseInOut(float t)
    {
        return (Mathf.Asin(2f*t-1f)+ Mathf.PI*0.5f) / Mathf.PI;
    }

}
