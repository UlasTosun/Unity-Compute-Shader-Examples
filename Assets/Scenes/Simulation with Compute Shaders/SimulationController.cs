using UnityEngine;
using Unity.Mathematics;



public class SimulationController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private ComputeShader _computeShader;
    [SerializeField] private Transform[] _transforms;

    [Header("Simulation Settings")]
    [SerializeField] private float _amplitude = 2f;
    [SerializeField] private float _frequency = 9f;
    [SerializeField] private float _phaseMultiplier = 5f;




    void Start() {
        // Set the compute shader properties.
        _computeShader.SetFloat("Amplitude", _amplitude);
        _computeShader.SetFloat("Frequency", _frequency);
        _computeShader.SetFloat("PhaseMultiplier", _phaseMultiplier);
    }



    void Update() {
        // Set the compute shader properties.
        _computeShader.SetFloat("Amplitude", _amplitude);
        _computeShader.SetFloat("Frequency", _frequency);
        _computeShader.SetFloat("PhaseMultiplier", _phaseMultiplier);

        float3[] positions = GetPositions(_transforms); // Get the positions of the transforms.
        UpdatePositions(positions, Time.time); // Update the positions with the compute shader.
        SetPositions(_transforms, positions); // Set the positions back to the transforms.
    }



    private void UpdatePositions(float3[] positions, float elapsedTime) {
        _computeShader.GetKernelThreadGroupSizes(0, out uint threadsX, out uint threadsY, out uint threadsZ); // Get the thread group sizes.
        int stride = 12; // 3 floats (Vector3) = 12 bytes.

        ComputeBuffer computeBuffer = new(positions.Length, stride, ComputeBufferType.Default); // Create a compute buffer with the size of the positions array.
        computeBuffer.SetData(positions); // Set the data of the compute buffer to the positions array.

        _computeShader.SetBuffer(0, "PositionBuffer", computeBuffer); // Set the compute buffer to the compute shader.
        _computeShader.SetFloat("ElapsedTime", elapsedTime);

        int threadGroupsX = Mathf.CeilToInt(positions.Length / (float)threadsX); // Calculate the number of thread groups in the X direction.
        _computeShader.Dispatch(0, threadGroupsX, (int)threadsY, (int)threadsZ); // Dispatch the compute shader.

        computeBuffer.GetData(positions); // Get the data from the compute buffer back to the positions array.
        computeBuffer.Release(); // Release the compute buffer after the dispatch to free up memory.
    }



    private float3[] GetPositions(Transform[] transforms) {
        float3[] positions = new float3[transforms.Length];
        for (int i = 0; i < transforms.Length; i++) {
            positions[i] = transforms[i].position;
        }

        return positions;
    }



    private void SetPositions(Transform[] transforms, float3[] positions) {
        for (int i = 0; i < transforms.Length; i++) {
            transforms[i].position = positions[i];
        }
    }



}
