using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using MediaPipe.HandPose;

public class Character : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float walkingSpeed = 0.1f;
    [SerializeField] private float runningSpeed = 0.2f;
    private ARPlaneManager arPlaneManager;
    private Vector3 _destination;
    private ARPlane arPlane;
    private string trackedPlaneId;
    private bool stop = false;
    private Camera arcamera;
    private GameObject body;
    private SkinnedMeshRenderer bodyRenderer;
    private GameObject eye;
    private SkinnedMeshRenderer eyeRenderer;
    private GameObject acs;
    private SkinnedMeshRenderer acsRenderer;
    private float m_weight = 0.0f;
    public float starttime = 0f;
    private bool finishtouch = false;
    public bool tocameramove = false;
    private bool timestop = false;
    [SerializeField] Material[] materialArray = new Material[4];
    public GameObject happyeffect;
    public GameObject changeeffect;
    private GameObject effect;
    private GameObject effect2;

    private bool effectstop = false;
    private bool eating = false;
    public float speed = 1f; // パクパクの速度
    private int foodcount = 0;
    private bool materialchange = false;
    private HandAnimator HandAnimatorscript;
    private GameObject hand6;
    private GameObject food;
    private bool iceeat = false;
    void Start()
    {
        animator = GetComponent<Animator>();
        arPlaneManager = FindObjectOfType<ARPlaneManager>();
        arcamera = Camera.main;
        body = transform.Find("body").gameObject;
        bodyRenderer = body.GetComponent<SkinnedMeshRenderer>();
        eye = transform.Find("eye").gameObject;
        eyeRenderer = eye.GetComponent<SkinnedMeshRenderer>();
        acs = transform.Find("acs").gameObject;
        acsRenderer = acs.GetComponent<SkinnedMeshRenderer>();
        SetNewDestination();
    }

    void Update()
    {
        if(finishtouch && Time.time - starttime > 0.6f && !effectstop){
            Vector3 newPosition = new Vector3(transform.position.x, transform.position.y + 0.06f, transform.position.z + 0.02f);
            // エフェクトを生成
            effect = Instantiate(happyeffect, newPosition, Quaternion.identity);
            effectstop = true;
        }
        if(finishtouch && Time.time - starttime >2.5f){
            Destroy(effect);
            m_weight = 0f;
            eyeRenderer.SetBlendShapeWeight(0, m_weight);
            stop = false;
            finishtouch = false;
            effectstop = false;
            SetNewDestination();
        }
        if(eating){
            // 1.5往復（3秒）の間だけ動く
            if (Time.time - starttime <= 3.0f)
            {
                // 0から100を往復するように設定
                m_weight = Mathf.PingPong((Time.time - starttime)*300, 100f);

                // blend shape を更新
                bodyRenderer.SetBlendShapeWeight(2, m_weight);
            }else if(Time.time - starttime > 3.0f && Time.time - starttime <= 5.0f && !materialchange){
                //検索
                food = GameObject.Find("food");
                hand6 = GameObject.Find("HandTrackingController");
                HandAnimatorscript = hand6.GetComponent<HandAnimator>();
                HandAnimatorscript.SetHandItem(0);
                food.SetActive(false);
                m_weight = 0f;
                bodyRenderer.SetBlendShapeWeight(2, m_weight);
                bodyRenderer.SetBlendShapeWeight(1, m_weight);
                Vector3 newPosition = new Vector3(transform.position.x, transform.position.y + 0.06f, transform.position.z + 0.02f);
                // エフェクトを生成
                effect2 = Instantiate(changeeffect, newPosition, Quaternion.identity);
                //カウントを入れる
                bodyRenderer.material = materialArray[foodcount];
                eyeRenderer.material = materialArray[foodcount];
                acsRenderer.material = materialArray[foodcount];
                materialchange = true;
            }else if(Time.time - starttime > 5.0f && Time.time - starttime <= 7.0f){
                Destroy(effect2);
            }else if(Time.time - starttime > 7.0f){
                eating = false;
                stop = false;
                materialchange = false;
                SetNewDestination();
            }
        }
        if(iceeat){
            // 0から100を往復するように設定
                m_weight = Mathf.PingPong((Time.time - starttime)*300, 100f);
                // blend shape を更新
                bodyRenderer.SetBlendShapeWeight(2, m_weight);
        }
        if(tocameramove){
            stop = true;
            if (arcamera == null) return;
            // カメラの方向を向く（高さを固定）
            Vector3 directionToCamera = arcamera.transform.position - transform.position;
            directionToCamera.y = 0; // 高さの方向を無視する

            float targetYRotation = Quaternion.LookRotation(directionToCamera.normalized).eulerAngles.y;
            transform.rotation = Quaternion.Euler(0, targetYRotation, 0);
            // カメラに十分近づいたら停止する
            if (directionToCamera.magnitude < 0.15f)
            {
                if(!timestop){
                    starttime = Time.time;
                    timestop = true;
                }
            }else{
                // カメラに向かって移動する
                transform.position += directionToCamera.normalized * runningSpeed * Time.deltaTime;
            }
            if(Time.time - starttime > 10f){
                starttime = Time.time;
                tocameramove = false;
                stop = false;
                timestop = false;
                SetNewDestination();
            }
        }
        if(stop) return;
        var distance = Vector3.Distance(transform.position, _destination);

        if (distance <= 1f)
        {
            animator.SetBool("walking", false);
            SetNewDestination();
        }
        else
        {
            animator.SetBool("walking", true);
            MoveTowardsDestination(distance);
        }
    }

    private void SetNewDestination()
    {
        if(string.IsNullOrEmpty(trackedPlaneId))
        {
            foreach (var plane in arPlaneManager.trackables)
            {
                if (plane.boundary.Length > 0)
                {
                    arPlane = plane; // 平面を選択
                    trackedPlaneId = arPlane.trackableId.ToString();
                    break; // 最初に見つかった平面を使用
                }
            }
        }else{
            foreach (var plane in arPlaneManager.trackables)
            {
                if (plane.trackableId.ToString() == trackedPlaneId)
                {
                    arPlane = plane; // 平面を選択
                    break;
                }
            }
        }
        
        if (arPlane == null || arPlane.boundary == null || arPlane.boundary.Length == 0) return;

        // 平面のboundaryからランダムな頂点を選択
        var boundary = arPlane.boundary;
        var randomIndex = Random.Range(0, boundary.Length);
        var randomPoint = boundary[randomIndex];

        _destination = arPlane.transform.TransformPoint(randomPoint);
    }

    private void MoveTowardsDestination(float distance)
    {
        float step = walkingSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _destination, step);

        Vector3 lookAtTarget = new Vector3(_destination.x, transform.position.y, _destination.z);
        Vector3 direction = lookAtTarget - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
    }
    void OnTriggerEnter(Collider collision)
    {
        stop = true;
        animator.SetBool("walking", false);
        if (arcamera == null) return;
        // カメラの方向を向く（高さを固定）
        Vector3 directionToCamera = arcamera.transform.position - transform.position;
        directionToCamera.y = 0; // 高さの方向を無視する

        float targetYRotation = Quaternion.LookRotation(directionToCamera.normalized).eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, targetYRotation, 0);
        m_weight = 100.0f;
        if(!eating){
            if(collision.gameObject.tag == "candy"){
                foodcount = 0;
                bodyRenderer.SetBlendShapeWeight(1, m_weight);
                starttime = Time.time;
                eating = true;
            }else if(collision.gameObject.tag == "choco"){
                foodcount = 1;
                bodyRenderer.SetBlendShapeWeight(1, m_weight);
                starttime = Time.time;
                eating = true;
            }else if(collision.gameObject.tag == "strewberry"){
                foodcount = 2;
                bodyRenderer.SetBlendShapeWeight(1, m_weight);
                starttime = Time.time;
                eating = true;
            }else if(collision.gameObject.tag == "cookie"){
                foodcount = 3;
                bodyRenderer.SetBlendShapeWeight(1, m_weight);
                starttime = Time.time;
                eating = true;
            }else if(collision.gameObject.tag == "ice"){
                //数字はブレンドシェイプの数(0スタート)
                bodyRenderer.SetBlendShapeWeight(1, m_weight);
                starttime = Time.time;
                iceeat = true;
            }else{
                //数字はブレンドシェイプの数(0スタート)
                bodyRenderer.SetBlendShapeWeight(3, m_weight);
                eyeRenderer.SetBlendShapeWeight(1, m_weight);
            }
        }
    }
    void OnTriggerExit(Collider collision){
        if(!eating && !iceeat){
        m_weight = 0f;
        bodyRenderer.SetBlendShapeWeight(3, m_weight);
        eyeRenderer.SetBlendShapeWeight(1, m_weight);
        m_weight = 100.0f;
        eyeRenderer.SetBlendShapeWeight(0, m_weight);
        starttime = Time.time;
        finishtouch = true;
        }
        if(iceeat){
            m_weight = 0f;
            bodyRenderer.SetBlendShapeWeight(1, m_weight);
            bodyRenderer.SetBlendShapeWeight(2, m_weight);
            iceeat = false;
        }
    }
}