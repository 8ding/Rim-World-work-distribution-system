using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换模块
/// </summary>
public class ScenesMgr : BaseManager<ScenesMgr>
{
    /// <summary>
    /// 切换场景
    /// </summary>
    /// <param name="name"></param>
    public void LoadScene(string name,Action fun)
    {
        //场景同步加载
        SceneManager.LoadScene(name);
        //加载完成后执行
        fun();
    }

    public void LoadSceneAsyn(string name, Action fun)
    {
        MonoMgr.Instance.StartCoroutine(ReallyLoadSceneAsyn(name, fun));
        
    }

    private IEnumerator ReallyLoadSceneAsyn(string name, Action fun)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name);
        //可以得到场景加载的一个进度
        while (!asyncOperation.isDone)
        {
            //事件中心分发进度情况
            EventCenter.Instance.EventTrigger("SceneLoading",asyncOperation.progress);
            yield return asyncOperation.progress;
        }
        //加载完成后执行
        fun();
    }
}
