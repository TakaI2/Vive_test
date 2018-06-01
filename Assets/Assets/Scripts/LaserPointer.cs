using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LaserPointer : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    public GameObject laserPrefab;
    public GameObject bluelaserPrefab;
    private GameObject laser;
    private GameObject bluelaser;
    private Transform laserTransform;
    private Transform bluelaserTransform;
    private Vector3 hitPoint;

    public Transform cameraRigTransform;
    public GameObject teleportReticlePrefab;
    private GameObject reticle;
    private Transform teleportReticleTransform;
    public Transform headTransform;
    public Vector3 teleportReticleOffset;
    public LayerMask teleportMask;
    private bool shouldTeleport;

    //ボタン押下に関する関数
    private GameObject currentButton;
    private Clicker clicker = new Clicker();
    public LayerMask ButtonMask;




    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }


    // Use this for initialization
    void Start()
    {
        laser = Instantiate(laserPrefab);
        bluelaser = Instantiate(bluelaserPrefab);
        laserTransform = laser.transform;
        bluelaserTransform = bluelaser.transform;

        reticle = Instantiate(teleportReticlePrefab); //あたらしいレチクルを作成し、レチクルにそのレファレンスを保存する。
        teleportReticleTransform = reticle.transform; //レチクルの変換コンポーネントを格納



    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f); //コントローラとレイキャストが当たるポイントの間にレーザーを配置
        laserTransform.LookAt(hitPoint); //レイキャストが当たる位置にレーザーを向ける
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance); //レーザーを二つの位置の間に完全に収まるようにスケール
    }

    private void ShowblueLaser(RaycastHit hit)
    {
        bluelaser.SetActive(true);
        bluelaserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f); //コントローラとレイキャストが当たるポイントの間にレーザーを配置
        bluelaserTransform.LookAt(hitPoint); //レイキャストが当たる位置にレーザーを向ける
        bluelaserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance); //レーザーを二つの位置の間に完全に収まるようにスケール
    }


    private void Teleport()
    {
        shouldTeleport = false;
        reticle.SetActive(false);
        Vector3 difference = cameraRigTransform.position - headTransform.position;
        difference.y = 0; //計算ではプレイヤーの頭の垂直位置は考慮されないため、y位置を0にリセット
        cameraRigTransform.position = hitPoint + difference;
    }

	
	// Update is called once per frame
	void Update () {

        GameObject hitObject;
        RaycastHit hit;
        GameObject hitButton = null;
        PointerEventData data = new PointerEventData(EventSystem.current);




        


        if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100))
        {

            
            hitPoint = hit.point;
            hitObject = hit.collider.gameObject;
            ShowblueLaser(hit);

            if (hitObject.tag == "Button") { hitButton = hit.transform.parent.gameObject; }

            if (currentButton != hitButton)
            {
                if (currentButton != null) { ExecuteEvents.Execute<IPointerExitHandler>(currentButton, data, ExecuteEvents.pointerExitHandler); } //ハイライトを外す。
                currentButton = hitButton;
                if (currentButton != null) { ExecuteEvents.Execute<IPointerEnterHandler>(currentButton, data, ExecuteEvents.pointerEnterHandler); } //ハイライトする。

            }

            if (currentButton != null)
            {
                if (clicker.clicked()) { ExecuteEvents.Execute<IPointerClickHandler>(currentButton, data, ExecuteEvents.pointerClickHandler); } //レーザを向けたボタンをクリックする.
            }

        }


        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))  //タッチパッドが押されているとき
        {

            bluelaser.SetActive(false);

            //RaycastHit hit;
            //GameObject hitButton = null;
            //PointerEventData data = new PointerEventData(EventSystem.current);

            //コントローラから光線を射撃、何かに当たったらヒットしたポイントを保存してレーザーを表示
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, teleportMask))
            {
                hitPoint = hit.point;
                ShowLaser(hit);               

                reticle.SetActive(true);
                teleportReticleTransform.position = hitPoint + teleportReticleOffset;
                shouldTeleport = true;

            }

        }
        else //プレイヤーがタッチパッドを放したときにレーザーを隠す
        {
            laser.SetActive(false);
            reticle.SetActive(false);          
        }

        if(Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport) //タッチパッドが解放され、有効なテレポート位置がある場合、プレイヤーをテレポート
        {
            Teleport();
        }



    }
}
