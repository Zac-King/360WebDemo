using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WebTextureManager : MonoBehaviour
{
    private int m_CurrentViseoIndex = 0;

    public GameObject m_Screen;
    private MeshRenderer m_Renderer;

    private List<WebGLMovieTexture> m_MovieTextures = new List<WebGLMovieTexture>();
    public List<VideoInformation> m_VideoMeta = new List<VideoInformation>();
    
    private void Start()
    {
        m_Renderer = m_Screen.GetComponent<MeshRenderer>();
        m_Renderer.material = new Material(Shader.Find("Diffuse"));

        WebGLMovieTexture webText;
        foreach (VideoInformation vInfo in m_VideoMeta)
        {
            webText = new WebGLMovieTexture(@"StreamingAssets/" + vInfo.m_FileName);
            webText.loop = vInfo.m_Loop;
            m_MovieTextures.Add(webText);
        }

        StopCurrentAndPlayVideoAtIndex(0);
    }

    private void Update()
    {
        foreach (WebGLMovieTexture webTex in m_MovieTextures)
            webTex.Update();
    }

    public void StopCurrentAndPlayVideoAtIndex(int index)
    {
        if (index >= m_VideoMeta.Count) return;

       if (m_VideoMeta[m_CurrentViseoIndex] != null)
            foreach (GameObject go in m_VideoMeta[m_CurrentViseoIndex].m_VideoObjects)
                go.SetActive(false);

        foreach (GameObject go in m_VideoMeta[index].m_VideoObjects)
            go.SetActive(true);

        m_MovieTextures[m_CurrentViseoIndex].Pause();
        m_MovieTextures[m_CurrentViseoIndex].Seek(0f);

        m_CurrentViseoIndex = index;

        m_MovieTextures[m_CurrentViseoIndex].Seek(0f);
        m_MovieTextures[m_CurrentViseoIndex].Play();

        m_Renderer.material.mainTexture = m_MovieTextures[m_CurrentViseoIndex];
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