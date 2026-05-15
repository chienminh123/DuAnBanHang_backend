using backend.Data; // Thêm thư viện DB
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace backend.Controllers.Client
{
    [Route("api/client/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ShopContext _context; 

        private const string LALAMOVE_API_KEY = "pk_test_5a6ed8f1c57069a904ae968a024217ae";
        private const string LALAMOVE_API_SECRET = "sk_test_PoMyGMoB23qnMdrEuNt6VzzLzaiAC62kxfAeKOzsqQqDCcZCbMOzjGhZ8hAl8Ssc";
        private const string BASE_URL = "https://rest.sandbox.lalamove.com/v3";

        public DeliveryController(IHttpClientFactory httpClientFactory, ShopContext context)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
        }

        // Thêm tham số shopId, customerName, customerPhone
        [HttpGet("estimate-fee")]
        public async Task<IActionResult> EstimateFee(string destination, string lat, string lng, int shopId, string customerName, string customerPhone)
        {
            if (string.IsNullOrWhiteSpace(destination)) return Ok(new { fee = 15000 });

            if (string.IsNullOrEmpty(lat) || string.IsNullOrEmpty(lng))
            {
                lat = "21.003117";
                lng = "105.820140";
            }

            try
            {
                // LẤY THÔNG TIN CỬA HÀNG TỪ DATABASE
                var shop = await _context.cuaHangs.FindAsync(shopId);
                if (shop == null || !shop.Latitude.HasValue || !shop.Longitude.HasValue)
                {
                    return BadRequest("Không tìm thấy tọa độ cửa hàng.");
                }

                // Xử lý thông tin khách (Nếu rỗng thì để tạm)
                string cName = string.IsNullOrEmpty(customerName) ? "Khách hàng" : customerName;
                string cPhone = string.IsNullOrEmpty(customerPhone) ? "+84999999999" : customerPhone;

                // Chuẩn hóa sđt cho Lalamove (họ yêu cầu có mã quốc gia, ví dụ +84)
                if (cPhone.StartsWith("0")) cPhone = "+84" + cPhone.Substring(1);

                string latShop = shop.Latitude.Value.ToString("0.######", CultureInfo.InvariantCulture).Replace(",", ".");
                string lngShop = shop.Longitude.Value.ToString("0.######", CultureInfo.InvariantCulture).Replace(",", ".");

                double.TryParse(lat.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedLat);
                double.TryParse(lng.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedLng);
                string latClient = parsedLat == 0 ? "21.003117" : parsedLat.ToString("0.######", CultureInfo.InvariantCulture);
                string lngClient = parsedLng == 0 ? "105.820140" : parsedLng.ToString("0.######", CultureInfo.InvariantCulture);
                //DÙNG DỮ LIỆU THẬT CHO PAYLOAD
                var payload = new
                {
                    data = new
                    {
                        serviceType = "MOTORCYCLE",
                        language = "vi_VN",
                        stops = new[]
                        {
                            new {
                                coordinates = new { lat = latShop, lng = lngShop },
                                address = shop.ShopAddress ?? "Hà Nội"
                            },
                            new {
                                coordinates = new { lat = latClient, lng = lngClient },
                                address = destination
                            }
                        },
                        // Thêm thuộc tính item (bắt buộc của Lalamove khi giao đồ ăn)
                        item = new
                        {
                            quantity = "1",
                            weight = "LESS_THAN_3_KG",
                            categories = new[] { "FOOD_DELIVERY" },
                            handlingInstructions = new[] { "KEEP_UPRIGHT" }
                        }
                        
                    }
                };

                
                var options = new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
                string jsonBody = JsonSerializer.Serialize(payload, options);

                string path = "/v3/quotations";
                string method = "POST";
                string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

                
                string rawSignature = $"{timestamp}\r\n{method}\r\n{path}\r\n\r\n{jsonBody}";
                string signature = CreateHmacSignature(LALAMOVE_API_SECRET, rawSignature);

                var client = _httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Post, BASE_URL + path)
                {
                    Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                };

                request.Headers.Add("Authorization", $"hmac {LALAMOVE_API_KEY}:{timestamp}:{signature}");
                request.Headers.Add("Market", "VN");
                request.Headers.Add("Request-ID", Guid.NewGuid().ToString());

                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(content);
                    var totalStr = doc.RootElement.GetProperty("data").GetProperty("priceBreakdown").GetProperty("total").GetString();
                    if (double.TryParse(totalStr, out double totalFee))
                    {
                        return Ok(new { fee = totalFee });
                    }
                }

                Console.WriteLine("Lalamove Error: " + content);
                return Ok(new { fee = 15000 });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lalamove Exception: " + ex.Message);
                return Ok(new { fee = 15000 });
            }
        }

        private string CreateHmacSignature(string secret, string message)
        {
            var encoding = new UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);

            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashMessage = hmacsha256.ComputeHash(messageBytes);
                return BitConverter.ToString(hashMessage).Replace("-", "").ToLower();
            }
        }
    }
}