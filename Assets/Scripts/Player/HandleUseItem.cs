using UnityEngine;
using UnityEngine.UI;

public class HandleUseItem : MonoBehaviour
{
    public RecylableInventoryManager inventoryManager;
    public PlayerBase player;
    public Image Mu;
    public Image Ao;
    public Image Quan;
    public Image Gang;
    public Image Giay;

    [Header("Chỉ số cộng thêm khi mặc trang bị")]

    public int ChiSoGang = 25;
    public int ChiSoAo = 10;
    public int ChiSoQuan = 50;
    public int ChiSoGiay = 2;
    public int ChiSoMu = 50;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UseItem(ItemData item)
    {
        int x = inventoryManager.getQuantity(item);
        Debug.Log("Using item: " + item.itemName + " Số lượng :" + x );
        if (x > 0)
        {
                switch (item.itemName)
            {
                case "Găng Tay Sắt":
                    if (Gang.sprite == null)
                    {
                        Gang.sprite = item.icon;
                        player.Dame(ChiSoGang);
                    }
                    else
                    {
                        Gang.sprite = null;
                        player.Dame(-ChiSoGang);
                    }
                    Debug.Log("Dùng Găng");
                    break;

                case "Giáp Sắt":
                    if (Ao.sprite == null)
                    {
                        Ao.sprite = item.icon;
                        player.Defend(ChiSoAo);
                    }
                    else
                    {
                        Ao.sprite = null;
                        player.Defend(-ChiSoAo);
                    }

                    Debug.Log("Dùng giáp");
                    break;

                case "Giày Sắt":
                    if (Giay.sprite == null)
                    {
                        Giay.sprite = item.icon;
                        player.Speed(ChiSoGiay);
                    }
                    else
                    {
                        Giay.sprite = null;
                        player.Speed(-ChiSoGiay);
                    }
                        Debug.Log("Dùng giày");
                    break;

                case "Mũ Sắt":
                    if (Mu.sprite == null)
                    {
                        Mu.sprite = item.icon;
                        player.Defend(ChiSoMu);
                    }
                    else
                    {
                        Mu.sprite = null;
                        player.Defend(-ChiSoMu);
                    }
                    Debug.Log("Dùng Mũ");
                    break;

                case "Quần Giáp Sắt":
                    if (Quan.sprite == null)
                    {
                        Quan.sprite = item.icon;
                        player.Heal(ChiSoQuan);
                    }
                    else
                    {
                        Quan.sprite = null;
                        player.Heal(-ChiSoQuan);
                    }
                    Debug.Log("Dùng Quần");
                    break;

                default:
                    Debug.Log("⚠️ Item chưa được định nghĩa hành động!");
                    break;
            }
        }
        // Implement item usage logic here
    }
}
