using UnityEngine;
using System.Collections;

public class GreyScale : MonoBehaviour {
    public Material mat;

    void Start() {
        mat.SetFloat( "_Power", 1.0f );
    }

    void OnRenderImage( RenderTexture source, RenderTexture destination ) {
        Graphics.Blit( source, destination, mat );
    }
}