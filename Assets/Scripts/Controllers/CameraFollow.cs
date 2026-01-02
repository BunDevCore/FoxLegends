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

        [Header("Enable maxmin for x and y")] public bool enableLimits = true;
        [Header("Look Ahead Settings")] public float lookAheadFactor = 3f;

        private Transform mPlayer;
        private Rigidbody2D mPlayerBody;
        private Transform mCurrentTarget;

        private void Awake()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            mPlayer = playerObj.transform;
            mPlayerBody = playerObj.GetComponent<Rigidbody2D>();
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

        // Metody pomocnicze do marginesów
        private bool CheckXMargin(Vector3 targetPos)
        {
            return Mathf.Abs(transform.position.x - targetPos.x) > xMargin;
        }

        private bool CheckYMargin(Vector3 targetPos)
        {
            return Mathf.Abs(transform.position.y - targetPos.y) > yMargin;
        }

        private void TrackTransform(Transform targetTransform)
        {
            float targetX = targetTransform.position.x;
            float targetY = targetTransform.position.y;

            bool isFollowingPlayer = (targetTransform == mPlayer);

            if (isFollowingPlayer && Mathf.Abs(mPlayerBody.linearVelocity.x) > 0.1f)
            {
                float direction = Mathf.Sign(targetTransform.localScale.x);
                targetX += direction * lookAheadFactor;
            }

            if (enableLimits)
            {
                targetX = Mathf.Clamp(targetX, minXAndY.x, maxXAndY.x);
                targetY = Mathf.Clamp(targetY, minXAndY.y, maxXAndY.y);
            }


            float currentX = transform.position.x;
            float currentY = transform.position.y;
            float finalX = currentX;
            float finalY = currentY;

            if (isFollowingPlayer || Mathf.Abs(currentX - targetX) > xMargin)
                finalX = Mathf.Lerp(currentX, targetX, xSmooth * Time.deltaTime);

            if (isFollowingPlayer || Mathf.Abs(currentY - targetY) > yMargin)
                finalY = Mathf.Lerp(currentY, targetY, ySmooth * Time.deltaTime);

            transform.position = new Vector3(finalX, finalY, transform.position.z);
        }
    }
}