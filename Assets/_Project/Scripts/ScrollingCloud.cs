using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ScrollingCloud : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    [SerializeField] Vector2 resetPos = new Vector2(20, 0);
    
    private void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
        if(transform.position.x < -20f)
        {
            transform.position = resetPos;
        }
    }
}
