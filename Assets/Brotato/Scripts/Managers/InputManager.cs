using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    [Header(" Elements ")]
    [SerializeField] private MobileJoystick playerJoystick;
    [SerializeField] private InputActionAsset actions;

     [Header(" Input Actions ")]
    private InputAction movevement;
    
    private void Awake()
    {
        instance = this;
        movevement = actions.FindAction("Movement");

        actions.Enable();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   public Vector2 GetMoveVector()
{
    Vector2 move;
    if (SystemInfo.deviceType == DeviceType.Desktop)
        move = GetDesktopMoveVector();
    else
        move = playerJoystick.GetMoveVector();

    // Clamp magnitude về tối đa 1 để tránh chạy nhanh hơn bình thường
    return Vector2.ClampMagnitude(move, 1f);
}

    private Vector2 GetDesktopMoveVector()
    {
        return movevement.ReadValue<Vector2>();
    }
    
}
