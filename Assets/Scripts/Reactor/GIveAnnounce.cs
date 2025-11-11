using UnityEngine;

public class GIveAnnounce : MonoBehaviour
{
    [Header("Nội dung Thông báo")]
    public string announcementMessage;
    private bool hasAnnounced = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!hasAnnounced)
            {
                // Gửi thông báo
                Debug.Log("in rahhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh");
                MessageNPC.Instance.Show(announcementMessage);
                Debug.Log("kết thúchhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh");

                hasAnnounced = true;

            }
        }
    }
   
}
