using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MediaPipe.HandPose {

public sealed class HandAnimator : MonoBehaviour
{
        [SerializeField] private GameObject _jointPrefab;

        [SerializeField] private Transform _handParent;

        [SerializeField] CameraImageController _cameraTransfar = null;

        [SerializeField] ResourceSet _resources = null;
        [SerializeField] bool _useAsyncReadback = true;

        private HandPipeline _pipeline;

        private Dictionary<HandPipeline.KeyPoint, GameObject> _handJoints =
            new Dictionary<HandPipeline.KeyPoint, GameObject>();

        void Start()
        {
            _pipeline = new HandPipeline(_resources);
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
                float zPos = 0.5f + (position.z + position2.z) / 2.0f;
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
    }

} // namespace MediaPipe.HandPose


