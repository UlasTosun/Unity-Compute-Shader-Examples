using UnityEngine;



[System.Serializable]
public struct Circle {

    [Tooltip("Both X and Y values should be between 0 and 1.")]
    public Vector2 Position;

    [Range(0f, 1f)]
    public float Radius;

    [Range(0f, 1f)]
    public float Smoothness;

    public Color Color;

}
