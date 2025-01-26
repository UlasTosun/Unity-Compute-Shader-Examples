using UnityEngine;



public class BasicComputeShaderController : MonoBehaviour {
    
    [Header("References")]
    [Tooltip("The compute shader that we will use to process the input texture.")]
    [SerializeField] private ComputeShader _computeShader;
    [Tooltip("The renderer that we will use to display the output texture.")]
    [SerializeField] private Renderer _renderer;
    [Tooltip("The input texture that we will use to process.")]
    [SerializeField] private Texture _inputTexture;

    [Header("Settings")]
    [Tooltip("The resolution of the output render texture.")]
    [Min(32)]
    [SerializeField] private int _textureResolution = 256;

    private RenderTexture _renderTexture;



    void Start() {
        // You can create an asset in the editor and assign it to the renderer instead of creating it here.
        _renderTexture = new(_textureResolution, _textureResolution, 0, RenderTextureFormat.ARGB32);
        _renderTexture.enableRandomWrite = true;
        _renderTexture.Create();

        // Assign the render texture to the renderer (assign the output of the compute shader to the renderer).
        _renderer.material.mainTexture = _renderTexture;

        // Set the input texture to the compute shader.
        _computeShader.SetTexture(0, "InputTex", _inputTexture);

        // Set the renderer texture to the compute shader, so that we can write the output of the compute shader to the render texture.
        _computeShader.SetTexture(0, "Result", _renderTexture);

        // Since, we use [numthreads(8, 8, 1)] in the compute shader, we need to dispatch the compute shader with the thread group sizes.
        // For example, we have 8 threads in x axis and our texture has 256 pixels in x axis. So, we need 256 / 8 = 32 thread groups in x axis.
        _computeShader.GetKernelThreadGroupSizes(0, out uint threadsX, out uint threadsY, out uint threadsZ); // Get the thread group sizes.
        
        _computeShader.Dispatch(0, _textureResolution / (int) threadsX, _textureResolution / (int) threadsY, (int) threadsZ);
        
    }



}
