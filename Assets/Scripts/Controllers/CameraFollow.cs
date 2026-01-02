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

        // Flaga, czy limity są aktywne
        public bool enableLimits = true;

        private Transform mPlayer;
        private Transform mCurrentTarget;

        private void Awake()
        {
            mPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Start()
        {
            // Na starcie celujemy w gracza
            mCurrentTarget = mPlayer;
            enableLimits = true;
        }

        public void setPlace(Transform place)
        {
            mCurrentTarget = place;
            enableLimits = false;
        }

        public void resetPlace()
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

            if (enableLimits)
            {
                targetX = Mathf.Clamp(targetX, minXAndY.x, maxXAndY.x);
                targetY = Mathf.Clamp(targetY, minXAndY.y, maxXAndY.y);
            }

            bool isFollowingPlayer = (targetTransform == mPlayer);

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