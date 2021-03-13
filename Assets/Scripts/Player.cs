using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public static Vector3Int Pos;
    public static float SightRange = 7;

    private Rigidbody rb;

    public float speed;
    public float jumpForce;
    public float sightRange;

    private UnityAction<Vector2> redrawMap;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        SightRange = sightRange;

        Pos = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
    }

    private void Update()
    {
        // Update Pos and RedrawMap
        Vector3Int newPos = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
        if (newPos != Pos)
        {
            Vector2 offset = new Vector2(newPos.x - Pos.x, newPos.z - Pos.z);
            Pos = newPos;

            redrawMap(offset);
        }

        // Move
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (h != 0 || v != 0) transform.Translate(new Vector3(h, 0, v) * speed * Time.deltaTime);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space)) rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // Initialize
    public void Init(UnityAction<Vector2> redrawMap)
    {
        this.redrawMap = redrawMap;
    }
}
