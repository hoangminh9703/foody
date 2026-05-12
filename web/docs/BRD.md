# BRD - Web bán hàng đồ ăn theo ngày

## 1. Tổng quan

### 1.1 Mục đích
Xây dựng một website giới thiệu cửa hàng bán đồ ăn theo ngày, hỗ trợ khách xem thông tin cửa hàng, xem menu món ăn theo ngày và gửi yêu cầu đặt cơm. Hệ thống đồng thời cung cấp khu vực quản trị để admin tiếp nhận, theo dõi và xử lý các request đặt hàng.

### 1.2 Mục tiêu kinh doanh
- Tăng khả năng giới thiệu thương hiệu và món ăn đến khách hàng mới.
- Giảm thời gian trao đổi thủ công khi nhận đặt cơm.
- Tập trung hóa việc quản lý request và menu theo từng ngày.
- Tạo trải nghiệm đặt món đơn giản, nhanh và rõ ràng cho khách.

### 1.3 Phạm vi sản phẩm
Sản phẩm tập trung vào 3 phần chính:
- Landing page giới thiệu cửa hàng.
- Form gửi yêu cầu đặt cơm.
- Dashboard admin để quản lý request và cập nhật menu.

## 2. Người dùng và vai trò

### 2.1 Khách truy cập
Người dùng lần đầu hoặc người xem thông tin cửa hàng. Nhu cầu chính là tìm hiểu món ăn, xem menu theo ngày và liên hệ hoặc gửi yêu cầu đặt cơm.

### 2.2 Khách đặt cơm
Người dùng có nhu cầu đặt cơm trưa hoặc cơm chiều. Vai trò này sử dụng form đặt hàng để gửi thông tin cá nhân, lựa chọn món và ghi chú.

### 2.3 Admin
Người quản lý hệ thống, chịu trách nhiệm:
- Cập nhật menu món ăn theo ngày.
- Xem và xử lý các request đặt cơm.
- Theo dõi trạng thái xử lý đơn.
- Quản lý nội dung hiển thị trên landing page nếu cần.

## 3. Chức năng chi tiết

### 3.1 Landing page
Landing page là trang chính để giới thiệu cửa hàng và dẫn người dùng tới hành động đặt cơm.

#### Nội dung chính
- Hero section với tên thương hiệu, thông điệp ngắn gọn và nút kêu gọi hành động.
- Giới thiệu ngắn về mô hình bán cơm theo ngày.
- Danh sách điểm nổi bật như món ngon, giao tiếp nhanh, menu thay đổi mỗi ngày.
- Khu vực hiển thị menu món ăn theo ngày hoặc theo bữa.
- Thông tin liên hệ cơ bản như số điện thoại, giờ nhận đơn, khu vực phục vụ.
- Nút dẫn tới form đặt cơm.

#### Yêu cầu hành vi
- Người dùng có thể xem nội dung mà không cần đăng nhập.
- Nội dung menu cần thể hiện rõ ngày áp dụng.
- CTA đặt cơm phải dễ nhìn và xuất hiện ở các vị trí hợp lý.

### 3.2 Form gửi yêu cầu đặt cơm
Form là điểm chuyển đổi chính của hệ thống.

#### Thông tin cần thu thập
- Họ và tên.
- Số điện thoại.
- Bữa ăn cần đặt: cơm trưa hoặc cơm chiều.
- Ngày đặt.
- Món ăn hoặc combo mong muốn.
- Số lượng phần ăn.
- Ghi chú thêm như ít cay, không hành, yêu cầu giao hàng, v.v.

#### Quy tắc nghiệp vụ
- Các trường bắt buộc phải được kiểm tra trước khi gửi.
- Số điện thoại phải đúng định dạng hợp lệ.
- Người dùng chỉ được chọn menu thuộc ngày tương ứng hoặc menu còn hiệu lực.
- Hệ thống phải lưu request kèm thời gian tạo để admin dễ tra cứu.

#### Phản hồi sau khi gửi
- Hiển thị thông báo gửi thành công.
- Cung cấp mã request hoặc trạng thái tiếp nhận nếu cần.
- Nếu gửi lỗi, hiển thị thông báo rõ ràng để người dùng nhập lại.

### 3.3 Xem menu món ăn theo ngày
Menu là dữ liệu thay đổi thường xuyên, phục vụ cho cả khách xem và admin quản lý.

#### Chức năng cần có
- Hiển thị danh sách món ăn theo từng ngày.
- Phân biệt menu cho cơm trưa và cơm chiều nếu áp dụng.
- Cho phép xem món chính, món phụ, giá hoặc ghi chú đặc biệt.
- Cho phép admin cập nhật nội dung menu nhanh chóng.

#### Yêu cầu hiển thị
- Menu phải dễ đọc trên cả desktop và mobile.
- Ngày áp dụng phải được hiển thị rõ ràng để tránh nhầm lẫn.
- Nếu không có menu cho ngày hiện tại, cần hiển thị trạng thái phù hợp như "đang cập nhật".

### 3.4 Dashboard admin
Khu vực quản trị phục vụ việc vận hành hằng ngày.

#### Quản lý request
- Danh sách request theo thời gian tạo.
- Tìm kiếm hoặc lọc theo trạng thái, ngày, loại bữa ăn.
- Xem chi tiết request.
- Cập nhật trạng thái xử lý, ví dụ: mới tạo, đã liên hệ, đã xác nhận, đã hoàn tất, đã hủy.

#### Quản lý menu
- Tạo, sửa, xóa menu theo ngày.
- Gán menu cho bữa trưa hoặc bữa chiều.
- Đánh dấu menu đang hoạt động.

#### Quản lý nội dung cơ bản
- Cập nhật nội dung banner, giới thiệu và thông tin liên hệ nếu cần.

## 4. User Flow

### 4.1 Luồng khách xem và đặt cơm
1. Khách truy cập landing page.
2. Khách xem giới thiệu cửa hàng và menu theo ngày.
3. Khách chọn nút đặt cơm.
4. Khách điền form yêu cầu đặt hàng.
5. Hệ thống kiểm tra dữ liệu và gửi request.
6. Hệ thống thông báo đã nhận yêu cầu.
7. Admin tiếp nhận request từ dashboard.

### 4.2 Luồng admin xử lý request
1. Admin đăng nhập vào khu vực quản trị.
2. Admin xem danh sách request mới.
3. Admin mở chi tiết request để kiểm tra thông tin khách và món đã chọn.
4. Admin cập nhật trạng thái xử lý.
5. Admin liên hệ khách nếu cần xác nhận thêm.
6. Admin hoàn tất hoặc hủy request khi kết thúc xử lý.

### 4.3 Luồng cập nhật menu
1. Admin đăng nhập dashboard.
2. Admin tạo hoặc chỉnh sửa menu cho ngày hiện tại hoặc ngày sắp tới.
3. Admin lưu menu và đánh dấu trạng thái hoạt động.
4. Landing page hiển thị menu mới cho khách truy cập.

## 5. Yêu cầu kỹ thuật

### 5.1 Kiến trúc hệ thống
- Tách rõ front-end cho landing page và admin dashboard.
- Có backend API để xử lý request, menu và dữ liệu quản trị.
- Dữ liệu cần được lưu trong cơ sở dữ liệu tập trung.

### 5.2 API và dữ liệu
Hệ thống cần tối thiểu các nhóm API sau:
- API lấy thông tin landing page.
- API lấy menu theo ngày.
- API tạo request đặt cơm.
- API quản lý request cho admin.
- API quản lý menu cho admin.
- API xác thực đăng nhập admin.

### 5.3 Gợi ý mô hình dữ liệu
Các thực thể dữ liệu chính nên gồm:
- User/Admin.
- Menu.
- MenuItem.
- OrderRequest.
- OrderRequestItem nếu cần lưu chi tiết món đã chọn.
- ContactInfo hoặc SiteContent nếu quản lý nội dung landing page.

### 5.4 Tích hợp và vận hành
- Hỗ trợ gửi thông báo nội bộ hoặc email nếu được bổ sung sau.
- Có cơ chế ghi log lỗi ở mức phù hợp để theo dõi vận hành.
- Hỗ trợ sao lưu dữ liệu định kỳ nếu triển khai production.

## 6. Yêu cầu phi chức năng

### 6.1 Hiệu năng
- Trang landing page phải tải nhanh trên thiết bị di động.
- Form đặt cơm cần phản hồi nhanh sau khi gửi.
- Dashboard admin cần thao tác mượt với danh sách request có kích thước vừa phải.

### 6.2 Trải nghiệm người dùng
- Giao diện modern, clean, dùng màu trắng, vàng và đen theo brief.
- Form nhập liệu rõ ràng, ít bước, dễ hoàn thành.
- Nội dung quan trọng phải dễ thấy trên màn hình nhỏ.

### 6.3 Tính ổn định
- Hệ thống phải hạn chế lỗi khi thiếu dữ liệu menu hoặc request.
- Các thao tác cập nhật cần có thông báo thành công/thất bại rõ ràng.

### 6.4 Bảo mật
- Admin phải được xác thực trước khi truy cập dashboard.
- Dữ liệu đầu vào từ form phải được kiểm tra và làm sạch trước khi lưu.
- Các API quản trị phải giới hạn quyền truy cập theo vai trò.

### 6.5 Khả năng mở rộng
- Thiết kế đủ linh hoạt để sau này bổ sung thanh toán, thông báo tự động hoặc theo dõi trạng thái đơn hàng.
- Cấu trúc dữ liệu nên hỗ trợ thêm nhiều loại menu hoặc nhiều khung giờ bán hàng.

### 6.6 Tương thích
- Hoạt động tốt trên desktop, tablet và mobile.
- Giao diện cần tương thích với các trình duyệt phổ biến hiện nay.

## 7. Giả định và phạm vi chưa bao gồm

### 7.1 Giả định
- Khách hàng sẽ đặt cơm trực tiếp qua form thay vì đăng ký tài khoản.
- Admin là người duy nhất cần quyền quản trị.
- Menu thay đổi theo ngày và được admin cập nhật thủ công.

### 7.2 Chưa bao gồm trong phạm vi hiện tại
- Thanh toán online.
- Tài khoản khách hàng.
- Tự động giao hàng hoặc định tuyến shipper.
- Hệ thống tích điểm, voucher hoặc khuyến mãi phức tạp.

