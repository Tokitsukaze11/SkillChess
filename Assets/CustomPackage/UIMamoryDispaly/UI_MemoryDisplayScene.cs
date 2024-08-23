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

        List<float> fpsListPerSecond = new List<float>();
        float elapsedTime = 0f;

        while (true)
        {
            float memoryInMB = System.GC.GetTotalMemory(false) / (1024.0f * 1024.0f);
            this.memoryInMB = memoryInMB;
            float fps = 1.0f / Time.deltaTime;
            this.fps = fps;

            if (fps < minFPS)
            {
                minFPS = fps;
            }
            if (fps > maxFPS)
            {
                maxFPS = fps;
            }

            msFPS = Time.deltaTime * 1000.0f;

            // 1초 동안의 FPS 데이터 수집
            fpsListPerSecond.Add(fps);
            elapsedTime += Time.deltaTime;

            // 1초가 지나면 하위 1%와 상위 95% 계산
            if (elapsedTime >= 1f)
            {
                fpsListPerSecond.Sort();
                int count = fpsListPerSecond.Count;

                lower1PercentFPS = fpsListPerSecond[Mathf.FloorToInt(count * 0.01f)];
                height95PercentFPS = fpsListPerSecond[Mathf.FloorToInt(count * 0.95f)];

                // 리스트 초기화 및 경과 시간 재설정
                fpsListPerSecond.Clear();
                elapsedTime = 0f;
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
            displayText.text = $"Memory: {memoryInMB:F2} MB\nFPS: {fps:F2}\n" +
                               $"MaxFPS: {maxFPS:F2}\nMinFPS: {minFPS:F2}\nLower1%FPS: {lower1PercentFPS:F2}\nHeight95%FPS: {height95PercentFPS:F2}\n" +
                               $"FPS Time: {msFPS:F2}ms";
            yield return new WaitForSeconds(1f);
        }
        yield break;
    }
#endif
}
