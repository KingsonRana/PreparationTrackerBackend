using PreparationTracker.DTO.RequestDTO;
using System.Text.RegularExpressions;

namespace PreparationTracker.Utilities
{
    public static class UserUtilities
    {
        public static bool verifyRequiredFields(UserSignupRequestDto user)
        {
            if (user == null)
            {
                Console.WriteLine("_______User is null__________");
                return false;
            }
            else if (string.IsNullOrWhiteSpace(user.Name))
            {
                Console.WriteLine("_______User name null or empty__________");
                return false;
            }
            else if (!IsValidEmail(user.Email))
            {
                Console.WriteLine("_______User email format is invalid__________");
                return false;
            }
            else if (user.Password == null || user.Password.Length < 4)
            {
                Console.WriteLine("_______User password must be at least 4 characters long__________");
                return false;
            }
            else if (string.IsNullOrWhiteSpace(user.Gender))
            {
                Console.WriteLine("_______User gender null or empty__________");
                return false;
            }
            else if (user.Age <= 0)
            {
                Console.WriteLine("_______User age must be greater than 0______________");
                return false;
            }
            else if (user.DOB == DateTime.MinValue)
            {
                Console.WriteLine("_______User date of birth is not set__________");
                return false;
            }
            else if (!IsValidPhoneNumber(user.Phone))
            {
                Console.WriteLine("_______User phone number format is invalid__________");
                return false;
            }
            return true;
        }

        // Email validation method using regex
        public static bool IsValidEmail(string email)
        {
            // Simple regex for validating an email address
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        // Phone number validation method using regex
        public static bool IsValidPhoneNumber(string phone)
        {
            // Example pattern for US phone number validation (adjust as necessary)
            var phonePattern = @"^\+?[1-9]\d{1,14}$"; // E.164 format
            return Regex.IsMatch(phone, phonePattern);
        }
    }
}
