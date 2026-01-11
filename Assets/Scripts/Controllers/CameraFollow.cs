using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class CameraFollow : MonoBehaviour
    {
        public float xMargin = 1f;
        public float yMargin = 1f;
        public float xSmooth = 8f;
        public float ySmooth = 8f;
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

        private void Awake()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            mPlayer = playerObj.transform;
            mPlayerBody = playerObj.GetComponent<Rigidbody2D>();
            mPlayerCtrl = playerObj.GetComponent<PlayerController>();
        }

        private void Start()
        {
            // Na starcie celujemy w gracza
            mCurrentTarget = mPlayer;
            enableLimits = true;
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

        private void Update()
        {
            TrackTransform(mCurrentTarget);
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

            transform.position = new Vector3(finalX, finalY, transform.position.z);
        }
    }
}