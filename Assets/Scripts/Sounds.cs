using UnityEngine;

public class Sounds : MonoBehaviour
{
    private static Sounds Instance;

    [SerializeField]
    private AudioClip movementSound;
    public static AudioClip MovementSound => Instance.movementSound;
    [SerializeField]
    private AudioClip buttonSound;
    public static AudioClip ButtonSound => Instance.buttonSound;
    [SerializeField]
    private AudioClip riseSound;
    public static AudioClip RiseSound => Instance.riseSound;
    [SerializeField]
    private AudioClip lowerSound;
    public static AudioClip LowerSound => Instance.lowerSound;
    [SerializeField]
    private AudioClip winSound;
    public static AudioClip WinSound => Instance.winSound;
    [SerializeField]
    private AudioClip loseSound;
    public static AudioClip LoseSound => Instance.loseSound;
    [SerializeField]
    private AudioClip damageSound;
    public static AudioClip DamageSound => Instance.damageSound;

    private void Start()
    {
        Instance = this;
    }
}
