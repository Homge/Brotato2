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
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
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
        if (SystemInfo.deviceType == DeviceType.Desktop)
            return GetDesktopMoveVector();
        else
            return playerJoystick.GetMoveVector();
    }

    private Vector2 GetDesktopMoveVector()
    {
        return movevement.ReadValue<Vector2>();
    }
    
}
