using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour, IPointerMoveHandler
{
    [SerializeField] private Texture2D cursorPointTexture;

    public void OnPointerMove(PointerEventData eventData)
    {
        GameObject gameObject = eventData.pointerCurrentRaycast.gameObject;
        if (gameObject != null &&
            (gameObject.GetComponentInParent<Button>() != null || gameObject.GetComponentInParent<Slider>() != null))
            Cursor.SetCursor(cursorPointTexture, Vector2.zero, CursorMode.Auto);
        else
            ResetCursor();
    }

    public void ResetCursor() => Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

    public void HideAndResetCursor()
    {
        ResetCursor();
        HideCursor();
    }

    public void HideCursor() => Cursor.visible = false;

    public void ShowCursor()
    {
        Mouse.current.WarpCursorPosition(new Vector2(Screen.width / 2f, Screen.height / 2f));
        Cursor.visible = true;
    }
}