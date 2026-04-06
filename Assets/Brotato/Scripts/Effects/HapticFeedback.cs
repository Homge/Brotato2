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

        CandyCoded.HapticFeedback.HapticFeedback.LightFeedback();
    }
}