using UnityEngine;

public class Clouds : MonoBehaviour
{
    public float speed = 0.1f;
    private float yPos;
    private float direction = 1;  // Used to control the direction of the movement

    private void Start()
    {
        yPos = transform.position.y;
        int rand = Random.Range(0, 2);
        if (rand == 1)
            direction = -1;  // Start moving in the opposite direction if rand is 1
    }

    private void Update()
    {   
        // Check if the cloud has moved 3 units above or below its original position
        if (transform.position.y > yPos + 3f || transform.position.y < yPos - 3f)
        {
            direction *= -1;  // Invert direction if it goes out of bounds
        }

        // Apply movement in the y-axis using the current direction and speed
        Vector3 newPosition = transform.position + new Vector3(0, speed * direction * Time.deltaTime, 0);
        transform.position = newPosition;
    }
}
