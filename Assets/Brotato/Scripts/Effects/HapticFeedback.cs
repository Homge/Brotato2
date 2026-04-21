using UnityEngine;

public class HapticFeedback : MonoBehaviour
{
    private void Awake()
    {
       
        PlayerHealth.onPlayerTookDamage += VibrateOnDamageTaken;
    }

    private void OnDestroy()
    {
 
        PlayerHealth.onPlayerTookDamage -= VibrateOnDamageTaken;
    }

    private void VibrateOnDamageTaken()
    {

       #if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        Handheld.Vibrate(); 
        #endif
    }
}