using System.Collections.Generic;
using UnityEngine;



public class ComputeBufferController : MonoBehaviour {
    
    [Header("References")]
    [Tooltip("The compute shader that will be used to draw the circles on the texture.")]
    [SerializeField] private ComputeShader _computeShader;
    [Tooltip("The renderer that will display the output texture.")]
    [SerializeField] private Renderer _renderer;

    [Header("Settings")]
    [Tooltip("The resolution of the output texture.")]
    [Min(32)]
    [SerializeField] private int _textureResolution = 256;
    [Tooltip("The circles that will be drawn on the texture.")]
    [SerializeField] private List <Circle> _circles = new();

    private RenderTexture _renderTexture;
    private ComputeBuffer _computeBuffer;



    void Start() {
        CreateTexture();

        // Assign the render texture to the renderer (assign the output of the compute shader to the renderer).
        _renderer.material.mainTexture = _renderTexture;

        // Set the renderer texture to the compute shader, so that we can write the output of the compute shader to the render texture.
        _computeShader.SetTexture(0, "Result", _renderTexture);
        
    }



    void Update() {
        SetTexture();
    }



    private void CreateTexture() {
        // You can create an asset in the editor and assign it to the renderer instead of creating it here.
        _renderTexture = new(_textureResolution, _textureResolution, 0, RenderTextureFormat.ARGB32);
        _renderTexture.enableRandomWrite = true;
        _renderTexture.Create();
    }



    private void SetTexture() {
        // Since, we use [numthreads(64, 1, 1)] in the compute shader, we need to dispatch the compute shader with the thread group sizes.
        _computeShader.GetKernelThreadGroupSizes(0, out uint threadsX, out uint threadsY, out uint threadsZ); // Get the thread group sizes.

        // The size of the struct in the compute shader in bytes. Each float is 4 bytes (32 bits).
        // Vector2 = 2 floats, and color = 4 floats because it has 4 channels (RGBA).
        // So, the size of the struct is 1 + 1 + 2 + 4 = 8 floats and 8 * 4 = 32 bytes.
        int stride = 32;

        _computeBuffer = new ComputeBuffer(_circles.Count, stride, ComputeBufferType.Default); // Create a compute buffer with the size of the circles list.
        _computeBuffer.SetData(_circles.ToArray()); // Set the data of the compute buffer to the circles list.
        _computeShader.SetBuffer(0, "CircleBuffer", _computeBuffer); // Set the compute buffer to the compute shader.

        _computeShader.Dispatch(0, _textureResolution / (int)threadsX, _textureResolution / (int)threadsY, (int)threadsZ);
        _computeBuffer.Release(); // Release the compute buffer after the dispatch to free up memory.
    }



    // Split a list into smaller lists.
    public static IEnumerable<List<T>> SplitList<T>(List<T> list, int size) {
        for (int i = 0; i < list.Count; i += size) {
            yield return list.GetRange(i, Mathf.Min(size, list.Count - i));
        }
    }



}
