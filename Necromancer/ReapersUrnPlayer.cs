using UnityEngine;

public class ReapersUrnPlayer : MonoBehaviour
{
    private bool playerON;
    [SerializeField] private ReapersUrn_Shoot shoot;

    void Update()
    {
        if (!playerON) shoot.Return();
    }
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            playerON = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            playerON = false;
        }
    }
}
