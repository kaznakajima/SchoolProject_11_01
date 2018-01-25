using UnityEngine;
using System.Collections;
public class CameraAngle : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        //カメラリセット
        bool canResetPos = Input.GetKey(KeyCode.R);


        // マウスホイールでズーム処理
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        float view = cam.fieldOfView - wheel;
        //float view = transform.position + transform.forward * wheel;

        cam.fieldOfView = Mathf.Clamp(value: view, min: 0.1f, max: 45f);


        // Wキーで前進
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, 0, 6 * Time.deltaTime);
        }

        // Sキーで後退
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(0, 0, -6 * Time.deltaTime);
        }

        // Dキーで右移動
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(8 * Time.deltaTime, 0, 0);
        }

        // Aキーで左移動
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-8 * Time.deltaTime, 0, 0);
        }

        // Qキーで左回転
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0 * Time.deltaTime, -1, 0);
        }

        // Eキーで右回転
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0 * Time.deltaTime,1 , 0);
        }

        // Tキーで前転
        if (Input.GetKey(KeyCode.T))
        {
            transform.Rotate(50 * Time.deltaTime, 0, 0);
        }

        // Gキーで後転
        if (Input.GetKey(KeyCode.G))
        {
            transform.Rotate(-50 * Time.deltaTime, 0, 0);
        }

        // Rキーで視点リセット
        if (canResetPos)
        {
            transform.localRotation = Quaternion.identity;
        }
    }
}




