# The Last Dawn – 2D Action Adventure Game

**The Last Dawn** là game 2D pixel-art phong cách side-scroller, nơi người chơi hóa thân vào một nhân vật sống sót cuối cùng lang thang trong thế giới đổ nát sau tận thế, chiến đấu với quái vật và khám phá bí mật phía sau “bình minh cuối cùng”.

Game được phát triển bằng **Unity (C#)**, tập trung vào cảm giác điều khiển mượt, combat đơn giản nhưng “đã tay” và không khí u ám – cô độc.

---

## 🎮 Gameplay Overview

- Thể loại: **2D Action Adventure / Platformer**
- Góc nhìn: **Side-scrolling**
- Phong cách đồ họa: **Pixel-art**
- Người chơi:
  - Điều khiển nhân vật di chuyển qua nhiều khu vực khác nhau.
  - Né chướng ngại, vượt platform, chiến đấu với quái.
  - Thu thập tài nguyên / item để sinh tồn.

---

## ✨ Tính năng chính

### 1. Hệ thống nhân vật

- Di chuyển trái/phải, nhảy, rơi, rơi tự do.
- Combat cơ bản:
  - Đánh cận chiến (melee).
  - Có khung hitbox / hurtbox rõ ràng.
- Thanh máu (HP), sát thương, trạng thái chết / hồi sinh.

### 2. Bản đồ & màn chơi

- Nhiều map / khu vực khác nhau (rừng, tàn tích, khu vực hoang tàn…).
- Cơ chế **cổng dịch chuyển / cửa** (portal/gate) giữa các map.
- Mỗi map có:
  - Checkpoint / spawn point.
  - Kẻ địch riêng / bố cục chướng ngại riêng.

### 3. Kẻ địch & AI

- Nhiều loại quái với:
  - Máu, damage, tốc độ di chuyển khác nhau.
  - Hành vi cơ bản: tuần tra, phát hiện người chơi, tấn công.
- Kẻ địch rơi loot (vàng, item, vật phẩm nâng cấp).

### 4. Hệ thống item / inventory *(mức cơ bản)*

- Thu thập vật phẩm trên đường đi (vàng, potion, item hỗ trợ).
- Inventory đơn giản để xem item đang sở hữu.
- Một số item có thể:
  - Hồi máu.
  - Tăng chỉ số tạm thời (tùy thiết kế).

### 5. UI & UX

- HUD in-game:
  - Thanh máu.
  - (Tuỳ chọn) thanh stamina / mana.
- Màn hình:
  - Main Menu (Play, Settings, Quit).
  - Pause Menu.
  - Game Over.

---

## 🕹️ Điều khiển (mặc định – có thể thay đổi)

- **A / D** hoặc **← / →** – Di chuyển trái/phải.
- **Space** – Nhảy.
- **J / K** – Tấn công / kỹ năng (tuỳ thiết kế).
- **Esc** – Pause game.

---

## 🧱 Công nghệ & cấu trúc dự án

- **Engine:** Unity (phiên bản khuyến nghị: `2021.x` trở lên)
- **Ngôn ngữ:** C#
- **Mô hình tổ chức code:**
  - Tách riêng:
    - `Player` (movement, combat, animation).
    - `Enemy` (AI, patrol, attack).
    - `Map / Gate / Portal` (quản lý chuyển scene).
    - `GameManager / UIManager` (quản lý flow & UI).

Ví dụ cấu trúc folder:

```text
Assets/
 ├─ Scripts/
 │   ├─ Player/
 │   ├─ Enemy/
 │   ├─ Managers/
 │   ├─ UI/
 │   └─ Environment/
 ├─ Art/
 │   ├─ Characters/
 │   ├─ Enemies/
 │   └─ Tilesets/
 ├─ Prefabs/
 ├─ Scenes/
 │   ├─ MainMenu.unity
 │   ├─ Level_01.unity
 │   └─ Level_02.unity
 └─ UI/
