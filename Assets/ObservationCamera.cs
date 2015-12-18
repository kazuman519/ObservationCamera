using UnityEngine;
using System.Collections;

public class ObservationCamera : MonoBehaviour {
    public Camera camera; // 使用するカメラ
    public bool isAutoRotate = false; // 最初に自動で回転させておくかのフラグ
    public float minCameraAngleX = 310.0f; // カメラの最小角度
    public float maxCameraAngleX = 20.0f; // カメラの最大角度
    public float swipeTurnSpeed = 30.0f; // スワイプで回転するときの回転スピード
    public float pinchZoomSpeed = 100.0f; // ピンチするときのズームスピード
    public float autoRotateSpeed = 20.0f; // 自動で回転させるときの回転スピード

    private Vector3 baseMousePos; // 基準となるタップの座標
    private Vector3 baseCameraPos; // 基準となるカメラの座標
    private float basePinchZoomDistanceX = 0f; // ズームの基準となるピンチの距離 x
    private float basePinchZoomDistanceY = 0f; // ズームの基準となるピンチの距離 y
    private float basePinchDistance = 0f; //  // 基準となるピンチ時の指と指の距離
    private bool isMouseDown = false; // マウスが押されているかのフラグ
    private bool isPinchStart = true; // ピンチスタートしたかを管理するフラグ

        
	void Start () {
	}
	
	void Update () {
		// 自動で回す
		if (isAutoRotate) {
            float angleY = this.transform.eulerAngles.y - Time.deltaTime * autoRotateSpeed;
			this.transform.eulerAngles = new Vector3 (this.transform.eulerAngles.x, angleY, 0);
		}

		// タップの種類の判定 & 対応処理
		if ((Input.touchCount == 1 && !isMouseDown)|| Input.GetMouseButtonDown(0)) {
			baseMousePos = Input.mousePosition;
			isMouseDown = true;
            isAutoRotate = false;
		} 
		else if (Input.touchCount == 2) {
			// ピンチでズーム用の処理群
          

            if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[1].phase == TouchPhase.Ended)
            {
                isPinchStart = true;
            }
            else if (Input.touches[0].phase == TouchPhase.Moved || Input.touches[1].phase == TouchPhase.Moved) {
                if (isPinchStart) {
                    isPinchStart = false;

                    basePinchDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
                    baseCameraPos = camera.transform.localPosition;
                }

                float currentPinchDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
                float pinchZoomDistance = (basePinchDistance - currentPinchDistance) * pinchZoomSpeed * 0.05f;
                float cameraPosZ = baseCameraPos.z - pinchZoomDistance;

                camera.transform.localPosition = new Vector3 (camera.transform.localPosition.x, camera.transform.localPosition.y, cameraPosZ);
            }
				
			isMouseDown = false;
            isAutoRotate = false;
		}

        // 指離した時の処理
		if (Input.GetMouseButtonUp(0)) {
			isMouseDown = false;

			basePinchZoomDistanceX = 0;
			basePinchZoomDistanceY = 0;
		}

		// スワイプ回転処理
		if (isMouseDown) {
			Vector3 mousePos = Input.mousePosition;
            Vector3 distanceMousePos = (mousePos - baseMousePos);
            float angleX = this.transform.eulerAngles.x - distanceMousePos.y * swipeTurnSpeed * 0.01f;
            float angleY = this.transform.eulerAngles.y + distanceMousePos.x * swipeTurnSpeed * 0.01f;

			basePinchZoomDistanceX += distanceMousePos.x;
            basePinchZoomDistanceY += distanceMousePos.y;

            // カメラのアングルに制限をかける もっとイカした書き方にしたい
            if ((angleX >= -10f && angleX <= maxCameraAngleX) || (angleX >= minCameraAngleX && angleX <= 370f)) {
				this.transform.eulerAngles = new Vector3 (angleX, angleY, 0);
			} else {
				this.transform.eulerAngles = new Vector3 (this.transform.eulerAngles.x, angleY, 0);
			}

			baseMousePos = mousePos;
		}
	}
}
