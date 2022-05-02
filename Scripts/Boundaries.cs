using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundaries : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Camera cam;
    private Vector2 camBounds;
    private float objectWidth;
    private float objectheight;

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = transform.GetComponent<SpriteRenderer>();
        cam = Camera.main;
        camBounds = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, cam.transform.position.z));
        objectWidth = _spriteRenderer.bounds.size.x / 2;
        objectheight = _spriteRenderer.bounds.size.y / 2;
    }

    void LateUpdate()
    {
        Vector3 inScreenPosition = transform.position;
        inScreenPosition.x = Mathf.Clamp(inScreenPosition.x, -camBounds.x + objectWidth, camBounds.x - objectWidth);
        inScreenPosition.y = Mathf.Clamp(inScreenPosition.y, -camBounds.y + objectheight, camBounds.y - objectheight);
        transform.position = inScreenPosition;
    }
}
