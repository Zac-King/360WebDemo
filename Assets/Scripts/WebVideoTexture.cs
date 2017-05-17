using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class WebVideoTexture : MonoBehaviour
{
	WebGLMovieTexture m_CurrentMovieTexture;
    WebGLMovieTexture m_NextMovieTexture;

    private VideoInformation m_CurrentVideo;
    private VideoInformation m_NextVideo;

    public float m_MaxZoom, m_ZoomSpeed;

    public GameObject Screen;

    public System.Collections.Generic.List<VideoInformation> m_Videos =
        new System.Collections.Generic.List<VideoInformation>();

    private void Start()
    {
        Screen.transform.localScale = Vector3.one * 1f;

        m_CurrentVideo = m_Videos[0];

        SetVideoTextureTo(m_CurrentVideo);
        m_CurrentMovieTexture.Play();
    }

    public void SetVideoTextureTo(VideoInformation newVideo)
    {
        m_CurrentVideo.SetEqualTo(newVideo);
        m_CurrentMovieTexture = new WebGLMovieTexture(@"StreamingAssets/" + m_CurrentVideo.m_FileName);
        Screen.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
        Screen.GetComponent<MeshRenderer>().material.mainTexture = m_CurrentMovieTexture;

    }
    
    private void Update()
    {
        m_CurrentMovieTexture.Update();
    }

    public void PlayNextVideoInList()
    {
        int currentIndex = 0;

        for (int i = 0; i < m_Videos.Count; ++i)
        {
            if (m_Videos[i].m_FileName == m_CurrentVideo.m_FileName)
            {
                print(i);
                currentIndex = i;
                break;
            }
        }

        int nextIndex = currentIndex + 1;
        nextIndex = nextIndex % m_Videos.Count;

        PlayVideoAtIndex(nextIndex);
    }

    public void PlayVideoAtIndex(int index)
    {
        StartCoroutine(_WarpToVideo(m_Videos[index]));
    }

    public IEnumerator _WarpToVideo(VideoInformation nextVideo)
    {
        foreach (GameObject go in m_CurrentVideo.m_VideoObjects)
            go.SetActive(false);

        StartCoroutine(_PreloadNextVideo(nextVideo));

        float originalZoom = Camera.main.fieldOfView;

        while (Camera.main.fieldOfView > m_MaxZoom)
        {
            Camera.main.fieldOfView -= Time.deltaTime * m_ZoomSpeed;
            yield return null;
        }
        Camera.main.fieldOfView = originalZoom;
        m_CurrentVideo = m_NextVideo;

        foreach (GameObject go in m_CurrentVideo.m_VideoObjects)
            go.SetActive(true);
    }

    public IEnumerator _PreloadNextVideo(VideoInformation nextVideo)
    {
        m_NextVideo.SetEqualTo(nextVideo);
        m_NextMovieTexture = new WebGLMovieTexture(@"StreamingAssets/" + m_NextVideo.m_FileName);

        while(!m_NextMovieTexture.isReady)
        {
            m_NextMovieTexture.Update();
            yield return null;
        }

        m_CurrentMovieTexture = m_NextMovieTexture;
        Screen.GetComponent<MeshRenderer>().material.mainTexture = m_CurrentMovieTexture;

        m_CurrentMovieTexture.Play();


    }
}

[System.Serializable]
public class VideoInformation
{
    public string m_FileName;
    public System.Collections.Generic.List<GameObject> m_VideoObjects;

    public void SetEqualTo(VideoInformation vidInfo)
    {
        m_FileName = vidInfo.m_FileName;
        m_VideoObjects = vidInfo.m_VideoObjects;
    }
}