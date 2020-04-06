using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTextController : MonoBehaviour
{
    public GameObject text;
    public Vector3 offset;


    public void ShowScore(int score)
    {
        GameObject textInstance = Instantiate(text, transform.position + offset, Quaternion.identity);
        textInstance.transform.parent = transform;
        TextMesh textMesh = textInstance.GetComponent<TextMesh>();
        textMesh.text = string.Format("+{0}", score);
        Destroy(textInstance, 1.0f);
        textInstance.GetComponent<Rigidbody>().velocity = Vector3.up * 100.0f;
    }
}
