using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 过场动画开始事件
/// </summary>
public delegate void OnCinematicStart(GameObject cinematicTrigger);

/// <summary>
/// 过场动画结束事件
/// </summary>
public delegate void OnCinematicEnd(GameObject cinematicTrigger, bool cancelled);

public delegate void DelayFunc();

/// <summary>
/// 过场图片组件
/// - 播放过场图片时是以一个较高的优先级将图片UI覆盖到整个屏幕上
/// </summary>
public class CinematicPicObj : MonoBehaviour
{
    [Tooltip("过场名称")]
    public string cinematicsName;
    
    [Tooltip("过场图片集")]
    public List<Sprite> SerialPictures = new List<Sprite>();

    [Tooltip("是否点击翻页（为false则自动翻页）")]
    public bool clickForPageDown = true;

    [Tooltip("每一页的停留时间，与SerialPictures一一对应。clickForPageDown为false时才生效。")]
    public List<float> residenceTimeForPages = new List<float>();

    public OnCinematicStart onCinematicStart;
    public OnCinematicEnd onCinematicEnd;

    private PlayerInput _playerInput;

    /****** 组件状态 ******/
    private int _picIndex = 0;
    
    /// <summary>
    /// 过场动画中的ActionMap
    /// </summary>
    private string CINEMATIC_ACTION_MAP = "Cinematics";

    /// <summary>
    /// 进入过场动画前的ActionMap
    /// </summary>
    private string _playerActionMap;

    // Start is called before the first frame update
    void Start()
    {
        _playerInput = FindFirstObjectByType<PlayerInput>();
    }

    /// <summary>
    /// 播放过场动画
    /// </summary>
    public void Play()
    {
        if (_playerActionMap.Length == 0)
        {
            _playerActionMap = _playerInput.currentActionMap.name;
        }
        _playerInput.SwitchCurrentActionMap(CINEMATIC_ACTION_MAP);
        
        // TODO: 显示UI
        
        _picIndex = -1;
        PageDown();
        
        if (clickForPageDown)
        {
            // 注册切换图片
        }
        else
        {
            // TODO: 延时切换图片
        }
        
        // IEnumerator Delay(float timeSeconds, DelayFunc func)
        // {
        //     // 位移动画
        //     while (t < duration)
        //     {
        //         ingredientTrans.position = Vector3.Lerp(ingredientTrans.position, targetPos, t / duration);
        //         ingredientTrans.rotation = Quaternion.Lerp(ingredientTrans.rotation, targetRot, t / duration);
        //
        //         t += delta;
        //         yield return new WaitForSeconds(timeSeconds);
        //     }
        // }
    }
    
    public void Stop()
    {
        if (_playerActionMap.Length > 0)
        {
            _playerInput.SwitchCurrentActionMap(_playerActionMap);
        }
        onCinematicEnd?.Invoke(gameObject, true);
    }

    public void PageDown()
    {
        _picIndex++;
        if (_picIndex >= SerialPictures.Count)
        {
            // 播放结束
            _playerInput.SwitchCurrentActionMap(_playerActionMap);
            onCinematicEnd?.Invoke(gameObject, false);
            return;
        }
        
        // TODO: 切换图片
    }
}
