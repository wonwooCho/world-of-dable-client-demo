using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class Banner : MonoBehaviour
{
    [SerializeField] private Renderer adThumbnail;
    [SerializeField] private Text adTextTitle;
    [SerializeField] private Text adTextAdvetiser;

    [Space(10)]
    [SerializeField] private GameObject filledObj;
    [SerializeField] private GameObject unfilledObj;

    public string AdRedirectLink { get; private set; } = string.Empty;

    private Coroutine requestAdCoroutine = null;

    private void Start()
    {
        SetFillState(false);
    }

    public void SetFillState(bool filled)
    {
        filledObj.SetActive(filled);
        unfilledObj.SetActive(!filled);

        if (!filled)
        {
            AdRedirectLink = string.Empty;
        }
    }

    public void RequestAdData()
    {
        Action<AdDataList> callback = (AdDataList data) => {
            SetFillState(true);

            int randomIndex = UnityEngine.Random.Range(0, data.result.Count);
            AdData picked = data.result[randomIndex];

            adTextAdvetiser.text = picked.ad_mark;
            adTextTitle.text = picked.title;
            AdRedirectLink = picked.link;

            LoadAdThumbnail(picked.thumbnail);
        };
        
        NetworkManager.Instance.RequestAdData(callback);
    }

    private void LoadAdThumbnail(string url)
    {
        if (requestAdCoroutine != null)
        {
            StopCoroutine(requestAdCoroutine);
            requestAdCoroutine = null;
        }

        requestAdCoroutine = StartCoroutine(CoLoadAdThumbnail(url));
    }

    private IEnumerator CoLoadAdThumbnail(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogErrorFormat("[Banner] Ad request error! {1}", www.error);
        }
        else
        {
            adThumbnail.material.mainTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
    }
}
