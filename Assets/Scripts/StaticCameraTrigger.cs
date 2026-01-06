using UnityEngine;
using UnityStandardAssets._2D;

namespace DefaultNamespace
{
    public class CameraTrigger : MonoBehaviour
    {
        [Header("Zmień kolizję na teren na który ma to działać, potem")]
        [Header("Edytuj obiekt (Transform), na który kamera ma wskazywać.\nJeśli nie ma to stwórz, najlepiej jakby był jako dziecko tego elementu")]
        public Transform focusPoint;

        [Tooltip("Czy ma triggerować na wejście z boxa")]
        public bool enter = true;
        [Tooltip("Czy ma triggerować na wyjście z boxa")]
        public bool exit = true;

        private CameraFollow mCameraFollow;

        private void Awake()
        {
            mCameraFollow = Camera.main.GetComponent<CameraFollow>();
            
            if (mCameraFollow == null)
            {
                Debug.LogError("Nie znaleziono skryptu CameraFollow na Main Camera!");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Sprawdź, czy to Gracz wszedł w strefę
            if (other.CompareTag("Player") && !enter)
            {
                mCameraFollow.SetPlace(focusPoint);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // Kiedy gracz wychodzi, kamera wraca do śledzenia gracza
            if (other.CompareTag("Player") && !exit)
            {
                mCameraFollow.ResetPlace();
            }
        }
    }
}