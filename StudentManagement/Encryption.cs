using System.Text;
using System.Security.Cryptography;

namespace StudentManagement
{
    class Encryption
    {
        Database database = new Database();
        private const string salt = "|S|;q&S5o26-~xz!!++EYT%s)98foVe%FX[G1-.*DY]LMPSrjqRB7/gK:&LT<T{U";
        public string Encrypt(string password)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            SHA256Managed SHA256Hasher = new SHA256Managed();
            byte[] hashedDataBytes = SHA256Hasher.ComputeHash(encoder.GetBytes(password));
            StringBuilder output = new StringBuilder();
            for (int i = 0; i < hashedDataBytes.Length; i++)
            {
                output.Append(hashedDataBytes[i].ToString("x2"));
            }
            return output.ToString();
        }

        public bool Verify(string username, string password)
        {
            string verifiedPassword = Encrypt(password);
            return database.GetLoginData(username, verifiedPassword);
        }
    }
}
