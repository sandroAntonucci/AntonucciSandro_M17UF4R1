using UnityEngine;

public class OrbitingItem : MonoBehaviour
{
    public float rotationSpeed = 50f;      // Degrees per second
    public float floatSpeed = 1f;          // Speed of up/down motion
    public float floatHeight = 0.5f;       // Height of the float movement

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Spin around Y-axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Float up and down using a sine wave
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
