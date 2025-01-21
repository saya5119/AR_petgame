using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MediaPipe.HandPose {

public sealed class HandAnimator : MonoBehaviour
{
        [SerializeField] public GameObject _jointPrefab;

        [SerializeField] public Transform _handParent;
        [SerializeField] public GameObject _spherejointPrefab;

        [SerializeField] public Transform _spherehandParent;
        [SerializeField] public GameObject _icejointPrefab;

        [SerializeField] public Transform _icehandParent;

        [SerializeField] public GameObject _chocolatejointPrefab;

        [SerializeField] public Transform _chocolatehandParent;
        [SerializeField] public GameObject _strewberryjointPrefab;

        [SerializeField] public Transform _strewberryhandParent;
        [SerializeField] public GameObject _cookiejointPrefab;

        [SerializeField] public Transform _cookiehandParent;
        [SerializeField] public GameObject _candyjointPrefab;

        [SerializeField] public Transform _candyhandParent;

        [SerializeField] CameraImageController _cameraTransfar = null;

        [SerializeField] ResourceSet _resources = null;
        [SerializeField] bool _useAsyncReadback = true;

        private HandPipeline _pipeline;

        public int handitem = 0;

        private Dictionary<HandPipeline.KeyPoint, GameObject> _handJoints =
            new Dictionary<HandPipeline.KeyPoint, GameObject>();

        void Start()
        {
            _pipeline = new HandPipeline(_resources);
            updateHandConfiguration();
            initalizeHandJoint();
        }

        private void OnDestroy()
        {
            _pipeline.Dispose();
        }

        private void LateUpdate()
        {
            _pipeline.UseAsyncReadback = _useAsyncReadback;
            var cameraTexture = _cameraTransfar.m_Texture;
            if (cameraTexture == null) return;
            _pipeline.ProcessImage(cameraTexture);

            //手の座標更新
            updateHandPose();
        }

        /// <summary>
        /// 手のパーツの初期化
        /// </summary>
        private void initalizeHandJoint()
        {
            for (int i = 0; i < HandPipeline.KeyPointCount; i++)
            {
                var go = Instantiate(_jointPrefab, _handParent);
                var keyPoint = (HandPipeline.KeyPoint)i;
                _handJoints.Add(keyPoint, go);
            }
        }

        /// <summary>
        /// 手の構成を更新
        /// </summary>
        private void updateHandConfiguration()
        {
            switch (handitem)
            {
                case 0:
                    _jointPrefab = _spherejointPrefab;
                    _handParent = _spherehandParent;
                    break;
                case 1:
                    _jointPrefab = _icejointPrefab;
                    _handParent = _icehandParent;
                    break;
                case 2:
                    _jointPrefab = _chocolatejointPrefab;
                    _handParent = _chocolatehandParent;
                    break;
                case 3:
                    _jointPrefab = _strewberryjointPrefab;
                    _handParent = _strewberryhandParent;
                    break;
                case 4:
                    _jointPrefab = _cookiejointPrefab;
                    _handParent = _cookiehandParent;
                    break;
                case 5:
                    _jointPrefab = _candyjointPrefab;
                    _handParent = _candyhandParent;
                    break;
                default:
                    Debug.LogWarning("Invalid handitem value.");
                    break;
            }

            // 現在の手のパーツを再初期化
            foreach (var joint in _handJoints.Values)
            {
                Destroy(joint);
            }
            _handJoints.Clear();
            initalizeHandJoint();
        }

        /// <summary>
        /// 手の座標更新
        /// </summary>
        private void updateHandPose()
        {
                var position = _pipeline.GetKeyPoint(0);
                var keyPoint = (HandPipeline.KeyPoint)0;
                var position2 = _pipeline.GetKeyPoint(9);
            //ワールド座標に変換する
                float xPos = Screen.width * normalize((position.x + position2.x) / 2.0f);
                float yPos = Screen.height * normalize((position.y + position2.y) / 2.0f);
                float zPos = 0;
                if(handitem == 0){
                    zPos = 0.5f + (position.z + position2.z) / 2.0f;
                }else{
                    zPos = 0.4f + (position.z + position2.z) / 2.0f;
                }
                Vector3 cameraPos = new Vector3(xPos, yPos, zPos);
                var screenPosition = Camera.main.ScreenToWorldPoint(cameraPos);
                //それぞれの手のパーツに座標を代入
                _handJoints[keyPoint].transform.position = screenPosition;

            //ローカル関数:座標の正規化
            float normalize(float value)
            {
                float min = -0.5f;
                float max = 0.5f;
                float cValue = Mathf.Clamp(value, min, max);
                return (cValue - min) / (max - min);
            }
        }
        // 外部から handitem を変更し構成を更新する
        public void SetHandItem(int newHandItem)
        {
            if (handitem != newHandItem)
            {
                handitem = newHandItem;
                updateHandConfiguration();
            }
        }
    }

} // namespace MediaPipe.HandPose


