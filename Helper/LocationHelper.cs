using System;

namespace backend.Helpers 
{
    public class LocationHelper
    {
        //tính khoảng cách giữa 2 điểm (lat1, lon1) và (lat2, lon2) sử dụng công thức Haversine và trả về kết quả bằng mét
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371000; // Bán kính trái đất tính bằng mét
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRadians(double angle) => angle * Math.PI / 180;
    }
}