using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shrinkGrowAnim : MonoBehaviour
{
    public float min;
    public float max;
    public float speed;
    float timer = 0;

    // Start is called before the first frame update
    void OnEnable()
    {
        timer = -3.14f/2;
        timer = 0;
        gameObject.transform.localPosition = new Vector3((max * Mathf.Sin(timer)),
                                                         gameObject.transform.localPosition.y,
                                                         gameObject.transform.localPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.localPosition = new Vector3((max * Mathf.Sin(timer)),
                                                         gameObject.transform.localPosition.y,
                                                         gameObject.transform.localPosition.z);
        //gameObject.transform.localScale = Vector3.one *( min +  Mathf.PingPong(Time.time * speed , max));
        timer += Time.deltaTime;
    }
}
