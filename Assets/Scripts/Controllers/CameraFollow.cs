using System.Collections;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class CameraFollow : MonoBehaviour
    {
        public float xMargin = 1f;
        public float yMargin = 1f;
        public float xSmooth = 8f;
        public float ySmooth = 8f;
        public float zoomSmooth = 5f;
        public float zoomDefault = 1.0f;
        public Vector2 maxXAndY;
        public Vector2 minXAndY;
        
        [Header("Camera settings")] public bool enableLimits = true;
        public bool enableSmoothing = true;
        [Header("Look Ahead Settings")] public float lookAheadX = 1.5f;
        public float lookAheadY = .5f;

        private Transform mPlayer;
        private Rigidbody2D mPlayerBody;
        private PlayerController mPlayerCtrl;
        private Transform mCurrentTarget;
        private Vector3 currentShakeOffset = Vector3.zero;
        private Camera mCamera;
        public float mTargetZoom;

        private void Awake()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            mPlayer = playerObj.transform;
            mPlayerBody = playerObj.GetComponent<Rigidbody2D>();
            mPlayerCtrl = playerObj.GetComponent<PlayerController>();
            mCamera = GetComponent<Camera>();
            mTargetZoom = zoomDefault;
        }

        private void Start()
        {
            // Na starcie celujemy w gracza
            mCurrentTarget = mPlayer;
            enableLimits = true;
        }
        
        public void Shake(float timeInSeconds)
        {
            // Zatrzymujemy poprzednie trzęsienie, jeśli jakieś trwało
            StopAllCoroutines(); 
            StartCoroutine(DoShakeCoroutine(timeInSeconds));
        }
        
        private IEnumerator DoShakeCoroutine(float duration)
        {
            float elapsed = 0f;
            float intensity = GameManager.ShakeIntensity;
            float multiplier = 0.08f; 

            while (elapsed < duration)
            {
                float damper = 1.0f - Mathf.Clamp01(elapsed / duration);
                float x = Random.Range(-1f, 1f) * intensity * multiplier * damper;
                float y = Random.Range(-1f, 1f) * intensity * multiplier * damper;
                currentShakeOffset = new Vector3(x, y, 0);
                elapsed += Time.unscaledDeltaTime; 
                yield return null;
            }
            currentShakeOffset = Vector3.zero;
        }

        public void SetPlace(Transform place)
        {
            mCurrentTarget = place;
            enableLimits = false;
        }

        public void ResetPlace()
        {
            mCurrentTarget = mPlayer;
            enableLimits = true;
        }

        public void SetTempZoom(float zoom)
        {
            mTargetZoom = zoom;
        }

        public void ResetTempZoom()
        {
            mTargetZoom = zoomDefault;
        }

        private void Update()
        {
            HandleZoom();
            TrackTransform(mCurrentTarget);
        }
        
        private void HandleZoom()
        {
            if (Mathf.Abs(mCamera.orthographicSize - mTargetZoom) > 0.01f)
            {
                mCamera.orthographicSize = Mathf.Lerp(
                    mCamera.orthographicSize, 
                    mTargetZoom, 
                    zoomSmooth * Time.unscaledDeltaTime
                );
            }
        }

        private void TrackTransform(Transform targetTransform)
        {
            float targetX = targetTransform.position.x;
            float targetY = targetTransform.position.y;

            bool isFollowingPlayer = (targetTransform == mPlayer);

            if (isFollowingPlayer && Mathf.Abs(mPlayerBody.linearVelocity.x) > 0.1f)
            {
                float direction = Mathf.Sign(mPlayerBody.linearVelocity.x);
                targetX += direction * lookAheadX;
            }

            if (isFollowingPlayer && Mathf.Abs(mPlayerBody.linearVelocity.y) > 0.1f && mPlayerCtrl.isOnMovingPlatform)
            {
                float direction = Mathf.Sign(mPlayerBody.linearVelocity.y);
                targetY += direction * lookAheadY;
            }

            if (enableLimits)
            {
                targetX = Mathf.Clamp(targetX, minXAndY.x, maxXAndY.x);
                targetY = Mathf.Clamp(targetY, minXAndY.y, maxXAndY.y);
            }

            float currentX = transform.position.x;
            float currentY = transform.position.y;
            float finalX = targetX;
            float finalY = targetY;
            float xMarg = xMargin;
            float yMarg = yMargin;
            if (!isFollowingPlayer)
            {
                xMarg = 0;
                yMarg = 0;
            }

            if (enableSmoothing)
            {
                if (isFollowingPlayer || Mathf.Abs(currentX - targetX) > xMarg)
                    finalX = Mathf.Lerp(currentX, targetX, xSmooth * Time.unscaledDeltaTime);
                if (isFollowingPlayer || Mathf.Abs(currentY - targetY) > yMarg)
                    finalY = Mathf.Lerp(currentY, targetY, ySmooth * Time.unscaledDeltaTime);
            }
            
            transform.position = new Vector3(finalX, finalY, transform.position.z) + currentShakeOffset;
        }
    }
}