using System;
using System.Security.Cryptography;
using System.Text;
using Julas.Utils;
using Julas.Utils.Extensions;
using Npgsql;
using NpgsqlTypes;
using Common;
using WebApiClient;
using WebApiClient.Interfaces;
using WebApiClient.Models.Account;

namespace WebApiServer.Modules
{
    public class Account : ModuleBase, IAccount
    {
        private readonly ISessionCache _cache;
        private readonly NpgsqlConnection _connection;

        public Account(ISessionCache cache, NpgsqlConnection connection)
        {
            _cache = cache;
            _connection = connection;
        }

        public CreateAccountResponse CreateAccount(CreateAccountRequest request)
        {
            var sha = new SHA512Managed();
            string passwordHash = EncodeBase64(sha.ComputeHash(request.Password.EncodeUTF8()));

            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO users (username, password_hash) VALUES (@username, @password_hash);";
                cmd.Parameters.AddWithValue("@username", NpgsqlDbType.Text, request.UserID?.ToLower().IfNullThenDBNull());
                cmd.Parameters.AddWithValue("@password_hash", NpgsqlDbType.Text, passwordHash);
                cmd.ExecuteNonQuery();
            }
            return new CreateAccountResponse()
            {
                Message = $"A new account for \"{request?.UserID?.ToLower()}\" was created."
            };
        }

        public LoginResponse Login(LoginRequest request)
        {
            var sha = new SHA512Managed();
            string passwordHash = EncodeBase64(sha.ComputeHash(request.Password.EncodeUTF8()));
            string dbHash = null;
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT password_hash FROM users WHERE username = @userName";
                cmd.Parameters.AddWithValue("@userName", NpgsqlDbType.Text, request?.UserID?.ToLower().IfNullThenDBNull());
                dbHash = (string)cmd.ExecuteScalar();
            }
            if (dbHash != passwordHash)
            {
                throw new WebApiException(401, "Invalid username or password.");
            }
            var session = _cache.CreateSession(request.UserID.ToLower());
            return new LoginResponse()
            {
                SessionID = session.SessionID
            };
        }

        public LogoutResponse Logout(LogoutRequest request)
        {
            _cache.CloseSession(request.SessionID);
            return new LogoutResponse();
        }

        public ChangePasswordResponse ChangePassword(ChangePasswordRequest request)
        {
            VerifySession(request.UserID, request.SessionID);
            var sha = new SHA512Managed();
            var oldPasswordHash = EncodeBase64(sha.ComputeHash(request.OldPassword.EncodeUTF8()));
            var newPasswordHash = EncodeBase64(sha.ComputeHash(request.NewPassword.EncodeUTF8()));
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE users SET password_hash = @newPasswordHash WHERE username = @userName AND password_hash = @oldPasswordHash;";
                cmd.Parameters.AddWithValue("@userName", NpgsqlDbType.Text, request?.UserID?.ToLower().IfNullThenDBNull());
                cmd.Parameters.AddWithValue("@oldPasswordHash", NpgsqlDbType.Text, oldPasswordHash);
                cmd.Parameters.AddWithValue("@newPasswordHash", NpgsqlDbType.Text, newPasswordHash);
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new WebApiException(401, "Invalid username or password.");
                }
                else
                {
                    return new ChangePasswordResponse()
                    {
                        
                    };
                }
            }
        }

        private void VerifySession(string userID, string sessionID)
        {
            if (!_cache.VerifySession(userID?.ToLower(), sessionID))
            {
                throw new WebApiException(401, "Invalid session.");
            }
        }

        private string EncodeBase64(byte[] data)
        {
            return Convert.ToBase64String(data);
        }
    }
}
