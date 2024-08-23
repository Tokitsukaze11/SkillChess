using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MemoryDisplayScene : MonoBehaviour
{
    public Text displayText;
    
    private List<float> fpsListForLower = new List<float>();
    private List<float> fpsListForHeight = new List<float>();

    private float minFPS = float.MaxValue;
    private float maxFPS = 0.0f;
    private float lower1PercentFPS = 0.0f;
    private float fps = 0.0f;
    private float memoryInMB = 0.0f;
    private float msFPS = 0.0f;
    private float height95PercentFPS = 0.0f;
    
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    private void Start()
    {
        StartCoroutine(FPSCheck());
    }
    private IEnumerator FPSCheck()
    {
        StartCoroutine(UpdateUI());
        yield return new WaitForSeconds(1f);
        while (true)
        {
            float memoryInMB = System.GC.GetTotalMemory(false) / (1024.0f * 1024.0f);
            this.memoryInMB = memoryInMB;
            float fps = 1.0f / Time.deltaTime;
            this.fps = fps;
            if(fps < minFPS)
            {
                minFPS = fps;
            }
            if(fps > maxFPS)
            {
                maxFPS = fps;
            }
            msFPS = Time.deltaTime * 1000.0f;
            fpsListForLower.Add(fps);
            fpsListForLower = fpsListForLower.OrderBy(x => x).ToList();
            lower1PercentFPS = fpsListForLower[Mathf.FloorToInt(fpsListForLower.Count * 0.01f)];
            if(fpsListForLower.Count > 100)
            {
                fpsListForLower.RemoveAt(fpsListForLower.Count - 1);
            }
            fpsListForHeight.Add(fps);
            fpsListForHeight = fpsListForHeight.OrderByDescending(x => x).ToList();
            height95PercentFPS = fpsListForHeight[Mathf.FloorToInt(fpsListForHeight.Count * 0.05f)];
            if(fpsListForHeight.Count > 100)
            {
                fpsListForHeight.RemoveAt(fpsListForHeight.Count - 1);
            }
            yield return null;
        }
    }
    private IEnumerator UpdateUI()
    {
        displayText.text = "Initializing...";
        yield return new WaitForSeconds(2f);
        while (true)
        {
            displayText.text = $"Memory: {memoryInMB:F2} MB\nFPS: {fps:F2}\nMaxFPS: {maxFPS:F2}\nMinFPS: {minFPS:F2}\nLower1%FPS: {lower1PercentFPS:F2}\nHeight95%FPS: {height95PercentFPS:F2}\nFPS Time: {msFPS:F2}ms";
            yield return new WaitForSeconds(1f);
        }
        yield break;
    }
#endif
}
