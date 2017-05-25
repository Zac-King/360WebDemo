﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiScreenVideoManager : MonoBehaviour
{
    private int m_currentVideoIndex = -1;

    public float m_minWarp;

    public float m_warpSpeed;


    /// <summary>
    /// Mesh for the screen the WebMovieTextures will be rendered on
    /// </summary>
    public Mesh m_screenMesh;

    /// <summary>
    /// List of IVV meta data (Video path, loop, object to be spawned on play)
    /// </summary>
    public List<VideoInformation> m_videoMeta = new List<VideoInformation>();

    /// <summary>
    /// All the videos and their screens
    /// </summary>
    private List<GameObject> m_videoScreens = new List<GameObject>();
    /// <summary>
    /// All the WebMovieTextures linked by their respective screens
    /// </summary>
    private Dictionary<GameObject, WebGLMovieTexture> m_screenTexturePairs =
        new Dictionary<GameObject, WebGLMovieTexture>();

    void Start ()
    {
        MeshFilter filter;
        MeshRenderer renderer;
        WebGLMovieTexture webTex;

		foreach (VideoInformation vInfo in m_videoMeta)
        {
            GameObject vScreen = new GameObject(vInfo.m_FileName);
            vScreen.transform.parent = transform;
            vScreen.transform.localPosition = Vector3.zero;

            filter = vScreen.AddComponent<MeshFilter>();
            renderer = vScreen.AddComponent<MeshRenderer>();

            filter.mesh = m_screenMesh;
            renderer.material = new Material(Shader.Find("Diffuse"));

            webTex = new WebGLMovieTexture(@"StreamingAssets/" + vInfo.m_FileName);
            webTex.loop = vInfo.m_Loop;

            renderer.material.mainTexture = webTex;

            m_videoScreens.Add(vScreen);
            m_screenTexturePairs.Add(vScreen, webTex);

            vScreen.SetActive(false);
            webTex.Pause();
        }

        StopCurrentAndPlayVideoAtIndex(0);
	}
	
	void Update ()
    {
        foreach (KeyValuePair<GameObject, WebGLMovieTexture> webTex in m_screenTexturePairs)
            webTex.Value.Update();
	}

    public void StopCurrentAndPlayVideoAtIndex(int index)
    {
        if (index >= m_videoMeta.Count || index == m_currentVideoIndex) return;


        m_videoScreens[index].SetActive(true);
        m_screenTexturePairs[m_videoScreens[index]].Play();

        foreach (GameObject go in m_videoMeta[index].m_VideoObjects)
            go.SetActive(true);


        if (m_currentVideoIndex >= 0 && m_currentVideoIndex < m_videoMeta.Count)
        {
            if (m_videoMeta[m_currentVideoIndex] != null)
                foreach (GameObject go in m_videoMeta[m_currentVideoIndex].m_VideoObjects)
                    go.SetActive(false);

            m_screenTexturePairs[m_videoScreens[m_currentVideoIndex]].Pause();
            m_screenTexturePairs[m_videoScreens[m_currentVideoIndex]].Seek(0f);
            m_videoScreens[m_currentVideoIndex].SetActive(false);
        }


        m_currentVideoIndex = index;
    }

    public void WarpToVideoAtIndex(int index)
    {
        StartCoroutine(_WarpToVideoAtIndex(index));
    }

    public IEnumerator _WarpToVideoAtIndex(int index)
    {
        float originalZoom = Camera.main.fieldOfView;

        while(Camera.main.fieldOfView > m_minWarp)
        {
            Camera.main.fieldOfView -= (m_warpSpeed * Time.deltaTime);
            yield return null;
        }

        StopCurrentAndPlayVideoAtIndex(index);
        Camera.main.fieldOfView = originalZoom;
    }

    [System.Serializable]
    public class VideoInformation
    {
        public string m_FileName;
        public bool m_Loop;
        public List<GameObject> m_VideoObjects;

        public void SetEqualTo(VideoInformation vidInfo)
        {
            m_FileName = vidInfo.m_FileName;
            m_VideoObjects = vidInfo.m_VideoObjects;
        }
    }
}
