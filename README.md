# **Project 2 — 8 Puzzle**
* 1712774 Nguyễn Chí Thành
* 1712800 Mai Huy Thông
> Chương trình được thiết kế bằng **WPF** dựa trên nền tảng **ngôn ngữ lập trình C#**. Người chơi chọn một tập tin hình ảnh, chương trình sẽ cắt từ hình gốc ra 9 hình nhỏ rời nhau với vị trí ngẫu nhiên.<br>
Một trong 9 mảnh (chủ yếu ở góc trái dưới hoặc phải dưới) sẽ bị bỏ ra làm ô trống.<br>
Người chơi di chuyển mỗi lần một ô trong các ô còn lại để thay thế cho ô trống sao cho toàn bộ 8 ô còn lại vào đúng vị trí cuối cùng.

<br> Đây là các source tham khảo trong quá trình thực hiện project
<br> **Reference** [https://github.com/DianaSensei/8_Puzzle](https://github.com/DianaSensei/8_Puzzle) 
<br> **Video demo: [https://youtu.be/x5IAa8psys8](https://youtu.be/x5IAa8psys8)**

## **Những chức năng đã làm được**

### **I. Nhóm chức năng chính** 
----
Các chức năng theo yêu cầu của project và điểm tự đánh giá:
1. Chọn hình để cắt ra 8 ô và hiển thị 8 ô này theo một trật tự **ngẫu nhiên** (cần hiển thị thêm hình gốc ban đầu để người chơi biết) (2 điểm)
2. **Kéo thả** một ô bằng chuột để thay thế cho ô trống (2 điểm)
3. Khi kéo thả tự động **snap** vào ô gần nhất chứ không cần phải kéo chính xác (1 điểm)
4. Sử dụng **4 button** (4 nút điều hướng trên keyboard) để điểu khiển với 4 hướng trái, phải, lên, xuống để tự động di chuyển một ô thay thế ô trống (1 điểm)
5. Khi bắt đầu chơi game, bật đồng hồ đếm ngược thời gian ví dụ chỉ cho 3 phút (1 điểm)
6. Kiểm tra kết quả cuối cùng, thông báo chiến thắng hoặc thua cuộc nếu hết giờ (1 điểm)
7. Giao diện gọn gàng, tươm tất, có bắt lỗi dữ liệu nhập đầy đủ (1 điểm)
8. Lưu và nạp lại game đang chơi dở dang để chơi tiếp – Chỉ yêu cầu lưu được 1 game là đủ không cần phải lưu nhiều game (1 điểm)
9. Những đặc điểm dặc biệt ở **phần II**

### **II. Các đặc điểm và chức năng khác**
----
* Có thêm chức năng puzzle thứ 3 là **click chuột trái vào mỗi ảnh bị cắt** cũng có thể di chuyển ảnh giữa các ô
* Làm mới lại game bằng cách nhấn nút **Refresh** ở trên thanh menu
* Các chức năng đã được test qua, chi tiết có thể xem ở video demo: [Click here](https://youtu.be/x5IAa8psys8)