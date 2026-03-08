using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    [Header(" Elements ")]
    [SerializeField] private MobileJoystick playerJoystick;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
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
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}
